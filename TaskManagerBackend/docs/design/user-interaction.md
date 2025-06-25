# User Interaction

## About

This article describes `User Interaction` feature design.

`User Interaction` is application feature responsible for providing user cooperation and multi-user support in project.

## Details

Current (`0.3.0`) project implementation provides multi-user support only from persistence perspective. For example, multiple `Users` may `Sign Up` and `Log In`, however, each `User` can interact only with their own "workspace" consisting of `Tracking Logs` and their `Entries` and `Statuses`.

Multi-user support is required for project, hence following design is proposed.

### User Story

#### Invitation process

`User` can create `Invitation Links` to invite other `Users` to work on the same `Tracking Log`.

Sending request to some API, for example, `POST` `/api/invitations` with provided `Bearer token`, adds `User` to list of `Users` authorized to work on this `Tracking Log`.

When `User` requests list of available `Tracking Logs`, they see:

- their `Own` `Tracking Logs`
- all `Tracking Logs` that `User` was invited to work on

#### Permissions

`User` can control `Permissions` to `Tracking Log`:

- Any `User` can request list of `Users` authorized to work on specified `Tracking Log`
- `User` with specific `Permissions` can manage this list:
    - Remove `Users` from `Tracking Log`
    - Update `Users` `Permissions`

Following `Permissions` are considered:

- `Create` – permits creation of all data inside entity (equivalent to having both `Create Entries` and `Create Statuses`)
    - `Create Entries` – permits creation of `Tracking Log Entries` for specified `Tracking Log`
    - `Create Statuses` – permits creation of `Tracking Log Entriy Statuses` for specified `Tracking Log`
- `Edit` – permits editing of all data inside entity (equivalent to having `Edit Entries` and `Edit Statuses`)
    - `Edit Entries` – permits editing `Tracking Log Entries` for specified `Tracking Log`
    - `Edit Statuses` – permits editing `Tracking Log Entriy Statuses` for specified `Tracking Log`
- `Read` – permits reading of all data inside entity
- `Delete` – permits deletion of all data inside entity (equivalent to having both `Delete Entries` and `Delete Statuses`).
    - `Delete Entries` – permits deletion of `Tracking Log Entries` for specified `Tracking Log`
    - `Delete Statuses` – permits deletion of `Tracking Log Entriy Statuses` for specified `Tracking Log`
- `Control Access` – permits managing list of `Users` who are authorized to work on specified `Tracking Log`

At least two extra roles are considered for `Users` authorized to work on `Tracking Log`. `Owner` role is used to connect `Tracking Log` and `User` who has absolute control over `Tracking Log`. `Administrator` is used to specify `Users` who have elevated control over `Tracking Log` and who may replace `Owner` if required. Following rules are considered:

- `Owner` and `Administrator` have all `Permissions` and they cannot be changed
- Only `Owner` can select `Administrators`
- Only `Owner` can delete the whole `Tracking Log`
- If `Owner` is inactive for a long period of time, `Administrators` can vote to select a new `Owner` among themselves. Period of time is 6 month by default. Later it will be able to be changed via settings.

### Implementation Details

### Roles and initial creation of list of authorized users

Firstly, database schema and existing API must be changed to support lists of `Users`:

- New table `Role` will describe available roles for `Tracking Log`
  - `int` `Id` – 1 for `User`, 2 for `Admin` and 3 for `Owner`.
  - Table has no CRUD operations from backend side and used only as a source of truth of other tables` FKs.
-  New table `TrackingLogUsers` will describe relationship between `Tracking Log` and `Users`:
  - `int` `TrackingLogId`
  - `int` `UserId`
  - `int` `RoleId` – describes whether `User` is `Owner`, `Administrator` or simple `User`. Has FK on `Role`.

Secondly, API implementation must be changed:

- New enum `Role` describes available `Role` values.
- `POST` `/api/tracking/logs` will add new record for `TrackingLogUsers` with `TrackingLogId` of newly created `Tracking Log`, `UserId` of creator and `RoleId` specifying `Owner` `Role`.
- `DELETE` `/api/tracking/logs/{id}` will support cascade deletion of records in `TrackingLogUsers` for specified `Tracking Log` id. 
- `GET` `api/tracking/logs` now returns records that `User` can work on, not just those created by them.

### Invitation Links

Update database schema:

- New table `Type` will describe various types of objects.
  - `int` `Id` – 1 for `User`, 2 for `Tracking Log`. Value 1 is not planned to be used for now, but may be added to support registration invitation if future.
  - Table has no CRUD operations from backend side and used only as a source of truth of other tables` FKs.
- New table `InvitationLink` will describe current links for various objects:
  - `int` `Id` – id of record in table of `type`
  - `int` `TypeId` – id of type

Add new API endpoint to create and get `Invitation Link`:

- New enum `Type` describes available `Type` values.
- `POST` `api/invitations` with body schema `{ "id": int, "type": string }`. For example, sending request with `{ "id": 1, "type": "TrackingLog" }` creates new `Invitation Link` for `Tracking Log` with `id` 1. If link already exists, it is regenerated.
- `GET` `api/invitations?id={id}&type={type}` returns `Invitation Link` for specified entity.
- `POST` `api/invitations/{link}` activates invitation. For `Tracking Log` invitation it adds `User` to `TrackingLogUsers`

### Permissions and Controlling Access

Update database schema:

- New table `Permission` will describe various rights/
  - `int` `Id` – ...
- New table `UserPermission` will describe relationship between `User` and their `Permission`
  - `int` `UserId`
  - `int` `PermissionId`

Add new functionality to backend:

- New enum `Permission` describes available `Permission` values.
- Add caching to remove excessive roundtrips to check permissions. Cache would be something like Dictionary<int, Dictionary<ResourceType, List<Permission>>> 
- Add API to manage `Permission`.
  - `POST` `/api/users/{id}/permissions` with body schema `{ "resourceId": int, "resourceType": string, "role": string | null, "permissions": [ string ] | null}` replaces permission set of specified `User` over some resource. For example, sending request at `api/users/1/permissions/` `{ "resourceId": 1, "resourceType": "TrackingLog", permissions: [ "Read" ]}` sets read-only permission set for `User` with id 1 over `Tracking Log` with id 1. Fields "role" and "permissions" are mutually exclusive: consumer has to specify only one setting. 
    - Execution of this endpoint must be protected with requirement of `Control Access` `Permission` over specified resource.
  - `GET` `/api/users/{id}/permissions` gets all permissions of specified `User`.

Add `Permission` checks to existing endpoints:

- CRUD operations over `Tracking Logs`, `Entries` and `Statuses`

### Changing owner

Changing owner may be a bit more complex, because case of `Owner` inactivity has to be considered. Voting mechanism between `Administrators` is considered to make possible changing `Owner` if they were inactive for a long period of time.

- TBA