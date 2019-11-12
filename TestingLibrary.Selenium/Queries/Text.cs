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
    public static Task<IEnumerable<IWebElement>> FindAllByTextAsync(this ISearchContext container, Matcher matcher, QueryOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetAllByText(matcher, options));
    }
    public static Task<IWebElement> FindByTextAsync(this ISearchContext container, Matcher matcher, QueryOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetByText(matcher, options));
    }

    public static IWebElement GetByText(this ISearchContext container, Matcher matcher, QueryOptions options = null)
    {
      return container.QueryByText(matcher, options) ?? throw new ElementException($"Unable to find an element with the text: {matcher}. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.", container); ;
    }

    public static IEnumerable<IWebElement> GetAllByText(this ISearchContext container, Matcher matcher, QueryOptions options = null)
    {
      IEnumerable<IWebElement> results = container.QueryAllByText(matcher, options);
      if (!results.Any())
      {
        throw new ElementException($"Unable to find an element with the text: {matcher}. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.", container);
      }
      return results;
    }

    public static IWebElement QueryByText(this ISearchContext container, Matcher matcher, QueryOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.QueryAllByText(matcher, options);
      try
      {
        return results.SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements with the text: {matcher}", container);
      }
    }

    public static IEnumerable<IWebElement> QueryAllByText(this ISearchContext container, Matcher matcher, QueryOptions options = null)
    {
      options = options ?? new QueryOptions();

      MatchesUtils.MatchesFunction matcherFunction = options.Exact
        ? (MatchesUtils.MatchesFunction)MatchesUtils.Matches
        : (MatchesUtils.MatchesFunction)MatchesUtils.FuzzyMatches;

      NormalizerFunction matchNormalizer = MatchesUtils.MakeNormalizer(new MatchesUtils.MakeNormalizerOptions
      {
        CollapseWhitespace = options.CollapseWhitespace,
        Normalizer = options.Normalizer,
        Trim = options.Trim,
      });

      IEnumerable<IWebElement> baseArray = Enumerable.Empty<IWebElement>();
      if (container is IWebElement containerElement && containerElement.Matches(options.Selector))
      {
        baseArray = new IWebElement[] { containerElement };
      }

      return baseArray.Concat(container.FindElements(By.CssSelector(options.Selector)))
        .Where(x => x.Parent() != null)
        .Where(node => string.IsNullOrEmpty(options.Ignore) || !node.Matches(options.Ignore))
        .Where(node => matcherFunction(Utils.GetNodeText(node), node, matcher, matchNormalizer));
    }
  }
}
