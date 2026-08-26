# Changelog

本文件记录了项目的所有重要变更。每个版本的变更都应在发布时记录在此文件中。

格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/),
版本号遵循 [Semantic Versioning](https://semver.org/lang/zh-CN/).

---

## [Unreleased]

### Added

- 后端项目初始化：基于 .NET 10 的 ASP.NET Core Web API
- 前端项目初始化：Vue 3 + TypeScript + Vite
- GitHub Actions CI/CD 工作流配置
  - 自动检测前后端代码变化并运行相应的构建测试
  - CodeQL 代码安全分析
  - Dependabot 依赖更新和自动合并
  - 依赖审查
- GitHub Issue 和 PR 模板
- 项目配置文件（.editorconfig, .gitignore, .gitattributes）
- 前后端单元测试框架配置（xUnit / Vitest）
- Dependabot npm 依赖更新配置（前端）

### Fixed

- 修复前端 CI 测试步骤在无测试文件时失败的问题（添加 passWithNoTests 配置）

[Unreleased]: https://github.com/xiting910/TodoList/commits/main
