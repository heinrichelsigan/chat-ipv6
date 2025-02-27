namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    partial class Form1
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
            ListDragSource = new ListBox();
            ListDragTarget = new ListBox();
            UseCustomCursorsCheck = new CheckBox();
            DropLocationLabel = new Label();
            SuspendLayout();
            // 
            // ListDragSource
            // 
            ListDragSource.Items.AddRange(new object[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" });
            ListDragSource.Location = new Point(12, 12);
            ListDragSource.Name = "ListDragSource";
            ListDragSource.Size = new Size(112, 214);
            ListDragSource.TabIndex = 1;
            ListDragSource.GiveFeedback += ListDragSource_GiveFeedback;
            ListDragSource.QueryContinueDrag += ListDragSource_QueryContinueDrag;
            ListDragSource.MouseDown += ListDragSource_MouseDown;
            ListDragSource.MouseMove += ListDragSource_MouseMove;
            ListDragSource.MouseUp += ListDragSource_MouseUp;
            // 
            // ListDragTarget
            // 
            ListDragTarget.AllowDrop = true;
            ListDragTarget.Location = new Point(156, 12);
            ListDragTarget.Name = "ListDragTarget";
            ListDragTarget.Size = new Size(130, 214);
            ListDragTarget.TabIndex = 2;
            ListDragTarget.DragDrop += ListDragTarget_DragDrop;
            ListDragTarget.DragEnter += ListDragTarget_DragEnter;
            ListDragTarget.DragOver += ListDragTarget_DragOver;
            ListDragTarget.DragLeave += ListDragTarget_DragLeave;
            // 
            // UseCustomCursorsCheck
            // 
            UseCustomCursorsCheck.Location = new Point(10, 243);
            UseCustomCursorsCheck.Name = "UseCustomCursorsCheck";
            UseCustomCursorsCheck.Size = new Size(137, 24);
            UseCustomCursorsCheck.TabIndex = 3;
            UseCustomCursorsCheck.Text = "Use Custom Cursors";
            // 
            // DropLocationLabel
            // 
            DropLocationLabel.Location = new Point(153, 243);
            DropLocationLabel.Name = "DropLocationLabel";
            DropLocationLabel.Size = new Size(143, 19);
            DropLocationLabel.TabIndex = 4;
            DropLocationLabel.Text = "None";
            // 
            // Form1
            // 
            ClientSize = new Size(296, 229);
            Controls.Add(ListDragSource);
            Controls.Add(ListDragTarget);
            Controls.Add(UseCustomCursorsCheck);
            Controls.Add(DropLocationLabel);
            MinimumSize = new Size(312, 268);
            Name = "Form1";
            Text = "drag-and-drop Example";
            FormClosed += DragNDropDemo_FormClosed;
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            DragOver += Form1_DragOver;
            DragLeave += Form1_DragLeave;
            ResumeLayout(false);
        }
        #endregion
        System.Windows.Forms.ListBox ListDragSource;
        System.Windows.Forms.ListBox ListDragTarget;
        System.Windows.Forms.CheckBox UseCustomCursorsCheck;
        System.Windows.Forms.Label DropLocationLabel;
        System.Windows.Forms.Cursor MyNormalCursor;
        System.Windows.Forms.Cursor MyNoDropCursor;
    }

}