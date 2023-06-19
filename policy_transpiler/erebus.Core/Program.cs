namespace erebus {

    using erebus.Core.Scripts.Transpile;
    
    public class Program {

        static void Main(string[] args) {
            string inputfile = "Resources/input.el";
            string outputfile = "Resources/output.c1s";
            
            try
            {
                if (File.Exists(inputfile)) {
                    var code = File.ReadAllText(inputfile);
                    Console.WriteLine(code);
                    var transpiler = new erebusTranspiler();
                    var result = transpiler.RunTranspiler(code, outputfile);

                    Console.WriteLine("TimeElapsed: {0}, \n Output: {1}", 
                                        result.TimeElapsed, result.output);
                }
                else {
                    Console.WriteLine("Input file doesn't exist!");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

