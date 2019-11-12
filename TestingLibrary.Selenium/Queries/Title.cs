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
    public static Task<IEnumerable<IWebElement>> FindAllByTitleAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetAllByTitle(matcher, options));
    }
    public static Task<IWebElement> FindByTitleAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetByTitle(matcher, options));
    }

    public static IWebElement GetByTitle(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return container.QueryByTitle(matcher, options) ?? throw new ElementException($"Unable to find an element with the title: {matcher}.", container); ;
    }

    public static IEnumerable<IWebElement> GetAllByTitle(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      IEnumerable<IWebElement> results = container.QueryAllByTitle(matcher, options);
      if (!results.Any())
      {
        throw new ElementException($"Unable to find an element with the title: {matcher}.", container);
      }
      return results;
    }

    public static IWebElement QueryByTitle(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.QueryAllByTitle(matcher, options);
      try
      {
        return results.SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements with the title: {matcher}", container);
      }
    }

    public static IEnumerable<IWebElement> QueryAllByTitle(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
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

      return container.FindElements(By.CssSelector("[title], svg > title"))
        .Where(x => x.Parent() != null)
        .Where(node => matcherFunction(node.GetAttribute("title"), node, matcher, matchNormalizer)
          || matcherFunction(Utils.GetNodeText(node), node, matcher, matchNormalizer));
    }
  }
}
