using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tokenizer
{
    internal class TokenType
    {
        public string Name { get; }

        public Regex Pattern { get; }

        private readonly int _hashCode;

        public TokenType(string name, string pattern) : this(
            name,
            new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant))
        { }

        public TokenType(string name, Regex pattern)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            _hashCode = HashCode.Combine(name, pattern);
        }

        public override bool Equals(object? obj)
        {
            return obj is TokenType && _hashCode == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string? ToString()
        {
            return Name;
        }

        public static HashSet<TokenType> TokenTypes = new()
        {
            new TokenType("kw_for", @"for"),
            new TokenType("kw_int", @"int"),
            new TokenType("kw_vector", @"vector"),
            new TokenType("kw_str", @"str"),
            new TokenType("kw_var", @"var"),
            new TokenType("kw_def", @"def"),
            new TokenType("kw_return", @"return"),
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
            new TokenType("comment", @"#.*$"),
        };
    }
}
