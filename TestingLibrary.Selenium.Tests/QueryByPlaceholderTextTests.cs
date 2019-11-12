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
  public class QueryByPlaceholderTextTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task FindAllByPlaceholderText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindAllByPlaceholderTextAsync("FindAllByPlaceholderText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindAllByPlaceholderText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<input type='text' placeholder='FindAllByPlaceholderText_Success' />
<input type='text' placeholder='FindAllByPlaceholderText_Success' />
        ", TimeSpan.FromSeconds(1));
        IEnumerable<IWebElement> results = await driver.FindAllByPlaceholderTextAsync("FindAllByPlaceholderText_Success");
        Assert.AreEqual(2, results.Count());
      }
    }

    [TestMethod]
    public async Task FindByPlaceholderText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindByPlaceholderTextAsync("FindByPlaceholderText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindByPlaceholderText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<input type='text' placeholder='FindByPlaceholderText_Fail_Multiple' />
<input type='text' placeholder='FindByPlaceholderText_Fail_Multiple' />
        ", TimeSpan.FromSeconds(1));
        await Assert.ThrowsExceptionAsync<MultipleElementsFoundException>(() => driver.FindByPlaceholderTextAsync("FindByPlaceholderText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public async Task FindByPlaceholderText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml("<input type='text' placeholder='FindByPlaceholderText_Success' />", TimeSpan.FromSeconds(1));
        IWebElement result = await driver.FindByPlaceholderTextAsync("FindByPlaceholderText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetAllByPlaceholderText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetAllByPlaceholderText("GetAllByPlaceholderText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetAllByPlaceholderText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input type='text' placeholder='GetAllByPlaceholderText_Success' />
<input type='text' placeholder='GetAllByPlaceholderText_Success' />
        ");
        IEnumerable<IWebElement> results = driver.GetAllByPlaceholderText("GetAllByPlaceholderText_Success");
        Assert.AreEqual(2, results.Count());
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetByPlaceholderText("GetByPlaceholderText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input type='text' placeholder='GetByPlaceholderText_Fail_Multiple' />
<input type='text' placeholder='GetByPlaceholderText_Fail_Multiple' />
        ");
        Assert.ThrowsException<MultipleElementsFoundException>(() => driver.GetByPlaceholderText("GetByPlaceholderText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Fail_ThrowsWhenNormalizerOptionsInvalid()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByPlaceholderText("GetByPlaceholderText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          CollapseWhitespace = true,
          Normalizer = (text) => text,
        }));
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByPlaceholderText("GetByPlaceholderText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          Trim = true,
          Normalizer = (text) => text,
        }));
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' placeholder='GetByPlaceholderText_Success' />");
        IWebElement result = driver.GetByPlaceholderText("GetByPlaceholderText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Success_ExactFalse()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' placeholder='Foo GetByPlaceholderText_success_exactfalse bar' />");
        IWebElement result = driver.GetByPlaceholderText("GetByPlaceholderText_Success_ExactFalse", new QueryByAttributeOptions
        {
          Exact = false,
        });
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Success_MatcherFunction()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input id='target' type='text' placeholder='GetByPlaceholderText_Success_MatcherFunction' />
<input type='text' placeholder='foo' />
");
        MatcherFunction matcher = (text, node) => node.GetAttribute("placeholder").ToLower() == "getbyplaceholdertext_success_matcherfunction";
        IWebElement result = driver.GetByPlaceholderText(matcher);
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Success_Normalizer()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input id='target' type='text' placeholder='GetByPlaceholderText_Success_Normalizer' />
<input type='text' placeholder='foo' />
");
        IWebElement result = driver.GetByPlaceholderText("getbyplaceholdertext_success_normalizer", new QueryByAttributeOptions
        {
          Normalizer = text => text.ToLower(),
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Success_Regex()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' placeholder='GetByPlaceholderText_Success_Regex' />");
        IWebElement result = driver.GetByPlaceholderText(new Regex("GetByPlaceholderText.*"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByPlaceholderText_Success_Within()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='parent'>
  <input id='target' type='text' placeholder='GetByPlaceholderText_Success_Within' />
</div>
<input type='text' placeholder='GetByPlaceholderText_Success_Within' />
        ");
        IWebElement parent = driver.FindElement(By.CssSelector("#parent"));
        IWebElement result = parent.GetByPlaceholderText("GetByPlaceholderText_Success_Within");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }
  }
}
