namespace Parser.SymbolTableUtil
{
    /// <summary>
    /// Represents a symbol which has a name and type.
    /// </summary>
    internal interface ISymbol
    {
        /// <summary>
        /// Gets name of the symbol.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets type of the symbol.
        /// </summary>
        public SymbolTable.TSLangTypes Type { get; }
    }
}
