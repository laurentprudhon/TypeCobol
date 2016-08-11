﻿namespace TypeCobol.Compiler.Nodes {

using TypeCobol.Compiler.CodeElements;

public interface Statement { }



public class Accept: Node<AcceptStatement>, Statement {
	public Accept(AcceptStatement statement): base(statement) { }
}

public class Alter: Node<AlterStatement>, Statement {
	public Alter(AlterStatement statement): base(statement) { }
}

public class Call: Node<CallStatement>, Statement {
	public Call(CallStatement statement): base(statement) { }
}

public class Cancel: Node<CancelStatement>, Statement {
	public Cancel(CancelStatement statement): base(statement) { }
}

public class Continue: Node<ContinueStatement>, Statement {
	public Continue(ContinueStatement statement): base(statement) { }
}

public class Delete: Node<DeleteStatement>, Statement {
	public Delete(DeleteStatement statement): base(statement) { }
}

public class Display: Node<DisplayStatement>, Statement {
	public Display(DisplayStatement statement): base(statement) { }
}

public class Entry: Node<EntryStatement>, Statement {
	public Entry(EntryStatement statement): base(statement) { }
}

public class Exec: Node<ExecStatement>, Statement {
	public Exec(ExecStatement statement): base(statement) { }
}

public class Exit: Node<ExitStatement>, Statement {
	public Exit(ExitStatement statement): base(statement) { }
}

public class ExitMethod: Node<ExitMethodStatement>, Statement {
	public ExitMethod(ExitMethodStatement statement): base(statement) { }
}

public class ExitProgram: Node<ExitProgramStatement>, Statement {
	public ExitProgram(ExitProgramStatement statement): base(statement) { }
}

public class Goback: Node<GobackStatement>, Statement {
	public Goback(GobackStatement statement): base(statement) { }
}

public class Goto: Node<GotoStatement>, Statement {
	public Goto(GotoStatement statement): base(statement) { }
}

public class Initialize: Node<InitializeStatement>, Statement {
	public Initialize(InitializeStatement statement): base(statement) { }
}

public class Inspect: Node<InspectStatement>, Statement {
	public Inspect(InspectStatement statement): base(statement) { }
}

public class Invoke: Node<InvokeStatement>, Statement {
	public Invoke(InvokeStatement statement): base(statement) { }
}

public class Merge: Node<MergeStatement>, Statement {
	public Merge(MergeStatement statement): base(statement) { }
}

public class Move: Node<MoveStatement>, Statement {
	public Move(MoveStatement statement): base(statement) { }
}

public class Release: Node<ReleaseStatement>, Statement {
	public Release(ReleaseStatement statement): base(statement) { }
}

public class Return: Node<ReturnStatement>, Statement {
	public Return(ReturnStatement statement): base(statement) { }
}

public class Set: Node<SetStatement>, Statement {
	public Set(SetStatement statement): base(statement) { }
}

public class Sort: Node<SortStatement>, Statement {
	public Sort(SortStatement statement): base(statement) { }
}

public class Start: Node<StartStatement>, Statement {
	public Start(StartStatement statement): base(statement) { }
}

public class Stop: Node<StopStatement>, Statement {
	public Stop(StopStatement statement): base(statement) { }
}

public class String: Node<StringStatement>, Statement {
	public String(StringStatement statement): base(statement) { }
}

public class Unstring: Node<UnstringStatement>, Statement {
	public Unstring(UnstringStatement statement): base(statement) { }
}

public class XmlGenerate: Node<XmlGenerateStatement>, Statement {
	public XmlGenerate(XmlGenerateStatement statement): base(statement) { }
}

public class XmlParse: Node<XmlParseStatement>, Statement {
	public XmlParse(XmlParseStatement statement): base(statement) { }
}



// --- ARITHMETIC STATEMENTS ---

public class Add: Node<AddStatement>, Statement {
	public Add(AddStatement statement): base(statement) { }
}

public class Subtract: Node<SubtractStatement>, Statement {
	public Subtract(SubtractStatement statement): base(statement) { }
}

public class Multiply: Node<MultiplyStatement>, Statement {
	public Multiply(MultiplyStatement statement): base(statement) { }
}

public class Divide: Node<DivideStatement>, Statement {
	public Divide(DivideStatement statement): base(statement) { }
}

public class Compute: Node<ComputeStatement>, Statement {
	public Compute(ComputeStatement statement): base(statement) { }
}



// --- FILE STATEMENTS ---

public class Open: Node<OpenStatement>, Statement {
	public Open(OpenStatement statement): base(statement) { }
}

public class Close: Node<CloseStatement>, Statement {
	public Close(CloseStatement statement): base(statement) { }
}

public class Read: Node<ReadStatement>, Statement {
	public Read(ReadStatement statement): base(statement) { }
}

public class Write: Node<WriteStatement>, Statement {
	public Write(WriteStatement statement): base(statement) { }
}

public class Rewrite: Node<RewriteStatement>, Statement {
	public Rewrite(RewriteStatement statement): base(statement) { }
}



// --- FLOW CONTROL STATEMENTS ---

public interface StatementCondition { }

// TODO#248
// IF
//  |---> THEN
//  |      \--> statements
//  \---> ELSE
//         \--> statements

public class If: Node<IfStatement>, Statement {
	public If(IfStatement statement): base(statement) { }
}
public class Then: Node<CodeElement>, StatementCondition {
	public Then(): base(null) { }
}
public class Else: Node<ElseCondition>, StatementCondition {
	public Else(ElseCondition statement): base(statement) { }
}
public class NextSentence: Node<NextSentenceStatement>, Statement {
	public NextSentence(NextSentenceStatement statement): base(statement) { }
}

// TODO#248
// EVALUATE
//  |---> WHEN
//  |      \--> conditions
//  |---> THEN
//  |      \--> statements
//  |---> WHEN
//  |      \--> conditions
//  |---> THEN
//  |      \--> statements
// ...
//  \---> WHEN-OTHER
//         \--> statements
//
// or maybe:
// EVALUATE
//  |---> WHEN
//  |      |--> conditions
//  |      \--> THEN
//  |            \--> statements
//  |---> WHEN
//  |      |--> conditions
//  |      \--> THEN
//  |            \--> statements
// ...
//  \---> WHEN-OTHER
//         \--> statements

public class Evaluate: Node<EvaluateStatement>, Statement {
	public Evaluate(EvaluateStatement statement): base(statement) { }
}
public class WhenGroup: Node<CodeElement>, StatementCondition {
	public WhenGroup(): base(null) { }
}
public class When: Node<WhenCondition>, StatementCondition {
	public When(WhenCondition statement): base(statement) { }
}
public class WhenOther: Node<WhenOtherCondition>, StatementCondition {
	public WhenOther(WhenOtherCondition statement): base(statement) { }
}

// TODO#248
// PERFORM
//  \---> statements

public class Perform: Node<PerformStatement>, Statement {
	public Perform(PerformStatement statement): base(statement) { }
}
public class PerformProcedure: Node<PerformProcedureStatement>, Statement {
	public PerformProcedure(PerformProcedureStatement statement): base(statement) { }
}

// TODO#248
// SEARCH
//  |---> WHEN
//  |      \--> conditions
//  |---> THEN
//  |      \--> statements
//  |---> WHEN
//  |      \--> conditions
//  |---> THEN
//         \--> statements
//
// or maybe:
// SEARCH
//  |---> WHEN
//  |      |--> conditions
//  |      \--> THEN
//  |            \--> statements
//  |---> WHEN
//         |--> conditions
//         \--> THEN
//               \--> statements
public class Search: Node<SearchStatement>, Statement {
	public Search(SearchStatement statement): base(statement) { }
}
public class WhenSearch: Node<WhenSearchCondition>, StatementCondition {
	public WhenSearch(WhenSearchCondition statement): base(statement) { }
}

} // end of namespace TypeCobol.Compiler.Nodes