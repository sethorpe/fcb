using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
namespace FCBDemo.UI
{
    public class TestClass
    {
        public IWebDriver driver = new ChromeDriver();
        public string url = "https://www.firstcitizens.com";
        //public By Commerical = By.XPath("/html/body/header/div/div[2]/ul/li[3]/a/text()");
        public By Commercial = By.CssSelector("body > header > div > div.fcb-header__nav-desktop > ul > li:nth-child(3) > a");
        public By CAdvantagePanel = By.XPath("//*[@id=\"fcbMainContent\"]/article/div[1]/section/article/div[2]/div");
        public By LearnMore = By.XPath("//*[@id=\"fcbMainContent\"]/article/div[1]/section/article/div[2]/div/div[1]/div[3]/div/a");
        public By TalkToBankerBtn = By.XPath("//*[@id=\"fcbMainContent\"]/article/section/article/section/article/section[1]/article/div[2]/a[1]");
        public By LogInBtn = By.XPath("//*[@id=\"fcbMainContent\"]/article/section/article/section/article/section[1]/article/div[2]/a[2]");
        public By AllBankersTxt = By.CssSelector("#main > div > div.l-container.Main-breadcrumbs.Main-breadcrumbs--desktop > nav > ol > li > span");
        public By userNameField = By.CssSelector("#okta-signin-username");
        public By pwdField = By.CssSelector("#okta-signin-password");
        public By signInBtn = By.CssSelector("#okta-signin-submit");
        public By LoginErrorMsg = By.CssSelector("#form32 > div.o-form-content.o-form-theme.clearfix > div.o-form-error-container.o-form-has-errors > div > div");
        public By Solutions = By.CssSelector("body > header > div > div.fcb-header__nav-desktop > ul > li:nth-child(3) > div > a:nth-child(1) > svg");
        public By CBTileWrapper = By.XPath("//div[@class=\"fcb-tg__tile-wrapper  \"]");
        public By SolutionsTiles = By.XPath("//div[@class=\"fcb-tg__tile-wrapper  \"]//a[@class=\"fcb-tile\"]");
        public By TileTitles = By.XPath("//h3[@class=\"fcb-tile__title fcb-tile__title--long\"]");

        [SetUp]
        public void RunBeforeAll()
        {
            try
            {
                driver.Navigate().GoToUrl(url);
                driver.Manage().Window.Maximize();
                Thread.Sleep(7000);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to launch page", ex);
            }
        }

        [TearDown]
        public void RunAfterAll()
        {
            driver.Quit();
        }

        public void GoToCommericalBanking()
        {
            driver.FindElement(Commercial).Click();
            Thread.Sleep(5000);
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(CAdvantagePanel));
            driver.FindElement(LearnMore).Click();
            Thread.Sleep(5000);
        }
        public string TalkToCABanker()
        {
            driver.FindElement(TalkToBankerBtn).Click();
            Thread.Sleep(3000);
            driver.SwitchTo().Window(driver.WindowHandles[2 - 1]);
            var result = driver.FindElement(AllBankersTxt).Text;
            return result;
        }
        public string LoginToCommercialAdvantage(string user, string pwd)
        {
            driver.FindElement(LogInBtn).Click();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(25));
            driver.SwitchTo().Window(driver.WindowHandles[2 - 1]);
            Thread.Sleep(3000);
            driver.FindElement(userNameField).SendKeys(user + Keys.Tab);
            driver.FindElement(pwdField).SendKeys(pwd + Keys.Tab);
            driver.FindElement(signInBtn).Click();
            Thread.Sleep(5000);
            var result = driver.FindElement(LoginErrorMsg).Text;
            return result;

        }

        public void GoToCBSolutions()
        {
            driver.FindElement(Commercial).Click();
            Thread.Sleep(5000);
            new Actions(driver)
                .MoveToElement(driver.FindElement(Commercial))
                .Pause(TimeSpan.FromSeconds(3))
                .Perform();
            new Actions(driver)
                .MoveToElement(driver.FindElement(Solutions))
                .Click()
                .Perform();
            Thread.Sleep(5000);
        }

        public int CountOfCBSolutions()
        {
            GoToCBSolutions();
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("arguments[0].scrollIntoView()", driver.FindElement(CBTileWrapper));
            Thread.Sleep(5000);
            IList<IWebElement> solutions = driver.FindElements(TileTitles);
            Console.WriteLine($"The count of tiles is: {solutions.Count}");
            return solutions.Count;

        }

        public string GetTextJavascriptExecutor(IWebDriver driver, By element)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(element));
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            string text = ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].value;", driver.FindElement(element)).ToString();
            return text;
        }
        [Test]
        public void Test1()
        {
            Assert.That(driver.Title == "Home | TransLink");
        }
        [TestCase("All Bankers")]
        public void Test2(string value)
        {
            var data = value;
            GoToCommericalBanking();
            var result = TalkToCABanker();
            Assert.AreEqual(data, result);
        }
        [TestCase("BTech.Seyi", "Test1234!")]
        public void Test3(string user, string pwd)
        {
            GoToCommericalBanking();
            var result = LoginToCommercialAdvantage(user, pwd);
            Assert.AreEqual("Login Success!", result);
        }
        [Test]
        public void TestCBSolutions()
        {
            GoToCBSolutions();

        }
        [Test]
        public void TestCountOfCBSolutions()
        {
            CountOfCBSolutions();
        }
    }
}

