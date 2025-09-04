using System.Windows.Forms;

namespace EU.CqrXs.WinForm.SecureChat.Controls.GroupBoxes
{
    partial class LinkLabelsBox
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LinkLabel8 = new LinkLabel();
            LinkLabel7 = new LinkLabel();
            LinkLabel6 = new LinkLabel();
            LinkLabel5 = new LinkLabel();
            LinkLabel4 = new LinkLabel();
            LinkLabel3 = new LinkLabel();
            LinkLabel2 = new LinkLabel();
            LinkLabel1 = new LinkLabel();            
            SuspendLayout();
            // 
            // LinkLabel8
            // 
            LinkLabel8.AutoSize = true;
            LinkLabel8.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel8.Location = new Point(1, 175);
            LinkLabel8.Margin = new Padding(1, 0, 1, 0);
            LinkLabel8.Name = "LinkLabel8";
            LinkLabel8.Size = new Size(64, 15);
            LinkLabel8.TabIndex = 96;
            LinkLabel8.TabStop = true;
            LinkLabel8.Text = "LinkLabel8";
            LinkLabel8.Visible = false;
            // 
            // LinkLabel7
            // 
            LinkLabel7.AutoSize = true;
            LinkLabel7.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel7.Location = new Point(1, 155);
            LinkLabel7.Margin = new Padding(1, 0, 1, 0);
            LinkLabel7.Name = "LinkLabel7";
            LinkLabel7.Size = new Size(64, 15);
            LinkLabel7.TabIndex = 95;
            LinkLabel7.TabStop = true;
            LinkLabel7.Text = "LinkLabel7";
            LinkLabel7.Visible = false;
            // 
            // LinkLabel6
            // 
            LinkLabel6.AutoSize = true;
            LinkLabel6.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel6.Location = new Point(1, 135);
            LinkLabel6.Margin = new Padding(1, 0, 1, 0);
            LinkLabel6.Name = "LinkLabel6";
            LinkLabel6.Size = new Size(64, 15);
            LinkLabel6.TabIndex = 94;
            LinkLabel6.TabStop = true;
            LinkLabel6.Text = "LinkLabel6";
            LinkLabel6.Visible = false;
            // 
            // LinkLabel5
            // 
            LinkLabel5.AutoSize = true;
            LinkLabel5.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel5.Location = new Point(1, 115);
            LinkLabel5.Margin = new Padding(1, 0, 1, 0);
            LinkLabel5.Name = "LinkLabel5";
            LinkLabel5.Size = new Size(64, 15);
            LinkLabel5.TabIndex = 93;
            LinkLabel5.TabStop = true;
            LinkLabel5.Text = "LinkLabel5";
            LinkLabel5.Visible = false;
            // 
            // LinkLabel4
            // 
            LinkLabel4.AutoSize = true;
            LinkLabel4.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel4.Location = new Point(1, 95);
            LinkLabel4.Margin = new Padding(1, 0, 1, 0);
            LinkLabel4.Name = "LinkLabel4";
            LinkLabel4.Size = new Size(64, 15);
            LinkLabel4.TabIndex = 92;
            LinkLabel4.TabStop = true;
            LinkLabel4.Text = "LinkLabel4";
            LinkLabel4.Visible = false;
            // 
            // LinkLabel3
            // 
            LinkLabel3.AutoSize = true;
            LinkLabel3.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel3.Location = new Point(1, 75);
            LinkLabel3.Margin = new Padding(1, 0, 1, 0);
            LinkLabel3.Name = "LinkLabel3";
            LinkLabel3.Size = new Size(64, 15);
            LinkLabel3.TabIndex = 91;
            LinkLabel3.TabStop = true;
            LinkLabel3.Text = "LinkLabel3";
            LinkLabel3.Visible = false;
            // 
            // LinkLabel2
            // 
            LinkLabel2.AutoSize = true;
            LinkLabel2.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel2.Location = new Point(1, 55);
            LinkLabel2.Margin = new Padding(1, 0, 1, 0);
            LinkLabel2.Name = "LinkLabel2";
            LinkLabel2.Size = new Size(64, 15);
            LinkLabel2.TabIndex = 90;
            LinkLabel2.TabStop = true;
            LinkLabel2.Text = "LinkLabel2";
            LinkLabel2.Visible = false;
            // 
            // LinkLabel1
            // 
            LinkLabel1.AutoSize = true;
            LinkLabel1.Font = new Font("Lucida Sans Unicode", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LinkLabel1.Location = new Point(1, 35);
            LinkLabel1.Margin = new Padding(1, 0, 1, 0);
            LinkLabel1.Name = "LinkLabel1";
            LinkLabel1.Size = new Size(64, 15);
            LinkLabel1.TabIndex = 89;
            LinkLabel1.TabStop = true;
            LinkLabel1.Text = "LinkLabel1";
            LinkLabel1.Visible = false;            
            // 
            // LinkLabels
            //                         
            AllowDrop = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            BackColor = SystemColors.ButtonFace;
            Controls.Add(LinkLabel8);
            Controls.Add(LinkLabel7);
            Controls.Add(LinkLabel6);
            Controls.Add(LinkLabel5);
            Controls.Add(LinkLabel4);
            Controls.Add(LinkLabel3);
            Controls.Add(LinkLabel2);
            Controls.Add(LinkLabel1);
            Font = new Font("Lucida Sans Unicode", 9F);
            Location = new Point(0, 0);
            Margin = new Padding(0);
            Name = "LinkLabels";
            Padding = new Padding(0);
            Size = new Size(152, 192);
            TabIndex = 81;
            TabStop = false;
            Text = "Attachments";
            DragDrop += LinkLabelsBox_DragDrop;
            DragEnter += LinkLabelsBox_DragEnter;
            DragLeave += LinkLabelsBox_DragLeave;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LinkLabel LinkLabel8;
        private LinkLabel LinkLabel7;
        private LinkLabel LinkLabel6;
        private LinkLabel LinkLabel5;
        private LinkLabel LinkLabel4;
        private LinkLabel LinkLabel3;
        private LinkLabel LinkLabel2;
        private LinkLabel LinkLabel1;       
    
    }

}
