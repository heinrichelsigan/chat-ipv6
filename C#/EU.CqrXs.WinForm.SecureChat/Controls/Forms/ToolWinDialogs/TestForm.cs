﻿using Area23.At.Framework.Core.Net.NameService;
using Area23.At.Framework.Core.Net.WebHttp;
using EU.CqrXs.WinForm.SecureChat.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IDataObject_Com = System.Runtime.InteropServices.ComTypes.IDataObject;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Crypt.Hash;
using Area23.At.Framework.Core.Crypt.EnDeCoding;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    public partial class TestForm : Form
    {
        string myServerKey = string.Empty;

        protected internal static DateTime LastExternalTime = DateTime.MinValue;
        protected internal static IPAddress? _externalIPAddress, _externalIPAddressV6, _serverIp;
        public static IPAddress? ExternalIpAddress
        {
            get
            {
                if (_externalIPAddress != null && DateTime.Now.Subtract(LastExternalTime).TotalSeconds < 1800)
                {
                    return _externalIPAddress;
                }

                LastExternalTime = DateTime.Now;
                _externalIPAddress = WebClientRequest.ExternalClientIpFromServer("https://ipv4.cqrxs.eu/cqrsrv/cqrjd/R.aspx");
                return _externalIPAddress;
            }
        }

        public TestForm()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ClientSize = new Size(361, 498);
            if (ConfigurationManager.AppSettings["ServerIPv4"] != null)
            {
                this.textBoxServerIp.Text = (string)ConfigurationManager.AppSettings["ServerIPv4"];
                _serverIp = IPAddress.Parse(textBoxServerIp.Text);
                SLog.Log("ServerIPv4: " + textBoxServerIp.Text);
            }
            if (ConfigurationManager.AppSettings["ServerIPv6"] != null)
            {
                //this.textBoxServerIp6.Text = (string)ConfigurationManager.AppSettings["ServerIPv6"];
                //SLog.Log("ServerIPv6: " + textBoxServerIp6.Text);
            }
            this.textBoxExternalIp.Text = ExternalIpAddress.ToString();

            string? appServerKey = MemoryCache.CacheDict.GetValue<string>(Constants.APP_SERVER_KEY);
            if (!string.IsNullOrEmpty(appServerKey))
                myServerKey = (string)appServerKey;
            else
            {
                myServerKey = ExternalIpAddress.ToString() + Constants.APP_NAME;
                MemoryCache.CacheDict.SetValue<string>(Constants.APP_SERVER_KEY, myServerKey);
            }

            if (!string.IsNullOrEmpty(myServerKey))
            {
                this.textBoxSecKey.Text = myServerKey;
                CqrFacade facade1 = new CqrFacade(myServerKey);
                this.textBoxPipeHash.Text = facade1.PipeString;
            }

        }




        #region Media Methods

        /// <summary>
        /// PlaySoundFromResource - plays a sound embedded in application ressource file
        /// </summary>
        /// <param name="soundName">unique qualified name for sound</param>
        protected static bool PlaySoundFromResource(string soundName)
        {
            bool played = false;
            if (true)
            {
                byte[] soundBytes = (byte[])EU.CqrXs.WinForm.SecureChat.Properties.Resources.ResourceManager.GetObject(soundName);

                if (soundBytes != null && soundBytes.Length > 0)
                {
                    try
                    {
                        // Place the data into a stream
                        using (MemoryStream ms = new MemoryStream(soundBytes))
                        {
                            // Construct the sound player
                            SoundPlayer player = new SoundPlayer(ms);
                            player.Play();
                            played = true;
                        }
                    }
                    catch (Exception exSound)
                    {
                        SLog.Log(exSound);
                        played = false;
                    }
                    //fixed (byte* bufferPtr = &bytes[0])
                    //{
                    //    System.IO.UnmanagedMemoryStream ums = new UnmanagedMemoryStream(bufferPtr, bytes.Length);
                    //    SoundPlayer player = new SoundPlayer(ums);                        
                    //    player.Play();
                    //}
                }
            }

            return played;
        }



        protected virtual async Task<bool> PlaySoundFromResourcesAsync(string soundName)
        {
            return await Task<bool>.Run<bool>(() => (PlaySoundFromResource(soundName)));
        }

        #endregion Media Methods

        private void textBoxSecKey_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBoxSecKey.Text))
            {
                myServerKey = this.textBoxSecKey.Text;
                MemoryCache.CacheDict.SetValue<string>(Constants.APP_SERVER_KEY, myServerKey);
                CqrFacade facade2 = new CqrFacade(myServerKey);
                this.textBoxPipeHash.Text = facade2.PipeString;
            }
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.textBoxSecKey.Text))
                {
                    myServerKey = this.textBoxSecKey.Text;
                    MemoryCache.CacheDict.SetValue<string>(Constants.APP_SERVER_KEY, myServerKey);
                    CqrFacade facade = new CqrFacade(myServerKey);
                    this.textBoxPipeHash.Text = facade.PipeString;

                    CContent cc = new CContent(this.textBoxSource.Text, facade.PipeString, CType.Json, MD5Sum.HashString(this.textBoxSource.Text, ""));

                    if (!this.checkBoxDecrypt.Checked)
                    {
                        string encrypted = cc.EncryptToJson(myServerKey);
                        this.textBoxDestination.Text = encrypted;
                    }
                    else
                    {
                        string decrypted = string.Empty;
                        CContent? content = cc.DecryptFromJson(myServerKey, this.textBoxSource.Text);
                        this.textBoxDestination.Text = content._message;
                    }

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                this.textBoxDestination.Text += ex.ToString();
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxDestination.Text = string.Empty;
            textBoxSource.Text = string.Empty;
        }

        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void buttonSend1stSrvMsg_Click(object sender, EventArgs e)
        {
            CContact myContact = new CContact();
            if (Entities.Settings.Singleton.MyContact != null)
            {
                CqrFacade facade = new CqrFacade(myServerKey);
                _serverIp = IPAddress.Parse(textBoxServerIp.Text);
                myContact = new CContact(Entities.Settings.Singleton.MyContact, facade.PipeString);
                myContact.ContactImage = null;

                this.textBoxSource.Text = JsonConvert.SerializeObject(myContact);

                CContact? responseContact = facade.Test_Send1st_CqrSrvMsg1_Soap(myContact, Area23.At.Framework.Core.Crypt.EnDeCoding.EncodingType.Base64);

                if (responseContact != null)
                    this.textBoxDestination.Text = responseContact.SerializedMsg;

                try
                {
                    this.textBoxSource.Text = JsonConvert.SerializeObject(textBoxDestination);
                }
                catch (Exception exRetMsg)
                {
                    this.textBoxSource.Text = $"Exception {exRetMsg.GetType()} {exRetMsg.Message}\n+{exRetMsg.ToString()}";
                }

            }
        }

        private void button_Base64Enc_Click(object sender, EventArgs e)
        {
            byte[] sourceBytes = System.Text.Encoding.UTF8.GetBytes(textBoxSource.Text);
            this.textBoxDestination.Text = Base64.ToBase64(sourceBytes);
        }

        private void button_Base64Dec_Click(object sender, EventArgs e)
        {
            string source = textBoxSource.Text;
            if (!Base64.IsValidBase64(source, out string error))
            {
                this.textBoxDestination.Text = "Error decoding base64\n" + error;
                return;
            }

            this.textBoxDestination.Text = EnDeCodeHelper.GetStringFromBytesTrimNulls(Base64.FromBase64(source));
        }
    }
}
