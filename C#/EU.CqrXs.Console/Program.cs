using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Zfx;
using Org.BouncyCastle.Utilities;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using Windows.Networking;


namespace EU.CqrXs.Console
{
    
    /// <summary>
    /// EU.CqrXs.Console.Program 
    /// -i | --inFile= | --inText={string|EnviromentVariable} | --inStd    
    /// -o | --outFile= | --outText=EnviromentVariable | --outStd
    /// -u | --unzip={gzip|bzip2}
    /// -z | --zip={gzip|bzip2}
    /// -d | --decode={raw|hex16|hex32|base32|base64|uu}
    /// -e | --encode={raw|hex16|hex32|base32|base64|uu}
    /// -C | --crypt={[des3,fish2,fish3]|key}
    /// -D | --decrypt={[des3,fish2,fish3]|key}
    /// -s | --hashSum={md5|sha256|sha512} 
    /// -h | --help 
    /// </summary>
    internal class Program
    {

        static string? inName = null, outName = null, outEnviron = null, key = null;
        static FileInfo? inFile = null, outFile = null;
        static byte[]? inBytes = null, outBytes = null;

        static readonly string? progName = System.Environment.ProcessPath;
        static readonly string? progDirectory = Path.GetFullPath(System.Environment.ProcessPath);
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

            Dictionary<OptEnum, string> dict = new Dictionary<OptEnum, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string optStr = GetOption(args[i], out OptEnum optEnum);

                if (optEnum == OptEnum.OutP || optEnum == OptEnum.InParam)
                    ; // nothing todo for input or output options
                else if (optEnum == OptEnum.Key)
                    key = optStr;
                else if (optEnum == OptEnum.Help)
                    Usage();
                else
                    dict.Add(optEnum, optStr);
            }
            if (string.IsNullOrEmpty(inName))
            {
                System.Console.WriteLine("Reading from stdin, enter \r\n^Z (Enter Strg - z Enter) to stop reading from stdin");
                using (Stream stdin = System.Console.OpenStandardInput())
                {
                    List<byte> listBytes = new List<byte>();
                    byte[] buffer = new byte[2048];
                    int bytes;
                    while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
                        listBytes.AddRange(buffer);

                    outBytes = EnDeCodeHelper.GetBytesTrimCrLfNulls(listBytes.ToArray());
                    inBytes = new byte[outBytes.Length];
                    Array.Copy(outBytes, 0, inBytes, 0, outBytes.Length);
                }
            }


            foreach (OptEnum optVar in dict.Keys)
            {
                string optStr = dict[optVar];
                switch (optVar)
                {
                    case OptEnum.Zip:
                        if (optStr.ToLower().Contains("gz"))
                            outBytes = GZ.GZipBytes(inBytes);
                        else if (optStr.ToLower().Contains("bz"))
                            outBytes = BZip2.BZip2Bytes(inBytes);
                        else
                        {
                            System.Console.Error.WriteLine("Urecognized zip option: " + optStr);
                            Usage();
                        }
                        break;
                    case OptEnum.Unzip:
                        if (optStr.ToLower().Contains("gz") || optStr.ToLower().Contains("gunzip"))
                            outBytes = GZ.GUnZipBytes(inBytes);
                        else if (optStr.ToLower().Contains("bz") || optStr.ToLower().Contains("bunzip") || optStr.ToLower().Contains("2"))
                            outBytes = BZip2.BUnZip2Bytes(inBytes);
                        else
                        {
                            System.Console.Error.WriteLine("Urecognized unzip option: " + optStr);
                            Usage();
                        }
                        break;
                    case OptEnum.Encode:
                        outBytes = EnDeCodeHelper.EncodeBytes(inBytes, optStr);
                        break;
                    case OptEnum.Decode:
                        outBytes = EnDeCodeHelper.DecodeBytes(inBytes, optStr);
                        break;
                    case OptEnum.Key: 
                    case OptEnum.Crypt:
                        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                        {
                            System.Console.Error.WriteLine($"Urecognized crypt option \"{optStr}\" without --key={key} ");
                            Usage();
                        }
                        SymmCipherPipe inPipe;
                        if (string.IsNullOrEmpty(optStr))
                            inPipe = new SymmCipherPipe(key, EnDeCodeHelper.KeyToHex(key)); 
                        else
                        {
                            string[] algos = optStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            inPipe = new SymmCipherPipe(algos);
                        }
                        outBytes = inPipe.MerryGoRoundEncrpyt(inBytes, key, EnDeCodeHelper.KeyToHex(key)); // EnDeCodeHelper.KeyToHex(optStr));
                        break;
                    case OptEnum.Decrypt:
                        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                        {
                            System.Console.Error.WriteLine($"Urecognized decrypt option \"{optStr}\" without --key={key} ");
                            Usage();
                        }
                        SymmCipherPipe outPipe;
                        if (string.IsNullOrEmpty(optStr))
                            outPipe = new SymmCipherPipe(key, EnDeCodeHelper.KeyToHex(key));
                        else
                        {
                            string[] algos = optStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            outPipe = new SymmCipherPipe(algos);
                        }                       
                        outBytes = outPipe.DecrpytRoundGoMerry(inBytes, key, EnDeCodeHelper.KeyToHex(key));
                        break;
                    case OptEnum.HashSum:
                        if (optStr.ToLower().Contains("md5"))
                            outBytes = MD5Sum.HashBytes(inBytes);
                        else if (optStr.ToLower().Contains("sha255"))
                            outBytes = Sha256Sum.HashBytes(inBytes);
                        else if (optStr.ToLower().Contains("sha512"))
                            outBytes = Sha512Sum.HashBytes(inBytes);
                        break;
                    default: break;

                }
                inBytes = outBytes;
            }

