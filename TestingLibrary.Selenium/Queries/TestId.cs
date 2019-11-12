using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TestingLibrary.Selenium.Exceptions;

namespace TestingLibrary.Selenium
{
  public static partial class Queries
  {
    public static Task<IEnumerable<IWebElement>> FindAllByTestIdAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetAllByTestId(matcher, options));
    }
    public static Task<IWebElement> FindByTestIdAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetByTestId(matcher, options));
    }

    public static IWebElement GetByTestId(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return container.QueryByTestId(matcher, options) ?? throw new ElementException($"Unable to find an element by: [data-testid=\"{matcher}\"]", container);
    }

    public static IEnumerable<IWebElement> GetAllByTestId(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      IEnumerable<IWebElement> results = container.QueryAllByTestId(matcher, options);
      if (!results.Any())
      {
        throw new ElementException($"Unable to find an element by: [data-testid=\"{matcher}\"]", container);
      }
      return results;
    }

    public static IWebElement QueryByTestId(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.QueryAllByTestId(matcher, options);
      try
      {
        return results.SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements by: [data-testid=\"{matcher}\"]", container);
      }
    }

    public static IEnumerable<IWebElement> QueryAllByTestId(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return QueryHelpers.QueryAllByAttribute("data-testid", container, matcher, options);
    }
  }
}
