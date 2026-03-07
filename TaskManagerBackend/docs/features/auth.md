# Auth

## About

This article describes `Auth` feature.

`Auth` is application feature responsible for: `User` registration, authentication and authorization. 
This project does not rely on external component for managing `Users`, their authentication and authorization.

## Details

Web API uses `/api/auth` to denote all endpoints related to `Auth`.
Following endpoints are provided, which can be seen in `AuthController`:

- `/api/auth/signup` – endpoint for `Signing Up` (registration). It supports only `POST` requests;
- `/api/auth/token` – endpoint for `Issuing Tokens` (signing in). It supports only `POST` requests.
- `/api/auth/revoke` – endpoint for `Token Revocation` (ending all sessions). It supports only `POST` requests;

### Functionality

#### Signing Up

`Signing Up` is available at `/api/auth/signup` and requires API user to provide:

- username;
- e-mail address;
- password (no encryption is used on request level);

Application does the following to complete request:

- validates e-mail address;
- checks if `User` of the same username and/or e-mail address already exists;

If everything is correct (e-mail is validated and `User` identification data is unique), new `User` is registered.

> ℹ️ **Information**
>
> Currently, application does not support e-mailing functionality. 
> It means that at the moment data regarding e-mail address is stored only for information purposes. In future
> e-mail provider may be used to send e-mails for various functionality like verification / account restoration.

#### Signing In / Issuing Tokens

`Signing In` or `Issuing Tokens` are available at `/api/auth/token` and mean 
that API provides access and refresh tokens to user. This functionality requires API user to provide:

- `grant_type` specifying whether password or refresh authentication is used;
- Credentials for one of two authentication flows:
  - If `password` `grant_type` is used, `User` identification value (username or e-mail address) and 
password are used (no encryption is used on request level);
  - If `refresh_token` `grant_type` is used, only refresh token value is used (no encryption is used on request level);

Application does the following to complete request:

- checks if specified user exists;
- checks if specified password or refresh token is correct;

If everything is correct (user credentials are valid), JWT is issued to `User`.

#### Token Revocation

`Token Revocation` is available at `/api/auth/revoke`. It does not require any arguments, only `User` access token.

Application does the following to complete request:

- deletes all refresh token data for `User`;

### JWT

Application uses JWT format for both access and refresh tokens.