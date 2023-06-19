namespace erebus.Core.Scripts.Transpile
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public class erebusTranspiler {
        private erebusListener listener;

        public erebusTranspiler() {
            this.listener = new erebusListener();
        }

        public erebusParser.CompilationUnitContext GenerateAST(string input) {
            
            var inputStream = new AntlrInputStream(input);
            var lexer = new erebusLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new erebusParser(tokens);
            parser.ErrorHandler = new BailErrorStrategy();

            return parser.compilationUnit();
        }

        public string GenerateTranspiledCode(string inputText) {

            var astree = this.GenerateAST(inputText);
            ParseTreeWalker.Default.Walk(listener, astree);
            return listener.Output;
        }

        public TranspileResult RunTranspiler(string code, string filename) {
            
            var result = new TranspileResult();

            if (string.IsNullOrEmpty(code))
                return result;
            
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try {

                var resultCode = this.GenerateTranspiledCode(code);
                if (resultCode == null) {
                    watch.Stop();
                    result.TimeElapsed = watch.Elapsed.ToString();
                    return result;
                }

                File.WriteAllText(filename, resultCode);

            }
            catch (Exception e) {
                result.output = e.Message;
                Console.WriteLine(e.Message);
            }
            finally {
                watch.Stop();
                result.TimeElapsed = watch.Elapsed.ToString();
                result.output = "Successfully transpiled code!";
            }
            return result;
        }
    }
}