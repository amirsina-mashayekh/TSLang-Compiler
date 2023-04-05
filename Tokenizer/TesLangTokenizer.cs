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
                    if (ch == '\n')
                    {
                        line++;
                        column = 1;
                        stream.Read();
                        continue;
                    }
                    else if (ch == '\r')
                    {
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
        /// A collection of <see cref="TokenType"/>s for TesLang.
        /// </summary>
        public static readonly ReadOnlyCollection<TokenType> TokenTypes = new(new List<TokenType>
        {
            new TokenType("kw_for", @"for"),
            new TokenType("kw_int", @"int"),
            new TokenType("kw_vector", @"vector"),
            new TokenType("kw_str", @"str"),
            new TokenType("kw_var", @"var"),
            new TokenType("kw_def", @"def"),
            new TokenType("kw_return", @"return"),
            new TokenType("kw_to", @"to"),
            new TokenType("identifier", @"[A-Za-z_][A-Za-z1-9_]*"),
            new TokenType("integerLiteral", @"[0-9]+"),
            new TokenType("stringLiteral_singleQuote", @"'[^'\r\n]*'"),
            new TokenType("stringLiteral_doubleQuote", @"""[^""\r\n]*"""),
            new TokenType("semicolon", @";"),
            new TokenType("leftParenthesis", @"\("),
            new TokenType("rightParenthesis", @"\)"),
            new TokenType("leftBrace", @"\{"),
            new TokenType("rightBrace", @"\}"),
            new TokenType("leftBracket", @"\["),
            new TokenType("rightBracket", @"\]"),
            new TokenType("lessThan", @"<"),
            new TokenType("greaterThan", @">"),
            new TokenType("equals", @"="),
            new TokenType("plus", @"\+"),
            new TokenType("minus", @"\-"),
            new TokenType("asterisk", @"\*"),
            new TokenType("slash", @"\/"),
            new TokenType("comment", @"#.*"),
            new TokenType("invalid", ""),
        });
    }
}
