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
            for (int r = 0; r < dataGridContacts.SelectedRows.Count; r++)
            {
                var v1 = dataGridContacts.SelectedRows[r].Cells[0].Value;
                var v2 = dataGridContacts.SelectedRows[r].Cells[1].Value;
                var v3 = dataGridContacts.SelectedRows[r].Cells[2].Value;
            }

            this.Close();
            this.Dispose();
        }

        private void DataGrid_MultiSelectChange(object sender, EventArgs e)
        {
            var x = e;
            var o = sender;

        }

        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            for (int r = 0; r < dataGridContacts.SelectedRows.Count; r++)
            {
                var v1 = dataGridContacts.SelectedRows[r].Cells[0].Value;
                var v2 = dataGridContacts.SelectedRows[r].Cells[1].Value;
                var v3 = dataGridContacts.SelectedRows[r].Cells[2].Value;
            }
        }

        private void CuidCombo_TextUpdate(object sender, EventArgs e)
        {
            CqrContact[] contacts;
            this.dataGridContacts.Rows.Clear();
            if ((contacts = Entities.Settings.Singleton.Contacts.ToArray()) != null)
            {
                contactsCount = contacts.Length;
                foreach (CqrContact contact in contacts)
                {
                    if (string.IsNullOrEmpty(this.ComboName.Text) ||
                        contact.Cuid.ToString().Contains(this.ComboName.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.dataGridContacts.Rows.Add(contact.GetRowParams());
                    }
                }
            }
        }

        private void ComboName_TextUpdate(object sender, EventArgs e)
        {
            CqrContact[] contacts;
            this.dataGridContacts.Rows.Clear();
            if ((contacts = Entities.Settings.Singleton.Contacts.ToArray()) != null)
            {
                contactsCount = contacts.Length;
                foreach (CqrContact contact in contacts)
                {
                    if (string.IsNullOrEmpty(this.ComboName.Text) || 
                        contact.Name.Contains(this.ComboName.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.dataGridContacts.Rows.Add(contact.GetRowParams());
                    }
                }
            }
        }

        private void ComboAddress_TextUpdate(object sender, EventArgs e)
        {
            CqrContact[] contacts;
            this.dataGridContacts.Rows.Clear();
            if ((contacts = Entities.Settings.Singleton.Contacts.ToArray()) != null)
            {
                contactsCount = contacts.Length;
                foreach (CqrContact contact in contacts)
                {
                    if (string.IsNullOrEmpty(this.ComboName.Text) ||
                        contact.Address.Contains(this.ComboName.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.dataGridContacts.Rows.Add(contact.GetRowParams());
                    }
                }
            }
        }

        private void ComboMobil_TextUpdate(object sender, EventArgs e)
        {
            CqrContact[] contacts;
            this.dataGridContacts.Rows.Clear();
            if ((contacts = Entities.Settings.Singleton.Contacts.ToArray()) != null)
            {
                contactsCount = contacts.Length;
                foreach (CqrContact contact in contacts)
                {
                    if (string.IsNullOrEmpty(this.ComboMobil.Text) ||
                        contact.Mobile.Contains(this.ComboMobil.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.dataGridContacts.Rows.Add(contact.GetRowParams());
                    }
                }
            }
        }

        private void ComboEmail_TextUpdate(object sender, EventArgs e)
        {
            CqrContact[] contacts;
            this.dataGridContacts.Rows.Clear();
            if ((contacts = Entities.Settings.Singleton.Contacts.ToArray()) != null)
            {
                contactsCount = contacts.Length;
                foreach (CqrContact contact in contacts)
                {
                    if (string.IsNullOrEmpty(this.ComboEmail.Text) ||
                        contact.Email.Contains(this.ComboEmail.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.dataGridContacts.Rows.Add(contact.GetRowParams());
                    }
                }
            }
        }
    }
}
