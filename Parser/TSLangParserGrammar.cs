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
            // <empty>
            // Func Prog

            if (tokenizer.EndOfStream)
                return;
            else if (CurrentToken.Type != TSLangTokenTypes.kw_def)
                Error("Expected 'def'");

            Func();
            Prog();
        }

        private void Func()
        {
            // def Type iden ( Flist ) { Body }
            // def Type iden ( Flist ) return Expr;

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
            // <empty>
            // Stmt Body

            if (CurrentToken.Type == TSLangTokenTypes.rightBrace)
                return;

            Stmt();

            Body();
        }

        private void Stmt()
        {
            throw new NotImplementedException();
        }

        private void DefVar()
        {
            // var Type iden
            // var Type iden = Expr

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
            // <empty>
            // Type iden
            // Type iden , Flist

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
            // <empty>
            // expr
            // expr , clist

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
            throw new NotImplementedException();
        }
    }
}
