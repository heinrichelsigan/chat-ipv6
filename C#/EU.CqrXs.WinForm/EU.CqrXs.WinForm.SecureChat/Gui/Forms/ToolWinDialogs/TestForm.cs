using Area23.At.Framework.Core;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.CqrXs;
using Area23.At.Framework.Core.CqrXs.CqrSrv;
using Area23.At.Framework.Core.Net.NameService;
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

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
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
                Area23Log.LogStatic("ServerIPv4: " + textBoxServerIp.Text);
            }
            if (ConfigurationManager.AppSettings["ServerIPv6"] != null)
            {
                //this.textBoxServerIp6.Text = (string)ConfigurationManager.AppSettings["ServerIPv6"];
                //Area23Log.LogStatic("ServerIPv6: " + textBoxServerIp6.Text);
            }
            this.textBoxExternalIp.Text = ExternalIpAddress.ToString();

            if (AppDomain.CurrentDomain.GetData("ServerKey") != null)
                myServerKey = (string)AppDomain.CurrentDomain.GetData("ServerKey");
            else
            {
                myServerKey = ExternalIpAddress.ToString() + Constants.APP_NAME;
                AppDomain.CurrentDomain.SetData("ServerKey", myServerKey);
            }

            if (!string.IsNullOrEmpty(myServerKey))
            {
                this.textBoxSecKey.Text = myServerKey;
                SrvMsg1 srvMsg1 = new SrvMsg1(myServerKey);
                this.textBoxPipeHash.Text = srvMsg1.PipeString;
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
                        Area23Log.LogStatic(exSound);
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
                AppDomain.CurrentDomain.SetData("ServerKey", myServerKey);
                SrvMsg1 srv1stMsg = new SrvMsg1(myServerKey);
                this.textBoxPipeHash.Text = srv1stMsg.PipeString;
            }
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.textBoxSecKey.Text))
                {
                    myServerKey = this.textBoxSecKey.Text;
                    AppDomain.CurrentDomain.SetData("ServerKey", myServerKey);
                    SrvMsg1 srv1stMsg = new SrvMsg1(myServerKey);
                    this.textBoxPipeHash.Text = srv1stMsg.PipeString;

                    if (!this.checkBoxDecrypt.Checked)
                    {
                        string encrypted = srv1stMsg.CqrBaseMsg(this.textBoxSource.Text);
                        this.textBoxDestination.Text = encrypted;
                    }
                    else
                    {
                        string decrypted = string.Empty;
                        MsgContent content = srv1stMsg.NCqrBaseMsg(this.textBoxSource.Text);
                        this.textBoxDestination.Text = content.Message;
                    }

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                this.textBoxDestination.Text += ex.ToString();
                Area23Log.LogStatic(ex);
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
            CqrContact myContact = new CqrContact();
            if (Entities.Settings.Singleton.MyContact != null)
            {
                SrvMsg1 srvMsg1 = new SrvMsg1(myServerKey);
                _serverIp = IPAddress.Parse(textBoxServerIp.Text);
                myContact = new CqrContact(Entities.Settings.Singleton.MyContact, srvMsg1.PipeString);
                myContact.ContactImage = null;

                this.textBoxSource.Text = JsonConvert.SerializeObject(myContact);
                
                
                string encrypted = srvMsg1.CqrSrvMsg1(myContact, Area23.At.Framework.Core.Crypt.EnDeCoding.EncodingType.Base64);

                this.textBoxDestination.Text = encrypted;

                string response = srvMsg1.Send1st_CqrSrvMsg1_Soap(myContact, _serverIp, Area23.At.Framework.Core.Crypt.EnDeCoding.EncodingType.Base64);
                SrvMsg1 srvRetMsg1 = new SrvMsg1(this.textBoxSecKey.Text);
                
                this.textBoxDestination.Text = response;

                try
                {
                    CqrContact returnedContact = srvRetMsg1.NCqrSrvMsg1(response);
                    this.textBoxSource.Text = JsonConvert.SerializeObject(returnedContact);
                }
                catch (Exception exRetMsg)
                {
                    this.textBoxSource.Text = $"Exception {exRetMsg.GetType()} {exRetMsg.Message}\n+{exRetMsg.ToString()}";
                }
                
            }
        }
    }
}
