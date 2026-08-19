# TileFall 踏格迷阵

> 一款基于 **Unity 2022.3 LTS + URP** 开发的 3D 记忆/反应类休闲小游戏，已发布到 itch.io 在线试玩。

## 🎮 游戏简介

玩家需要在一张 8×8 的方阵上，观察随机闪烁的符号提示，在倒计时内站到**与提示符号一致的格子**上；猜错的格子会塌陷，掉入深渊即游戏结束。每回合正确概率逐渐降低、倒计时越来越短，考验记忆与反应。

## 🕹️ 玩法

1. 回合开始：所有格子随机闪烁，屏幕上方显示目标符号（× ● ■ ▲）
2. 用 **WASD / 方向键** 控制角色移动，站到目标符号的格子上
3. 倒计时结束：错误符号的格子塌陷，正确格子高亮
4. 存活所有回合即挑战更高难度；踩空掉落即游戏结束

## ✨ 技术亮点

- **Unity 2022.3 LTS + URP**：平衡质量档位，8×8 动态网格 + TMP 符号渲染
- **WebGL 部署性能优化**（针对 itch.io）：
  - 关闭 SSAO / Bloom / Tonemapping 等高开销后处理，锁定 60 FPS
  - `devicePixelRatio` 限制为 1，避免高分屏 4 倍像素渲染
  - 缓存 `Enum.GetValues`、倒计时 UI 每秒仅更新一次，降低 GC 压力
  - 压缩格式 Disabled，兼容 itch.io CDN 透明压缩
- **一键构建**：`Assets/Editor/BuildWebGL.cs` 提供菜单 `Build → WebGL (itch)`，自动生成可直接上传 itch 的 Gzip/无压缩产物

## 🔗 在线试玩

<!-- itch 链接待补充 -->
- itch.io：<占位，上传后补充链接>

## 📂 项目结构

```
Assets/
  Script/          # 游戏核心脚本（GameManager / Tile / PlayerMovement / Walls / AudioManager ...）
  Settings/        # URP 渲染管线配置（Performant / Balanced / High Fidelity）
  Scenes/          # Start（开始界面）、MainGame（主游戏）
  Editor/          # 一键 WebGL 构建脚本
  TextMesh Pro/    # TMP 资源
ProjectSettings/   # 项目与质量设置
Packages/          # 依赖包清单
```

## 🚀 本地运行

1. 使用 **Unity Hub** 打开项目（要求 Unity 2022.3 LTS 及以上）
2. 打开场景 `Assets/Scenes/Start.unity`，点击 Play

## 🛠️ 技术栈

Unity 2022.3 LTS · URP 14 · C# · WebGL · TextMesh Pro
