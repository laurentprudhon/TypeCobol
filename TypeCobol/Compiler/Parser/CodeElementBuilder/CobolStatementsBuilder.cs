﻿using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TypeCobol.Compiler.AntlrUtils;
using TypeCobol.Compiler.CodeElements;
using TypeCobol.Compiler.Parser.Generated;
using TypeCobol.Compiler.Scanner;

namespace TypeCobol.Compiler.Parser
{
    internal class CobolStatementsBuilder
    {
        public CobolStatementsBuilder(CobolWordsBuilder cobolWordsBuilder, CobolExpressionsBuilder cobolExpressionsBuilder)
        {
            CobolWordsBuilder = cobolWordsBuilder;
            CobolExpressionsBuilder = cobolExpressionsBuilder;
        }

        private CobolWordsBuilder CobolWordsBuilder { get; set; }
        private CobolExpressionsBuilder CobolExpressionsBuilder { get; set; }

        ///////////////////////////////
        // PROCEDURE DIVISION HEADER //
        ///////////////////////////////

        internal ProcedureDivisionHeader CreateProcedureDivisionHeader(CodeElementsParser.ProcedureDivisionHeaderContext context)
        {
            var statement = new ProcedureDivisionHeader();
            if (context.programInputParameters() != null)
            {
                foreach (var inputParametersContext in context.programInputParameters())
                {
                    SyntaxProperty<ReceivingMode> receivingMode = null;
                    if (inputParametersContext.REFERENCE() != null)
                    {
                        receivingMode = new SyntaxProperty<ReceivingMode>(ReceivingMode.ByReference,
                            ParseTreeUtils.GetFirstToken(inputParametersContext.REFERENCE()));
                    }
                    else if (inputParametersContext.VALUE() != null)
                    {
                        receivingMode = new SyntaxProperty<ReceivingMode>(ReceivingMode.ByValue,
                            ParseTreeUtils.GetFirstToken(inputParametersContext.VALUE()));
                    }
                    foreach (var storageAreaContext in inputParametersContext.storageArea2())
                    {
                        if (statement.InputParameters == null)
                        {
                            statement.InputParameters = new List<ProgramInputParameter>(1);
                        }
                        var inputParameter = new ProgramInputParameter
                        {
                            ReceivingMode = receivingMode,
                            ReceivingStorageArea = CobolExpressionsBuilder.CreateStorageArea(storageAreaContext)
                        };
                        inputParameter.ReceivingStorageArea.DataSourceType = DataSourceType.ReceiveFromCallingProgram;
                        statement.InputParameters.Add(inputParameter);
                    }
                }
            }
            if (context.programOutputParameter() != null)
            {
                statement.OutputParameter =
                    CobolExpressionsBuilder.CreateStorageArea(context.programOutputParameter().storageArea2());
            }
            return statement;
        }
        
        //////////////////////
        // ACCEPT STATEMENT //
        //////////////////////

        internal AcceptFromInputDeviceStatement CreateAcceptDataTransferStatement(CodeElementsParser.AcceptDataTransferContext context)
        {
            var statement = new AcceptFromInputDeviceStatement();
            statement.ReceivingStorageArea = CobolExpressionsBuilder.CreateAlphanumericStorageArea(context.alphanumericStorageArea());
            statement.ReceivingStorageArea.DataSourceType = DataSourceType.ReadFromInputDevice;
            if (context.mnemonicForEnvironmentNameReferenceOrEnvironmentName() != null)
            {
                statement.InputDevice = CobolWordsBuilder.CreateMnemonicForEnvironmentNameReferenceOrEnvironmentName(context.mnemonicForEnvironmentNameReferenceOrEnvironmentName());
            }
            return statement;
        }

        internal AcceptFromSystemDateStatement CreateAcceptSystemDateTime(CodeElementsParser.AcceptSystemDateTimeContext context)
        {
            var statement = new AcceptFromSystemDateStatement();
            statement.ReceivingStorageArea = CobolExpressionsBuilder.CreateAlphanumericStorageArea(context.alphanumericStorageArea());
            statement.ReceivingStorageArea.DataSourceType = DataSourceType.ReadFromSystemCall;
            if (context.YYYYMMDD() != null)
            {
                statement.SystemDateFormat = new SyntaxProperty<SystemDateFormat>(SystemDateFormat.DATE_YYYYMMDD,
                    ParseTreeUtils.GetFirstToken(context.YYYYMMDD()));
            }
            else if (context.DATE() != null)
            {
                statement.SystemDateFormat = new SyntaxProperty<SystemDateFormat>(SystemDateFormat.DATE_YYMMDD,
                    ParseTreeUtils.GetFirstToken(context.DATE()));
            }
            else if (context.YYYYDDD() != null)
            {
                statement.SystemDateFormat = new SyntaxProperty<SystemDateFormat>(SystemDateFormat.DAY_YYYYDDD,
                    ParseTreeUtils.GetFirstToken(context.YYYYDDD()));
            }
            else if (context.DAY() != null)
            {
                statement.SystemDateFormat = new SyntaxProperty<SystemDateFormat>(SystemDateFormat.DAY_YYDDD,
                    ParseTreeUtils.GetFirstToken(context.DAY()));
            }
            else if (context.DAY_OF_WEEK() != null)
            {
                statement.SystemDateFormat = new SyntaxProperty<SystemDateFormat>(SystemDateFormat.DAY_OF_WEEK,
                    ParseTreeUtils.GetFirstToken(context.DAY_OF_WEEK()));
            }
            else if (context.TIME() != null)
            {
                statement.SystemDateFormat = new SyntaxProperty<SystemDateFormat>(SystemDateFormat.TIME,
                    ParseTreeUtils.GetFirstToken(context.TIME()));
            }
            return statement;
        }

        ////////////////////
        // ADD STATEMENT //  
        ////////////////////

        internal CodeElement CreateAddStatement(CodeElementsParser.AddSimpleContext context)
        {
            var statement = new AddSimpleStatement();

            statement.VariablesAddedTogether = BuildObjectArrrayFromParserRules(
                context.numericVariable3(), 
                ctx => CobolExpressionsBuilder.CreateNumericVariable(ctx));

            statement.SendingAndReceivingStorageAreas = BuildObjectArrrayFromParserRules(
                context.numericStorageAreaRounded(),
                ctx => CreateRoundedResult(ctx));

            return statement;
        }

        private RoundedResult CreateRoundedResult(CodeElementsParser.NumericStorageAreaRoundedContext context)
        {
            var roundedResult = new RoundedResult();
            roundedResult.ReceivingStorageArea = CobolExpressionsBuilder.CreateNumericStorageArea(context.numericStorageArea());
            roundedResult.IsRounded = CreateSyntaxProperty(true, context.ROUNDED());
            return roundedResult;
        }

        internal CodeElement CreateAddGivingStatement(CodeElementsParser.AddGivingContext context)
        {
            var statement = new AddGivingStatement();

            statement.VariablesAddedTogether = BuildObjectArrrayFromParserRules(
                context.numericVariable3(),
                ctx => CobolExpressionsBuilder.CreateNumericVariable(ctx));

            statement.ToOperand = CobolExpressionsBuilder.CreateNumericVariable(context.toOperand);

            statement.ReceivingStorageAreas = BuildObjectArrrayFromParserRules(
                context.numericStorageAreaRounded(),
                ctx => CreateRoundedResult(ctx));

            return statement;
        }

        internal CodeElement CreateAddCorrespondingStatement(CodeElementsParser.AddCorrespondingContext context)
        {
            var statement = new AddCorrespondingStatement();
            statement.GroupItem = CobolExpressionsBuilder.CreateDataItemReference(context.groupItem);
            statement.ToGroupItem = CobolExpressionsBuilder.CreateDataItemReference(context.toGroupItem);
            return statement;
        }

        ////////////////////
        // ALTER STATEMENT //
        ////////////////////

