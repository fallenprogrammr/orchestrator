using System;
using System.Threading;
using OpenQA.Selenium;
using PlayerCore;

namespace SamplePlayer
{
    public class Player : IWebPlayer
    {
        public void Execute(IWebDriver browser) {
            browser.Url = "http://www.google.com";
            var searchTextBox = browser.FindElement(By.CssSelector("#gbqfq"));
            searchTextBox.SendKeys("unicorns");
            var searchButton = browser.FindElement(By.CssSelector("#gbqfba"));
            searchButton.Click();
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}
