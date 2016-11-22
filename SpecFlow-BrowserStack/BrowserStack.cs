using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.Specialized;
using System.Diagnostics;
using BrowserStack;

namespace SpecFlow_BrowserStack
{
    [Binding]
    public sealed class BrowserStack
    {
        private IWebDriver _driver;
        private Local _browserStackLocal;

        [BeforeScenario]
        public void BeforeScenario()
        {
            var username = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME") ??
                          ConfigurationManager.AppSettings.Get("user");

            var accesskey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY") ??
                            ConfigurationManager.AppSettings.Get("key");

            if (Process.GetProcessesByName("BrowserStackLocal").Length == 0)
            {
                _browserStackLocal = new Local();
                var bsLocalArgs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("key", accesskey)
                };
                _browserStackLocal.start(bsLocalArgs);
            }

            var capabilities = new DesiredCapabilities();
            var caps = ConfigurationManager.GetSection("capabilities") as NameValueCollection;

            if (caps != null)
                foreach (var key in caps.AllKeys)
                {
                    capabilities.SetCapability(key, caps[key]);
                }
            capabilities.SetCapability("name", ScenarioContext.Current.ScenarioInfo.Title);
            capabilities.SetCapability("project", "Specflow-BrowserStack");

            capabilities.SetCapability("browserstack.user", username);
            capabilities.SetCapability("browserstack.key", accesskey);

            _driver = new RemoteWebDriver(new Uri(ConfigurationManager.AppSettings["server"]), capabilities);
            _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));
            if (caps?.Get("os") != null && (caps.Get("os").ToLower().Equals("windows") || caps.Get("os").ToLower().Equals("os x")))
                _driver.Manage().Window.Maximize();
            ScenarioContext.Current["driver"] = _driver;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _driver?.Quit();
            _browserStackLocal?.stop();
        }
    }
}