        internal CodeElement CreateAlterStatement(CodeElementsParser.AlterStatementContext context)
        {
            var statement = new AlterStatement();

            int alterInstructionsCount = context.procedureName().Length / 2;
            statement.AlterGotoInstructions = new AlterGotoInstruction[alterInstructionsCount];
            for(int i = 0; i < alterInstructionsCount; i++)
            {
                var alterInstruction = new AlterGotoInstruction();
                alterInstruction.AlteredProcedure = CobolWordsBuilder.CreateProcedureName(context.procedureName()[2*i]);
                alterInstruction.NewTargetProcedure= CobolWordsBuilder.CreateProcedureName(context.procedureName()[2*i + 1]);                    
                statement.AlterGotoInstructions[i] = alterInstruction;
            }

            return statement;
        }

        ////////////////////
        // CALL STATEMENT //
        ////////////////////

        internal CallStatement CreateCallStatement(CodeElementsParser.CallStatementContext context)
        {
            var statement = new CallStatement();

            statement.ProgramOrProgramEntryOrProcedureOrFunction = 
                CobolExpressionsBuilder.CreateProgramNameOrProgramEntryOrProcedurePointerOrFunctionPointerVariable(
                    context.programNameOrProgramEntryOrProcedurePointerOrFunctionPointerVariable());

            if (context.callProgramInputParameters() != null)
            {
                foreach (var inputParametersContext in context.callProgramInputParameters())
                {
                    SyntaxProperty<SendingMode> sendingMode = null;
                    if (inputParametersContext.REFERENCE() != null)
                    {
                        sendingMode = CreateSyntaxProperty(SendingMode.ByReference,
                            inputParametersContext.REFERENCE());
                    }
                    else if (inputParametersContext.CONTENT() != null)
                    {
                        sendingMode = CreateSyntaxProperty(SendingMode.ByContent,
                            inputParametersContext.CONTENT());
                    }
                    else if (inputParametersContext.VALUE() != null)
                    {
                        sendingMode = CreateSyntaxProperty(SendingMode.ByValue,
                            inputParametersContext.VALUE());
                    }
                    foreach (var variableOrFileNameOrOmittedContext in inputParametersContext.variableOrFileNameOrOmitted())
                    {
                        if (statement.InputParameters == null)
                        {
                            statement.InputParameters = new List<CallInputParameter>(1);
                        }
                        var inputParameter = new CallInputParameter
                        {
                            SendingMode = sendingMode
                        };
                        if (variableOrFileNameOrOmittedContext.variableOrFileName() != null)
                        {
                            inputParameter.SendingVariable = CobolExpressionsBuilder.CreateVariableOrFileName(
                                variableOrFileNameOrOmittedContext.variableOrFileName());
                        }
                        else if (variableOrFileNameOrOmittedContext.OMITTED() != null)
                        {
                            inputParameter.IsOmitted = CreateSyntaxProperty(true,
                                variableOrFileNameOrOmittedContext.OMITTED());
                        }
                        statement.InputParameters.Add(inputParameter);
                    }
                }
            }
            if (context.callProgramOutputParameter() != null)
            {
                statement.OutputParameter =
                    CobolExpressionsBuilder.CreateStorageArea(context.callProgramOutputParameter().storageArea1());
                statement.OutputParameter.DataSourceType = DataSourceType.ReceiveFromCalledProgram;
            }

            return statement;
        }
        
        // TO DO : Implement these controls at the next parsing step, after symbol type resolution
        // DiagnosticUtils.AddError(statement, "CALL .. USING: Illegal function identifier", e);
        // DiagnosticUtils.AddError(statement, "CALL .. USING: Illegal LINAGE COUNTER", e);
        // DiagnosticUtils.AddError(statement, "CALL .. USING: Illegal LENGTH OF in BY REFERENCE phrase", e);
        // DiagnosticUtils.AddError(statement, "CALL .. USING: <filename> only allowed in BY REFERENCE phrase", e);
        // DiagnosticUtils.AddError(statement, "CALL .. USING: Illegal <literal> in BY REFERENCE phrase", e);
        // DiagnosticUtils.AddError(statement, "CALL .. USING: Illegal OMITTED in BY VALUE phrase", token, rulestack);
        
        //////////////////////
        // CANCEL STATEMENT //
        //////////////////////

        internal CodeElement CreateCancelStatement(CodeElementsParser.CancelStatementContext context)
        {
            var statement = new CancelStatement();

            statement.Programs = BuildObjectArrrayFromParserRules(context.programNameVariable(),
                ctx => CobolExpressionsBuilder.CreateProgramNameVariable(ctx));
            
            return statement;
        }

        //////////////////////
        // CLOSE STATEMENT //
        //////////////////////

        internal CodeElement CreateCloseStatement(CodeElementsParser.CloseStatementContext context)
        {
            var statement = new CloseStatement();

            statement.CloseFileInstructions = BuildObjectArrrayFromParserRules(context.closeFileDirective(),
                ctx => CreateCloseFileInstruction(ctx));

            return statement;
        }       

        private CloseFileInstruction CreateCloseFileInstruction(CodeElementsParser.CloseFileDirectiveContext context)
        {
            var instruction = new CloseFileInstruction();
            instruction.FileName = CobolWordsBuilder.CreateFileNameReference(context.fileNameReference());
            instruction.IsReelUnit = CreateSyntaxProperty(true, context.REEL());
            if(instruction.IsReelUnit == null)
            {
                instruction.IsReelUnit = CreateSyntaxProperty(true, context.UNIT());
            }
            instruction.IsForRemoval = CreateSyntaxProperty(true, context.REMOVAL());
            instruction.IsWithNoRewind = CreateSyntaxProperty(true, context.REWIND());
            instruction.IsWithLock = CreateSyntaxProperty(true, context.LOCK());
            return instruction;
        }

        ///////////////////////
        // COMPUTE STATEMENT //
        ///////////////////////

        internal CodeElement CreateComputeStatement(CodeElementsParser.ComputeStatementContext context)
        {
            var statement = new ComputeStatement();

            statement.ReceivingStorageAreas = BuildObjectArrrayFromParserRules(context.numericStorageAreaRounded(),
                ctx => CreateRoundedResult(ctx));

            statement.ArithmeticExpression = CobolExpressionsBuilder.CreateArithmeticExpression(context.arithmeticExpression());

            return statement;
        }

        //////////////////////
        // DELETE STATEMENT //
        //////////////////////

        internal CodeElement CreateDeleteStatement(CodeElementsParser.DeleteStatementContext context)
        {
            var statement = new DeleteStatement();

            statement.FileName = CobolWordsBuilder.CreateFileNameReference(context.fileNameReference());

            return statement;
        }

        //////////////////////
        // DISPLAY STATEMENT //
        //////////////////////

        internal CodeElement CreateDisplayStatement(CodeElementsParser.DisplayStatementContext context)
        {
            var statement = new DisplayStatement();

            statement.Variables = BuildObjectArrrayFromParserRules(context.variable4(),
                ctx => CobolExpressionsBuilder.CreateVariable(ctx));

            if(context.uponOutputDevice() != null)
            {
                statement.OutputDeviceName = CobolWordsBuilder.CreateMnemonicForEnvironmentNameReferenceOrEnvironmentName(
                    context.uponOutputDevice().mnemonicForEnvironmentNameReferenceOrEnvironmentName());
            }
            if (context.withNoAdvancing() != null)
            {
                statement.IsWithNoAdvancing = CreateSyntaxProperty(true, context.withNoAdvancing().ADVANCING());
            }

            return statement;
        }

        //////////////////////
        // DIVIDE STATEMENT //
        //////////////////////

        internal CodeElement CreateDivideStatement(CodeElementsParser.DivideSimpleContext context)
        {
            var statement = new DivideSimpleStatement();

            statement.Divisor = CobolExpressionsBuilder.CreateNumericVariable(context.divisor);
            statement.DividendsAndQuotients = BuildObjectArrrayFromParserRules(context.numericStorageAreaRounded(),
                ctx => CreateRoundedResult(ctx));

            return statement;
        }

