"""精灵图 → YOLO 合成数据集 + 训练。

读取 datasets/raw/ 下的怪物 PNG 精灵图（带透明通道），
自动贴到模拟游戏背景上生成训练数据，然后训练 YOLO。

用法：
  python tools/train_from_sprites.py
  python tools/train_from_sprites.py --epochs 100 --samples 500
"""
import argparse
import os
import random
import sys
from dataclasses import dataclass, field

import cv2
import numpy as np

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

# 模拟游戏画面尺寸（790px 参考宽度）
PF_W, PF_H = 790, 520
# 每个 sprite 的随机缩放范围
MIN_SCALE, MAX_SCALE = 0.7, 1.3
# 每张图放几只怪
MIN_MOBS, MAX_MOBS = 1, 6
# 底部留白区域（地面层）
GROUND_Y_RATIO = 0.65  # 地面线位置


@dataclass
class Sprite:
    """解析好的精灵图：非透明区域的裁剪图 + 宽高。"""
    name: str          # 原文件名（不含后缀）
    img: np.ndarray    # BGRA 裁剪图（已去掉透明边缘）
    ox: int            # 原始图中非透明区域的 left
    oy: int
    w: int             # 裁剪后宽度
    h: int             # 裁剪后高度


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
            name=fname[:-4],
            img=crop,
            ox=x1, oy=y1,
            w=crop.shape[1], h=crop.shape[0],
        ))
    return sprites


def _make_background() -> np.ndarray:
    """生成模拟枫谷游戏背景：深色渐变 + 地面 + 随机噪点。"""
    bg = np.zeros((PF_H, PF_W, 3), dtype=np.uint8)
    # 天空渐变（深蓝黑 → 深灰）
    for y in range(PF_H):
        t = y / PF_H
        r = int(20 + 10 * (1 - t))
        g = int(20 + 15 * (1 - t))
        b = int(35 + 10 * (1 - t))
        cv2.line(bg, (0, y), (PF_W, y), (b, g, r), 1)
    # 地面（深棕色条）
    ground_y = int(PF_H * GROUND_Y_RATIO)
    cv2.rectangle(bg, (0, ground_y), (PF_W, PF_H), (30, 25, 20), -1)
    # 随机噪点
    noise = np.random.randint(0, 8, (PF_H, PF_W, 3), dtype=np.uint8)
    bg = cv2.add(bg, noise)
    return bg


def _paste_sprite(bg: np.ndarray, sprite: Sprite, x: int, y: int, scale: float) -> None:
    """把精灵图贴到背景上，处理透明通道。"""
    h, w = sprite.h, sprite.w
    if scale != 1.0:
        new_w = max(int(w * scale), 4)
        new_h = max(int(h * scale), 4)
        img = cv2.resize(sprite.img, (new_w, new_h), interpolation=cv2.INTER_AREA)
    else:
        img = sprite.img
        new_w, new_h = w, h

    h2, w2 = img.shape[:2]
    x1, y1 = max(x, 0), max(y, 0)
    x2, y2 = min(x + w2, PF_W), min(y + h2, PF_H)
    if x1 >= x2 or y1 >= y2:
        return

    roi = bg[y1:y2, x1:x2]
    spr = img[y1 - y:y2 - y, x1 - x:x2 - x]
    # 透明混合
    alpha = spr[:, :, 3].astype(np.float32) / 255.0
    fg = spr[:, :, :3].astype(np.float32)
    bg_roi = roi.astype(np.float32)
    blended = fg * alpha[:, :, None] + bg_roi * (1 - alpha[:, :, None])
    bg[y1:y2, x1:x2] = blended.astype(np.uint8)


