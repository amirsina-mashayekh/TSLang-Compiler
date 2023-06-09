using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tokenizer;

namespace Parser.SymbolTableUtil
{
    /// <summary>
    /// Static class which includes Symbol types of TSLang.
    /// </summary>
    public static class TSLangSymbolTypes
    {
        public static readonly SymbolType
            integer_type = new("integer", TSLangTokenTypes.kw_int),

            string_type = new("string", TSLangTokenTypes.kw_str),

            vector_type = new("vector", TSLangTokenTypes.kw_vector),

            null_type = new("null", TSLangTokenTypes.kw_null),

            invalid_type = new("invalid", TSLangTokenTypes.invalid);
    }
}
