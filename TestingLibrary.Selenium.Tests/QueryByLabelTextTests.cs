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
  public class QueryByLabelTextTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task FindAllByLabelText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindAllByLabelTextAsync("FindAllByLabelText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindAllByLabelText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<label>
  FindAllByLabelText_Success
  <input type='text' />
</label>
<label for='input1'>FindAllByLabelText_Success</label>
<input type='text' id='input1' />
<label id='label1'>FindAllByLabelText_Success</label>
<input type='text' aria-labelledby='label1' />
<button type='button' aria-label='FindAllByLabelText_Success'></button>
        ", TimeSpan.FromSeconds(1));
        IEnumerable<IWebElement> results = await driver.FindAllByLabelTextAsync("FindAllByLabelText_Success");
        Assert.AreEqual(4, results.Count());
      }
    }

    [TestMethod]
    public async Task FindByLabelText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindByLabelTextAsync("FindByLabelText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindByLabelText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<button type='button' aria-label='FindByLabelText_Fail_Multiple'></button>
<button type='button' aria-label='FindByLabelText_Fail_Multiple'></button>
        ", TimeSpan.FromSeconds(1));
        await Assert.ThrowsExceptionAsync<MultipleElementsFoundException>(() => driver.FindByLabelTextAsync("FindByLabelText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public async Task FindByLabelText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml("<button type='button' aria-label='FindByLabelText_Success'></button>", TimeSpan.FromSeconds(1));
        IWebElement result = await driver.FindByLabelTextAsync("FindByLabelText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetAllByLabelText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetAllByLabelText("GetAllByLabelText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetAllByLabelText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<label>
  GetAllByLabelText_Success
  <input type='text' />
</label>
<label for='input1'>GetAllByLabelText_Success</label>
<input type='text' id='input1' />
<label id='label1'>GetAllByLabelText_Success</label>
<input type='text' aria-labelledby='label1' />
<button type='button' aria-label='GetAllByLabelText_Success'></button>
        ");
        IEnumerable<IWebElement> results = driver.GetAllByLabelText("GetAllByLabelText_Success");
        Assert.AreEqual(4, results.Count());
      }
    }

    [TestMethod]
    public void GetByLabelText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetByLabelText("GetByLabelText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetByLabelText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<button type='button' aria-label='GetByLabelText_Fail_Multiple'></button>
<button type='button' aria-label='GetByLabelText_Fail_Multiple'></button>
        ");
        Assert.ThrowsException<MultipleElementsFoundException>(() => driver.GetByLabelText("GetByLabelText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public void GetByLabelText_Fail_ThrowsWhenNormalizerOptionsInvalid()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByLabelText("GetByLabelText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByLabelTextOptions
        {
          CollapseWhitespace = true,
          Normalizer = (text) => text,
        }));
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByLabelText("GetByLabelText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByLabelTextOptions
        {
          Trim = true,
          Normalizer = (text) => text,
        }));
      }
    }

    [TestMethod]
    public void GetByLabelText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<button type='button' aria-label='GetByLabelText_Success'></button>");
        IWebElement result = driver.GetByLabelText("GetByLabelText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByLabelText_Success_ExactFalse()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<button type='button' aria-label='Foo GetByLabelText_success_exactfalse bar'></button>");
        IWebElement result = driver.GetByLabelText("GetByLabelText_Success_ExactFalse", new QueryByLabelTextOptions
        {
          Exact = false,
        });
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByLabelText_Success_MatcherFunction()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<button id='target' type='button' aria-label='GetByLabelText_Success_MatcherFunction'></button>
<button type='button' aria-label='foo'></button>
        ");
        MatcherFunction matcher = (text, node) => node.GetAttribute("aria-label")?.ToLower() == "getbylabeltext_success_matcherfunction";
        IWebElement result = driver.GetByLabelText(matcher);
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByLabelText_Success_Normalizer()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<label>
  GetByLabelText_Success_Normalizer
  <input id='target' type='text' />
</label>
<label>
  foo
  <input type='text' />
</label>
        ");
        IWebElement result = driver.GetByLabelText("getbylabeltext_success_normalizer", new QueryByLabelTextOptions
        {
          Normalizer = text => text.ToLower(),
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByLabelText_Success_Regex()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<button type='button' aria-label='GetByLabelText_Success_Regex'></button>");
        IWebElement result = driver.GetByLabelText(new Regex("GetByLabelText.*"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByLabelText_Success_Within()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='parent'>
  <button id='target' type='button' aria-label='GetByLabelText_Success_Within'></button>
</div>
<button type='button' aria-label='GetByLabelText_Success_Within'></button>
        ");
        IWebElement parent = driver.FindElement(By.CssSelector("#parent"));
        IWebElement result = parent.GetByLabelText("GetByLabelText_Success_Within");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }
  }
}
