﻿namespace Parser.SymbolTableUtil
{
    /// <summary>
    /// Represents a variable in symbol table.
    /// </summary>
    internal class Variable : ISymbol
    {
        /// <summary>
        /// Gets identifier (name) of the variable.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets type of the variable.
        /// </summary>
        public SymbolType Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> struct.
        /// </summary>
        /// <param name="identifier">Identifier (name) of the variable.</param>
        /// <param name="type">Type of the variable. Cannot be <see cref="SymbolType.null_type"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public Variable(string identifier, SymbolType type)
        {
            if (type == TSLangSymbolTypes.null_type)
            {
                throw new ArgumentException("Variable type cannot be null.");
            }
            Identifier = identifier;
            Type = type;
        }
    }
}
