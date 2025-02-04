using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU.CqrXs.Framework.Core.Crypt.CqrJd
{
    public class MimeAttachment
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Base64Mime { get; set; }
        public int ContentLength { get; set; }
        public string Verification {  get; set; }
        public string MimeMsg { get; set; }

        public MimeAttachment()
        {
            FileName = string.Empty;
            MimeType = string.Empty;
            Base64Mime = string.Empty;
            ContentLength = 0;
            Verification = string.Empty;
            MimeMsg = string.Empty;
        }

        public MimeAttachment(string fileName, string mimeType, string base64Mime, string verification)
        {
            FileName = fileName;
            MimeType = mimeType;
            Base64Mime = base64Mime;
            ContentLength = base64Mime.Length;
            Verification = verification;
            MimeMsg = GetMimeMessage();
        }

        public string GetMimeMessage()
        {
            string mimeMsg = $"Content-Type: {MimeType}; name=\"{FileName}\";\n";
            mimeMsg += $"Content-Transfer-Encoding: base64;\n";
            mimeMsg += $"FileName: {FileName};\n";
            mimeMsg += $"Content-Length: {Base64Mime.Length};\n";
            mimeMsg += $"Content-Verification: {Verification};\n";
            mimeMsg += Base64Mime + "\n";
            return mimeMsg;
        }

        public MimeAttachment GetMimeAttachment(string plainAttachment)
        {
            string restString = plainAttachment;

            MimeType = restString.Substring(restString.IndexOf("Content-Type: "));
            MimeType = MimeType.Substring("Content-Type: ".Length, MimeType.IndexOf(";\n"));

            FileName = restString.Substring(restString.IndexOf("FileName: "));
            FileName = FileName.Substring("FileName: ".Length, FileName.IndexOf(";\n"));

            string contentLengthString = restString.Substring(restString.IndexOf("Content-Length: "));
            contentLengthString = contentLengthString.Substring(0, contentLengthString.IndexOf(";"));
            ContentLength = Int32.Parse(contentLengthString);

            restString = restString.Substring(restString.IndexOf("Content-Verification: "));
            Verification = restString.Substring("Content-Verification: ".Length, restString.IndexOf(";\n"));

            Base64Mime = restString.Substring(restString.IndexOf(";\n"));
            Base64Mime = Base64Mime.Substring(0, Base64Mime.LastIndexOf("\n"));

            return (MimeAttachment)this;

        }

        public static MimeAttachment GetBase64Attachment(string plainAttachment)
        {
            string restString = plainAttachment;
            
            string mimeType = restString.Substring(restString.IndexOf("Content-Type: ") + "Content-Type: ".Length);
            mimeType = mimeType.Substring(0, mimeType.IndexOf(";"));

            string fileName = restString.Substring(restString.IndexOf("FileName: ") + "FileName: ".Length);
            fileName = fileName.Substring(0, fileName.IndexOf(";\n"));

            string contentLengthString = restString.Substring(restString.IndexOf("Content-Length: ") + "Content-Length: ".Length);
            contentLengthString = contentLengthString.Substring(0, contentLengthString.IndexOf(";\n"));
            string contentLenString = string.Empty;
            foreach (char ch in contentLengthString.ToCharArray())
            {
                if (Char.IsDigit(ch) || Char.IsNumber(ch) || ch == '.')
                    contentLenString += ch.ToString();
            }
            int contentLen = Int32.Parse(contentLenString);

            restString = restString.Substring(restString.IndexOf("Content-Verification: ") + "Content-Verification: ".Length);
            string verification = restString.Substring(0, restString.IndexOf(";\n"));

            string mimeBase64 = restString.Substring(restString.IndexOf(";\n"));
            mimeBase64 = mimeBase64.Substring(0, mimeBase64.LastIndexOf("\n"));

            MimeAttachment mimeAttach = new MimeAttachment(fileName, mimeType, mimeBase64, verification);
            return mimeAttach;

        }

        public static string GetMimeMessage(string fileName, string mimeType, string base64Mime, string verification)
        {
            string mimeMsg = $"Content-Type: {mimeType}; name=\"{fileName}\";\n";
            mimeMsg += $"Content-Transfer-Encoding: base64;\n";
            mimeMsg += $"FileName: {fileName};\n";
            mimeMsg += $"Content-Length: {base64Mime.Length};\n";
            mimeMsg += $"Content-Verification: {verification};\n";
            mimeMsg += base64Mime + "\n";
            return mimeMsg;
        }

    }

}
