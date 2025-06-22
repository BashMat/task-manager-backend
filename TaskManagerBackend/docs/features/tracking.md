# Tracking

## About

This article describes `Tracking` feature.

`Tracking` is application feature responsible for managing:
- `Tracking Logs` – containers of `Tracking Log Entries`, representing a collection of trackable items of the same common purpose. For example, tasks related to the same project or activity, like household chores or event planning.
- `Tracking Log Entries` – trackable items contained in `Tracking Logs` with assigned `Tracking Log Entry Status`.
- `Tracking Log Entry Status` – status description for trackable items, which are shared via `Tracking Log`.

## Details

Web API uses `/api/tracking` to denote all endpoints related to `Tracking`.
Following endpoints are provided, which can be seen in `TrackingController`:
- `/api/tracking/logs` – endpoints for `Tracking Logs`:
    - `/api/tracking/logs` – endpoint for `Tracking Logs` creation and reading:
        - `GET` request returns all `Tracking Logs` of `User`;
        - `POST` request creates new `Tracking Log`.
    - `/api/tracking/logs/{id}` – endpoint for `Tracking Log` with specified id reading, update or deletion:
        - `GET` request returns specified `Tracking Log`;
        - `DELETE` request deletes specified `Tracking Log`;
- `/api/tracking/statuses` – endpoints for `Tracking Log Entry Statuses`:
    - `/api/tracking/statuses` – endpoint for `Tracking Log Entry Statuses` creation:
        - `POST` request creates new `Tracking Log Entry Status`.
    - `/api/tracking/statuses/{id}` – endpoint for `Tracking Log Entry Statuses` with specified id deletion:
        - `DELETE` request deletes specified `Tracking Log Entry Status`;
- `/api/tracking/log-entries` – endpoints for `Tracking Log Entries`:
    - `/api/tracking/log-entries` – endpoint for `Tracking Log Entry` creation and reading:
        - `GET` request returns all `Tracking Log Entries` of `User`;
        - `POST` request creates new `Tracking Log Entry`.
    - `/api/tracking/log-entries/{id}` – endpoint for `Tracking Log Entry` with specified id reading, update or deletion:
        - `GET` request returns specified `Tracking Log Entry`;
        - `PUT` request updated specified `Tracking Log Entry`;
        - `DELETE` request deletes specified `Tracking Log Entry`;

### Object structure

All specified objects have similar structure:
- `int` `Id` – `required` value.
- `string` `Title` – `required` value used as a "name" for object
- `string (nullable)` `Description` – `optional` `nullable` value used as a detailed description.
- audit fields:
    - `Created By` – `required` value (usually in a form of `int` or `User` instance depending on object representation) representing data about object author.
    - `DateTime` `Created` – `required` value storing date and time of object creation.
    - `Modified By` – `required` value (usually in a form of `int` or `User` instance depending on object representation) representing data about last object editor. Upon creation, it is set as equal to `Created By`.
    - `DateTime` `Modified` – `required` value storing date and time of object edit. Upon creation, it is set as equal to `Created`.

All objects share the same relationship ruled for `User`:
- Objects are created by a single `User` instance.
- Objects are modified by a single `User` instance (when created, `Modified` data is the same as `Created`).

Additionally, objects have the following relationship rules:
- A `Tracking Log Entry` instance belongs to a single `Tracking Log Entry Status` instance, representing relationship "`Tracking Log Entry` has one `Tracking Log Entry Status`".
- A `Tracking Log Entry Status` instance belongs to a single `Tracking Log` instance, representing relationship "`Tracking Log Entry Status` is available in context of one `Tracking Log`".

Hence, following rules can be inferred:
- A `Tracking Log Entry Status` instance may include any number of `Tracking Log Entries`, representing relationship "Under single `Tracking Log Entry Status` may be any number of `Tracking Log Entries`".
- A `Tracking Log` instance may include any `Tracking Log Entry Statuses` and `Tracking Log Entries` (through `Tracking Log Entry Status`), representing relationship "Single `Tracking Log` contains any number of `Tracking Log Entry Statuses` and `Tracking Log Entries`";

### Tracking Log Entries

`Tracking Log Entries` have a more unique structure due to support for user-specified ordering and priority level. An `int (nullable)` `Priority` value is stored to describe relative priority level of items. Value `1` is considered the highest priority. `Priority` may be set as `null` to describe not set priority level at the time. `double` `Order Index` value is used as a virtual property to sort items, meaning it may be used by client to sort items by specific sorting value instead of certain properties like dates or `Priority`. 