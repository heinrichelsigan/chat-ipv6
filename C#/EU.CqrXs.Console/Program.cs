using Area23.At.Framework.Core.Zfx;
using System.Windows;


namespace EU.CqrXs.Console
{
    internal class Program
    {

        static readonly string? progName = System.Environment.ProcessPath;
        //progName = System.AppDomain.CurrentDomain.FriendlyName;
        //progName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        //progName = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //progName = System.Environment.GetCommandLineArgs()[0];
        //progName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        static void Main(string[] args)
        {            
            if (args.Length <= 1)
            {
                Usage();
            }

            string outmes = "";
            switch (args[0])
            {
                case "-gzip": outmes = GZ.GzFile(args[1], true); break;
                case "-gunzip": outmes = GZ.GzFile(args[1], false); break;
                case "-bzip":
                case "-bzip2": outmes = BZip2.BzFile(args[1], true); break;
                case "-bunzip":
                case "-bunzip2": outmes = BZip2.BzFile(args[1], false); break;
                default: Usage(); break;
            }

            System.Console.WriteLine(outmes);

            return;
        }


        static void Usage()
        {
            System.Console.Out.WriteLine($"Usage:\t{Path.GetFileName(progName)} [-gzip|-bzip|-gunzip|-bunzip] filename");
            System.Console.Out.WriteLine($"      \t{Path.GetFileName(progName)} [-encrypt|-decrypt] -key=secret_key filename");
            System.Console.Out.WriteLine($"      \t{Path.GetFileName(progName)} [-encode|-decode] --encoding=[hex16|hex32|base32|base64|uu] filename");
            System.Console.Out.WriteLine($"      \t{Path.GetFileName(progName)} and many other things in future....");
            System.Environment.Exit(0);
        }

    }

}
