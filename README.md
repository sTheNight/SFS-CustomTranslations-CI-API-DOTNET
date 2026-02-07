## SFS-CustomTranslations-CI-API

本项目用于自动拉取 [SFS-CustomTranslations-Installer](https://github.com/youfeng11/SFS-MobileTools) 的 CI 构建。

后端部分使用 [GNU AGPL v3](./LICENSE.txt) 协议发布。

## 环境变量

在部署前需确保设置以下环境变量：

```
GITHUB_REPO_NAME=SFS-CustomTranslations-Installer
GITHUB_REPO_OWNER=youfeng11
GITHUB_TOKEN=your_personal_github_token_here
```

`GITHUB_TOKEN` 请使用你自己的 GitHub Token，并确保不要泄露到公共仓库。

支持读取项目运行目录下的 `.env` 文件，格式为 `KEY=VALUE`。
