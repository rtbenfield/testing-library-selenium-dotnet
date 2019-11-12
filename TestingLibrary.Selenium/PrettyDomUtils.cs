using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  public static class PrettyDomUtils
  {
    public static string PrettyDom(ISearchContext container, int maxLength = 7000)
    {
      string debugContent = GetContainerHtml(container);
      if (debugContent.Length > maxLength)
      {
        return debugContent.Substring(0, maxLength);
      }
      else
      {
        return debugContent;
      }
    }

    private static string GetContainerHtml(ISearchContext container)
    {
      if (container is IWebDriver driver)
      {
        return driver.FindElement(By.CssSelector("body")).GetProperty("outerHTML");
      }
      else if (container is IWebElement element)
      {
        return element.GetProperty("outerHTML");
      }
      else
      {
        return string.Empty;
      }
    }
  }
}