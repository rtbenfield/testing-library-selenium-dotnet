using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  public class QueryByAttributeOptions
  {
    public bool? CollapseWhitespace { get; set; }

    public bool Exact { get; set; } = true;

    public NormalizerFunction Normalizer { get; set; }

    public bool? Trim { get; set; }
  }

  public static class QueryHelpers
  {
    public static IEnumerable<IWebElement> QueryAllByAttribute(string attribute, ISearchContext container, Matcher matcher, QueryByAttributeOptions options = null)
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

      return container.FindElements(By.CssSelector($"[{attribute}]"))
        .Where(x => x.Parent() != null)
        .Where(node => matcherFunction(node.GetAttribute(attribute), node, matcher, matchNormalizer));
    }
  }
}
