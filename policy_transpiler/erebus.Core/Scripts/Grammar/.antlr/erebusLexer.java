// Generated from /home/sgoutam/Documents/Research/AR_AccessControl/erebus_csharp/erebus/erebus.Core/Scripts/Grammar/erebus.g4 by ANTLR 4.9.2
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class erebusLexer extends Lexer {
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
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	private static String[] makeRuleNames() {
		return new String[] {
			"T__0", "T__1", "T__2", "T__3", "T__4", "WS", "COMMENT", "SEMI", "PLUS", 
			"MINUS", "MULT", "DIV", "EQUAL", "ASSIGN", "NOTEQUAL", "GT", "LT", "GTEQ", 
			"LTEQ", "AND", "OR", "NOT", "CONTAINS", "LPAREN", "RPAREN", "LBRACKET", 
			"RBRACKET", "BLOCK_OPEN", "BLOCK_CLOSE", "TRUE", "FALSE", "POLICY", "INT", 
			"NUMBER", "ID", "STRING"
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


	public erebusLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "erebus.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2%\u00fa\b\1\4\2\t"+
		"\2\4\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13"+
		"\t\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\3\2\3\2\3\2\3\2\3\2\3\2\3\2\3\2\3\2\3\3"+
		"\3\3\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\5\3\5\3\5\3"+
		"\5\3\6\3\6\3\6\3\7\6\7l\n\7\r\7\16\7m\3\7\3\7\3\b\3\b\3\b\3\b\7\bv\n\b"+
		"\f\b\16\by\13\b\3\b\3\b\3\t\3\t\3\n\3\n\3\13\3\13\3\f\3\f\3\r\3\r\3\16"+
		"\3\16\3\16\3\17\3\17\3\20\3\20\3\20\3\21\3\21\3\22\3\22\3\23\3\23\3\23"+
		"\3\24\3\24\3\24\3\25\3\25\3\25\3\26\3\26\3\26\3\27\3\27\3\30\3\30\3\30"+
		"\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30"+
		"\3\30\3\30\3\30\3\30\3\30\3\30\3\30\5\30\u00b9\n\30\3\31\3\31\3\32\3\32"+
		"\3\33\3\33\3\34\3\34\3\35\3\35\3\36\3\36\3\37\3\37\3\37\3\37\3\37\3 \3"+
		" \3 \3 \3 \3 \3!\3!\3!\3!\3!\3!\3!\3!\3!\5!\u00db\n!\3\"\6\"\u00de\n\""+
		"\r\"\16\"\u00df\3#\3#\3#\6#\u00e5\n#\r#\16#\u00e6\5#\u00e9\n#\3$\3$\7"+
		"$\u00ed\n$\f$\16$\u00f0\13$\3%\3%\7%\u00f4\n%\f%\16%\u00f7\13%\3%\3%\2"+
		"\2&\3\3\5\4\7\5\t\6\13\7\r\b\17\t\21\n\23\13\25\f\27\r\31\16\33\17\35"+
		"\20\37\21!\22#\23%\24\'\25)\26+\27-\30/\31\61\32\63\33\65\34\67\359\36"+
		";\37= ?!A\"C\2E#G$I%\3\2\b\5\2\13\f\17\17\"\"\4\2\f\f\17\17\3\2\62;\5"+
		"\2C\\aac|\6\2\62;C\\aac|\4\2\f\f$$\2\u0102\2\3\3\2\2\2\2\5\3\2\2\2\2\7"+
		"\3\2\2\2\2\t\3\2\2\2\2\13\3\2\2\2\2\r\3\2\2\2\2\17\3\2\2\2\2\21\3\2\2"+
		"\2\2\23\3\2\2\2\2\25\3\2\2\2\2\27\3\2\2\2\2\31\3\2\2\2\2\33\3\2\2\2\2"+
		"\35\3\2\2\2\2\37\3\2\2\2\2!\3\2\2\2\2#\3\2\2\2\2%\3\2\2\2\2\'\3\2\2\2"+
		"\2)\3\2\2\2\2+\3\2\2\2\2-\3\2\2\2\2/\3\2\2\2\2\61\3\2\2\2\2\63\3\2\2\2"+
		"\2\65\3\2\2\2\2\67\3\2\2\2\29\3\2\2\2\2;\3\2\2\2\2=\3\2\2\2\2?\3\2\2\2"+
		"\2A\3\2\2\2\2E\3\2\2\2\2G\3\2\2\2\2I\3\2\2\2\3K\3\2\2\2\5T\3\2\2\2\7V"+
		"\3\2\2\2\tc\3\2\2\2\13g\3\2\2\2\rk\3\2\2\2\17q\3\2\2\2\21|\3\2\2\2\23"+
		"~\3\2\2\2\25\u0080\3\2\2\2\27\u0082\3\2\2\2\31\u0084\3\2\2\2\33\u0086"+
		"\3\2\2\2\35\u0089\3\2\2\2\37\u008b\3\2\2\2!\u008e\3\2\2\2#\u0090\3\2\2"+
		"\2%\u0092\3\2\2\2\'\u0095\3\2\2\2)\u0098\3\2\2\2+\u009b\3\2\2\2-\u009e"+
		"\3\2\2\2/\u00b8\3\2\2\2\61\u00ba\3\2\2\2\63\u00bc\3\2\2\2\65\u00be\3\2"+
		"\2\2\67\u00c0\3\2\2\29\u00c2\3\2\2\2;\u00c4\3\2\2\2=\u00c6\3\2\2\2?\u00cb"+
		"\3\2\2\2A\u00da\3\2\2\2C\u00dd\3\2\2\2E\u00e1\3\2\2\2G\u00ea\3\2\2\2I"+
		"\u00f1\3\2\2\2KL\7h\2\2LM\7w\2\2MN\7p\2\2NO\7e\2\2OP\7v\2\2PQ\7k\2\2Q"+
		"R\7q\2\2RS\7p\2\2S\4\3\2\2\2TU\7.\2\2U\6\3\2\2\2VW\7e\2\2WX\7q\2\2XY\7"+
		"p\2\2YZ\7u\2\2Z[\7q\2\2[\\\7n\2\2\\]\7g\2\2]^\7\60\2\2^_\7n\2\2_`\7q\2"+
		"\2`a\7i\2\2ab\7*\2\2b\b\3\2\2\2cd\7n\2\2de\7g\2\2ef\7v\2\2f\n\3\2\2\2"+
		"gh\7k\2\2hi\7h\2\2i\f\3\2\2\2jl\t\2\2\2kj\3\2\2\2lm\3\2\2\2mk\3\2\2\2"+
		"mn\3\2\2\2no\3\2\2\2op\b\7\2\2p\16\3\2\2\2qr\7\61\2\2rs\7\61\2\2sw\3\2"+
		"\2\2tv\n\3\2\2ut\3\2\2\2vy\3\2\2\2wu\3\2\2\2wx\3\2\2\2xz\3\2\2\2yw\3\2"+
		"\2\2z{\b\b\2\2{\20\3\2\2\2|}\7=\2\2}\22\3\2\2\2~\177\7-\2\2\177\24\3\2"+
		"\2\2\u0080\u0081\7/\2\2\u0081\26\3\2\2\2\u0082\u0083\7,\2\2\u0083\30\3"+
		"\2\2\2\u0084\u0085\7\61\2\2\u0085\32\3\2\2\2\u0086\u0087\7?\2\2\u0087"+
		"\u0088\7?\2\2\u0088\34\3\2\2\2\u0089\u008a\7?\2\2\u008a\36\3\2\2\2\u008b"+
		"\u008c\7#\2\2\u008c\u008d\7?\2\2\u008d \3\2\2\2\u008e\u008f\7@\2\2\u008f"+
		"\"\3\2\2\2\u0090\u0091\7>\2\2\u0091$\3\2\2\2\u0092\u0093\7@\2\2\u0093"+
		"\u0094\7?\2\2\u0094&\3\2\2\2\u0095\u0096\7>\2\2\u0096\u0097\7?\2\2\u0097"+
		"(\3\2\2\2\u0098\u0099\7(\2\2\u0099\u009a\7(\2\2\u009a*\3\2\2\2\u009b\u009c"+
		"\7~\2\2\u009c\u009d\7~\2\2\u009d,\3\2\2\2\u009e\u009f\7#\2\2\u009f.\3"+
		"\2\2\2\u00a0\u00a1\7\60\2\2\u00a1\u00a2\7k\2\2\u00a2\u00a3\7p\2\2\u00a3"+
		"\u00a4\7e\2\2\u00a4\u00a5\7n\2\2\u00a5\u00a6\7w\2\2\u00a6\u00a7\7f\2\2"+
		"\u00a7\u00a8\7g\2\2\u00a8\u00b9\7u\2\2\u00a9\u00aa\7\60\2\2\u00aa\u00ab"+
		"\7o\2\2\u00ab\u00ac\7c\2\2\u00ac\u00ad\7v\2\2\u00ad\u00ae\7e\2\2\u00ae"+
		"\u00af\7j\2\2\u00af\u00b0\7g\2\2\u00b0\u00b9\7u\2\2\u00b1\u00b2\7\60\2"+
		"\2\u00b2\u00b3\7y\2\2\u00b3\u00b4\7k\2\2\u00b4\u00b5\7v\2\2\u00b5\u00b6"+
		"\7j\2\2\u00b6\u00b7\7k\2\2\u00b7\u00b9\7p\2\2\u00b8\u00a0\3\2\2\2\u00b8"+
		"\u00a9\3\2\2\2\u00b8\u00b1\3\2\2\2\u00b9\60\3\2\2\2\u00ba\u00bb\7*\2\2"+
		"\u00bb\62\3\2\2\2\u00bc\u00bd\7+\2\2\u00bd\64\3\2\2\2\u00be\u00bf\7]\2"+
		"\2\u00bf\66\3\2\2\2\u00c0\u00c1\7_\2\2\u00c18\3\2\2\2\u00c2\u00c3\7}\2"+
		"\2\u00c3:\3\2\2\2\u00c4\u00c5\7\177\2\2\u00c5<\3\2\2\2\u00c6\u00c7\7v"+
		"\2\2\u00c7\u00c8\7t\2\2\u00c8\u00c9\7w\2\2\u00c9\u00ca\7g\2\2\u00ca>\3"+
		"\2\2\2\u00cb\u00cc\7h\2\2\u00cc\u00cd\7c\2\2\u00cd\u00ce\7n\2\2\u00ce"+
		"\u00cf\7u\2\2\u00cf\u00d0\7g\2\2\u00d0@\3\2\2\2\u00d1\u00d2\7C\2\2\u00d2"+
		"\u00d3\7n\2\2\u00d3\u00d4\7n\2\2\u00d4\u00d5\7q\2\2\u00d5\u00db\7y\2\2"+
		"\u00d6\u00d7\7F\2\2\u00d7\u00d8\7g\2\2\u00d8\u00d9\7p\2\2\u00d9\u00db"+
		"\7{\2\2\u00da\u00d1\3\2\2\2\u00da\u00d6\3\2\2\2\u00dbB\3\2\2\2\u00dc\u00de"+
		"\t\4\2\2\u00dd\u00dc\3\2\2\2\u00de\u00df\3\2\2\2\u00df\u00dd\3\2\2\2\u00df"+
		"\u00e0\3\2\2\2\u00e0D\3\2\2\2\u00e1\u00e8\5C\"\2\u00e2\u00e4\7\60\2\2"+
		"\u00e3\u00e5\5C\"\2\u00e4\u00e3\3\2\2\2\u00e5\u00e6\3\2\2\2\u00e6\u00e4"+
		"\3\2\2\2\u00e6\u00e7\3\2\2\2\u00e7\u00e9\3\2\2\2\u00e8\u00e2\3\2\2\2\u00e8"+
		"\u00e9\3\2\2\2\u00e9F\3\2\2\2\u00ea\u00ee\t\5\2\2\u00eb\u00ed\t\6\2\2"+
		"\u00ec\u00eb\3\2\2\2\u00ed\u00f0\3\2\2\2\u00ee\u00ec\3\2\2\2\u00ee\u00ef"+
		"\3\2\2\2\u00efH\3\2\2\2\u00f0\u00ee\3\2\2\2\u00f1\u00f5\7$\2\2\u00f2\u00f4"+
		"\n\7\2\2\u00f3\u00f2\3\2\2\2\u00f4\u00f7\3\2\2\2\u00f5\u00f3\3\2\2\2\u00f5"+
		"\u00f6\3\2\2\2\u00f6\u00f8\3\2\2\2\u00f7\u00f5\3\2\2\2\u00f8\u00f9\7$"+
		"\2\2\u00f9J\3\2\2\2\f\2mw\u00b8\u00da\u00df\u00e6\u00e8\u00ee\u00f5\3"+
		"\b\2\2";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}