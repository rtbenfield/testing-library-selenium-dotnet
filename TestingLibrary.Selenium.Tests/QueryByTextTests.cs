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

namespace TestingLibrary.Selenium.Tests
{
  [TestClass]
  public class QueryByTextTests
  {
    private IWebDriver CreateWebDriver()
    {
      return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions());
    }

    [TestMethod]
    public async Task FindAllByText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<DomElementException>(() => driver.FindAllByTextAsync("FindAllByText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindAllByText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<div>FindAllByText_Success</div>
<div>FindAllByText_Success</div>
        ", TimeSpan.FromSeconds(1));
        IEnumerable<IWebElement> results = await driver.FindAllByTextAsync("FindAllByText_Success");
        Assert.AreEqual(2, results.Count());
      }
    }

    [TestMethod]
    public async Task FindByText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Stopwatch timer = Stopwatch.StartNew();
        await Assert.ThrowsExceptionAsync<DomElementException>(() => driver.FindByTextAsync("FindByText_Fail_Missing"));
        timer.Stop();
        Assert.IsTrue(timer.Elapsed >= TimeSpan.FromSeconds(5));
      }
    }

    [TestMethod]
    public async Task FindByText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"
<div>FindByText_Fail_Multiple</div>
<div>FindByText_Fail_Multiple</div>
        ", TimeSpan.FromSeconds(1));
        await Assert.ThrowsExceptionAsync<DomElementException>(() => driver.FindByTextAsync("FindByText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public async Task FindByText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderDelayedHtml(@"<div>FindByText_Success</div>", TimeSpan.FromSeconds(1));
        IWebElement result = await driver.FindByTextAsync("FindByText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetAllByText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<DomElementException>(() => driver.GetAllByText("GetAllByText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetAllByText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div>GetAllByText_Success</div>
<div>
  GetAllByText_Success
</div>
<div>GetAllByText_Success <span></span></div>
        ");
        IEnumerable<IWebElement> results = driver.GetAllByText("GetAllByText_Success");
        Assert.AreEqual(3, results.Count());
      }
    }

    [TestMethod]
    public void GetByText_Fail_Missing()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<DomElementException>(() => driver.GetByText("GetByText_Fail_Missing"));
      }
    }

    [TestMethod]
    public void GetByText_Fail_Multiple()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div>GetByText_Fail_Multiple</div>
<div>GetByText_Fail_Multiple</div>
        ");
        Assert.ThrowsException<DomElementException>(() => driver.GetByText("GetByText_Fail_Multiple"));
      }
    }

    [TestMethod]
    public void GetByText_Fail_ThrowsWhenNormalizerOptionsInvalid()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(string.Empty);
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByText("GetByText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryOptions
        {
          CollapseWhitespace = true,
          Normalizer = (text) => text,
        }));
        Assert.ThrowsException<InvalidOperationException>(() => driver.GetByText("GetByText_Fail_ThrowsWhenNormalizerOptionsInvalid", new QueryOptions
        {
          Trim = true,
          Normalizer = (text) => text,
        }));
      }
    }

    [TestMethod]
    public void GetByText_Success()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div>GetByText_Success</div>");
        IWebElement result = driver.GetByText("GetByText_Success");
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByText_Success_ExactFalse()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div>Foo Getbytext_success_exactfalse bar</div>");
        IWebElement result = driver.GetByText("GetByText_Success_ExactFalse", new QueryOptions
        {
          Exact = false,
        });
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByText_Success_Ignore()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='target'>GetByText_Success_Ignore</div>
<div id='ignore'>GetByText_Success_Ignore</div>
        ");
        IWebElement result = driver.GetByText("GetByText_Success_Ignore", new QueryOptions
        {
          Ignore = "#ignore",
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [DataTestMethod]
    [DataRow("button")]
    [DataRow("submit")]
    public void GetByText_Success_InputButton(string type)
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml($"<input id='target' value='GetByText_Success_InputButton' type='{type}' />");
        IWebElement result = driver.GetByText("GetByText_Success_InputButton");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByText_Success_MatcherFunction()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='target'>GetByText_Success_MatcherFunction</div>
<div>foo</div>
");
        MatcherFunction matcher = (text, node) => node.Text == "GetByText_Success_MatcherFunction";
        IWebElement result = driver.GetByText(matcher);
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByText_Success_Normalizer()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='target'>GetByText_Success_Normalizer</div>
<div>foo</div>
");
        IWebElement result = driver.GetByText("getbytext_success_normalizer", new QueryOptions
        {
          Normalizer = text => text.ToLower(),
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByText_Success_Regex()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div>GetByText_Success_Regex</div>");
        IWebElement result = driver.GetByText(new Regex("GetByText.*"));
        Assert.IsNotNull(result);
      }
    }

    [TestMethod]
    public void GetByText_Success_Self()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml("<div id='target'>GetByText_Success_Self</div>");
        IWebElement container = driver.FindElement(By.CssSelector("#target"));
        IWebElement result = container.GetByText("GetByText_Success_Self");
        Assert.AreEqual(container, result);
      }
    }

    [TestMethod]
    public void GetByText_Success_Selector()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div>GetByText_Success_Selector</div>
<span id='target'>GetByText_Success_Selector</span>
        ");
        IWebElement result = driver.GetByText("GetByText_Success_Selector", new QueryOptions
        {
          Selector = "span",
        });
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }

    [TestMethod]
    public void GetByText_Success_Within()
    {
      using (IWebDriver driver = CreateWebDriver())
      {
        driver.RenderHtml(@"
<div id='parent'>
  <div id='target'>GetByText_Success_Within</div>
</div>
<div>GetByText_Success_Within</div>
        ");
        IWebElement parent = driver.FindElement(By.CssSelector("#parent"));
        IWebElement result = parent.GetByText("GetByText_Success_Within");
        Assert.IsNotNull(result);
        Assert.AreEqual("target", result.GetAttribute("id"));
      }
    }
  }
}
