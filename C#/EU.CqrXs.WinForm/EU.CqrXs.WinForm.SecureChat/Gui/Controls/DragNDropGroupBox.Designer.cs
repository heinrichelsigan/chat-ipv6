namespace EU.CqrXs.WinForm.SecureChat.Gui.Controls
{
    partial class DragNDropGroupBox
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
            components = new System.ComponentModel.Container();
            SuspendLayout();
            AllowDrop = true;
            this.BackColor = SystemColors.ControlLightLight;
            this.Font = new Font("Lucida Sans Unicode", 9F);
            this.Location = new Point(0, 0);
            this.Margin = new Padding(0);
            this.Name = "DragNDropGroupBox";
            this.Padding = new Padding(1);
            this.Size = new Size(151, 151);
            this.TabIndex = 81;
            this.TabStop = false;
            this.Text = "Drag and Drop Area";
            DragDrop += GroupBox_DragDrop;
            DragEnter += GroupBox_DragEnter;
            DragLeave += GroupBox_DragLeave;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
