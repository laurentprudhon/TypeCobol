﻿using System;
using TypeCobol.Compiler.Scanner;

namespace TypeCobol.Compiler.CodeElements
{
    /// <summary>
    /// mnemonic-name-1 follows the rules of formation for user-defined names. 
    /// mnemonic-name-1 can be used in ACCEPT, DISPLAY, and WRITE statements. 
    /// Mnemonic-names and environment-names need not be unique. If you
    /// choose a mnemonic-name that is also an environment-name, its definition
    /// as a mnemonic-name will take precedence over its definition as an
    /// environment-name.
    /// </summary>
    public class EnvironmentName : MnemonicOrEnvironmentName
    {

        public EnvironmentName(Token userDefinedWord) :
            base(userDefinedWord, SymbolType.EnvironmentName)
        {
            EnvironmentNameEnum envNameValue;
            Enum.TryParse<EnvironmentNameEnum>(userDefinedWord.Text, true, out envNameValue);
            EnvironmentNameValue = envNameValue;
        }

        public EnvironmentNameEnum EnvironmentNameValue { get; private set; }
    }
}
