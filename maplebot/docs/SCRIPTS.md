# maplebot 可独立运行脚本全景文档

> 生成日期：2026-08-14 | 项目版本：v2.0.0

本文档系统化梳理 maplebot 项目下所有可独立运行的 Python 脚本，按使用阶段分类，
每个脚本包含用途、适用环境、前置条件、用法示例、输入输出和关联脚本。

---

## 1. 总览表

| 脚本 | 位置 | 运行环境 | 阶段 | 用途 |
|------|------|----------|------|------|
| `main.py` | 根目录 | 游戏机 | 运行 | CLI 模式启动 bot |
| `gui.py` | 根目录 | 游戏机 | 运行 | tkinter 图形界面启动 bot |
| `tools/doctor.py` | tools/ | 游戏机 | 诊断 | 环境自检：套件/配置/ROI 一致性 |
| `tools/test_keys.py` | tools/ | 游戏机 | 诊断 | 按鍵诊断：确认 SendInput 是否送进游戏 |
| `tools/check_remote.py` | tools/ | 游戏机 | 诊断 | 远端推理服务器连通性检查 |
| `tools/calibrate.py` | tools/ | 游戏机 | 校正 | 框选 5 个 ROI 区域，写入 config |
| `tools/debug_view.py` | tools/ | 游戏机 | 调试 | 可视化识别结果，实时/快照模式 |
| `tools/record_route.py` | tools/ | 游戏机 | 录制 | 录制巡邏路线，生成 waypoints |
| `tools/grab_template.py` | tools/ | 游戏机 | 模板 | 截取怪物/UI 模板图 |
| `tools/collect_dataset.py` | tools/ | 游戏机 | 数据 | 定时采集游戏截图作为训练数据 |
| `tools/auto_pipeline.py` | tools/ | 训练机 | 标注+训练 | 一键：自动标注→切分→训练 |
| `tools/autolabel.py` | tools/ | 训练机 | 标注 | 自动标注（老师模式），输出 YOLO 标签 |
| `tools/autolabel_outline.py` | tools/ | 训练机 | 标注 | 描边+模板联合标注，支持 UI 排除 |
| `tools/label_gdino.py` | tools/ | 训练机 | 标注 | GroundingDINO 大模型自动标注 |
| `tools/prepare_dataset.py` | tools/ | 训练机 | 数据 | 切分 train/val，生成 dataset.yaml |
| `tools/train_yolo.py` | tools/ | 训练机 | 训练 | 训练 YOLO（已有标注数据时） |
| `tools/train_from_sprites.py` | tools/ | 训练机 | 训练 | 精灵图合成数据集+训练 |
| `tools/export_onnx.py` | tools/ | 训练机 | 导出 | .pt → .onnx（推理去 PyTorch 依赖） |
| `tools/serve_yolo.py` | tools/ | 训练机 | 推理 | YOLO 远端推理服务器 |

---

## 2. 核心入口

### 2.1 `main.py` — CLI 模式启动

**用途**：从命令行启动 bot 主循环。

**运行环境**：游戏机（需要游戏窗口在运行）

**前置条件**：
- 已完成 ROI 校正（`tools/calibrate.py`）
- 已配置 profile（`config/profiles/*.yaml`）
- 已通过自检（`tools/debug_view.py --snapshot`）

**用法**：
```bash
# 预演模式（不送出按键，只打印决策）
python main.py --profile config/profiles/example.yaml --dry-run

# 从静态截图运行（离线程开发）
python main.py --source shot.png --max-ticks 20

# 正式运行
python main.py --profile config/profiles/example.yaml
```

**输入**：`config/default.yaml` + `config/local.yaml` + `config/profiles/<name>.yaml`

**输出**：日志到 `logs/run_*.log`，报告到 `logs/reports/`

**关联脚本**：`gui.py`（图形界面版）、`tools/calibrate.py`（上游）、`tools/debug_view.py`（上游）

---

### 2.2 `gui.py` — 图形界面启动

**用途**：tkinter 控制台，集成状态监控、配置编辑、路线录制、ROI 校正。

**运行环境**：游戏机

**前置条件**：同 `main.py`

**用法**：
```bash
python gui.py
```

**功能**：
- 状态面板：HP/MP/EXP 进度条，当前决策，怪物/玩家数
- 控制面板：开始/停止/暂停，profile 切换，预演开关
- 设定面板：攻击键、技能、药水、buff 循环按键
- 录制面板：一键录制巡邏路线，自动填入 waypoints
- 系统日志：实时滚动显示

**输入/输出**：同 `main.py`，但通过 GUI 编辑并保存到 `config/local.yaml`