        internal CodeElement CreateDivideGivingStatement(CodeElementsParser.DivideGivingContext context)
        {
            var statement = new DivideGivingStatement();

            if(context.divisor1 != null)
            {
                statement.Divisor = CobolExpressionsBuilder.CreateNumericVariable(context.divisor1);
            }
            else if (context.divisor2 != null)
            {
                statement.Divisor = CobolExpressionsBuilder.CreateNumericVariable(context.divisor2);
            }
            if (context.dividend1 != null)
            {
                statement.Dividend = CobolExpressionsBuilder.CreateNumericVariable(context.dividend1);
            }
            else if (context.dividend2 != null)
            {
                statement.Dividend = CobolExpressionsBuilder.CreateNumericVariable(context.dividend2);
            }
            statement.Quotients = BuildObjectArrrayFromParserRules(context.numericStorageAreaRounded(),
                ctx => CreateRoundedResult(ctx));

            return statement;
        }

        internal CodeElement CreateDivideRemainderStatement(CodeElementsParser.DivideRemainderContext context)
        {
            var statement = new DivideRemainderStatement();

            if (context.divisor1 != null)
            {
                statement.Divisor = CobolExpressionsBuilder.CreateNumericVariable(context.divisor1);
            }
            else if (context.divisor2 != null)
            {
                statement.Divisor = CobolExpressionsBuilder.CreateNumericVariable(context.divisor2);
            }
            if (context.dividend1 != null)
            {
                statement.Dividend = CobolExpressionsBuilder.CreateNumericVariable(context.dividend1);
            }
            else if (context.dividend2 != null)
            {
                statement.Dividend = CobolExpressionsBuilder.CreateNumericVariable(context.dividend2);
            }
            statement.Quotient = CreateRoundedResult(context.numericStorageAreaRounded());
            statement.Remainder = CobolExpressionsBuilder.CreateNumericStorageArea(context.numericStorageArea());

            return statement;
        }

        /////////////////////
        // ENTRY STATEMENT //
        /////////////////////

        internal CodeElement CreateEntryStatement(CodeElementsParser.EntryStatementContext context)
        {
            var statement = new EntryStatement();
            if (context.programEntryDefinition() != null)
            {
                statement.ProgramName = CobolWordsBuilder.CreateLiteral(context.programEntryDefinition().alphanumericLiteral());
            }
            foreach (var by in context.byReferenceOrByValueIdentifiers())
            {
                var u = new EntryStatement.Using<Identifier>();
                var identifiers = CobolWordsBuilder.CreateIdentifiers(by.identifier());
                foreach (var i in identifiers) u.Add(i);
                u.ByValue = by.VALUE() != null;
                statement.Usings.Add(u);
            }
            return statement;
        }

        ////////////////////////
        // EVALUATE STATEMENT //
        ////////////////////////

        internal CodeElement CreateEvaluateStatement(CodeElementsParser.EvaluateStatementContext context)
        {
            return new EvaluateStatement();
        }

        ////////////////////
        // EXEC STATEMENT //
        ////////////////////

        internal CodeElement CreateExecStatement(CodeElementsParser.ExecStatementContext context)
        {
            var statement = new ExecStatement();
            Token node = null;
            if (context.execTranslatorName() != null)
            {
                node = ParseTreeUtils.GetTokenFromTerminalNode(context.execTranslatorName().ExecTranslatorName());
            }
            if (node != null) statement.Compiler = node.Text;
            var str = new StringBuilder();
            foreach (var line in context.ExecStatementText())
            {
                node = ParseTreeUtils.GetTokenFromTerminalNode(line);
                if (node != null) str.Append(node.Text);
            }
            statement.Code = str.ToString();
            return statement;
        }

        ////////////////////
        // GOTO STATEMENT //
        ////////////////////

        internal CodeElement CreateGotoStatement(CodeElementsParser.GotoSimpleContext gotoSimpleContext)
        {
            throw new NotImplementedException();
        }

        internal CodeElement CreateGotoConditionalStatement(CodeElementsParser.GotoConditionalContext gotoConditionalContext)
        {
            var statement = new GotoStatement();
            foreach (var procedure in context.procedureName())
            {
                QualifiedProcedureName procedurename = CobolWordsBuilder.CreateProcedureName(procedure);
                if (procedurename != null) statement.Procedures.Add(procedurename);
            }
            if (context.identifier() != null)
                statement.DependingOn = CobolWordsBuilder.CreateIdentifier(context.identifier());
            if (statement.Procedures.Count > 1 && statement.DependingOn == null)
                DiagnosticUtils.AddError(statement, "GO TO: Required only one <procedure name> or DEPENDING phrase", context);
            if (statement.Procedures.Count < 1 && statement.DependingOn != null)
                DiagnosticUtils.AddError(statement, "Conditional GO TO: Required <procedure name>", context);
            if (statement.Procedures.Count > 255)
                DiagnosticUtils.AddError(statement, "Conditional GO TO: Maximum 255 <procedure name> allowed", context);
            return statement;
        }

        //////////////////
        // IF STATEMENT //
        //////////////////

        internal CodeElement CreateIfStatement(CodeElementsParser.IfStatementContext context)
        {
            var statement = new IfStatement();
            if (context.conditionalExpression() != null)
            {
                statement.condition = new LogicalExpressionBuilder().createCondition(context.conditionalExpression());
            }
            return statement;
        }

        //////////////////////////
        // INITIALIZE STATEMENT //
        //////////////////////////

        internal InitializeStatement CreateInitializeStatement(CodeElementsParser.InitializeStatementContext context)
        {
            var statement = new InitializeStatement();
            statement.Receiving = CobolWordsBuilder.CreateIdentifiers(context.identifier());
            foreach (var sending in context.initializeReplacing())
            {
                var expression = CobolWordsBuilder.CreateIdentifierOrLiteral(sending.identifierOrLiteral());
                statement.Sending.Add(new InitializeStatement.Replacing(expression, CreateInitializeMode(sending)));
            }
            return statement;
        }

        private InitializeStatement.Replacing.Mode CreateInitializeMode(CodeElementsParser.InitializeReplacingContext context)
        {
            if (context.ALPHABETIC() != null) return InitializeStatement.Replacing.Mode.ALPHABETIC;
            if (context.ALPHANUMERIC() != null) return InitializeStatement.Replacing.Mode.ALPHANUMERIC;
            if (context.ALPHANUMERIC_EDITED() != null) return InitializeStatement.Replacing.Mode.ALPHANUMERIC_EDITED;
            if (context.NATIONAL() != null) return InitializeStatement.Replacing.Mode.NATIONAL;
            if (context.NATIONAL_EDITED() != null) return InitializeStatement.Replacing.Mode.NATIONAL_EDITED;
            if (context.NUMERIC() != null) return InitializeStatement.Replacing.Mode.NUMERIC;
            if (context.NUMERIC_EDITED() != null) return InitializeStatement.Replacing.Mode.NUMERIC_EDITED;
            if (context.DBCS() != null) return InitializeStatement.Replacing.Mode.DBCS;
            if (context.EGCS() != null) return InitializeStatement.Replacing.Mode.EGCS;
            return InitializeStatement.Replacing.Mode.UNKNOWN;
        }

        ///////////////////////
        // INSPECT STATEMENT //
        ///////////////////////

        internal CodeElement CreateInspectStatement(CodeElementsParser.InspectStatementContext context)
        {
            var identifier = CobolWordsBuilder.CreateIdentifier(context.identifier());

            // CONVERTING
            if (context.inspectConverting() != null)
                return CreateInspectConverting(context.inspectConverting());

            var statement = new InspectStatement();
            statement.Item = identifier;

            //TALLYING
            foreach (var t in context.inspectTallying())
            {
                var tallying = new InspectStatement.Tallying();
                tallying.Count = CobolWordsBuilder.CreateIdentifier(t.identifier());
                if (t.inspectCharacters() != null)
                    tallying.CharactersPhrase = CreateInspectSubject(t.inspectCharacters(), statement, false);
                if (t.inspectIdentifiers() != null)
                    tallying.IdentifiersPhrase = CreateInspectIdentifier(t.inspectIdentifiers(), statement, false, false);
                statement.TallyingList.Add(tallying);
            }

            // REPLACING
            foreach (var c in context.inspectCharacters())
                statement.ReplacingCharacters.Add(CreateInspectSubject(c, statement, true));
            foreach (var c in context.inspectIdentifiers())
                statement.ReplacingIdentifiers.Add(CreateInspectIdentifier(c, statement, true, true));
            return statement;
        }

