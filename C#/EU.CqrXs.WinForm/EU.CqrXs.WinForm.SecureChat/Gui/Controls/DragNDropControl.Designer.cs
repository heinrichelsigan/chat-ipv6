namespace EU.CqrXs.WinForm.SecureChat.Gui.Controls
{
    partial class DragNDropControl
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
            labelInfo = new Label();
            toolTipInfo = new ToolTip(components);
            SuspendLayout();
            // 
            // labelInfo
            // 
            labelInfo.AutoSize = true;
            labelInfo.Dock = DockStyle.Top;
            labelInfo.Font = new Font("Trebuchet MS", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelInfo.Location = new Point(0, 0);
            labelInfo.Margin = new Padding(1, 0, 1, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(154, 18);
            labelInfo.TabIndex = 0;
            labelInfo.Text = ".                                  .";
            // 
            // DragNDropControl
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            Controls.Add(labelInfo);
            Margin = new Padding(1);
            Name = "DragNDropControl";
            DragDrop += Control_DragDrop;
            DragEnter += Control_DragEnter;
            DragLeave += Control_DragLeave;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelInfo;
        private ToolTip toolTipInfo;
    }
}
