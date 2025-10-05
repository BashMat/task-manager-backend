# History

## About

This article describes `History` feature design.

`History` is an application feature responsible for preserving a history of actions. The first example of such history log is a history of entity: when certain entity was created, updated, etc. 

## Details

Current (`0.3.0`) project implementation provides only single-user experience. Future versions target multi-user support for group projects. For example, certain group may work together on project with multiple `Tracking Logs` used to track different types of entries: tasks, planned and implemented features, etc. As every group member has ability to update their or other member`s entries, it is required to be able to see what actions were applied to entries.

This functionality is required for project, hence following design is proposed.

## Terms and Definitions

- `Event` – an entry of some action registered in system
- `Dispatching` – action of causing an `Event`. Hence, `User` who caused `Event` is the one who dispatched it.

### User Story

#### Entity history

`User` actions operate the same way as before. `User` can create `Tracking Logs`, `Tracking Log Statuses`, `Tracking Log Entries`, update them and even delete them.

For every separate entity `User` may request its history. For specified entities it would look like this:

- `Tracking Log`
  - Created at <> by `User` with following data: <>
  - Updated at <> by `User` with following data: <>
  - ...
- `Tracking Log Status`
  - Created at <> by `User` with following data: <>
- `Tracking Log Entry`
  - Created at <> by `User` with following data: <>
  - Updated at <> by `User` with following data: <>
  - ...

Deleting entity may lead to deletion of all its `Events` if entity is allowed to be deleted. Some entities may have deletion disabled. For purposes of this document, rules to determine if entity may or ay not be deleted are not considered as they are a detail of implementation of domain rules.

Every registered action is considered an `Event`. `Event` should store following data:

- What entity `Event` is related to
- Who dispatched `Event`
- What type of action `Event` is associated with: creation, editing, etc.

Reading history (all `Event` of certain entity) is allowed to those with read permission on requested entity.

### Implementation Details

### Database model

`Events` are persisted in the same database as the main application. If required, they may be extracted into different persistence model.

- New table `Event` will describe all registered `Events`:
  - `GUID` or `int` `Id` is proposed to be a surrogate PK of table. However, explicit natural (business) key would be also used. This attribute is a matter of change depending on results of implementation. **TODO: revisit decision**.
  - `int` `EntityType`, `EntityId` and `EntityVersion` are used to identify what entity `Event` applies to. `EnitityType` would reference a number registered as an id but not on the level of entity instance, but whole entity group itself. For example, number `1` may refer to `Tracking Log` with `EntityId` value of `56` referring to `Tracking Log` with id `56`. `EntityVersion` is used to separate multiple `Events` applied to the same entity in sequential order.
  - `JSON` `Data` is a serialized representation of `Event`. It allows to use the same table for all entities without redesigning its attributes. If serialization becomes a performance bottleneck, creating separate table with predefined columns may be considered.
  - `int` `DispatchedByUserId`, `datetime` `DispatchedAt` and `GUID` `CorrelationId` are used for proper observability of `Event`. `DispatchedByUserId` would be a FK to `User` table, and `CorrelationId` would be created on application side during logging.

### API

TBA