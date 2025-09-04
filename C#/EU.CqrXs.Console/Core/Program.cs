using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Zfx;
using System.Text;


namespace EU.CqrXs.Console.Core
{
    /// <summary>
    /// OptEnum different option types
    /// </summary>
    public enum OptEnum
    {
        Usage = 0x0,
        InParam = 0x1,
        OutP = 0x2,
        Zip = 0x3,
        Unzip = 0x4,
        Encode = 0x5,
        Decode = 0x6,
        Crypt = 0x7,
        Key = 0x8,
        Decrypt = 0x9,
        HashSum = 0xa,
        Help = 0xb
    }


    /// <summary>
    /// Console app pipe for crypt/decrypt zip/unzip encode/decode md5sum/shaSum
    /// 
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
        static readonly string? progName = System.Environment.ProcessPath;
        static readonly string? progDirectory = Path.GetFullPath(System.Environment.ProcessPath);
        private static readonly Lock _lock = new Lock();
        static string? inName = null, outName = null, outEnviron = null, key = null;
        static FileInfo? inFile = null, outFile = null;
        static byte[]? inBytes = null, outBytes = null;


        /// <summary>
        /// Console app pipe for crypt/decrypt zip/unzip encode/decode md5sum/shaSum
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            lock (_lock)
            {
                if (args.Length <= 1)
                    Usage();
                Constants.DirCreate = false;
                Constants.NOLog = true;
            }

            Dictionary<OptEnum, string> dict = new Dictionary<OptEnum, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string optStr = GetOption(args[i], out OptEnum optEnum);


                if (optEnum == OptEnum.OutP || optEnum == OptEnum.InParam)
                    ; // nothing todo for input or output options
                else if (optEnum == OptEnum.Help || optEnum == OptEnum.Usage)
                    Usage();
                else if (optEnum == OptEnum.Key)
                    key = optStr;
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
                            outBytes = BZip2.BZip(inBytes);
                        else
                            Usage("urecognized zip option: " + optStr);   
                        
                        break;
                    case OptEnum.Unzip:
                        if (optStr.ToLower().Contains("gz") || optStr.ToLower().Contains("gunzip"))
                            outBytes = GZ.GUnZipBytes(inBytes);
                        else if (optStr.ToLower().Contains("bz") || optStr.ToLower().Contains("bunzip") || optStr.ToLower().Contains("2"))
                            outBytes = BZip2.BUnZip(inBytes);
                        else
                            Usage("urecognized unzip option: " + optStr);

                        break;
                    case OptEnum.Encode:
                        outBytes = EnDeCodeHelper.EncodeBytes(inBytes, optStr);
                        break;
                    case OptEnum.Decode:
                        outBytes = EnDeCodeHelper.DecodeBytes(inBytes, optStr);
                        break;
                    // case OptEnum.Key: 
                    case OptEnum.Crypt:
                        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                            Usage($"urecognized crypt option \"{optStr}\" without --key={key} ");
                        
                        SymmCipherPipe inPipe;
                        if (string.IsNullOrEmpty(optStr))
                            inPipe = new SymmCipherPipe(key, EnDeCodeHelper.KeyToHex(key)); 
                        else
                        {
                            string[] algos = optStr.Split(",;:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            inPipe = new SymmCipherPipe(algos);
                        }
                        outBytes = inPipe.MerryGoRoundEncrpyt(inBytes, key, EnDeCodeHelper.KeyToHex(key)); // EnDeCodeHelper.KeyToHex(optStr));
                        break;
                    case OptEnum.Decrypt:
                        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))                        
                            Usage($"unrecognized decrypt option \"{optStr}\" without --key={key} ");
                        
                        SymmCipherPipe outPipe;
                        if (string.IsNullOrEmpty(optStr))
                            outPipe = new SymmCipherPipe(key, EnDeCodeHelper.KeyToHex(key));
                        else
                        {
                            string[] algos = optStr.Split(",;:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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


        /// <summary>
        /// Gets an option by argument
        /// </summary>
        /// <param name="argument">cmd line argument</param>
        /// <param name="optEnum"><see cref="OptEnum">OptEnum cmd arg option enum</see></param>
        /// <returns></returns>
        public static string GetOption(string argument, out OptEnum optEnum)
        {
            string optArg = "";
            if (string.IsNullOrEmpty(argument) || argument.Length < 2 || argument[0] != '-')
            {
                optEnum = OptEnum.Usage;
                return optArg;
            }
            string arg = argument.TrimStart("-/".ToCharArray());

            if (arg.Contains("="))
                optArg = arg.GetSubStringByPattern("=", true, "", " ", true, StringComparison.CurrentCultureIgnoreCase);

            switch (arg[0])
            {
                case 'I':
                case 'i': optEnum = OptEnum.InParam;
                    inName = optArg;
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
                    else
                        Usage($"unrecognized option: {argument}.");

                    return optArg;
                case 'O':
                case 'o': optEnum = OptEnum.OutP;
                    outName = optArg;
                    if (string.IsNullOrEmpty(outName))
                        ; // to stdout                    
                    else if (arg.ToLower().Contains("file") || optArg.Contains(LibPaths.SepChar) || optArg.Contains('.') || !string.IsNullOrEmpty(outName))
                        outFile = new FileInfo(outName);
                    else if (!string.IsNullOrEmpty(outName) || arg.ToLower().Contains("text") || optArg.StartsWith("$"))
                        outEnviron = optArg;

                    return optArg;
                case 'K':
                case 'k': optEnum = OptEnum.Key; return optArg;
                case 'Z':
                case 'z': optEnum = OptEnum.Zip; return optArg;
                case 'U':
                case 'u': optEnum = OptEnum.Unzip; return optArg;
                case 'E':
                case 'e': optEnum = OptEnum.Encode; return optArg;
                case 'd': optEnum = OptEnum.Decode; return optArg;
                case 'C':
                case 'c': optEnum = OptEnum.Crypt; return optArg;
                case 'D': optEnum = OptEnum.Decrypt; return optArg;
                case '?':
                case 'H':
                case 'h': optEnum = OptEnum.Help; Usage();  return optArg;
                case 'S':
                case 's': optEnum = OptEnum.HashSum; return optArg;
                default: optEnum = OptEnum.Usage; Usage($"unrecognized option: {argument}.");  return argument;
            }
        }

        /// <summary>
        /// Usage shows the usage of console application
        /// </summary>
        static void Usage(string errMsg = "")
        {
            if (!string.IsNullOrEmpty(errMsg))
                System.Console.Error.WriteLine(errMsg);

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
    -s | --Sum={md5|sha256|sha512} 
    -h | --help");

            System.Console.Out.WriteLine($"\nExamples:");
            System.Console.Out.WriteLine($"      \t{Path.GetFileName(progName)} -i=test.jpg -z=bzip2 -e=base32 -o=test.jpg.bz2.base32");
            System.Console.Out.WriteLine($"      \t{Path.GetFileName(progName)} -i=test.jpg.bz2.base32 -d=base32 -u=bzip2 -o=test1.jpg");
            System.Console.Out.WriteLine($"      \t{Path.GetFileName(progName)} --inFile=test.jpg --zip=gzip --crypt=AesLight,Fish3 -k=MySecretKey -e=base64 -o=test.jpg.gz.aeslight.fish3.base64");
            System.Console.Out.WriteLine($"      \t{Path.GetFileName(progName)} -i=test.jpg.gz.aeslight.fish3.base64 -d=base64  -D=AesLight,Fish3 -k=MySecretKey -e=base64  --unzip=gzip  -o=test2.jpg");

            System.Environment.Exit(0);
        }

    }

}
