using OpenQA.Selenium;
using Reqnroll;
using NUnit.Framework;
using TestAutomation.project.tests.Pages;

namespace TestAutomation.project.tests
{
    [Binding]
    internal class StepDefinition
    {
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver _driver => _scenarioContext.Get<IWebDriver>("WebDriver");
        private HomePage homePage;

        public StepDefinition(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario(Order = 1)]
        public void BeforeScenario()
        {
            // Initialize or create a test entry for this scenario
            ReportManager.CreateTest(_scenarioContext.ScenarioInfo.Title);
            // retrieve page objects created by Hooks
            if (_scenarioContext.ContainsKey("HomePage"))
                homePage = (HomePage)_scenarioContext["HomePage"];
            else
                homePage = new HomePage(_driver);
        }

        [AfterStep]
        public void AfterStep()
        {
            var stepInfo = _scenarioContext.StepContext.StepInfo;
            if (_scenarioContext.TestError == null)
            {
                ReportManager.LogInfo($"{stepInfo.StepDefinitionType}: {stepInfo.Text}");
            }
            else
            {
                // Capture screenshot on failure and attach to report
                try
                {
                    var file = SaveScreenshot(_driver, _scenarioContext.ScenarioInfo.Title);
                    ReportManager.LogFail(_scenarioContext.TestError.Message);
                    ReportManager.AddScreenCapture(file);
                }
                catch
                {
                    // swallow to avoid masking original test error
                }
            }
        }

        [AfterScenario(Order = 0)]
        public void AfterScenario()
        {
            ReportManager.Flush();
        }

        [Given("I open the MatchingEngine homepage")]
        public void GivenIOpenTheMatchingEngineHomepage()
        {
            homePage.Open();
        }
        [Given("I accept cookies")]
        public void GivenIAcceptCookies()
        {
            homePage.AcceptCookies();
        }

        [When("I expand the Solutions menu in the header")]
        public void WhenIExpandTheSolutionsMenuInTheHeader()
        {
            homePage.ExpandSolutions();
        }

        [Then("I should see a list of Solutions displayed")]
        public void ThenIShouldSeeAListOfSolutionsDisplayed()
        {
            if (!homePage.IsSolutionsListDisplayed())
                throw new Exception("Solutions list was not displayed.");

            // assert number of visible items and their exact texts
            var expected = new[]
            {
                "Music and copyright solutions",
                "Repertoire management",
                "Repertoire and usage matching",
                "Data ingestion and integration",
                "Distribution processing",
                "Member management",
                "Member self service"
            };

            var texts = homePage.GetSolutionsMenuItemTexts();
            Assert.AreEqual(expected.Length, texts.Count, $"Expected {expected.Length} solution items but found {texts.Count}");
            CollectionAssert.AreEqual(expected, texts);
        }

        [When("I click \"Distribution Processing\" from the Solutions list")]
        public void WhenIClickDistributionProcessingFromTheSolutionsList()
        {
            homePage.ClickDistributionProcessing();
        }

        [When("I scroll to the \"All-in-one solution for scale\" section")]
        public void WhenIScrollToTheAllInOneSolutionForScaleSection()
        {
            homePage.ScrollToAllInOneSection();
        }

        [Then("I should see content present in the \"All-in-one solution for scale\" section")]
        public void ThenIShouldSeeContentPresentInTheAllInOneSolutionForScaleSection()
        {
            if (!homePage.HasAllInOneSectionContent())
                throw new Exception("Expected content in the section but none was found.");
        }

        private static string SaveScreenshot(IWebDriver driver, string scenarioName)
        {
            if (driver is not ITakesScreenshot screenshotDriver)
                throw new InvalidOperationException("Driver does not support screenshots");

            var screenshotsDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResults", "Screenshots");
            Directory.CreateDirectory(screenshotsDir);
            var safeName = string.Join("_", scenarioName.Split(Path.GetInvalidFileNameChars()));
            var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{safeName}.png";
            var filePath = Path.Combine(screenshotsDir, fileName);
            var ss = screenshotDriver.GetScreenshot();
            ss.SaveAsFile(filePath, ScreenshotImageFormat.Png);
            return filePath;
        }
    }
}
