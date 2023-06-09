using Parser.SymbolTableUtil;
using Tokenizer;
using static Parser.SymbolTableUtil.SymbolTable;

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
            SymbolType type = TSLangSymbolTypes.invalid_type;
            List<Variable>? parameters = null;
            string? id = null;
            var prevScope = currentSymTab;

            if (CurrentToken.Type != TSLangTokenTypes.kw_def)
            {
                SyntaxError("Expected 'def'");

                TokenType[] recoveryPoint =
                {
                    TSLangTokenTypes.kw_def,
                    TSLangTokenTypes.leftParenthesis,
                    TSLangTokenTypes.rightParenthesis,
                    TSLangTokenTypes.leftBrace,
                    TSLangTokenTypes.kw_return,
                };

                do
                {
                    DropToken();
                    if (Done)
                        return;
                } while (!recoveryPoint.Contains(CurrentToken.Type));

                if (CurrentToken.Type == TSLangTokenTypes.kw_def)
                    goto rp1;
                else if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
                    goto rp2;
                else if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    goto rp3;
                else
                    goto rp4;
            }
        rp1:
            DropToken();

            type = Type();
            
            if (CurrentToken.Type != TSLangTokenTypes.identifier)
            {
                SyntaxError("Expected identifier");

                TokenType[] recoveryPoint =
                {
                    TSLangTokenTypes.leftParenthesis,
                    TSLangTokenTypes.rightParenthesis,
                    TSLangTokenTypes.leftBrace,
                    TSLangTokenTypes.kw_return,
                };

                do
                {
                    DropToken();
                    if (Done)
                        return;
                } while (!recoveryPoint.Contains(CurrentToken.Type));
                
                if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
                    goto rp2;
                else if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    goto rp3;
                else
                    goto rp4;
            }
            else
            {
                id = CurrentToken.Value;
            }
            DropToken();

            if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
            {
                SyntaxError("Expected '('");

                TokenType[] recoveryPoint =
                {
                    TSLangTokenTypes.leftParenthesis,
                    TSLangTokenTypes.rightParenthesis,
                    TSLangTokenTypes.leftBrace,
                    TSLangTokenTypes.kw_return,
                };

                do
                {
                    DropToken();
                    if (Done)
                        return;
                } while (!recoveryPoint.Contains(CurrentToken.Type));

                if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
                    goto rp2;
                else if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    goto rp3;
                else
                    goto rp4;
            }
        rp2:
            DropToken();

            parameters = FList();

            if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
            {
                SyntaxError("Expected ')'");

                TokenType[] recoveryPoint =
                {
                    TSLangTokenTypes.rightParenthesis,
                    TSLangTokenTypes.leftBrace,
                    TSLangTokenTypes.kw_return,
                };

                do
                {
                    DropToken();
                    if (Done)
                        return;
                } while (!recoveryPoint.Contains(CurrentToken.Type));

                if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    goto rp3;
                else
                    goto rp4;
            }
        rp3:
            DropToken();

            if (type != TSLangSymbolTypes.invalid_type && parameters is not null && id is not null)
            {
                if (currentSymTab.Exists(id))
                    SemanticError($"Function with name '{id}' already exists in this in current scope");
                else
                {
                    currentSymTab.Add(new Function(id, type, parameters));
                    currentSymTab = new(prevScope);
                    foreach (ISymbol item in parameters)
                    {
                        currentSymTab.Add(item);
                    }
                }
            }

        rp4:
            if (CurrentToken.Type == TSLangTokenTypes.leftBrace)
            {
                DropToken();

                Body(type);
                if (CurrentToken.Type != TSLangTokenTypes.rightBrace)
                    SyntaxError("Expected '}'");
                else DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_return)
            {
                DropToken();

                if (type != TSLangSymbolTypes.null_type)
                {
                    var t = Expr();
                    if (t != type)
                    {
                        SemanticError($"Expected '{type}' return expression");
                    }
                }

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    SyntaxError("Expected ';'");
                else DropToken();
            }
            else
            {
                SyntaxError("Expected '{' or 'return'");
                while (CurrentToken.Type != TSLangTokenTypes.kw_def && !Done)
                    DropToken();
            }
            
            currentSymTab = prevScope;
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// *EMPTY*
        /// Stmt Body
        /// </code>
        /// </summary>
        /// <param name="returnType">Expected type of return expression for functions.</param>
        private void Body(SymbolType returnType)
        {
            if (CurrentToken.Type == TSLangTokenTypes.rightBrace)
                // *EMPTY*
                return;

            var tmp = currentSymTab;
            currentSymTab = new(tmp);

            Stmt(returnType);

            Body(returnType);

            currentSymTab = tmp;
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
        /// <param name="returnType">Expected type of return expression for functions.</param>
        private void Stmt(SymbolType returnType)
        {
            if (CurrentToken.Type == TSLangTokenTypes.kw_if)
            {
                // if ( Expr ) Stmt
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                {
                    SyntaxError("Expected '('");

                    TokenType[] recoveryPoint =
                    {
                        TSLangTokenTypes.leftParenthesis,
                        TSLangTokenTypes.rightParenthesis,
                    };

                    while (!recoveryPoint.Contains(CurrentToken.Type))
                    {
                        DropToken();
                        if (Done)
                            return;
                    }

                    if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    {
                        DropToken();
                        goto rp1;
                    }
                }
                DropToken();

                var t = Expr();

                if (t != TSLangSymbolTypes.integer_type)
                {
                    SemanticError("Condition expression type must be integer");
                }

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                else DropToken();

            rp1:
                Stmt(returnType);

                if (CurrentToken.Type == TSLangTokenTypes.kw_else)
                {
                    // if ( Expr ) Stmt else Stmt
                    DropToken();

                    Stmt(returnType);
                }
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_while)
            {
                // while ( Expr ) Stmt
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                {
                    SyntaxError("Expected '('");

                    TokenType[] recoveryPoint =
                    {
                        TSLangTokenTypes.leftParenthesis,
                        TSLangTokenTypes.rightParenthesis,
                    };

                    while (!recoveryPoint.Contains(CurrentToken.Type))
                    {
                        DropToken();
                        if (Done)
                            return;
                    }

                    if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    {
                        DropToken();
                        goto rp1;
                    }
                }
                DropToken();

                var t = Expr();

                if (t != TSLangSymbolTypes.integer_type)
                {
                    SemanticError("Condition expression type must be integer");
                }

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                else DropToken();
            rp1:
                Stmt(returnType);
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_for)
            {
                // for ( iden = Expr to Expr ) Stmt
                DropToken();

                string? id = null;
                SymbolType typeStart = TSLangSymbolTypes.invalid_type;
                SymbolType typeEnd;
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                var prevScope = currentSymTab;

                if (CurrentToken.Type != TSLangTokenTypes.leftParenthesis)
                {
                    SyntaxError("Expected '('");

                    TokenType[] recoveryPoint =
                    {
                        TSLangTokenTypes.leftParenthesis,
                        TSLangTokenTypes.equals,
                        TSLangTokenTypes.kw_to,
                        TSLangTokenTypes.rightParenthesis,
                    };

                    while (!recoveryPoint.Contains(CurrentToken.Type))
                    {
                        DropToken();
                        if (Done)
                            return;
                    }

                    if (CurrentToken.Type == TSLangTokenTypes.equals)
                        goto rp1;
                    else if (CurrentToken.Type == TSLangTokenTypes.kw_to)
                        goto rp2;
                    else if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    {
                        DropToken();
                        goto rp3;
                    }
                }
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.identifier)
                {
                    SyntaxError("Expected identifier");

                    TokenType[] recoveryPoint =
                    {
                        TSLangTokenTypes.equals,
                        TSLangTokenTypes.kw_to,
                        TSLangTokenTypes.rightParenthesis,
                    };

                    while (!recoveryPoint.Contains(CurrentToken.Type))
                    {
                        DropToken();
                        if (Done)
                            return;
                    }

                    if (CurrentToken.Type == TSLangTokenTypes.equals)
                        goto rp1;
                    else if (CurrentToken.Type == TSLangTokenTypes.kw_to)
                        goto rp2;
                    else if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    {
                        DropToken();
                        goto rp3;
                    }
                }
                else
                {
                    id = CurrentToken.Value;
                }
                DropToken();

                if (CurrentToken.Type != TSLangTokenTypes.equals)
                {
                    SyntaxError("Expected '='");

                    TokenType[] recoveryPoint =
                    {
                        TSLangTokenTypes.equals,
                        TSLangTokenTypes.kw_to,
                        TSLangTokenTypes.rightParenthesis,
                    };

                    while (!recoveryPoint.Contains(CurrentToken.Type))
                    {
                        DropToken();
                        if (Done)
                            return;
                    }

                    if (CurrentToken.Type == TSLangTokenTypes.equals)
                        goto rp1;
                    else if (CurrentToken.Type == TSLangTokenTypes.kw_to)
                        goto rp2;
                    else if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    {
                        DropToken();
                        goto rp3;
                    }
                }
            rp1:
                DropToken();

                typeStart = Expr();
                if (typeStart != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid type '{typeStart}' as start of loop range", l, c);
                    typeStart = TSLangSymbolTypes.invalid_type;
                }

                if (CurrentToken.Type != TSLangTokenTypes.kw_to)
                {
                    SyntaxError("Expected 'to'");

                    TokenType[] recoveryPoint =
                    {
                        TSLangTokenTypes.kw_to,
                        TSLangTokenTypes.rightParenthesis,
                    };

                    while (!recoveryPoint.Contains(CurrentToken.Type))
                    {
                        DropToken();
                        if (Done)
                            return;
                    }

                    if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                    {
                        DropToken();
                        goto rp3;
                    }
                }
            rp2:
                DropToken();

                typeEnd = Expr();
                if (typeEnd != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid type '{typeEnd}' as end of loop range", l, c);
                    typeEnd = TSLangSymbolTypes.invalid_type;
                }

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                else DropToken();

                if (id is not null && typeStart != TSLangSymbolTypes.invalid_type && typeEnd != TSLangSymbolTypes.invalid_type)
                {
                    currentSymTab = new(prevScope);
                    if (currentSymTab.Exists(id))
                    {
                        if (currentSymTab.Get(id).Type != typeStart)
                            SemanticError("Invalid loop iterator type");
                    }
                    else
                    {
                        currentSymTab.Add(new Variable(id, typeStart));
                    }
                }

            rp3:
                Stmt(returnType);

                currentSymTab = prevScope;
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_return)
            {
                // return Expr ;
                DropToken();

                if (returnType != TSLangSymbolTypes.null_type)
                {
                    var t = Expr();
                    if (t != returnType)
                    {
                        SemanticError($"Expected '{returnType}' return expression");
                    }
                }

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    SyntaxError("Expected ';'");
                else DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftBrace)
            {
                // { Body }
                DropToken();

                Body(returnType);

                if (CurrentToken.Type != TSLangTokenTypes.rightBrace)
                    SyntaxError("Expected '}'");
                else DropToken();
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
                else DropToken();
            }
            else
            {
                // Expr ;
                Expr();

                if (CurrentToken.Type != TSLangTokenTypes.semicolon)
                    SyntaxError("Expected ';'");
                else DropToken();
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
            else DropToken();

            var type = Type();

            if (type == TSLangSymbolTypes.null_type)
                SemanticError("Cannot define null type variable");

            string? id = null;
            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                SyntaxError("Expected identifier");
            else
            {
                id = CurrentToken.Value;
                DropToken();
            }

            if (type is not null && type != TSLangSymbolTypes.null_type && id is not null)
            {
                if (currentSymTab.Exists(id))
                    SemanticError($"Variable with name '{id}' already exists in current scope");
                else
                    currentSymTab.Add(new Variable(id, type));
            }

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
        private List<Variable> FList()
        {
            List<Variable> list = new();

            if (CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
                // *EMPTY*
                return list;


            var type = Type();

            if (type == TSLangSymbolTypes.null_type)
                SemanticError("Cannot define null type function parameter");

            string? id = null;
            if (CurrentToken.Type != TSLangTokenTypes.identifier)
                SyntaxError("Expected identifier");
            else
            {
                id = CurrentToken.Value;
                DropToken();
            }

            if (type is not null && type != TSLangSymbolTypes.null_type && id is not null)
            {
                list.Add(new Variable (id, type));
            }

            if (CurrentToken.Type == TSLangTokenTypes.comma)
            {
                // Type iden, FList
                DropToken();

                var next = FList();
                if (next is not null)
                {
                    foreach (Variable v in next)
                    {
                        if (list.Any(x => x.Identifier == v.Identifier))
                            SemanticError($"Parameter with name '{v.Identifier}' already exists");
                        list.Add(v);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// *EMPTY*
        /// Expr
        /// Expr, CList
        /// </code>
        /// </summary>
        private List<SymbolType> CList()
        {
            List<SymbolType> list = new();

            if (CurrentToken.Type == TSLangTokenTypes.rightBracket
                || CurrentToken.Type == TSLangTokenTypes.rightParenthesis)
            {
                // *EMPTY*
                return list;
            }

            var t = Expr();
            list.Add(t);

            if (CurrentToken.Type == TSLangTokenTypes.comma)
            {
                // Expr, CList
                DropToken();

                var lst = CList();
                list.AddRange(lst);
            }

            return list;
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
        private SymbolType Type()
        {
            if (CurrentToken.Type == TSLangTokenTypes.kw_int)
            {
                DropToken();
                return TSLangSymbolTypes.integer_type;
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_vector)
            {
                DropToken();
                return TSLangSymbolTypes.vector_type;
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_str)
            {
                DropToken();
                return TSLangSymbolTypes.string_type;
            }
            else if (CurrentToken.Type == TSLangTokenTypes.kw_null)
            {
                DropToken();
                return TSLangSymbolTypes.null_type;
            }
            else
            {
                SyntaxError("Expected type");
                return TSLangSymbolTypes.invalid_type;
            }
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr ? Expr : Expr
        /// </code>
        /// </summary>
        private SymbolType Expr()
        {
            var type = LOrExpr();

            while (CurrentToken.Type == TSLangTokenTypes.questionMark)
            {
                // Expr ? Expr : Expr
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                DropToken();

                if (type != TSLangSymbolTypes.integer_type)
                    SemanticError("Condition expression type must be integer", l, c);

                var typeL = Expr();

                if (CurrentToken.Type != TSLangTokenTypes.colon)
                    SyntaxError("Expected ':'");
                else DropToken();

                var typeR = Expr();

                if (typeL != typeR)
                {
                    SemanticError($"Invalid operation: '{type}' ? '{typeL}' : '{typeR}' ");
                    type = TSLangSymbolTypes.invalid_type;
                }
                else
                {
                    type = typeL;
                }
            }

            return type;
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr || Expr
        /// </code>
        /// </summary>
        private SymbolType LOrExpr()
        {
            var typeL = LAndExpr();
            var type = typeL;

            while (CurrentToken.Type == TSLangTokenTypes.logicalOr)
            {
                // Expr || Expr
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                string op = CurrentToken.Value;
                DropToken();

                var typeR = LAndExpr();

                if (typeL != TSLangSymbolTypes.integer_type || typeR != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid operation: '{typeL}' {op} '{typeR}'", l, c);
                    type = TSLangSymbolTypes.invalid_type;
                }
                else if (type != TSLangSymbolTypes.invalid_type)
                {
                    type = typeL;
                }

                typeL = typeR;
            }

            return type;
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr && Expr
        /// </code>
        /// </summary>
        private SymbolType LAndExpr()
        {
            var typeL = EqNeqExpr();
            var type = typeL;

            while (CurrentToken.Type == TSLangTokenTypes.logicalAnd)
            {
                // Expr && Expr
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                string op = CurrentToken.Value;
                DropToken();

                var typeR = EqNeqExpr();

                if (typeL != TSLangSymbolTypes.integer_type || typeR != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid operation: '{typeL}' {op} '{typeR}'", l, c);
                    type = TSLangSymbolTypes.invalid_type;
                }
                else if (type != TSLangSymbolTypes.invalid_type)
                {
                    type = typeL;
                }

                typeL = typeR;
            }

            return type;
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr == Expr
        /// Expr != Expr
        /// </code>
        /// </summary>
        private SymbolType EqNeqExpr()
        {
            var typeL = CompareExpr();
            var type = typeL;

            while (CurrentToken.Type == TSLangTokenTypes.doubleEquals
                || CurrentToken.Type == TSLangTokenTypes.notEquals)
            {
                // Expr == Expr
                // Expr != Expr
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                string op = CurrentToken.Value;
                DropToken();

                var typeR = CompareExpr();

                if (typeL != TSLangSymbolTypes.integer_type || typeR != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid operation: '{typeL}' {op} '{typeR}'", l, c);
                    type = TSLangSymbolTypes.invalid_type;
                }
                else if (type != TSLangSymbolTypes.invalid_type)
                {
                    type = typeL;
                }

                typeL = typeR;
            }

            return type;
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
        private SymbolType CompareExpr()
        {
            var typeL = AddSubExpr();
            var type = typeL;

            while (CurrentToken.Type == TSLangTokenTypes.lessThan
                || CurrentToken.Type == TSLangTokenTypes.greaterThan
                || CurrentToken.Type == TSLangTokenTypes.lessThanOrEqual
                || CurrentToken.Type == TSLangTokenTypes.greaterThanOrEqual)
            {
                // Expr > Expr
                // Expr < Expr
                // Expr >= Expr
                // Expr <= Expr
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                string op = CurrentToken.Value;
                DropToken();

                var typeR = AddSubExpr();

                if (typeL != TSLangSymbolTypes.integer_type || typeR != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid operation: '{typeL}' {op} '{typeR}'", l, c);
                    type = TSLangSymbolTypes.invalid_type;
                }
                else if (type != TSLangSymbolTypes.invalid_type)
                {
                    type = typeL;
                }

                typeL = typeR;
            }

            return type;
        }

        /// <summary>
        /// Parses the following grammars:
        /// <code>
        /// Expr
        /// Expr + Expr
        /// Expr - Expr
        /// </code>
        /// </summary>
        private SymbolType AddSubExpr()
        {
            var typeL = MulDivModExpr();
            var type = typeL;

            while (CurrentToken.Type == TSLangTokenTypes.plus
                || CurrentToken.Type == TSLangTokenTypes.minus)
            {
                // Expr + Expr
                // Expr - Expr
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                string op = CurrentToken.Value;
                DropToken();

                var typeR = MulDivModExpr();

                if (typeL != typeR)
                {
                    SemanticError($"Invalid operation: '{typeL}' {op} '{typeR}'", l, c);
                    type = TSLangSymbolTypes.invalid_type;
                }
                else if (type != TSLangSymbolTypes.invalid_type)
                {
                    type = typeL;
                }

                typeL = typeR;
            }

            return type;
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
        private SymbolType MulDivModExpr()
        {
            var typeL = FinalExpr();
            var type = typeL;

            while (CurrentToken.Type == TSLangTokenTypes.asterisk
                || CurrentToken.Type == TSLangTokenTypes.slash
                || CurrentToken.Type == TSLangTokenTypes.percent)
            {
                // Expr * Expr
                // Expr / Expr
                // Expr % Expr
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                string op = CurrentToken.Value;
                DropToken();

                var typeR = FinalExpr();

                if (typeL != TSLangSymbolTypes.integer_type || typeR != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid operation: '{typeL}' {op} '{typeR}'", l, c);
                    type = TSLangSymbolTypes.invalid_type;
                }
                else if (type != TSLangSymbolTypes.invalid_type)
                {
                    type = typeL;
                }

                typeL = typeR;
            }

            return type;
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
        private SymbolType FinalExpr()
        {
            SymbolType type;

            if (CurrentToken.Type == TSLangTokenTypes.identifier)
            {
                // iden
                var id = CurrentToken.Value;
                bool found = currentSymTab.Exists(id);
                ISymbol? symbol = null;

                if (found)
                {
                    symbol = currentSymTab.Get(id);
                    type = symbol.Type;
                }
                else
                {
                    SemanticError($"Cannot find variable or function '{id}' in current scope");
                    type = TSLangSymbolTypes.invalid_type;
                }
                DropToken();

                if (CurrentToken.Type == TSLangTokenTypes.leftBracket)
                {
                    // iden [ Expr ]
                    type = TSLangSymbolTypes.integer_type;

                    if (found && symbol!.Type != TSLangSymbolTypes.vector_type)
                    {
                        SemanticError($"'{id}' is not vector");
                    }
                    DropToken();

                    var indexType = Expr();
                    if (indexType != TSLangSymbolTypes.integer_type)
                        SemanticError($"Cannot use expression of type '{indexType}' as vector index");

                    if (CurrentToken.Type != TSLangTokenTypes.rightBracket)
                        SyntaxError("Expected ']'");
                    DropToken();

                    if (CurrentToken.Type == TSLangTokenTypes.equals)
                    {
                        // iden [ Expr ] = Expr
                        int l = CurrentToken.Line;
                        int c = CurrentToken.Column;
                        DropToken();

                        var assignedType = Expr();

                        if (assignedType != TSLangSymbolTypes.integer_type)
                            SemanticError($"Cannot assign expression of type '{assignedType}' to a vector member", l, c);
                    }
                }
                else if (CurrentToken.Type == TSLangTokenTypes.equals)
                {
                    // iden = Expr
                    int l = CurrentToken.Line;
                    int c = CurrentToken.Column;
                    DropToken();

                    var assignedType = Expr();

                    if (found)
                    {
                        var t = symbol!.Type;
                        if (symbol is Function)
                        {
                            SemanticError("Cannot assign to a function", l, c);
                        }
                        else if (assignedType != t)
                        {
                            SemanticError($"Cannot assign expression of type '{assignedType}' to a variable of type '{t}'", l, c);
                        }
                    }

                    type = assignedType;
                }
                else if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
                {
                    // iden ( CList )
                    int l = CurrentToken.Line;
                    int c = CurrentToken.Column;
                    DropToken();

                    if (found)
                    {
                        if (symbol is not Function)
                        {
                            SemanticError($"'{id}' is not a function", l, c);
                        }
                        else
                        {
                            type = symbol!.Type;
                        }
                    }
                    else
                    {
                        type = TSLangSymbolTypes.invalid_type;
                    }

                    var args = CList();

                    if (symbol is Function f)
                    {
                        int pc = f.ParametersCount;

                        if (pc != args.Count)
                        {
                            SemanticError($"Expected {pc} arguments, got {args.Count}", l, c);
                        }

                        pc = Math.Min(pc, args.Count);

                        for (int i = 0; i < pc; i++)
                        {
                            var et = f.Parameters[i].Type;
                            if (et != args[i])
                            {
                                SemanticError($"Arg{i + 1}: Expected argument of type '{et}', got '{args[i]}'", l, c);
                            }
                        }
                    }

                    if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                        SyntaxError("Expected ')'");
                    DropToken();
                }
            }
            else if (CurrentToken.Type == TSLangTokenTypes.literal_integer)
            {
                // number
                type = TSLangSymbolTypes.integer_type;
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.literal_string_doubleQuote
                || CurrentToken.Type == TSLangTokenTypes.literal_string_singleQuote)
            {
                // string
                type = TSLangSymbolTypes.string_type;
                DropToken();
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftBracket)
            {
                // [ CList ]
                type = TSLangSymbolTypes.vector_type;
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
                int l = CurrentToken.Line;
                int c = CurrentToken.Column;
                string op = CurrentToken.Value;
                DropToken();

                var t = Expr();

                if (t != TSLangSymbolTypes.integer_type)
                {
                    SemanticError($"Invalid operation '{op}' on type '{t}'");
                    type = TSLangSymbolTypes.invalid_type;
                }
                else
                {
                    type = t;
                }
            }
            else if (CurrentToken.Type == TSLangTokenTypes.leftParenthesis)
            {
                // ( Expr )
                DropToken();

                type = Expr();

                if (CurrentToken.Type != TSLangTokenTypes.rightParenthesis)
                    SyntaxError("Expected ')'");
                DropToken();
            }
            else
            {
                type = TSLangSymbolTypes.invalid_type;

                SyntaxError("Expected valid expression");

                TokenType[] recoveryTokens =
                {
                    TSLangTokenTypes.semicolon,
                    TSLangTokenTypes.rightParenthesis,
                    TSLangTokenTypes.rightBracket,
                    TSLangTokenTypes.rightBrace,
                    TSLangTokenTypes.kw_to,
                    TSLangTokenTypes.comma,
                };
                while (!recoveryTokens.Contains(CurrentToken.Type) && !Done)
                {
                    DropToken();
                }
            }

            return type;
        }
    }
}
