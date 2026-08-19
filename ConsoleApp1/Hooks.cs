using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;

namespace TestAutomation.project.tests
{
    [Binding]
    internal sealed class Hooks
    {
        private readonly ScenarioContext _scenarioContext;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario()
        {
            // Load configuration (appsettings.json + environment variables)
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var browser = config["TestSettings:Browser"] ?? "Chrome";
            var headless = bool.TryParse(config["TestSettings:Headless"], out var h) && h;
            var baseUrl = config["TestSettings:BaseUrl"] ?? "https://www.matchingengine.com/";
            var timeoutSeconds = int.TryParse(config["TestSettings:DefaultTimeoutSeconds"], out var to) ? to : 10;
            var reportPath = config["TestSettings:ReportPath"] ?? "TestResults/Report.html";

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--no-sandbox");
            if (headless)
                chromeOptions.AddArgument("--headless=new");
            // Remove any stale chromedriver.exe copies from output folders to avoid using an outdated bundled driver
            try
            {
                var baseDir = AppContext.BaseDirectory;
                var currentDir = Directory.GetCurrentDirectory();
                var directoriesToCheck = new[] { baseDir, currentDir };
                foreach (var dir in directoriesToCheck)
                {
                    try
                    {
                        var path = Path.Combine(dir, "chromedriver.exe");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    catch
                    {
                        // ignore file delete failures
                    }
                }

                // Use default ChromeDriverService so Selenium Manager can resolve the correct driver for the installed Chrome
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                var driver = new ChromeDriver(service, chromeOptions);
                if (bool.TryParse(config["TestSettings:MaximizeWindow"], out var maximize) && maximize)
                    driver.Manage().Window.Maximize();

                _scenarioContext.Set<IWebDriver>(driver, "WebDriver");

                // configure report output path before tests start
                ReportManager.SetReportPath(reportPath);

                // create and store page objects for convenience, pass baseUrl and timeout
                var home = new Pages.HomePage(driver, baseUrl, TimeSpan.FromSeconds(timeoutSeconds));
                _scenarioContext["HomePage"] = home;
            }
            catch (WebDriverException ex)
            {
                throw new InvalidOperationException(
                    "ChromeDriver startup failed. Ensure no older chromedriver.exe is present in the test output and that Selenium Manager can download the matching driver for Chrome v151. If you are offline, place a matching chromedriver.exe on PATH.", ex);
            }
        }

        [AfterScenario(Order = 1)]
        public void AfterScenario()
        {
            if (_scenarioContext.TryGetValue<IWebDriver>("WebDriver", out var driver))
            {
                try
                {
                    driver.Quit();
                    driver.Dispose();
                }
                catch
                {
                    // swallow exceptions on cleanup
                }
            }
        }
    }
}