        private InspectConvertingStatement CreateInspectConverting(CodeElementsParser.InspectConvertingContext context)
        {
            var statement = new InspectConvertingStatement();
            if (context.identifierOrLiteral().Length > 0)
            {
                int c = 0;
                foreach (var i in context.identifierOrLiteral())
                {
                    if (c == 0) statement.Replaced = CobolWordsBuilder.CreateIdentifierOrLiteral(i);
                    if (c == 1) statement.Replacing = CobolWordsBuilder.CreateIdentifierOrLiteral(i);
                    c++;
                }
            }
            statement.Delimiters = CreateDelimiters(context.inspectPhrase1(), statement);
            return statement;
        }

        private InspectStatement.Subject CreateInspectSubject(CodeElementsParser.InspectByIdentifiersContext context, InspectStatement statement, bool meWantsBy)
        {
            var result = new InspectStatement.Subject();
            result.SubstitutionField = CreateInspectBy(context.inspectBy());
            if (!meWantsBy && result.SubstitutionField != null)
                DiagnosticUtils.AddError(statement, "INSPECT  TALLYING: illegal CHARACTERS BY <identifier> or <literal>", context.inspectBy());
            if (meWantsBy && result.SubstitutionField == null)
                DiagnosticUtils.AddError(statement, "INSPECT REPLACING: Missing CHARACTERS BY <identifier> or <literal>", context);
            result.Delimiters = CreateDelimiters(context.inspectPhrase1(), statement);
            return result;
        }

        private InspectStatement.Subject CreateInspectSubject(CodeElementsParser.InspectCharactersContext context, InspectStatement statement, bool meWantsBy)
        {
            var result = new InspectStatement.Subject();
            result.SubstitutionField = CreateInspectBy(context.inspectBy());
            if (!meWantsBy && result.SubstitutionField != null)
                DiagnosticUtils.AddError(statement, "INSPECT TALLYING: illegal CHARACTERS BY <identifier> or <literal>", context.inspectBy());
            if (meWantsBy && result.SubstitutionField == null)
                DiagnosticUtils.AddError(statement, "INSPECT REPLACING: Missing CHARACTERS BY <identifier> or <literal>", context);
            result.Delimiters = CreateDelimiters(context.inspectPhrase1(), statement);
            return result;
        }

        private InspectStatement.ALF CreateInspectIdentifier(CodeElementsParser.InspectIdentifiersContext context, InspectStatement statement, bool meWantsBy, bool isFirstAllowed)
        {
            var alf = new InspectStatement.ALF();
            alf.All = context.ALL() != null;
            alf.Leading = context.LEADING() != null;
            alf.First = context.FIRST() != null;
            if (alf.First && !isFirstAllowed)
                DiagnosticUtils.AddError(statement, "INSPECT TALLYING: illegal FIRST", context);
            foreach (var c in context.inspectByIdentifiers())
            {
                var sub = CreateInspectSubject(c, statement, meWantsBy);
                if (sub != null) alf.Subjects.Add(sub);
            }
            return alf;
        }

        private IList<Delimiter> CreateDelimiters(IReadOnlyList<CodeElementsParser.InspectPhrase1Context> context, CodeElement e)
        {
            var delimiters = new List<Delimiter>();
            bool seenBefore = false, seenAfter = false;
            foreach (var phrase in context)
            {
                var delimiter = CreateDelimiter(phrase);
                if (delimiter.Before)
                {
                    if (seenBefore) DiagnosticUtils.AddError(e, "INSPECT: Maximum one BEFORE phrase for any one ALL, LEADING, CHARACTERS, FIRST or CONVERTING phrase", phrase);
                    seenBefore = true;
                }
                if (delimiter.After)
                {
                    if (seenAfter) DiagnosticUtils.AddError(e, "INSPECT: Maximum one AFTER phrase for any one ALL, LEADING, CHARACTERS, FIRST or CONVERTING phrase", phrase);
                    seenAfter = true;
                }
                delimiters.Add(delimiter);
            }
            return delimiters;
        }

        private Delimiter CreateDelimiter(CodeElementsParser.InspectPhrase1Context context)
        {
            var delimiter = new Delimiter();
            delimiter.Initial = context.INITIAL() != null;
            delimiter.Before = context.BEFORE() != null;
            delimiter.After = context.AFTER() != null;
            delimiter.Item = CobolWordsBuilder.CreateIdentifierOrLiteral(context.identifierOrLiteral());
            return delimiter;
        }

        private Expression CreateInspectBy(CodeElementsParser.InspectByContext context)
        {
            if (context == null) return null;
            return CobolWordsBuilder.CreateIdentifierOrLiteral(context.identifierOrLiteral());
        }

        //////////////////////
        // INVOKE STATEMENT //
        ////////////////////// 

        internal InvokeStatement CreateInvokeStatement(CodeElementsParser.InvokeStatementContext context)
        {
            var statement = new InvokeStatement();

            // class name or object reference
            if (context.identifierOrClassName() != null)
            {
                // A single UserDefinedWord can reference either a class name or a data name (object reference)
                var identifier = context.identifierOrClassName();
                Token symbolToken = CobolWordsBuilder.GetSymbolTokenIfIdentifierIsOneUserDefinedWord(identifier);
                if (symbolToken != null)
                {
                    // TO DO : use the symbol table to resolve this ambiguity
                    // Only one of the following two properties should be set
                    statement.ClassName = new ClassName(symbolToken);
                    statement.Instance = CobolWordsBuilder.CreateIdentifier(identifier);
                }
                else
                {
                    statement.Instance = CobolWordsBuilder.CreateIdentifier(identifier);
                }
                
            }
            else if(context.SELF() != null)            
            {
                statement.IsSelf = true;
            }
            else if(context.SUPER() != null)
            {
                statement.IsSuper = true;
            }

            // method name
            if (context.NEW() != null)
                statement.MethodName = new New();
            else
            if (context.methodNameFromData() != null)
                statement.MethodName = CobolWordsBuilder.CreateIdentifier(context.methodNameFromData().identifier());
            else
            if (context.methodNameReference() != null)
                statement.MethodName = CobolWordsBuilder.CreateLiteral(context.methodNameReference().alphanumOrNationalLiteral());

            // usings
            statement.Usings = new List<Expression>();
            if (context.invokeUsing() != null)
                foreach (var use in context.invokeUsing())
                {
                    foreach (var c in use.literal())
                    {
                        var e = CobolWordsBuilder.CreateLiteral(c);
                        if (e != null) statement.Usings.Add(e);
                    }
                    foreach (var c in use.identifier())
                    {
                        var e = CobolWordsBuilder.CreateIdentifier(c);
                        // TODO: "(LENGTH OF)? identifier" only
                        if (e != null) statement.Usings.Add(e);
                    }
                }

            // returning
            if (context.invokeReturning() != null)
            {
                statement.Returning = CobolWordsBuilder.CreateIdentifier(context.invokeReturning().identifier());
                if (IdentifierUtils.IsReferenceModified(statement.Returning))
                    DiagnosticUtils.AddError(statement, "INVOKE: Illegal <identifier> reference modification", context.invokeReturning().identifier());
            }

            return statement;
        }

        /////////////////////
        // MERGE STATEMENT //
        /////////////////////

        internal MergeStatement CreateMergeStatement(CodeElementsParser.MergeStatementContext context)
        {
            var statement = new MergeStatement();
            statement.FileName = CobolWordsBuilder.CreateFileName(context.fileNameReference());
            statement.Keys = CreateKeyDataItems(context.onAscendingDescendingKey());
            statement.CollatingSequence = CobolWordsBuilder.CreateAlphabetName(context.alphabetNameReference());
            if (context.usingFilenames() != null)
            {
                statement.Using = CobolWordsBuilder.CreateFileNames(context.usingFilenames().fileNameReference());
                if (statement.Using.Count == 1)
                    DiagnosticUtils.AddError(statement, "MERGE: USING <filename> <filename>+", context.usingFilenames());
            }
            if (context.givingFilenames() != null) statement.Giving = CobolWordsBuilder.CreateFileNames(context.givingFilenames().fileNameReference());
            if (context.outputProcedure() != null) statement.Output = CobolWordsBuilder.CreateProcedureNames(context.outputProcedure().procedureName());
            return statement;
        }

