# Glossary

## About

This article provides glossary used for project. It includes following topics:

- Domain knowledge
- Development (code-level) naming conventions regarding formal usage of different common concepts
- etc.

## Definitions

### Domain

| Term                 | Definition                                                                                                                                                                                                 |
|----------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `User`               | Represents application user. They can be registered (`sign up` action) and later get access to execute actions in application.                                                                             |
| `Tracking Log`       | Represents log for tracking (log of tracked items). Contains `Tracking Log Entry` and used `Tracking Log Entry Statuses`. Depending on use case may act as grid or board of tasks and other tracked items. |
| `Tracking Log Entry` | Represents status for `Tracking Log Entry`.                                                                                                                                                                |
| `Tracking Log Entry` | Represents tracked item of `Tracking Log`.                                                                                                                                                                 |

### Development

| Term         | Definition                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
|--------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Controller` | `Controller` is a class responsible for processing incoming application requests. See ASP .NET Core Controllers.                                                                                                                                                                                                                                                                                                                                                                                    |
| `Service`    | `Service` is a common name for `service classes`, i.e. classes which primary responsibility is to provide API for common action. This behaviour is in contrast with `data classes` which have no or only minimal behaviour, and `rich domain entity classes` which represent specific domain entity instance with its behaviour. `Services` are primarily stateless, and act as orchestrators of actions, but they may contain some state (for example, if `Service` provides caching capabilities) |
| `Repository` | `Repository` is a `Service` responsible for data access. `Repositories` usually expose only specific data actions, required for domain actions.                                                                                                                                                                                                                                                                                                                                                     |
| `Request`    | `Request` classes are meant only for incoming application request DTOs.                                                                                                                                                                                                                                                                                                                                                                                                                             |
| `Response`   | `Response` classes are meant only for outgoing application request DTOs.                                                                                                                                                                                                                                                                                                                                                                                                                            |
| `Mapping`    | `Mapping` refers to act of getting one `data class` from another. Other projects may use `Converting` or `Projecting`.                                                                                                                                                                                                                                                                                                                                                                              |