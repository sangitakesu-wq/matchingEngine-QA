using OpenQA.Selenium;

namespace TestAutomation.project.tests.Pages
{
    internal class HomePage : BasePage
    {
        private readonly string _url;
        private readonly By solutionsDropdown = By.CssSelector("span[id=nav-toggle-solutions]");
        private readonly By distributionLink = By.CssSelector("a[aria-label='Distribution processing']");
        private readonly By solutionsMenuContainer = By.CssSelector("div.SPTNavigation_submenuHost__y5nJH.SPTNavigation_submenuOpen__AyMca");
        private readonly By acceptCookies = By.Id("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll");
        private readonly By allInOneHeading = By.CssSelector("h2.Text_text__l6YJt.Text_text--h2__ES1ch.Text_text--left__IIYVU");
        private readonly By allInOneHeadingAlt = By.CssSelector("h2[class=\"Text_text__l6YJt Text_text--h2__ES1ch Text_text--left__IIYVU\"]");

        public HomePage(IWebDriver driver, string? baseUrl = null, TimeSpan? timeout = null) : base(driver, timeout)
        {
            _url = baseUrl ?? "https://www.matchingengine.com/";
        }

        public void Open()
        {
            NavigateTo(_url);
        }

        public void ExpandSolutions()
        {
            Click(solutionsDropdown);
        }

        public bool IsSolutionsListDisplayed()
        {
            // Check that the solutions submenu container is open and visible
            return IsDisplayed(solutionsMenuContainer);
        }

        // Return the count of visible solution items inside the open submenu container
        public int GetSolutionsMenuItemCount()
        {
            var container = TryFind(solutionsMenuContainer, 2);
            if (container == null)
                return 0;

            var items = container.FindElements(By.CssSelector("a, li"));
            return items.Count(e => e.Displayed && !string.IsNullOrWhiteSpace(e.Text));
        }

        // Return the visible texts of solution items in order
        public List<string> GetSolutionsMenuItemTexts()
        {
            var container = TryFind(solutionsMenuContainer, 2);
            if (container == null)
                return new List<string>();

            var items = container.FindElements(By.CssSelector("a, li"));
            return items.Where(e => e.Displayed)
                .Select(e => (e.Text ?? string.Empty).Trim())
                .Where(s => s.Length > 0)
                .ToList();
        }

        public void ClickDistributionProcessing()
        {
            Click(distributionLink);
        }

        public void ScrollToAllInOneSection()
        {
            if (TryFind(allInOneHeading) != null)
                ScrollIntoView(allInOneHeading);
            else if (TryFind(allInOneHeadingAlt) != null)
                ScrollIntoView(allInOneHeadingAlt);
        }

        public bool HasAllInOneSectionContent()
        {
            var h = TryFind(allInOneHeading) ?? TryFind(allInOneHeadingAlt);
            if (h == null)
                return false;

            var text = GetText(h);
            if (!string.IsNullOrEmpty(text))
                return true;

            var parent = TryFind(By.XPath(".."));
            if (parent == null)
                return false;

            return !string.IsNullOrEmpty(GetText(parent));
        }

        // helper overload to use GetText on IWebElement
        private string GetText(IWebElement element)
        {
            return element.Text?.Trim() ?? string.Empty;
        }

        public void AcceptCookies()
        {
            var elements = Driver.FindElements(acceptCookies);
            if (elements == null || elements.Count == 0)
                return;

            var el = elements[0];
            try
            {
                el.Click();
            }
            catch
            {
                try
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", el);
                }
                catch
                {
                    // ignore any errors when trying to accept cookies
                }
            }
        }
    }
}
