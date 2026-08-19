# MatchingEngine Automation Assessment

Automated test solution for the MatchingEngine technical assessment, built with Selenium WebDriver, Reqnroll, and C#.

## What this covers

- Visits https://www.matchingengine.com/
- Expands the "Solutions" menu in the header
- Asserts the list of Solutions is displayed
- Clicks "Distribution Processing" from the Solutions list
- Scrolls to the "All-in-one solution for scale" section
- Asserts the section's content is present

Tests run in Google Chrome.

## Tech stack
- Selenium WebDriver (C#)
- Reqnroll (Gherkin/BDD)
- NUnit (test runner)

## How to run

1. Clone the repository
2. Restore dependencies: dotnet restore
3. Run the tests: dotnet test or run through VS


## Project structure

ConsoleApp1/
├── Test.feature # Gherkin scenario
├── StepDefinition.cs # Step implementations
├── Hooks.cs # Driver setup/teardown
├── ReportManager.cs # Test reporting
└── ...

## Reporting
See REPORTING.md for details on generated test reports.
