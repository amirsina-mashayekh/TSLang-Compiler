namespace Parser.SymbolTableUtil
{
    /// <summary>
    /// Represents a symbol table for TSLang.
    /// </summary>
    internal class SymbolTable
    {
        /// <summary>
        /// Gets the symbol table of the upper scope.
        /// </summary>
        public SymbolTable? UpperScope { get; }

        /// <summary>
        /// Includes symbols of the symbol table.
        /// </summary>
        private readonly Dictionary<string, ISymbol> symbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolTable"/> class.
        /// </summary>
        public SymbolTable()
        {
            symbols = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolTable"/> class.
        /// </summary>
        /// <param name="upperScope">The symbol table for the upper scope.</param>
        public SymbolTable(SymbolTable? upperScope)
        {
            symbols = new();
            UpperScope = upperScope;
        }

        /// <summary>
        /// Checks whether a symbol with provided identifier (name) exists in the symbol table.
        /// </summary>
        /// <param name="identifier">Identifier to check for.</param>
        /// <returns>
        /// True if a symbol with the specified identifier exists in the symbol table.
        /// False otherwise.
        /// </returns>
        public bool Exists(string identifier)
        {
            bool e;
            SymbolTable? st = this;

            do
            {
                e = st.symbols.ContainsKey(identifier);
                st = st.UpperScope;
            } while (!e && st is not null);

            return e;
        }

        /// <summary>
        /// Adds new symbol to the symbol table.
        /// </summary>
        /// <param name="symbol">The symbol to be added to the symbol table.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(ISymbol symbol)
        {
            if (Exists(symbol.Identifier))
                throw new ArgumentException("A symbol with the same identifier already exists in the symbol table.");
            symbols.Add(symbol.Identifier, symbol);
        }

        /// <summary>
        /// Gets the symbol with specified identifier (name) from the symbol table.
        /// </summary>
        /// <param name="identifier">Identifier (name) of the symbol.</param>
        /// <returns>The symbol with the specified identifier.</returns>
        /// <exception cref="ArgumentException"></exception>
        public ISymbol Get(string identifier)
        {
            SymbolTable? st = this;

            do
            {
                if (st.symbols.ContainsKey(identifier))
                    return st.symbols[identifier];
                st = st.UpperScope;
            } while (st is not null);

            throw new ArgumentException("Identifier does not exist in the symbol table.");
        }
    }
}
