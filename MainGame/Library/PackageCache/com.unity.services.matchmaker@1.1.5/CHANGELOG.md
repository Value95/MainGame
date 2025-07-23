# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.1.5] - 2024-07-11
* Fix ABTesting overrides on non-default environment

## [1.1.4] - 2024-04-30
* Updated content of apple privacy manifest

## [1.1.3] - 2024-04-30
* Added apple privacy manifest
* Updated com.unity.services.core to 1.12.5
* Updated com.unity.services.authentication to 2.7.4

## [1.1.2] - 2023-10-16
* Updated backfill approval request timeout.

## [1.1.1] - 2023-05-17
* Add missing error handling for backfill update.
* Updated Core to 1.8.2
* Fix create ticket response accepting pool ID

## [1.1.0] - 2023-03-07
* Add support for tickets with MatchId assignments.
* [Closed Beta] Adding support for AB testing.

## [1.0.0-pre.14] - 2022-11-25
* Bugfix: In-Package sample was breaking on WebGL builds due to using Task.Delay, this has been changed with a Coroutine implementation on polling.
* MatchmakingResults model has new property 'PoolId'.

## [1.0.0-pre.13] - 2022-08-24
* Bugfix: Serialization for the Multiplay token class would be stripped on some IL2CPP platforms compilation. 

## [1.0.0-pre.12] - 2022-07-25
* Bugfix: Serialization for the Multiplay token class would be stripped on some Linux platforms compilation.
* Added PoolId and MatchId to CreateBackfillTicketOptions for Photon integration.

## [1.0.0-pre.11] - 2022-06-23
* MatchId added to MatchmakingResults payload.

## [1.0.0-pre.10] - 2022-06-23
* MatchId field added to MultiplayAssignment for Photon.
* Updated required fields for model classes (Player, Team).
* Updated documentation.

## [1.0.0-pre.9] - 2022-05-27
* Added QoS results integration for server region allocation.
* Added PayloadAllocation model for integration with Multiplay SDK (or API) to serialize Matchmaker payload data.
* Updated Auth, Core & Newtonsoft.Json dependencies.
* IDynamicObject has been replaced with IDeserializable.
* Dashboard URLs added to package (accessible via package manager on editor version 2022 and onwards).
* Fixed infinite polling issue when timing out in sample.
* Replaced coroutine usage with task usage in sample.

## [1.0.0-pre.8] - 2022-03-09
* Updated MultiplayAssignment Status member to an enum instead of a string
* Updated BackfillTicketProperties with new Player list member.

## [1.0.0-pre.7] - 2022-03-01
* Added Backfill API and implemented interface / wrapper for associated functionality.
* Removed QoSResult and CustomData from CreateTicketRequest.
* **Breaking Change!** Renamed IMatchmakerSdkService to IMatchmakerService and Matchmaker to MatchmakerService. SDK is now accessed via "MatchmakerService.Instance" instead of "Matchmaker.Instance".
* Updated Auth and Core dependencies.

## [1.0.0-pre.4] - 2022-02-10

* Added EventPollingManualTestScene as SDK Sample (Pacman)
* Updated to Tickets API to contract v2. 
* Updated associated code (Sample, Tests, Wrapper) for Tickets API contract v2.

## [1.0.0-pre.3] - 2022-01-20
 
* Updated project with Tickets v2 API generated code.
* Applied changes to Wrapper and TestingScene scripts for compatibility with Tickets v2.
* Updated git patches.
* TestStubs removed and replaced with Moq testing.
* IDynamicObject for flexible types in SDK Generated Models.
* Updated error code rage to match Operate RFCs.
* Changed regenerate-sdh.sh to load from a local copy of Ticket v2 MM OpenAPI spec that has fixes.
* Removed utilities relating to projectId and envId in API endpoint.

## [1.0.0-pre.2] - 2021-12-22

* This is an internally-only used iteration of the Matchmaker SDK.
* Updated TestProject with event-polling for retrieving match details.
* Fixed bug in previous version of wrapper causing [requests to always timeout](https://jira.unity3d.com/browse/MPSSDK-113).
* Fixed bug with OpenAPI and wrapper where in certain cases [errors would not show when an exception was thrown](https://jira.unity3d.com/browse/MPSSDK-114).
* Added patch files and patching behaviour to regeneration script to fix issues related to generated API code.
* Updated SDK wrapper to expose parameters for creating a matchmaking ticket with custom parameters.
* Added Tickets v1 API compatibility for converting C# models to byte[] types as required. 
* Updated SDK Generator version (0.8.0 -> 0.9.0).

## [1.0.0-pre.1] - 2021-12-03

* This is an internally-only used iteration of the Matchmaker SDK
* This version demonstrates testable matchmaking ticket processing and allocation
* Added TestProject with scene to demo Ticket API functionality
* Added gitignore
* Setup default documentation and licence files
