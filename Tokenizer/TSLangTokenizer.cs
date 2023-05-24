using System.Collections.ObjectModel;
using System.Text;

namespace Tokenizer
{
    /// <summary>
    /// Represents a TSLang source code tokenizer.
    /// </summary>
    public class TSLangTokenizer
    {
        /// <summary>
        /// Current line of source code which <see cref="stream"/> is pointing to.
        /// </summary>
        private int line;

        /// <summary>
        /// Current column of source code which <see cref="stream"/> is pointing to.
        /// </summary>
        private int column;

        /// <summary>
        /// A <see cref="StreamReader"/> which provides source code content.
        /// </summary>
        private readonly StreamReader stream;

        /// <summary>
        /// Gets weather the tokenizer reached end of source code.
        /// </summary>
        public bool EndOfStream { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TSLangTokenizer"/> class
        /// for the provided source code stream.
        /// </summary>
        /// <param name="stream">A <see cref="StreamReader"/> providing source code.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TSLangTokenizer(StreamReader stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            EndOfStream = false;
            line = 1;
            column = 1;
        }

        /// <summary>
        /// Returns first available token in
        /// the provided source code stream.
        /// Closes source code <see cref="StreamReader"/> when it
        /// reaches to the end of it.
        /// </summary>
        /// <returns>Next token in the source code.</returns>
        public Token NextToken()
        {
            int tokenStartLine = line;
            int tokenStartCol = column;

            StringBuilder tmpToken = new();

            TokenType prevType = TokenType.Invalid;
            bool endOfToken = false;

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
                    stream.Read();
                    line++;
                    column = 1;

                    if (tmpToken.Length > 0)
                    {
                        endOfToken = true;
                    }
                    else
                    {
                        tokenStartLine = line;
                        tokenStartCol = column;
                        continue;
                    }
                }
                else if (ch == ' ' || ch == '\t')
                {
                    if (tmpToken.Length == 0)
                    {
                        column++;
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
                    stream.EndOfStream |
                    ((prevType != TokenType.Invalid) &&
                    (currentType == TokenType.Invalid));

                if (endOfToken)
                {
                    tmpToken.Length--;
                    break;
                }

                stream.Read();
                column++;
                prevType = currentType;
            }

            string tokenStr = tmpToken.ToString();

            if (stream.EndOfStream)
            {
                stream.Close();
                EndOfStream = true;
                prevType = TypeOfToken(tokenStr);
            }

            return new Token(prevType, tokenStr, tokenStartLine, tokenStartCol);
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
                    else
                    {
                        // More than one match. Invalid token.
                        tokenType = TokenType.Invalid;
                        break;
                    }
                }
            }

            // Keywords also match identifier pattern.
            // Check if token is actually identifier or keyword.
            if (tokenType.Name == "identifier")
            {
                foreach (TokenType type in Keywords)
                {
                    if (type.Pattern.IsMatch(tokenStr))
                    {
                        tokenType = type;
                        break;
                    }
                }
            }

            return tokenType;
        }

        /// <summary>
        /// A collection of non-keyword <see cref="TokenType"/>s for TSLang.
        /// </summary>
        public static readonly ReadOnlyCollection<TokenType> TokenTypes = new(new List<TokenType>
        {
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

        /// <summary>
        /// A collection of keyword <see cref="TokenType"/>s for TSLang.
        /// </summary>
        public static readonly ReadOnlyCollection<TokenType> Keywords = new(new List<TokenType>()
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
        });
    }
}
