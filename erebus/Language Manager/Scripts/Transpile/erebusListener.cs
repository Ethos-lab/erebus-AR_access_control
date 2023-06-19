namespace erebus.Core.Transpile
{
    using Antlr4.Runtime.Misc;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System;

    public class erebusListener : erebusBaseListener
    {

        public string Output { get; private set; }

        // use for formatting indentation of each block
        public int _templateDepth = 0;

        public erebusListener()
        {
            this.Output = string.Empty;
        }

        public void IncreaseTemplateDepth()
        {
            _templateDepth++;
        }

        public void DecreaseTemplateDepth()
        {
            _templateDepth--;
            if ( _templateDepth < 0 ) 
            {
                _templateDepth = 0;
            }
        }

        public void AddTemplateDepth()
        {
            for (int i = 0; i < _templateDepth; i++)
            {
                this.Output += "\t";
            }
        }

        public override void EnterCompilationUnit([NotNull] erebusParser.CompilationUnitContext context)
        {
            this.Output += "using System.Collections.Generic;\n";
            this.Output += "using System.Reflection;\n";
            this.Output += "using System.Linq;\n";
            this.Output += "using System;\n";
            this.Output += "using UnityEngine;\n";
            this.Output += "namespace Erebus\n{\n";
            IncreaseTemplateDepth();
            AddTemplateDepth();
            this.Output += "namespace AccessControl\n";
            AddTemplateDepth();
            this.Output += "{\n";
            IncreaseTemplateDepth();
            AddTemplateDepth();
            this.Output += "public class ErebusAccessControl : BaseAssemblyEntryPoint\n";
            AddTemplateDepth();
            this.Output += "{\n";
            IncreaseTemplateDepth();
            AddTemplateDepth();
            //Fixed constructor template
            this.Output += "public ErebusAccessControl(Assembly baseAssembly, string baseProgramName) : base(baseAssembly, baseProgramName)\n";
            AddTemplateDepth();
            this.Output += "{ }\n";
        }

        public override void ExitCompilationUnit([NotNull] erebusParser.CompilationUnitContext context)
        {
            DecreaseTemplateDepth();
            AddTemplateDepth();
            this.Output += "}\n";
            DecreaseTemplateDepth();
            AddTemplateDepth();
            this.Output += "}\n";
            DecreaseTemplateDepth();
            this.Output += "}\n";
        }

        public override void EnterFunction([NotNull] erebusParser.FunctionContext context)
        {
            var fname = context.funcname().ID().GetText();            
            AddTemplateDepth();
            this.Output += $"public bool Check{fname} ()\n";
            AddTemplateDepth();
            this.Output += "{\n";
            IncreaseTemplateDepth();
        }

        public override void ExitFunction([NotNull] erebusParser.FunctionContext context)
        {
            AddTemplateDepth();
            this.Output += "return false;\n";
            DecreaseTemplateDepth();
            AddTemplateDepth();
            this.Output += "}\n\n";
        }

        public override void EnterAssignstmt([NotNull] erebusParser.AssignstmtContext context)
        {
            string target = context.ID().GetText();
            AddTemplateDepth();
            dynamic value;
            if (context.value() != null)
            {
                value = this.ResolveValue(context.value());
            }
            else 
            {
                value = context.api().GetText();
            }
            this.Output += "dynamic " + target + " = " + value + ";\n";
        }

        public override void EnterPrintstmt([NotNull] erebusParser.PrintstmtContext context)
        {
            var resolvedExp = context.value() == null ? string.Empty : this.ResolveValue(context.value());
            AddTemplateDepth();
            this.Output += $"System.Console.WriteLine({resolvedExp});\n";
        }

        public override void EnterIfstmt([NotNull] erebusParser.IfstmtContext context)
        {
            var conditionExpression = this.ResolveConditionExpression(context.conditionExpr());
            AddTemplateDepth();
            this.Output += $"if ( {conditionExpression } )\n";
            AddTemplateDepth();
            this.Output += "{\n";
            IncreaseTemplateDepth();
            AddTemplateDepth();
            if (context.POLICY().GetText() == "Allow")
            {
                this.Output += "return true;\n";
            }
            else if (context.POLICY().GetText() == "Deny")
            {
                this.Output += "return false;\n";
            }
            DecreaseTemplateDepth();
        }

        public override void ExitIfstmt([NotNull] erebusParser.IfstmtContext context)
        {
            AddTemplateDepth();
            this.Output += "}\n";
        }

        private object ResolveConditionExpression(erebusParser.ConditionExprContext conditionExprContext)
        {
            var logicalExpr = conditionExprContext.logicalExpr();
            return this.ResolveLogicalExpression(logicalExpr);
        }

        private object ResolveLogicalExpression(erebusParser.LogicalExprContext logicalExprContext)
        {
            Console.WriteLine("Resolving Logical Expression!\n");

            if (logicalExprContext.logicOp() != null)
            {
                var leftExpr = logicalExprContext.logicalExpr().First();
                var rightExpr = logicalExprContext.logicalExpr().Last();

                var left = this.ResolveLogicalExpression(leftExpr);
                var right = this.ResolveLogicalExpression(rightExpr);

                var logicOp = this.ResolveLogicOp(logicalExprContext.logicOp());

                return left + " " + logicOp + " " + right ;
            }
            
            else if (logicalExprContext.NOT() != null)
            {
                var logicalExpr = 
                    this.ResolveLogicalExpression(logicalExprContext.logicalExpr().First());
                return "! " + logicalExpr;
            }
            
            else if (logicalExprContext.comparisonExpr() != null)
            {
                return this.ResolveComparisonExpr(logicalExprContext.comparisonExpr());
            }

            else if (logicalExprContext.LPAREN() != null && logicalExprContext.RPAREN() != null)
            {
                var logicalExpr = this.ResolveLogicalExpression(logicalExprContext.logicalExpr().First());
                return logicalExprContext.LPAREN().GetText() + logicalExpr 
                                + logicalExprContext.RPAREN().GetText();
            } 

            else if (logicalExprContext.logicalEntity() != null)
            {
                return this.ResolveLogicalEntity(logicalExprContext.logicalEntity());
            }

            else return default(dynamic);

        }

        private object ResolveComparisonExpr(erebusParser.ComparisonExprContext comparisonExprContext)
        {
            if ( comparisonExprContext.cmpOp() != null)
            {
                // Console.WriteLine("Resolving CmpExpressions!\n");
                var leftOperand = comparisonExprContext.cmpOperand().First();
                var rightOperand = comparisonExprContext.cmpOperand().Last();

                var left = this.ResolveCmpOperand(leftOperand);
                var right = this.ResolveCmpOperand(rightOperand);
                
                var cmpOp = this.ResolveCmpOperator(comparisonExprContext.cmpOp());
                
                return left + " " + cmpOp + " " + right;
            }

            else if ( comparisonExprContext.CONTAINS() != null)
            {
                var listOperand = comparisonExprContext.cmpOperand().First();
                var elemOperand = comparisonExprContext.cmpOperand().Last();

                var list = this.ResolveCmpOperand(listOperand);
                var elem = this.ResolveCmpOperand(elemOperand);

                return list + ".Contains(" + elem + ")";
            }

            return default(dynamic);
        }

        private object ResolveCmpOperand(erebusParser.CmpOperandContext cmpOperandContext)
        {
            Console.WriteLine("Resolving CmpOperand!\n");
            return this.ResolveOpExpression(cmpOperandContext.opExpression());
        }

        private dynamic ResolveOpExpression(erebusParser.OpExpressionContext opExpressionContext)
        {
            
            if ( opExpressionContext.LPAREN() != null && opExpressionContext.RPAREN() != null)
            {
                var opExpression = this.ResolveOpExpression(opExpressionContext);
                return opExpressionContext.LPAREN().GetText() + opExpression + 
                        opExpressionContext.RPAREN().GetText();
            }

            else if ( opExpressionContext.numericTerm() != null )
            {
                Console.WriteLine("Resolving opExpression numeric term.\n");
                return opExpressionContext.numericTerm().GetText();

            }
            else 
            {
                var leftExpr = opExpressionContext.opExpression().First();
                var rightExpr = opExpressionContext.opExpression().Last();

                var left = this.ResolveOpExpression(leftExpr);
                var right = this.ResolveOpExpression(rightExpr);
                
                var op = string.Empty;

                if ( opExpressionContext.MULT() != null )
                {
                    op = opExpressionContext.MULT().GetText();
                }
                else if ( opExpressionContext.DIV() != null )
                {
                    op = opExpressionContext.DIV().GetText();
                }
                else if ( opExpressionContext.PLUS() != null )
                {
                    op = opExpressionContext.PLUS().GetText();
                }
                else 
                {
                    op = opExpressionContext.MINUS().GetText();
                }

                return left + " " + op + " " + right;
            }
        }

        private dynamic ResolveNumericTerm(erebusParser.NumericTermContext termContext)
        {
            if (termContext.NUMBER() != null)
            {
                return termContext.NUMBER().GetText();
            }
            else if (termContext.ID() != null)
            {
                return termContext.ID().GetText();
            }
            else return default(dynamic);
        }

        private dynamic ResolveValue(erebusParser.ValueContext valueContext)
        {
            if (valueContext.ID() != null)
            {
                return valueContext.ID().GetText();
            }
            else if (valueContext.NUMBER() != null)
            {
                return valueContext.NUMBER().GetText();
            }
            else if (valueContext.STRING() != null)
            {
                Regex regex = new Regex("/\\$\\{([^\\}]+)\\}/g");
                var contextText = valueContext.GetText();
                var replacedString = regex.Replace(contextText, "$1");
                return replacedString;
            }
            else if (valueContext.list() != null)
            {
                Console.WriteLine("Returning a list!~\n");
                return valueContext.list().GetText();
            }
            else if (valueContext.array() != null)
            {
                return valueContext.array().GetText();
            }
            else return default(dynamic);
        }
        
        private object ResolveLogicOp(erebusParser.LogicOpContext logicOpContext)
        {
            if ( logicOpContext.AND() != null )
            {
                return "&&";
            }
            else if ( logicOpContext.OR() != null )
            {
                return "||";
            }
            else return default(dynamic);
        }

        private dynamic ResolveLogicalEntity(erebusParser.LogicalEntityContext logicalEntityContext)
        {
            if (logicalEntityContext.ID() != null)
            {
                return logicalEntityContext.ID().GetText();
            }
            else if (logicalEntityContext.STRING() != null)
            {
                Regex regex = new Regex("/\\$\\{([^\\}]+)\\}/g");
                var contextText = logicalEntityContext.GetText();
                var replacedString = regex.Replace(contextText, "$1");
                return replacedString;
            }
            else if (logicalEntityContext.TRUE() != null)
            {
                return logicalEntityContext.TRUE().GetText();
            }
            if (logicalEntityContext.FALSE() != null)
            {
                return logicalEntityContext.FALSE().GetText();
            }
            else return default(dynamic);
        }

        private dynamic ResolveCmpOperator(erebusParser.CmpOpContext cmpOpContext)
        {
            if (cmpOpContext.EQUAL() != null)
            {
                return cmpOpContext.EQUAL().GetText();
            }
            if ( cmpOpContext.NOTEQUAL() != null )
            {
                return cmpOpContext.NOTEQUAL().GetText();
            }
            if ( cmpOpContext.GT() != null )
            {
                return cmpOpContext.GT().GetText();
            }
            if ( cmpOpContext.LT() != null )
            {
                return cmpOpContext.LT().GetText();
            }
            if ( cmpOpContext.GTEQ() != null )
            {
                return cmpOpContext.GTEQ().GetText();
            }
            if ( cmpOpContext.LTEQ() != null )
            {
                return cmpOpContext.LTEQ().GetText();
            }
            return default(dynamic);
        }
    }
}
