# task-manager-backend

Own backend project for task management. Frontend client is located [here](https://github.com/BashMat/task-manager-frontend).

App supports:
- creating user profiles (requires username, email and password)
- managing user kanban-style boards with columns and cards

## Stack
Project created using:
- .NET 6
  - C# 10
  - ASP.NET Core 6
  - Dapper
- SQL
  - Microsoft SQL Server
  - Transact-SQL

## Structure
- \TaskManagerBackend – main directory
  - \src – directory for actual app code
    - \TaskManagerBackend.Application – core project, includes startup file, controllers and services
    - \TaskManagerBackend.Common – project for common items like const strings
    - \TaskManagerBackend.DataAccess – project for Data Access Layer, includes classes for DB queries and SQL scripts
    - \TaskManagerBackend.Domain – project for domain classes
    - \TaskManagerBackend.Dto – project for DTOs
  - \tests – directory for test code
    - \Tests.Common – project for common testing related items
    - \TaskManagerBackend.Tests – project for TaskManagerBackend related unit tests

## API routes
- /swagger/index.html - built-in endpoint for swagger