using System;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium.Exceptions
{
  public class MultipleElementsFoundException : ElementException
  {
    public MultipleElementsFoundException(string message, ISearchContext container)
      : base($"{message}{Environment.NewLine}{Environment.NewLine}(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`))", container)
    {
    }
  }
}
