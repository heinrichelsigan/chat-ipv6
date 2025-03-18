using StackExchange.Redis;
using Area23.At.Framework.Library;
using Area23.At.Framework.Library.CqrXs;
using Area23.At.Framework.Library.CqrXs.CqrMsg;
using Area23.At.Framework.Library.CqrXs.CqrSrv;
using Area23.At.Framework.Library.Crypt.EnDeCoding;
using Area23.At.Framework.Library.Net.IpSocket;
using Area23.At.Framework.Library.Static;
using Area23.At.Framework.Library.Util;
using EU.CqrXs.CqrSrv.CqrJd.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Services;
using System.Net;
using Newtonsoft.Json;
using System.Net.NetworkInformation;


namespace EU.CqrXs.CqrSrv.CqrJd
{

    /// <summary>
    /// CqrService offers a simple chat room service with strong encryption
    /// </summary>
    [WebService(Namespace = "https://cqrjd.eu/cqrsrv/cqrjd/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CqrService : Util.BaseWebService
    {

        /// <summary>
        /// Send1StSrvMsg sends first registration message of contact
        /// </summary>
        /// <param name="cryptMsg">with sercerkey encrypted message</param>
        /// <returns>with serverkey encrypted responnse of own contact</returns>
        [WebMethod]
        public string Send1StSrvMsg(string cryptMsg)
        {
            Area23Log.LogStatic($"Send1StSrvMsg(string cryptMsg) called.  cryptMsg.Length = {cryptMsg.Length}.\n");
            InitMethod();

            if (PersistMsgInApplicationState)
                HttpContext.Current.Application["lastmsg"] = cryptMsg;
            if (PersistMsgInAmazonElasticCache)                           
                RedIs.Db.StringSet("lastmsg", cryptMsg);            

            SrvMsg1 srv1stMsg = new SrvMsg1(_serverKey);
            SrvMsg1 srv1stRespMsg = new SrvMsg1(_serverKey);
            
            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    _contact = srv1stMsg.NCqrSrvMsg1(cryptMsg);
                    _decrypted = _contact.ToJson();
                    Area23Log.LogStatic("$Contact decrypted successfully: {_decrypted}\n");
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic($"Exception {ex.GetType()} when decrypting contact: {ex.Message}\n\t{ex.ToString()}\n");
            }

            _responseString = srv1stRespMsg.CqrBaseMsg("", EncodingType.Base64);

            if (!string.IsNullOrEmpty(_decrypted) && _contact != null && !string.IsNullOrEmpty(_contact.NameEmail))
            {
                if (PersistMsgInApplicationState)
                    Application["lastdecrypted"] = _decrypted;
                if (PersistMsgInAmazonElasticCache)
                    RedIs.Db.StringSet("lastdecrypted", _decrypted);

                CqrContact foundCt = AddContact(_contact);
                _responseString = srv1stRespMsg.CqrSrvMsg1(foundCt, EncodingType.Base64);
            }

            Area23Log.LogStatic($"Send1StSrvMsg(string cryptMsg) finished.  _contact.Cuid = {_contact.Cuid}.\n");
            return _responseString;
        }

        /// <summary>
        /// Invites to a chat romm 
        /// with an encrypted <see cref="FullSrvMsg<string>"/>
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/></param>
        /// <returns>encrypted <see cref="FullSrvMsg<string>"/> including chatroom number</returns>
        [WebMethod]             
        public string ChatRoomInvite(string cryptMsg)
        {
            Area23Log.LogStatic($"ChatRoomInvite(string cryptMsg) called.  cryptMsg.Length = {cryptMsg.Length}.\n");
            InitMethod();

            _chatRoomNumber = "";
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg, chatRSrvMsg;

            _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);           // decrypt FullSrvMsg<string>            
                    _contact = AddContact(fullSrvMsg.Sender);                   // add contact from FullSrvMsg<string>   
                    chatRSrvMsg = InviteToChatRoom(fullSrvMsg);                 // generate a FullSrvMsg<string> chatserver message by inviting                           

                    _responseString = srvMsg.CqrSrvMsg<string>(chatRSrvMsg);
                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Area23Log.LogStatic($"ChatRoomInvite(string cryptMsg) finished. ChatRoomNr = {_chatRoomNumber}.\n");
            return _responseString;

        }

        /// <summary>
        /// Polls a chat room for new messages
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled dates</param>
        /// <returns>
        /// encrypted <see cref="FullSrvMsg<string>"/> including chatroom number 
        /// with encrypted clientmsg with clientkey.
        /// Server doesn't know client key and always delivers encrypted encrypted messages
        /// Server can only read and decrypt outer envelope message encrypted with server key
        /// </returns>
        [WebMethod]
        public string ChatRoomPoll(string cryptMsg)
        {
            Area23Log.LogStatic($"ChatRoomPoll(string cryptMsg) called.  cryptMsg.Length = {cryptMsg.Length}.\n");
            InitMethod();

            Dictionary<long, string> dict = new Dictionary<long, string>();
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;

            _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);           // decrypt FullSrvMsg<string>
                    _contact = fullSrvMsg.Sender;
                    _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomNr;

                    FullSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(fullSrvMsg, _chatRoomNumber);
                    isValid = ChatRoomCheckPermission(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                    if (isValid)
                    {
                        dict = GetCachedMessageDict(_chatRoomNumber);

                        List<long> pollKeys = new List<long>();
                        foreach (long ticksLong in dict.Keys)
                        {
                            chatRoomMsg.TicksLong.Add(ticksLong);
                            if (ticksLong > fullSrvMsg.Sender.LastPolled.Ticks)
                                pollKeys.Add(ticksLong);
                        }
                        long polledPtr = -1;
                        if (pollKeys.Count > 0)
                        {
                            polledPtr = pollKeys[0];
                            string firstPollClientMsg = dict[polledPtr];
                            if (string.IsNullOrEmpty(firstPollClientMsg))
                            {
                                if (pollKeys.Count > 1)
                                {
                                    polledPtr = pollKeys[1];
                                    firstPollClientMsg = dict[polledPtr];
                                }
                            }

                            if (fullSrvMsg.TicksLong.Contains(polledPtr) || fullSrvMsg.Sender.TicksLong.Contains(polledPtr))
                                chatRoomMsg.TContent = "";
                            else
                            {
                                chatRoomMsg.TContent = firstPollClientMsg;
                                chatRoomMsg.TicksLong.Add(polledPtr);
                                chatRoomMsg.Sender.TicksLong.Add(polledPtr);
                            }

                            DateTime polledMsgDate = new DateTime(polledPtr);
                            _contact = AddPollDate(_contact, polledMsgDate, false);
                            chatRoomMsg.Sender = AddPollDate(chatRoomMsg.Sender, polledMsgDate, false);                         

                            UpdateContact(chatRoomMsg.Sender);
                            chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, _chatRoomNumber);
                            chatRoomMsg.Sender.LastPolled = polledMsgDate.AddSeconds(5);
                        }

                    }

                    _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Area23Log.LogStatic($"ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finihed. ChatRoomNr =  {_chatRoomNumber}.\n");
            return _responseString;

        }