**关联脚本**：`main.py`（CLI 版）、`tools/calibrate.py`（内置校正按钮）

---

## 3. 环境诊断

### 3.1 `tools/doctor.py` — 环境自检

**用途**：开跑前一次性检查所有"一定会失败"的配置问题。

**运行环境**：游戏机

**前置条件**：已安装依赖

**用法**：
```bash
python tools/doctor.py
python tools/doctor.py --profile config/profiles/multilevel.yaml
```

**检查项**：
- Python 版本与依赖包
- 配置文件存在性与格式
- ROI 区域是否超出视窗范围
- 血条 ROI 是否被包含在 playfield 内
- `potions.hp.below_ratio` 是否低于 `critical_hp_ratio`
- `calibrated_for` 尺寸是否与当前视窗匹配

**输出**：每个检查项 ✅/❌ 状态，有 ❌ 时 exit code = 1

**关联脚本**：`tools/calibrate.py`（修复 ROI 问题）、`main.py`（下一步）

---

### 3.2 `tools/test_keys.py` — 按键诊断

**用途**：诊断"log 显示按键但游戏没反应"的三大原因。

**运行环境**：游戏机（需要游戏窗口在运行）

**前置条件**：游戏已打开

**用法**：
```bash
# 默认测试方向键右
python tools/test_keys.py

# 指定按键和次数
python tools/test_keys.py --key x --times 3
```

**检查项**：
1. 游戏是否以管理员运行（UIPI 挡掉 SendInput）
2. 游戏窗口是否在前景（按键打到了别的程序）
3. SendInput 实际返回状态

**输出**：诊断报告，指出具体问题和修复方法

**关联脚本**：`main.py`（修复后运行）

---

### 3.3 `tools/check_remote.py` — 远端推理连通性检查

**用途**：在游戏机上确认推理服务器可用。

**运行环境**：游戏机（需要推理服务器在线）

**前置条件**：推理服务器已启动（`tools/serve_yolo.py`）

**用法**：
```bash
# 使用 config 中的 remote_endpoint
python tools/check_remote.py

# 指定端点
python tools/check_remote.py --endpoint http://192.168.1.50:8100/detect

# 用静态图测试（不用开游戏）
python tools/check_remote.py --source shot.png
```

**检查项**：/health 端点、推理延迟、检测结果

**输出**：连通性报告 + 延迟统计

**关联脚本**：`tools/serve_yolo.py`（服务器端）

---

## 4. 画面校正

### 4.1 `tools/calibrate.py` — ROI 区域校正

**用途**：框选 5 个关键区域（小地图、HP、MP、EXP、playfield），写入配置。

**运行环境**：游戏机（需要游戏窗口在运行）

**前置条件**：游戏已打开，窗口模式

**用法**：
```bash
# 从游戏窗口抓图，直接写入 config/local.yaml
python tools/calibrate.py --write

# 从静态截图校正
python tools/calibrate.py --source shot.png
```

**框选顺序**：
1. `minimap` — 小地图画布本体（不含标题列）
2. `hp_bar` — 红色 HP 条本体（不含 `HP[...]` 文字）
3. `mp_bar` — 蓝色 MP 条本体
4. `exp_bar` — 经验条
5. `playfield` — 主游戏画面（不含底部 UI 列）

**操作**：拖曳矩形 → Enter 确认 / c 跳过

**输出**：`config/local.yaml` 的 `regions:` 区块 + `calibrated_for` 尺寸

**关联脚本**：`tools/debug_view.py`（验证）、`tools/doctor.py`（检查）、`main.py`（下一步）

---

## 5. 调试预览

### 5.1 `tools/debug_view.py` — 可视化识别结果

**用途**：实时或快照模式查看识别结果，调参利器。

**运行环境**：游戏机

**前置条件**：已完成 ROI 校正

**用法**：
```bash
# 快照模式（推荐，不会拍到调试窗口）
python tools/debug_view.py --snapshot check.png

# 实时窗口（按 q 退出）
python tools/debug_view.py

# 跟踪小地图坐标
python tools/debug_view.py --track
```

**输出可视化**：
- 绿圈 = 玩家在小地图的位置
- 红圈 = 其他玩家
- 黄框 = 检测到的怪物
- 青色框 = 攻击范围
- 洋红十字 = 角色定位点
- 绿框(hpbar) = 怪物血条

**终端输出**：
```
HP 100% | MP 100% | EXP 59%
玩家小地图坐标: (36, 45) | 其他玩家: 0
检测到怪物: 3
当前决策: Attack
```

