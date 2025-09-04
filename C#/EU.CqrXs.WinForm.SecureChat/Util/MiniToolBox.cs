using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Crypt.Cipher.Symmetric;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

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
                Area23Log.LogOriginMsgEx("MiniToolBox", 
                    $"CreateAttachDirectory() Exception, when creating attachment directory: {LibPaths.AttachmentFilesDir}.", 
                    dirEx);
            }
        }

        internal static CContact? FindContactOrCreateByNameEmail(string nameEmail, string chatRoomSession, string pipeText)
        {
            CContact? friendContact = null, tmpContact = null;
            foreach (CContact c in Entities.Settings.Singleton.Contacts)
            {
                if (!string.IsNullOrEmpty(nameEmail) && nameEmail.Length > 3)
                {
                    if (c.NameEmail.Contains(nameEmail, StringComparison.InvariantCultureIgnoreCase) ||
                        c.Email.Contains(nameEmail, StringComparison.CurrentCultureIgnoreCase))
                    {
                        friendContact = new CContact(c, chatRoomSession, pipeText);
                        friendContact.Message = chatRoomSession;
                        
                        break;
                    }
                }
            }

            if (friendContact == null && nameEmail.IsEmail())
            {
                string nameContact = nameEmail;
                if (nameEmail.Contains("@"))
                    nameContact = nameEmail.Substring(0, nameEmail.IndexOf("@")).Replace("@", "").Replace(".", " ");
                if (!Int32.TryParse(DateTime.Now.ToString("yyMMdd"), out int cId))
                    Int32.TryParse(DateTime.Now.ToString("Mdd"), out cId);
                CImage? friendImg = null;
                tmpContact = new CContact(cId, Guid.NewGuid(), nameContact, nameEmail, "", "", friendImg);
                tmpContact.Message = chatRoomSession;
                    
                friendContact = new CContact(tmpContact, chatRoomSession, pipeText);
                
                Settings.Singleton.Contacts.Add(friendContact);
                Settings.SaveSettings();
            }

            return friendContact;
        }


        internal static string ShowZenMatrixPermutation(string secretKey, string iv = "")
        {
            ZenMatrix z = new ZenMatrix(secretKey, iv, true);
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