        /// <summary>
        /// Pushes a new message for chatroom to the server
        /// </summary>
        /// <param name="cryptMsg">encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled dates</param>
        /// <param name="chatRoomMembersCrypted">with client key encrypted message, that is stored in proc of server, but server can't decrypt</param>
        /// <returns>encrypted <see cref="FullSrvMsg<string>"/> with chat room number and last polled date changed to now</returns>
        [WebMethod]
        public string ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted)
        {
            Area23Log.LogStatic($"ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) called. len = {chatRoomMembersCrypted.Length}.\n");
            InitMethod();
            bool isValid = false;
            Dictionary<long, string> dict;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;

            _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                    _contact = fullSrvMsg.Sender;
                    _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomNr;

                    FullSrvMsg<string> chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).LoadJsonChatRoom(fullSrvMsg, _chatRoomNumber);
                    isValid = ChatRoomCheckPermission(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                    if (isValid)
                    {
                        DateTime now = DateTime.Now;

                        dict = GetCachedMessageDict(_chatRoomNumber);

                        dict.Add(now.Ticks, chatRoomMembersCrypted);
                        chatRoomMsg.TicksLong.Add(now.Ticks);
                        chatRoomMsg.Sender.TicksLong.Add(now.Ticks);

                        SetCachedMessageDict(_chatRoomNumber, dict); 

                         _contact = AddPollDate(_contact, now, true);
                        chatRoomMsg.Sender = AddPollDate(chatRoomMsg.Sender, now, true);

                        UpdateContact(_contact);
                        chatRoomMsg = (new JsonChatRoom(_chatRoomNumber)).SaveJsonChatRoom(chatRoomMsg, _chatRoomNumber);
                        chatRoomMsg.Sender.LastPushed = now;
                    
                    }

                    _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Area23Log.LogStatic($"ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted) finished. ChatRoomNr =  {_chatRoomNumber}.\n");
            return _responseString;
        }

        /// <summary>
        /// ChatRoomClose
        /// </summary>
        /// <param name="cryptMsg"></param>
        /// <returns></returns>
        [WebMethod]
        public string ChatRoomClose(string cryptMsg)
        {
            Area23Log.LogStatic($"ChatRoomClose(string cryptMsg) started. cryptMsg.Length =  {cryptMsg.Length}.\n");
            InitMethod();
            bool isValid = false;
            SrvMsg srvMsg = new SrvMsg(_serverKey, _serverKey);
            FullSrvMsg<string> fullSrvMsg;
            List<CqrContact> _invited = new List<CqrContact>();

            _responseString = srvMsg.CqrBaseMsg(Constants.NACK);

            try
            {
                if (!string.IsNullOrEmpty(cryptMsg) && cryptMsg.Length >= 8)
                {
                    fullSrvMsg = srvMsg.NCqrSrvMsg<string>(cryptMsg);
                    _contact = AddContact(fullSrvMsg.Sender);
                    _chatRoomNumber = (!string.IsNullOrEmpty(fullSrvMsg.ChatRoomNr)) ? fullSrvMsg.ChatRoomNr : fullSrvMsg.Sender.ChatRoomNr;
                    JsonChatRoom jsonChatRoom = new JsonChatRoom(_chatRoomNumber);
                    FullSrvMsg<string> chatRoomMsg = JsonChatRoom.LoadChatRoom(fullSrvMsg, _chatRoomNumber);
                    isValid = ChatRoomCheckPermission(fullSrvMsg, chatRoomMsg, _chatRoomNumber);
                    if (isValid)
                    {
                        jsonChatRoom.DeleteJsonChatRoom(_chatRoomNumber);
                    }                   

                    _responseString = srvMsg.CqrSrvMsg<string>(chatRoomMsg);

                }
            }
            catch (Exception ex)
            {
                CqrException.SetLastException(ex);
                Area23Log.LogStatic(ex);
            }

            Area23Log.LogStatic($"ChatRoomClose(string cryptMsg) finished. deleted chat room ChatRoomNr =  {_chatRoomNumber}.\n");

            return _responseString;

        }


        [WebMethod]
        public override string TestService()
        {
            return base.TestService();
        }

        [WebMethod]
        public override string GetIPAddress()
        {
            return base.GetIPAddress();
        }

        [WebMethod]
        public virtual string TestCache()
        {
            return base.TestCache();
        }


    }


}