def generate_dataset(sprites: list, out_dir: str, samples: int, val_ratio: float):
    """生成合成数据集，返回 (train_dir, val_dir, class_names)。"""
    train_dir = os.path.join(out_dir, "train", "images")
    val_dir = os.path.join(out_dir, "val", "images")
    train_label = os.path.join(out_dir, "train", "labels")
    val_label = os.path.join(out_dir, "val", "labels")
    for d in [train_dir, val_dir, train_label, val_label]:
        os.makedirs(d, exist_ok=True)

    # 生成样本
    val_count = max(1, int(samples * val_ratio))
    train_count = samples - val_count
    indices = list(range(samples))
    random.shuffle(indices)
    val_set = set(indices[:val_count])

    for i in range(samples):
        is_val = i in val_set
        bg = _make_background()
        n_mobs = random.randint(MIN_MOBS, MAX_MOBS)
        labels = []
        placed = []

        for _ in range(n_mobs):
            sp = random.choice(sprites)
            scale = random.uniform(MIN_SCALE, MAX_SCALE)
            sw, sh = int(sp.w * scale), int(sp.h * scale)
            if sw < 4 or sh < 4:
                continue

            # 尝试放置（不与已有精灵重叠太多）
            for attempt in range(10):
                margin = 10
                x = random.randint(margin, PF_W - sw - margin)
                y = random.randint(int(PF_H * 0.45), PF_H - sh - margin)
                # 检查重叠
                ok = True
                for px, py, pw, ph in placed:
                    if abs(x - px) < (sw + pw) // 3 and abs(y - py) < (sh + ph) // 3:
                        ok = False
                        break
                if ok:
                    break

            _paste_sprite(bg, sp, x, y, scale)

            # YOLO 格式：class x_center y_center width height (归一化)
            cx = (x + sw / 2) / PF_W
            cy = (y + sh / 2) / PF_H
            nw = sw / PF_W
            nh = sh / PF_H
            # 裁切边界
            cx = max(0.0, min(1.0, cx))
            cy = max(0.0, min(1.0, cy))
            nw = max(0.001, min(2.0, nw))
            nh = max(0.001, min(2.0, nh))
            labels.append(f"0 {cx:.6f} {cy:.6f} {nw:.6f} {nh:.6f}")
            placed.append((x, y, sw, sh))

        # 随机水平翻转（增强）
        if random.random() < 0.5:
            bg = cv2.flip(bg, 1)
            labels = [
                f"0 {1 - float(l.split()[1]):.6f} {l.split()[2]} {l.split()[3]} {l.split()[4]}"
                for l in labels
            ]

        # 写入
        img_name = f"sprite_{i:06d}.png"
        label_name = f"sprite_{i:06d}.txt"
        if is_val:
            cv2.imwrite(os.path.join(val_dir, img_name), bg)
            with open(os.path.join(val_label, label_name), "w") as f:
                f.write("\n".join(labels) + "\n") if labels else f.write("")
        else:
            cv2.imwrite(os.path.join(train_dir, img_name), bg)
            with open(os.path.join(train_label, label_name), "w") as f:
                f.write("\n".join(labels) + "\n") if labels else f.write("")

        if (i + 1) % 100 == 0:
            print(f"  生成 {i + 1}/{samples}")

    return train_dir, val_dir, ["mob"]


def main():
    ap = argparse.ArgumentParser(description="精灵图 → YOLO 合成数据集 + 训练")
    ap.add_argument("--raw", default="datasets/raw", help="精灵图目录")
    ap.add_argument("--out", default="datasets/yolo", help="输出目录")
    ap.add_argument("--samples", type=int, default=600, help="生成多少张训练图")
    ap.add_argument("--val-ratio", type=float, default=0.15, help="验证集比例")
    ap.add_argument("--model", default="yolo11n.pt", help="YOLO 模型")
    ap.add_argument("--epochs", type=int, default=100)
    ap.add_argument("--batch", type=int, default=-1, help="-1 = 自动")
    ap.add_argument("--imgsz", type=int, default=800)
    ap.add_argument("--device", default="0", help="GPU 编号")
    ap.add_argument("--project", default="runs/mobs")
    ap.add_argument("--name", default="mobs")
    args = ap.parse_args()

    print("[1/3] 加载精灵图...")
    sprites = load_sprites(args.raw)
    if not sprites:
        print(f"❌ {args.raw} 下没有找到带透明通道的 PNG 精灵图")
        return 2
    print(f"  加载 {len(sprites)} 个精灵图")

    print(f"[2/3] 生成合成数据集 ({args.samples} 张)...")
    train_dir, val_dir, classes = generate_dataset(
        sprites, args.out, args.samples, args.val_ratio)
    print(f"  train: {train_dir}")
    print(f"  val:   {val_dir}")
    print(f"  类别:  {classes}")

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

    print(f"[3/3] 训练 YOLO ({args.model}, {args.epochs} epochs)...")
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
        # 游戏画面特化参数
        hsv_h=0.02,      # 色相微调
        hsv_s=0.3,       # 饱和度微调
        hsv_v=0.3,       # 明度微调
        degrees=0.0,     # 不旋转（2D 横版卷轴）
        translate=0.1,   # 轻微平移
        scale=0.3,       # 缩放增强
        shear=0.0,       # 不剪切
        perspective=0.0, # 不透视
        flipud=0.0,      # 不上下翻转
        fliplr=0.5,      # 50% 左右翻转（怪会转向）
        mosaic=0.5,      # mosaic 增强
        mixup=0.1,       # mixup 增强
        cos_lr=True,
        warmup_epochs=3,
        patience=20,     # early stopping
    )

    best = str(results.save_dir / "weights" / "best.pt")
    print(f"\n=== 训练完成 ===")
    print(f"最佳权重: {best}")

    # 转 ONNX
    print("\n转 ONNX...")
    onnx_path = str(model.export(format="onnx", imgsz=args.imgsz, half=False))
    print(f"ONNX: {onnx_path}")

    print("\n=== 部署到游戏机 ===")
    print(f"1. 复制 {onnx_path} 到游戏机")
    print("2. 游戏机装: pip install onnxruntime")
    print("3. config/local.yaml 加:")
    print("  vision:")
    print("    mob_detector: yolo")
    print(f"    yolo_model: {onnx_path}")
    print("    yolo_confidence: 0.5")
    print("\n验证: python tools/debug_view.py --snapshot check.png")
    return 0


if __name__ == "__main__":
    sys.exit(main())