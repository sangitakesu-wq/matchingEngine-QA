using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TestAutomation.project.tests.Pages
{
    internal abstract class BasePage
    {
        protected IWebDriver Driver { get; }
        protected WebDriverWait Wait { get; }

        protected BasePage(IWebDriver driver, TimeSpan? timeout = null)
        {
            // Require an initialized IWebDriver from test setup (Hooks or factory)
            if (driver == null)
                throw new ArgumentNullException(nameof(driver), "IWebDriver must be provided by test setup.");

            Driver = driver;
            Wait = new WebDriverWait(Driver, timeout ?? TimeSpan.FromSeconds(10));
        }

        protected IWebElement Find(By locator)
        {
            var timeout = TimeSpan.FromSeconds(10);
            var end = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow <= end)
            {
                try
                {
                    var el = Driver.FindElement(locator);
                    if (el != null)
                        return el;
                }
                catch (NoSuchElementException)
                {
                    // ignore and retry until timeout
                }
                catch (StaleElementReferenceException)
                {
                    // try again
                }

                Thread.Sleep(200);
            }

            throw new NoSuchElementException($"Element not found: {locator}");
        }

        protected System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindAll(By locator)
        {
            return Driver.FindElements(locator);
        }

        protected void Click(By locator)
        {
            var el = Find(locator);
            el.Click();
        }

        protected void ScrollIntoView(By locator)
        {
            var el = Find(locator);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView();", el);
        }

        // Very basic helpers
        protected void NavigateTo(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        protected void SendKeys(By locator, string text, bool clearFirst = true)
        {
            var el = Find(locator);
            if (clearFirst)
                el.Clear();
            el.SendKeys(text);
        }

        protected string GetText(By locator)
        {
            var el = Find(locator);
            return el.Text?.Trim() ?? string.Empty;
        }

        protected bool IsDisplayed(By locator)
        {
            try
            {
                var el = Find(locator);
                return el.Displayed;
            }
            catch
            {
                return false;
            }
        }

        protected IWebElement? TryFind(By locator, int timeoutSeconds = 2)
        {
            var timeout = TimeSpan.FromSeconds(timeoutSeconds);
            var end = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow <= end)
            {
                try
                {
                    var el = Driver.FindElement(locator);
                    if (el != null)
                        return el;
                }
                catch (NoSuchElementException)
                {
                    // continue polling
                }
                catch (StaleElementReferenceException)
                {
                    // continue polling
                }

                Thread.Sleep(200);
            }

            return null;
        }
    }
}
