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
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Cache;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    public partial class TransparentBadge : System.Windows.Forms.Form
    {
        static int i = 0;

        public string TFormType
        {
            get => this.GetType().ToString();
        }

        public TransparentBadge()
        {
            InitializeComponent();
            int? iBadge = MemoryCache.CacheDict.GetValue<int>(Constants.APP_TRANSPARENT_BADGE);
            if (iBadge.HasValue && iBadge.Value != 0)
                i = (int)iBadge.Value;
            else
                MemoryCache.CacheDict.SetValue<int>(Constants.APP_TRANSPARENT_BADGE, i);            
        }

        public TransparentBadge(string text) : this()
        {            
            string name = DateTime.Now.Area23DateTimeWithMillis();
            this.Name = name;
            this.labelTitle.Text = text;
            // notifyIcon.BalloonTipText = text;
            // notifyIcon.BalloonTipTitle = text;
            notifyIcon.Text = text;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            
        }

        public TransparentBadge(string title, string text, MessageBoxIcon icon) : this (text)
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

            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;          
            notifyIcon.Text = text;
            labelTitle.Text = text;            
        }


        public TransparentBadge(string text, Image image) : this(text)
        {
            this.BackgroundImage = image;    
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public TransparentBadge(string title, string text, MessageBoxIcon icon, Image image) : this(text, title, icon)
        {
            this.BackgroundImage = image;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }


        internal void TransparentBadge_Load(object sender, EventArgs e)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(4000);
            int x = this.DesktopLocation.X;
            int y = this.DesktopLocation.Y;
            Font badgeFont = new Font("Lucida Sans Unicode", 10F, FontStyle.Regular);

            System.Timers.Timer timer0 = new System.Timers.Timer { Interval = 800 };
            System.Timers.Timer timerDispose = new System.Timers.Timer { Interval = 4200 };

            timerDispose.Elapsed += (s, en) =>
            {
                this.Invoke(new Action(() =>
                {
                    if (this != null)
                    {
                        try
                        {
                            Close();
                        }
                        catch { }
                        try
                        {
                            this.DestroyHandle();
                        }
                        catch
                        {
                        }
                    }
                }));
                timerDispose.Stop(); // Stop the timer(otherwise keeps on calling)
            };


            timer0.Elapsed += (s, en) =>
            {
                this.Invoke(new Action(() =>
                {
                    y = this.DesktopLocation.Y;
                    for (int j = i; j < 17; j++)
                    {
                        if (i >= 16)
                        {
                            try
                            {
                                Close();
                            }
                            catch { }
                            try
                            {
                                this.DestroyHandle();
                            }
                            catch
                            {
                            }
                            break;
                        }

                         if (i % 4 == 0)
                            badgeFont = new Font("Lucida Sans Unicode", 11F, FontStyle.Regular);
                        if (i % 4 == 1)
                            badgeFont = new Font("Lucida Sans Unicode", 11F, FontStyle.Bold);
                        if (i % 4 == 2)
                            badgeFont = new Font("Lucida Sans Unicode", 11F, FontStyle.Italic);
                        if (i % 4 == 3)
                            badgeFont = new Font("Lucida Sans Unicode", 11F, FontStyle.Underline);

                        this.labelTitle.Font = badgeFont;

                        MemoryCache.CacheDict.SetValue<int>(Constants.APP_TRANSPARENT_BADGE, j);
                        y -= (j - i);
                        this.SetDesktopLocation(x, y);
                        Thread.Sleep(200);
                    }
                }));
                timer0.Stop(); // Stop the timer(otherwise keeps on calling)
            };


            timerDispose.Start(); // Starts the DisposePictureMessage timer after 4sec
            timer0.Start(); // Starts the show autosave timer after 2,5 sec
            

        }

        private void TransparentBadge_Shown(object sender, EventArgs e)
        {
            
        }

    }
}
