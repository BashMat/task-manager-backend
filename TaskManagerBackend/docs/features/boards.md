# Boards

## About
This article describes `Boards` feature.

`Boards` is application feature responsible for managing:
- `Boards` – containers of `Columns`, representing a collection of tasks of the same common purpose. For example, tasks related to the same project or activity, like household chores or event planning.
- `Columns` – containers of `Cards`, representing a collection of tasks of the same status/progress. For example, planned, delayed or tasks in progress.
- `Cards` – items representing tasks themselves.

## Details
Web API uses `/api/boards` to denote all endpoints related to `Boards`.
Following endpoints are provided, which can be seen in `BoardsController`:
- `/api/boards` – endpoints for `Boards`:
  - `/api/boards` – endpoint for `Board` creation and reading:
    - `GET` request returns all `Boards` of `User`;
    - `POST` request creates new `Board`.
  - `/api/boards/{id}` – endpoint for `Board` with specified id reading, update or deleting:
    - `GET` request returns specified `Board`;
    - `PUT` request updated specified `Board`;
    - `DELETE` request deletes specified `Board`;
- `/api/boards/columns` – endpoints for `Columns`:
  - `/api/boards/columns` – endpoint for `Column` creation and reading:
      - `GET` request returns all `Columns` of `User`;
      - `POST` request creates new `Column`.
  - `/api/boards/columns/{id}` – endpoint for `Column` with specified id reading, update or deleting:
      - `GET` request returns specified `Column`;
      - `PUT` request updated specified `Column`;
      - `DELETE` request deletes specified `Column`;
- `/api/boards/cards` – endpoints for `Cards`:
    - `/api/boards/cards` – endpoint for `Card` creation and reading:
        - `GET` request returns all `Cards` of `User`;
        - `POST` request creates new `Card`.
    - `/api/boards/cards/{id}` – endpoint for `Card` with specified id reading, update or deleting:
        - `GET` request returns specified `Card`;
        - `PUT` request updated specified `Card`;
        - `DELETE` request deletes specified `Card`;

### Object structure
All specified objects have similar structure:
- `int` Id – `required` value.
- `string` Title – `required` value used as a "name" for object
- `string` Description – `optional` value used as a detailed description.
- audit fields:
  - Created By – `required` value (usually in a form of `int` or `User` instance) representing data about object author.
  - `DateTime` Created – `required` value storing date and time of object creation.
  - Modified By – `required` value (usually in a form of `int` or `User` instance) representing data about last object editor.
  - `DateTime` Modified – `required` value storing date and time of object edit.

All objects share the same relationship ruled for `User`:
- Objects are created by a single `User` instance.
- Objects are modified by a single `User` instance (when created, `Modified` data is the same as `Created`).

Additionally, objects have the following relationship rules:
- A `Card` instance belongs to a single `Column` instance.
- A `Column` instance belongs to a single `Board` instance.

Hence, following rules can be inferred:
- A `Column` instance may include any `Cards`.
- A `Board` instance may include any `Columns` and `Cards` (through `Columns`);

### Cards
`Cards` have a more unique structure due to support for user-specified ordering. An `int` order index value is stored to sort `Card` instances accordingly to user-specified values.

> ⚠️ **Warning**
>
> Due to complications of using `int` ordering values (it is impossible to insert value between items with indices `x` and `x + 1`, which required to recalculate values for multiple records), it is planned to implement ordering based on `double` values in future versions. It will make easier reordering by calculating new index as `(x + y) / 2`, for example.