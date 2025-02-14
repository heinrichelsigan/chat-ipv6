﻿using Area23.At.Framework.Library.Crypt.EnDeCoding;
using Area23.At.Framework.Library.Net.WebHttp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Area23.At.Framework.Library.Crypt.CqrJd
{


    /// <summary>
    /// Provides a secure encrypted message to send to the server or receive from server
    /// </summary>
    [DataContract(Name = "Cqr1stServerMsg")]
    [Description("cqrxs.eu cqr1stservermsg")]
    public class Cqr1stServerMsg : CqrBaseMsg
    {


        /// <summary>
        /// Cqr1stServerMsg constructor with srvKey
        /// </summary>
        /// <param name="srvKey">server key (normally client ip + secret)</param>
        /// <exception cref="ArgumentNullException">thrown, when srvKey is null or <see cref="string.Empty"/></exception>
        public Cqr1stServerMsg(string srvKey = "") : base(srvKey) { }



        /// <summary>
        /// Cqr1stSrvMsg encrypts a msg 
        /// </summary>
        /// <param name="msg">plain text string</param>
        /// <param name="encType"><see cref="EncodingType"/></param>
        /// <returns>encrypted msg via <see cref="SymmCipherPipe"/></returns>
        /// <exception cref="InvalidDataException"></exception>
        public string Cqr1stSrvMsg(CqrContact myContact, EncodingType encType)
        {
            string msg = JsonConvert.SerializeObject(myContact);
            return CqrMsg(msg, encType);
        }


        /// <summary>
        /// Cqr1stSrvMsgImage encrypts the picture of user as attchment
        /// </summary>
        /// <param name="myImage"><see cref="CqrImage"/></param>        
        /// <param name="attachment">out <see cref="MimeAttachment"/></param>
        /// <param name="encType"><see cref="EncodingType"/></param>
        /// <returns>encrypted attachment msg via <see cref="SymmCipherPipe"/></returns>
        public string Cqr1stSrvMsgImage(CqrContact myContact, HashSet<CqrContact> contacts, EncodingType encType = EncodingType.Base64)
        {
            CqrImage myImage = myContact.ContactImage;
            string md5 = Hash.MD5Sum.Hash(myImage.ImageData, myImage.ImageFileName);
            string sha256 = Hash.Sha256Sum.Hash(myImage.ImageData, myImage.ImageFileName);
            MimeAttachment mimeAttachment = new MimeAttachment(myImage.ImageFileName, myImage.ImageMimeType, myImage.ImageBase64, "", md5, sha256);
            MimeAttachment attachment; 
            return CqrMsgAttachment(myImage.ImageFileName, myImage.ImageMimeType, myImage.ImageBase64, out attachment, encType, md5, sha256);
        }

        /// <summary>
        /// Cqr1stSrvMsgImg encrypts the picture of user as attchment
        /// </summary>
        /// <param name="myImage"><see cref="CqrImage"/></param>        
        /// <param name="attachment">out <see cref="MimeAttachment"/></param>
        /// <param name="encType"><see cref="EncodingType"/></param>
        /// <returns>encrypted attachment msg via <see cref="SymmCipherPipe"/></returns>
        public string Cqr1stSrvMsgImg(CqrImage myImage, out MimeAttachment attachment, EncodingType encType = EncodingType.Base64)
        {
            string md5 = Hash.MD5Sum.Hash(myImage.ImageData, myImage.ImageFileName);
            string sha256 = Hash.Sha256Sum.Hash(myImage.ImageData, myImage.ImageFileName);
            MimeAttachment mimeAttachment = new MimeAttachment(myImage.ImageFileName, myImage.ImageMimeType, myImage.ImageBase64, "", md5, sha256);
            return CqrMsgAttachment(myImage.ImageFileName, myImage.ImageMimeType, myImage.ImageBase64, out attachment, encType, md5, sha256);
        }


        /// <summary>
        /// NCqr1stSrvMsg decryptes an secure encrypted msg 
        /// </summary>
        /// <param name="cqrMessage">secure encrypted msg </param>
        /// <param name="encType"><see cref="EncodingType"/></param>
        /// <returns><see cref="CqrContact"/>CqrContact decrypted string</returns>
        /// <exception cref="InvalidOperationException">will be thrown, 
        /// if server and client or both side use a different secret key 4 encryption</exception>
        public CqrContact NCqr1stSrvMsg(string cqrMessage, EncodingType encType = EncodingType.Base64)
        {
            CqrContact myContact = null;
            MsgContent msgContent = base.NCqrMsg(cqrMessage, encType);
            if (msgContent != null && !string.IsNullOrEmpty(msgContent.Message))
                myContact = JsonConvert.DeserializeObject<CqrContact>(msgContent.Message);
            
            return myContact;
        }


        /// <summary>
        /// Send1stCqrSrvMsg sends registration msg to server
        /// </summary>
        /// <param name="myContact"><see cref="CqrContact"/></param>
        /// <param name="srvIp">public availible server ip address</param>
        /// <param name="encodingType"><+
        /// see cref="EncodingType"/></param>
        /// <returns></returns>
        public string Send1stCqrSrvMsg(CqrContact myContact, IPAddress srvIp, EncodingType encodingType = EncodingType.Base64)
        {
            string msg = JsonConvert.SerializeObject(myContact);
            string encMsg = CqrMsg(msg, encodingType);
            string encrypted = String.Format("TextBoxEncrypted={0}\r\nTextBoxDecrypted=\r\nTextBoxLastMsg=\r\nButtonSubmit=Submit",
                encMsg);

            string posturl = ConfigurationManager.AppSettings["ServerUrlToPost"].ToString();
            string hostheader = ConfigurationManager.AppSettings["SendHostHeader"].ToString();

            string response = WebClientRequest.PostMessage(encrypted, posturl, hostheader, srvIp.ToString());

            return response;
        }



        /// <summary>
        /// Send1stCqrSrvAttachment sends an attached base64 encoded file
        /// </summary>
        /// <param name="myImage"><see cref="CqrImage"/>
        /// <param name="srvIp">public availible server ip address</param>
        /// <param name="encodingType"><see cref="EncodingType"/></param>
        /// <returns>response string</returns>
        public string Send1stCqrSrvImage(CqrImage myImage, IPAddress srvIp, out MimeAttachment mimeAttachment, EncodingType encodingType = EncodingType.Base64)
        {
            string md5 = Hash.MD5Sum.Hash(myImage.ImageData, myImage.ImageFileName);
            string sha256 = Hash.Sha256Sum.Hash(myImage.ImageData, myImage.ImageFileName);
            MimeAttachment attachment = new MimeAttachment(myImage.ImageFileName, myImage.ImageMimeType, myImage.ImageBase64, "", md5, sha256);
            CqrMsgAttachment(myImage.ImageFileName, myImage.ImageMimeType, myImage.ImageBase64, out mimeAttachment, encodingType, md5, sha256);

            string encryptedAttach = Cqr1stSrvMsgImg(myImage, out mimeAttachment, encodingType);
            string encrypted = String.Format("TextBoxEncrypted={0}\r\nTextBoxDecrypted=\r\nTextBoxLastMsg=\r\nButtonSubmit=Submit",
                encryptedAttach);

            string posturl = ConfigurationManager.AppSettings["ServerUrlToPost"].ToString();
            string hostheader = ConfigurationManager.AppSettings["SendHostHeader"].ToString();

            string response = WebClientRequest.PostMessage(encrypted, posturl, hostheader, srvIp.ToString());

            return response;
        }

    }

}
