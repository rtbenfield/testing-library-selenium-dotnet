using System.Linq;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  internal static class Utils
  {
    public static string GetNodeText(IWebElement node)
    {
      if (node.Matches("input[type=submit], input[type=button]"))
      {
        return node.GetProperty("value");
      }

      // Start with the node text, which includes all child nodes.
      // Remove each child node's text from this node to result in the text representative of this node alone.
      return node.FindElements(By.XPath("./*"))
        .Select(x => x.Text)
        .Where(x => !string.IsNullOrEmpty(x))
        .Aggregate(node.Text, (prev, curr) => prev.Replace(curr, string.Empty));
    }
  }
}