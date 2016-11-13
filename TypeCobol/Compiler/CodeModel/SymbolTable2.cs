using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.Compiler.CodeModel
{
    /// <summary>
    /// Dictionary of all Cobol symbols defined in one visibility scope.
    /// Symbol tables can be chained together between parent / child / siblings visibility scopes.
    /// </summary>
    public class SymbolTable2
    {
        /// <summary>
        /// Constructor for the root visibility scope : CompilationProject
        /// </summary>
        public SymbolTable2()
        {

        }

        public SymbolTable2(SymbolTable2 parentScope)
        {

        }
    }

    /// <summary>
    /// See Cobol Reference p59 : Chapter 7. Scope of names
    /// </summary>
    public enum SymbolResolutionScope
    {
        CompilationProject,
        Program,
        NestedProgram,
        Function,
        Class,
        Factory,
        Object,
        Method
    }
}
