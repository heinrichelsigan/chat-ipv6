using EU.CqrXs.WinForm.SecureChat.Controls.Forms;
using EU.CqrXs.WinForm.SecureChat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Area23.At.Framework.Core.Util;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    public partial class TransparentBadge : System.Windows.Forms.Form
    {

        public string TFormType
        {
            get => this.GetType().ToString();
        }

        public TransparentBadge()
        {
            InitializeComponent();
        }

        public TransparentBadge(string labelName) : this()
        {            
            string name = DateTime.Now.Area23DateTimeWithMillis();
            this.Name = name;
            this.labelBadge.Text = labelName;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
        }

        public TransparentBadge(string labelName, MessageBoxIcon icon) : this (labelName)
        {
            switch (icon)
            {
                case MessageBoxIcon.None:
                    notifyIcon.BalloonTipIcon = ToolTipIcon.None;
                    break;                
                // case MessageBoxIcon.Exclamation:
                case MessageBoxIcon.Warning:
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Warning; 
                    break;
                // case MessageBoxIcon.Hand:
                // case MessageBoxIcon.Stop:
                case MessageBoxIcon.Error:
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Error; 
                    break;
                // case MessageBoxIcon.Asterisk:
                case MessageBoxIcon.Question:
                case MessageBoxIcon.Information:
                default:
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    break;
            }  
        }


        public TransparentBadge(string labelName, Image image) : this(labelName)
        {
            this.BackgroundImage = image;            
        }


        internal void TransparentBadge_Load(object sender, EventArgs e)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(4000);
            Point pt = this.DesktopLocation;
            Font badgeFont = new Font("Lucida Sans Unicode", 10F, FontStyle.Regular);

            System.Timers.Timer timer = new System.Timers.Timer { Interval = 1000 };
            System.Timers.Timer timerDispose = new System.Timers.Timer { Interval = 3000 };
            timer.Elapsed += (s, en) =>
            {
                this.Invoke(new Action(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (i % 3 == 0)
                            badgeFont = new Font("Lucida Sans Unicode", 10F, FontStyle.Regular);
                        if (i % 3 == 1)
                            badgeFont = new Font("Lucida Sans Unicode", 10F, FontStyle.Bold);
                        if (i % 3 == 2)
                            badgeFont = new Font("Lucida Sans Unicode", 10F, FontStyle.Italic);
                        this.labelBadge.Font = badgeFont;

                        this.SetDesktopLocation(pt.X, pt.Y - (i * 2));
                        Thread.Sleep(200);
                    }
                }));
                timer.Stop(); // Stop the timer(otherwise keeps on calling)
            };

            timerDispose.Elapsed += (s, en) =>
            {
                this.Invoke(new Action(() =>
                {
                    if (this != null)
                    {
                        Close();
                        // Dispose();
                    }
                }));
                timerDispose.Stop(); // Stop the timer(otherwise keeps on calling)
            };

            timer.Start(); // Starts the show autosave timer after 2,5 sec
            timerDispose.Start(); // Starts the DisposePictureMessage timer after 4sec
        }

        private void TransparentBadge_Shown(object sender, EventArgs e)
        {
            
        }

    }
}
