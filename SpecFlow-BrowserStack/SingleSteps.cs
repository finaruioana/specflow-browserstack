using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace SpecFlow_BrowserStack
{
    [Binding]
    public class SingleSteps
    {
        private readonly IWebDriver _driver;
        

        public SingleSteps()
        {
            _driver = (IWebDriver)ScenarioContext.Current["driver"];
        }

        [Given(@"I am on the google page")]
        public void GivenIAmOnTheGooglePage()
        {
            _driver.Navigate().GoToUrl("https://www.google.com/");
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string keyword)
        {
            var q = _driver.FindElement(By.Name("q"));
            q.SendKeys(keyword);
            q.Submit();
        }

        [Then(@"I should see title ""(.*)""")]
        public void ThenIShouldSeeTitle(string title)
        {
            Thread.Sleep(5000);
            Assert.That(_driver.Title, Is.EqualTo(title));
        }
    }
}