        ////////////////////
        // MOVE STATEMENT //
        ////////////////////

        internal CodeElement CreateMoveStatement(CodeElementsParser.MoveSimpleContext moveSimpleContext)
        {
            throw new NotImplementedException();
        }

        internal CodeElement CreateMoveCorrespondingStatement(CodeElementsParser.MoveCorrespondingContext moveCorrespondingContext)
        {
            var sending = CobolWordsBuilder.CreateIdentifierOrLiteral(context.identifierOrLiteral());
            var receiving = CobolWordsBuilder.CreateIdentifiers(context.identifier());
            var statement = new MoveStatement(sending, receiving, context.corresponding() != null);
            if (context.corresponding() != null)
            {
                if (sending is Literal)
                    DiagnosticUtils.AddError(statement, "MOVE CORRESPONDING: illegal <literal> before TO", context.identifierOrLiteral());
                if (receiving != null && receiving.Count > 1)
                    DiagnosticUtils.AddError(statement, "MOVE CORRESPONDING: maximum 1 group item after TO", context.identifierOrLiteral());
            }
            Debug.Assert(receiving != null, "receiving != null");
            foreach (var identifier in receiving)
            {
                var function = identifier as FunctionReference;
                if (function != null)
                {
                    var rulestack = new TypeCobol.Compiler.AntlrUtils.RuleStackBuilder().GetRuleStack(context);
                    DiagnosticUtils.AddError(statement, "MOVE: illegal <intrinsic function> after TO", function.Symbol.NameToken, rulestack);
                }
            }
            // [TYPECOBOL]
            statement.IsUnsafe = context.UNSAFE() != null;
            // [/TYPECOBOL]
            return statement;
        }

        ////////////////////////
        // MULTIPLY STATEMENT //
        ////////////////////////

        internal CodeElement CreateMultiplyStatement(CodeElementsParser.MultiplySimpleContext multiplySimpleContext)
        {
            var builder = new ArithmeticStatementBuilder('×');
            return builder.InitializeFormat1Statement(context.identifierOrNumericLiteral(), context.identifierRounded());
        }

        internal CodeElement CreateMultiplyGivingStatement(CodeElementsParser.MultiplyGivingContext multiplyGivingContext)
        {
            var builder = new ArithmeticStatementBuilder('×');
            return builder.InitializeFormat2Statement(context.identifierOrNumericLiteral(), context.identifierOrNumericLiteralTmp(),
                context.identifierRounded());
        }

        ////////////////////
        // OPEN STATEMENT //
        ////////////////////

        internal CodeElement CreateOpenStatement(CodeElementsParser.OpenStatementContext context)
        {
            var filenames = new Dictionary<OpenMode, IList<OpenFileName>>();
            var list = new List<OpenFileName>();
            if (context.openInput() != null)
            {
                foreach (var c in context.openInput())
                {
                    IList<OpenFileName> l = CreateOpenFileNames(c);
                    if (l != null) list.AddRange(l);
                }
            }
            filenames.Add(OpenMode.INPUT, list);
            list = new List<OpenFileName>();
            if (context.openOutput() != null)
            {
                foreach (var c in context.openOutput())
                {
                    IList<OpenFileName> l = CreateOpenFileNames(c);
                    if (l != null) list.AddRange(l);
                }
            }
            filenames.Add(OpenMode.OUTPUT, list);
            list = new List<OpenFileName>();
            if (context.openIO() != null)
            {
                foreach (var c in context.openIO())
                {
                    IList<OpenFileName> l = CreateOpenFileNames(c);
                    if (l != null) list.AddRange(l);
                }
            }
            filenames.Add(OpenMode.IO, list);
            list = new List<OpenFileName>();
            if (context.openExtend() != null)
            {
                foreach (var c in context.openExtend())
                {
                    IList<OpenFileName> l = CreateOpenFileNames(c);
                    if (l != null) list.AddRange(l);
                }
            }
            filenames.Add(OpenMode.EXTEND, list);
            return new OpenStatement(filenames);
        }

        private IList<OpenFileName> CreateOpenFileNames(CodeElementsParser.OpenInputContext context)
        {
            if (context.fileNameWithNoRewindOrReversed() == null) return null;
            var filenames = new List<OpenFileName>();
            foreach (var filename in context.fileNameWithNoRewindOrReversed())
            {
                var f = CobolWordsBuilder.CreateFileName(filename.fileNameReference());
                bool norewind = filename.NO() != null;
                bool reversed = filename.REVERSED() != null;
                if (f != null) filenames.Add(new OpenFileName(f, norewind, reversed));
            }
            return filenames;
        }

        private IList<OpenFileName> CreateOpenFileNames(CodeElementsParser.OpenOutputContext context)
        {
            if (context.fileNameWithNoRewind() == null) return null;
            var filenames = new List<OpenFileName>();
            foreach (var filename in context.fileNameWithNoRewind())
            {
                var f = CobolWordsBuilder.CreateFileName(filename.fileNameReference());
                bool norewind = filename.NO() != null;
                if (f != null) filenames.Add(new OpenFileName(f, norewind));
            }
            return filenames;
        }

        private IList<OpenFileName> CreateOpenFileNames(CodeElementsParser.OpenIOContext context)
        {
            if (context.fileNameReference() == null) return null;
            var filenames = new List<OpenFileName>();
            foreach (var filename in context.fileNameReference())
            {
                var f = CobolWordsBuilder.CreateFileName(filename);
                if (f != null) filenames.Add(new OpenFileName(f));
            }
            return filenames;
        }

        private IList<OpenFileName> CreateOpenFileNames(CodeElementsParser.OpenExtendContext context)
        {
            if (context.fileNameReference() == null) return null;
            var filenames = new List<OpenFileName>();
            foreach (var filename in context.fileNameReference())
            {
                var f = CobolWordsBuilder.CreateFileName(filename);
                if (f != null) filenames.Add(new OpenFileName(f));
            }
            return filenames;
        }

        ///////////////////////
        // PERFORM STATEMENT //
        ///////////////////////

        internal CodeElement CreatePerformStatement(CodeElementsParser.PerformStatementContext context)
        {
            return new PerformStatement();
        }

        internal CodeElement CreatePerformProcedureStatement()
        {
            return new PerformProcedureStatement();
        }

        ////////////////////
        // READ STATEMENT //
        ////////////////////

        internal CodeElement CreateReadStatement(CodeElementsParser.ReadStatementContext context)
        {
            if (context == null) return null;
            return new ReadStatement(
                CobolWordsBuilder.CreateFileName(context.fileNameReference()),
                CobolWordsBuilder.CreateIdentifier(context.identifier()),
                CobolWordsBuilder.CreateQualifiedName(context.qualifiedDataName()),
                context.NEXT() != null,
                context.RECORD() != null
                );
        }

        ///////////////////////
        // RELEASE STATEMENT //
        ///////////////////////

        internal CodeElement CreateReleaseStatement(CodeElementsParser.ReleaseStatementContext context)
        {
            var statement = new ReleaseStatement();
            statement.RecordName = CobolWordsBuilder.CreateQualifiedName(context.qualifiedDataName());
            statement.From = CobolWordsBuilder.CreateIdentifier(context.identifier());
            return statement;
        }
        
        //////////////////////
        // RETURN STATEMENT //
        //////////////////////

        internal ReturnStatement CreateReturnStatement(Generated.CodeElementsParser.ReturnStatementContext context)
        {
            if (context == null) return null;
            var filename = CobolWordsBuilder.CreateFileName(context.fileNameReference());
            var identifier = CobolWordsBuilder.CreateIdentifier(context.identifier());
            var statement = new ReturnStatement(filename, identifier);
            return statement;
        }
        
        ///////////////////////
        // REWRITE STATEMENT //
        ///////////////////////

        internal CodeElement CreateRewriteStatement(CodeElementsParser.RewriteStatementContext context)
        {
            if (context == null) return null;
            return new RewriteStatement(
                CobolWordsBuilder.CreateQualifiedName(context.qualifiedDataName()),
                CobolWordsBuilder.CreateIdentifier(context.identifier())
                );
        }
        
