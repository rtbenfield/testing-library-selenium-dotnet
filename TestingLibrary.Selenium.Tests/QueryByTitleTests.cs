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
  public class QueryByTitleTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task FindAllByTitle_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindAllByTitleAsync("FindAllByTitle_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindAllByTitle_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<div title='FindAllByTitle_Success'></div>
<div title='FindAllByTitle_Success'></div>
        ", TimeSpan.FromSeconds(1));
        IEnumerable<IWebElement> results = await driver.FindAllByTitleAsync("FindAllByTitle_Success");
        Assert.AreEqual(2, results.Count());
      }
    }

    [TestMethod]
    public async Task FindByTitle_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindByTitleAsync("FindByTitle_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindByTitle_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<div title='FindByTitle_Fail_Multiple'></div>
<div title='FindByTitle_Fail_Multiple'></div>
        ", TimeSpan.FromSeconds(1));
        await Assert.ThrowsExceptionAsync<MultipleElementsFoundException>(() => driver.FindByTitleAsync("FindByTitle_Fail_Multiple"));
      }
    }

    [TestMethod]
    public async Task FindByTitle_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"<div title='FindByTitle_Success'></div>", TimeSpan.FromSeconds(1));
        IWebElement result = await driver.FindByTitleAsync("FindByTitle_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetAllByTitle_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetAllByTitle("GetAllByTitle_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetAllByTitle_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div title='GetAllByTitle_Success'></div>
<svg>
  <title>GetAllByTitle_Success</title>
</svg>
        ");
        IEnumerable<IWebElement> results = driver.GetAllByTitle("GetAllByTitle_Success");
        Assert.AreEqual(2, results.Count());
      }
    }

    [TestMethod]
    public void GetByTitle_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetByTitle("GetByTitle_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetByTitle_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div title='GetByTitle_Fail_Multiple'></div>
<div title='GetByTitle_Fail_Multiple'></div>
        ");
        Assert.ThrowsException<MultipleElementsFoundException>(() => driver.GetByTitle("GetByTitle_Fail_Multiple"));
      }
    }

    [TestMethod]
    public void GetByTitle_Fail_ThrowsWhenNormalizerOptionsInvalid()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByTitle("GetByTitle_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          CollapseWhitespace = true,
          Normalizer = (text) => text,
        }));
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByTitle("GetByTitle_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          Trim = true,
          Normalizer = (text) => text,
        }));
      }
    }

    [TestMethod]
    public void GetByTitle_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div title='GetByTitle_Success'></div>");
        IWebElement result = driver.GetByTitle("GetByTitle_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByTitle_Success_ExactFalse()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div title='Foo GetByTitle_success_exactfalse bar'></div>");
        IWebElement result = driver.GetByTitle("GetByTitle_Success_ExactFalse", new QueryByAttributeOptions
        {
          Exact = false,
        });
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByTitle_Success_MatcherFunction()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='target' title='GetByTitle_Success_MatcherFunction'></div>
<div>foo</div>
");
        MatcherFunction matcher = (text, node) => node.GetAttribute("title").ToLower() == "getbytitle_success_matcherfunction";
        IWebElement result = driver.GetByTitle(matcher);
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByTitle_Success_Normalizer()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='target' title='GetByTitle_Success_Normalizer'></div>
<div>foo</div>
");
        IWebElement result = driver.GetByTitle("getbytitle_success_normalizer", new QueryByAttributeOptions
        {
          Normalizer = text => text.ToLower(),
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByTitle_Success_Regex()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div title='GetByTitle_Success_Regex'></div>");
        IWebElement result = driver.GetByTitle(new Regex("GetByTitle.*"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByTitle_Success_Within()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='parent'>
  <div id='target' title='GetByTitle_Success_Within'></div>
</div>
<div title='GetByTitle_Success_Within'></div>
        ");
        IWebElement parent = driver.FindElement(By.CssSelector("#parent"));
        IWebElement result = parent.GetByTitle("GetByTitle_Success_Within");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }
  }
}
