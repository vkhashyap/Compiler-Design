using System;
using System.IO;
using System.Collections.Generic;

namespace Compiler
{
    class Lexical_Analyzer
    {

        public static String localvalue = null;
        public static int localline = 1;
        public static int linecount = 0;
        public static string stringatocc = null;
        public static List<Tokens> allTokens = new List<Tokens>();
        public static Dictionary<string, List<string>> errorDict = new Dictionary<string, List<string>>();
        public static void Main(string[] args)
        {
            StreamReader fileToRead;

            /*Console.WriteLine("Please provide filename");
            string fileNameToConcatenate = Console.ReadLine();
            string nameOfFile = @"C:\Users\Khashyap\Documents\" + fileNameToConcatenate + ".txt";*/

            string nameOfFile = @"C:\Users\Khashyap\Documents\CompilerInputCode.txt";

            if (File.Exists(nameOfFile) == true)
            {
                try
                {
                    fileToRead = new StreamReader(nameOfFile);
                }
                catch (IOException)
                {
                    fileToRead = new StreamReader(Console.OpenStandardInput(8192));
                }
            }
            else
            {
                fileToRead = new StreamReader(Console.OpenStandardInput(8192));
                Console.WriteLine("File not found. Please check name and try again");
                Console.ReadLine();
                Environment.Exit(0);
            }

            string sourceCode = fileToRead.ReadToEnd();
            sourceCode = sourceCode.Replace("\r", "");

            Scanner scanner = new Scanner(sourceCode);
            allTokens = scanner.scan();
            Console.WriteLine("\n");
            Console.WriteLine("Line" + " Position" + "  Nature" + "             Value" + "    Message to User");
            foreach (Tokens t in allTokens)
            {
                linecount++;
                Console.WriteLine(t.ToString());
                Console.WriteLine(t.Value);

                if (t.Error == null)
                {
                    atocc(t.Type, t.Value, t.Line, t.Position);
                }
                if (t.Error != null)
                {
                    errorlog(t.Error, t.Line, t.Position);
                }

            }

            using (StreamWriter writetoatoccfile = new StreamWriter("C:\\Users\\Khashyap\\Documents\\AtoCCInput.txt", append: true))
            {

                writetoatoccfile.WriteLine(stringatocc);

            }
            Console.WriteLine("Line" + " Position" + "  Nature" + "             Value" + "    Message to User");
            foreach (KeyValuePair<string, List<string>> kp in errorDict)
            {
                string dictkey = kp.Key;
                string[] dictValue = kp.Value.ToArray();
                string[] array = new string[10];
                array = dictkey.Split(',');
                Console.WriteLine(String.Format("Line:{0} Position:{1} Type&Error:{2}", array[0], array[1], dictValue[0]));
                Console.WriteLine("\n");
            }

            var parser = new Parser(allTokens);
           //call error method
            Console.ReadLine();
        }

        public static void errorlog(String msg, int line, int pos)
        {
            String append = msg + "  @line:" + line + "  @position" + pos + "  @" + DateTime.Now;
            using (StreamWriter writetologfile = new StreamWriter("C:\\Users\\Khashyap\\Documents\\ErrorLog.txt", append: true))
            {
                writetologfile.WriteLine(append);
            }
        }

        public static void atocc(TokenType type, String value, int line, int pos)
        {

            localline++;
            if (type.ToString().Equals("Identifier"))
            {
                value = "id";
            }
            if (type.ToString().Equals("InvalidId"))
            {
                value = "";
            }
            if (type.ToString().Equals("Integer"))
            {
                value = "Integer_Value";
            }
            if (type.ToString().Equals("Float"))
            {
                value = "Float_Value";
            }
            stringatocc += value + " ";
        }
    }

}
