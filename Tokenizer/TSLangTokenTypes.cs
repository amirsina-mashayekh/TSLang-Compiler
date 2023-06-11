namespace Tokenizer
{
    /// <summary>
    /// Static class which includes token types of TSLang.
    /// </summary>
    public static class TSLangTokenTypes
    {
        public static readonly TokenType
            invalid
            = new("invalid", ""),

            identifier
            = new("identifier", @"[A-Za-z_][A-Za-z1-9_]*"),

            literal_integer
            = new("literal_integer", @"[0-9]+"),

            literal_string_singleQuote
            = new("literal_string_singleQuote", @"'[^'\r\n]*'"),

            literal_string_doubleQuote
            = new("literal_string_doubleQuote", @"""[^""\r\n]*"""),

            semicolon
            = new("semicolon", @";"),

            leftParenthesis
            = new("leftParenthesis", @"\("),

            rightParenthesis
            = new("rightParenthesis", @"\)"),

            leftBrace
            = new("leftBrace", @"\{"),

            rightBrace
            = new("rightBrace", @"\}"),

            leftBracket
            = new("leftBracket", @"\["),

            rightBracket
            = new("rightBracket", @"\]"),

            lessThan
            = new("lessThan", @"<"),

            greaterThan
            = new("greaterThan", @">"),

            lessThanOrEqual
            = new("lessThanOrEqual", @"<="),

            greaterThanOrEqual
            = new("greaterThanOrEqual", @">="),

            equals
            = new("equals", @"="),

            plus
            = new("plus", @"\+"),

            minus
            = new("minus", @"\-"),

            asterisk
            = new("asterisk", @"\*"),

            slash
            = new("slash", @"\/"),

            percent
            = new("percent", @"\%"),

            doubleEquals
            = new("doubleEquals", @"=="),

            notEquals
            = new("notEquals", @"!="),

            logicalOr
            = new("logicalOr", @"\|\|"),

            logicalAnd
            = new("logicalAnd", @"&&"),

            exclamationMark
            = new("exclamationMark", @"!"),

            questionMark
            = new("questionMark", @"\?"),

            colon
            = new("colon", @":"),

            comma
            = new("comma", @","),

            comment
            = new("comment", @"#.*"),

            kw_for
            = new("kw_for", @"for"),

            kw_while
            = new("kw_while", @"while"),

            kw_if
            = new("kw_if", @"if"),

            kw_else
            = new("kw_else", @"else"),

            kw_var
            = new("kw_var", @"var"),

            kw_def
            = new("kw_def", @"def"),

            kw_int
            = new("kw_int", @"int"),

            kw_vector
            = new("kw_vector", @"vector"),

            kw_str
            = new("kw_str", @"str"),

            kw_null
            = new("kw_null", @"null"),

            kw_return
            = new("kw_return", @"return"),

            kw_to
            = new("kw_to", @"to");
    }
}
