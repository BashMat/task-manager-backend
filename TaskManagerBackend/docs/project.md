# Project

## About

This article describes main project information.

`Task Manager Backend` is a web API hobby project for task management (task tracking). It is inspired by other task managing software applications and methodologies.

See `Task Manager Frontend` client in its own repository: [task-manager-frontend](https://github.com/BashMat/task-manager-frontend).

## Versioning

Application uses `semver` for versioning. Currently, it is deep under development, hence version `0.x.x` is used.

## Stack

Project uses following technologies:
- `.NET 8`
    - `C# 12`
    - `ASP.NET Core 8`
    - Different libraries for:
      - core application logic and infrastructure:
        - `Dapper`
        - `NLog`
        - `prometheus-net`
        - `Swagger`
      - testing:
        - `xUnit`
        - `Moq`
        - `Bogus`
        - `FluentAssertions`
- `SQL`
    - `Microsoft SQL Server`
    - `Transact-SQL`
- `Docker`

## Project Structure

- `task-management-backend/TaskManagerBackend` – main directory for project.
    - `/docs` – directory for documentation. `.md` file extension is used for articles.
    - `/src` – directory for actual app source code.
        - `/TaskManagerBackend.Application` – web application level project, includes startup file, controllers and services.
        - `/TaskManagerBackend.Common` – project for common items like const strings.
        - `/TaskManagerBackend.DataAccess` – project for Data Access Layer, includes classes for database queries and SQL scripts.
        - `/TaskManagerBackend.Domain` – project for domain classes.
        - `/TaskManagerBackend.Dto` – project for DTOs.
    - `/tests` – directory for test code.
        - `/TaskManagerBackend.Application.Tests` – project for TaskManagerBackend.Application related unit tests.
        - `/TaskManagerBackend.Domain.Tests` – project for TaskManagerBackend.Domain related unit tests.
        - `/TaskManagerBackend.Tests.Common` – project for common testing related items.
    - Other files are located outside specific directories:
      - `Directory.Build.props` and `TaskManagerBackend.sln` are common file for managing .NET application.
      - `docker-compose.yaml`, `Dockerfile` and `.dockerignore` files are used for application building and deploying via `Docker`.

## Application Components

Application consists of two main components:
- Actual web API application (API provider).
- Relational database storing persisted data.

To see existing API consuming client, check [task-manager-frontend](https://github.com/BashMat/task-manager-frontend).