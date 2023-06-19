//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.10.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Scripts/Grammar/erebus.g4 by ANTLR 4.10.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="erebusParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.10.1")]
[System.CLSCompliant(false)]
public interface IerebusListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.compilationUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompilationUnit([NotNull] erebusParser.CompilationUnitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.compilationUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompilationUnit([NotNull] erebusParser.CompilationUnitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] erebusParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] erebusParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunction([NotNull] erebusParser.FunctionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunction([NotNull] erebusParser.FunctionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.bodystmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBodystmt([NotNull] erebusParser.BodystmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.bodystmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBodystmt([NotNull] erebusParser.BodystmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.printstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrintstmt([NotNull] erebusParser.PrintstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.printstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrintstmt([NotNull] erebusParser.PrintstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.assignstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignstmt([NotNull] erebusParser.AssignstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.assignstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignstmt([NotNull] erebusParser.AssignstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.ifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfstmt([NotNull] erebusParser.IfstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.ifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfstmt([NotNull] erebusParser.IfstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.action"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAction([NotNull] erebusParser.ActionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.action"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAction([NotNull] erebusParser.ActionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.conditionExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionExpr([NotNull] erebusParser.ConditionExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.conditionExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionExpr([NotNull] erebusParser.ConditionExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.logicalExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicalExpr([NotNull] erebusParser.LogicalExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.logicalExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicalExpr([NotNull] erebusParser.LogicalExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.comparisonExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComparisonExpr([NotNull] erebusParser.ComparisonExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.comparisonExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComparisonExpr([NotNull] erebusParser.ComparisonExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.cmpOperand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCmpOperand([NotNull] erebusParser.CmpOperandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.cmpOperand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCmpOperand([NotNull] erebusParser.CmpOperandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.logicalEntity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicalEntity([NotNull] erebusParser.LogicalEntityContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.logicalEntity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicalEntity([NotNull] erebusParser.LogicalEntityContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.opExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpExpression([NotNull] erebusParser.OpExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.opExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpExpression([NotNull] erebusParser.OpExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.cmpOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCmpOp([NotNull] erebusParser.CmpOpContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.cmpOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCmpOp([NotNull] erebusParser.CmpOpContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.logicOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicOp([NotNull] erebusParser.LogicOpContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.logicOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicOp([NotNull] erebusParser.LogicOpContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.numericTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumericTerm([NotNull] erebusParser.NumericTermContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.numericTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumericTerm([NotNull] erebusParser.NumericTermContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.funcname"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFuncname([NotNull] erebusParser.FuncnameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.funcname"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFuncname([NotNull] erebusParser.FuncnameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValue([NotNull] erebusParser.ValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValue([NotNull] erebusParser.ValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.api"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterApi([NotNull] erebusParser.ApiContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.api"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitApi([NotNull] erebusParser.ApiContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterList([NotNull] erebusParser.ListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitList([NotNull] erebusParser.ListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="erebusParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArray([NotNull] erebusParser.ArrayContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="erebusParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArray([NotNull] erebusParser.ArrayContext context);
}