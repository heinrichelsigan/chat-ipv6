using Area23.At.Framework.Core.Cqr;
using Area23.At.Framework.Core.Cqr.Srv;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Util;
using Area23.At.Framework.Core.Static;
using EU.CqrXs.WinForm.SecureChat.Controls.Forms.Base;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{

    /// <summary>
    /// MenuChat inherited from <see cref="BaseChatForm"/> is abstraction for all menu items provided by derived classed,
    /// e.g. <see cref="SecureChat"/>, <see cref="RichTextChat"/>, <see cref="Peer2PeerChat"/>
    /// </summary>
    public partial  class BaseMenuForm : BaseChatForm
    {

        protected internal static bool send1stReg = false;
        
        #region Constructor BaseMenuForm BaseMenuForm_Load

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseMenuForm() : base()
        {
            InitializeComponent();
            Load += new System.EventHandler(async (sender, e) => await BaseMenuForm_Load(sender, e));
        }

        /// <summary>
        /// BaseMenuForm_Load first time forces you to enter your contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual async Task BaseMenuForm_Load(object sender, EventArgs e)
        {            

            if (Entities.Settings.LoadSettings() == null || Entities.Settings.Singleton == null || Entities.Settings.Singleton.MyContact == null)
            {
                // var badge = new TransparentBadge($"Error reading Settings from {LibPaths.SystemDirPath + Constants.JSON_SETTINGS_FILE}.");
                // badge.Show();
                MenuContactsItemMyContact_Click(sender, e);
                while (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Email) || string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Name))
                {
                    string notFullReason = string.Empty;
                    if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Name))
                        notFullReason += "Name is missing!" + Environment.NewLine;
                    if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Email))
                        notFullReason += "Email Address is missing!" + Environment.NewLine;
                    // if (string.IsNullOrEmpty(Entities.Settings.Singleton.MyContact.Mobile))
                    //     notFullReason += "Mobile phone is missing!" + Environment.NewLine;
                    MessageBox.Show(notFullReason, "Please fill out your info fully", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    MenuContactsItemMyContact_Click(sender, e);
                }
                send1stReg = true;
            }
        }

        #endregion Constructor BaseMenuForm BaseMenuForm_Load


        #region MenuView

        protected internal virtual void MenuView_ItemLeftRíght_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuView_ItemTopBottom_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuView_Item1View_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        #endregion MenuView

        #region MenuChatCommands

        protected internal virtual void MenuItemSend_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuItemAttach_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuItemRefresh_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        protected internal virtual void MenuItemClear_Click(object sender, EventArgs e) { /* make it abstract l8r again */ ; }

        #endregion MenuChatCommands

        #region MenuContacts

        protected internal virtual void AddContactsToIpContact() { /* make it abstract l8r again */ ; }

        protected internal virtual void AddContactsByRefComboBox(ref System.Windows.Forms.ComboBox contactCombo)
        {
            string ipContact = (contactCombo != null) ? (GetComboBoxText(contactCombo) ?? string.Empty) : string.Empty;
            var items = GetComboBoxItems(contactCombo);
            if (items != null)
                items.Clear();

            foreach (CContact ct in Entities.Settings.Singleton.Contacts)
            {
                if (ct != null && !string.IsNullOrEmpty(ct.NameEmail))
                    AddItemToComboBox(contactCombo, ct.NameEmail);
            }
            SetComboBoxText(contactCombo, ipContact);
        }


        /// <summary>
        /// MenuContactsItemMyContact_Click edits or adds at 1st time starting own contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuContactsItemMyContact_Click(object sender, EventArgs e)
        {
            Bitmap? bmp = null;
            ContactSettings contactSettings = new ContactSettings("My Contact Info", 0);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            if (Settings.Singleton.MyContact != null && Settings.Singleton.MyContact.ContactImage != null &&
                Settings.Singleton.MyContact.ContactImage.ImageData != null &&
                Settings.Singleton.MyContact.ContactImage.ImageData.Length > 0)
            {
                try
                {
                    bmp = Settings.Singleton.MyContact.ContactImage.ToDrawingBitmap();
                    // if (bmp != null)
                    //  this.PictureBoxYou.Image = bmp;                    
                }
                catch (Exception exBmp)
                {
                    CqrException.SetLastException(exBmp);
                }

                Settings.SaveSettings(Settings.Singleton);
            }

            return; // bmp;

        }

        /// <summary>
        /// MenuItemAddContact_Click launches <see cref="ContactSettings"/> to add or edit a contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuItemAddContact_Click(object sender, EventArgs e)
        {
            ContactSettings contactSettings = new ContactSettings("Add Contact Info", 1);
            contactSettings.ShowInTaskbar = true;
            contactSettings.ShowDialog();

            AddContactsToIpContact();
        }


        /// <summary> 
        /// MenuContactsItemView_Click launches <see cref="ContactsView"/> to view contacts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuContactsItemView_Click(object sender, EventArgs e)
        {
            ContactsView cview = new ContactsView();
            cview.ShowDialog();
        }

        /// <summary>
        /// MenuContactstemImport_Click performs an import of existing contacts from csv or vcard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void MenuContactstemImport_Click(object sender, EventArgs e)
        {
            int contactId = Entities.Settings.Singleton.Contacts.Count;
            int contactsImported = 0;
            string cname = string.Empty, cemail = string.Empty, cmobile = string.Empty, cphone = string.Empty, caddress = string.Empty;
            string firstImport = string.Empty;
            string lastImport = string.Empty;

            HashSet<string> exCnames = new HashSet<string>();
            HashSet<string> exCemails = new HashSet<string>();
            foreach (CContact c in Entities.Settings.Singleton.Contacts)
            {
                if (!string.IsNullOrEmpty(c.Name) && !exCnames.Contains(c.Name))
                    exCnames.Add(c.Name);
                if (!string.IsNullOrEmpty(c.Email) && c.Email.IsEmail() && !exCemails.Contains(c.Email))
                    exCemails.Add(c.Email);
                contactId = Math.Max(contactId, c.ContactId);
            }
            contactId++;

            FileOpenDialog = DialogFileOpen;
            FileOpenDialog.Filter = "CSV (*.csv)|*.csv|VCard (*.vcf)|*.vcf"; //|All files (*.*)|*.*";
            DialogResult result = FileOpenDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                if (File.Exists(FileOpenDialog.FileName))
                {
                    string extension = Path.GetExtension(FileOpenDialog.FileName).ToLower();
                    string[] lines = System.IO.File.ReadAllLines(FileOpenDialog.FileName);


                    switch (extension)
                    {
                        case "csv":
                        case ".csv":

                            int csvCnt = 0;
                            List<int> mailfields = new List<int>();
                            List<int> phonefields = new List<int>();
                            List<int> mobilefields = new List<int>();

                            string[] attributes = lines[0].Split(',');
                            foreach (string attribute in attributes)
                            {
                                if (attribute.ToLower().Contains("e-mail") || attribute.ToLower().Contains("email") || attribute.ToLower().Contains("mail"))
                                    mailfields.Add(csvCnt);
                                if (attribute.ToLower().Contains("phone"))
                                    phonefields.Add(csvCnt);
                                if (attribute.ToLower().Contains("mobil"))
                                    mobilefields.Add(csvCnt);
                                csvCnt++;
                            }

                            for (int i = 1; i < lines.Length; i++)
                            {
                                csvCnt = 0;
                                cname = string.Empty; cemail = string.Empty; cphone = string.Empty; cmobile = string.Empty;
                                string[] fields = lines[i].Split(',');
                                for (int j = 0; j < fields.Length; j++)
                                {
                                    if (j == 0 || j == 2)
                                    {
                                        if (!string.IsNullOrEmpty(fields[j]) && !string.IsNullOrWhiteSpace(fields[j]))
                                            cname += fields[j] + " ";
                                    }
                                    if (j == 3 && !string.IsNullOrWhiteSpace(cname) && cname.EndsWith(' '))
                                        cname = cname.TrimEnd(' ');

                                    if (mailfields.Contains(j) && !string.IsNullOrEmpty(fields[j]) && fields[j].IsEmail())
                                    {
                                        if (string.IsNullOrEmpty(cemail))
                                            cemail = fields[j];
                                    }

                                    if (phonefields.Contains(j) && !string.IsNullOrEmpty(fields[j]) && fields[j].IsPhoneOrMobile())
                                    {
                                        if (string.IsNullOrEmpty(cphone))
                                            cphone = fields[j];
                                    }
                                    if (mobilefields.Contains(j) && !string.IsNullOrEmpty(fields[j]) && fields[j].IsPhoneOrMobile())
                                    {
                                        if (string.IsNullOrEmpty(cmobile))
                                            cmobile = fields[j];
                                    }
                                }
                                cmobile = (string.IsNullOrEmpty(cmobile)) ? cphone : cmobile;
                                if (!string.IsNullOrEmpty(cname) && !exCnames.Contains(cname))
                                {
                                    if (!string.IsNullOrEmpty(cemail) && !exCemails.Contains(cemail))
                                    {
                                        CContact contact = new CContact()
                                        {
                                            ContactId = contactId++,
                                            Cuid = Guid.NewGuid(),
                                            Name = cname,
                                            Email = cemail,
                                            Mobile = cmobile
                                        };
                                        Entities.Settings.Singleton.Contacts.Add(contact);
                                        if (string.IsNullOrEmpty(firstImport) && contactsImported == 0)
                                            firstImport = contact.NameEmail;
                                        else if (contactsImported > 0)
                                            lastImport = contact.NameEmail;
                                        contactsImported++;
                                    }
                                }

                            }

                            Entities.Settings.SaveSettings(Entities.Settings.Singleton);
                            break;
                        case "vcf":
                        case ".vcf":

                            int vcfCnt = 0;
                            bool beginEndVcard = false;


                            for (int i = 0; i < lines.Length; i++)
                            {

                                if (lines[i].ToUpper().StartsWith("BEGIN:VCARD"))
                                {
                                    beginEndVcard = true;
                                    cname = string.Empty; cemail = string.Empty; cphone = string.Empty; cmobile = string.Empty; caddress = string.Empty;
                                }


                                if (beginEndVcard)
                                {
                                    string tmpString = string.Empty;
                                    if (lines[i].ToUpper().StartsWith("FN:"))
                                    {
                                        tmpString = lines[i].Substring(3);
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3)
                                            cname = tmpString;
                                    }
                                    if (lines[i].ToUpper().StartsWith("N:") && string.IsNullOrEmpty(cname))
                                    {
                                        tmpString = lines[i].Substring(2).Replace(";", " ").TrimEnd(' ');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3)
                                            cname = tmpString;
                                    }
                                    if (lines[i].ToUpper().Contains("EMAIL") && lines[i].Contains("@") && string.IsNullOrEmpty(cemail))
                                    {
                                        tmpString = lines[i].Substring(lines[i].LastIndexOf(':')).Trim(':');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3 && tmpString.IsEmail())
                                            cemail = tmpString;
                                    }

                                    if (lines[i].ToUpper().Contains("TEL") && lines[i].Contains("CELL") && string.IsNullOrEmpty(cmobile))
                                    {
                                        tmpString = lines[i].Substring(lines[i].LastIndexOf(':')).Trim(':');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3 && tmpString.IsPhoneOrMobile())
                                            cmobile = tmpString;
                                    }
                                    if (lines[i].ToUpper().Contains("TEL") && string.IsNullOrEmpty(cmobile))
                                    {
                                        tmpString = lines[i].Substring(lines[i].LastIndexOf(':')).Trim(':');
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3 && tmpString.IsPhoneOrMobile())
                                            cmobile = tmpString;
                                    }
                                    if (lines[i].ToUpper().Contains("ADR") && string.IsNullOrEmpty(caddress))
                                    {
                                        tmpString = lines[i].Substring(lines[i].IndexOf(':')).Trim(':').Replace(";;;", " ").Replace(";;", " ").Replace(";", " ");
                                        if (!string.IsNullOrEmpty(tmpString) && tmpString.Length > 3)
                                            caddress = tmpString;
                                    }

                                    // TODO Photo add


                                }


                                if (lines[i].ToUpper().StartsWith("END:VCARD"))
                                {
                                    vcfCnt++;
                                    beginEndVcard = false;
                                    if (!string.IsNullOrEmpty(cname) && !exCnames.Contains(cname))
                                    {
                                        if (!string.IsNullOrEmpty(cemail))
                                        {
                                            CContact contact = new CContact() { ContactId = contactId++, Cuid = Guid.NewGuid(), Name = cname, Email = cemail, Mobile = cmobile };
                                            Entities.Settings.Singleton.Contacts.Add(contact);
                                            if (string.IsNullOrEmpty(firstImport) && contactsImported == 0)
                                                firstImport = contact.NameEmail;
                                            else if (contactsImported > 0)
                                                lastImport = contact.NameEmail;
                                            contactsImported++;
                                        }
                                    }
                                }


                            }

                            Entities.Settings.SaveSettings(Entities.Settings.Singleton);

                            break;
                        default:
                            break;

                    }


                    string importedMsg = $"{contactsImported} new contacts imported!";
                    if (!string.IsNullOrEmpty(firstImport))
                        importedMsg += $"\nFirst: {firstImport}";
                    if (!string.IsNullOrEmpty(lastImport))
                        importedMsg += $"\n Last: {lastImport}";
                    MessageBox.Show(importedMsg, $"Contacts import finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion MenuContacts

        #region MenuFile Open Save Click Eventhandler

        protected internal virtual void MenuFileItemOpen_Click(object sender, EventArgs e)
        {
            FileOpenDialog = DialogFileOpen;
            DialogResult res = FileOpenDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                MessageBox.Show($"FileName: {FileOpenDialog.FileName} init directory: {FileOpenDialog.InitialDirectory}", $"{Text} type {FileOpenDialog.GetType()}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected internal virtual void MenuFileItemSave_Click(object sender, EventArgs e)
        {
            SafeFileName();
        }

        protected internal virtual byte[] OpenCryptFileDialog(ref string loadDir)
        {
            FileOpenDialog = DialogFileOpen;
            byte[] fileBytes;
            DialogResult diaOpenRes = FileOpenDialog.ShowDialog();
            if (diaOpenRes == DialogResult.OK || diaOpenRes == DialogResult.Yes)
            {
                if (!string.IsNullOrEmpty(FileOpenDialog.FileName) && File.Exists(FileOpenDialog.FileName))
                {
                    loadDir = Path.GetDirectoryName(FileOpenDialog.FileName) ?? System.AppDomain.CurrentDomain.BaseDirectory;
                    fileBytes = File.ReadAllBytes(FileOpenDialog.FileName);
                    return fileBytes;
                }
            }

            fileBytes = new byte[0];
            return fileBytes;
        }

        protected internal virtual string SafeFileName(string? filePath = "", byte[]? content = null)
        {
            string? saveDir = Environment.GetEnvironmentVariable("TEMP");
            string ext = ".hex";
            string fileName = DateTime.Now.Area23DateTimeWithSeconds() + ext;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileName = System.IO.Path.GetFileName(filePath);
                saveDir = System.IO.Path.GetDirectoryName(filePath);
                ext = System.IO.Path.GetExtension(filePath);
            }

            if (saveDir != null)
            {
                FileSaveDialog = DialogFileSave;
                FileSaveDialog.RestoreDirectory = true;
                FileSaveDialog.DefaultExt = ext;
            }
            FileSaveDialog.FileName = fileName;
            DialogResult diaRes = FileSaveDialog.ShowDialog();
            if (diaRes == DialogResult.OK || diaRes == DialogResult.Yes)
            {
                if (content != null && content.Length > 0)
                    System.IO.File.WriteAllBytes(FileSaveDialog.FileName, content);

                // var badge = new TransparentBadge($"File {fileName} saved to directory {saveDir}.");
                // badge.Show();
            }

            return (FileSaveDialog != null && FileSaveDialog.FileName != null && File.Exists(FileSaveDialog.FileName)) ? FileSaveDialog.FileName : null;
        }

        #endregion MenuFile Open Save Click Eventhandler


    }
}
