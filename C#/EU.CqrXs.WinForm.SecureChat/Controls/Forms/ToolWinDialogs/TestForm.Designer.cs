namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class TestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (MyNormalCursor != null)
                    MyNormalCursor.Dispose();
                MyNormalCursor = null;

                if (MyNoDropCursor != null)
                    MyNoDropCursor.Dispose();
                MyNoDropCursor = null;

                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            checkBoxDecrypt = new CheckBox();
            labelServerIp = new Label();
            labelExternalClientIp = new Label();
            textBoxSecKey = new TextBox();
            labelSecKey = new Label();
            textBoxExternalIp = new TextBox();
            textBoxServerIp = new TextBox();
            labelPipeHash = new Label();
            textBoxSource = new TextBox();
            textBoxPipeHash = new TextBox();
            panelInput = new Panel();
            textBoxServerIp6 = new TextBox();
            labelServerIp6 = new Label();
            panelButtons = new Panel();
            buttonClear = new Button();
            buttonSend1stSrvMsg = new Button();
            buttonSubmit = new Button();
            button_Base64Dec = new Button();
            button_Base64Enc = new Button();
            groupBoxOutPut = new GroupBox();
            textBoxDestination = new TextBox();
            panelInput.SuspendLayout();
            panelButtons.SuspendLayout();
            groupBoxOutPut.SuspendLayout();
            SuspendLayout();
            // 
            // checkBoxDecrypt
            // 
            checkBoxDecrypt.Location = new Point(14, 140);
            checkBoxDecrypt.Margin = new Padding(2);
            checkBoxDecrypt.Name = "checkBoxDecrypt";
            checkBoxDecrypt.Size = new Size(72, 23);
            checkBoxDecrypt.TabIndex = 3;
            checkBoxDecrypt.Text = "Decrypt";
            // 
            // labelServerIp
            // 
            labelServerIp.AutoSize = true;
            labelServerIp.Location = new Point(22, 13);
            labelServerIp.Margin = new Padding(2, 0, 2, 0);
            labelServerIp.Name = "labelServerIp";
            labelServerIp.Size = new Size(55, 15);
            labelServerIp.TabIndex = 5;
            labelServerIp.Text = "Server Ip:";
            // 
            // labelExternalClientIp
            // 
            labelExternalClientIp.AutoSize = true;
            labelExternalClientIp.Location = new Point(12, 73);
            labelExternalClientIp.Margin = new Padding(2, 0, 2, 0);
            labelExternalClientIp.Name = "labelExternalClientIp";
            labelExternalClientIp.Size = new Size(64, 15);
            labelExternalClientIp.TabIndex = 6;
            labelExternalClientIp.Text = "External Ip:";
            // 
            // textBoxSecKey
            // 
            textBoxSecKey.Location = new Point(83, 104);
            textBoxSecKey.Margin = new Padding(2);
            textBoxSecKey.Name = "textBoxSecKey";
            textBoxSecKey.Size = new Size(296, 23);
            textBoxSecKey.TabIndex = 7;
            textBoxSecKey.TextChanged += textBoxSecKey_TextChanged;
            // 
            // labelSecKey
            // 
            labelSecKey.AutoSize = true;
            labelSecKey.Location = new Point(10, 108);
            labelSecKey.Margin = new Padding(2, 0, 2, 0);
            labelSecKey.Name = "labelSecKey";
            labelSecKey.Size = new Size(67, 15);
            labelSecKey.TabIndex = 8;
            labelSecKey.Text = "🔑 Sec-Key:";
            // 
            // textBoxExternalIp
            // 
            textBoxExternalIp.Location = new Point(82, 69);
            textBoxExternalIp.Margin = new Padding(2);
            textBoxExternalIp.Name = "textBoxExternalIp";
            textBoxExternalIp.ReadOnly = true;
            textBoxExternalIp.Size = new Size(297, 23);
            textBoxExternalIp.TabIndex = 9;
            // 
            // textBoxServerIp
            // 
            textBoxServerIp.Location = new Point(82, 11);
            textBoxServerIp.Margin = new Padding(2);
            textBoxServerIp.Name = "textBoxServerIp";
            textBoxServerIp.ReadOnly = true;
            textBoxServerIp.Size = new Size(297, 23);
            textBoxServerIp.TabIndex = 10;
            // 
            // labelPipeHash
            // 
            labelPipeHash.AutoSize = true;
            labelPipeHash.Location = new Point(83, 143);
            labelPipeHash.Margin = new Padding(2, 0, 2, 0);
            labelPipeHash.Name = "labelPipeHash";
            labelPipeHash.Size = new Size(83, 15);
            labelPipeHash.TabIndex = 11;
            labelPipeHash.Text = "🔑pipe #hash: ";
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(14, 171);
            textBoxSource.Multiline = true;
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(365, 92);
            textBoxSource.TabIndex = 12;
            // 
            // textBoxPipeHash
            // 
            textBoxPipeHash.Location = new Point(172, 139);
            textBoxPipeHash.Margin = new Padding(2);
            textBoxPipeHash.Name = "textBoxPipeHash";
            textBoxPipeHash.ReadOnly = true;
            textBoxPipeHash.Size = new Size(207, 23);
            textBoxPipeHash.TabIndex = 13;
            // 
            // panelInput
            // 
            panelInput.BackColor = SystemColors.AppWorkspace;
            panelInput.Controls.Add(textBoxServerIp6);
            panelInput.Controls.Add(labelServerIp6);
            panelInput.Controls.Add(textBoxServerIp);
            panelInput.Controls.Add(textBoxPipeHash);
            panelInput.Controls.Add(checkBoxDecrypt);
            panelInput.Controls.Add(textBoxSource);
            panelInput.Controls.Add(labelServerIp);
            panelInput.Controls.Add(labelPipeHash);
            panelInput.Controls.Add(labelExternalClientIp);
            panelInput.Controls.Add(textBoxSecKey);
            panelInput.Controls.Add(textBoxExternalIp);
            panelInput.Controls.Add(labelSecKey);
            panelInput.Location = new Point(9, 8);
            panelInput.Margin = new Padding(2);
            panelInput.Name = "panelInput";
            panelInput.Size = new Size(394, 273);
            panelInput.TabIndex = 14;
            // 
            // textBoxServerIp6
            // 
            textBoxServerIp6.Location = new Point(82, 39);
            textBoxServerIp6.Margin = new Padding(2);
            textBoxServerIp6.Name = "textBoxServerIp6";
            textBoxServerIp6.ReadOnly = true;
            textBoxServerIp6.Size = new Size(297, 23);
            textBoxServerIp6.TabIndex = 15;
            // 
            // labelServerIp6
            // 
            labelServerIp6.AutoSize = true;
            labelServerIp6.Location = new Point(17, 43);
            labelServerIp6.Margin = new Padding(2, 0, 2, 0);
            labelServerIp6.Name = "labelServerIp6";
            labelServerIp6.Size = new Size(61, 15);
            labelServerIp6.TabIndex = 14;
            labelServerIp6.Text = "Server Ip6:";
            // 
            // panelButtons
            // 
            panelButtons.BackColor = SystemColors.AppWorkspace;
            panelButtons.Controls.Add(buttonClear);
            panelButtons.Controls.Add(buttonSend1stSrvMsg);
            panelButtons.Controls.Add(buttonSubmit);
            panelButtons.Controls.Add(button_Base64Dec);
            panelButtons.Controls.Add(button_Base64Enc);
            panelButtons.Location = new Point(415, 8);
            panelButtons.Margin = new Padding(2);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(85, 483);
            panelButtons.TabIndex = 20;
            // 
            // buttonClear
            // 
            buttonClear.Location = new Point(3, 450);
            buttonClear.Margin = new Padding(2);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(75, 23);
            buttonClear.TabIndex = 25;
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += buttonClear_Click;
            // 
            // buttonSend1stSrvMsg
            // 
            buttonSend1stSrvMsg.Location = new Point(3, 9);
            buttonSend1stSrvMsg.Margin = new Padding(2);
            buttonSend1stSrvMsg.Name = "buttonSend1stSrvMsg";
            buttonSend1stSrvMsg.Size = new Size(75, 23);
            buttonSend1stSrvMsg.TabIndex = 21;
            buttonSend1stSrvMsg.Text = "1st SrvMsg";
            buttonSend1stSrvMsg.UseVisualStyleBackColor = true;
            buttonSend1stSrvMsg.Click += buttonSend1stSrvMsg_Click;
            // 
            // buttonSubmit
            // 
            buttonSubmit.Location = new Point(3, 240);
            buttonSubmit.Margin = new Padding(2);
            buttonSubmit.Name = "buttonSubmit";
            buttonSubmit.Size = new Size(75, 23);
            buttonSubmit.TabIndex = 24;
            buttonSubmit.Text = "Submit";
            buttonSubmit.UseVisualStyleBackColor = true;
            buttonSubmit.Click += buttonSubmit_Click;
            // 
            // button_Base64Dec
            // 
            button_Base64Dec.Location = new Point(3, 205);
            button_Base64Dec.Margin = new Padding(2);
            button_Base64Dec.Name = "button_Base64Dec";
            button_Base64Dec.Size = new Size(75, 23);
            button_Base64Dec.TabIndex = 23;
            button_Base64Dec.Text = "Base64Dec";
            button_Base64Dec.UseVisualStyleBackColor = true;
            button_Base64Dec.Click += button_Base64Dec_Click;
            // 
            // button_Base64Enc
            // 
            button_Base64Enc.Location = new Point(3, 170);
            button_Base64Enc.Margin = new Padding(2);
            button_Base64Enc.Name = "button_Base64Enc";
            button_Base64Enc.Size = new Size(75, 23);
            button_Base64Enc.TabIndex = 22;
            button_Base64Enc.Text = "Base64Enc";
            button_Base64Enc.UseVisualStyleBackColor = true;
            button_Base64Enc.Click += button_Base64Enc_Click;
            // 
            // groupBoxOutPut
            // 
            groupBoxOutPut.BackColor = SystemColors.AppWorkspace;
            groupBoxOutPut.Controls.Add(textBoxDestination);
            groupBoxOutPut.Location = new Point(9, 285);
            groupBoxOutPut.Margin = new Padding(2);
            groupBoxOutPut.Name = "groupBoxOutPut";
            groupBoxOutPut.Padding = new Padding(2);
            groupBoxOutPut.Size = new Size(394, 206);
            groupBoxOutPut.TabIndex = 15;
            groupBoxOutPut.TabStop = false;
            groupBoxOutPut.Text = "Output";
            // 
            // textBoxDestination
            // 
            textBoxDestination.Location = new Point(15, 25);
            textBoxDestination.Margin = new Padding(2);
            textBoxDestination.Multiline = true;
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.ReadOnly = true;
            textBoxDestination.Size = new Size(362, 166);
            textBoxDestination.TabIndex = 16;
            // 
            // TestForm
            // 
            BackColor = SystemColors.ActiveBorder;
            ClientSize = new Size(506, 498);
            Controls.Add(panelButtons);
            Controls.Add(groupBoxOutPut);
            Controls.Add(panelInput);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimumSize = new Size(480, 480);
            Name = "TestForm";
            Text = "En-/Decrypt TestForm";
            FormClosing += TestForm_FormClosing;
            panelInput.ResumeLayout(false);
            panelInput.PerformLayout();
            panelButtons.ResumeLayout(false);
            groupBoxOutPut.ResumeLayout(false);
            groupBoxOutPut.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        System.Windows.Forms.CheckBox checkBoxDecrypt;
        System.Windows.Forms.Cursor MyNormalCursor;
        System.Windows.Forms.Cursor MyNoDropCursor;
        private Label labelServerIp;
        private Label labelExternalClientIp;
        private TextBox textBoxSecKey;
        private Label labelSecKey;
        private TextBox textBoxExternalIp;
        private TextBox textBoxServerIp;
        private Label labelPipeHash;
        private TextBox textBoxSource;
        private TextBox textBoxPipeHash;
        private Panel panelInput;
        private Panel panelButtons;
        private GroupBox groupBoxOutPut;
        private Button buttonSend1stSrvMsg;
        private Button button_Base64Enc;
        private Button button_Base64Dec;
        private Button buttonSubmit;
        private Button buttonClear;
        private TextBox textBoxServerIp6;
        private Label labelServerIp6;
        private TextBox textBoxDestination;
    }

}