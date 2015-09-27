﻿using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using TypeCobol.Compiler.AntlrUtils;
using TypeCobol.Compiler.CodeModel;
using TypeCobol.Compiler.Directives;
using TypeCobol.Compiler.File;
using TypeCobol.Compiler.Parser;
using TypeCobol.Compiler.Text;

namespace TypeCobol.Compiler.TypeChecker
{
    public class SemanticsDocument : IObserver<CodeElementChangedEvent>
    {
        private readonly ISubject<CodeModelChangedEvent> codeModelChangedEventsSource = new Subject<CodeModelChangedEvent>();
        private readonly ISubject<IList<CompilationError>> compilationErrorsEventsSource = new Subject<IList<CompilationError>>();

        public SemanticsDocument(CodeElementsDocument parseTree, TypeCobolOptions compilerOptions)
        {
            SyntaxDocument = parseTree;
            CompilerOptions = compilerOptions;
        }

        /// <summary>
        ///     Underlying SyntaxTree
        /// </summary>
        public CodeElementsDocument SyntaxDocument { get; private set; }

        /// <summary>
        ///     Root object representing the Cobol program
        /// </summary>
        public Program Program { get; private set; }

        /// <summary>
        ///     Root object representing the Cobol class
        /// </summary>
        public Class Class { get; private set; }

        /// <summary>
        ///     List of errors found while analyzing the program or the class
        /// </summary>
        public IList<TypeDiagnostic> Diagnostics { get; private set; }

        /// <summary>
        ///     Cobol text generated by the TypeCobol transpiler
        /// </summary>
        public ITextDocument GeneratedTextDocument { get; private set; }

        /// <summary>
        ///     Cobol file where the text generated by the TypeCobol transpiler is saved
        /// </summary>
        public CobolFile GeneratedCobolFile { get; private set; }

        /// <summary>
        ///     Compiler options directing the type checker operations
        /// </summary>
        public TypeCobolOptions CompilerOptions { get; private set; }

        public IObservable<CodeModelChangedEvent> CodeModelChangedEventsSource
        {
            get { return codeModelChangedEventsSource; }
        }

        public IObservable<IList<CompilationError>> CompilationErrorsEventsSource
        {
            get { return compilationErrorsEventsSource; }
        }

        public void OnNext(CodeElementChangedEvent parseEvent)
        {
            // Analyse result with the type checker
            var typeChecker = new CobolTypeChecker();
            //walker.Walk(typeChecker, SyntaxDocument.ParseTree);
            Diagnostics = typeChecker.Diagnostics;

            // Send compilation errors event
            IList<CompilationError> errors = new List<CompilationError>();
            foreach (TypeDiagnostic diag in typeChecker.Diagnostics)
            {
                errors.Add(new CompilationError
                {
                    LineNumber = SyntaxDocument.ProcessedTokensDocument.TokensDocument.TokensLines.IndexOf(diag.StartToken.TokensLine, diag.StartToken.TokensLine.InitialLineIndex),
                    StartColumn = diag.StartToken.Column,
                    Message = diag.Message
                });
            }
            foreach(TypeCobol.Compiler.CodeElements.CodeElement e in SyntaxDocument.CodeElements) {
                foreach (ParserDiagnostic diag in e.Diagnostics)
                {
                    int lineNumber = 1;
                    if(diag.OffendingSymbol != null)
                    {
                        lineNumber = SyntaxDocument.ProcessedTokensDocument.TokensDocument.TokensLines.IndexOf(
                                diag.OffendingSymbol.TokensLine,
                                diag.OffendingSymbol.TokensLine.InitialLineIndex
                            );
                    }

                    errors.Add(new CompilationError {
                        LineNumber = /* temp patch -> */ lineNumber <= 0 ? 1 : lineNumber,
                        StartColumn = diag.ColumnStart,
                        EndColumn = diag.ColumnEnd,
                        Message = diag.Message
                    });
                    //TODO if an NullPointerException is thrown here, it causes problem when 2 successives TextDocument.LoadChars(string) are used
                }
            }
            compilationErrorsEventsSource.OnNext(errors);
        }

        public void OnCompleted()
        {
            // do nothing here
        }

        public void OnError(Exception error)
        {
            // do nothing here
        }

        // --- Implement IObservable<CodeModelChangedEvent>
    }

    /// <summary>
    ///     Temporary diagnostic class for the demo
    /// </summary>
    public class CompilationError
    {
        public int LineNumber { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }

        public string Message { get; set; }
    }
}