using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using TestingLibrary.Selenium.Exceptions;

namespace TestingLibrary.Selenium.Tests
{
  [TestClass]
  public class WaitForElementTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task WaitForElement_Fail()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<WaitForElementException>(async () =>
        {
          await WaitForElement.WaitForElementAsync(() => driver.QueryByText("Hello World"));
        });
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task WaitForElement_Fail_CustomTimeout()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml("<div>Hello World</div>", TimeSpan.FromSeconds(4));
        await Assert.ThrowsExceptionAsync<WaitForElementException>(async () =>
        {
          await WaitForElement.WaitForElementAsync(() => driver.QueryByText("Hello World"), new WaitForElementOptions
          {
            Timeout = TimeSpan.FromMilliseconds(10),
          });
        });
      }
    }

    [TestMethod]
    public async Task WaitForElement_Fail_GetByText()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        await Assert.ThrowsExceptionAsync<ElementException>(async () =>
        {
          await WaitForElement.WaitForElementAsync(() => driver.GetByText("Hello World"));
        });
      }
    }

    [TestMethod]
    public async Task WaitForElement_Fail_NoCallback()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
          await WaitForElement.WaitForElementAsync<object>(null);
        });
      }
    }

    [TestMethod]
    public async Task WaitForElement_Success_GetByText()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml("<div>Hello World</div>", TimeSpan.FromSeconds(1));
        IWebElement result = await WaitForElement.WaitForElementAsync(() => driver.GetByText("Hello World"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public async Task WaitForElement_Success_QueryByText()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml("<div>Hello World</div>", TimeSpan.FromSeconds(1));
        IWebElement result = await WaitForElement.WaitForElementAsync(() => driver.QueryByText("Hello World"));
        Assert.IsNotNull(result);
      }
    }
  }
}