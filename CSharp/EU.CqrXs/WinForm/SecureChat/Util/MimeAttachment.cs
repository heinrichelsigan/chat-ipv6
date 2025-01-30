using EU.CqrXs.Framework.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU.CqrXs.WinForm.SecureChat.Util
{
    public class MimeAttachment
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Base64Mime { get; set; }
        public string MimeMsg { get; set; }

        public MimeAttachment()
        {
            FileName = string.Empty;
            MimeType = string.Empty;
            Base64Mime = string.Empty;
            MimeMsg = string.Empty;
        }

        public MimeAttachment(string fileName, string mimeType, string base64Mime)
        {
            FileName = fileName;
            MimeType = mimeType;
            Base64Mime = base64Mime;
            MimeMsg = GetMimeMessage();
        }

        public string GetMimeMessage()
        {
            string mimeMsg = $"Content-Type: {MimeType}; name=\"{FileName}\"\n";
            mimeMsg += $"Content-Transfer-Encoding: base64\nContent-Disposition: attachment; filename=\"{FileName}\"\n";
            mimeMsg += Base64Mime + "\n";
            return mimeMsg;
        }

        public static string GetMimeMessage(string fileName, string mimeType, string base64Mime)
        {
            string mimeMsg = $"Content-Type: {mimeType}; name=\"{fileName}\"\n";
            mimeMsg += $"Content-Transfer-Encoding: base64\nContent-Disposition: attachment; filename=\"{fileName}\"\n";
            mimeMsg += base64Mime + "\n";
            return mimeMsg;
        }

    }
}
