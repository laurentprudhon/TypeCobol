﻿using System.Collections.Generic;
using System.IO;
using TypeCobol.Codegen.Skeletons;
using TypeCobol.Compiler.CodeElements;
using TypeCobol.Compiler.CodeElements.Expressions;
using TypeCobol.Compiler.CodeModel;
using TypeCobol.Compiler.Scanner;
using TypeCobol.Compiler.Text;

namespace TypeCobol.Codegen {

	public class Generator: NodeVisitor {

		private readonly List<ICobolTextLine> Output;
		private readonly TextWriter Writer;
		private readonly List<Skeleton> Skeletons;

		/// <summary>Table of symbols</summary>
		private SymbolTable Table;


		public Generator(TextWriter destination, IReadOnlyList<ICobolTextLine> source, Tools.CodeElementDiagnostics converter, List<Skeleton> skeletons) {
			Output = new List<ICobolTextLine>();
			Output.AddRange(source);
			Writer = destination;
			Skeletons = skeletons ?? new List<Skeleton>();
		}

		/// <summary>Generates code</summary>
		/// <param name="tree">Root of a syntax tree</param>
		/// <param name="table">Table of symbols</param>
		/// <param name="columns">Columns layout</param>
		public void Generate(Node tree, SymbolTable table, ColumnsLayout columns = ColumnsLayout.FreeTextFormat) {
			Table = table;
			tree.Accept(this);

			foreach(var line in Output) {
System.Console.WriteLine(line.Text);
				Writer.WriteLine(line.Text);
			}
		}

		public void Visit(Node node) {
			var lines = new List<ITokensLine>();
			var indexes = new List<int>();
			if (node.CodeElement != null) {
				foreach(var token in node.CodeElement.ConsumedTokens) {
					if (!lines.Contains(token.TokensLine)) {
						lines.Add(token.TokensLine);
					}
				}
			}

			bool children = ProcessNode(node, lines);
			if (children) foreach(var child in node.Children) child.Accept(this);
		}

		private bool ProcessNode(Node node, List<ITokensLine> lines) {
			foreach (var action in GetActions(node)) action.Execute(node, lines);
			return true;
		}
		private IEnumerable<Action> GetActions(Node node) {
			var actions = new List<Action>();
			var skeleton = GetActiveSkeleton(node);
			if (skeleton != null) {
				var properties = GetProperties(node, skeleton.Properties);
				foreach(var pattern in skeleton) {
					var action = GetAction(node, pattern);
					if (action != null) actions.Add(action);
				}
			}
			return actions;
		}

		private Skeleton GetActiveSkeleton(Node node) {
			foreach(var skeleton in Skeletons) {
				bool active = false;
				foreach(var condition in skeleton.Conditions) {
					active = active || condition.Verify(node); // OR
				}
				if (active) return skeleton;//TODO: what if more than 1 skel activates?
			}
			return null;
		}

		private Dictionary<string,object> GetProperties(Node node, IEnumerable<string> properties) {
			var result = new Dictionary<string,object>();
			var errors = new System.Text.StringBuilder();
			foreach(var pname in properties) {
				if (node[pname] != null) result[pname] = node[pname];
				else errors.Append(pname).Append(',');
			}
			if (errors.Length > 0) {
				errors.Length -= 1;
				throw new System.ArgumentException("Undefined properties for node: "+errors.ToString());
			}
			return result;
		}

		private Action GetAction(Node node, Pattern pattern) {
			var dstnode = GetLocation(node, pattern.Location);
System.Console.WriteLine("pattern.Action="+pattern.Action+" --- pattern.Location="+pattern.Location+" > "+(dstnode.CodeElement!=null?dstnode.CodeElement.GetType().Name:"?"));
//					if ("comment".Equals(pattern.Action)) return new Comment(Output);
//					if ("delete" .Equals(pattern.Action)) return new Delete(Output);
//					if ("expand" .Equals(pattern.Action)) return new GenerateCustomTypedDataDescription(Output, Table);
//					if ("replace".Equals(pattern.Action)) return new Replace(Output, CreateGeneratedText(node, pattern));
//					return new Write(Output);// no peculiar codegen --> write as is
			return null;
		}

		private Node GetLocation(Node node, string location) {
			if (location == null || location.ToLower().Equals("node")) return node;
			Node root = node;
			while(root.Parent != null) root = root.Parent;
			var result = root.Get(location);
			if (result != null) return result;
			throw new System.ArgumentException("Undefined URI: "+location);
		}



