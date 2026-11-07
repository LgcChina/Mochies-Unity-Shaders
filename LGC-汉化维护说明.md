# Mochie Shaders LGC 中文版维护说明

> 当前版本：`1.78.3`
> 更新日期：`2026-11-07`  
> 上游基线：`MochiesCode/Mochies-Unity-Shaders` 的 `cf00c15`（`v1.78 update`）  
> VPM 包名：`com.lgcchina.mochies-shaders`  
> 本文是 LGC fork 唯一需要长期维护的说明文档。

## 当前状态

- 当前实现直接翻译 Unity 材质检查器源码中的显示字符串，不使用运行时翻译层。
- 只修改 `Mochie/Unity/Editor/` 下的检查器界面文本。
- Shader 实现、贴图、预制件、资源路径和内部标识均不在汉化范围内；与 VPM 安装位置相关的硬编码路径不属于翻译，已单独修正。
- 11 个源码文件已完成中文检查器文本处理：
  - `Mochie/Unity/Editor/StandardEditor.cs`
  - `Mochie/Unity/Editor/WaterEditor.cs`
  - `Mochie/Unity/Editor/UberEditor.cs`
  - `Mochie/Unity/Editor/GlassEditor.cs`
  - `Mochie/Unity/Editor/ParticleEditor.cs`
  - `Mochie/Unity/Editor/ScreenFXEditor.cs`
  - `Mochie/Unity/Editor/UnderwaterEditor.cs`
  - `Mochie/Unity/Editor/LEDEditor.cs`
  - `Mochie/Unity/Editor/Common/Tips.cs`
  - `Mochie/Unity/Editor/Common/MGUI.cs`
  - `Mochie/Unity/Editor/Common/Foldouts.cs`
- 8 个材质检查器页脚已显示 `LGC 中文版 1.78.3`。
- `package.json` 已使用 `legacyFolders` 自动清理旧的 `Assets/Mochie`，避免重复类型、Shader 名称和 GUID 冲突。
- 渐变贴图生成到 `Assets/LGC/MochieShaders/Textures/Ramps`，不再尝试写入只读的 VPM 包目录。
- “恢复默认贴图”改为从 `Packages` 动态查找包内资源，不再依赖 `Assets/Mochie/Unity/Textures`。
- 当前机器未找到可用的 Unity 可执行文件，因此尚未完成 Unity 内的实际界面截图检查。

## 为什么上游更新后中文可能消失

当前汉化直接写在作者源码中，这是最容易维护、也最符合当前需求的方式。但它不是独立翻译层，所以上游修改同一段文本时，Git 合并可能：

- 产生冲突。
- 接受上游英文文本，导致对应位置重新显示英文。
- 成功合并没有冲突，但新增的界面文本仍然是英文。

因此上游更新后不能只看“合并成功”就结束，必须重新检查 11 个文件。无需重新 fork；继续保留当前 fork，并把 `upstream` 的新版本合并进来即可。

## 仓库与分支

本地仓库：

```text
E:\Users\LgcChina\LgcCh\GitHub\Mochies-Unity-Shaders
```

远程：

```text
origin   https://github.com/LgcChina/Mochies-Unity-Shaders.git
upstream https://github.com/MochiesCode/Mochies-Unity-Shaders.git
```

建议每次发布前都提交当前汉化，再给提交打与 `package.json` 相同的 Git 标签，例如 `1.78.3`。这样上游更新出现问题时可以随时回到上一份可用中文版。

## 上游更新流程

1. 先提交或暂存当前修改，确保 `git status --short` 中没有未处理的源码变化。
2. 获取上游更新：

```powershell
git fetch upstream --tags
git log --oneline --decorate HEAD..upstream/master
```

3. 合并上游 `master`：

```powershell
git switch master
git merge upstream/master
```

4. 逐个处理冲突。冲突时以新版代码结构为准，再把英文显示文本换成中文。
5. 即使没有冲突，也要重新检查全部 11 个文件；上游新增的文本不会自动变成中文。
6. 更新所有 `versionLabel` 中的 LGC 包版本，例如：

```csharp
string versionLabel = "v2.14.2 | LGC 中文版 1.78.3";
```

前半段保留作者原来的 Shader 版本，后半段使用本 VPM 包的版本。
7. 完成检查和 Unity 界面验证后，再更新 `package.json`、Git 标签和 GitHub Release。

## 直接翻译规则

只翻译用户实际能在 Unity 检查器里看到的字符串，例如：

