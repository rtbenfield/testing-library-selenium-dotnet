using System;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium.Exceptions
{
  public class ElementException : Exception
  {
    private readonly ISearchContext _container;
    private readonly string _prettyDom;

    public ElementException(string message, ISearchContext container)
      : base(message)
    {
      _container = container;

      // Grab the PrettyDOM string early. Message may not be called until after the driver is disposed.
      _prettyDom = PrettyDomUtils.PrettyDom(container);
    }

    public override string Message
    {
      get
      {
        return string.Join(Environment.NewLine + Environment.NewLine, base.Message, _prettyDom);
      }
    }
  }
}
