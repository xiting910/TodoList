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
- 待办事项 CRUD API 控制器（TodoController）
- 数据库上下文（TodoDbContext）和 SQLite 数据库配置
- 数据库迁移（InitialCreate）
- 请求模型（CreateRequest, UpdateRequest）
- CORS 配置，支持前端开发服务器（localhost:5173）
- 完整的控制器单元测试（TodoControllerTests）
- 前端待办事项界面（Vue 3 + TypeScript）
  - 待办事项列表展示, 支持加载中与空状态
  - 新增待办事项, 回车或点击按钮提交
  - 勾选切换完成状态, 双击编辑描述, 删除待办事项
  - 全部 / 进行中 / 已完成筛选与未完成数量统计
  - API 请求封装（todoApi）与 Vite 开发代理配置
  - 组件单元测试（TodoInput / TodoFilter / TodoListItem / TodoList, 共 18 个用例）

### Changed

- 移除测试项目中的 Moq 依赖，改用 InMemory 数据库进行测试
- 删除冒烟测试（SmokeTest），替换为完整的 CRUD 测试
- 修改数据库连接字符串名称从 "Default" 改为 "TodoList"

### Fixed

- 修复前端 CI 测试步骤在无测试文件时失败的问题（添加 passWithNoTests 配置）
- 修复 ESLint 配置中 .vue 文件 glob 模式未匹配 src 目录导致解析失败的问题

[Unreleased]: https://github.com/xiting910/TodoList/commits/main
