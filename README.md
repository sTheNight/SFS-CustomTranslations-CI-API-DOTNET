## SFS-CustomTranslations-CI-API

本项目用于自动拉取 [SFS-CustomTranslations-Installer](https://github.com/youfeng11/SFS-CustomTranslations-Installer) 的 CI 构建。

后端部分使用 [GNU AGPL v3](./LICENSE.txt) 协议发布。

## 编译

IDE 很好用，推荐使用 Visual Studio 的发布功能进行编译。

本项目基于 .NET 9.0，发布时可选择：

- 框架依赖模式 (Framework-dependent)：生成 `.dll` 文件，需要服务器安装 .NET 9.0 Runtime。
- 自包含模式 (Self-contained)：生成可执行文件，无需额外安装 .NET 运行时。

## 部署

- 框架依赖发布：执行
  ```bash
  dotnet SFSCtinstallerAPI.dll
  ```
- 自包含发布：直接执行生成的二进制文件即可。

## 环境变量

在部署前需确保设置以下环境变量：

```
GITHUB_REPO_NAME=SFS-CustomTranslations-Installer
GITHUB_REPO_OWNER=youfeng11
GITHUB_TOKEN=your_personal_github_token_here
```

`GITHUB_TOKEN` 请使用你自己的 GitHub Token，并确保不要泄露到公共仓库。

支持读取项目运行目录下的 `.env` 文件，格式为 `KEY=VALUE`。
