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
    public static Task<IEnumerable<IWebElement>> FindAllByAltTextAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetAllByAltText(matcher, options));
    }
    public static Task<IWebElement> FindByAltTextAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetByAltText(matcher, options));
    }

    public static IWebElement GetByAltText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return container.QueryByAltText(matcher, options) ?? throw new ElementException($"Unable to find an with the alt text: {matcher}", container);
    }

    public static IEnumerable<IWebElement> GetAllByAltText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      IEnumerable<IWebElement> results = container.QueryAllByAltText(matcher, options);
      if (!results.Any())
      {
        throw new ElementException($"Unable to find an with the alt text: {matcher}", container);
      }
      return results;
    }

    public static IWebElement QueryByAltText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.QueryAllByAltText(matcher, options);
      try
      {
        return results.SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements with the alt text: {matcher}", container);
      }
    }

    public static IEnumerable<IWebElement> QueryAllByAltText(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      options = options ?? new QueryByAttributeOptions();

      MatchesUtils.MatchesFunction matcherFunction = options.Exact
        ? (MatchesUtils.MatchesFunction)MatchesUtils.Matches
        : (MatchesUtils.MatchesFunction)MatchesUtils.FuzzyMatches;

      NormalizerFunction matchNormalizer = MatchesUtils.MakeNormalizer(new MatchesUtils.MakeNormalizerOptions
      {
        CollapseWhitespace = options.CollapseWhitespace,
        Normalizer = options.Normalizer,
        Trim = options.Trim,
      });

      return container.FindElements(By.CssSelector("img,input,area"))
        .Where(x => x.Parent() != null)
        .Where(node => matcherFunction(node.GetAttribute("title"), node, matcher, matchNormalizer)
          || matcherFunction(node.GetAttribute("alt"), node, matcher, matchNormalizer));
    }
  }
}
