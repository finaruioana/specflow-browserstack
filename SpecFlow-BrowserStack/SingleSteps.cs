using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace SpecFlow_BrowserStack
{
  [Binding]
  public class SingleSteps
  {
    private IWebDriver _driver;

    public SingleSteps()
    {
            _driver = (IWebDriver)ScenarioContext.Current["driver"];
    }

    [Given(@"I am on the google page")]
    public void GivenIAmOnTheGooglePage()
    {
      _driver.Navigate().GoToUrl("https://devtest.likelyloans.com/");
    }

    [When(@"I search for ""(.*)""")]
    public void WhenISearchFor(string keyword)
    {
      var q = _driver.FindElement(By.XPath("//button[@type='submit']"));
      q.Click();
    }

        [Then(@"I get search results")]
        public void ThenIGetSearchResults()
        {
            Assert.That(_driver.Url, Is.EqualTo("https://devtest.likelyloans.com/apply?LoanAmount=2000&LoanTermInMonths=24"));
        }

    }
}
