﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.ML.Tokenizers
{
    /// <summary>
    /// The pre-tokenizer for Tiktoken tokenizer.
    /// </summary>
    public sealed class TikTokenPreTokenizer : PreTokenizer
    {
        private readonly Regex? _specialTokensRegex;
        private readonly Regex _regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TikTokenPreTokenizer"/> class.
        /// </summary>
        /// <param name="regex">The regex to use for splitting the text into smaller tokens in the pre-tokenization process.</param>
        /// <param name="specialTokensEncoder">Encode the special token to Id.</param>
        /// <exception cref="ArgumentNullException">When regex is null</exception>
        public TikTokenPreTokenizer(Regex regex, IReadOnlyDictionary<string, int>? specialTokensEncoder)
        {
            if (regex is null)
            {
                throw new ArgumentNullException(nameof(regex));
            }

            _regex = regex;

            if (specialTokensEncoder is { Count: > 0 })
            {
                _specialTokensRegex = new Regex(string.Join("|", specialTokensEncoder.Keys.Select(s => Regex.Escape(s))), RegexOptions.Compiled);
            }
        }

        /// <summary>
        /// Splits the given string in multiple substrings at the word boundary, keeping track of the offsets of said substrings from the original string.
        /// </summary>
        /// <param name="sentence">The string to split into tokens.</param>
        /// <param name="skipSpecialTokens">Indicates whether to skip the special tokens.</param>
        /// <returns>The list of the splits containing the tokens and the token's offsets to the original string.</returns>
        public override IEnumerable<Split> PreTokenize(string sentence, bool skipSpecialTokens = false)
        {
            if (string.IsNullOrEmpty(sentence))
            {
                return Array.Empty<Split>();
            }

            return SplitSentences(sentence, _regex, skipSpecialTokens ? null : _specialTokensRegex);

            static IEnumerable<Split> SplitSentences(string sentence, Regex regex, Regex? specialTokensRegex)
            {
                (int Offset, int Length) match;
                int beginning = 0;

                if (specialTokensRegex is not null)
                {
                    while (true)
                    {
                        (int Offset, int Length) specialMatch;
                        if (!TryGetMatch(specialTokensRegex, sentence, beginning, sentence.Length - beginning, out specialMatch))
                        {
                            break;
                        }

                        while (TryGetMatch(regex, sentence, beginning, specialMatch.Offset - beginning, out match))
                        {
                            yield return new Split(sentence, null, (match.Offset, match.Offset + match.Length));
                            beginning = match.Offset + match.Length;
                        }

                        yield return new Split(sentence, null, (specialMatch.Offset, specialMatch.Offset + specialMatch.Length), isSpecialToken: true);
                        beginning = specialMatch.Offset + specialMatch.Length;
                    }
                }

                while (TryGetMatch(regex, sentence, beginning, sentence.Length - beginning, out match))
                {
                    yield return new Split(sentence, null, (match.Offset, match.Offset + match.Length));
                    beginning = match.Length + match.Offset;
                }
            }
        }
    }
}
