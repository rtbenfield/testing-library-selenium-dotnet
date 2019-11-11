using System;

namespace TestingLibrary.Selenium
{
  public class WaitForElementException : Exception
  {
    public WaitForElementException(string message)
      : base(message)
    {

    }
  }
}
