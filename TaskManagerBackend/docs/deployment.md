# Deployment

## About
This article describes application deployment process.

## Deployment
1. Prerequisites:
   - `.NET 8` must be installed to build and run application. See `.NET 8 SDK`.
   - `Docker` must be installed if you want to build and deploy application as a `Docker` image. `Docker 4.29.0` was used during development.
   - `Microsoft SQL Server` must be installed to create and use database. `SQL Server 2019` was used during development.
   - Proxy web server should be used for actual deployment. 
2. Set up environment:
   - Set up your `Microsoft SQL Server` instance to enable network connection. Depending on your use case, it may be local connection or connection to remote machine.
   - Create new database using `Microsoft SQL Server`. Any name can be used. 
   - Create server instance login and related database user and set its password. 
   - Run migration scripts to set up database. See `task-manager-backend/TaskManagerBackend/src/TaskManagerBackend.DataAccess/Sql/Migrations`. Script execution order is specified with `x_x_` prefix.
3. Build:
   - Using .NET:
     - `cd` to root directory of source code: `cd task-manager-backend/TaskManagerBackend`.
     - Run `dotnet publish -o <>`, where `<>` denotes your target path for application executable files and other related files
   - Using Docker:
     - Run `docker build` using provided `Dockerfile` at `task-manager-backend/TaskManagerBackend`. Tag image as you see fit.
4. Setup variables:
    - If you build directly from source or use `Docker` without environment variables, edit `appsettings.json` in `task-manager-backend/TaskManagerBackend/src/TaskManagerBackend.Application`:
        - Set `logsDirectory` variable value for logs path.
        - Set `TaskManagerDb` values in `ConnectionStringsData`. Use database name and user data used when creating it in previous steps.
        - Set `Token` string value to be used as a secret.
    - If you wish to use `Docker` with environment variables, edit `docker-compose.yaml` in `task-manager-backend/TaskManagerBackend/infrastructure`:
        - Set `logsDirectory` variable value for logs path.
        - Set `TaskManagerDb` values. Use database name and user data used when creating it in previous steps.
        - Set `Token` value to be used as a secret.
5. Run application:
   - If you build directly from source, find `TaskManagerBackend.Applcation.exe` file at your chosen output directory path, and run it.
   - If you use `Docker`, run `docker compose` with specified `docker-compose.yaml` file.