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

            TokenType prevType = TSLangTokenTypes.invalid;
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
                else if (ch is ' ' or '\t')
                {
                    if (tmpToken.Length == 0)
                    {
                        column++;
                        tokenStartCol = column;
                        stream.Read();
                        continue;
                    }
                    else if (tmpToken[0] is not '"' and not '\'' and not '#')
                    {
                        endOfToken = true;
                    }
                }

                tmpToken.Append(ch);

                TokenType currentType = TypeOfToken(tmpToken.ToString());

                endOfToken |=
                    stream.EndOfStream |
                    ((prevType != TSLangTokenTypes.invalid) &&
                    (currentType == TSLangTokenTypes.invalid));

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
        /// <see cref="TSLangTokenTypes.invalid"/> if <paramref name="tokenStr"/> does not
        /// match any type of token or matches more than one type (except keyword and identifier).
        /// </returns>
        private static TokenType TypeOfToken(string tokenStr)
        {
            TokenType tokenType = TSLangTokenTypes.invalid;
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
                        tokenType = TSLangTokenTypes.invalid;
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
            TSLangTokenTypes.identifier,
            TSLangTokenTypes.literal_integer,
            TSLangTokenTypes.literal_string_singleQuote,
            TSLangTokenTypes.literal_string_doubleQuote,
            TSLangTokenTypes.semicolon,
            TSLangTokenTypes.leftParenthesis,
            TSLangTokenTypes.rightParenthesis,
            TSLangTokenTypes.leftBrace,
            TSLangTokenTypes.rightBrace,
            TSLangTokenTypes.leftBracket,
            TSLangTokenTypes.rightBracket,
            TSLangTokenTypes.lessThan,
            TSLangTokenTypes.greaterThan,
            TSLangTokenTypes.lessThanOrEqual,
            TSLangTokenTypes.greaterThanOrEqual,
            TSLangTokenTypes.equals,
            TSLangTokenTypes.plus,
            TSLangTokenTypes.minus,
            TSLangTokenTypes.asterisk,
            TSLangTokenTypes.slash,
            TSLangTokenTypes.percent,
            TSLangTokenTypes.doubleEquals,
            TSLangTokenTypes.notEquals,
            TSLangTokenTypes.logicalOr,
            TSLangTokenTypes.logicalAnd,
            TSLangTokenTypes.exclamationMark,
            TSLangTokenTypes.questionMark,
            TSLangTokenTypes.colon,
            TSLangTokenTypes.comma,
            TSLangTokenTypes.comment,
        });

        /// <summary>
        /// A collection of keyword <see cref="TokenType"/>s for TSLang.
        /// </summary>
        public static readonly ReadOnlyCollection<TokenType> Keywords = new(new List<TokenType>()
        {
            TSLangTokenTypes.kw_for,
            TSLangTokenTypes.kw_while,
            TSLangTokenTypes.kw_if,
            TSLangTokenTypes.kw_else,
            TSLangTokenTypes.kw_var,
            TSLangTokenTypes.kw_def,
            TSLangTokenTypes.kw_int,
            TSLangTokenTypes.kw_vector,
            TSLangTokenTypes.kw_str,
            TSLangTokenTypes.kw_null,
            TSLangTokenTypes.kw_return,
            TSLangTokenTypes.kw_to,
        });
    }
}