            if (string.IsNullOrEmpty(outName))
                System.Console.WriteLine(Encoding.UTF8.GetString(outBytes));
            else if (outFile != null)
                File.WriteAllBytes(outFile.FullName, outBytes);
            else if (!string.IsNullOrEmpty(outEnviron))
                System.Environment.SetEnvironmentVariable(outEnviron, Encoding.UTF8.GetString(outBytes));


            return;
        }


        public static string GetOption(string argument, out OptEnum optEnum)
        {
            string optOut = "";
            if (string.IsNullOrEmpty(argument) || argument.Length < 2 || argument[0] != '-') 
                {
                optEnum = OptEnum.Usage;
                return optOut;
            }
            string arg = argument.TrimStart("-".ToCharArray());

            if (arg.Contains("="))
                optOut = arg.GetSubStringByPattern("=", true, "", " ", true, StringComparison.CurrentCultureIgnoreCase);

            switch (arg[0])
            {
                case 'I':
                case 'i': optEnum = OptEnum.InParam;
                    inName = optOut;
                    if (string.IsNullOrEmpty(inName))
                        ; // Else
                    else if (arg.ToLower().Contains("file") || File.Exists(inName) || File.Exists(Path.Combine(progDirectory, inName)))
                    {
                        if (File.Exists(Path.Combine(progDirectory, inName))) 
                        {
                            inFile = new FileInfo(Path.Combine(progDirectory, inName));
                            inBytes = File.ReadAllBytes(Path.Combine(progDirectory, inName));
                        }
                        else if (File.Exists(inName)) 
                        {
                            inFile = new FileInfo(inName);
                            inBytes = File.ReadAllBytes(inName);
                        }
                        
                    }
                    else if (arg.ToLower().Contains("text") || !string.IsNullOrEmpty(inName))
                    {
                        string? inStr = Environment.GetEnvironmentVariable(inName.TrimStart("$".ToCharArray()));
                        if (inStr == null || inStr.Length == 0)
                            inStr = inName;
                        inBytes = Encoding.UTF8.GetBytes(inStr);
                    }
                    else {
                        System.Console.Error.WriteLine("Unrecognized option: " + argument);
                        Usage();
                    }
                    return optOut;
                case 'O':
                case 'o': optEnum = OptEnum.OutP;
                    outName = optOut;
                    if (string.IsNullOrEmpty(outName))
                        ; // to stdout                    
                    else if (arg.ToLower().Contains("file") || optOut.Contains(LibPaths.SepChar) || optOut.Contains('.') || !string.IsNullOrEmpty(outName))
                        outFile = new FileInfo(outName);
                    else if (!string.IsNullOrEmpty(outName) || arg.ToLower().Contains("text") || optOut.StartsWith("$"))
                        outEnviron = optOut;
                    return optOut;
                case 'K':
                case 'k': optEnum = OptEnum.Key; return optOut;
                case 'Z':
                case 'z': optEnum = OptEnum.Zip; return optOut;
                case 'U':
                case 'u': optEnum = OptEnum.Unzip; return optOut;
                case 'E':
                case 'e': optEnum = OptEnum.Encode; return optOut;
                case 'd': optEnum = OptEnum.Decode; return optOut;
                case 'C':
                case 'c': optEnum = OptEnum.Crypt; return optOut;
                case 'D': optEnum = OptEnum.Decrypt; return optOut;
                case 'H':
                case 'h': optEnum = OptEnum.HashSum; return optOut;
                default: optEnum = OptEnum.Usage; return argument;
            }
        }

        static void Usage()
        {
            System.Console.Out.WriteLine("Usage:\t" + Path.GetFileName(progName) + @"
    -i | --inFile= | --inText={string|EnviromentVariable} | --inStd    
    -o | --outFile= | --outText=EnviromentVariable | --outStd
    -u | --unzip={gzip|bzip2}
    -z | --zip={gzip|bzip2}
    -d | --decode={raw|hex16|hex32|base32|base64|uu}
    -e | --encode={raw|hex16|hex32|base32|base64|uu}
    -c | --crypt={algo1,algo2,...}  
        -k | --key={secret_key}
         algo:
            Aes,AesLight,Rijndael,Des,Des3,Dstu7624,
            Aria,Camellia,CamelliaLight,Cast5,Cast6,
            BlowFish,Fish2,Fish3,ThreeFish256,
            Gost28147,Idea,Noekeon,
            RC2,RC532,RC564,RC6,
            Seed,SkipJack,Serpent,SM4,
            Tea,Tnepres,XTea,
            ZenMatrix,ZenMatrix2
    -D | --decrypt={algo1,algo2,...} 
        -k | --key={secret_key}
    -s | --hashSum={md5|sha256|sha512} 
    -h | --help");

            System.Console.Out.WriteLine($"\n      \t{Path.GetFileName(progName)} and many other things in future....");
            System.Environment.Exit(0);
        }

    }

}
