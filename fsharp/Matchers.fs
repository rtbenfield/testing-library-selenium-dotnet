namespace TestingLibrary.Selenium

open OpenQA.Selenium
open System.Text.RegularExpressions

type MatcherFunction = string * IWebElement -> bool

type NormalizerFunction = string -> string

type Matcher =
    | Text of string
    | Expression of Regex
    | Function of MatcherFunction

module Matchers =
    let matches (textToMatch: string, node: IWebElement, matcher: Matcher, normalizer: NormalizerFunction): bool =
        // if typeof textToMatch !== "string"
        let normalizedText = normalizer (textToMatch)
        match matcher with
        | Text(matcher) -> normalizedText = matcher
        | Expression(matcher) -> matcher.IsMatch(textToMatch)
        | Function(matcher) -> matcher (textToMatch, node)

    let fuzzyMatches (textToMatch: string, node: IWebElement, matcher: Matcher, normalizer: NormalizerFunction): bool =
        // if typeof textToMatch !== "string"
        let normalizedText = normalizer (textToMatch)
        match matcher with
        | Text(matcher) -> normalizedText.ToLower().Contains(matcher.ToLower())
        | Expression(matcher) -> matcher.IsMatch(textToMatch)
        | Function(matcher) -> matcher (textToMatch, node)

    let getDefaultNormalizer (trim: bool option, collapseWhitespace: bool option) (text: string) =
        let mutable normalizedText = text
        normalizedText <-
            if not trim.IsSome || trim.Value then normalizedText.Trim()
            else normalizedText
        normalizedText <-
            if not collapseWhitespace.IsSome || collapseWhitespace.Value then Regex.Replace(normalizedText, @"\s+", " ")
            else normalizedText
        normalizedText

    let makeNormalizer (trim: bool option, collapseWhitespace: bool option, normalizer: NormalizerFunction option): NormalizerFunction =
        if normalizer.IsSome then normalizer.Value
        else getDefaultNormalizer (trim, collapseWhitespace)
