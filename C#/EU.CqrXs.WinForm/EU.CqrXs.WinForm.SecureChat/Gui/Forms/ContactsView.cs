using EU.CqrXs.WinForm.SecureChat.Gui.Forms;
using EU.CqrXs.WinForm.SecureChat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using EU.CqrXs.WinForm.SecureChat.Properties;
using System.Reflection.Metadata;
using Area23.At.Framework.Core.CqrXs.CqrMsg;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
{
    partial class ContactsView : System.Windows.Forms.Form
    {

        int contactsCount = 0;

        public ContactsView()
        {
            InitializeComponent();                
        }

        private void ContactsView_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            Refresh_DataGrid(sender, e);
            this.Text = $"ContactView shows {contactsCount} contacts";
        }

        internal void Refresh_DataGrid(object sender, EventArgs e)
        {
            CqrContact[] contacts;
            this.dataGridContacts.Rows.Clear();
            // DataGridViewRow row = new DataGridViewRow();
            string userCreatorOwner = Environment.UserName;            
            if ((contacts = Entities.Settings.Instance.Contacts.ToArray()) != null)
            {
                contactsCount = contacts.Length;
                foreach (CqrContact contact in contacts)
                {
                    this.dataGridContacts.Rows.Add(contact.GetRowParams());
                }
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }


    }
}
