# ADR 2026-04-15-001: Scope and Tech Stack

## Tags

- `Date`: `2026-04-15`
- `Extra`:
  - `API`
  - `ASP.NET Core`
  - `C#`
  - `SQL`

## Status

`ADR` is `Active`.

## About

This `ADR` covers decisions around project scope and used technology stack.

## Context

This is initial document acting as introduction to project and decisions. 
It documents accepted decisions on future plans around scope and with pros and cons were evaluated.

## Decisions

### Scope

> 📜 **Summary**
>
> Project targets ...

### Tech Stack

> 📜 **Summary**
>
> Project uses classic `.NET` platform tech stack. Decision is based around familiarity with `C#` 
> as a programming language.
> 
> Details:
> - `C#` programming language.
>   - Familiar language with active support by Microsoft.
>   - C-like syntax.
>   - Multiparadigm support: heavily used for OOP, but has influence of functional languages.
>   - Implemented as cross-platform since `.NET Core`/`.NET`.
>   - Is well-known and used.
> - `ASP.NET Core` framework.
>   - Main framework by Microsoft.
>   - Provides great capabilities around middleware, authentication and authorization.
>   - Is well-known and used.
> - `Entity Framework Core` as ORM.
>   - One of main ORM solutions for `C#`.
>   - Won over `Dapper` due to being much simpler to be used without performance issues.
>   - Is well-known and used.
> - `Microsoft SQL Server` as RDBMS.
>   - One of main RDBMS used with `C#` and `.NET`.
>   - Is well-known and used.