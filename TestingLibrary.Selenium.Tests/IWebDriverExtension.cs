using System;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium.Tests
{
  public static class IWebDriverExtensions
  {
    public static void RenderDelayedHtml(this IWebDriver driver, string html, TimeSpan delay)
    {
      driver.RenderHtml($@"
<script>
  setTimeout(function() {{
    var div = document.createElement('div');
    document.body.appendChild(div);
    div.outerHTML = `{html}`;
  }}, {delay.TotalMilliseconds});
</script>
      ");
    }

    public static void RenderHtml(this IWebDriver driver, string html)
    {
      string wrappedHtml = $@"
<html>
  <head></head>

  <body>
    {html}
  </body>
</html>
      ";
      driver.Url = $"data:text/html;charset=utf-8,{wrappedHtml}";
    }
  }
}
