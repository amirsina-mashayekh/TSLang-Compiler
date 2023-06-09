using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokenizer;

namespace Parser.SymbolTableUtil
{
    /// <summary>
    /// Represents type of an <see cref="ISymbol"/>.
    /// </summary>
    public class SymbolType
    {
        /// <summary>
        /// Gets name of the type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets keyword associated with this type.
        /// </summary>
        public TokenType TokenType { get; }

        /// <summary>
        /// Initializes new instance of the <see cref="SymbolType"/> class.
        /// </summary>
        /// <param name="name">Name of the type.</param>
        /// <param name="type">A <see cref="Tokenizer.TokenType"/> representing keyword associated with this type.</param>
        public SymbolType(string name, TokenType type)
        {
            Name = name;
            TokenType = type;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
