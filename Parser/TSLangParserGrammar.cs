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
        private void Prog()
        {
            /*
             * <empty>
             * Func Prog
             */

            if (tokenizer.EndOfStream)
                return;
            else if (CurrentToken.Type != TSLangTokenTypes.kw_def)
                Error("Expected 'def'");

            Func();
            Prog();
        }

        private void Func()
        {
            /*
             * def Type iden ( FList ) { Body }
             * def Type iden ( FList ) return Expr ;
             */

            if (CurrentToken.Type != TSLangTokenTypes.kw_def)
                Error("Expected 'def'");
            DropToken();

            Type();
            
            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                Error("Expected identifier");
            DropToken();

            if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                Error("Expected '('");
            DropToken();

            FList();

            if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                Error("Expected ')'");
            DropToken();

            if (CurrentToken.Type == TSLangTokenTypes.leftBrace)
            {
                DropToken();

                Body();

                if (CurrentToken.Type != TSLangTokenTypes.rightBrace)
                    Error("Expected '}'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_return)
            {
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    Error("Expected ';'");
                DropToken();
            }
            else
            {
                Error("Expected '{' or 'return'");
            }
        }

        private void Body()
        {
             /*
              * <empty>
              * Stmt Body
              */

            if (CurrentToken.Type == TSLangTokenTypes.rightBrace)
                return;

            Stmt();

            Body();
        }

        private void Stmt()
        {
             /*
              * if ( Expr ) Stmt
              * if ( Expr ) Stmt else Stmt
              * while ( Expr ) Stmt
              * for ( iden = Expr to Expr ) Stmt
              * return Expr ;
              * { Body }
              * Func
              * Defvar ;
              * Expr ;
              */

            if (CurrentToken.Type == TSLangTokenTypes.kw_if)
            {
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                    Error("Expected '('");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    Error("Expected ')'");
                DropToken();

                Stmt();

                if (CurrentToken.Type == TSLangTokenTypes.kw_else)
                {
                    DropToken();

                    Stmt();
                }
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_while)
            {
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                    Error("Expected '('");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    Error("Expected ')'");
                DropToken();

                Stmt();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_for)
            {
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                    Error("Expected '('");
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.identifier)
                    Error("Expected identifier");
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.equals)
                    Error("Expected '='");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.kw_to)
                    Error("Expected 'to'");
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    Error("Expected ')'");
                DropToken();

                Stmt();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_return)
            {
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    Error("Expected ';'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftBrace)
            {
                DropToken();

                Body();

                if (CurrentToken.Type != TSLangTokenTypes.rightBrace)
                    Error("Expected '}'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_def)
            {
                Func();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_var)
            {
                DefVar();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    Error("Expected ';'");
                DropToken();
            }
            else
            {
                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    Error("Expected ';'");
                DropToken();
            }
        }

        private void DefVar()
        {
            /*
             * var Type iden
             * var Type iden = Expr
             */

            if (CurrentToken.Type != TSLangTokenTypes.kw_var)
                Error("Expected 'var'");
            DropToken();

            Type();

            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                Error("Expected identifier");
            DropToken();

            if (CurrentToken.Type == TSLangTokenTypes.equals)
            {
                DropToken();

                Expr();
            }
        }

        private void FList()
        {
             /*
              * <empty>
              * Type iden
              * Type iden , FList
              */

            if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                return;

            Type();

            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                Error("Expected identifier");
            DropToken();

            if (CurrentToken.Type == TSLangTokenTypes.comma)
            {
                DropToken();

                FList();
            }
        }

        private void CList()
        {
            /*
             * <empty>
             * Expr
             * Expr , CList
             */

            if (CurrentToken.Type == TSLangTokenTypes.leftBracket
                || CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
            {
                return;
            }

            Expr();

            if (CurrentToken.Type == TSLangTokenTypes.comma)
            {
                DropToken();

                CList();
            }
        }

        private void Type()
        {
            /*
             * int
             * vector
             * str
             * null
             */

            if (CurrentToken.Type == TSLangTokenTypes.kw_int
                || CurrentToken.Type == TSLangTokenTypes.kw_vector
                || CurrentToken.Type == TSLangTokenTypes.kw_str
                || CurrentToken.Type == TSLangTokenTypes.kw_null)
            {
                DropToken();
            }
            else
            {
                Error("Expected type");
            }
        }

        private void Expr()
        {
            /*
             * iden
             * iden [ Expr ]
             * iden = Expr
             * iden ( Clist )
             * number
             * string
             * [ Clist ]
             * ( Expr )
             * ! Expr
             * + Expr
             * - Expr
             * Expr * Expr
             * Expr / Expr
             * Expr % Expr
             * Expr + Expr
             * Expr - Expr
             * Expr > Expr
             * Expr < Expr
             * Expr >= Expr
             * Expr <= Expr
             * Expr == Expr
             * Expr != Expr
             * Expr && Expr
             * Expr || Expr
             * Expr ? Expr : Expr
             */

            LOrExpr();

            while (CurrentToken.Type == TSLangTokenTypes.questionMark)
            {
                DropToken();

                LOrExpr();

                if (CurrentToken.Type != TSLangTokenTypes.colon)
                    Error("Expected ':'");
                DropToken();

                LOrExpr();
            }
        }

        private void LOrExpr()
        {
            LAndExpr();
            while (CurrentToken.Type == TSLangTokenTypes.logicalOr)
            {
                DropToken();

                LAndExpr();
            }
        }

        private void LAndExpr()
        {
            EqNeqExpr();
            while (CurrentToken.Type == TSLangTokenTypes.logicalAnd)
            {
                DropToken();

                EqNeqExpr();
            }
        }

        private void EqNeqExpr()
        {
            CompareExpr();
            while (CurrentToken.Type == TSLangTokenTypes.doubleEquals
                || CurrentToken.Type == TSLangTokenTypes.notEquals)
            {
                DropToken();

                CompareExpr();
            }
        }

        private void CompareExpr()
        {
            AddSubExpr();
            while (CurrentToken.Type == TSLangTokenTypes.lessThan
                || CurrentToken.Type == TSLangTokenTypes.greaterThan
                || CurrentToken.Type == TSLangTokenTypes.lessThanOrEqual
                || CurrentToken.Type == TSLangTokenTypes.greaterThan)
            {
                DropToken();

                AddSubExpr();
            }
        }

        private void AddSubExpr()
        {
            MulDivModExpr();
            while (CurrentToken.Type == TSLangTokenTypes.plus
                || CurrentToken.Type == TSLangTokenTypes.minus)
            {
                DropToken();

                MulDivModExpr();
            }
        }

        private void MulDivModExpr()
        {
            FinalExpr();
            while (CurrentToken.Type == TSLangTokenTypes.asterisk
                || CurrentToken.Type == TSLangTokenTypes.slash
                || CurrentToken.Type == TSLangTokenTypes.percent)
            {
                DropToken();

                FinalExpr();
            }
        }

        private void FinalExpr()
        {
            if (CurrentToken.Type == TSLangTokenTypes.identifier)
            {
                DropToken();

                if (CurrentToken.Type == TSLangTokenTypes.leftBracket)
                {
                    DropToken();

                    Expr();

                    if (CurrentToken.Type != TSLangTokenTypes.rightBracket)
                        Error("Expected ']'");
                    DropToken();
                }
                else if (CurrentToken.Type == TSLangTokenTypes.equals)
                {
                    DropToken();

                    Expr();
                }
                else if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
                {
                    DropToken();

                    CList();

                    if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                        Error("Expected ')'");
                    DropToken();
                }
            }
            else if (CurrentToken.Type == TSLangTokenTypes.literal_integer
                || CurrentToken.Type == TSLangTokenTypes.literal_string_doubleQuote
                || CurrentToken.Type == TSLangTokenTypes.literal_string_singleQuote)
            {
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftBracket)
            {
                DropToken();

                CList();

                if (CurrentToken.Type != TSLangTokenTypes.rightBracket)
                    Error("Expected ']'");
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.exclamationMark
                || CurrentToken.Type == TSLangTokenTypes.plus
                || CurrentToken.Type == TSLangTokenTypes.minus)
            {
                DropToken();

                Expr();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
            {
                DropToken();

                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    Error("Expected ')'");
                DropToken();
            }
            else
            {
                Error("Expected expression");
            }
        }
    }
}