- `new GUIContent(...)` 中显示的标签和提示。
- `ShaderProperty(...)`、`TexturePropertySingleLine(...)` 的显示标签。
- `DoFoldout(...)`、`BeginFoldout(...)`、`GetState(...)` 的折叠标题。
- `BoldLabel(...)`、`PropertyButton(...)`、`SimpleButton(...)`。
- `DisplayError(...)`、`DisplayWarning(...)`、`DisplayInfo(...)`、`DisplayText(...)`。
- `GUILayout.Button(...)`、`GUI.Label(...)`、`GUI.Button(...)` 的可见文本。
- 菜单项、通知、帮助框和对话框文本。

常用查找命令：

```powershell
rg -n 'new GUIContent\(|ShaderProperty\(|TexturePropertySingleLine\(|DoWaveFoldout\(|DoFoldout\(|BeginFoldout\(|GetState\(|BoldLabel\(|PropertyButton\(|SimpleButton\(|Display(Error|Warning|Info|Text)\(|GUILayout\.Button\(|GUI\.(Label|Button)\(' Mochie/Unity/Editor --glob '*.cs'
```

同一行可能同时包含可见文本和内部字符串，必须逐个判断，不能整行替换。

### 禁止翻译

- Shader 属性名和关键字，例如 `_MainTex`、`_BlendMode`、`_FLIPBOOK_MODE`。
- `Shader.Find(...)`、`Resources.Load(...)`、`EditorGUIUtility.Load(...)` 的参数。
- `Assets/...` 路径、URL、文件名、GUID 和资源名。
- `RenderType`、`Opaque`、`Transparent`、`ShadowCaster` 等内部 Tag。
- `DoFooter(versionLabel, "Water-Shader")` 的第二个参数。
- `new[] { "Occlusion", "Roughness", "Metallic" }` 等内部通道名。
- `FindProperty(...)`、`HasProperty(...)`、`GetFloat(...)`、`SetFloat(...)` 等 API 中的属性名。
- `GUIStyle("button")` 等 Unity 样式名。样式名本身必须保留英文，但同一行其它按钮文本可以翻译。
- 作者名、支持者姓名、许可证文本和上游 README。
- `Mochie/Unity/Editor/Tools/`、所有 `.shader` 文件和 Shader 功能代码。

固定保留的专有名词包括：`ACES`、`AreaLit`、`Audio Link`、`FPS`、`HDR`、`LTCGI`、`Matcap`、`UBER`、`UBER X`。

### 折叠键与尾随空格

折叠标题是状态键，同一个折叠区域的以下调用必须使用完全相同的字符串：

- `Foldouts.DoFoldout(...)`
- `Foldouts.BeginFoldout(...)`
- `Foldouts.GetState(...)`

不要统一 `Trim()` 或自动整理这些标题。当前代码中有意保留了尾随空格，例如：

```csharp
"克隆"
"克隆 "
"遮罩"
"遮罩 "
"滤镜"
"滤镜 "
```

这些空格不能随手删除，否则状态键可能不再匹配，出现折叠状态错乱或重复显示。

## 更新后的检查

先确认改动以字符串为主：

```powershell
git diff --check
git diff --word-diff=porcelain -- Mochie/Unity/Editor
```

再检查页脚和版本：

```powershell
rg -n 'versionLabel|DoFooter\(' Mochie/Unity/Editor --glob '*.cs'
```

必须满足：

- 8 个页脚仍分别使用 `Standard-Shader`、`Water-Shader`、`Uber-Shader`、`Glass-Shader`、`Particle-Shader`、`ScreenFX-Shader`、`Underwater-Visuals`、`LED-Screen-Shader`。
- 所有 `versionLabel` 都包含当前 LGC 包版本。
- 没有把 Shader 属性名、资源路径或样式名翻译成中文。
- 折叠标题和对应状态键一致，尾随空格没有被清理。
- C# 文件保存为 UTF-8；当前源码带 UTF-8 BOM。

最后在 Unity 中实际打开每个材质的检查器，检查：

- 文本没有溢出、截断或重叠。
- 折叠区域可以正常展开和收起。
- 按钮、菜单、提示和重置功能正常。
- 切换 Shader、重新编译 Editor 脚本后没有报错。

## VPM 打包与发布

`package.json` 中以下值必须一致：

- `version`：`1.78.3`
- Git 标签：`1.78.3`
- GitHub Release 标签：`1.78.3`
- Release 中的 ZIP 文件名：`com.lgcchina.mochies-shaders-1.78.3.zip`
- ZIP 内 `package.json` 的 `version`：`1.78.3`

VPM ZIP 根目录必须直接包含：

```text
package.json
Mochie/
LICENSE
LGC-汉化维护说明.md
```

不要把整个仓库再套一层父目录，否则 ACOM/VPM 可能找不到 `package.json`。必须保留根目录的 `LICENSE`，并明确这是非官方中文修改版。

