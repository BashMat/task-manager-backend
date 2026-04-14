# ADR 2026-04-11-001: API requests and responses

## Tags

- `Date`: `2026-04-15`
- `Extra`:
  - `API`
  - `DTO`
  - `ASP.NET Core`
  - `C#`
  - `Validation`
  - `JSON`

## Status

`ADR` is `Active`.

## About

This `ADR` covers decisions around structuring API request and response DTOs, how these DTOs are used in application and 
how API handlers are designed.

## Context

API endpoints may be designed differently according to team standards. These decisions affect naming,
used formats, etc. Some decisions affect only API style, others affect functionality.

Application uses `ASP.NET Core` as a framework to host web API. Traditional HTTP-based Web API applications expose 
API endpoints as a combination of `route` / `path`, `HTTP method` and other attributes.

Among these, `request DTOs` are
usually used as `C#` `data objects` acting as "bags" of attributes without behavior.

However, `C#` `data objects` using only old traditional language capabilities 
(properties with getters and setters, primitive and non-primitive types) by themselves are not enough due to problems 
of data `serialization` and `deserialization`: web server accepts and returns data using `JSON` format. API consumer 
may pass request data incorrectly, hence web API based applications have to handle `data validation` problem during 
`data deserialization` into expected `request DTOs`.

As a result, all these moments require actual architecture decisions to control API design.

## Decisions

### API design

