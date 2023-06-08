using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokenizer;

namespace Parser
{
    public partial class TSLangParser
    {
        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// *EMPTY*
        /// Func Prog
        /// </code>
        /// </summary>
        private void Prog()
        {
            if (Done)
                // *EMPTY*
                return;

            Func();
            Prog();
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// def Type iden ( FList ) { Body }
        /// def Type iden ( FList ) return Expr ;
        /// </code>
        /// </summary>
        private void Func()
        {
            if (CurrentToken.Type != TSLangTokenTypes.kw_def)
                SyntaxError("Expected 'def'");
            DropToken();

            Type();
            
            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                SyntaxError("Expected identifier");
            DropToken();

            if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                SyntaxError("Expected '('");
            DropToken();

            FList();

            if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                SyntaxError("Expected ')'");
            DropToken();

            if (CurrentToken.Type == TSLangTokenTypes.leftBrace)
            {
                DropToken();

                Body();

                if (CurrentToken.Type != TSLangTokenTypes.rightBrace)
                    SyntaxError("Expected '}'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_return)
            {
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    SyntaxError("Expected ';'");
                DropToken();
            }
            else
            {
                SyntaxError("Expected '{' or 'return'");
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// *EMPTY*
        /// Stmt Body
        /// </code>
        /// </summary>
        private void Body()
        {
            if (CurrentToken.Type == TSLangTokenTypes.rightBrace)
                // *EMPTY*
                return;

            Stmt();

            Body();
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// if ( Expr ) Stmt
        /// if ( Expr ) Stmt else Stmt
        /// while ( Expr ) Stmt
        /// for ( iden = Expr to Expr ) Stmt
        /// return Expr ;
        /// { Body }
        /// Func
        /// Defvar ;
        /// Expr ;
        /// </code>
        /// </summary>
        private void Stmt()
        {
            if (CurrentToken.Type == TSLangTokenTypes.kw_if)
            {
                // if ( Expr ) Stmt
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                    SyntaxError("Expected '('");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                DropToken();

                Stmt();

                if (CurrentToken.Type == TSLangTokenTypes.kw_else)
                {
                    // if ( Expr ) Stmt else Stmt
                    DropToken();

                    Stmt();
                }
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_while)
            {
                // while ( Expr ) Stmt
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                    SyntaxError("Expected '('");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                DropToken();

                Stmt();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_for)
            {
                // for ( iden = Expr to Expr ) Stmt
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                    SyntaxError("Expected '('");
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.identifier)
                    SyntaxError("Expected identifier");
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.equals)
                    SyntaxError("Expected '='");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.kw_to)
                    SyntaxError("Expected 'to'");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                DropToken();

                Stmt();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_return)
            {
                // return Expr ;
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    SyntaxError("Expected ';'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftBrace)
            {
                // { Body }
                DropToken();

                Body();

                if (CurrentToken.Type != TSLangTokenTypes.rightBrace)
                    SyntaxError("Expected '}'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_def)
            {
                // Func
                Func();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_var)
            {
                // DefVar ;
                DefVar();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    SyntaxError("Expected ';'");
                DropToken();
            }
            else
            {
                // Expr ;
                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    SyntaxError("Expected ';'");
                DropToken();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// var Type iden
        /// var Type iden = Expr
        /// </code>
        /// </summary>
        private void DefVar()
        {
            if (CurrentToken.Type != TSLangTokenTypes.kw_var)
                SyntaxError("Expected 'var'");
            DropToken();

            Type();

            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                SyntaxError("Expected identifier");
            DropToken();

            if (CurrentToken.Type == TSLangTokenTypes.equals)
            {
                // var Type iden = Expr
                DropToken();

                Expr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// *EMPTY*
        /// Type iden
        /// Type iden, FList
        /// </code>
        /// </summary>
        private void FList()
        {
            if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                // *EMPTY*
                return;

            Type();

            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                SyntaxError("Expected identifier");
            DropToken();

            if (CurrentToken.Type == TSLangTokenTypes.comma)
            {
                // Type iden, FList
                DropToken();

                FList();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// *EMPTY*
        /// Expr
        /// Expr, CList
        /// </code>
        /// </summary>
        private void CList()
        {
            if (CurrentToken.Type == TSLangTokenTypes.leftBracket
                || CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
            {
                // *EMPTY*
                return;
            }

            Expr();

            if (CurrentToken.Type == TSLangTokenTypes.comma)
            {
                // Expr, CList
                DropToken();

                CList();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// int
        /// vector
        /// str
        /// null
        /// </code>
        /// </summary>
        private void Type()
        {
            if (CurrentToken.Type == TSLangTokenTypes.kw_int
                || CurrentToken.Type == TSLangTokenTypes.kw_vector
                || CurrentToken.Type == TSLangTokenTypes.kw_str
                || CurrentToken.Type == TSLangTokenTypes.kw_null)
            {
                DropToken();
            }
            else
            {
                SyntaxError("Expected type");
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr ? Expr : Expr
        /// </code>
        /// </summary>
        private void Expr()
        {
            LOrExpr();

            while (CurrentToken.Type == TSLangTokenTypes.questionMark)
            {
                // Expr ? Expr : Expr
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.colon)
                    SyntaxError("Expected ':'");
                DropToken();

                Expr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr || Expr
        /// </code>
        /// </summary>
        private void LOrExpr()
        {
            LAndExpr();

            while (CurrentToken.Type == TSLangTokenTypes.logicalOr)
            {
                // Expr || Expr
                DropToken();

                LAndExpr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr && Expr
        /// </code>
        /// </summary>
        private void LAndExpr()
        {
            EqNeqExpr();

            while (CurrentToken.Type == TSLangTokenTypes.logicalAnd)
            {
                // Expr && Expr
                DropToken();

                EqNeqExpr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr == Expr
        /// Expr != Expr
        /// </code>
        /// </summary>
        private void EqNeqExpr()
        {
            CompareExpr();

            while (CurrentToken.Type == TSLangTokenTypes.doubleEquals
                || CurrentToken.Type == TSLangTokenTypes.notEquals)
            {
                // Expr == Expr
                // Expr != Expr
                DropToken();

                CompareExpr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr > Expr
        /// Expr &lt; Expr
        /// Expr >= Expr
        /// Expr &lt;= Expr
        /// </code>
        /// </summary>
        private void CompareExpr()
        {
            AddSubExpr();

            while (CurrentToken.Type == TSLangTokenTypes.lessThan
                || CurrentToken.Type == TSLangTokenTypes.greaterThan
                || CurrentToken.Type == TSLangTokenTypes.lessThanOrEqual
                || CurrentToken.Type == TSLangTokenTypes.greaterThanOrEqual)
            {
                // Expr > Expr
                // Expr < Expr
                // Expr >= Expr
                // Expr <= Expr
                DropToken();

                AddSubExpr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr + Expr
        /// Expr - Expr
        /// </code>
        /// </summary>
        private void AddSubExpr()
        {
            MulDivModExpr();

            while (CurrentToken.Type == TSLangTokenTypes.plus
                || CurrentToken.Type == TSLangTokenTypes.minus)
            {
                // Expr + Expr
                // Expr - Expr
                DropToken();

                MulDivModExpr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr * Expr
        /// Expr / Expr
        /// Expr % Expr
        /// </code>
        /// </summary>
        private void MulDivModExpr()
        {
            FinalExpr();

            while (CurrentToken.Type == TSLangTokenTypes.asterisk
                || CurrentToken.Type == TSLangTokenTypes.slash
                || CurrentToken.Type == TSLangTokenTypes.percent)
            {
                // Expr * Expr
                // Expr / Expr
                // Expr % Expr
                DropToken();

                FinalExpr();
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// iden
        /// iden [ Expr ]
        /// iden [ Expr ] = Expr
        /// iden = Expr
        /// iden ( CList )
        /// number
        /// string
        /// [ CList ]
        /// ! Expr
        /// + Expr
        /// - Expr
        /// ( Expr )
        /// </code>
        /// </summary>
        private void FinalExpr()
        {
            if (CurrentToken.Type == TSLangTokenTypes.identifier)
            {
                // iden
                DropToken();

                if (CurrentToken.Type == TSLangTokenTypes.leftBracket)
                {
                    // iden [ Expr ]
                    DropToken();

                    Expr();

                    if (CurrentToken.Type != TSLangTokenTypes.rightBracket)
                        SyntaxError("Expected ']'");
                    DropToken();

                    if (CurrentToken.Type == TSLangTokenTypes.equals)
                    {
                        // iden [ Expr ] = Expr
                        DropToken();

                        Expr();
                    }
                }
                else if (CurrentToken.Type == TSLangTokenTypes.equals)
                {
                    // iden = Expr
                    DropToken();

                    Expr();
                }
                else if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
                {
                    // iden ( CList )
                    DropToken();

                    CList();

                    if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                        SyntaxError("Expected ')'");
                    DropToken();
                }
            }
            else if (CurrentToken.Type == TSLangTokenTypes.literal_integer
                || CurrentToken.Type == TSLangTokenTypes.literal_string_doubleQuote
                || CurrentToken.Type == TSLangTokenTypes.literal_string_singleQuote)
            {
                // number
                // string
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftBracket)
            {
                // [ CList ]
                DropToken();

                CList();

                if (CurrentToken.Type != TSLangTokenTypes.rightBracket)
                    SyntaxError("Expected ']'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.exclamationMark
                || CurrentToken.Type == TSLangTokenTypes.plus
                || CurrentToken.Type == TSLangTokenTypes.minus)
            {
                // ! Expr
                // + Expr
                // - Expr
                DropToken();

                Expr();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
            {
                // ( Expr )
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                DropToken();
            }
            else
            {
                SyntaxError("Expected valid expression");
            }
        }
    }
}