发布到 `LGC-VRC-Packages` 时，将对应版本加入仓库索引：

```text
https://lgcchina.github.io/LGC-VRC-Packages/index.json
```

版本条目至少需要正确的 `name`、`displayName`、`version`、`unity`、`description`、`author`、`url` 和 `zipSHA256`。`url` 指向上面的 GitHub Release ZIP，`zipSHA256` 必须根据最终上传的 ZIP 计算。

## 旧版安装冲突与 `legacyFolders`

不能同时保留直接导入的 `Assets/Mochie` 和本 VPM 包。两者包含相同的 Editor 类型、Shader 名称和资源 GUID，可能造成重复类型、资源覆盖、材质引用错乱或脚本无法编译。只在安装后再弹窗提示并不可靠，因为重复类型可能使包内 Editor 脚本在弹窗运行前就编译失败。

本包在 `package.json` 中使用以下 VPM 迁移规则：

```json
"legacyFolders": {
  "Assets\\Mochie": ""
}
```

VCC/ACOM 会在安装或升级 VPM 包时处理该规则。VCC 2.0.0 及以后会把匹配的旧 `Assets/Mochie` 视为旧版安装，并在导入新版包前删除它。该处理发生在 Unity 编译脚本之前，因此比包内脚本检测更可靠。

需要注意：

- VPM 没有安装前确认弹窗。`legacyFolders` 会直接删除整个 `Assets/Mochie`，不会询问，也不会保留已修改文件。
- 安装前必须备份项目；如果用户改过旧目录里的 Shader、预制件或贴图，必须先把备份移到项目外，例如项目根目录的 `Mochie_Backup`（与 `Assets` 同级），或者打包成项目外的 ZIP。
- 不要只把旧目录改名后继续放在 `Assets` 下，例如 `Assets/Mochie_Backup`。Unity 仍会编译其中的 Editor 类型，重复类型冲突不会消失，同时 `legacyFolders` 也不会清理这个改名后的目录。
- 不要为了“帮助清理”再发布一个独立清理工具包。脚本编译冲突可能先发生，`legacyFolders` 是安装阶段就能生效的正确机制。
- 本仓库没有根目录 `Mochie.meta`，因此这里只按旧路径匹配，不使用目录 GUID。不要凭 `Mochie/Unity.meta` 的 GUID 代替整个 `Assets/Mochie`。
- 修改、移动或重命名包内资源时必须保留对应 `.meta`；这与清理旧目录是两件不同的事。

“应用”渐变时，生成的阴影渐变和虹彩渐变会写入项目目录：

```text
Assets/LGC/MochieShaders/Textures/Ramps
```

这样既不会试图修改 VPM 缓存中的只读包，也不会再次创建 `Assets/Mochie`。

## 修改记录

### 1.78.3 - 2026-11-07

- 明确旧版备份必须移到 `Assets` 目录外，不能只在 `Assets` 内改名保留。
- 包描述改为明确提示安装会直接删除旧版 `Assets/Mochie` 且不会询问。
- 将 8 个材质检查器页脚更新为 `LGC 中文版 1.78.3`。

### 1.78.2 - 2026-11-07

- 在 `package.json` 增加 `legacyFolders`，安装时自动移除旧的 `Assets/Mochie`，解决重复类型、Shader 名称和 GUID 冲突。
- 包描述加入安装前备份警告，并在维护文档记录 VPM 会直接删除旧目录的行为。
- 将阴影和虹彩渐变贴图的生成位置改为 `Assets/LGC/MochieShaders/Textures/Ramps`。
- 将“恢复默认贴图”改为动态查找 VPM 包内资源，不再硬编码 `Assets/Mochie/Unity/Textures`。
- 将检查器里的 `Depth Light` 路径提示改为 VPM 包内目录，避免误导用户重新导入到 `Assets/Mochie`。
- 将 8 个材质检查器页脚更新为 `LGC 中文版 1.78.2`。

### 1.78.1 - 2026-11-07

- 完成 8 个材质检查器及公共 UI 组件的中文界面处理。
- 为 8 个检查器加入 `LGC 中文版 1.78.1` 页脚版本标识。
- 补充水体 `Wave 1` 至 `Wave 4`、ScreenFX 公共标签和 `Fix Now` 等遗漏文本。
- 修正支持者名单说明和按钮同行样式名的处理规则。
- 统一 VPM 包清单为中文显示名称和当前版本。
- 将维护说明与修改日志合并为这一份文档。

### 1.78.0 - 2026-11-07

- 建立 LGC fork 和 VPM 包清单。
- 包名确定为 `com.lgcchina.mochies-shaders`。
