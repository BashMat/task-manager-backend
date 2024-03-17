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
- \TaskManagerApi – main folder
  - \Src – folder for actual app code
    - \TaskManagerApi – core project, includes startup file, controllers and services
    - \TaskManagerApi.Common – project for common files like const strings
    - \TaskManagerApi.DataAccess – project for Data Access Layer, includes classes for DB queries and SQL scripts
    - \TaskManagerApi.Domain – project for domain classes and DTOs
    - \TaskManagerApi.Dto – project for DTOs
  - \Tests – folder for test code
    - \TaskManagerApi.Tests – project for TaskManagerApi related unit tests

## API routes
TBA