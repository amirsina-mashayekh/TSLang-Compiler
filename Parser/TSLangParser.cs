﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokenizer;

namespace Parser
{
    public partial class TSLangParser
    {
        private readonly TSLangTokenizer tokenizer;

        private Token lastToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="TSLangParser"/> class
        /// for the provided source code stream.
        /// </summary>
        /// <param name="stream">A <see cref="StreamReader"/> providing source code.</param>
        public TSLangParser(StreamReader stream)
        {
            tokenizer = new TSLangTokenizer(stream);
            lastToken = new Token(TSLangTokenTypes.comment, "", 0, 0);
            DropToken();
        }

        /// <summary>
        /// Starts parsing the provided souce code.
        /// </summary>
        public void Parse()
        {
            Prog();
        }

        private Token CurrentToken => lastToken;

        private void DropToken()
        {
            do
            {
                lastToken = tokenizer.NextToken();
            } while (lastToken.Type == TSLangTokenTypes.comment);
        }

        private void Error(string message)
        {
            throw new Exception(
                "Ln: " + CurrentToken.Line.ToString() + ", " +
                "Ch: " + CurrentToken.Column.ToString() + ", " +
               message);
        }
    }
}