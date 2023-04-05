using System.Text.RegularExpressions;

namespace Tokenizer
{
    /// <summary>
    /// Represents type of tokens in tokenizer.
    /// Includes a list of token types for TesLang.
    /// </summary>
    public class TokenType
    {
        /// <summary>
        /// Gets the name of the current <see cref="TokenType"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the regex object to match tokens with the current <see cref="TokenType"/>.
        /// </summary>
        public Regex Pattern { get; }

        /// <summary>
        /// Saves hash code of the current <see cref="TokenType"/>. 
        /// </summary>
        private readonly int _hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenType"/> class
        /// with the specified name and regex pattern and defaul regex options (compiled and culture invariant).
        /// </summary>
        /// <param name="name">The name of this token type.</param>
        /// <param name="pattern">The regular expression pattern to match tokens of this type.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TokenType(string name, string pattern) : this(
            name,
            new Regex("^" + pattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenType"/> class
        /// with the specified name and regex object.
        /// </summary>
        /// <param name="name">The name of this token type.</param>
        /// <param name="pattern">The <see cref="Regex"/> object to match tokens of this type.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TokenType(string name, Regex pattern)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            _hashCode = HashCode.Combine(name, pattern);
        }

        public override bool Equals(object? obj)
        {
            return obj is TokenType && _hashCode == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string? ToString()
        {
            return Name;
        }
    }
}
