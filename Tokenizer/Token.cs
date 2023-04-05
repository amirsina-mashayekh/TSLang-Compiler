using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokenizer
{
    internal class Token
    {
        public TokenType Type { get; }

        public string Value { get; }

        public int Line { get; }

        public int Column { get; }

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Value = value;
            Line = line;
            Column = column;
        }

        public override string? ToString()
        {
            return
                Value + ", " +
                Type + ", " +
                Line + ", " +
                Column;
        }
    }
}
