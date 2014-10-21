using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using PlayerCore;

namespace Orchestrator {
    partial class Program {
        private static Assembly playerPlugin;
        private static List<IWebDriver> webDrivers;
        static void Orchestrate() {
            Type iWebPlayerType = null;
            playerPlugin = Assembly.LoadFile(player);
            foreach (Type type in playerPlugin.GetTypes()) {
                if (type.GetInterfaces().Any(x => x == typeof(IWebPlayer))) {
                    iWebPlayerType = type;
                    break;
                }
            }
            webDrivers = new List<IWebDriver>();
            if (iWebPlayerType != null) {
                ReleaseTheKraken(iWebPlayerType);
            }
        }

        private static void ReleaseTheKraken(Type iWebPlayerType) {
            var individualBrowsers = browsers.Split(',');
            try {
                Parallel.ForEach(individualBrowsers, browser => Parallel.For(0,instances, x => Kraken(iWebPlayerType, browser)));
            }
            finally {
                foreach (var webDriver in webDrivers) {
                    webDriver.Quit();
                }
            }
        }

        private static void Kraken(Type iWebPlayerType, string browser) {
            int i;
            switch (browser) {
                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("test-type");
                    browserDriver = new ChromeDriver(chromeOptions);
                    break;
                case "ie":
                    browserDriver = new InternetExplorerDriver();
                    break;
                case "firefox":
                    browserDriver = new FirefoxDriver();
                    break;
                case "phantomjs":
                    browserDriver = new PhantomJSDriver();
                    break;
                default:
                    browserDriver = null;
                    break;
            }
            if (browserDriver != null) {
                webDrivers.Add(browserDriver);
                browserDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
                var playerExecution = Activator.CreateInstance(iWebPlayerType) as IWebPlayer;
                if (playerExecution != null) {
                    playerExecution.Execute(browserDriver);
                }
            }
        }
    }
}
