# Quickbase Demo

## API Tests in C#

Interaction with Quickbase API v1 via HTTPS with requests resembling examples from OAS v2.

## Files

* **GeneratedTests.cs** OpenApiTests output given simplified OAS
* **JsonFileDataAttribute.cs** Serves as xUnit Theory attribute
* **MockoonTests.cs** Mocked API logic
* **NBomberTests.cs** Load testing with time measurements
* **NSwagGenerated.cs** Generated API client
* **NSwagTests.cs** Generates API client given schema
* **OpenApiTests.cs** Traverses paths and operations
* **QuickbaseTests.cs*** Online API calls and assertions
* **Table.cs** GPT generated domain object based on schema example
* **schema.json** Scaled-down version of official Quickbase schema
* **tables.json** Test data divided into cases

## Environment

* **IDE:** Visual Studio 2022
* **xUnit:** 2.9.2
* **.NET SDK:** net9.0
* **Mockoon:** 9.2.0

## Test Execution

All tests in the project can be run with Visual Studio xUnit Runner.