> 📜 **Summary**
>
> Project provides HTTP API. As **true REST API is never implemented properly, this project does not 
> try to pose as REST API application** (see below for clarification if required). 
> Endpoints and handlers are considered to provide RPC-style API. 
> Following guidelines are to be taken into account when designing new API:
>
> - Endpoint paths use kebab-case.
> - Each endpoint starts with `/api/`.
> - Endpoints specify target entity type, possibly with shared base path: `/api/feature/entity-type-1` and 
>   `/api/feature/entity-type-2`
> - It is **okay to use verbs in paths to describe executing actions on entity**. It is preferred to have clearly named endpoint
>   instead of trying to use a single path with different handlers for each HTTP method. For example,
>   using `POST entity` to create entity, `POST entity/edit` to edit entity attributes and `POST entity/action` to
>   do domain-specific action is preferred over `POST entity`, `PATCH entity/{id}` and `POST entity/noun`, respectively.  
> - The most simple HTTP methods are preferred, like `GET` and `POST`. This project considers `POST` endpoints 
>   to be implemented according to [RFC 7231](https://datatracker.ietf.org/doc/html/rfc7231) as an endpoint that 
>   acts as a `target resource process the representation enclosed in the request according to the 
>   resource's own specific semantics`.

1. Project provides HTTP API as it is most used way to provide network based API.
2. Project API is designed to be pragmatic, not to chase mainstream ideas:
    - **True REST API is never implemented properly, and this project does not try to pose as REST API application**.
      See following articles for clarification:
        - ["It is okay to use POST" by Roy T. Fielding](https://roy.gbiv.com/untangled/2009/it-is-okay-to-use-post)
        - ["REST APIs must be hypertext-driven" by Roy T. Fielding](https://roy.gbiv.com/untangled/2008/rest-apis-must-be-hypertext-driven)
        - ["Richardson Maturity Model" by Martin Fowler](https://martinfowler.com/articles/richardsonMaturityModel.html)
        - ["API Example Using REST" by Jeremy H](https://thereisnorightway.blogspot.com/2012/05/api-example-using-rest.html)
        - ["Roy Fielding's Misappropriated REST Dissertation" by Two-Bit History](https://twobithistory.org/2020/06/28/rest.html)
    - API is considered to be implemented in RPC-style, where each endpoint handler provides action according to its
      interface contract. Project does not use `RPC` term as a synonym to such frameworks/protocols as
      `gRPC`, `JSON-RPC`, `SOAP`, `CORBA` or others. Instead `RPC` is considered as a common idea of `calling functions (procedures)
      implemented in different address space, usually via network` without any details on implementation. See following articles
      for clarification:
        - ["RPC is Not Dead: Rise, Fall and the Rise of Remote Procedure Calls" by Muzammil Abdul Rehman and Paul Grosu](http://dist-prog-book.com/chapter/1/rpc.html)
3. Endpoint naming:
   - Paths use kebab-case.
     - Kebab-case is not affected by possible ambiguity between underscores (`_`) and 
       hyperlink representation in text processors(https://example.example)
     - Kebab-case makes separate words visually obvious.
     - Kebab-case is not affected by possible differences between web servers handling upper- and lowercase.
   - Each endpoint starts with `/api/`. This is just an accepted style choice that clearly separates hosted backend
     from possible frontend application
   - Usually endpoints specify target entity type. If multiple entities belong to common functionality, they may be joined
     via a shared feature name. For example, `/api/feature/entity-type-1` and `/api/feature/entity-type-2` are used for
     entities used in common functionality.
   - Verb usage is preferred for specific actions (`/api/feature/entity-type-1/action-1`).
   - Examples:
     - `/api/tracking/logs`
     - `/api/tracking/logs/rename`
     - `/api/tracking/log-entries`
     - `/api/auth/token`
4. The most simple HTTP methods are preferred, like `GET` and `POST`.
   - `GET` is preferred for read API unless too many arguments are used. In this case `POST` method 
     with explicit request should be used instead of query string.
   - This project considers `POST` endpoints to be implemented according to 
     [RFC 7231](https://datatracker.ietf.org/doc/html/rfc7231) as an endpoint that acts as a 
     `target resource process the representation enclosed in the request according to the resource's own 
     specific semantics`.
   - `PUT` method should be avoided, unless it really makes sense. According to 
     [RFC 7231](https://datatracker.ietf.org/doc/html/rfc7231),
     `PUT` `requests that the state of the target resource be created or replaced with the state defined by 
     the representation enclosed in the request message payload.`. It acts as an upsert which is not as
     obvious as separate Create and Update actions. Moreover, `PUT` is often used to pass multiple attributes together,
     even if they are very different, and by itself does not handle cases of possibly missing arguments (should `null`
     be considered as `set attribute to null` or `preserve value`?). `POST` endpoint with `JSON Merge Patch` semantics
     is preferred (see [`RFC 7386`](https://datatracker.ietf.org/doc/html/rfc7386)).
     - `POST` with `Content-Type: application/json` are used for `JSON Merge Patch` for simplicity. 

### API handler flow

> 📜 **Summary**
>
> Project uses classic `Controller`-`Service`-`Repository` layering architecture with `ASP.NET Core` middleware.
> Usually, this pipeline targets single feature and named accordingly:
> 
> - `<Feature>Controller`
> - `<Feature>Service`
> - `<Feature>Repository`
> 
> However, this rule is not strict and may differ from case to case.

1. Application uses classic `Controller`-`Service`-`Repository` layering architecture with `ASP.NET Core` middleware.
Following decisions are used:
   1. `Controller` and surrounding framework elements (helper classes, middleware, etc.) are responsible for operating
   on transport level objects (HTTP request/response object).
      - Usage examples:
        - Reading token claims
        - Writing HTTP status codes
      - `Controller` classes are located in `TaskManagerBackend.Application` project in `/Features` directory according 
      to feature name.
      - `Controller` classes derive from `ControllerBase` located in `/Features` as root directory.
      - `Controller` contains `endpoint handlers` that handle actual HTTP requests.
   2. `Service` classes are responsible for higher level actions. They act as orchestration layer, combining domain logic
   in use cases. From design point of view such classes are considered as `Application Services`.
      - Usage examples:
        - Creating `Domain` classes and calculating values.
        - Checking conditions and enforcing access restrictions.
        - Setting final action result.
      - `Service` classes are located in `TaskManagerBackend.Application` project in `/Features` directory according
      to feature name next to specified `Controller` class. This way we do not pollute project with too many 
      different locations and specify that `Service` class is responsible for specified feature. This does not break
      layering as `Service` class is not considered to be part of domain, but a higher level handler. This may change in
      future if `Service` classes start providing more complex logic.
        - `Service` classes return `ServiceResponse<T>` objects. They act as a response data bag suitable for serialization
        as they contain all required data: payload (`Data`), action status result (`ActionResult`) and 
        possible message (`Message`). `Controller` uses `ServiceResponse<T>` to return actual HTTP response.
   3. `Repository` classes are responsible for data access: reading and writing data from/to external components like 
   web API providers and databases.
       - Usage examples:
         - Querying data from database.
         - Persisting changes to database.
2. As a common approach, `Controller`-`Service`-`Repository` correspond to single feature, creating a direct flow. Therefore,
they have common naming:
   - `<Feature>Controller`
   - `<Feature>Service`
   - `<Feature>Repository`

### Request and response DTOs and their validation

> 📜 **Summary**
>
> Project uses request and response DTOs for each endpoint handler. It is preferred that each endpoint handler 
> uses their own DTOs, but for simplicity they may be shared. 
> 
> - Each DTO is named as `<Name>Request` or `<Name>Response`, where `<Name>` suits target action. For example,
> `<Entity><Action>` (`OrderCreateRequest` and `OrderGetResponse`) style may be used for domain entity based actions, 
> while `<Action><Object>` (`IssueTokenRequest`) can be used for less domain-oriented actions.
> - Attribute-based validation is used.
>   - `[Required]` attribute is used over `required` keyword.
> - Properties use `{ get; init; }`

Each endpoint handler has corresponding request and response DTOs.