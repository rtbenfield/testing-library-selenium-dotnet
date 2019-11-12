using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TestingLibrary.Selenium.Exceptions;

namespace TestingLibrary.Selenium
{
  public class QueryByLabelTextOptions
  {
    public bool? CollapseWhitespace { get; set; }

    public bool Exact { get; set; } = true;

    public string Ignore { get; set; } = "script, style";

    public NormalizerFunction Normalizer { get; set; }

    public string Selector { get; set; } = "*";

    public bool? Trim { get; set; }
  }

  public static partial class Queries
  {
    public static Task<IEnumerable<IWebElement>> FindAllByLabelTextAsync(this ISearchContext container, Matcher matcher, QueryByLabelTextOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetAllByLabelText(matcher, options));
    }
    public static Task<IWebElement> FindByLabelTextAsync(this ISearchContext container, Matcher matcher, QueryByLabelTextOptions options = null)
    {
      return WaitForElement.WaitForElementAsync(() => container.GetByLabelText(matcher, options));
    }

    public static IWebElement GetByLabelText(this ISearchContext container, Matcher matcher, QueryByLabelTextOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.GetAllByLabelText(matcher, options);
      try
      {
        return results.SingleOrDefault() ?? throw new ElementException($"Unable to find an with the placeholder text of: {matcher}", container);
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements with the text of: {matcher}", container);
      }
    }

    public static IEnumerable<IWebElement> GetAllByLabelText(this ISearchContext container, Matcher matcher, QueryByLabelTextOptions options = null)
    {
      IEnumerable<IWebElement> els = container.QueryAllByLabelText(matcher, options);
      if (!els.Any())
      {
        IEnumerable<IWebElement> labels = QueryAllLabelsByText(container, matcher, options);
        if (labels.Any())
        {
          throw new ElementException($"Found a label with the text of: {matcher}, however no form control was found associated to that label. Make sure you're using the \"for\" attribute or \"aria-labelledby\" attribute correctly.", container);
        }
        else
        {
          throw new ElementException($"Unable to find a label with the text of: {matcher}", container);
        }
      }
      return els;
    }

    public static IWebElement QueryByLabelText(this ISearchContext container, Matcher matcher, QueryByLabelTextOptions options = null)
    {
      // Throws InvalidOperationException for invalid options
      IEnumerable<IWebElement> results = container.QueryAllByLabelText(matcher, options);
      try
      {
        return results.SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw new MultipleElementsFoundException($"Found multiple elements with the text of: {matcher}", container);
      }
    }

    public static IEnumerable<IWebElement> QueryAllByLabelText(this ISearchContext container, Matcher matcher, QueryByLabelTextOptions options = null)
    {
      options = options ?? new QueryByLabelTextOptions();

      NormalizerFunction matchNormalizer = MatchesUtils.MakeNormalizer(new MatchesUtils.MakeNormalizerOptions
      {
        CollapseWhitespace = options.CollapseWhitespace,
        Normalizer = options.Normalizer,
        Trim = options.Trim,
      });

      IEnumerable<IWebElement> labels = QueryAllLabelsByText(container, matcher, new QueryByLabelTextOptions
      {
        Exact = options.Exact,
        Normalizer = matchNormalizer,
      });

      IEnumerable<IWebElement> labelledElements = labels
        .Select(label =>
        {
          try
          {
            // TODO: if (label.control) does this work in Selenium?
            if (!string.IsNullOrEmpty(label.GetAttribute("for")))
            {
              // we're using this notation because with the # selector we would have to escape special characters e.g. user.name
              // see https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelector#Escaping_special_characters
              // <label for="someId">text</label><input id="someId" />
              return container.FindElement(By.CssSelector($"[id=\"{label.GetAttribute("for")}\"]"));
            }
            else if (!string.IsNullOrEmpty(label.GetAttribute("id")))
            {
              // <label id="someId">text</label><input aria-labelledby="someId" />
              return container.FindElement(By.CssSelector($"[aria-labelledby~=\"{label.GetAttribute("id")}\"]"));
            }
            else if (label.FindElements(By.XPath("./*")).Any())
            {
              // <label>text: <input /></label>
              return label.FindElement(By.CssSelector(options.Selector));
            }
            else
            {
              return null;
            }
          }
          catch (NoSuchElementException)
          {
            return null;
          }
        })
        .Where(label => label != null)
        .Concat(QueryHelpers.QueryAllByAttribute("aria-label", container, matcher, new QueryByAttributeOptions
        {
          Exact = options.Exact,
        }));

      var test = QueryHelpers.QueryAllByAttribute("aria-label", container, matcher, new QueryByAttributeOptions
      {
        Exact = options.Exact,
      }).Select(x => x.GetProperty("outerHTML")).ToArray();

      IEnumerable<IWebElement> possibleAriaLabelElements = container.QueryAllByText(matcher, new QueryOptions()
      {
        Exact = options.Exact,
        Normalizer = matchNormalizer,
      });

      IEnumerable<IWebElement> ariaLabelledElements = possibleAriaLabelElements.Aggregate(Enumerable.Empty<IWebElement>(), (allLabelledElements, nextLabelElement) =>
      {
        string labelId = nextLabelElement.GetAttribute("id");

        if (string.IsNullOrEmpty(labelId))
        {
          return allLabelledElements;
        }

        // ARIA labels can label multiple elements
        IEnumerable<IWebElement> labelledNodes = container.FindElements(By.CssSelector($"[aria-labelledby~=\"{labelId}\"]"));

        return allLabelledElements.Concat(labelledNodes);
      });

      return new HashSet<IWebElement>(labelledElements.Concat(ariaLabelledElements));
    }

    private static IEnumerable<IWebElement> QueryAllLabelsByText(ISearchContext container, Matcher matcher, QueryByLabelTextOptions options = null)
    {
      options = options ?? new QueryByLabelTextOptions();

      MatchesUtils.MatchesFunction matcherFunction = options.Exact
        ? (MatchesUtils.MatchesFunction)MatchesUtils.Matches
        : (MatchesUtils.MatchesFunction)MatchesUtils.FuzzyMatches;

      NormalizerFunction matchNormalizer = MatchesUtils.MakeNormalizer(new MatchesUtils.MakeNormalizerOptions
      {
        CollapseWhitespace = options.CollapseWhitespace,
        Normalizer = options.Normalizer,
        Trim = options.Trim,
      });

      return container.FindElements(By.CssSelector("label"))
        .Where(x => x.Parent() != null)
        .Where(label => matcherFunction(Utils.GetNodeText(label), label, matcher, matchNormalizer));
    }
  }
}
