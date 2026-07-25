# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [5.0.0] - 2026-07-25

### Breaking Changes
- Changed runtime and editor namespaces to the `ParkMinPackages.Foundation` convention.
- Moved workflow-specific actors, bindings, build settings, and project bootstrap utilities to Workflow.Default.

### Added
- Added a dedicated Foundation editor assembly.
- Added `DontDestroyOnLoadGameObject` as a reusable Unity component.

### Fixed
- Reworked the script icon window with safe selection validation, responsive layout, icon removal, and reliable importer updates.
## [3.0.1] - 2026-07-25

### Added
- Added SceneExtensions for finding components within loaded Unity scenes.

## [3.0.0] - 2026-07-25

### Breaking Changes
- Reorganized Runtime scripts and updated namespaces to match the new folder structure.

## [2.0.0] - 2026-07-25

### Breaking Changes
- Renamed public namespaces and assembly definitions from Mutant to ParkMinPackages.
- Projects using the previous namespaces or assembly names must update their references.

## [0.1.0] - 2026-04-06

### This is the first release of *\<Expansion\>*.

*Short description of this release*