**关联脚本**：`tools/calibrate.py`（上游）、`main.py`（下一步）

---

## 6. 路线录制

### 6.1 `tools/record_route.py` — 录制巡邏路线

**用途**：自己走一趟路线，自动生成 `patrol.waypoints`。

**运行环境**：游戏机（需要游戏窗口在运行）

**前置条件**：已完成 ROI 校正

**用法**：
```bash
# 交互式录制：按 Enter 开始，Ctrl+C 停止
python tools/record_route.py --profile config/profiles/弓箭手訓練場一.yaml

# 定时录制
python tools/record_route.py --profile ... --seconds 60 --out route.yaml
```

**录制逻辑**：
- 方向键不录进去（由巡邏逻辑自己决定）
- 小地图 y 跳阶的地方自动变成爬绳点
- 站定按的键挂在对应巡邏点上
- 折返点变成端点

**输出**：`patrol.waypoints` YAML 区块，可直接粘贴到 profile

**关联脚本**：`gui.py`（内置录制按钮）、`main.py`（下一步）

---

## 7. 模板采集

### 7.1 `tools/grab_template.py` — 截取模板图

**用途**：框选怪物/UI 元素，存为模板 PNG。

**运行环境**：游戏机

**前置条件**：游戏已打开

**用法**：
```bash
# 截取怪物模板
python tools/grab_template.py --name snail

# 框选 UI 元素（小地图角落）
python tools/grab_template.py --dir data/templates/ui --name minimap_tl

# 从静态截图截取
python tools/grab_template.py --name green_mushroom --source shot.png
```

**输出**：`data/templates/mobs/<name>.png` 或 `data/templates/ui/<name>.png`

**建议**：同一只怪截 2~3 张不同动作帧，框选贴紧身体

**关联脚本**：`tools/calibrate.py`（截取 minimap_tl/br 用于 auto 定位）

---

## 8. 数据集采集

### 8.1 `tools/collect_dataset.py` — 采集游戏截图

**用途**：定时抓取 playfield 画面作为 YOLO 训练数据，自动去重。

**运行环境**：游戏机（需要游戏窗口在运行）

**前置条件**：已完成 ROI 校正

**用法**：
```bash
# 默认：每 2 秒一张，共 300 张 PNG
python tools/collect_dataset.py

# 自定义参数
python tools/collect_dataset.py --interval 2 --count 400

# 省硬盘（会降低标注质量）
python tools/collect_dataset.py --format jpg
```

**采集建议**：
- 边玩边采集，多换位置/地形
- 含怪的各种动作帧、不同数量、重叠
- 含 10~20% 完全没怪的空景（负样本）
- 建议先跟自己组队（利于离线程标注时定位角色）
- 默认 PNG（无压缩，描边标注需要纯黑像素）

**输出**：`datasets/raw/YYYYMMDD_HHMMSS_xxx.png`（约 300 张 ≈ 200MB）

**关联脚本**：`tools/auto_pipeline.py`（下一步）、`tools/train_yolo.py`（下一步）

---

## 9. 自动标注

### 9.1 `tools/auto_pipeline.py` — 一键标注+训练

**用途**：自动标注→切分→训练，全程零人工标注。

**运行环境**：训练机（有 GPU）

**前置条件**：`datasets/raw/` 已采集好截图

**用法**：
```bash
# 先预览标注质量（强烈建议）
python tools/auto_pipeline.py --check

# 正式训练（默认描边老师）
python tools/auto_pipeline.py

# 用模板匹配当老师
python tools/auto_pipeline.py --teacher template

# 自定义参数
python tools/auto_pipeline.py --epochs 120 --model yolo11s.pt
```

**流程**（3 步合一）：
1. 自动标注（描边/模板老师）
2. 切分 train/val
3. 训练 YOLO

**输出**：`runs/mobs/mobs/weights/best.pt` + 训练完打印的配置行

**关联脚本**：`tools/collect_dataset.py`（上游）、`tools/export_onnx.py`（下游）

---

### 9.2 `tools/autolabel.py` — 灵活自动标注

**用途**：比 `auto_pipeline.py` 更灵活，可以只标注不训练。

**运行环境**：训练机

**前置条件**：`datasets/raw/` 已采集

**用法**：
```bash
# 预览标注
python tools/autolabel.py --preview 6

# 描边门檻扫描（调参用）
python tools/autolabel.py --scan-black

# 描边老师，指定门檻
python tools/autolabel.py --black-level 15

# 模板老师
python tools/autolabel.py --teacher template --threshold 0.68
```

