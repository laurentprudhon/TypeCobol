﻿using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.Compiler.Diagnostics;
using TypeCobol.Compiler.Scanner;

namespace TypeCobol.Compiler.AntlrUtils
{
    /// <summary>
    /// Register all errors encountered by the Antlr parser in a list of Diagnostic objects
    /// </summary>
    public class DiagnosticSyntaxErrorListener : BaseErrorListener 
    {
        /// <summary>
        /// List of errors found by parsing the program
        /// </summary>
        public IList<ParserDiagnostic> Diagnostics { get; private set; }

        public DiagnosticSyntaxErrorListener()
        {
            Diagnostics = new List<ParserDiagnostic>();
        }

        /// <summary>
        /// Register a ParserDiagnostic for each syntax error encountered by the parser
        /// </summary>
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            // Build a string representing the current grammar rules being recognized
            StringBuilder ruleStack = new StringBuilder();
            IList<String> stack = ((Antlr4.Runtime.Parser)recognizer).GetRuleInvocationStack();
            foreach (string ruleInvocation in stack.Reverse())
            {
                ruleStack.AppendLine(ruleInvocation);
            }

            // Register a new diagnostic
            ParserDiagnostic diagnostic = new ParserDiagnostic(msg, (Token)offendingSymbol, ruleStack.ToString());
            Diagnostics.Add(diagnostic);
        }
    }

    /// <summary>
    /// Enhanced error message containing additional information about 
    /// the origin of the syntax error in the grammar : offending symbol, rule stack
    /// </summary>
    public class ParserDiagnostic : Diagnostic
    {
        public ParserDiagnostic(string message, Token offendingSymbol, string ruleStack) :
            base(MessageCode.SyntaxErrorInParser, offendingSymbol != null ? offendingSymbol.Column : -1, offendingSymbol != null ? offendingSymbol.EndColumn : -1, message)
        {
            OffendingSymbol = offendingSymbol;
            RuleStack = ruleStack;
        }

        /// <summary>
        /// First token which did not match the current rule in the gramme
        /// </summary>
        public Token OffendingSymbol { get; private set; }

        /// <summary>
        /// Stack of grammar rules which were being recognized when an incorrect token occured
        /// </summary>
        public string RuleStack { get; private set; }
    }
}
