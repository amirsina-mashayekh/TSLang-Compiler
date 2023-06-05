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
            //
            // Func Prog
            if (tokenizer.EndOfStream)
                return;
            else if (GetToken().Type != TSLangTokenTypes.kw_def)
                Error("Expected 'def'");

            Func();
            Prog();
        }

        private void Func()
        {
            // def Type iden ( Flist ) { Body }
            // def Type iden ( Flist ) return Expr;

            if (GetToken().Type != TSLangTokenTypes.kw_def)
                Error("Expected 'def'");
            DropToken();

            Type();
            
            if (GetToken().Type != TSLangTokenTypes.identifier)
                Error("Expected identifier");
            DropToken();

            if (GetToken().Type != TSLangTokenTypes.leftParenthesis)
                Error("Expected '('");
            DropToken();

            FList();

            if (GetToken().Type != TSLangTokenTypes.rightParenthesis)
                Error("Expected ')'");
            DropToken();

            if (GetToken().Type == TSLangTokenTypes.leftBrace)
            {
                DropToken();

                Body();

                if (GetToken().Type != TSLangTokenTypes.rightBrace)
                    Error("Expected '}'");
                DropToken();
            }
            else if (GetToken().Type == TSLangTokenTypes.kw_return)
            {
                DropToken();

                Expr();

                if (GetToken().Type != TSLangTokenTypes.semicolon)
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
            throw new NotImplementedException();
        }

        private void Stmt()
        {
            throw new NotImplementedException();
        }

        private void DefVar()
        {
            throw new NotImplementedException();
        }

        private void FList()
        {
            throw new NotImplementedException();
        }

        private void CList()
        {
            throw new NotImplementedException();
        }

        private void Type()
        {
            throw new NotImplementedException();
        }

        private void Expr()
        {
            throw new NotImplementedException();
        }
    }
}
