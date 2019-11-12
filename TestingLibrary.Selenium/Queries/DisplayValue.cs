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
    public static Task<IEnumerable<IWebElement>> FindAllByDisplayValueAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetAllByDisplayValue(matcher, options));
    }
    public static Task<IWebElement> FindByDisplayValueAsync(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetByDisplayValue(matcher, options));
    }

    public static IWebElement GetByDisplayValue(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      return container.QueryByDisplayValue(matcher, options) ?? throw new ElementException($"Unable to find an with the value: {matcher}", container);
    }

    public static IEnumerable<IWebElement> GetAllByDisplayValue(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      IEnumerable<IWebElement> results = container.QueryAllByDisplayValue(matcher, options);
      if (!results.Any())
      {
        throw new ElementException($"Unable to find an with the value: {matcher}", container);
      }
      return results;
    }

    public static IWebElement QueryByDisplayValue(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.QueryAllByDisplayValue(matcher, options);
      try
      {
        return results.SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements with the value: {matcher}", container);
      }
    }

    public static IEnumerable<IWebElement> QueryAllByDisplayValue(this ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
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

      return container.FindElements(By.CssSelector("input,textarea,select"))
        .Where(x => x.Parent() != null)
        .Where(node =>
        {
          if (node.TagName == "SELECT")
          {
            IEnumerable<IWebElement> selectedOptions = node.FindElements(By.CssSelector("option:selected"));
            return selectedOptions.Any(optionNode => matcherFunction(Utils.GetNodeText(optionNode), optionNode, matcher, matchNormalizer));
          }
          else
          {
            return matcherFunction(node.GetProperty("value"), node, matcher, matchNormalizer);
          }
        });
    }
  }
}
