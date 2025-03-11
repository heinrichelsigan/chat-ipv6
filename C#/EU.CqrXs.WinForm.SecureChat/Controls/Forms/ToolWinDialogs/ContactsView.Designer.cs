using System.Security.Cryptography;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class ContactsView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected internal System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected internal void InitializeComponent()
        {
            dataGridContacts = new DataGridView();
            ComboCuid = new ComboBox();
            ComboName = new ComboBox();
            ComboEmail = new ComboBox();
            labelSearch = new Label();
            ComboMobil = new ComboBox();
            ComboAddress = new ComboBox();
            ContactId = new DataGridViewTextBoxColumn();
            Cuid = new DataGridViewTextBoxColumn();
            ContactName = new DataGridViewTextBoxColumn();
            ContactEmail = new DataGridViewTextBoxColumn();
            ContactMobile = new DataGridViewTextBoxColumn();
            ContactAddress = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridContacts).BeginInit();
            SuspendLayout();
            // 
            // dataGridContacts
            // 
            dataGridContacts.AllowUserToDeleteRows = false;
            dataGridContacts.AllowUserToResizeColumns = false;
            dataGridContacts.AllowUserToResizeRows = false;
            dataGridContacts.BackgroundColor = SystemColors.Control;
            dataGridContacts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridContacts.Columns.AddRange(new DataGridViewColumn[] { ContactId, Cuid, ContactName, ContactEmail, ContactMobile, ContactAddress });
            dataGridContacts.Dock = DockStyle.Bottom;
            dataGridContacts.Location = new Point(2, 40);
            dataGridContacts.Margin = new Padding(2);
            dataGridContacts.Name = "dataGridContacts";
            dataGridContacts.Size = new Size(959, 430);
            dataGridContacts.TabIndex = 26;
            dataGridContacts.MultiSelectChanged += DataGrid_MultiSelectChange;
            // 
            // ComboCuid
            // 
            ComboCuid.FormattingEnabled = true;
            ComboCuid.Location = new Point(105, 5);
            ComboCuid.Name = "ComboCuid";
            ComboCuid.Size = new Size(106, 24);
            ComboCuid.TabIndex = 27;
            ComboCuid.TextUpdate += CuidCombo_TextUpdate;
            // 
            // ComboName
            // 
            ComboName.FormattingEnabled = true;
            ComboName.Location = new Point(218, 5);
            ComboName.Name = "ComboName";
            ComboName.Size = new Size(176, 24);
            ComboName.TabIndex = 28;
            ComboName.TextUpdate += ComboName_TextUpdate;
            // 
            // ComboEmail
            // 
            ComboEmail.FormattingEnabled = true;
            ComboEmail.Location = new Point(400, 5);
            ComboEmail.Name = "ComboEmail";
            ComboEmail.Size = new Size(206, 24);
            ComboEmail.TabIndex = 29;
            ComboEmail.TextUpdate += ComboEmail_TextUpdate;
            // 
            // labelSearch
            // 
            labelSearch.AutoSize = true;
            labelSearch.Location = new Point(5, 8);
            labelSearch.Name = "labelSearch";
            labelSearch.Size = new Size(54, 17);
            labelSearch.TabIndex = 30;
            labelSearch.Text = "Search";
            // 
            // ComboMobil
            // 
            ComboMobil.FormattingEnabled = true;
            ComboMobil.Location = new Point(615, 5);
            ComboMobil.Name = "ComboMobil";
            ComboMobil.Size = new Size(120, 24);
            ComboMobil.TabIndex = 31;
            ComboMobil.TextUpdate += ComboMobil_TextUpdate;
            // 
            // ComboAddress
            // 
            ComboAddress.FormattingEnabled = true;
            ComboAddress.Location = new Point(744, 5);
            ComboAddress.Name = "ComboAddress";
            ComboAddress.Size = new Size(208, 24);
            ComboAddress.TabIndex = 32;
            ComboAddress.TextUpdate += ComboAddress_TextUpdate;
            // 
            // ContactId
            // 
            ContactId.HeaderText = "Id";
            ContactId.MinimumWidth = 32;
            ContactId.Name = "ContactId";
            ContactId.ReadOnly = true;
            ContactId.ToolTipText = "Contact Identifier";
            ContactId.Width = 60;
            // 
            // Cuid
            // 
            Cuid.HeaderText = "Cuid";
            Cuid.MinimumWidth = 72;
            Cuid.Name = "Cuid";
            Cuid.ReadOnly = true;
            Cuid.Width = 108;
            // 
            // ContactName
            // 
            ContactName.HeaderText = "Name";
            ContactName.MinimumWidth = 96;
            ContactName.Name = "ContactName";
            ContactName.ReadOnly = true;
            ContactName.Resizable = DataGridViewTriState.True;
            ContactName.ToolTipText = "Contact Name";
            ContactName.Width = 184;
            // 
            // ContactEmail
            // 
            ContactEmail.HeaderText = "E-Mail";
            ContactEmail.MinimumWidth = 120;
            ContactEmail.Name = "ContactEmail";
            ContactEmail.ReadOnly = true;
            ContactEmail.ToolTipText = "Contact E-Mail";
            ContactEmail.Width = 216;
            // 
            // ContactMobile
            // 
            ContactMobile.HeaderText = "Mobile";
            ContactMobile.MinimumWidth = 60;
            ContactMobile.Name = "ContactMobile";
            ContactMobile.ReadOnly = true;
            ContactMobile.Resizable = DataGridViewTriState.True;
            ContactMobile.ToolTipText = "Contact Mobile";
            ContactMobile.Width = 128;
            // 
            // ContactAddress
            // 
            ContactAddress.HeaderText = "ContactAddress";
            ContactAddress.MinimumWidth = 96;
            ContactAddress.Name = "ContactAddress";
            ContactAddress.ReadOnly = true;
            ContactAddress.ToolTipText = "Contact Address";
            ContactAddress.Width = 216;
            // 
            // ContactsView
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            ClientSize = new Size(963, 472);
            Controls.Add(ComboAddress);
            Controls.Add(ComboMobil);
            Controls.Add(labelSearch);
            Controls.Add(ComboEmail);
            Controls.Add(ComboName);
            Controls.Add(ComboCuid);
            Controls.Add(dataGridContacts);
            Name = "ContactsView";
            Text = "ContactsView";
            TopMost = true;
            FormClosed += Form_Closed;
            Load += ContactsView_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridContacts).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DataGridView dataGridContacts;
        private ComboBox ComboCuid;
        private ComboBox ComboName;
        private ComboBox ComboEmail;
        private Label labelSearch;
        private ComboBox ComboMobil;
        private ComboBox ComboAddress;
        private DataGridViewTextBoxColumn ContactId;
        private DataGridViewTextBoxColumn Cuid;
        private DataGridViewTextBoxColumn ContactName;
        private DataGridViewTextBoxColumn ContactEmail;
        private DataGridViewTextBoxColumn ContactMobile;
        private DataGridViewTextBoxColumn ContactAddress;
    }
}
