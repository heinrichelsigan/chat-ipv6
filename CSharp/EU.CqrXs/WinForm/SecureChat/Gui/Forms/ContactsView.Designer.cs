using System.Security.Cryptography;

namespace EU.CqrXs.WinForm.SecureChat.Gui.Forms
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
            buttonClose = new Button();
            ContactId = new DataGridViewTextBoxColumn();
            ContactName = new DataGridViewTextBoxColumn();
            ContactEmail = new DataGridViewTextBoxColumn();
            ContactMobile = new DataGridViewTextBoxColumn();
            ContactAddress = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridContacts).BeginInit();
            SuspendLayout();
            // 
            // dataGridContacts
            // 
            dataGridContacts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridContacts.Columns.AddRange(new DataGridViewColumn[] { ContactId, ContactName, ContactEmail, ContactMobile, ContactAddress });
            dataGridContacts.Location = new Point(12, 12);
            dataGridContacts.Margin = new Padding(2);
            dataGridContacts.Name = "dataGridContacts";
            dataGridContacts.Size = new Size(644, 306);
            dataGridContacts.TabIndex = 26;
            // 
            // buttonClose
            // 
            buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonClose.DialogResult = DialogResult.Cancel;
            buttonClose.Location = new Point(568, 324);
            buttonClose.Margin = new Padding(2);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(88, 30);
            buttonClose.TabIndex = 28;
            buttonClose.Text = "&Close";
            buttonClose.Click += ButtonClose_Click;
            // 
            // ContactId
            // 
            ContactId.HeaderText = "ContactId";
            ContactId.MinimumWidth = 16;
            ContactId.Name = "ContactId";
            ContactId.ReadOnly = true;
            ContactId.ToolTipText = "Contact Identifier";
            ContactId.Width = 72;
            // 
            // ContactName
            // 
            ContactName.HeaderText = "Name";
            ContactName.MinimumWidth = 32;
            ContactName.Name = "ContactName";
            ContactName.ReadOnly = true;
            ContactName.Resizable = DataGridViewTriState.True;
            ContactName.ToolTipText = "Contact Name";
            ContactName.Width = 136;
            // 
            // ContactEmail
            // 
            ContactEmail.HeaderText = "E-Mail";
            ContactEmail.MinimumWidth = 32;
            ContactEmail.Name = "ContactEmail";
            ContactEmail.ReadOnly = true;
            ContactEmail.ToolTipText = "Contact E-Mail";
            ContactEmail.Width = 162;
            // 
            // ContactMobile
            // 
            ContactMobile.HeaderText = "Mobile";
            ContactMobile.MinimumWidth = 24;
            ContactMobile.Name = "ContactMobile";
            ContactMobile.ReadOnly = true;
            ContactMobile.Resizable = DataGridViewTriState.True;
            ContactMobile.ToolTipText = "Contact Mobile";
            ContactMobile.Width = 96;
            // 
            // ContactAddress
            // 
            ContactAddress.HeaderText = "ContactAddress";
            ContactAddress.MinimumWidth = 32;
            ContactAddress.Name = "ContactAddress";
            ContactAddress.ReadOnly = true;
            ContactAddress.ToolTipText = "Contact Address";
            ContactAddress.Width = 144;
            // 
            // ContactsView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(673, 360);
            Controls.Add(buttonClose);
            Controls.Add(dataGridContacts);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2, 2, 2, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ContactsView";
            Padding = new Padding(4);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ContactsView";
            TopMost = true;
            TransparencyKey = SystemColors.Control;
            Load += ContactsView_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridContacts).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private DataGridView dataGridContacts;
        private Button buttonClose;
        private DataGridViewTextBoxColumn ContactId;
        private DataGridViewTextBoxColumn ContactName;
        private DataGridViewTextBoxColumn ContactEmail;
        private DataGridViewTextBoxColumn ContactMobile;
        private DataGridViewTextBoxColumn ContactAddress;

    }
}
