using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using TestingLibrary.Selenium.Exceptions;

namespace TestingLibrary.Selenium.Tests
{
  [TestClass]
  public class QueryByTestIdTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task FindAllByTestId_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindAllByTestIdAsync("FindAllByTestId_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindAllByTestId_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<div data-testid='FindAllByTestId_Success'></div>
<div data-testid='FindAllByTestId_Success'></div>
        ", TimeSpan.FromSeconds(1));
        IEnumerable<IWebElement> results = await driver.FindAllByTestIdAsync("FindAllByTestId_Success");
        Assert.AreEqual(2, results.Count());
      }
    }

    [TestMethod]
    public async Task FindByTestId_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindByTestIdAsync("FindByTestId_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindByTestId_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<div data-testid='FindByTestId_Fail_Multiple'></div>
<div data-testid='FindByTestId_Fail_Multiple'></div>
        ", TimeSpan.FromSeconds(1));
        await Assert.ThrowsExceptionAsync<MultipleElementsFoundException>(() => driver.FindByTestIdAsync("FindByTestId_Fail_Multiple"));
      }
    }

    [TestMethod]
    public async Task FindByTestId_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"<div data-testid='FindByTestId_Success'></div>", TimeSpan.FromSeconds(1));
        IWebElement result = await driver.FindByTestIdAsync("FindByTestId_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetAllByTestId_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetAllByTestId("GetAllByTestId_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetAllByTestId_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div data-testid='GetAllByTestId_Success'></div>
<div data-testid='GetAllByTestId_Success'></div>
        ");
        IEnumerable<IWebElement> results = driver.GetAllByTestId("GetAllByTestId_Success");
        Assert.AreEqual(2, results.Count());
      }
    }

    [TestMethod]
    public void GetByTestId_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetByTestId("GetByTestId_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetByTestId_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div data-testid='GetByTestId_Fail_Multiple'></div>
<div data-testid='GetByTestId_Fail_Multiple'></div>
        ");
        Assert.ThrowsException<MultipleElementsFoundException>(() => driver.GetByTestId("GetByTestId_Fail_Multiple"));
      }
    }

    [TestMethod]
    public void GetByTestId_Fail_ThrowsWhenNormalizerOptionsInvalid()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByTestId("GetByTestId_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          CollapseWhitespace = true,
          Normalizer = (text) => text,
        }));
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByTestId("GetByTestId_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          Trim = true,
          Normalizer = (text) => text,
        }));
      }
    }

    [TestMethod]
    public void GetByTestId_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div data-testid='GetByTestId_Success'></div>");
        IWebElement result = driver.GetByTestId("GetByTestId_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByTestId_Success_ExactFalse()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div data-testid='Foo GetByTestId_success_exactfalse bar'></div>");
        IWebElement result = driver.GetByTestId("GetByTestId_Success_ExactFalse", new QueryByAttributeOptions
        {
          Exact = false,
        });
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByTestId_Success_MatcherFunction()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='target' data-testid='GetByTestId_Success_MatcherFunction'></div>
<div>foo</div>
");
        MatcherFunction matcher = (text, node) => node.GetAttribute("data-testid").ToLower() == "getbytestid_success_matcherfunction";
        IWebElement result = driver.GetByTestId(matcher);
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByTestId_Success_Normalizer()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='target' data-testid='GetByTestId_Success_Normalizer'></div>
<div>foo</div>
");
        IWebElement result = driver.GetByTestId("getbytestid_success_normalizer", new QueryByAttributeOptions
        {
          Normalizer = text => text.ToLower(),
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByTestId_Success_Regex()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div data-testid='GetByTestId_Success_Regex'></div>");
        IWebElement result = driver.GetByTestId(new Regex("GetByTestId.*"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByTestId_Success_Within()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='parent'>
  <div id='target' data-testid='GetByTestId_Success_Within'></div>
</div>
<div data-testid='GetByTestId_Success_Within'></div>
        ");
        IWebElement parent = driver.FindElement(By.CssSelector("#parent"));
        IWebElement result = parent.GetByTestId("GetByTestId_Success_Within");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }
  }
}
