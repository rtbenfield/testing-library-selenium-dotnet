namespace TestingLibrary.Selenium
{
  public class QueryOptions
  {
    public bool? CollapseWhitespace { get; set; }

    public bool Exact { get; set; } = true;

    public string Ignore { get; set; } = "script, style";

    public NormalizerFunction Normalizer { get; set; }

    public string Selector { get; set; } = "*";

    public bool? Trim { get; set; }
  }
}
