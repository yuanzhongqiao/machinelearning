﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.ML.Tokenizers
{
    /// <summary>
    /// The pre-tokenizer for Roberta English tokenizer.
    /// </summary>
    public sealed partial class RobertaPreTokenizer : PreTokenizer
    {
        /// <summary>
        /// Gets a singleton instance of the Roberta pre-tokenizer..
        /// </summary>
        public static RobertaPreTokenizer Instance { get; } = new RobertaPreTokenizer();

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

            return SplitSentence(sentence, Tokenizer.P50kBaseRegex());
        }
    }
}
