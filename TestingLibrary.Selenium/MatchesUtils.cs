using System;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  public delegate string NormalizerFunction(string input);

  internal static class MatchesUtils
  {
    public delegate bool MatchesFunction(string textToMatch, IWebElement node, Matcher matcher, NormalizerFunction matchNormalizer);

    public static bool FuzzyMatches(string textToMatch, IWebElement node, Matcher matcher, NormalizerFunction normalizer)
    {
      if (textToMatch == null)
      {
        return false;
      }

      string normalizedText = normalizer(textToMatch);
      return matcher.Invoke(normalizedText, node, false);
    }

    public static bool Matches(string textToMatch, IWebElement node, Matcher matcher, NormalizerFunction normalizer)
    {
      if (textToMatch == null)
      {
        return false;
      }

      string normalizedText = normalizer(textToMatch);
      return matcher.Invoke(normalizedText, node, true);
    }

    public class MakeNormalizerOptions
    {
      public bool? Trim { get; set; }

      public bool? CollapseWhitespace { get; set; }

      public NormalizerFunction Normalizer { get; set; }
    }

    public static NormalizerFunction MakeNormalizer(MakeNormalizerOptions options)
    {
      if (options == null)
      {
        throw new ArgumentNullException(nameof(options));
      }

      if (options.Normalizer != null)
      {
        // User has specified a custom normalizer
        if (options.Trim.HasValue || options.CollapseWhitespace.HasValue)
        {
          // They've also specified a value for Trim or CollapseWhitespace
          throw new InvalidOperationException(
            $"{nameof(MakeNormalizerOptions.Trim)} and {nameof(MakeNormalizerOptions.CollapseWhitespace)} are not supported with {nameof(MakeNormalizerOptions.Normalizer)}"
            + $"If you want to use the default {nameof(MakeNormalizerOptions.Trim)} and {nameof(MakeNormalizerOptions.CollapseWhitespace)} logic in your normalizer, "
            + $"use \"({nameof(GetDefaultNormalizer)}\" and compose that into your normalizer"
          );
        }

        return options.Normalizer;
      }
      else
      {
        // No custom normalizer specified. Just use default.
        return GetDefaultNormalizer(options);
      }
    }

    private static NormalizerFunction GetDefaultNormalizer(MakeNormalizerOptions options)
    {
      return text =>
      {
        string normalizedText = text;
        normalizedText = options.Trim ?? true ? normalizedText.Trim() : normalizedText;
        normalizedText = options.CollapseWhitespace ?? true ? Regex.Replace(normalizedText, @"\s+", " ") : normalizedText;
        return normalizedText;
      };
    }
  }
}
