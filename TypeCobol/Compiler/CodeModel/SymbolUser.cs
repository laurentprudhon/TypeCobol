﻿using System.Collections.Generic;
using TypeCobol.Compiler.CodeElements.Expressions;

namespace TypeCobol.Compiler.CodeModel
{
	interface SymbolUser
	{
		/// <summary>List of symbols used, wether they are written or read-only.</summary>
		ICollection<QualifiedName> Symbols { get; }
	}

	interface SymbolWriter
	{
		/// <summary>
		/// List of symbol pairs: the first element of the pair is read-only
		/// and its content is written into the second element of the pair.
		/// </summary>
		ICollection<System.Tuple<QualifiedName,QualifiedName>> Symbols { get; }
		/// <summary>Are unsafe write operations allowed?</summary>
		bool IsUnsafe { get; }
	}
}