		private string CreateGeneratedText(Node node, Pattern pattern) {
			var variables = CreateVariables(pattern.Variables, node);
			string generated = Config.Cheetah.Replace(pattern.Template, variables, pattern.Delimiter);
			if (pattern.Trim) generated = generated.Trim();
			if (pattern.Indent) {
				string indent = CreateIndent(node.CodeElement as DataDescriptionEntry);
				var lines = generated.Split('\n');
				var str = new System.Text.StringBuilder();
				foreach(string line in lines) str.Append(indent+line);
				generated = str.ToString();
			}
			return generated;
		}

		private Dictionary<string,string> CreateVariables(Dictionary<string,string> variables, Node node) {
			string delimiter = "$";
			var results = new Dictionary<string,string>();
			foreach(var key in variables.Keys) {
				string value = variables[key];
				if (value.StartsWith(delimiter)) //remove starting delimiter
					value = node[value.Trim().Substring(delimiter.Length)].ToString();
				results[key] = value;
			}
			return results;
		}

		private string CreateIndent(DataDescriptionEntry data) {
			var indent = "       ";
			if (data != null) {
				for(int c=0; c<data.Generation; c++) indent += "  ";
			}
			return indent;
		}
	}

	public interface Action {
		bool Execute(Node node, List<ITokensLine> lines);
	}

	public class Write: Action {
		protected List<ICobolTextLine> output;
		protected CodeLineFactory factory;
		public Write(List<ICobolTextLine> output) {
			this.output = output;
			this.factory = new CodeLineFactory();
		}
		public virtual bool Execute(Node node, List<ITokensLine> lines) {
			// nothing to do
			return true;
		}

		protected int IndexOf(ITokensLine line) {
			int index = 0;
			foreach(var l in output)
				if (l == line) return index;
				else index++;
			return -1;
		}

		protected int Replace(ITokensLine oldline, ICobolTextLine newline) {
			int index = IndexOf(oldline);
			if (index > -1) {
				output.Remove(oldline);
				output.Insert(index, newline);
			}
			return index;
		}
	}

	public class Delete: Write {
		public Delete(List<ICobolTextLine> output): base(output) { }
		public override bool Execute(Node node, List<ITokensLine> lines) {
			foreach(var line in lines) output.Remove(line);
			return true;
		}
	}

	public class Comment: Write {
		public Comment(List<ICobolTextLine> output): base(output) { }
		public override bool Execute(Node node, List<ITokensLine> lines) {
			foreach(var line in lines) {
				Replace(line, factory.CreateCommentedLine(line));
			}
			return true;
		}
	}

	public class Replace: Write {
		private readonly string text;

		public Replace(List<ICobolTextLine> output, string text)
			: base(output) {
			this.text = text;
		}

		public override bool Execute(Node node, List<ITokensLine> lines) {
			var data = node.CodeElement as DataDescriptionEntry;
			foreach(var line in lines) {
				var index = IndexOf(line);
				output.Remove(line);
				output.Insert(index++, factory.CreateInsertedLine(factory.CreateCommentedLine(line)));
				output.Insert(index,   factory.CreateInsertedLine(text));
			}
			return false;
		}
	}

	public class GenerateCustomTypedDataDescription: Write {
		protected SymbolTable table;
		public GenerateCustomTypedDataDescription(List<ICobolTextLine> output, SymbolTable table)
			: base(output) {
			this.table = table;
		}
		public override bool Execute(Node node, List<ITokensLine> lines) {
			var data = node.CodeElement as DataDescriptionEntry;
			int index = -1;
			foreach(var line in lines) {
				index = IndexOf(line);
				output.Remove(line);
				output.Insert(index++, factory.CreateInsertedLine(factory.CreateCommentedLine(line)));
				output.Insert(index, factory.CreateCustomTypedData(line, data.DataType.Name));
			}
			var type = table.GetCustomType(data.DataType.Name);
			insertChildren(type, ref index, data.LevelNumber+1, data.Generation);
			return false;
		}

		private void insertChildren(TypeDefinition type, ref int line, int level, int generation) {
			foreach(var child in type.Subordinates) {
				bool isCustomTypeToo = !child.IsTypeDefinition && table.IsCustomType(child.DataType);
				ICobolTextLine inserted = factory.CreateDataDefinition(child, level, generation, isCustomTypeToo);
				output.Insert(++line, inserted);
				if (isCustomTypeToo) {
					var indent = "            ";
					for(int c=0; c<generation; c++) indent += "  ";
					output.Insert(++line, factory.CreateCommentedLine(factory.CreateInsertedLine(indent+child.Name+" TYPE "+child.DataType.Name)));
					insertChildren(table.GetCustomType(child.DataType.Name), ref line, level+1, generation+1);
				}
			}
		}
	}