        //////////////////////
        // SEARCH STATEMENT //
        //////////////////////

        internal CodeElement CreateSerialSearchStatement(CodeElementsParser.SerialSearchContext serialSearchContext)
        {
            throw new NotImplementedException();
        }

        internal CodeElement CreateBinarySearchStatement(CodeElementsParser.BinarySearchContext binarySearchContext)
        {
            var statement = new SearchStatement();
            statement.All = context.ALL() != null;
            // SEARCH ALL? identifier
            var identifier1 = context.identifier();
            if (identifier1 != null)
            {
                statement.Element = CobolWordsBuilder.CreateIdentifier(identifier1);
                if (IdentifierUtils.IsSubscripted(statement.Element))
                    DiagnosticUtils.AddError(statement, "SEARCH: Illegal subscripted identifier", context);
                if (IdentifierUtils.IsReferenceModified(statement.Element))
                    DiagnosticUtils.AddError(statement, "SEARCH: Illegal reference-modified identifier", context);
            }
            var identifier2 = context.identifierOrIndexName();
            // SEARCH ... VARYING identifier
            if (identifier2 != null)
            {
                statement.VaryingIdentifier = CobolWordsBuilder.CreateIdentifier(identifier2);
                // TO DO : lookup symbol table to distinguish between identifier or index name
                // statement.VaryingIndex = SyntaxElementBuilder.CreateIndexName(identifier2);
            }
            if (statement.All && statement.IsVarying)
                DiagnosticUtils.AddError(statement, "Illegal VARYING after SEARCH ALL", context);
            return statement;
        }

        ///////////////////
        // SET STATEMENT //
        ///////////////////

        internal CodeElement CreateSetStatementForAssignation(CodeElementsParser.SetStatementForAssignationContext setStatementForAssignationContext)
        {
            var statement = new SetStatementForAssignation();
            if (context.identifier() != null)
            {
                statement.Receiving = new List<Expression>();
                foreach (var identifierContext in context.identifier())
                {
                    Expression receiving;
                    if (identifierContext != null)
                    {
                        receiving = CobolWordsBuilder.CreateIdentifier(identifierContext);
                    }
                    else break;
                    statement.Receiving.Add(receiving);
                }
            }

            if (context.setStatementForAssignationSending() != null)
            {
                if (context.setStatementForAssignationSending().identifier() != null)
                {
                    statement.Sending = CobolWordsBuilder.CreateIdentifier(context.setStatementForAssignationSending().identifier());
                }
                else if (context.setStatementForAssignationSending().IntegerLiteral() != null)
                {
                    statement.Sending = new Number(new SyntaxNumber(ParseTreeUtils.GetTokenFromTerminalNode(context.setStatementForAssignationSending().IntegerLiteral())));
                }
                else if (context.setStatementForAssignationSending().TRUE() != null)
                {
                    statement.Sending = new Literal(new SyntaxBoolean(ParseTreeUtils.GetTokenFromTerminalNode(context.setStatementForAssignationSending().TRUE())));
                }
                else if (context.setStatementForAssignationSending().FALSE() != null)
                {
                    statement.Sending = new Literal(new SyntaxBoolean(ParseTreeUtils.GetTokenFromTerminalNode(context.setStatementForAssignationSending().FALSE())));
                }
                else if (context.setStatementForAssignationSending().NULL() != null)
                {
                    statement.Sending = new SyntaxString(ParseTreeUtils.GetTokenFromTerminalNode(context.setStatementForAssignationSending().NULL()));
                }
                else if (context.setStatementForAssignationSending().NULLS() != null)
                {
                    statement.Sending = new SyntaxString(ParseTreeUtils.GetTokenFromTerminalNode(context.setStatementForAssignationSending().NULLS()));
                }
                else if (context.setStatementForAssignationSending().SELF() != null)
                {
                    statement.Sending =
                        new SyntaxString(ParseTreeUtils.GetTokenFromTerminalNode(context.setStatementForAssignationSending().SELF()));
                }
            }
            return statement;
        }

        internal CodeElement CreateSetStatementForIndexes(CodeElementsParser.SetStatementForIndexesContext setStatementForIndexesContext)
        {
            var statement = new SetStatementForIndex();

            if (context.indexNameReference() != null)
            {
                var indexs = new List<Index>();
                foreach (var indexNameContext in context.indexNameReference())
                {
                    indexs.Add(CobolWordsBuilder.CreateIndex(indexNameContext));
                }
                statement.ReceivingIndexs = indexs;
            }
            statement.UpBy = (context.UP() != null);
            statement.DownBy = (context.DOWN() != null);

            if (context.identifier() != null)
            {
                statement.SendingField = CobolWordsBuilder.CreateIdentifier(context.identifier());
            }
            else if (context.IntegerLiteral() != null)
            {
                statement.SendingField = new Number(new SyntaxNumber(ParseTreeUtils.GetTokenFromTerminalNode(context.IntegerLiteral())));
            }
            return statement;
        }

        internal CodeElement CreateSetStatementForSwitches(CodeElementsParser.SetStatementForSwitchesContext setStatementForSwitchesContext)
        {
            var statement = new SetStatementForSwitches();

            if (context.setStatementForSwitchesWhat() != null)
            {
                var setExternalSwitchs = new List<SetExternalSwitch>();
                foreach (var switchesWhatContext in context.setStatementForSwitchesWhat())
                {
                    var setExternalSwitch = new SetExternalSwitch();

                    if (switchesWhatContext.mnemonicForUPSISwitchNameReference() != null)
                    {
                        var mnemonics = new List<MnemonicForEnvironmentName>();
                        foreach (var mnemonicContext in switchesWhatContext.mnemonicForUPSISwitchNameReference())
                        {
                            mnemonics.Add(new MnemonicForEnvironmentName(ParseTreeUtils.GetFirstToken(mnemonicContext)));
                        }
                        setExternalSwitch.MnemonicForEnvironmentNames = mnemonics;
                    }
                    setExternalSwitch.ToOn = (switchesWhatContext.ON() != null);
                    setExternalSwitch.ToOff = (switchesWhatContext.OFF() != null);
                    setExternalSwitchs.Add(setExternalSwitch);
                }
                statement.SetExternalSwitches = setExternalSwitchs;
            }
            return statement;
        }

        internal CodeElement CreateSetStatementForConditions(CodeElementsParser.SetStatementForConditionsContext setStatementForConditionsContext)
        {
            throw new NotImplementedException();
        }
        
        ////////////////////
        // SORT STATEMENT //
        ////////////////////

        internal SortStatement CreateSortStatement(CodeElementsParser.SortStatementContext context)
        {
            var statement = new SortStatement();
            statement.FileName = CobolWordsBuilder.CreateFileName(context.fileNameReference());
            statement.Keys = CreateKeyDataItems(context.onAscendingDescendingKey());
            statement.IsDuplicates = context.DUPLICATES() != null// each of these words is only
                                  || context.WITH() != null     // used for DUPLICATES phrase,
                                  || context.IN() != null      // so the presence of any one
                                  || context.ORDER() != null; // shows us the writer's intent
            statement.CollatingSequence = CobolWordsBuilder.CreateAlphabetName(context.alphabetNameReference());
            if (context.usingFilenames()  != null) statement.Using  = CobolWordsBuilder.CreateFileNames(context.usingFilenames().fileNameReference());
            if (context.givingFilenames() != null) statement.Giving = CobolWordsBuilder.CreateFileNames(context.givingFilenames().fileNameReference());
            if (context.inputProcedure()  != null) statement.Input  = CobolWordsBuilder.CreateProcedureNames(context.inputProcedure().procedureName());
            if (context.outputProcedure() != null) statement.Output = CobolWordsBuilder.CreateProcedureNames(context.outputProcedure().procedureName());
            return statement;
        }

        private IList<KeyDataItem> CreateKeyDataItems(IReadOnlyList<CodeElementsParser.OnAscendingDescendingKeyContext> context)
        {
            var keys = new List<KeyDataItem>();
            foreach (var key in context)
            {
                KeyDataItem x = CreateKeyDataItem(key);
                if (x != null) keys.Add(x);
            }
            return keys;
        }

