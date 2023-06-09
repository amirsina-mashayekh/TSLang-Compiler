using System.Collections.ObjectModel;

namespace Parser.SymbolTableUtil
{
    /// <summary>
    /// Represents a function in symbol table.
    /// </summary>
    internal class Function : ISymbol
    {
        /// <summary>
        /// Gets identifier (name) of the function.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets return type of the function.
        /// </summary>
        public SymbolTable.TSLangTypes Type { get; }

        /// <summary>
        /// Gets list of the function parameters.
        /// </summary>
        public ReadOnlyCollection<Variable> Parameters { get; }

        /// <summary>
        /// Gets count of the function parameters.
        /// </summary>
        public int ParametersCount => Parameters.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> struct.
        /// </summary>
        /// <param name="identifier">Identifier (name) of the function.</param>
        /// <param name="type">Return type of the function.</param>
        /// <param name="parameters">Array of the function parameters.</param>
        public Function(string identifier, SymbolTable.TSLangTypes type, Variable[] parameters)
        {
            Identifier = identifier;
            Type = type;
            Parameters = new ReadOnlyCollection<Variable>(parameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> struct.
        /// </summary>
        /// <param name="identifier">Identifier (name) of the function.</param>
        /// <param name="type">Return type of the function.</param>
        /// <param name="parameters">List of the function parameters.</param>
        public Function(string identifier, SymbolTable.TSLangTypes type, List<Variable> parameters)
        {
            Identifier = identifier;
            Type = type;
            Parameters = new ReadOnlyCollection<Variable>(parameters);
        }
    }
}
