using System;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  internal static class IWebElementExtensions
  {
    public static IWebElement Parent(this IWebElement node)
    {
      try
      {
        return node.FindElement(By.XPath("./.."));
      }
      catch (InvalidSelectorException)
      {
        // Must have hit the root
        return null;
      }
    }

    public static bool Matches(this IWebElement node, string selector)
    {
      IWebElement parent = node.Parent();
      if (parent == null)
      {
        throw new InvalidOperationException("Matches only supports nodes with a parent element");
      }
      return parent.FindElements(By.CssSelector(selector)).Contains(node);
    }
  }
}
