﻿using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.Compiler.CodeElements;
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
            bool isFirst = true;
            foreach (string ruleInvocation in stack.Reverse())
            {
                if(isFirst) { isFirst = false;  }
                else
                {
                    ruleStack.Append('>');
                }
                ruleStack.Append(ruleInvocation);
            }

            // Register a new diagnostic
            ParserDiagnostic diagnostic = new ParserDiagnostic(msg, offendingSymbol, ruleStack.ToString());
            Diagnostics.Add(diagnostic);
        }
    }

    /// <summary>
    /// Enhanced error message containing additional information about 
    /// the origin of the syntax error in the grammar : offending symbol, rule stack
    /// </summary>
    public class ParserDiagnostic : Diagnostic
    {
        public ParserDiagnostic(string message, IToken offendingSymbol, string ruleStack, MessageCode code = MessageCode.SyntaxErrorInParser) :
            base(code, offendingSymbol != null ? offendingSymbol.Column : -1, offendingSymbol != null ? offendingSymbol.StopIndex + 1 : -1, message)
        {
            OffendingSymbol = offendingSymbol;
            RuleStack = ruleStack;
        }

        /// <summary>
        /// First token which did not match the current rule in the grammar
        /// </summary>
        public IToken OffendingSymbol { get; private set; }

        /// <summary>
        /// Stack of grammar rules which were being recognized when an incorrect token occured
        /// </summary>
        public string RuleStack { get; private set; }

        public string ToStringWithRuleStack()
        {
            int lineindex = OffendingSymbol.Line;
            if (lineindex < 0) { // ProgramClass parsing
                CodeElement e = OffendingSymbol as CodeElement;
                if (e != null) lineindex = e.ConsumedTokens[0].Line;
            }
            // TO DO - IMPORTANT : this is the INITIAL line number, and not the CURRENT line number
            // This is enough to pass all unit tests, but will return false informations in real usage !
            // for real line number, use a Snapshot
            return base.ToString() + " (RuleStack=" + RuleStack + ", OffendingSymbol=" + OffendingSymbol.ToString() + " on line " + lineindex + ")";
        }
    }
}
