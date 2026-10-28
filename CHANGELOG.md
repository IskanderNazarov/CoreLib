# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.1.0] - 2026-10-10
### Added
- Added `ILiveEventsService` for handling in-game events.
- Added `LiveEventsService_GP` implementation for GamePush SDK integration.
- Added `LiveEventsService_PG` stub implementation for Playgama.
- Added `LiveEventsService_Editor` mock implementation for testing events within the Unity Editor.

## [1.1.1] - 2026-10-10
### Changed
- Fixed installing serivce for gp and pg. Binded ILiveEventService to its different implementtions.

## [1.1.2] - 2026-10-17
### Added
- Searchable AdvancedDropdown for TranslationKeyAttribute property drawer with search filter, mouse scrolling, and alphabetical sorting.

## [1.1.5] - 2026-10-28
### Fixed
- Fixed Purchaser_PG: initialized productsInfo list by default and added null-safe check in GetProdInfoByID to prevent ArgumentNullException when payments are unsupported or uninitialized.

## [1.1.4] - 2026-10-26
### Changed
- Fixed `DataSaver_GP.Load` to read synchronized player data after `GP_Init.isReady` without calling `GP_Player.Load()` (prevents extra paid server requests, 5s timeout, and overwriting platform/local fields).
- Added `IsInitialized` guard in `SaveManager<T>` to prevent saving default data before initialization completes.
- Added optional `saveImmediate` flag to `IBoosterInventory` and `BoosterInventory`.
