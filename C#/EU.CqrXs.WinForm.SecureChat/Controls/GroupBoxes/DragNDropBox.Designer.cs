namespace EU.CqrXs.WinForm.SecureChat.Controls.GroupBoxes
{
    partial class DragNDropBox
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
            SuspendLayout();
            AllowDrop = true;
            this.DragEnter += DragNDropBox_DragEnter;
            this.DragOver += DragNDropBox_DragOver;
            this.DragDrop += DragNDropBox_DragDrop;
            this.DragLeave += DragNDropBox_DragLeave;
            this.BackColor = SystemColors.ControlLightLight;
            this.Font = new Font("Lucida Sans Unicode", 8.5F);
            this.Location = new Point(0, 0);
            this.Margin = new Padding(1);
            this.Name = "DragNDropBox";
            this.Padding = new Padding(1);
            this.Size = new Size(144, 112);
            this.TabIndex = 81;
            this.TabStop = false;
            this.Text = "Drag and Drop Area";            
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

    }
}
