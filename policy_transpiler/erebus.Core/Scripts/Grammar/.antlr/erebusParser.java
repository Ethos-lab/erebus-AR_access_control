// Generated from /home/sgoutam/Documents/Research/AR_AccessControl/erebus_csharp/erebus/erebus.Core/Scripts/Grammar/erebus.g4 by ANTLR 4.9.2
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class erebusParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.9.2", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, WS=6, COMMENT=7, SEMI=8, PLUS=9, 
		MINUS=10, MULT=11, DIV=12, EQUAL=13, ASSIGN=14, NOTEQUAL=15, GT=16, LT=17, 
		GTEQ=18, LTEQ=19, AND=20, OR=21, NOT=22, CONTAINS=23, LPAREN=24, RPAREN=25, 
		LBRACKET=26, RBRACKET=27, BLOCK_OPEN=28, BLOCK_CLOSE=29, TRUE=30, FALSE=31, 
		POLICY=32, NUMBER=33, ID=34, STRING=35;
	public static final int
		RULE_compilationUnit = 0, RULE_statement = 1, RULE_function = 2, RULE_bodystmt = 3, 
		RULE_printstmt = 4, RULE_assignstmt = 5, RULE_ifstmt = 6, RULE_action = 7, 
		RULE_conditionExpr = 8, RULE_logicalExpr = 9, RULE_comparisonExpr = 10, 
		RULE_cmpOperand = 11, RULE_logicalEntity = 12, RULE_opExpression = 13, 
		RULE_cmpOp = 14, RULE_logicOp = 15, RULE_numericTerm = 16, RULE_funcname = 17, 
		RULE_value = 18, RULE_api = 19, RULE_list = 20, RULE_array = 21;
	private static String[] makeRuleNames() {
		return new String[] {
			"compilationUnit", "statement", "function", "bodystmt", "printstmt", 
			"assignstmt", "ifstmt", "action", "conditionExpr", "logicalExpr", "comparisonExpr", 
			"cmpOperand", "logicalEntity", "opExpression", "cmpOp", "logicOp", "numericTerm", 
			"funcname", "value", "api", "list", "array"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "'function'", "','", "'console.log('", "'let'", "'if'", null, null, 
			"';'", "'+'", "'-'", "'*'", "'/'", "'=='", "'='", "'!='", "'>'", "'<'", 
			"'>='", "'<='", "'&&'", "'||'", "'!'", null, "'('", "')'", "'['", "']'", 
			"'{'", "'}'", "'true'", "'false'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, null, null, null, null, null, "WS", "COMMENT", "SEMI", "PLUS", 
			"MINUS", "MULT", "DIV", "EQUAL", "ASSIGN", "NOTEQUAL", "GT", "LT", "GTEQ", 
			"LTEQ", "AND", "OR", "NOT", "CONTAINS", "LPAREN", "RPAREN", "LBRACKET", 
			"RBRACKET", "BLOCK_OPEN", "BLOCK_CLOSE", "TRUE", "FALSE", "POLICY", "NUMBER", 
			"ID", "STRING"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "erebus.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public erebusParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	public static class CompilationUnitContext extends ParserRuleContext {
		public TerminalNode EOF() { return getToken(erebusParser.EOF, 0); }
		public List<StatementContext> statement() {
			return getRuleContexts(StatementContext.class);
		}
		public StatementContext statement(int i) {
			return getRuleContext(StatementContext.class,i);
		}
		public CompilationUnitContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_compilationUnit; }
	}

	public final CompilationUnitContext compilationUnit() throws RecognitionException {
		CompilationUnitContext _localctx = new CompilationUnitContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_compilationUnit);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(47);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__0) {
				{
				{
				setState(44);
				statement();
				}
				}
				setState(49);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(50);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StatementContext extends ParserRuleContext {
		public FunctionContext function() {
			return getRuleContext(FunctionContext.class,0);
		}
		public StatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement; }
	}

	public final StatementContext statement() throws RecognitionException {
		StatementContext _localctx = new StatementContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_statement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(52);
			function();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FunctionContext extends ParserRuleContext {
		public FuncnameContext funcname() {
			return getRuleContext(FuncnameContext.class,0);
		}
		public TerminalNode LPAREN() { return getToken(erebusParser.LPAREN, 0); }
		public TerminalNode RPAREN() { return getToken(erebusParser.RPAREN, 0); }
		public TerminalNode BLOCK_OPEN() { return getToken(erebusParser.BLOCK_OPEN, 0); }
		public TerminalNode BLOCK_CLOSE() { return getToken(erebusParser.BLOCK_CLOSE, 0); }
		public List<TerminalNode> ID() { return getTokens(erebusParser.ID); }
		public TerminalNode ID(int i) {
			return getToken(erebusParser.ID, i);
		}
		public List<BodystmtContext> bodystmt() {
			return getRuleContexts(BodystmtContext.class);
		}
		public BodystmtContext bodystmt(int i) {
			return getRuleContext(BodystmtContext.class,i);
		}
		public FunctionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_function; }
	}

	public final FunctionContext function() throws RecognitionException {
		FunctionContext _localctx = new FunctionContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_function);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(54);
			match(T__0);
			setState(55);
			funcname();
			setState(56);
			match(LPAREN);
			setState(65);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ID) {
				{
				setState(57);
				match(ID);
				setState(62);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==T__1) {
					{
					{
					setState(58);
					match(T__1);
					setState(59);
					match(ID);
					}
					}
					setState(64);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
			}

			setState(67);
			match(RPAREN);
			setState(68);
			match(BLOCK_OPEN);
			setState(72);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__2) | (1L << T__3) | (1L << T__4) | (1L << POLICY))) != 0)) {
				{
				{
				setState(69);
				bodystmt();
				}
				}
				setState(74);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(75);
			match(BLOCK_CLOSE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BodystmtContext extends ParserRuleContext {
		public IfstmtContext ifstmt() {
			return getRuleContext(IfstmtContext.class,0);
		}
		public PrintstmtContext printstmt() {
			return getRuleContext(PrintstmtContext.class,0);
		}
		public AssignstmtContext assignstmt() {
			return getRuleContext(AssignstmtContext.class,0);
		}
		public ActionContext action() {
			return getRuleContext(ActionContext.class,0);
		}
		public BodystmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_bodystmt; }
	}

	public final BodystmtContext bodystmt() throws RecognitionException {
		BodystmtContext _localctx = new BodystmtContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_bodystmt);
		try {
			setState(81);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__4:
				enterOuterAlt(_localctx, 1);
				{
				setState(77);
				ifstmt();
				}
				break;
			case T__2:
				enterOuterAlt(_localctx, 2);
				{
				setState(78);
				printstmt();
				}
				break;
			case T__3:
				enterOuterAlt(_localctx, 3);
				{
				setState(79);
				assignstmt();
				}
				break;
			case POLICY:
				enterOuterAlt(_localctx, 4);
				{
				setState(80);
				action();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PrintstmtContext extends ParserRuleContext {
		public TerminalNode RPAREN() { return getToken(erebusParser.RPAREN, 0); }
		public TerminalNode SEMI() { return getToken(erebusParser.SEMI, 0); }
		public ValueContext value() {
			return getRuleContext(ValueContext.class,0);
		}
		public PrintstmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_printstmt; }
	}

	public final PrintstmtContext printstmt() throws RecognitionException {
		PrintstmtContext _localctx = new PrintstmtContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_printstmt);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(83);
			match(T__2);
			setState(85);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << LBRACKET) | (1L << NUMBER) | (1L << ID) | (1L << STRING))) != 0)) {
				{
				setState(84);
				value();
				}
			}

			setState(87);
			match(RPAREN);
			setState(88);
			match(SEMI);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AssignstmtContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(erebusParser.ID, 0); }
		public TerminalNode ASSIGN() { return getToken(erebusParser.ASSIGN, 0); }
		public TerminalNode SEMI() { return getToken(erebusParser.SEMI, 0); }
		public ValueContext value() {
			return getRuleContext(ValueContext.class,0);
		}
		public ApiContext api() {
			return getRuleContext(ApiContext.class,0);
		}
		public AssignstmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assignstmt; }
	}

	public final AssignstmtContext assignstmt() throws RecognitionException {
		AssignstmtContext _localctx = new AssignstmtContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_assignstmt);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(90);
			match(T__3);
			setState(91);
			match(ID);
			setState(92);
			match(ASSIGN);
			setState(95);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,6,_ctx) ) {
			case 1:
				{
				setState(93);
				value();
				}
				break;
			case 2:
				{
				setState(94);
				api();
				}
				break;
			}
			setState(97);
			match(SEMI);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IfstmtContext extends ParserRuleContext {
		public TerminalNode LPAREN() { return getToken(erebusParser.LPAREN, 0); }
		public ConditionExprContext conditionExpr() {
			return getRuleContext(ConditionExprContext.class,0);
		}
		public TerminalNode RPAREN() { return getToken(erebusParser.RPAREN, 0); }
		public TerminalNode BLOCK_OPEN() { return getToken(erebusParser.BLOCK_OPEN, 0); }
		public BodystmtContext bodystmt() {
			return getRuleContext(BodystmtContext.class,0);
		}
		public TerminalNode BLOCK_CLOSE() { return getToken(erebusParser.BLOCK_CLOSE, 0); }
		public IfstmtContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ifstmt; }
	}

	public final IfstmtContext ifstmt() throws RecognitionException {
		IfstmtContext _localctx = new IfstmtContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_ifstmt);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(99);
			match(T__4);
			setState(100);
			match(LPAREN);
			setState(101);
			conditionExpr();
			setState(102);
			match(RPAREN);
			setState(103);
			match(BLOCK_OPEN);
			setState(104);
			bodystmt();
			setState(105);
			match(BLOCK_CLOSE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ActionContext extends ParserRuleContext {
		public TerminalNode POLICY() { return getToken(erebusParser.POLICY, 0); }
		public TerminalNode SEMI() { return getToken(erebusParser.SEMI, 0); }
		public ActionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_action; }
	}

	public final ActionContext action() throws RecognitionException {
		ActionContext _localctx = new ActionContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_action);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(107);
			match(POLICY);
			setState(108);
			match(SEMI);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConditionExprContext extends ParserRuleContext {
		public LogicalExprContext logicalExpr() {
			return getRuleContext(LogicalExprContext.class,0);
		}
		public ConditionExprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_conditionExpr; }
	}

	public final ConditionExprContext conditionExpr() throws RecognitionException {
		ConditionExprContext _localctx = new ConditionExprContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_conditionExpr);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(110);
			logicalExpr(0);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LogicalExprContext extends ParserRuleContext {
		public TerminalNode NOT() { return getToken(erebusParser.NOT, 0); }
		public List<LogicalExprContext> logicalExpr() {
			return getRuleContexts(LogicalExprContext.class);
		}
		public LogicalExprContext logicalExpr(int i) {
			return getRuleContext(LogicalExprContext.class,i);
		}
		public ComparisonExprContext comparisonExpr() {
			return getRuleContext(ComparisonExprContext.class,0);
		}
		public TerminalNode LPAREN() { return getToken(erebusParser.LPAREN, 0); }
		public TerminalNode RPAREN() { return getToken(erebusParser.RPAREN, 0); }
		public LogicalEntityContext logicalEntity() {
			return getRuleContext(LogicalEntityContext.class,0);
		}
		public LogicOpContext logicOp() {
			return getRuleContext(LogicOpContext.class,0);
		}
		public LogicalExprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_logicalExpr; }
	}

	public final LogicalExprContext logicalExpr() throws RecognitionException {
		return logicalExpr(0);
	}

	private LogicalExprContext logicalExpr(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		LogicalExprContext _localctx = new LogicalExprContext(_ctx, _parentState);
		LogicalExprContext _prevctx = _localctx;
		int _startState = 18;
		enterRecursionRule(_localctx, 18, RULE_logicalExpr, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(121);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,7,_ctx) ) {
			case 1:
				{
				setState(113);
				match(NOT);
				setState(114);
				logicalExpr(4);
				}
				break;
			case 2:
				{
				setState(115);
				comparisonExpr();
				}
				break;
			case 3:
				{
				setState(116);
				match(LPAREN);
				setState(117);
				logicalExpr(0);
				setState(118);
				match(RPAREN);
				}
				break;
			case 4:
				{
				setState(120);
				logicalEntity();
				}
				break;
			}
			_ctx.stop = _input.LT(-1);
			setState(129);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,8,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					{
					_localctx = new LogicalExprContext(_parentctx, _parentState);
					pushNewRecursionContext(_localctx, _startState, RULE_logicalExpr);
					setState(123);
					if (!(precpred(_ctx, 5))) throw new FailedPredicateException(this, "precpred(_ctx, 5)");
					setState(124);
					logicOp();
					setState(125);
					logicalExpr(6);
					}
					} 
				}
				setState(131);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,8,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class ComparisonExprContext extends ParserRuleContext {
		public List<CmpOperandContext> cmpOperand() {
			return getRuleContexts(CmpOperandContext.class);
		}
		public CmpOperandContext cmpOperand(int i) {
			return getRuleContext(CmpOperandContext.class,i);
		}
		public CmpOpContext cmpOp() {
			return getRuleContext(CmpOpContext.class,0);
		}
		public TerminalNode CONTAINS() { return getToken(erebusParser.CONTAINS, 0); }
		public TerminalNode LPAREN() { return getToken(erebusParser.LPAREN, 0); }
		public TerminalNode RPAREN() { return getToken(erebusParser.RPAREN, 0); }
		public ComparisonExprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_comparisonExpr; }
	}

	public final ComparisonExprContext comparisonExpr() throws RecognitionException {
		ComparisonExprContext _localctx = new ComparisonExprContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_comparisonExpr);
		try {
			setState(142);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,9,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(132);
				cmpOperand();
				setState(133);
				cmpOp();
				setState(134);
				cmpOperand();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(136);
				cmpOperand();
				setState(137);
				match(CONTAINS);
				setState(138);
				match(LPAREN);
				setState(139);
				cmpOperand();
				setState(140);
				match(RPAREN);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CmpOperandContext extends ParserRuleContext {
		public OpExpressionContext opExpression() {
			return getRuleContext(OpExpressionContext.class,0);
		}
		public CmpOperandContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cmpOperand; }
	}

	public final CmpOperandContext cmpOperand() throws RecognitionException {
		CmpOperandContext _localctx = new CmpOperandContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_cmpOperand);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(144);
			opExpression(0);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LogicalEntityContext extends ParserRuleContext {
		public TerminalNode TRUE() { return getToken(erebusParser.TRUE, 0); }
		public TerminalNode FALSE() { return getToken(erebusParser.FALSE, 0); }
		public TerminalNode ID() { return getToken(erebusParser.ID, 0); }
		public TerminalNode STRING() { return getToken(erebusParser.STRING, 0); }
		public LogicalEntityContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_logicalEntity; }
	}

	public final LogicalEntityContext logicalEntity() throws RecognitionException {
		LogicalEntityContext _localctx = new LogicalEntityContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_logicalEntity);
		int _la;
		try {
			setState(149);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case TRUE:
			case FALSE:
				enterOuterAlt(_localctx, 1);
				{
				setState(146);
				_la = _input.LA(1);
				if ( !(_la==TRUE || _la==FALSE) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
				break;
			case ID:
				enterOuterAlt(_localctx, 2);
				{
				setState(147);
				match(ID);
				}
				break;
			case STRING:
				enterOuterAlt(_localctx, 3);
				{
				setState(148);
				match(STRING);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OpExpressionContext extends ParserRuleContext {
		public TerminalNode LPAREN() { return getToken(erebusParser.LPAREN, 0); }
		public List<OpExpressionContext> opExpression() {
			return getRuleContexts(OpExpressionContext.class);
		}
		public OpExpressionContext opExpression(int i) {
			return getRuleContext(OpExpressionContext.class,i);
		}
		public TerminalNode RPAREN() { return getToken(erebusParser.RPAREN, 0); }
		public NumericTermContext numericTerm() {
			return getRuleContext(NumericTermContext.class,0);
		}
		public TerminalNode MULT() { return getToken(erebusParser.MULT, 0); }
		public TerminalNode DIV() { return getToken(erebusParser.DIV, 0); }
		public TerminalNode PLUS() { return getToken(erebusParser.PLUS, 0); }
		public TerminalNode MINUS() { return getToken(erebusParser.MINUS, 0); }
		public OpExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_opExpression; }
	}

	public final OpExpressionContext opExpression() throws RecognitionException {
		return opExpression(0);
	}

	private OpExpressionContext opExpression(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		OpExpressionContext _localctx = new OpExpressionContext(_ctx, _parentState);
		OpExpressionContext _prevctx = _localctx;
		int _startState = 26;
		enterRecursionRule(_localctx, 26, RULE_opExpression, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(157);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case LPAREN:
				{
				setState(152);
				match(LPAREN);
				setState(153);
				opExpression(0);
				setState(154);
				match(RPAREN);
				}
				break;
			case NUMBER:
			case ID:
				{
				setState(156);
				numericTerm();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.LT(-1);
			setState(173);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,13,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(171);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,12,_ctx) ) {
					case 1:
						{
						_localctx = new OpExpressionContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_opExpression);
						setState(159);
						if (!(precpred(_ctx, 6))) throw new FailedPredicateException(this, "precpred(_ctx, 6)");
						setState(160);
						match(MULT);
						setState(161);
						opExpression(7);
						}
						break;
					case 2:
						{
						_localctx = new OpExpressionContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_opExpression);
						setState(162);
						if (!(precpred(_ctx, 5))) throw new FailedPredicateException(this, "precpred(_ctx, 5)");
						setState(163);
						match(DIV);
						setState(164);
						opExpression(6);
						}
						break;
					case 3:
						{
						_localctx = new OpExpressionContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_opExpression);
						setState(165);
						if (!(precpred(_ctx, 4))) throw new FailedPredicateException(this, "precpred(_ctx, 4)");
						setState(166);
						match(PLUS);
						setState(167);
						opExpression(5);
						}
						break;
					case 4:
						{
						_localctx = new OpExpressionContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_opExpression);
						setState(168);
						if (!(precpred(_ctx, 3))) throw new FailedPredicateException(this, "precpred(_ctx, 3)");
						setState(169);
						match(MINUS);
						setState(170);
						opExpression(4);
						}
						break;
					}
					} 
				}
				setState(175);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,13,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class CmpOpContext extends ParserRuleContext {
		public TerminalNode GT() { return getToken(erebusParser.GT, 0); }
		public TerminalNode GTEQ() { return getToken(erebusParser.GTEQ, 0); }
		public TerminalNode LT() { return getToken(erebusParser.LT, 0); }
		public TerminalNode LTEQ() { return getToken(erebusParser.LTEQ, 0); }
		public TerminalNode EQUAL() { return getToken(erebusParser.EQUAL, 0); }
		public TerminalNode NOTEQUAL() { return getToken(erebusParser.NOTEQUAL, 0); }
		public CmpOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cmpOp; }
	}

	public final CmpOpContext cmpOp() throws RecognitionException {
		CmpOpContext _localctx = new CmpOpContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_cmpOp);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(176);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << EQUAL) | (1L << NOTEQUAL) | (1L << GT) | (1L << LT) | (1L << GTEQ) | (1L << LTEQ))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LogicOpContext extends ParserRuleContext {
		public TerminalNode AND() { return getToken(erebusParser.AND, 0); }
		public TerminalNode OR() { return getToken(erebusParser.OR, 0); }
		public LogicOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_logicOp; }
	}

	public final LogicOpContext logicOp() throws RecognitionException {
		LogicOpContext _localctx = new LogicOpContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_logicOp);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(178);
			_la = _input.LA(1);
			if ( !(_la==AND || _la==OR) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NumericTermContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(erebusParser.ID, 0); }
		public TerminalNode NUMBER() { return getToken(erebusParser.NUMBER, 0); }
		public NumericTermContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_numericTerm; }
	}

	public final NumericTermContext numericTerm() throws RecognitionException {
		NumericTermContext _localctx = new NumericTermContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_numericTerm);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(180);
			_la = _input.LA(1);
			if ( !(_la==NUMBER || _la==ID) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FuncnameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(erebusParser.ID, 0); }
		public FuncnameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcname; }
	}

	public final FuncnameContext funcname() throws RecognitionException {
		FuncnameContext _localctx = new FuncnameContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_funcname);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(182);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ValueContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(erebusParser.ID, 0); }
		public TerminalNode NUMBER() { return getToken(erebusParser.NUMBER, 0); }
		public TerminalNode STRING() { return getToken(erebusParser.STRING, 0); }
		public ListContext list() {
			return getRuleContext(ListContext.class,0);
		}
		public ArrayContext array() {
			return getRuleContext(ArrayContext.class,0);
		}
		public ValueContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_value; }
	}

	public final ValueContext value() throws RecognitionException {
		ValueContext _localctx = new ValueContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_value);
		try {
			setState(189);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,14,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(184);
				match(ID);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(185);
				match(NUMBER);
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(186);
				match(STRING);
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(187);
				list();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(188);
				array();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ApiContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(erebusParser.ID, 0); }
		public TerminalNode LPAREN() { return getToken(erebusParser.LPAREN, 0); }
		public TerminalNode RPAREN() { return getToken(erebusParser.RPAREN, 0); }
		public ValueContext value() {
			return getRuleContext(ValueContext.class,0);
		}
		public ApiContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_api; }
	}

	public final ApiContext api() throws RecognitionException {
		ApiContext _localctx = new ApiContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_api);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(191);
			match(ID);
			setState(192);
			match(LPAREN);
			setState(194);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << LBRACKET) | (1L << NUMBER) | (1L << ID) | (1L << STRING))) != 0)) {
				{
				setState(193);
				value();
				}
			}

			setState(196);
			match(RPAREN);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ListContext extends ParserRuleContext {
		public TerminalNode LBRACKET() { return getToken(erebusParser.LBRACKET, 0); }
		public List<TerminalNode> STRING() { return getTokens(erebusParser.STRING); }
		public TerminalNode STRING(int i) {
			return getToken(erebusParser.STRING, i);
		}
		public TerminalNode RBRACKET() { return getToken(erebusParser.RBRACKET, 0); }
		public ListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_list; }
	}

	public final ListContext list() throws RecognitionException {
		ListContext _localctx = new ListContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(198);
			match(LBRACKET);
			setState(199);
			match(STRING);
			setState(204);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__1) {
				{
				{
				setState(200);
				match(T__1);
				setState(201);
				match(STRING);
				}
				}
				setState(206);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(207);
			match(RBRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArrayContext extends ParserRuleContext {
		public TerminalNode LBRACKET() { return getToken(erebusParser.LBRACKET, 0); }
		public List<TerminalNode> NUMBER() { return getTokens(erebusParser.NUMBER); }
		public TerminalNode NUMBER(int i) {
			return getToken(erebusParser.NUMBER, i);
		}
		public TerminalNode RBRACKET() { return getToken(erebusParser.RBRACKET, 0); }
		public ArrayContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_array; }
	}

	public final ArrayContext array() throws RecognitionException {
		ArrayContext _localctx = new ArrayContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_array);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(209);
			match(LBRACKET);
			setState(210);
			match(NUMBER);
			setState(215);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__1) {
				{
				{
				setState(211);
				match(T__1);
				setState(212);
				match(NUMBER);
				}
				}
				setState(217);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(218);
			match(RBRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 9:
			return logicalExpr_sempred((LogicalExprContext)_localctx, predIndex);
		case 13:
			return opExpression_sempred((OpExpressionContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean logicalExpr_sempred(LogicalExprContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 5);
		}
		return true;
	}
	private boolean opExpression_sempred(OpExpressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 1:
			return precpred(_ctx, 6);
		case 2:
			return precpred(_ctx, 5);
		case 3:
			return precpred(_ctx, 4);
		case 4:
			return precpred(_ctx, 3);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3%\u00df\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\3\2\7\2\60\n\2\f\2"+
		"\16\2\63\13\2\3\2\3\2\3\3\3\3\3\4\3\4\3\4\3\4\3\4\3\4\7\4?\n\4\f\4\16"+
		"\4B\13\4\5\4D\n\4\3\4\3\4\3\4\7\4I\n\4\f\4\16\4L\13\4\3\4\3\4\3\5\3\5"+
		"\3\5\3\5\5\5T\n\5\3\6\3\6\5\6X\n\6\3\6\3\6\3\6\3\7\3\7\3\7\3\7\3\7\5\7"+
		"b\n\7\3\7\3\7\3\b\3\b\3\b\3\b\3\b\3\b\3\b\3\b\3\t\3\t\3\t\3\n\3\n\3\13"+
		"\3\13\3\13\3\13\3\13\3\13\3\13\3\13\3\13\5\13|\n\13\3\13\3\13\3\13\3\13"+
		"\7\13\u0082\n\13\f\13\16\13\u0085\13\13\3\f\3\f\3\f\3\f\3\f\3\f\3\f\3"+
		"\f\3\f\3\f\5\f\u0091\n\f\3\r\3\r\3\16\3\16\3\16\5\16\u0098\n\16\3\17\3"+
		"\17\3\17\3\17\3\17\3\17\5\17\u00a0\n\17\3\17\3\17\3\17\3\17\3\17\3\17"+
		"\3\17\3\17\3\17\3\17\3\17\3\17\7\17\u00ae\n\17\f\17\16\17\u00b1\13\17"+
		"\3\20\3\20\3\21\3\21\3\22\3\22\3\23\3\23\3\24\3\24\3\24\3\24\3\24\5\24"+
		"\u00c0\n\24\3\25\3\25\3\25\5\25\u00c5\n\25\3\25\3\25\3\26\3\26\3\26\3"+
		"\26\7\26\u00cd\n\26\f\26\16\26\u00d0\13\26\3\26\3\26\3\27\3\27\3\27\3"+
		"\27\7\27\u00d8\n\27\f\27\16\27\u00db\13\27\3\27\3\27\3\27\2\4\24\34\30"+
		"\2\4\6\b\n\f\16\20\22\24\26\30\32\34\36 \"$&(*,\2\6\3\2 !\4\2\17\17\21"+
		"\25\3\2\26\27\3\2#$\2\u00e4\2\61\3\2\2\2\4\66\3\2\2\2\68\3\2\2\2\bS\3"+
		"\2\2\2\nU\3\2\2\2\f\\\3\2\2\2\16e\3\2\2\2\20m\3\2\2\2\22p\3\2\2\2\24{"+
		"\3\2\2\2\26\u0090\3\2\2\2\30\u0092\3\2\2\2\32\u0097\3\2\2\2\34\u009f\3"+
		"\2\2\2\36\u00b2\3\2\2\2 \u00b4\3\2\2\2\"\u00b6\3\2\2\2$\u00b8\3\2\2\2"+
		"&\u00bf\3\2\2\2(\u00c1\3\2\2\2*\u00c8\3\2\2\2,\u00d3\3\2\2\2.\60\5\4\3"+
		"\2/.\3\2\2\2\60\63\3\2\2\2\61/\3\2\2\2\61\62\3\2\2\2\62\64\3\2\2\2\63"+
		"\61\3\2\2\2\64\65\7\2\2\3\65\3\3\2\2\2\66\67\5\6\4\2\67\5\3\2\2\289\7"+
		"\3\2\29:\5$\23\2:C\7\32\2\2;@\7$\2\2<=\7\4\2\2=?\7$\2\2><\3\2\2\2?B\3"+
		"\2\2\2@>\3\2\2\2@A\3\2\2\2AD\3\2\2\2B@\3\2\2\2C;\3\2\2\2CD\3\2\2\2DE\3"+
		"\2\2\2EF\7\33\2\2FJ\7\36\2\2GI\5\b\5\2HG\3\2\2\2IL\3\2\2\2JH\3\2\2\2J"+
		"K\3\2\2\2KM\3\2\2\2LJ\3\2\2\2MN\7\37\2\2N\7\3\2\2\2OT\5\16\b\2PT\5\n\6"+
		"\2QT\5\f\7\2RT\5\20\t\2SO\3\2\2\2SP\3\2\2\2SQ\3\2\2\2SR\3\2\2\2T\t\3\2"+
		"\2\2UW\7\5\2\2VX\5&\24\2WV\3\2\2\2WX\3\2\2\2XY\3\2\2\2YZ\7\33\2\2Z[\7"+
		"\n\2\2[\13\3\2\2\2\\]\7\6\2\2]^\7$\2\2^a\7\20\2\2_b\5&\24\2`b\5(\25\2"+
		"a_\3\2\2\2a`\3\2\2\2bc\3\2\2\2cd\7\n\2\2d\r\3\2\2\2ef\7\7\2\2fg\7\32\2"+
		"\2gh\5\22\n\2hi\7\33\2\2ij\7\36\2\2jk\5\b\5\2kl\7\37\2\2l\17\3\2\2\2m"+
		"n\7\"\2\2no\7\n\2\2o\21\3\2\2\2pq\5\24\13\2q\23\3\2\2\2rs\b\13\1\2st\7"+
		"\30\2\2t|\5\24\13\6u|\5\26\f\2vw\7\32\2\2wx\5\24\13\2xy\7\33\2\2y|\3\2"+
		"\2\2z|\5\32\16\2{r\3\2\2\2{u\3\2\2\2{v\3\2\2\2{z\3\2\2\2|\u0083\3\2\2"+
		"\2}~\f\7\2\2~\177\5 \21\2\177\u0080\5\24\13\b\u0080\u0082\3\2\2\2\u0081"+
		"}\3\2\2\2\u0082\u0085\3\2\2\2\u0083\u0081\3\2\2\2\u0083\u0084\3\2\2\2"+
		"\u0084\25\3\2\2\2\u0085\u0083\3\2\2\2\u0086\u0087\5\30\r\2\u0087\u0088"+
		"\5\36\20\2\u0088\u0089\5\30\r\2\u0089\u0091\3\2\2\2\u008a\u008b\5\30\r"+
		"\2\u008b\u008c\7\31\2\2\u008c\u008d\7\32\2\2\u008d\u008e\5\30\r\2\u008e"+
		"\u008f\7\33\2\2\u008f\u0091\3\2\2\2\u0090\u0086\3\2\2\2\u0090\u008a\3"+
		"\2\2\2\u0091\27\3\2\2\2\u0092\u0093\5\34\17\2\u0093\31\3\2\2\2\u0094\u0098"+
		"\t\2\2\2\u0095\u0098\7$\2\2\u0096\u0098\7%\2\2\u0097\u0094\3\2\2\2\u0097"+
		"\u0095\3\2\2\2\u0097\u0096\3\2\2\2\u0098\33\3\2\2\2\u0099\u009a\b\17\1"+
		"\2\u009a\u009b\7\32\2\2\u009b\u009c\5\34\17\2\u009c\u009d\7\33\2\2\u009d"+
		"\u00a0\3\2\2\2\u009e\u00a0\5\"\22\2\u009f\u0099\3\2\2\2\u009f\u009e\3"+
		"\2\2\2\u00a0\u00af\3\2\2\2\u00a1\u00a2\f\b\2\2\u00a2\u00a3\7\r\2\2\u00a3"+
		"\u00ae\5\34\17\t\u00a4\u00a5\f\7\2\2\u00a5\u00a6\7\16\2\2\u00a6\u00ae"+
		"\5\34\17\b\u00a7\u00a8\f\6\2\2\u00a8\u00a9\7\13\2\2\u00a9\u00ae\5\34\17"+
		"\7\u00aa\u00ab\f\5\2\2\u00ab\u00ac\7\f\2\2\u00ac\u00ae\5\34\17\6\u00ad"+
		"\u00a1\3\2\2\2\u00ad\u00a4\3\2\2\2\u00ad\u00a7\3\2\2\2\u00ad\u00aa\3\2"+
		"\2\2\u00ae\u00b1\3\2\2\2\u00af\u00ad\3\2\2\2\u00af\u00b0\3\2\2\2\u00b0"+
		"\35\3\2\2\2\u00b1\u00af\3\2\2\2\u00b2\u00b3\t\3\2\2\u00b3\37\3\2\2\2\u00b4"+
		"\u00b5\t\4\2\2\u00b5!\3\2\2\2\u00b6\u00b7\t\5\2\2\u00b7#\3\2\2\2\u00b8"+
		"\u00b9\7$\2\2\u00b9%\3\2\2\2\u00ba\u00c0\7$\2\2\u00bb\u00c0\7#\2\2\u00bc"+
		"\u00c0\7%\2\2\u00bd\u00c0\5*\26\2\u00be\u00c0\5,\27\2\u00bf\u00ba\3\2"+
		"\2\2\u00bf\u00bb\3\2\2\2\u00bf\u00bc\3\2\2\2\u00bf\u00bd\3\2\2\2\u00bf"+
		"\u00be\3\2\2\2\u00c0\'\3\2\2\2\u00c1\u00c2\7$\2\2\u00c2\u00c4\7\32\2\2"+
		"\u00c3\u00c5\5&\24\2\u00c4\u00c3\3\2\2\2\u00c4\u00c5\3\2\2\2\u00c5\u00c6"+
		"\3\2\2\2\u00c6\u00c7\7\33\2\2\u00c7)\3\2\2\2\u00c8\u00c9\7\34\2\2\u00c9"+
		"\u00ce\7%\2\2\u00ca\u00cb\7\4\2\2\u00cb\u00cd\7%\2\2\u00cc\u00ca\3\2\2"+
		"\2\u00cd\u00d0\3\2\2\2\u00ce\u00cc\3\2\2\2\u00ce\u00cf\3\2\2\2\u00cf\u00d1"+
		"\3\2\2\2\u00d0\u00ce\3\2\2\2\u00d1\u00d2\7\35\2\2\u00d2+\3\2\2\2\u00d3"+
		"\u00d4\7\34\2\2\u00d4\u00d9\7#\2\2\u00d5\u00d6\7\4\2\2\u00d6\u00d8\7#"+
		"\2\2\u00d7\u00d5\3\2\2\2\u00d8\u00db\3\2\2\2\u00d9\u00d7\3\2\2\2\u00d9"+
		"\u00da\3\2\2\2\u00da\u00dc\3\2\2\2\u00db\u00d9\3\2\2\2\u00dc\u00dd\7\35"+
		"\2\2\u00dd-\3\2\2\2\24\61@CJSWa{\u0083\u0090\u0097\u009f\u00ad\u00af\u00bf"+
		"\u00c4\u00ce\u00d9";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}