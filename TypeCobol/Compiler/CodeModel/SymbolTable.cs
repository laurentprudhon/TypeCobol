﻿using System.Collections.Generic;
using TypeCobol.Compiler.CodeElements;

namespace TypeCobol.Compiler.CodeModel
{
	public class SymbolTable {
		// TODO: should have one map/list per data set.

		/// <summary>
		/// The WORKING-STORAGE SECTION describes data records that are not part
		/// of data files but are developed and processed by a program or method.
		/// The WORKING-STORAGE SECTION also describes data items whose values
		/// are assigned in the source program or method and do not change
		/// during execution of the object program.
		/// The WORKING-STORAGE SECTION for programs (and methods) can also
		/// describe external data records, which are shared by programs
		/// and methods throughout the run unit.
		///
		/// The LOCAL-STORAGE SECTION defines storage that is allocated
		/// and freed on a per-invocation basis. On each invocation,
		/// data items defined in the LOCAL-STORAGE SECTION are reallocated.
		/// Each data item that has a VALUE clause is initialized to the value
		/// specified in that clause.
		/// For nested programs, data items defined in the LOCAL-STORAGE SECTION
		/// are allocated upon each invocation of the containing outermost program.
		/// However, each data item is reinitialized to the value specified
		/// in its VALUE clause each time the nested program is invoked.
		///
		/// The LINKAGE SECTION describes data made available from another
		/// program or method.
		/// Record description entries and data item description entries in the
		/// LINKAGE SECTION provide names and descriptions, but storage within
		/// the program or method is not reserved because the data area exists elsewhere.
		/// Data items defined in the LINKAGE SECTION of the called program or invoked
		/// method can be referenced within the PROCEDURE DIVISION of that program if
		/// and only if they satisfy one of the conditions as listed in the topic.
		/// - They are operands of the USING phrase of the PROCEDURE DIVISION header
		///   or the ENTRY statement.
		/// - They are operands of SET ADDRESS OF, CALL ... BY REFERENCE ADDRESS
		///   OF, or INVOKE ... BY REFERENCE ADDRESS OF.
		/// - They are defined with a REDEFINES or RENAMES clause, the object of which
		///   satisfies the above conditions.
		/// - They are items subordinate to any item that satisfies the condition in the rules
		///   above.
		/// - They are condition-names or index-names associated with data items that satisfy
		///   any of the above conditions.
		/// </summary>
		public Dictionary<string,List<DataDescriptionEntry>> DataEntries = new Dictionary<string,List<DataDescriptionEntry>>();

		public Scope CurrentScope { get; internal set; }
		public SymbolTable EnclosingScope { get; internal set; }

		public SymbolTable(SymbolTable enclosing = null, Scope current = Scope.Program) {
			CurrentScope = current;
			EnclosingScope = enclosing;
			if (EnclosingScope == null && CurrentScope != Scope.External)
				throw new System.InvalidOperationException("Only Table of EXTERNAL symbols don't have any enclosing scope.");
		}

		public void Add(DataDescriptionEntry symbol) {
			if (symbol.Name == null) return; // fillers and uncomplete ones don't have any name to be referenced by in the symbol table
			Get(symbol).Add(symbol);
			foreach(var sub in symbol.Subordinates) Add(sub);
		}

		private Scope GetScope(DataDescriptionEntry data) {
			if (data.IsExternal) return Scope.External;
			if (data.IsGlobal) return Scope.Global;
			return Scope.Program;
		}
		private SymbolTable GetTable(SymbolTable.Scope scope) {
			if (CurrentScope == scope) return this;
			SymbolTable table = this.EnclosingScope;
			while(table != null) {
				if (table.CurrentScope == scope) return table;
				table = table.EnclosingScope;
			}
			return null;
		}

		public List<DataDescriptionEntry> Get(DataDescriptionEntry data) {
			string key = data.Name.Name;
			var table = GetTable(GetScope(data)).DataEntries;
			if (!table.ContainsKey(key))
				table[key] = new List<DataDescriptionEntry>();
			return table[key];
		}


		internal IList<DataDescriptionEntry> Get(CodeElements.Expressions.QualifiedName name) {
			IList<DataDescriptionEntry> found = Get(name.Symbol.Name);
			int max = name.DataNames.Count;
			for(int c=0; c<max; c++) {
				string pname = name.DataNames[max-c-1].Name;
				found = Filter(found, pname, c+1);
			}
			return found;
		}
		private IList<DataDescriptionEntry> Filter(IList<DataDescriptionEntry> values, string pname, int generation) {
			var filtered = new List<DataDescriptionEntry>();
			foreach(var entry in values) {
				var parent = entry.GetAncestor(generation);
				if (parent == null) continue;
				if (parent.Name.Name.Equals(pname)) filtered.Add(entry);
			}
			return filtered;
		}

		private IList<DataDescriptionEntry> Get(string key) {
			var values = new List<DataDescriptionEntry>();
			if (DataEntries.ContainsKey(key))
				values.AddRange(DataEntries[key]);
			if (EnclosingScope!= null)
				values.AddRange(EnclosingScope.Get(key));
			return values;
		}

		/// <summary>
		/// Cobol has compile time binding for variables,
		/// sometimes called static scope.
		/// Within that, Cobol supports several layers of scope:
		/// External, Global and Program scope.
		/// </summary>
		public enum Scope {
			/// <summary>
			/// Variables declared as EXTERNAL are truly global.
			/// </summary>
			External,
			/// <summary>
			/// Variables declared in WORKING STORAGE as GLOBAL are visible
			/// to the entire program in which they are declared and
			/// in all nested subprograms contained in that program.
			/// </summary>
			Global,
			/// <summary>
			/// Variables declared in WORKING STORAGE are visible
			/// to the entire program in which they are declared.
			/// Variables declared in LOCAL STORAGE are visible
			/// to the entire program in which they are declared,
			/// but are deleted and reinitialized on every invocation.
			/// An infinite number of programs can be contained within a program,
			/// and the variables of each are visible only within the scope
			/// of that individual program.
			/// Cobol does not distinguish between programs and functions/procedures.
			/// </summary>
			Program,
		}



		/// <summary>Custom types defined in the current scope and usable in this table of symbols.</summary>
		public Dictionary<string,DataDescriptionEntry> CustomTypes = new Dictionary<string,DataDescriptionEntry>();

		/// <summary>Register a data description as a custom type.</summary>
		/// <param name="data">A TYPEDEF data description</param>
		internal void RegisterCustomType(DataDescriptionEntry data) {
			if (!data.IsTypeDefinition) throw new System.ArgumentException(data.Name+" is not a TYPEDEF data description");
			CustomTypes[data.Name.Name] = data;
		}

		internal DataDescriptionEntry GetCustomType(string type) {
			SymbolTable table = this;
			while (table != null) {
				try { return CustomTypes[type]; }
				catch(KeyNotFoundException ex) { } // should be in parent scope
				table = table.EnclosingScope;
			}
			throw new System.ArgumentException(type+" is not a custom type for this scope");
		}

		internal bool IsCustomType(DataType type) {
			if (type == null) return false;
			foreach(var key in CustomTypes.Keys)
				if (key.Equals(type.Name))
					return true;
			return false;
		}
	}
}
