using Parser.SymbolTableUtil;
using Tokenizer;

namespace Parser
{
    /// <summary>
    /// Represents a TSLang source code parser.
    /// </summary>
    public partial class TSLangParser
    {
        /// <summary>
        /// A <see cref="TSLangTokenizer"/> which provides tokens of code.
        /// </summary>
        private readonly TSLangTokenizer tokenizer;

        /// <summary>
        /// A <see cref="TextWriter"/> to write errors on it.
        /// </summary>
        private readonly TextWriter errorStream;

        /// <summary>
        /// The last token provided by tokenizer
        /// </summary>
        private Token lastToken;

        /// <summary>
        /// Gets current token.
        /// </summary>
        private Token CurrentToken => lastToken;

        /// <summary>
        /// Root symbol table of the code.
        /// </summary>
        private readonly SymbolTable rootSymbolTable;

        /// <summary>
        /// Symbol table for current scope.
        /// </summary>
        private SymbolTable currentScopeSymbolTable;

        /// <summary>
        /// Gets whether an error occured while parsing the code.
        /// </summary>
        public bool HasError { get; private set; }

        /// <summary>
        /// Last token which caused syntax error.
        /// </summary>
        private Token lastErrorToken;

        /// <summary>
        /// Gets whether parsing is done.
        /// </summary>
        public bool Done { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TSLangParser"/> class
        /// for the provided source code stream.
        /// </summary>
        /// <param name="stream">A <see cref="StreamReader"/> providing source code.</param>
        public TSLangParser(StreamReader stream, TextWriter errorStream)
        {
            tokenizer = new TSLangTokenizer(stream);
            this.errorStream = errorStream;

            HasError = false;
            Done = false;
            lastToken = new Token(TSLangTokenTypes.comment, "", 0, 0);
            lastErrorToken = lastToken;
            rootSymbolTable = new();
            currentScopeSymbolTable = rootSymbolTable;

            DropToken();
        }

        /// <summary>
        /// Starts parsing the provided souce code.
        /// </summary>
        public void Parse()
        {
            try
            {
                Prog();
            }
            catch { }
        }

        /// <summary>
        /// Gets next token from <see cref="tokenizer"/> (if available)
        /// and puts it in <see cref="CurrentToken"/>.
        /// </summary>
        private void DropToken()
        {
            if (tokenizer.EndOfStream)
            {
                Done = true;
                lastToken = new Token(TSLangTokenTypes.invalid, "", lastToken.Column, lastToken.Line);
                return;
            }

            do
            {
                lastToken = tokenizer.NextToken();
                if (lastToken.Type == TSLangTokenTypes.invalid)
                {
                    SyntaxError("Invalid token: " + lastToken.Value);
                }
            } while (lastToken.Type == TSLangTokenTypes.comment || lastToken.Type == TSLangTokenTypes.invalid);
        }

        /// <summary>
        /// Prints syntax error message and its location
        /// to the provided <see cref="errorStream"/>.
        /// </summary>
        /// <param name="message">Message of error.</param>
        private void SyntaxError(string message)
        {
            HasError = true;

            if (lastErrorToken == CurrentToken)
            {
                DropToken();
                if (Done)
                    throw new Exception("Reached end of file.");
                return;
            }

            lastErrorToken = CurrentToken;

            errorStream.WriteLine(
                CurrentToken.Line.ToString() + ":" +
                CurrentToken.Column.ToString() + ":\t" +
                message);
        }

        /// <summary>
        /// Prints semantic error message and its location
        /// to the provided <see cref="errorStream"/>.
        /// </summary>
        /// <param name="message">Message of error.</param>
        private void SemanticError(string message)
        {
            HasError = true;

            errorStream.WriteLine(
                CurrentToken.Line.ToString() + ":" +
                CurrentToken.Column.ToString() + ":\t" +
                message);

        }

        /// <summary>
        /// Prints semantic error message and its location
        /// to the provided <see cref="errorStream"/>.
        /// </summary>
        /// <param name="message">Message of error.</param>
        /// <param name="line">Line of error.</param>
        /// <param name="col">Column of error.</param>
        private void SemanticError(string message, int line, int col)
        {
            HasError = true;

            errorStream.WriteLine(
                line + ":" +
                col + ":\t" +
                message);

        }
    }
}
