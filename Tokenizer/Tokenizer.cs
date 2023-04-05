using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokenizer
{
    internal static class Tokenizer
    {
        public static List<Token> Tokenize(StreamReader stream)
        {
            List<Token> tokens = new();
            StringBuilder tmpToken = new();
            TokenType? possibleType = null;
            int matchCount;
            int line = 1;
            int column = 1;

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
                else if (ch == '\r' || (ch == ' ' && tmpToken.Length == 0))
                {
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
                    tokens.Add(new Token(possibleType, tmpToken.ToString(), line, column - tmpToken.Length));
                    tmpToken.Clear();
                    possibleType = null;
                }
                else
                {
                    stream.Read();
                }
                column++;
            }

            return tokens;
        }
    }
}
