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
            int tokenStart = 1;
            TokenType? possibleType = null;
            int matchCount;
            int line = 1;
            int column = 1;

            using (stream)
            {
                while (!stream.EndOfStream)
                {
                    char ch = (char)stream.Peek();
                    if (ch == '\r')
                    {
                        stream.Read();
                        continue;
                    }
                    else if (ch == '\n')
                    {
                        line++;
                        column = 1;
                        stream.Read();
                        continue;
                    }
                    else if (ch == ' ' && tmpToken.Length == 0)
                    {
                        column++;
                        tokenStart = column;
                        stream.Read();
                        continue;
                    }

                    tmpToken.Append(ch);
                    string tmpTokenStr = tmpToken.ToString();

                    matchCount = 0;
                    foreach (TokenType type in TokenTypes)
                    {
                        if (type.Pattern.IsMatch(tmpTokenStr))
                        {
                            matchCount++;
                            if (matchCount == 1)
                            {
                                possibleType = type;
                            }
                            else if (matchCount == 2)
                            {
                                // If token matches both "identifier" and "kw_*" (keywork) types,
                                // it should be interpreted as keyword.
                                if (type.Name.StartsWith("kw_") && possibleType!.Name == "identifier")
                                {
                                    possibleType = type;
                                }
                                else if (!(possibleType!.Name.StartsWith("kw_") && type.Name == "identifier"))
                                {
                                    // More than one match. Add next character and try again.
                                    possibleType = null;
                                    break;
                                }
                            }
                            else
                            {
                                // More than one match. Add next character and try again.
                                possibleType = null;
                                break;
                            }
                        }
                    }

                    if (matchCount == 0 && possibleType is not null)
                    {
                        // Last match was the correct type for token.
                        tmpToken.Length--;
                        tokens.Add(new Token(possibleType, tmpToken.ToString(), line, tokenStart));
                        tmpToken.Clear();
                        possibleType = null;
                        tokenStart = column;
                    }
                    else
                    {
                        stream.Read();
                        column++;
                    }
                }
            }

            return tokens;
        }

        /// <summary>
        /// A TokenType which represents error.
        /// </summary>
        public static readonly TokenType invalidToken = new("invalid", "");

        /// <summary>
        /// A collection of <see cref="TokenType"/>s for TesLang.
        /// </summary>
        public static readonly ReadOnlyCollection<TokenType> TokenTypes = new(new List<TokenType>
        {
            new("kw_for", @"for"),
            new("kw_int", @"int"),
            new("kw_vector", @"vector"),
            new("kw_str", @"str"),
            new("kw_var", @"var"),
            new("kw_def", @"def"),
            new("kw_return", @"return"),
            new("kw_to", @"to"),
            new("identifier", @"[A-Za-z_][A-Za-z1-9_]*"),
            new("integerLiteral", @"[0-9]+"),
            new("stringLiteral_singleQuote", @"'[^'\r\n]*'"),
            new("stringLiteral_doubleQuote", @"""[^""\r\n]*"""),
            new("semicolon", @";"),
            new("leftParenthesis", @"\("),
            new("rightParenthesis", @"\)"),
            new("leftBrace", @"\{"),
            new("rightBrace", @"\}"),
            new("leftBracket", @"\["),
            new("rightBracket", @"\]"),
            new("lessThan", @"<"),
            new("greaterThan", @">"),
            new("equals", @"="),
            new("plus", @"\+"),
            new("minus", @"\-"),
            new("asterisk", @"\*"),
            new("slash", @"\/"),
            new("comment", @"#.*"),
            invalidToken,
        });
    }
}
