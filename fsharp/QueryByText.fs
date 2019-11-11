namespace TestingLibrary.Selenium

open OpenQA.Selenium
open System
open System.Runtime.CompilerServices

type QueryOptions =
    { CollapseWhitespace: bool option
      Exact: bool option
      Ignore: string option
      Normalizer: NormalizerFunction option
      Selector: string option
      Trim: bool option }

[<Extension>]
type IWebElementExtensions =

    [<Extension>]
    static member inline Parent(node: IWebElement): IWebElement option =
        try
            By.XPath("./..")
            |> node.FindElement
            |> Some
        with :? InvalidOperationException -> None

    [<Extension>]
    static member inline Matches(node: IWebElement, selector: string) =
        let parent = node.Parent()
        if not parent.IsSome then
            raise (InvalidOperationException("Matches only supports nodes with a parent element"))
        else
            By.CssSelector(selector)
            |> parent.Value.FindElements
            |> Seq.contains node

module Utils =
    let getNodeText (node: IWebElement): string =
        if node.Matches("input[type=submit], input[type=button]") then
            node.GetProperty("value")
        else
            By.XPath("./*")
            |> node.FindElements
            |> Seq.map (fun x -> x.Text)
            |> Seq.filter (fun x -> x.Length > 0)
            |> Seq.fold (fun prev curr -> prev.Replace(curr, "")) node.Text

[<Extension>]
type QueryByText() =

    [<Extension>]
    static member inline QueryAllByText(container: ISearchContext, matcher: Matcher, options: QueryOptions) =
        let selector =
            if options.Selector.IsSome then options.Selector.Value
            else "*"
        let exact =
            if options.Exact.IsSome then options.Exact.Value
            else true
        let ignore =
            if options.Ignore.IsSome then options.Ignore.Value
            else "script, style"

        let matcherFunction =
            if exact then Matchers.matches
            else Matchers.fuzzyMatches

        let matchNormalizer = Matchers.makeNormalizer (options.CollapseWhitespace, options.Trim, options.Normalizer)

        By.CssSelector(selector)
        |> container.FindElements
        |> Seq.filter (fun node -> ignore = "" || not (node.Matches(ignore)))
        |> Seq.filter (fun node -> matcherFunction (Utils.getNodeText (node), node, matcher, matchNormalizer))

//     [<Extension>]
//     static member inline QueryByText(container: ISearchContext, matcher: Matcher, options: QueryOptions) =
//         let results = container.QueryAllByText(matcher, options)
//         results.
//         match results.Count with
//         |
//         results.
//         false
