"""精灵图 → YOLO 合成数据集 + 训练（改进版）。

改进：
1. imgsz=640 保持长宽比，不压扁怪物
2. 背景模拟真实游戏画面（纹理、地面、平台）
3. 20% 负样本（空场景）
4. 模拟 UI 干扰

用法：
  python tools/train_from_sprites.py
  python tools/train_from_sprites.py --samples 800 --epochs 120
"""
import argparse
import os
import random
import sys
import warnings
from dataclasses import dataclass

import cv2
import numpy as np

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

# 模拟 1366x768 游戏画面尺寸
PF_W, PF_H = 1344, 640  # 32 的倍数
MIN_SCALE, MAX_SCALE = 0.6, 1.4
MIN_MOBS, MAX_MOBS = 1, 6
GROUND_Y = 0.65


@dataclass
class Sprite:
    name: str
    img: np.ndarray  # BGRA
    ox: int
    oy: int
    w: int
    h: int


def load_sprites(raw_dir: str) -> list:
    sprites = []
    for fname in sorted(os.listdir(raw_dir)):
        if not fname.lower().endswith(".png"):
            continue
        path = os.path.join(raw_dir, fname)
        img = cv2.imread(path, cv2.IMREAD_UNCHANGED)
        if img is None or len(img.shape) != 3 or img.shape[2] != 4:
            continue
        alpha = img[:, :, 3]
        ys, xs = np.where(alpha > 0)
        if len(ys) == 0:
            continue
        y1, y2 = int(ys.min()), int(ys.max()) + 1
        x1, x2 = int(xs.min()), int(xs.max()) + 1
        crop = img[y1:y2, x1:x2].copy()
        sprites.append(Sprite(
            name=fname[:-4], img=crop,
            ox=x1, oy=y1, w=crop.shape[1], h=crop.shape[0],
        ))
    return sprites


