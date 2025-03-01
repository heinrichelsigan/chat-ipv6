using Area23.At.Framework.Core.CqrXs.CqrMsg;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{

    /// <summary>
    /// ContactsView views <see cref="CqrContact"/>
    /// </summary>
    partial class ContactsView : Dialog
    {

        int contactsCount = 0;

        public ContactsView() : base()
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
            if ((contacts = Entities.Settings.Singleton.Contacts.ToArray()) != null)
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
