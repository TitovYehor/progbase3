using System.Text;
using System.IO;


namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            ArgumentsProcessor.Run();

            // StreamReader sr = new StreamReader("../data/generator/us-500.csv");

            // sr.ReadLine();

            // StringBuilder sb = new StringBuilder();

            // for (int i = 0; i < 500; i++)
            // {
            //     string[] split = sr.ReadLine().Split(',');
            //     string splitted1 = split[0].Substring(1, split[0].Length - 2);
            //     string splitted2 = split[1].Substring(1, split[1].Length - 2);
            //     string fullname = splitted1 + " " + splitted2 + "\n";

            //     sb.Append(fullname);
            // }

            // File.WriteAllText("../data/generator/fullnames.txt", sb.ToString());

            // System.Console.WriteLine(sr.ReadLine());

            // sr.Close();
        }
    }
}