        private KeyDataItem CreateKeyDataItem(CodeElementsParser.OnAscendingDescendingKeyContext context)
        {
            if (context == null) return null;
            var key = new KeyDataItem();
            key.IsAscending = context.DESCENDING() == null;
            key.Data = CobolWordsBuilder.CreateQualifiedNames(context.qualifiedDataName());
            return key;
        }

        /////////////////////
        // START STATEMENT //
        /////////////////////

        internal CodeElement CreateStartStatement(CodeElementsParser.StartStatementContext context)
        {
            var statement = new StartStatement();
            statement.FileName = CobolWordsBuilder.CreateFileName(context.fileNameReference());
            statement.DataName = CobolWordsBuilder.CreateQualifiedName(context.qualifiedDataName());
            if (context.relationalOperator() != null)
                statement.Operator = new LogicalExpressionBuilder().CreateOperator(context.relationalOperator());
            return statement;
        }

        ////////////////////
        // STOP STATEMENT //
        ////////////////////

        internal CodeElement CreateStopStatement(CodeElementsParser.StopStatementContext context)
        {
            var statement = new StopStatement();
            if (context.literal() != null)
                statement.Literal = CobolWordsBuilder.CreateLiteral(context.literal());
            statement.IsStopRun = context.RUN() != null;
            return statement;
        }

        //////////////////////
        // STRING STATEMENT //
        //////////////////////

        internal CodeElement CreateStringStatement(CodeElementsParser.StringStatementContext context)
        {
            var statement = new StringStatement();

            if (context.stringStatementWhat() != null)
            {
                var statementWhatList = new List<StringStatementWhat>();
                foreach (CodeElementsParser.StringStatementWhatContext stringStatementWhatContext in context.stringStatementWhat())
                {
                    var stringStatementWhat = new StringStatementWhat();

                    if (stringStatementWhatContext.identifierToConcat != null)
                    {
                        var identifierToConcat = new List<Expression>();
                        foreach (
                            CodeElementsParser.IdentifierOrLiteralContext idOrLiteral in
                                stringStatementWhatContext.identifierOrLiteral())
                        {
                            identifierToConcat.Add(CreateIdentifierOrLiteral(idOrLiteral, statement, "String"));
                        }
                        stringStatementWhat.IdentifierToConcat = identifierToConcat;
                    }
                    //else don't set IdentifierToConcat. It will remains null


                    if (stringStatementWhatContext.stringStatementDelimiter() != null)
                    {
                        if (stringStatementWhatContext.stringStatementDelimiter().identifierOrLiteral() != null)
                        {
                            stringStatementWhat.DelimiterIdentifier =
                                CreateIdentifierOrLiteral(stringStatementWhatContext.stringStatementDelimiter().identifierOrLiteral(), statement, "String");
                        }
                        else
                        {
                            stringStatementWhat.DelimitedBySize = (stringStatementWhatContext.stringStatementDelimiter().SIZE() != null);
                        }
                    }
                    statementWhatList.Add(stringStatementWhat);
                }

                statement.StringStatementWhat = statementWhatList;
            }
            //else don't set statement.StringStatementWhat


            if (context.identifierInto != null)
            {
                statement.IntoIdentifier = CobolWordsBuilder.CreateIdentifier(context.identifierInto);
            } //else don't set statement.IntoIdentifier


            if (context.stringStatementWith() != null)
            {
                statement.PointerIdentifier = CobolWordsBuilder.CreateIdentifier(context.stringStatementWith().identifier());
            } //else don't set statement.PointerIdentifier

            return statement;
        }

        ////////////////////////
        // SUBTRACT STATEMENT //
        ////////////////////////

        internal CodeElement CreateSubtractStatement(CodeElementsParser.SubtractSimpleContext subtractSimpleContext)
        {
            var builder = new ArithmeticStatementBuilder('-');
            builder.InitializeFormat1Statement(context.identifierOrNumericLiteral(), context.identifierRounded());
            return builder.statement;
        }

        internal CodeElement CreateSubtractGivingStatement(CodeElementsParser.SubtractGivingContext subtractGivingContext)
        {
            var builder = new ArithmeticStatementBuilder('-');
            builder.InitializeFormat2Statement(context.identifierOrNumericLiteral(), context.identifierOrNumericLiteralTmp(),
                context.identifierRounded());
            return builder.statement;
        }

        internal CodeElement CreateSubtractCorrespondingStatement(CodeElementsParser.SubtractCorrespondingContext subtractCorrespondingContext)
        {
            var builder = new ArithmeticStatementBuilder('-');
            builder.InitializeFormat3Statement(context.identifier(), context.identifierRounded());
            return builder.statement;
        }

        ////////////////////////
        // UNSTRING STATEMENT //
        ////////////////////////

        internal CodeElement CreateUnstringStatement(CodeElementsParser.UnstringStatementContext context)
        {
            var statement = new UnstringStatement();

            if (context.unstringIdentifier != null)
            {
                statement.UnstringIdentifier = CobolWordsBuilder.CreateIdentifier(context.unstringIdentifier);
            }

            if (context.unstringDelimited() != null)
            {
                if (context.unstringDelimited().delimitedBy != null)
                {
                    statement.DelimitedBy = CreateIdentifierOrLiteral(context.unstringDelimited().delimitedBy, statement, "unstring");
                }

                if (context.unstringDelimited().ustringOthersDelimiters() != null)
                {
                    var otherDelimiters = new List<Expression>();
                    foreach (
                        CodeElementsParser.UstringOthersDelimitersContext ustringOthersDelimitersContext in
                            context.unstringDelimited().ustringOthersDelimiters())
                    {
                        otherDelimiters.Add(CreateIdentifierOrLiteral(ustringOthersDelimitersContext.identifierOrLiteral(), statement,
                            "Unstring"));
                    }
                    statement.OtherDelimiters = otherDelimiters;
                }
            }

            if (context.unstringReceiver() != null)
            {
                var unstringReceiverList = new List<UnstringReceiver>();
                foreach (CodeElementsParser.UnstringReceiverContext unstringReceiverContext in context.unstringReceiver())
                {
                    var unstringReceiver = new UnstringReceiver();
                    if (unstringReceiverContext.intoIdentifier != null)
                    {
                        unstringReceiver.IntoIdentifier = CobolWordsBuilder.CreateIdentifier(unstringReceiverContext.intoIdentifier);
                    }
                    if (unstringReceiverContext.unstringDelimiter() != null &&
                        unstringReceiverContext.unstringDelimiter().identifier() != null)
                    {
                        unstringReceiver.DelimiterIdentifier =
                            CobolWordsBuilder.CreateIdentifier(unstringReceiverContext.unstringDelimiter().identifier());
                    }
                    if (unstringReceiverContext.unstringCount() != null && unstringReceiverContext.unstringCount().identifier() != null)
                    {
                        unstringReceiver.CountIdentifier =
                            CobolWordsBuilder.CreateIdentifier(unstringReceiverContext.unstringCount().identifier());
                    }
                    unstringReceiverList.Add(unstringReceiver);
                }
                statement.UnstringReceivers = unstringReceiverList;
            }

            if (context.unstringPointer() != null && context.unstringPointer().identifier() != null)
            {
                statement.WithPointer = CobolWordsBuilder.CreateIdentifier(context.unstringPointer().identifier());
            }

            if (context.unstringTallying() != null && context.unstringTallying().identifier() != null)
            {
                statement.Tallying = CobolWordsBuilder.CreateIdentifier(context.unstringTallying().identifier());
            }
            return statement;
        }

        ///////////////////
        // WRITE STATEMENT //
        ///////////////////

        internal CodeElement CreateWriteStatement(CodeElementsParser.WriteStatementContext context)
        {
            if (context == null) return null;
            return new WriteStatement(
                CobolWordsBuilder.CreateQualifiedName(context.qualifiedDataName()),
                CobolWordsBuilder.CreateIdentifier(context.identifier()),
                context.BEFORE() != null,
                context.AFTER() != null,
                new ArithmeticExpressionBuilder().CreateNumberOrIdentifier(context.identifierOrInteger()),
                CobolWordsBuilder.CreateMnemonic(context.mnemonicForEnvironmentNameReference()),
                context.PAGE() != null
                );
        }