**输出**：`datasets/raw/*.txt`（YOLO 格式标签）+ `classes.txt`

**关联脚本**：`tools/prepare_dataset.py`（下一步）、`tools/auto_pipeline.py`（一键版）

---

### 9.3 `tools/autolabel_outline.py` — 描边+模板联合标注

**用途**：两个老师联集（outline ∪ template），盲点互补，支持 UI 排除。

**运行环境**：训练机

**前置条件**：`datasets/raw/` 已采集

**用法**：
```bash
# 联合标注 + UI 排除
python tools/autolabel_outline.py \
  --exclude 0,0,320,400 \
  --exclude 1700,1130,859,182
```

**优势**：outline 对所有怪成立，template 有怪种信息，IoU 合并去重

**`--exclude` 参数**：排除固定位置的 UI（小地图、快捷栏、公告），坐标是 playfield 内的 x,y,w,h

**关联脚本**：`tools/prepare_dataset.py`（下一步）

---

### 9.4 `tools/label_gdino.py` — GroundingDINO 大模型标注

**用途**：用开放词彙检测模型自动标注，只需一句 prompt。

**运行环境**：训练机（有 GPU）

**前置条件**：`pip install transformers pillow`

**用法**：
```bash
# 先测试一张（必须！）
python tools/label_gdino.py --test datasets/raw/xxxx.jpg --prompt "monster"

# 调整门檻
python tools/label_gdino.py --test shot.jpg --prompt "monster" --box-threshold 0.1

# 批次标注
python tools/label_gdino.py --prompt "monster" --box-threshold 0.15

# 自定义排除词
python tools/label_gdino.py --prompt "monster" \
  --reject "npc,person,player character,signboard"
```

**注意**：GroundingDINO 对卡通 sprite 不保证准确，前缀 `--test` 先试一张

**输出**：`datasets/raw/*.txt`（YOLO 格式标签）

**关联脚本**：`tools/prepare_dataset.py`（下一步）

---

## 10. 数据集准备

### 10.1 `tools/prepare_dataset.py` — 切分训练/验证集

**用途**：将标注好的数据切分为 train/val，生成 `dataset.yaml`。

**运行环境**：训练机

**前置条件**：`datasets/raw/` 已标注完成（含 .txt 标签）

**用法**：
```bash
# 默认 15% 验证集
python tools/prepare_dataset.py

# 自定义比例
python tools/prepare_dataset.py --val 0.2 --seed 7
```

**输出**：
- `datasets/yolo/train/` — 训练图片+标签
- `datasets/yolo/val/` — 验证图片+标签
- `datasets/yolo/dataset.yaml` — 训练配置

**关联脚本**：`tools/train_yolo.py`（下一步）

---

## 11. 训练

### 11.1 `tools/train_yolo.py` — 标准 YOLO 训练

**用途**：已有标注数据时，单独重跑训练步骤。

**运行环境**：训练机（有 GPU）

**前置条件**：`datasets/yolo/dataset.yaml` 已存在

**用法**：
```bash
# 默认参数
python tools/train_yolo.py

# 自定义参数
python tools/train_yolo.py --model yolo11s.pt --epochs 120 --batch 16
```

**参数**：
- `--model`：yolo11n.pt（快）/ yolo11s.pt（更准）
- `--epochs`：80~120
- `--batch`：-1 自动，显存不够加 `--batch 8`
- `--imgsz`：800（默认 800px 宽，会取 32 的倍数）

**输出**：`runs/mobs/mobs/weights/best.pt`

**关联脚本**：`tools/prepare_dataset.py`（上游）、`tools/export_onnx.py`（下游）

---

### 11.2 `tools/train_from_sprites.py` — 精灵图合成训练

**用途**：用怪物 PNG 精灵图（带透明通道）合成数据集 + 训练。**实验性**。

**运行环境**：训练机（有 GPU）

**前置条件**：怪物精灵图目录（PNG + 透明通道）

**用法**：
```bash
# 默认参数
python tools/train_from_sprites.py

# 自定义
python tools/train_from_sprites.py --samples 800 --epochs 120 --raw E:/path/to/sprites
```

**工作原理**：
1. 读取精灵图透明通道，裁剪非透明区域
2. 生成模拟背景（多层纹理 + 平台 + 地面 + 噪点）
3. 随机贴精灵图到背景上，自动生成 YOLO 标注
4. 20% 负样本（空场景）
5. 训练 YOLO

**局限性**：合成数据 ≠ 真实画面，模型在真实游戏中的表现可能不如标准流程。

**建议**：优先使用标准流程（`collect_dataset.py` → `auto_pipeline.py`）

