# task-manager-backend


Own backend project for task management. Frontend client is located [here](https://github.com/BashMat/task-manager-frontend).


App supports creating user profiles (requires username, email and password), projects and tasks.

## Stack
Project created using:
- C#
  - .NET 6
  - ASP.NET Core 6
  - Dapper
- SQL (Transact-SQL)
- Microsoft SQL Server


## Structure
- \TaskManagerApi
  - \TaskManagerApi - core project, includes startup file, controllers and services
  - \TaskManagerApi.Common - project for common files like const strings
  - \TaskManagerApi.DataAccess - project for Data Access Layer, includes classes for DB queries and SQL scripts
  - \TaskManagerApi.Domain - project for domain classes and DTOs

## API routes
TBA