        ////////////////////////////
        // XML GENERATE STATEMENT //
        //////////////////////////// 

        internal XmlGenerateStatement CreateXmlGenerateStatement(CodeElementsParser.XmlGenerateStatementContext context)
        {
            var statement = new XmlGenerateStatement();
            statement.Encoding = CobolWordsBuilder.CreateEncoding(context.codepage());
            int c = 0;
            foreach (var identifier in context.identifier()) // context.identifier().Count is 2 outside of syntax errors
            {
                if (c == 0) statement.Receiving = CobolWordsBuilder.CreateIdentifier(identifier);
                if (c == 1) statement.Data = CobolWordsBuilder.CreateIdentifier(identifier);
                c++;
            }
            if (context.xmlCount() != null) statement.Count = CobolWordsBuilder.CreateIdentifier(context.xmlCount().identifier());
            statement.Encoding = CobolWordsBuilder.CreateEncoding(context.codepage());
            statement.IsXMLDeclaration = context.XML_DECLARATION() != null;
            statement.IsAttributes = context.ATTRIBUTES() != null;
            if (context.xmlNamespace() != null)
            {
                statement.Namespace = CreateIdentifierOrAlphanumericOrNationalLiteral(
                    context.xmlNamespace().identifier(), context.xmlNamespace().alphanumOrNationalLiteral());
            }
            if (context.xmlNamespacePrefix() != null)
            {
                statement.NamespacePrefix = CreateIdentifierOrAlphanumericOrNationalLiteral(
                    context.xmlNamespacePrefix().identifier(), context.xmlNamespacePrefix().alphanumOrNationalLiteral());
            }
            foreach (var namecontext in context.xmlName())
            {
                try {
                    var x = CreateXmlName(namecontext);
                    if (x != null) statement.Names.Add(x);
                } catch(System.InvalidOperationException ex) {
                    DiagnosticUtils.AddError(statement, "Expected: Alphanumeric or National literal", namecontext);
                }
            }
            foreach (var typecontext in context.xmlType())
            {
                var x = CreateXmlType(typecontext);
                if (x != null) statement.Types.Add(x);
            }
            foreach (var suppresscontext in context.xmlSuppress())
            {
                var x = CreateXmlSuppression(statement, suppresscontext);
                if (x != null) statement.Suppressions.Add(x);
            }
            return statement;
        }

        private Expression CreateIdentifierOrAlphanumericOrNationalLiteral(CodeElementsParser.IdentifierContext identifier, 
                                                                                  CodeElementsParser.AlphanumOrNationalLiteralContext literal) {
            Expression result = CobolWordsBuilder.CreateIdentifier(identifier);
            if (result != null) return result;
            return CobolWordsBuilder.CreateLiteral(literal);
        }

        private XmlGenerateStatement.Name CreateXmlName(CodeElementsParser.XmlNameContext context)
        {
            if (context == null) return null;
            var result = new XmlGenerateStatement.Name();
            result.Old = CobolWordsBuilder.CreateIdentifier(context.identifier());
            result.New = CobolWordsBuilder.CreateLiteral(context.alphanumOrNationalLiteral());
            return result;
        }

        private XmlGenerateStatement.Type CreateXmlType(CodeElementsParser.XmlTypeContext context)
        {
            if (context == null) return null;
            var result = new XmlGenerateStatement.Type();
            result.Data = CobolWordsBuilder.CreateIdentifier(context.identifier());
            if (context.ATTRIBUTE() != null) result.DataType = XmlGenerateStatement.Type.Mode.ATTRIBUTE;
            if (context.ELEMENT()   != null) result.DataType = XmlGenerateStatement.Type.Mode.ELEMENT;
            if (context.CONTENT()   != null) result.DataType = XmlGenerateStatement.Type.Mode.CONTENT;
            return result;
        }

        private XmlGenerateStatement.Suppression CreateXmlSuppression(CodeElement e, CodeElementsParser.XmlSuppressContext context)
        {
            XmlGenerateStatement.Suppression result = new XmlGenerateStatement.Suppression();
            result.Specific = CobolWordsBuilder.CreateIdentifier(context.identifier());
            result.Generic = CreateXmlSuppressionMode(context.xmlSuppressGeneric());
            result.When = new List<FigurativeConstant>();
            if (context.whenPhrase() != null)
            {
                string[] allowed = { "ZERO", "ZEROES", "ZEROS", "SPACE", "SPACES", "LOW-VALUE", "LOW-VALUES", "HIGH-VALUE", "HIGH-VALUES" };
                foreach (var c in context.whenPhrase().figurativeConstant())
                {
                    var constant = CobolWordsBuilder.CreateFigurativeConstant(c);
                    if (constant != null)
                    {
                        if (System.Array.IndexOf(allowed, constant.Value) < 0)
                            DiagnosticUtils.AddError(e, "XML GENERATE: Illegal figurative constant " + constant.Value, c);
                        result.When.Add(constant);
                    }
                }
            }
            return result;
        }

        private XmlGenerateStatement.Suppression.Mode CreateXmlSuppressionMode(CodeElementsParser.XmlSuppressGenericContext context)
        {
            if (context == null) return XmlGenerateStatement.Suppression.Mode.UNKNOWN;
            var result = XmlGenerateStatement.Suppression.Mode.UNKNOWN;
            if (context.ATTRIBUTE() != null)
            {
                if (context.NUMERIC() != null) {
                    result = XmlGenerateStatement.Suppression.Mode.NUMERIC_ATTRIBUTE;
                }
                else
                if (context.NONNUMERIC() != null) {
                    result = XmlGenerateStatement.Suppression.Mode.NONNUMERIC_ATTRIBUTE;
                }
                else result = XmlGenerateStatement.Suppression.Mode.ATTRIBUTE;
            }
            if (context.ELEMENT() != null)
            {
                if (context.NUMERIC() != null) {
                    result = XmlGenerateStatement.Suppression.Mode.NUMERIC_ELEMENT;
                }
                else
                if (context.NONNUMERIC() != null) {
                    result = XmlGenerateStatement.Suppression.Mode.NONNUMERIC_ELEMENT;
                }
                else result = XmlGenerateStatement.Suppression.Mode.ELEMENT;
            }
            return result;
        }

        /////////////////////////
        // XML PARSE STATEMENT //
        /////////////////////////

        internal XmlParseStatement CreateXmlParseStatement(CodeElementsParser.XmlParseStatementContext context)
        {
            var statement = new XmlParseStatement();
            int c = 0;
            foreach (var identifier in context.identifier())
            {
                if (c == 0) statement.Identifier = CobolWordsBuilder.CreateIdentifier(identifier);
                if (c == 1) statement.ValidatingIdentifier = CobolWordsBuilder.CreateIdentifier(identifier);
                c++;
            }
            statement.Encoding = CobolWordsBuilder.CreateEncoding(context.codepage());
            statement.IsReturningNational = context.RETURNING() != null;
            statement.ValidatingFile = CobolWordsBuilder.CreateXmlSchemaName(context.xmlSchemaNameReference());
            foreach (var procedure in context.procedureName())
            {
                var procedurename = CobolWordsBuilder.CreateProcedureName(procedure);
                if (procedurename != null) statement.Procedures.Add(procedurename);
            }
            return statement;
        }

        // -- Utility methods --

        private O[] BuildObjectArrrayFromParserRules<R, O>(R[] parserRules, Func<R, O> createObject)
        {
            O[] objectArray = null;
            if (parserRules != null && parserRules.Length > 0)
            {
                objectArray = new O[parserRules.Length];
                for (int i = 0; i < parserRules.Length; i++)
                {
                    objectArray[i] = createObject(parserRules[i]);
                }
            }
            return objectArray;
        }

        private SyntaxProperty<T> CreateSyntaxProperty<T>(T value, ITerminalNode terminalNode)
        {
            if (terminalNode == null)
            {
                return null;
            }
            else
            {
                return new SyntaxProperty<T>(value, ParseTreeUtils.GetFirstToken(terminalNode));
            }
        }
    }
}
