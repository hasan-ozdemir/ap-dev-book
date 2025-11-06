# MAUI UI Test Samples

## Playwright (.NET)
- Install Playwright CLI: DOTNET_ENV=Development dotnet tool update --global Microsoft.Playwright.CLI.
- Navigate to your test project and run playwright install to fetch browsers.
- Execute tests: dotnet test Contoso.Mobile.UiTests.csproj --filter Category=Playwright.

## Appium
- Install Appium Server (
pm install -g appium).
- Start server: ppium --base-path /wd/hub.
- Run tests: dotnet test Contoso.Mobile.UiTests.csproj --filter Category=Appium.

Update project references to match your solution structure before running.

