using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.Zfx
{

    public static class BZip2
    {
        public static byte[] BZip(byte[] inBytes)
        {
            MemoryStream msIn = new MemoryStream();
            msIn.Write(inBytes, 0, inBytes.Length);
            msIn.Flush();
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();

            ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(msIn, msOut, false, 4);

            msOut.Flush();
            byte[] zipBytes = msOut.ToByteArray();

            msOut.Close();
            msOut.Dispose();
            msIn.Close();
            msIn.Dispose();

            return zipBytes;
        }

        public static byte[] BZipViaStream(byte[] inBytes)
        {
            int buflen = Math.Max(inBytes.Length, 4096);
            MemoryStream msIn = new MemoryStream();
            msIn.Write(inBytes, 0, inBytes.Length);
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();
            using (BZip2OutputStream bzOut = new BZip2OutputStream(msOut))
            {
                StreamUtils.Copy(msIn, bzOut, new byte[buflen]);
            }

            msIn.Close();
            msIn.Dispose();

            msOut.Flush();
            byte[] zipBytes = msOut.ToArray();
            msOut.Close();
            msOut.Dispose();

            return zipBytes;
        }


        /// <summary>
        /// BzFile bzips or bunzips a file
        /// </summary>
        /// <param name="infile"><see cref="string">full (unc) filepath to file</see></param>
        /// <param name="zip"><see cref="bool">(bool)true for bzip2, (bool)false for bunzip2 (bzip2 -d)</see></param>
        /// <returns><see cref="string">string name of processed (bzipped/bunzipped) file</see></returns>
        public static string BzFile(string inFile, bool zip = true)
        {
            if (string.IsNullOrEmpty(inFile) || !File.Exists(inFile))
                throw new ArgumentNullException("string BzFile(string inFile, bool zip = true) => inFile is either null or empty or doesn't exist!");

            string outreturn = (zip) ? $"...bzip2 {Path.GetFileName(inFile)} " : $"...bunzip2 {Path.GetFileName(inFile)} ";
            byte[] inBytes = File.ReadAllBytes(inFile);
            byte[] outBytes = (zip) ? BZipViaStream(inBytes) : BUnZipViaStream(inBytes);
            string outFile = (zip) ? inFile + "bz2" : inFile.EndsWith(".bz2") ?
                inFile.Replace(".bz2", "").Replace(".bz", "") : DateTime.Now.ToString("yy-MM-dd_") + inFile;
            File.WriteAllBytes(outFile, outBytes);

            FileInfo fi = new FileInfo(outFile);
            if (fi.Exists)
                outreturn += $"created file {fi.Name} length={fi.Length} at {fi.CreationTime.ToShortDateString()} {fi.CreationTime.ToShortTimeString()}\nin directory {fi.DirectoryName}";
            else outreturn += $"=> file {outFile} NOT created; something went wrong!";
            
            return outreturn;
        }


        public static byte[] BUnZip(byte[] inBytes)
        {
            MemoryStream msIn = new MemoryStream();
            msIn.Write(inBytes, 0, inBytes.Length);
            msIn.Flush();
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();

            ICSharpCode.SharpZipLib.BZip2.BZip2.Decompress(msIn, msOut, false);

            msOut.Flush();
            byte[] unZipBytes = msOut.ToByteArray();

            msOut.Close();
            msOut.Dispose();
            msIn.Close();
            msIn.Dispose();

            return unZipBytes;
        }


        public static byte[] BUnZipViaStream(byte[] inBytes)
        {
            int buflen = Math.Max(inBytes.Length, 4096);
            MemoryStream msIn = new MemoryStream(inBytes);
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();
            using (BZip2InputStream bzIn = new BZip2InputStream(msIn))
            {
                StreamUtils.Copy(bzIn, msOut, new byte[buflen]);
            }


            msOut.Flush();
            byte[] unZipBytes = msOut.ToArray();

            msIn.Close();
            msIn.Dispose();

            msOut.Close();
            msOut.Dispose();

            return unZipBytes;
        }

    }

}
