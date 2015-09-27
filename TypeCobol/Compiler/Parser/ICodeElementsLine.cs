﻿using System;
using System.Collections.Generic;
using TypeCobol.Compiler.AntlrUtils;
using TypeCobol.Compiler.CodeElements;
using TypeCobol.Compiler.Preprocessor;

namespace TypeCobol.Compiler.Parser
{
    /// <summary>
    /// Line of code elements after parser execution
    /// </summary>
    public interface ICodeElementsLine : IProcessedTokensLine
    {
        /// <summary>
        /// Code elements STARTING on this line 
        /// </summary>
        IList<CodeElement> CodeElements { get; }
        
        /// <summary>
        /// Error and warning messages produced while parsing the source text line
        /// </summary>
        IList<ParserDiagnostic> ParserDiagnostics { get; }
    }
}
