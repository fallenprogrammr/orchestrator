using System;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using PlayerCore;

namespace OrcPlayer {
    class Program {

        private static string browser;
        private static string player;
        private static IWebDriver browserDriver;
        static void Main(string[] args) {
            if (args.Count()<1) {
                ShowHelpMessage();
            }
            var tinyArgs = new Tiny(args);
            if (!ArgsAreValid(tinyArgs)) {
                ShowHelpMessage();
            }
            else {
                PlayScenario();
            }
        }

        private static void PlayScenario() {
            try {
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
                    browserDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                }

                var playerPlugin = Assembly.LoadFile(player);
                IWebPlayer playerExecution = null;
                foreach (Type type in playerPlugin.GetTypes()) {
                    if (type.GetInterfaces().Any(x => x == typeof(IWebPlayer))) {
                        playerExecution = Activator.CreateInstance(type) as IWebPlayer;
                        break;
                    }
                }
                if (playerExecution == null) {
                    Console.WriteLine("Cannot create instance of the web player plugin '" + player + "'.");
                }
                else {
                    playerExecution.Execute(browserDriver);
                }
            }
            finally {
                if (browserDriver!=null) {
                    browserDriver.Quit();
                }
            }
        }

        private static bool ArgsAreValid(Tiny tinyArgs) {
            try {
                browser = tinyArgs.Arguments.browser.ToLower();
                player = tinyArgs.Arguments.player;
            }
            catch (Exception) {
                return false;
            }
            if (!(browser.Contains("chrome") || browser.Contains("ie") || browser.Contains("firefox") || browser.Contains("phantomjs"))) {
                Console.WriteLine("The browsers argument passed does not contain a recognized browser.");
                return false;
            }
            
            if (!File.Exists(player)) {
                Console.WriteLine("The player argument passed contains a file that cannot be found.");
                return false;
            }
            return true;
        }


        private static void ShowHelpMessage() {
            Console.WriteLine("orcplayer usage:");
            Console.WriteLine("orcplayer browser:<chrome,ie,firefox,phantomjs> player:<path to the player dll file>");
            Console.WriteLine("parameters explained:");
            Console.WriteLine("browser - Browser for running the player dll scenario, example: browser:ie or browser:firefox.");
            Console.WriteLine("player: Path of the player dll to execute the scenario(s) for the web application.");
        }

    }
}
