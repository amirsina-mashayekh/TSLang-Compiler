using System.Collections.ObjectModel;
using System.Text;

namespace Tokenizer
{
    /// <summary>
    /// Includes methods and fields to tokenize a TesLang source code.
    /// </summary>
    public static class TesLangTokenizer
    {
        /// <summary>
        /// Tokenizes provided source code.
        /// </summary>
        /// <param name="stream">Stream of source code.</param>
        /// <returns>A list of detected tokens in the source code.</returns>
        public static List<Token> Tokenize(StreamReader stream)
        {
            List<Token> tokens = new();

            StringBuilder tmpToken = new();
            int tokenStartLine = 1;
            int tokenStartCol = 1;

            TokenType prevType = TokenType.Invalid;
            int line = 1;
            int column = 0;
            bool rollback = false;
            bool endOfToken = false;

            using (stream)
            {
                while (!stream.EndOfStream)
                {
                    char ch = (char)stream.Peek();
                    column++;

                    if (ch == '\r')
                    {
                        stream.Read();
                        continue;
                    }
                    else if (ch == '\n')
                    {
                        line++;
                        column = 0;
                        if (tmpToken.Length == 0)
                        {
                            stream.Read();
                            continue;
                        }
                        endOfToken = true;
                    }
                    else if (ch == ' ')
                    {
                        if (tmpToken.Length == 0)
                        {
                            tokenStartCol = column;
                            stream.Read();
                            continue;
                        }
                        else if (tmpToken[0] != '"' && tmpToken[0] != '\'' && tmpToken[0] != '#')
                        {
                            endOfToken = true;
                        }
                    }

                    tmpToken.Append(ch);

                    TokenType currentType = TypeOfToken(tmpToken.ToString());

                    endOfToken |=
                        (prevType != TokenType.Invalid) &&
                        (currentType == TokenType.Invalid);

                    if (endOfToken)
                    {
                        // Last match was the correct type for token.
                        tmpToken.Length--;
                        tokens.Add(new Token(prevType, tmpToken.ToString(), tokenStartLine, tokenStartCol));
                        tmpToken.Clear();

                        tokenStartLine = line;
                        tokenStartCol = (ch == ' ' || ch == '\n') ? column + 1 : column;
                        endOfToken = false;

                        rollback = ch != '\n' && ch != ' ';
                    }

                    if (!rollback)
                    {
                        stream.Read();
                        prevType = currentType;
                    }
                    else
                    {
                        column--;
                        rollback = false;
                    }
                }
            }

            // Add last token
            string lastTokenStr = tmpToken.ToString();
            tokens.Add(new Token(TypeOfToken(lastTokenStr), lastTokenStr, tokenStartLine, tokenStartCol));

            return tokens;
        }

        /// <summary>
        /// Detects type of a given token.
        /// </summary>
        /// <param name="tokenStr">String Representation of the token.</param>
        /// <returns>
        /// Type of token if <paramref name="tokenStr"/> matches a specific type.
        /// <see cref="TokenType.Invalid"/> if <paramref name="tokenStr"/> does not
        /// match any type of token or matches more than one type (except keyword and identifier).
        /// </returns>
        private static TokenType TypeOfToken(string tokenStr)
        {
            TokenType tokenType = TokenType.Invalid;
            int matchCount = 0;

            foreach (TokenType type in TokenTypes)
            {
                if (type.Pattern.IsMatch(tokenStr))
                {
                    matchCount++;
                    if (matchCount == 1)
                    {
                        tokenType = type;
                    }
                    else if (matchCount == 2)
                    {
                        // If token matches both "identifier" and "kw_*" (keywork) types,
                        // it should be interpreted as keyword.
                        if (type.Name.StartsWith("kw_") && tokenType.Name == "identifier")
                        {
                            tokenType = type;
                        }
                        else if (!(tokenType.Name.StartsWith("kw_") && type.Name == "identifier"))
                        {
                            // More than one match. Add next character and try again.
                            tokenType = TokenType.Invalid;
                            break;
                        }
                    }
                    else
                    {
                        // More than one match. Add next character and try again.
                        tokenType = TokenType.Invalid;
                        break;
                    }
                }
            }

            return tokenType;
        }

        /// <summary>
        /// A collection of <see cref="TokenType"/>s for TesLang.
        /// </summary>
        public static readonly ReadOnlyCollection<TokenType> TokenTypes = new(new List<TokenType>
        {
            new("kw_for", @"for"),
            new("kw_while", @"while"),
            new("kw_if", @"if"),
            new("kw_else", @"else"),
            new("kw_var", @"var"),
            new("kw_def", @"def"),
            new("kw_int", @"int"),
            new("kw_vector", @"vector"),
            new("kw_str", @"str"),
            new("kw_null", @"null"),
            new("kw_return", @"return"),
            new("kw_to", @"to"),
            new("identifier", @"[A-Za-z_][A-Za-z1-9_]*"),
            new("literal_integer", @"[0-9]+"),
            new("literal_string_singleQuote", @"'[^'\r\n]*'"),
            new("literal_string_doubleQuote", @"""[^""\r\n]*"""),
            new("semicolon", @";"),
            new("leftParenthesis", @"\("),
            new("rightParenthesis", @"\)"),
            new("leftBrace", @"\{"),
            new("rightBrace", @"\}"),
            new("leftBracket", @"\["),
            new("rightBracket", @"\]"),
            new("lessThan", @"<"),
            new("greaterThan", @">"),
            new("lessThanOrEqual", @"<="),
            new("greaterThanOrEqual", @">="),
            new("equals", @"="),
            new("plus", @"\+"),
            new("minus", @"\-"),
            new("asterisk", @"\*"),
            new("slash", @"\/"),
            new("percent", @"\%"),
            new("doubleEquals", @"=="),
            new("notEquals", @"!="),
            new("logicalOr", @"\|\|"),
            new("logicalAnd", @"&&"),
            new("exclamationMark", @"!"),
            new("questionMark", @"\?"),
            new("colon", @":"),
            new("comma", @","),
            new("comment", @"#.*"),
        });
    }
}
