using System;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TestingLibrary.Selenium
{
  public class WaitForElementOptions
  {
    // public ISearchContext Container { get; set; }

    // public object MutationObserverOptions { get; set; }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
  }

  public static class WaitForElement
  {
    public static async Task<T> WaitForElementAsync<T>(Func<T> callback, WaitForElementOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (callback == null)
      {
        throw new ArgumentNullException(nameof(callback));
      }

      options = options ?? new WaitForElementOptions();

      // Track the last exception thrown by the callback function to throw if the timeout passes
      Exception lastException = null;

      try
      {
        using (CancellationTokenSource pollCancellation = new CancellationTokenSource())
        {
          pollCancellation.CancelAfter(options.Timeout);

          while (!pollCancellation.IsCancellationRequested)
          {
            // If cancellation was requested externally, throw because of that
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
              // Attempt to run the callback and check the result, returning once it is successful
              T result = callback();
              if (result != null)
              {
                return result;
              }
            }
            catch (Exception ex)
            {
              lastException = ex;

              // If either the timeout or external token is canceled while the delay is running, bail on the delay
              using (CancellationTokenSource delayCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, pollCancellation.Token))
              {
                await Task.Delay(TimeSpan.FromMilliseconds(50), delayCancellation.Token);
              }
            }
          }
          throw lastException ?? new WaitForElementException("Timed out in WaitForElement.");
        }
      }
      catch (TaskCanceledException)
      {
        throw lastException ?? new WaitForElementException("Timed out in WaitForElement.");
      }
    }
  }
}