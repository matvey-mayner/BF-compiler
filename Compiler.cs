using System;
using System.Text;
using System.IO;
using System.Diagnostics;

class BFCompiler
{
    static void Main(string[] args)
    {
        Console.WriteLine("█▄▄ █▀█ ▄▀█ █ █▄░█   █▀▀   █▀▀ █▀█ █▀▄▀█ █▀█ █ █░░ █▀▀ █▀█");
        Console.WriteLine("█▄█ █▀▄ █▀█ █ █░▀█   █▀░   █▄▄ █▄█ █░▀░█ █▀▀ █ █▄▄ ██▄ █▀▄");
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: BFCompiler.exe <input.bf> <output.exe>");
            return;
        }

        string inputFilePath = args[0];
        string outputFilePath = args[1];

        try
        {
            string BFCode = File.ReadAllText(inputFilePath);
            string compiledCode = CompileBFToCSharp(BFCode);

            string tempCsFile = Path.GetTempFileName() + ".cs";
            File.WriteAllText(tempCsFile, compiledCode);

            CompileCSharpToExe(tempCsFile, outputFilePath);

            Console.WriteLine("Compilation successful! Executable written to " + outputFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static string CompileBFToCSharp(string BFCode)
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine("using System;");
        builder.AppendLine("class Program");
        builder.AppendLine("{");
        builder.AppendLine("    static void Main()");
        builder.AppendLine("    {");
        builder.AppendLine("        byte[] memory = new byte[30000];");
        builder.AppendLine("        int pointer = 0;");

        foreach (char command in BFCode)
        {
            switch (command)
            {
                case '>':
                    builder.AppendLine("        pointer++;");
                    break;
                case '<':
                    builder.AppendLine("        pointer--;");
                    break;
                case '+':
                    builder.AppendLine("        memory[pointer]++;");
                    break;
                case '-':
                    builder.AppendLine("        memory[pointer]--;");
                    break;
                case '.':
                    builder.AppendLine("        Console.Write((char)memory[pointer]);");
                    break;
                case ',':
                    builder.AppendLine("        memory[pointer] = (byte)Console.Read();");
                    break;
                case '[':
                    builder.AppendLine("        while (memory[pointer] != 0)");
                    builder.AppendLine("        {");
                    break;
                case ']':
                    builder.AppendLine("        }");
                    break;
            }
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");

        return builder.ToString();
    }

    static void CompileCSharpToExe(string inputCsFile, string outputExeFile)
    {
        Process process = new Process();
        process.StartInfo.FileName = "csc";
        process.StartInfo.Arguments = $"/out:{outputExeFile} {inputCsFile}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception("C# compilation failed: " + process.StandardError.ReadToEnd());
        }
    }
}