	public class CodeLineFactory {
		public ICobolTextLine CreateEmptyLine() {
			return new EmptyLine();
		}
		public ICobolTextLine CreateCommentedLine(ITextLine line) {
			return new CommentedLine(line);
		}
		public ICobolTextLine CreateInsertedLine(ITextLine line) {
			return new InsertedLine(line.Text);
		}
		public ICobolTextLine CreateInsertedLine(string line) {
			return new InsertedLine(line);
		}

		public ICobolTextLine CreateCustomTypedData(ITokensLine line, string typename) {
			return new EditedLine(line, new RemoveCustomTypeDeclaration(typename));
		}

		internal ICobolTextLine CreateDataDefinition(DataDescriptionEntry data, int level, int generation, bool isCustomType) {
			var line = new System.Text.StringBuilder("           ");
			for(int c=0; c<generation; c++) line.Append("  ");
			line.Append(level.ToString("00")).Append(' ').Append(data.Name.Name);
			if (!isCustomType) line.Append(" PIC ").Append(data.Picture);
			line.Append('.');
			return new InsertedLine(line.ToString());
		}
	}

	public class LineForCodegen: ICobolTextLine {
		protected ITextLine OriginalLine;
		public LineForCodegen(ITextLine line) {
			this.OriginalLine = line;
		}

		/// <summary>That's the only thing we need. The rest of what's implemented throws InvalidOperationException.</summary>
		public virtual string Text { get { return OriginalLine.Text; } }

        /// from ICobolTextLine
		public CobolTextLineType Type { get { throw new System.InvalidOperationException(); } }
		public TextArea SequenceNumber { get { throw new System.InvalidOperationException(); } }
		public string SequenceNumberText { get { throw new System.InvalidOperationException(); } }
		public TextArea Indicator { get { throw new System.InvalidOperationException(); } }
		public char IndicatorChar { get { throw new System.InvalidOperationException(); } }
		public TextArea Source { get { throw new System.InvalidOperationException(); } }
		public string SourceText { get { throw new System.InvalidOperationException(); } }
		public TextArea Comment { get { throw new System.InvalidOperationException(); } }
		public string CommentText { get { throw new System.InvalidOperationException(); } }
		public Compiler.Concurrency.CompilationStep CompilationStep {
			get { throw new System.InvalidOperationException(); }
			set { throw new System.InvalidOperationException(); }
		}
		public bool CanStillBeUpdatedBy(Compiler.Concurrency.CompilationStep updatingStep) { throw new System.InvalidOperationException(); }
		// from ITextLine
		public string TextSegment(int startIndex, int endIndexInclusive) { throw new System.InvalidOperationException(); }
		public int Length { get { throw new System.InvalidOperationException(); } }
		public bool IsReadOnly { get { throw new System.InvalidOperationException(); } }
		public int InitialLineIndex { get { throw new System.InvalidOperationException(); } }
		public object LineTrackingReferenceInSourceDocument { get { throw new System.InvalidOperationException(); } }
	}

	public class EmptyLine: LineForCodegen {
		public EmptyLine(): base(null) { }
		public override string Text { get { return ""; } }
	}

	public class CommentedLine: LineForCodegen {
		public CommentedLine(ITextLine line): base(line) { }
		public override string Text {
			get {
				string edited = OriginalLine.Text;
				edited = edited.Remove(6, 1);
				edited = edited.Insert(6, "*");
				return edited;
			}
		}
	}

	public class InsertedLine: LineForCodegen {
		private readonly string text;
		public InsertedLine(string text)
			: base(null) {
			this.text = text;
		}
		public override string Text { get { return text; } }
	}

	public class EditedLine: LineForCodegen {
		protected Edit edit;
		public EditedLine(ITextLine line, Edit edit)
			: base(line) {
			this.edit = edit;
		}
		public override string Text {
			get { return edit.Edit(OriginalLine.Text); }
		}
	}

	public interface Edit {
		string Edit(string span);
	}
	public class RemoveTypeDefinition: Edit {
		public string Edit(string span) {
			int start = span.IndexOf(" TYPEDEF");
			if (start == -1 ) return span; // nothing to do
			int end = start + " TYPEDEF".Length;
			for(start=start-1; span[start]==' '; start--) ;
			start++;
			span = span.Replace(span.Substring(start,end-start), "");
			return span;
		}
	}
	public class RemoveCustomTypeDeclaration: Edit {
		private readonly string typename;
		public RemoveCustomTypeDeclaration(string typename) {
			this.typename = typename;
		}
		public string Edit(string span) {
			int start = span.IndexOf("TYPE ");
			if (start == -1 ) return span; // nothing to do
			int end = span.IndexOf(" "+typename, start) ;
			if (end != -1) end += typename.Length +1;
			else end = start + "TYPE ".Length;
			for(start=start-1; span[start]==' '; start--) ;
			start++;
			span = span.Replace(span.Substring(start,end-start), "");
			return span;
		}
	}
}