**关联脚本**：`tools/export_onnx.py`（下游）

---

## 12. 模型导出

### 12.1 `tools/export_onnx.py` — .pt → .onnx

**用途**：将 PyTorch 模型导出为 ONNX，推理去除 PyTorch 依赖（2GB→30MB）。

**运行环境**：训练机

**前置条件**：已训练好的 `.pt` 模型

**用法**：
```bash
# 基本导出
python tools/export_onnx.py --model runs/mobs/mobs/weights/best.pt

# 带速度测试
python tools/export_onnx.py --model best.pt --benchmark
```

**输出**：`best.onnx`（约 10MB）

**部署到游戏机**：
```bash
# 游戏机上只需
pip install onnxruntime
# 复制 best.onnx 到项目目录
```

**config 配置**：
```yaml
vision:
  mob_detector: yolo
  yolo_model: runs/mobs/mobs/weights/best.onnx
  yolo_confidence: 0.5
```

**关联脚本**：`tools/train_yolo.py`（上游）、`main.py`（下游）

---

## 13. 推理服务

### 13.1 `tools/serve_yolo.py` — YOLO 远端推理服务器

**用途**：在有 GPU 的机器上运行推理服务器，游戏机通过 HTTP 调用。

**运行环境**：训练机/工作站

**前置条件**：已训练好的 YOLO 模型

**用法**：
```bash
# 启动服务
python tools/serve_yolo.py --model runs/mobs/mobs/weights/best.pt

# 指定端口和 GPU
python tools/serve_yolo.py --model best.pt --port 8100 --device 0
```

**端点**：
- `POST /detect?conf=0.5` — 发送 JPEG 图片，返回检测结果
- `GET /health` — 健康检查

**游戏机配置**：
```yaml
vision:
  mob_detector: remote
  remote_endpoint: "http://192.168.1.50:8100/detect"
  remote_timeout: 1.0
```

**注意**：没有身份验证，只适合局域网使用

**关联脚本**：`tools/check_remote.py`（连通性测试）、`tools/export_onnx.py`（模型准备）

---

## 14. 完整工作流指南

### 路径 A：默认模式（outline 零配置，最快上手）

```
[游戏机]
  calibrate.py              → 校正 5 个 ROI
  debug_view.py --snapshot  → 验证识别效果
  main.py --profile ...     → 直接上線
```

**适用场景**：首次使用，不需要额外配置，换地图换怪都能用。

---

### 路径 B：YOLO 真实截图训练（推荐，最稳定）

```
[游戏机]
  calibrate.py              → 校正 ROI
  collect_dataset.py        → 采集 400 张截图（边走边玩）
  ↓ 复制 datasets/raw/ 到训练机

[训练机]
  auto_pipeline.py --check  → 预览标注框
  auto_pipeline.py          → 自动标注+训练
  export_onnx.py            → .pt → .onnx
  ↓ 复制 best.onnx 到游戏机

[游戏机]
  debug_view.py --snapshot  → 验证 YOLO 检测效果
  main.py --profile ...     → 上線
```

**适用场景**：需要最稳定的怪物检测，解决宠物误判。

---

### 路径 C：精灵图合成训练（实验性）

```
[训练机]
  train_from_sprites.py     → 用精灵图合成数据+训练
  export_onnx.py            → .pt → .onnx（脚本已内置）
  ↓ 复制 best.onnx 到游戏机

[游戏机]
  debug_view.py --snapshot  → 验证检测效果
  main.py --profile ...     → 上線
```

**适用场景**：有精灵图但没有真实截图，可快速实验。

**注意**：合成数据 ≠ 真实画面，效果不如路径 B。

---

## 15. 自建临时脚本

以下脚本为本次会话创建的临时文件，不在官方工具链中：

| 脚本 | 用途 | 状态 |
|------|------|------|
| `_check_env.py` | CUDA 环境检查 | 临时，可删除 |
| `_train_yolo.py` | 精灵图合成训练（改进版） | 临时，功能已合入 `tools/train_from_sprites.py` |
| `_check_env.bat` | Windows 批处理环境检查 | 临时，可删除 |
| `_train_yolo.bat` | Windows 批处理训练 | 临时，可删除 |

---

## 参考

- [README.md](../README.md) — 项目总览
- [TUTORIAL.md](TUTORIAL.md) — 手把手教程
- [YOLO_TRAINING.md](YOLO_TRAINING.md) — YOLO 训练详细指南
- [COMPARISON.md](COMPARISON.md) — 与其他开源项目对比
- [CLAUDE.md](../CLAUDE.md) — 架构与开发约定