using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using EU.CqrXs.WinForm.SecureChat.Entities;

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
            CContact[] contacts;
            this.dataGridContacts.Rows.Clear();
            // DataGridViewRow row = new DataGridViewRow();
            string userCreatorOwner = Environment.UserName;
            if ((contacts = Entities.Settings.Singleton.Contacts.ToArray()) != null)
            {
                contactsCount = contacts.Length;
                foreach (CContact contact in contacts)
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
            HashSet<CContact> cqrContacts = new HashSet<CContact>();
            for (int r = 0; r < dataGridContacts.SelectedRows.Count; r++)
            {
                var cid = dataGridContacts.SelectedRows[r].Cells[0].Value;
                var ccuid = dataGridContacts.SelectedRows[r].Cells[1].Value;
                var cname = dataGridContacts.SelectedRows[r].Cells[2].Value?.ToString();
                var cemail = dataGridContacts.SelectedRows[r].Cells[3].Value?.ToString();
                var cmobile = dataGridContacts.SelectedRows[r].Cells[4].Value?.ToString();
                var caddress = dataGridContacts.SelectedRows[r].Cells[5].Value?.ToString();
                CContact ctc = JsonContacts.FindContactByNameEmail(Entities.Settings.Singleton.Contacts, cname, cemail, cmobile);
                if (ctc != null)
                    cqrContacts.Add(ctc);
            }
            if (cqrContacts.Count > 0) 
                AppDomain.CurrentDomain.SetData(Constants.JSON_CONTACTS_SELECTED, cqrContacts);
        }

        

        private void ComboName_TextUpdate(object sender, EventArgs e)
        {
            dataGridContacts.CurrentCell = null;
            for (int r = 0; r < this.dataGridContacts.Rows.Count; r++)
            {
                if (dataGridContacts.Rows[r] != null)
                    dataGridContacts.Rows[r].Visible = true;

                if (dataGridContacts.Rows[r] != null && dataGridContacts.Rows[r].Cells != null &&
                    !string.IsNullOrEmpty(this.ComboName.Text))
                {
                    if (dataGridContacts.Rows[r].Cells.Count > 3 && dataGridContacts.Rows[r].Cells[2] != null &&
                       dataGridContacts.Rows[r].Cells[2].Value != null && !string.IsNullOrEmpty(ComboName.Text))
                    {
                        string rname = dataGridContacts.Rows[r].Cells[2].Value.ToString();
                        if (!string.IsNullOrEmpty(rname) &&
                            rname.Contains(this.ComboName.Text, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // dataGridContacts.Rows[r].Visible = true;
                            continue;
                        }
                    }
                    dataGridContacts.Rows[r].Visible = false;
                }
            }            
        }

        private void ComboAddress_TextUpdate(object sender, EventArgs e)
        {
            dataGridContacts.CurrentCell = null;
            foreach (DataGridViewRow row in this.dataGridContacts.Rows)
            {
                if (row != null) 
                    row.Visible = true;
                
                if (row != null && row.Cells != null && !string.IsNullOrEmpty(ComboAddress.Text))
                {
                    if (row.Cells.Count > 4 && row.Cells[5] != null && row.Cells[5].Value != null &&
                        !string.IsNullOrEmpty(row.Cells[5].Value.ToString()) && !string.IsNullOrEmpty(ComboAddress.Text))
                    {
                        string raddress = row.Cells[5].Value.ToString();
                        if (!string.IsNullOrEmpty(raddress) &&
                                raddress.Contains(this.ComboAddress.Text, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // row.Visible = true;
                            continue;
                        }
                    }
                    row.Visible = false;
                }
            }
        }

        private void ComboMobil_TextUpdate(object sender, EventArgs e)
        {
            dataGridContacts.CurrentCell = null;
            foreach (DataGridViewRow row in this.dataGridContacts.Rows)
            {
                if (row != null)
                    row.Visible = true;

                if (row != null && row.Cells != null)
                {
                    if (row.Cells.Count > 4 && row.Cells[4] != null && row.Cells[4].Value != null &&
                        !string.IsNullOrEmpty(row.Cells[4].Value.ToString()) && !string.IsNullOrEmpty(ComboMobil.Text))
                    {
                        string rmobile = row.Cells[4].Value.ToString();
                        if (!string.IsNullOrEmpty(rmobile) &&
                                rmobile.Contains(this.ComboMobil.Text, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // row.Visible = true;
                            continue;
                        }
                    }
                    row.Visible = false;
                }
            }            
        }

        private void ComboEmail_TextUpdate(object sender, EventArgs e)
        {
            dataGridContacts.CurrentCell = null;
            foreach (DataGridViewRow row in this.dataGridContacts.Rows)
            {
                if (row != null)
                    row.Visible = true;

                if (row != null && row.Cells != null)
                {
                    if (row.Cells.Count > 3 && row.Cells[3] != null && row.Cells[3].Value != null &&
                        !string.IsNullOrEmpty(row.Cells[3].Value.ToString()) && !string.IsNullOrEmpty(ComboEmail.Text))
                    {
                        string remail = row.Cells[3].Value.ToString();
                        if (!string.IsNullOrEmpty(remail) &&
                                remail.Contains(this.ComboEmail.Text, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // row.Visible = true;
                            continue;
                        }
                    }
                    row.Visible = false;
                }
            }
        }
    }
}
