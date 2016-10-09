﻿using System;

namespace TypeCobol.Compiler.CodeElements
{
    /// <summary>
    /// Types of symbols defined by the Cobol syntax
    /// </summary>
    public enum SymbolType
    {
        // Symbols defined in the program
        AlphabetName,
        ClassName,
        CharacterClassName,
        ConditionName,
        ConditionForUPSISwitchName,
        DataName,
        DataTypeName, // <= TYPECOBOL specific : user defined data types
        ExternalClassName,
        IndexName,
        FileName,
        FunctionName, // <= TYPECOBOL specific : user defined functions
        MethodName,
        MnemonicForEnvironmentName,
        MnemonicForUPSISwitchName,
        ParagraphName,
        ProgramEntry,
        ProgramName,
        SectionName,
        SymbolicCharacter,
        XmlSchemaName,

        // External names defined by the environment
        AssignmentName,
        EnvironmentName,
        ExecTranslatorName,
        IntrinsicFunctionName,
        LibraryName,
        TextName,
        UPSISwitchName,

        // Used when the type of the symbol has not yet been resolved
        TO_BE_RESOLVED
    }
}