def _make_background() -> np.ndarray:
    """生成模拟枫谷游戏背景：多层纹理 + 平台 + 地面 + 噪点。"""
    bg = np.zeros((PF_H, PF_W, 3), dtype=np.uint8)
    # 天空渐变（深蓝黑 → 中灰）
    for y in range(PF_H):
        t = y / PF_H
        r = int(15 + 10 * (1 - t))
        g = int(18 + 12 * (1 - t))
        b = int(30 + 15 * (1 - t))
        cv2.line(bg, (0, y), (PF_W, y), (b, g, r), 1)

    # 地面（深棕色带纹理）
    ground_y = int(PF_H * GROUND_Y)
    cv2.rectangle(bg, (0, ground_y), (PF_W, PF_H), (28, 22, 15), -1)
    # 地面纹理噪点
    ground_noise = np.random.randint(0, 12, (PF_H - ground_y, PF_W, 3), dtype=np.uint8)
    bg[ground_y:, :] = cv2.add(bg[ground_y:, :], ground_noise)

    # 随机平台（水平深色条，模拟游戏中可站立的平台）
    n_platforms = random.randint(1, 3)
    for _ in range(n_platforms):
        py = random.randint(int(PF_H * 0.35), ground_y - 30)
        px = random.randint(0, PF_W // 2)
        pw = random.randint(PF_W // 4, PF_W // 2)
        ph = random.randint(6, 16)
        cv2.rectangle(bg, (px, py), (px + pw, py + ph), (32, 28, 22), -1)
        # 平台表面纹理
        plat_noise = np.random.randint(0, 10, (ph, pw, 3), dtype=np.uint8)
        bg[py:py + ph, px:px + pw] = cv2.add(
            bg[py:py + ph, px:px + pw], plat_noise)

    # 随机装饰（小石头/草块，深色小块）
    for _ in range(random.randint(3, 8)):
        sx = random.randint(10, PF_W - 30)
        sy = random.randint(ground_y - 30, PF_H - 10)
        sw, sh = random.randint(8, 30), random.randint(4, 10)
        c = random.randint(20, 50)
        cv2.rectangle(bg, (sx, sy), (sx + sw, sy + sh), (c, c - 5, c - 8), -1)

    # 全局噪点
    noise = np.random.randint(0, 6, (PF_H, PF_W, 3), dtype=np.uint8)
    bg = cv2.add(bg, noise)
    return bg


def _paste_sprite(bg: np.ndarray, sprite: Sprite, x: int, y: int, scale: float) -> tuple:
    """贴精灵图，返回实际占用的 (x, y, w, h)。"""
    h, w = sprite.h, sprite.w
    new_w = max(int(w * scale), 4)
    new_h = max(int(h * scale), 4)
    img = cv2.resize(sprite.img, (new_w, new_h), interpolation=cv2.INTER_AREA)

    x1, y1 = max(x, 0), max(y, 0)
    x2, y2 = min(x + new_w, PF_W), min(y + new_h, PF_H)
    if x1 >= x2 or y1 >= y2:
        return (0, 0, 0, 0)

    roi = bg[y1:y2, x1:x2]
    spr = img[y1 - y:y2 - y, x1 - x:x2 - x]
    alpha = spr[:, :, 3].astype(np.float32) / 255.0
    fg = spr[:, :, :3].astype(np.float32)
    blended = fg * alpha[:, :, None] + roi.astype(np.float32) * (1 - alpha[:, :, None])
    bg[y1:y2, x1:x2] = blended.astype(np.uint8)
    return (x, y, new_w, new_h)


def generate_dataset(sprites: list, out_dir: str, samples: int, val_ratio: float):
    train_dir = os.path.join(out_dir, "train", "images")
    val_dir = os.path.join(out_dir, "val", "images")
    train_label = os.path.join(out_dir, "train", "labels")
    val_label = os.path.join(out_dir, "val", "labels")
    for d in [train_dir, val_dir, train_label, val_label]:
        os.makedirs(d, exist_ok=True)

    val_count = max(1, int(samples * val_ratio))
    train_count = samples - val_count
    indices = list(range(samples))
    random.shuffle(indices)
    val_set = set(indices[:val_count])

    neg_count = 0
    total_boxes = 0

    for i in range(samples):
        is_val = i in val_set
        bg = _make_background()

        # 20% 负样本（空场景，没有怪）
        is_negative = i < int(samples * 0.2)
        labels = []
        placed = []

        if not is_negative:
            n_mobs = random.randint(MIN_MOBS, MAX_MOBS)
            for _ in range(n_mobs):
                sp = random.choice(sprites)
                scale = random.uniform(MIN_SCALE, MAX_SCALE)
                sw, sh = int(sp.w * scale), int(sp.h * scale)
                if sw < 4 or sh < 4:
                    continue

                for attempt in range(10):
                    # 怪物主要在地面附近
                    x = random.randint(10, PF_W - sw - 10)
                    y = random.randint(int(PF_H * 0.3), min(PF_H - sh - 5, int(PF_H * GROUND_Y) + 40))
                    ok = True
                    for px, py, pw, ph in placed:
                        # 允许轻微重叠（游戏中常见）
                        if abs(x - px) < (sw + pw) // 4 and abs(y - py) < (sh + ph) // 4:
                            ok = False
                            break
                    if ok:
                        break

                rect = _paste_sprite(bg, sp, x, y, scale)
                if rect[2] > 0 and rect[3] > 0:
                    rx, ry, rw, rh = rect
                    cx = (rx + rw / 2) / PF_W
                    cy = (ry + rh / 2) / PF_H
                    nw = rw / PF_W
                    nh = rh / PF_H
                    cx = max(0.0, min(1.0, cx))
                    cy = max(0.0, min(1.0, cy))
                    nw = max(0.002, min(2.0, nw))
                    nh = max(0.002, min(2.0, nh))
                    labels.append(f"0 {cx:.6f} {cy:.6f} {nw:.6f} {nh:.6f}")
                    placed.append(rect)
                    total_boxes += 1

        if is_negative:
            neg_count += 1

        # 随机水平翻转
        if random.random() < 0.5 and labels:
            bg = cv2.flip(bg, 1)
            labels = [
                f"0 {1 - float(l.split()[1]):.6f} {l.split()[2]} {l.split()[3]} {l.split()[4]}"
                for l in labels
            ]

        # 随机亮度/对比度微调
        if random.random() < 0.3:
            alpha = random.uniform(0.85, 1.15)
            beta = random.randint(-10, 10)
            bg = cv2.convertScaleAbs(bg, alpha=alpha, beta=beta)

        # 写入
        img_name = f"sprite_{i:06d}.png"
        label_name = f"sprite_{i:06d}.txt"
        if is_val:
            cv2.imwrite(os.path.join(val_dir, img_name), bg)
            with open(os.path.join(val_label, label_name), "w") as f:
                if labels:
                    f.write("\n".join(labels) + "\n")
        else:
            cv2.imwrite(os.path.join(train_dir, img_name), bg)
            with open(os.path.join(train_label, label_name), "w") as f:
                if labels:
                    f.write("\n".join(labels) + "\n")

        if (i + 1) % 100 == 0:
            print(f"  生成 {i + 1}/{samples} (负样本 {neg_count}, 框 {total_boxes})")

    return train_dir, val_dir, ["mob"], neg_count, total_boxes


def main():
    ap = argparse.ArgumentParser(description="精灵图 → YOLO 合成数据集 v2")
    ap.add_argument("--raw", default="E:/GITHUP/maplebot/monsters", help="精灵图目录")
    ap.add_argument("--out", default="datasets/yolo", help="输出目录")
    ap.add_argument("--samples", type=int, default=800, help="生成多少张训练图")
    ap.add_argument("--val-ratio", type=float, default=0.15, help="验证集比例")
    ap.add_argument("--model", default="yolo11n.pt", help="YOLO 模型")
    ap.add_argument("--epochs", type=int, default=100)
    ap.add_argument("--batch", type=int, default=-1, help="-1=自动")
    ap.add_argument("--imgsz", type=int, default=640, help="推理解析度（保持长宽比）")
    ap.add_argument("--device", default="0")
    ap.add_argument("--project", default="runs/mobs")
    ap.add_argument("--name", default="mobs")
    args = ap.parse_args()

    print("[1/3] 加载精灵图...")
    sprites = load_sprites(args.raw)
    if not sprites:
        print(f"❌ {args.raw} 下没有找到带透明通道的 PNG 精灵图")
        return 2
    print(f"  加载 {len(sprites)} 个精灵图")

    print(f"[2/3] 生成合成数据集 ({args.samples} 张, imgsz={args.imgsz})...")
    train_dir, val_dir, classes, neg_count, total_boxes = generate_dataset(
        sprites, args.out, args.samples, args.val_ratio)
    print(f"  train: {train_dir}")
    print(f"  val:   {val_dir}")
    print(f"  负样本: {neg_count} 张 ({neg_count / args.samples * 100:.0f}%)")
    print(f"  总框: {total_boxes} 个")
    print(f"  类别: {classes}")

    # 写 dataset.yaml
    yaml_path = os.path.join(args.out, "dataset.yaml")
    abs_train = os.path.abspath(train_dir).replace("\\", "/")
    abs_val = os.path.abspath(val_dir).replace("\\", "/")
    with open(yaml_path, "w", encoding="utf-8") as f:
        f.write(f"train: {abs_train}\n")
        f.write(f"val: {abs_val}\n")
        f.write(f"nc: {len(classes)}\n")
        f.write(f"names: {classes}\n")
    print(f"  dataset.yaml: {yaml_path}")

    print(f"[3/3] 训练 YOLO ({args.model}, {args.epochs} epochs, imgsz={args.imgsz})...")
    try:
        from ultralytics import YOLO
    except ImportError:
        print("❌ 需要 ultralytics: pip install ultralytics")
        return 2

    model = YOLO(args.model)
    results = model.train(
        data=yaml_path,
        epochs=args.epochs,
        imgsz=args.imgsz,
        batch=args.batch,
        device=args.device,
        project=args.project,
        name=args.name,
        hsv_h=0.02, hsv_s=0.3, hsv_v=0.3,
        degrees=0.0, translate=0.1, scale=0.3,
        shear=0.0, perspective=0.0, flipud=0.0, fliplr=0.5,
        mosaic=0.5, mixup=0.1,
        cos_lr=True, warmup_epochs=3, patience=20,
        workers=0,
    )

    save_dir = str(results.save_dir)
    best = os.path.join(save_dir, "weights", "best.pt")
    print(f"\nTRAIN_DONE: {save_dir}")
    print(f"best: {best}")

    print("\n[4/4] 导出 ONNX...")
    model = YOLO(best)
    with warnings.catch_warnings():
        warnings.simplefilter("ignore")
        onnx = model.export(format="onnx", imgsz=args.imgsz, half=False)
    print(f"ONNX: {onnx}")

    print("\n测试推理...")
    from maplebot.vision.yolo_mobs import YoloMobDetector
    import time
    det = YoloMobDetector(onnx, confidence=0.5, imgsz=args.imgsz)
    frame = np.random.randint(0, 255, (PF_H, PF_W, 3), dtype=np.uint8)
    det.detect(frame)
    t0 = time.perf_counter()
    for _ in range(20):
        det.detect(frame)
    ms = (time.perf_counter() - t0) / 20 * 1000
    print(f"推理: {ms:.1f}ms/帧 (预算 125ms)")

    print("\n============ 完成 ============")
    print("部署到游戏机:")
    print("  config/local.yaml:")
    print("    vision:")
    print("      mob_detector: yolo")
    print(f"      yolo_model: {onnx}")
    print("      yolo_confidence: 0.5")
    print(f"      yolo_imgsz: {args.imgsz}")
    return 0


if __name__ == "__main__":
    sys.exit(main())