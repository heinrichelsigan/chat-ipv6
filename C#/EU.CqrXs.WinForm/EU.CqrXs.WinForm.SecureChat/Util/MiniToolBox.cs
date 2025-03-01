using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EU.CqrXs.WinForm.SecureChat.Util
{
    internal static class MiniToolBox
    {

        /// <summary>
        /// Creates Attachment Directory 
        /// </summary>
        internal static void CreateAttachDirectory()
        {

            try
            {
                if (!Directory.Exists(LibPaths.AttachmentFilesDir))
                    Directory.CreateDirectory(LibPaths.AttachmentFilesDir);
            }
            catch (Exception dirEx)
            {
                SLog.Log($"Exeception: {dirEx.GetType()} {dirEx.Message}, when creating attachment directory: {LibPaths.AttachmentFilesDir}\n\t{dirEx}\n");
            }
        }



        internal static string ShowZenMatrixPermutation(string secretKey, string iv = "")
        {
            ZenMatrix z = new ZenMatrix(secretKey, iv, false);
            string zmVisualize = "| 0 | => | ";

            foreach (sbyte sb in z.PermutationKeyHash)
            {
                zmVisualize += sb.ToString("x1") + " ";
            }
            zmVisualize += "| \r\n";
            for (int zeni = 1; zeni < z.PermutationKeyHash.Count; zeni++)
            {
                sbyte sb = (sbyte)z.PermutationKeyHash.ElementAt(zeni);
                zmVisualize += "| " + zeni.ToString("x1") + " | => | " + sb.ToString("x1") + " | " + "\r\n";
            }
            // this.TextBoxDestionation.Text += ZenMatrix.EncryptString(this.richTextBoxChat.Text) + "\n";

            return zmVisualize;
        }

        //internal static byte[] ScrambleBytes(params byte[] bytes)
        //{
        //    foreach ()
        //}
        
    }
}
