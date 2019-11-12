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
    public static Task<IEnumerable<IWebElement>> FindAllByPlaceholderTextAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetAllByPlaceholderText(matcher, options));
    }
    public static Task<IWebElement> FindByPlaceholderTextAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetByPlaceholderText(matcher, options));
    }

    public static IWebElement GetByPlaceholderText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return container.QueryByPlaceholderText(matcher, options) ?? throw new ElementException($"Unable to find an with the placeholder text of: {matcher}", container);
    }

    public static IEnumerable<IWebElement> GetAllByPlaceholderText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      IEnumerable<IWebElement> results = container.QueryAllByPlaceholderText(matcher, options);
      if (!results.Any())
      {
        throw new ElementException($"Unable to find an with the placeholder text of: {matcher}", container);
      }
      return results;
    }

    public static IWebElement QueryByPlaceholderText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.QueryAllByPlaceholderText(matcher, options);
      try
      {
        return results.SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements with the placeholder text of: {matcher}", container);
      }
    }

    public static IEnumerable<IWebElement> QueryAllByPlaceholderText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return QueryHelpers.QueryAllByAttribute("placeholder", container, matcher, options);
    }
  }
}
