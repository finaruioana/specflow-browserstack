using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.Specialized;
using BrowserStack;
using System.IO;

namespace SpecFlow_BrowserStack
{
    [Binding]
    public sealed class BrowserStack
    {
        private BrowserStackDriver _bsDriver;

        [BeforeScenario]
        public void BeforeScenario()
        {
            _bsDriver = new BrowserStackDriver();
            ScenarioContext.Current["bsDriver"] = _bsDriver;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _bsDriver.Cleanup();
        }
    }


    public class BrowserStackDriver
    {
        private IWebDriver _driver;
        private Local _browserStackLocal;

        public IWebDriver Init(string profile, string environment)
        {
            var caps = ConfigurationManager.GetSection("capabilities/" + profile) as NameValueCollection;
            var settings = ConfigurationManager.GetSection("environments/" + environment) as NameValueCollection;

            var capability = new DesiredCapabilities();


            var logger = Path.Combine(Directory.GetCurrentDirectory(), "sf.log");
            if (caps != null)
                foreach (var key in caps.AllKeys)
                {
                    capability.SetCapability(key, caps[key]);
                }

            if (settings != null)
                foreach (var key in settings.AllKeys)
                {
                    capability.SetCapability(key, settings[key]);
                }

            var username = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME") ??
                           ConfigurationManager.AppSettings.Get("user");

            var accesskey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY") ??
                            ConfigurationManager.AppSettings.Get("key");

            capability.SetCapability("browserstack.user", username);
            capability.SetCapability("browserstack.key", accesskey);

            File.AppendAllText(logger, "Starting local");

            if (capability.GetCapability("browserstack.local") != null && capability.GetCapability("browserstack.local").ToString() == "true")
            {
                _browserStackLocal = new Local();
                var bsLocalArgs = new List<KeyValuePair<string, string>> {
                     new KeyValuePair<string, string>("key", accesskey)
                  };
                _browserStackLocal.start(bsLocalArgs);
            }

            File.AppendAllText(logger, "Starting driver");
            _driver = new RemoteWebDriver(new Uri("http://" + ConfigurationManager.AppSettings.Get("server") + "/wd/hub/"), capability);
            return _driver;
        }

        public void Cleanup()
        {
            _driver.Quit();
            _browserStackLocal?.stop();
        }
    }
}
