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
  public class QueryByAltTextTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task FindAllByAltText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindAllByAltTextAsync("FindAllByAltText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindAllByAltText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<input type='text' alt='FindAllByAltText_Success' />
<img alt='FindAllByAltText_Success' />
<map>
  <area alt='FindAllByAltText_Success' />
</map>
        ", TimeSpan.FromSeconds(1));
        IEnumerable<IWebElement> results = await driver.FindAllByAltTextAsync("FindAllByAltText_Success");
        Assert.AreEqual(3, results.Count());
      }
    }

    [TestMethod]
    public async Task FindByAltText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindByAltTextAsync("FindByAltText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindByAltText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<input type='text' alt='FindByAltText_Fail_Multiple' />
<input type='text' alt='FindByAltText_Fail_Multiple' />
        ", TimeSpan.FromSeconds(1));
        await Assert.ThrowsExceptionAsync<MultipleElementsFoundException>(() => driver.FindByAltTextAsync("FindByAltText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public async Task FindByAltText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml("<input type='text' alt='FindByAltText_Success' />", TimeSpan.FromSeconds(1));
        IWebElement result = await driver.FindByAltTextAsync("FindByAltText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetAllByAltText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetAllByAltText("GetAllByAltText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetAllByAltText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input type='text' alt='GetAllByAltText_Success' />
<img alt='GetAllByAltText_Success' />
<map>
  <area alt='GetAllByAltText_Success' />
</map>
        ");
        IEnumerable<IWebElement> results = driver.GetAllByAltText("GetAllByAltText_Success");
        Assert.AreEqual(3, results.Count());
      }
    }

    [TestMethod]
    public void GetByAltText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetByAltText("GetByAltText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetByAltText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input type='text' alt='GetByAltText_Fail_Multiple' />
<input type='text' alt='GetByAltText_Fail_Multiple' />
        ");
        Assert.ThrowsException<MultipleElementsFoundException>(() => driver.GetByAltText("GetByAltText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public void GetByAltText_Fail_ThrowsWhenNormalizerOptionsInvalid()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByAltText("GetByAltText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          CollapseWhitespace = true,
          Normalizer = (text) => text,
        }));
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByAltText("GetByAltText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          Trim = true,
          Normalizer = (text) => text,
        }));
      }
    }

    [TestMethod]
    public void GetByAltText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' alt='GetByAltText_Success' />");
        IWebElement result = driver.GetByAltText("GetByAltText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByAltText_Success_ExactFalse()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' alt='Foo GetByAltText_success_exactfalse bar' />");
        IWebElement result = driver.GetByAltText("GetByAltText_Success_ExactFalse", new QueryByAttributeOptions
        {
          Exact = false,
        });
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByAltText_Success_MatcherFunction()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input id='target' type='text' alt='GetByAltText_Success_MatcherFunction' />
<input type='text' alt='foo' />
");
        MatcherFunction matcher = (text, node) => node.GetAttribute("alt").ToLower() == "getbyalttext_success_matcherfunction";
        IWebElement result = driver.GetByAltText(matcher);
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByAltText_Success_Normalizer()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input id='target' type='text' alt='GetByAltText_Success_Normalizer' />
<input type='text' alt='foo' />
");
        IWebElement result = driver.GetByAltText("getbyalttext_success_normalizer", new QueryByAttributeOptions
        {
          Normalizer = text => text.ToLower(),
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByAltText_Success_Regex()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' alt='GetByAltText_Success_Regex' />");
        IWebElement result = driver.GetByAltText(new Regex("GetByAltText.*"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByAltText_Success_Within()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='parent'>
  <input id='target' type='text' alt='GetByAltText_Success_Within' />
</div>
<input type='text' alt='GetByAltText_Success_Within' />
        ");
        IWebElement parent = driver.FindElement(By.CssSelector("#parent"));
        IWebElement result = parent.GetByAltText("GetByAltText_Success_Within");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }
  }
}
