# Authorization

## About

This article describes `Authorization` feature.

`Authorization` is application feature responsible for: `User` registration, authentication and authorization. This project does not rely on external component for managing `Users`, their authentication and authorization.

## Details

Web API uses `/api/auth` to denote all endpoints related to `Authorization`.
Following endpoints are provided, which can be seen in `AuthController`:
- `/api/auth/signup` – endpoint for signing up (registration). It supports only `POST` requests.
- `/api/auth/login` – endpoint for logging in. It supports only `POST` requests.

### Signing up

Signing up requires API user to provide:
- username;
- e-mail address;
- password (no encryption is used on request level).

Before registering new `User`, application:
- validates e-mail address;
- checks if `User` of the same username and/or e-mail address already exists

If everything is correct (e-mail is validated and `User` identification data is unique), new `User` is registered.

### Logging in

Logging in requires API user to provide:
- `User` identification value (username of e-mail address);
- password (no encryption is used on request level).

Before authorizing `User`, application:
- checks if specified user exists;
- checks if specified password is correct.

If everything is correct (`User` exists and specified password hash is the same as saved one), JWT is issued to `User`.

### JWT

Application uses JWT format for Access-tokens.

> ⚠️ **Warning**
> 
> Refresh-tokens currently are not implemented. When Access-token is expired, API user has to request a new token manually, providing their credentials.