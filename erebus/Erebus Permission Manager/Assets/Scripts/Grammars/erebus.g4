grammar erebus;

/*
* Parser
*/

compilationUnit: statement* EOF;

statement: 
        function
        ;

function    : 'function' funcname LPAREN (ID (',' ID)*)? RPAREN 
                BLOCK_OPEN
                    bodystmt*
                BLOCK_CLOSE
            ;

bodystmt    : ifstmt | printstmt | assignstmt | action;

printstmt	: 'console.log(' value? ')' SEMI;
assignstmt	: 'let' ID ASSIGN (value|api) SEMI;
ifstmt  : 
        'if' LPAREN conditionExpr RPAREN
        BLOCK_OPEN
            bodystmt
        BLOCK_CLOSE;
action  : POLICY SEMI;


conditionExpr : logicalExpr;

logicalExpr 
    : logicalExpr logicOp logicalExpr
    | NOT logicalExpr
    | comparisonExpr
    | LPAREN logicalExpr RPAREN
    | logicalEntity
    ; 

comparisonExpr
    : cmpOperand cmpOp cmpOperand
    | cmpOperand CONTAINS LPAREN cmpOperand RPAREN
    ;

cmpOperand : opExpression;

logicalEntity : (TRUE|FALSE)
            | ID | STRING ;

opExpression
    : opExpression MULT opExpression
    | opExpression DIV opExpression
    | opExpression PLUS opExpression
    | opExpression MINUS opExpression
    | LPAREN opExpression RPAREN
    | numericTerm
    ;

cmpOp: GT | GTEQ | LT | LTEQ | EQUAL | NOTEQUAL;
logicOp: AND | OR;

numericTerm: ID | NUMBER ;

funcname : ID;
value   : ID | NUMBER | STRING | list | array ;

api: ID LPAREN value? RPAREN;
list: LBRACKET STRING (',' STRING)* RBRACKET;
array: LBRACKET NUMBER (',' NUMBER)* RBRACKET;

/* 
* Lexer
*/

// Keywords
WS: [ \n\t\r]+ -> skip;
COMMENT: '//' ~ [\r\n]*  -> skip;
SEMI: ';';

PLUS    : '+';
MINUS   : '-';
MULT    : '*';
DIV     : '/';
EQUAL   : '==';
ASSIGN  : '=';
NOTEQUAL: '!=';
GT      : '>';
LT      : '<';
GTEQ    : '>=';
LTEQ    : '<=';
AND     : '&&';
OR      : '||';
NOT     : '!';
CONTAINS: '.includes' | '.matches' | '.within';

LPAREN  : '(';
RPAREN  : ')';
LBRACKET: '[';
RBRACKET: ']';
BLOCK_OPEN      : '{';
BLOCK_CLOSE     : '}';


TRUE    : 'true';
FALSE   : 'false';
POLICY  : 'Allow' | 'Deny';


fragment INT: [0-9]+;
NUMBER: INT ('.'(INT)+)?;
ID: [a-zA-Z_][a-zA-Z0-9_]*;
STRING: '"' (~('\n' | '"' ))* '"';