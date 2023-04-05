using System.Text;

namespace Tokenizer
{
    public static class Tokenizer
    {
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
                    foreach (TokenType type in TokenType.TokenTypes)
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
                                if (type.Name.StartsWith("kw_") && possibleType!.Name == "identifier")
                                {
                                    possibleType = type;
                                }
                                else if (!(possibleType!.Name.StartsWith("kw_") && type.Name == "identifier"))
                                {
                                    possibleType = null;
                                    break;
                                }
                            }
                            else
                            {
                                possibleType = null;
                                break;
                            }
                        }
                    }

                    if (matchCount == 0 && possibleType is not null)
                    {
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
    }
}
