using System;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  public class DomElementException : Exception
  {
    private readonly IWebElement _element;

    public DomElementException(string message)
      : base(message)
    {
    }

    public DomElementException(string message, IWebElement element)
      : this(message)
    {
      _element = element;
    }
  }
}
