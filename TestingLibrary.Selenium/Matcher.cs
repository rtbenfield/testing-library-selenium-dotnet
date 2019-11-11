using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  public delegate bool MatcherFunction(string content, IWebElement element);

  public class Matcher
  {
    private delegate bool MatcherFunctionPrivate(string content, IWebElement element, bool exact);

    private readonly string _debugString;
    private readonly MatcherFunctionPrivate _matcherFunction;

    private Matcher(MatcherFunctionPrivate matcherFunction, string debugString)
    {
      _debugString = debugString;
      _matcherFunction = matcherFunction;
    }

    internal bool Invoke(string content, IWebElement element, bool exact)
    {
      return _matcherFunction(content, element, exact);
    }

    public override string ToString()
    {
      return _debugString;
    }

    public static implicit operator Matcher(MatcherFunction matcher) => new Matcher((content, node, exact) => matcher(content, node), "Function");

    public static implicit operator Matcher(string matcher) => new Matcher((content, node, exact) => exact ? content == matcher : content.ToLower().Contains(matcher.ToLower()), matcher);

    public static implicit operator Matcher(Regex matcher) => new Matcher((content, node, exact) => matcher.IsMatch(content), matcher.ToString());
  }
}
