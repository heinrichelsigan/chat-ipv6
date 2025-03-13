using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Core.Zfx
{

    /// <summary>
    /// abstraction of gnu zip gzip compression & decompression
    /// </summary>
    public static class GZ
    {

        /// <summary>
        /// GZip directly, please use <see cref="GZipViaStream(byte[])"/>
        /// </summary>
        /// <param name="inBytes"><see cref="byte[]"/> inBytes</param>
        /// <returns><see cref="byte[]"/> outbytes</returns>
        public static byte[] GZip(byte[] inBytes)
        {
            MemoryStream msIn = new MemoryStream();
            msIn.Write(inBytes, 0, inBytes.Length);
            msIn.Flush();
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();

            ICSharpCode.SharpZipLib.GZip.GZip.Compress(msIn, msOut, false);

            msOut.Flush();
            byte[] zipBytes = msOut.ToByteArray();

            msOut.Close();
            msOut.Dispose();
            msIn.Close();
            msIn.Dispose();

            return zipBytes;
        }

        /// <summary>
        /// GZipViaStream 
        /// </summary>
        /// <param name="inBytes"><see cref="byte[]"/> inBytes</param>
        /// <returns><see cref="byte[]"/> outbytes</returns>
        public static byte[] GZipViaStream(byte[] inBytes)
        {
            MemoryStream msIn = new MemoryStream();
            msIn.Write(inBytes, 0, inBytes.Length);
            msIn.Flush();
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();
            int buflen = Math.Max(inBytes.Length, 4096);


            // using (GZipOutputStream gzOut = new GZipOutputStream(msOut, buflen))
            using (GZipStream gzOut = new GZipStream(msOut, CompressionMode.Compress, false))
            {
                StreamUtils.Copy(msIn, gzOut, new byte[buflen]);
            }

            msOut.Flush();
            byte[] zipBytes = msOut.ToByteArray();

            msOut.Close();
            msOut.Dispose();
            msIn.Close();
            msIn.Dispose();

            return zipBytes;
        }


        /// <summary>
        /// GzFile gzips or gunzips a file
        /// </summary>
        /// <param name="infile"><see cref="string">full (unc) filepath to file</see></param>
        /// <param name="zip"><see cref="bool">(bool)true for gzip, (bool)false for gunzip</see></param>
        /// <returns><see cref="string">string name of processed (gzipped/gunzipped) file</see></returns>
        public static string GzFile(string inFile, bool zip = true)
        {
            if (string.IsNullOrEmpty(inFile) || !File.Exists(inFile))
                throw new ArgumentNullException("string GzFile(string inFile, bool zip = true) => inFile is either null or empty or doesn't exist!");

            string outreturn = (zip) ? $"gzip {Path.GetFileName(inFile)} ... " : $"gunzip {Path.GetFileName(inFile)} ... ";
            byte[] inBytes = File.ReadAllBytes(inFile);
            byte[] outBytes = (zip) ? GZipViaStream(inBytes) : GUnZipViaStream(inBytes);
            string outFile = (zip) ? inFile + ".gz" : inFile.EndsWith(".gz") ?
                inFile.Replace(".gz", "") : DateTime.Now.ToString("yy-MM-dd_") + inFile;
            File.WriteAllBytes(outFile, outBytes);

            FileInfo fi = new FileInfo(outFile);
            
            if (fi.Exists)
                outreturn += $"created file {fi.Name} length={fi.Length} at {fi.CreationTime.ToShortDateString()} {fi.CreationTime.ToShortTimeString()}\nin directory {fi.DirectoryName}";
            else outreturn += $"=> file {outFile} NOT created; something went wrong!";
            
            return outreturn;
        }


        /// <summary>
        /// Please use <see cref="GUnZipViaStream(byte[])"/>
        /// </summary>
        /// <param name="inBytes"><see cref="byte[]"/> inBytes</param>
        /// <returns><see cref="byte[]"/> outbytes</returns>
        public static byte[] GUnZip(byte[] inBytes)
        {
            MemoryStream msIn = new MemoryStream();
            msIn.Write(inBytes, 0, inBytes.Length);
            msIn.Flush();
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();

            ICSharpCode.SharpZipLib.GZip.GZip.Decompress(msIn, msOut, false);

            // msOut.Flush();
            // msOut.Seek(0, SeekOrigin.Begin);
            // msOut.Flush();

            byte[] unZipBytes = msOut.ToByteArray();

            msOut.Close();
            msOut.Dispose();
            msIn.Close();
            msIn.Dispose();

            return unZipBytes;
        }

        /// <summary>
        /// GUnZipViaStream
        /// </summary>
        /// <param name="inBytes"><see cref="byte[]"/> inBytes</param>
        /// <returns><see cref="byte[]"/> outbytes</returns>
        public static byte[] GUnZipViaStream(byte[] inBytes)
        {
            int buflen = Math.Max(inBytes.Length * 2, 4096);
            MemoryStream msIn = new MemoryStream();
            msIn.Write(inBytes, 0, inBytes.Length);
            msIn.Flush();
            msIn.Seek(0, SeekOrigin.Begin);

            MemoryStream msOut = new MemoryStream();

            // using (GZipInputStream gzIn = new GZipInputStream(msIn))
            using (GZipStream gzIn = new GZipStream(msIn, CompressionMode.Decompress, false))
            {
                StreamUtils.Copy(gzIn, msOut, new byte[buflen]);
            }

            // msOut.Flush();
            byte[] unZipBytes = msOut.ToByteArray();

            msOut.Close();
            msOut.Dispose();
            msIn.Close();
            msIn.Dispose();

            return unZipBytes;
        }

    }


}
