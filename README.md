# WitchDrawer.Plugins

[WitchDrawer](https://github.com/witchscottishfoldcat/WitchDrawer) 的官方插件目录和开发示例。时钟、性能组件等扩展通过原生 WPF 按需安装。

本仓库对应正在开发的插件 API v1；已发布的旧版 WitchDrawer 尚未包含插件入口。当前目录为空，时钟示例可自行打包并安装到支持插件的开发版。审核收录后，用户刷新插件中心即可获取插件。

## 提交插件

1. 在自己的公开 GitHub 仓库维护插件源码，发布固定版本的 Release ZIP。
2. Fork 本仓库，复制 `plugins/templates/catalog-entry.json` 到 `plugins/entries/{id}.json`，填写元数据、包地址和 SHA-256。
3. 在仓库根目录生成并检查目录：

   ```powershell
   dotnet run --project tools/PluginCatalog -- --write
   dotnet run --project tools/PluginCatalog -- --check
   ```

4. 向本仓库的 `main` 提交 PR，填写默认模板。维护者审核合并后上架。

也可以填写 [插件收录申请](https://github.com/witchscottishfoldcat/WitchDrawer.Plugins/issues/new?template=plugin-submission.yml)，由维护者审核后帮助加入目录。

[完整开发与投稿指南](plugins/README.md) 包含接口、权限说明、生命周期、资源开销要求和审核规则。插件与本体共享进程权限，启用前需要用户确认信任。

## 目录结构

| 路径 | 用途 |
| --- | --- |
| `plugins/entries/` | 经 PR 审核的插件条目 |
| `plugins/catalog.json` | 应用读取的目录，由工具生成 |
| `plugins/templates/` | 投稿条目模板 |
| `plugins/samples/Clock/` | 可打包的原生 WPF 时钟示例 |
| `src/WitchDrawer.PluginSdk/` | 插件 API v1 合约 |
| `tools/Pack-Plugin.ps1` | 本地打包脚本，输出 ZIP 和 SHA-256 |
| `tools/PluginCatalog/` | 无第三方依赖的目录检查工具 |

在 Windows 上安装 .NET 10 SDK 后，运行 `.\tools\Pack-Plugin.ps1` 打包时钟。目录检查可在 Windows、Linux 或 macOS 上运行。GitHub Actions 自动检查元数据和生成目录的一致性；投稿 ZIP 不会在 CI 中下载或执行。

线上目录：[catalog.json](https://raw.githubusercontent.com/witchscottishfoldcat/WitchDrawer.Plugins/main/plugins/catalog.json)。

SDK 与检查工具的 v1 合约源自 WitchDrawer 本体；维护者修改协议时应同步两仓库并保持兼容。开发示例采用与本体一致的 [软件许可证](LICENSE) 和 [文档许可证](LICENSE-DOCS)，保留原作者声明。作者自己的插件应明确各自的代码与素材许可证。