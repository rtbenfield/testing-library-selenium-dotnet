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
  public class QueryByDisplayValueTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task FindAllByDisplayValue_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindAllByDisplayValueAsync("FindAllByDisplayValue_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindAllByDisplayValue_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<input type='text' value='FindAllByDisplayValue_Success' />
<textarea>FindAllByDisplayValue_Success</textarea>
<select>
  <option>foo</option>
  <option selected>FindAllByDisplayValue_Success</option>
</select>
        ", TimeSpan.FromSeconds(1));
        IEnumerable<IWebElement> results = await driver.FindAllByDisplayValueAsync("FindAllByDisplayValue_Success");
        Assert.AreEqual(3, results.Count());
      }
    }

    [TestMethod]
    public async Task FindByDisplayValue_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<ElementException>(() => driver.FindByDisplayValueAsync("FindByDisplayValue_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindByDisplayValue_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<input type='text' value='FindByDisplayValue_Fail_Multiple' />
<input type='text' value='FindByDisplayValue_Fail_Multiple' />
        ", TimeSpan.FromSeconds(1));
        await Assert.ThrowsExceptionAsync<MultipleElementsFoundException>(() => driver.FindByDisplayValueAsync("FindByDisplayValue_Fail_Multiple"));
      }
    }

    [TestMethod]
    public async Task FindByDisplayValue_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml("<input type='text' value='FindByDisplayValue_Success' />", TimeSpan.FromSeconds(1));
        IWebElement result = await driver.FindByDisplayValueAsync("FindByDisplayValue_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetAllByDisplayValue_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetAllByDisplayValue("GetAllByDisplayValue_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetAllByDisplayValue_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input type='text' value='GetAllByDisplayValue_Success' />
<textarea>GetAllByDisplayValue_Success</textarea>
<select>
  <option>foo</option>
  <option selected>GetAllByDisplayValue_Success</option>
</select>
        ");
        IEnumerable<IWebElement> results = driver.GetAllByDisplayValue("GetAllByDisplayValue_Success");
        Assert.AreEqual(3, results.Count());
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<ElementException>(() => driver.GetByDisplayValue("GetByDisplayValue_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input type='text' value='GetByDisplayValue_Fail_Multiple' />
<input type='text' value='GetByDisplayValue_Fail_Multiple' />
        ");
        Assert.ThrowsException<MultipleElementsFoundException>(() => driver.GetByDisplayValue("GetByDisplayValue_Fail_Multiple"));
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Fail_ThrowsWhenNormalizerOptionsInvalid()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByDisplayValue("GetByDisplayValue_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          CollapseWhitespace = true,
          Normalizer = (text) => text,
        }));
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByDisplayValue("GetByDisplayValue_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryByAttributeOptions
        {
          Trim = true,
          Normalizer = (text) => text,
        }));
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' value='GetByDisplayValue_Success' />");
        IWebElement result = driver.GetByDisplayValue("GetByDisplayValue_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Success_ExactFalse()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' value='Foo GetByDisplayValue_success_exactfalse bar' />");
        IWebElement result = driver.GetByDisplayValue("GetByDisplayValue_Success_ExactFalse", new QueryByAttributeOptions
        {
          Exact = false,
        });
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Success_MatcherFunction()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input id='target' type='text' value='GetByDisplayValue_Success_MatcherFunction' />
<input type='text' value='foo' />
");
        MatcherFunction matcher = (text, node) => node.GetProperty("value").ToLower() == "getbydisplayvalue_success_matcherfunction";
        IWebElement result = driver.GetByDisplayValue(matcher);
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Success_Normalizer()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<input id='target' type='text' value='GetByDisplayValue_Success_Normalizer' />
<input type='text' value='foo' />
");
        IWebElement result = driver.GetByDisplayValue("getbydisplayvalue_success_normalizer", new QueryByAttributeOptions
        {
          Normalizer = text => text.ToLower(),
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Success_Regex()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<input type='text' value='GetByDisplayValue_Success_Regex' />");
        IWebElement result = driver.GetByDisplayValue(new Regex("GetByDisplayValue.*"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByDisplayValue_Success_Within()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='parent'>
  <input id='target' type='text' value='GetByDisplayValue_Success_Within' />
</div>
<input type='text' value='GetByDisplayValue_Success_Within' />
        ");
        IWebElement parent = driver.FindElement(By.CssSelector("#parent"));
        IWebElement result = parent.GetByDisplayValue("GetByDisplayValue_Success_Within");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }
  }
}
