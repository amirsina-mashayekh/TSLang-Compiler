using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokenizer
{
    public static class TSLangTokenTypes
    {
        public static readonly TokenType identifier
            = new ("identifier", @"[A-Za-z_][A-Za-z1-9_]*");

        public static readonly TokenType literal_integer
            = new ("literal_integer", @"[0-9]+");

        public static readonly TokenType literal_string_singleQuote
            = new ("literal_string_singleQuote", @"'[^'\r\n]*'");

        public static readonly TokenType literal_string_doubleQuote
            = new ("literal_string_doubleQuote", @"""[^""\r\n]*""");

        public static readonly TokenType semicolon
            = new ("semicolon", @";");

        public static readonly TokenType leftParenthesis
            = new ("leftParenthesis", @"\(");

        public static readonly TokenType rightParenthesis
            = new ("rightParenthesis", @"\)");

        public static readonly TokenType leftBrace
            = new ("leftBrace", @"\{");

        public static readonly TokenType rightBrace
            = new ("rightBrace", @"\}");

        public static readonly TokenType leftBracket
            = new ("leftBracket", @"\[");

        public static readonly TokenType rightBracket
            = new ("rightBracket", @"\]");

        public static readonly TokenType lessThan
            = new ("lessThan", @"<");

        public static readonly TokenType greaterThan
            = new ("greaterThan", @">");

        public static readonly TokenType lessThanOrEqual
            = new ("lessThanOrEqual", @"<=");

        public static readonly TokenType greaterThanOrEqual
            = new ("greaterThanOrEqual", @">=");

        public static readonly TokenType equals
            = new ("equals", @"=");

        public static readonly TokenType plus
            = new ("plus", @"\+");

        public static readonly TokenType minus
            = new ("minus", @"\-");

        public static readonly TokenType asterisk
            = new ("asterisk", @"\*");

        public static readonly TokenType slash
            = new ("slash", @"\/");

        public static readonly TokenType percent
            = new ("percent", @"\%");

        public static readonly TokenType doubleEquals
            = new ("doubleEquals", @"==");

        public static readonly TokenType notEquals
            = new ("notEquals", @"!=");

        public static readonly TokenType logicalOr
            = new ("logicalOr", @"\|\|");

        public static readonly TokenType logicalAnd
            = new ("logicalAnd", @"&&");

        public static readonly TokenType exclamationMark
            = new ("exclamationMark", @"!");

        public static readonly TokenType questionMark
            = new ("questionMark", @"\?");

        public static readonly TokenType colon
            = new ("colon", @":");

        public static readonly TokenType comma
            = new ("comma", @",");

        public static readonly TokenType comment
            = new ("comment", @"#.*");

        public static readonly TokenType kw_for
            = new ("kw_for", @"for");

        public static readonly TokenType kw_while
            = new ("kw_while", @"while");

        public static readonly TokenType kw_if
            = new ("kw_if", @"if");

        public static readonly TokenType kw_else
            = new ("kw_else", @"else");

        public static readonly TokenType kw_var
            = new ("kw_var", @"var");

        public static readonly TokenType kw_def
            = new ("kw_def", @"def");

        public static readonly TokenType kw_int
            = new ("kw_int", @"int");

        public static readonly TokenType kw_vector
            = new ("kw_vector", @"vector");

        public static readonly TokenType kw_str
            = new ("kw_str", @"str");

        public static readonly TokenType kw_null
            = new ("kw_null", @"null");

        public static readonly TokenType kw_return
            = new ("kw_return", @"return");

        public static readonly TokenType kw_to
            = new ("kw_to", @"to");

    }
}
