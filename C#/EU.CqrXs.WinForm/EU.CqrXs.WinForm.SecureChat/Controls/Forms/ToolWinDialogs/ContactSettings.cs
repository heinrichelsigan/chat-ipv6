using Area23.At.Framework.Core;
using Area23.At.Framework.Core.CqrXs.CqrMsg;
using Area23.At.Framework.Core.Crypt.EnDeCoding;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Properties;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{

    /// <summary>
    /// ContactSettings is Form to Edit <see cref="CqrContact"/>
    /// </summary>
    partial class ContactSettings : Dialog
    {
        private int _id = 1;
        private string base64image = string.Empty;
        private System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(ContactSettings));
        public ContactSettings() : base()
        {
            InitializeComponent();
            pictureBoxImage.Image = null;
            using (MemoryStream memoryStream = new MemoryStream(Resources.ClickToUpload))
            {                
                pictureBoxImage.BackgroundImage = new Bitmap(memoryStream);
            }
            using (MemoryStream ms = new MemoryStream(Resources.WinFormAboutDialog))
            {
                this.logoPictureBox.Image = new Bitmap(ms);
            }
            pictureBoxImage.SizeMode = PictureBoxSizeMode.AutoSize;

        }

        public ContactSettings(string name, int id = 1) : this()
        {
            this.Text = name;
            _id = id;
            if (Settings.Singleton != null)
            {
                if (_id == 0 && Settings.Singleton.MyContact != null)
                {
                    this.Text = Settings.Singleton.MyContact.ContactId + " " + Settings.Singleton.MyContact.Name;
                    this.comboBoxName.Text = Settings.Singleton.MyContact.Name;
                    this.textBoxEmail.Text = Settings.Singleton.MyContact.Email;
                    this.textBoxMobile.Text = Settings.Singleton.MyContact.Mobile;
                    this.textBoxAddress.Text = Settings.Singleton.MyContact.Address;                    
                    base64image = Entities.Settings.Singleton.MyContact.ContactImage?.ImageBase64 ?? string.Empty;
                    if (!string.IsNullOrEmpty(base64image))
                        this.pictureBoxImage.Image = base64image.Base64ToImage();
                }
                else if (_id > 0 && Entities.Settings.Singleton.Contacts.Count > 0)
                {
                    foreach (CqrContact contact in Entities.Settings.Singleton.Contacts)
                    {
                        if (!string.IsNullOrEmpty(contact.Name))
                            comboBoxName.Items.Add(contact.Name);
                    }
                }
            }
        }



        /// <summary>
        /// Get contact id from title of dialog
        /// </summary>
        /// <param name="title">string title of dialog</param>
        /// <returns>ContactId</returns>
        internal int GetIdFromTitle(string? title)
        {
            int id = -1;
            string? idStr = title ?? string.Empty;
            if (idStr.Contains(" "))
                idStr = idStr.Substring(0, idStr.IndexOf(" ")).TrimEnd();

            if (!Int32.TryParse(idStr, out id))
                id = -1;

            return id;
        }

        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            bool foundContact = false;
            int currentId = GetIdFromTitle(this.Text);

            if (_id > 0)
            {
                string? currentSelectedName = (comboBoxName.Text != null) ? comboBoxName.Text :
                    (comboBoxName.SelectedItem != null) ? comboBoxName.SelectedItem.ToString() : null;
                               
                if (!string.IsNullOrEmpty(currentSelectedName))
                {
                    foreach (CqrContact contact in Entities.Settings.Singleton.Contacts)
                    {
                        if (contact != null && contact.ContactId == currentId && currentId > 0)
                        {
                            contact.Name = this.comboBoxName.Text ?? string.Empty; // 
                            contact.Email = this.textBoxEmail.Text ?? string.Empty; //
                            contact.Mobile = this.textBoxMobile.Text ?? string.Empty; //
                            contact.Address = this.textBoxAddress.Text ?? string.Empty;
                            if (pictureBoxImage.Image != null)    
                                contact.ContactImage = CqrImage.FromDrawingImage(this.pictureBoxImage.Image, pictureBoxImage.Tag.ToString());

                            foundContact = true;
                            break;
                        }
                    }
                    if (!foundContact)
                    {
                        currentId = Entities.Settings.Singleton.Contacts.Count + 1;
                        Entities.Settings.Singleton.Contacts.Add(
                            new CqrContact()
                            {
                                ContactId = currentId,
                                Name = this.comboBoxName.Text ?? string.Empty,
                                Email = this.textBoxEmail.Text ?? string.Empty,
                                Mobile = this.textBoxMobile.Text ?? string.Empty,
                                Address = this.textBoxAddress.Text ?? string.Empty,
                                ContactImage = CqrImage.FromDrawingImage(this.pictureBoxImage.Image, pictureBoxImage.Tag.ToString())
                            }); 
                    }
                }
                Settings.SaveSettings(Entities.Settings.Singleton);
                return;
            }
        }


        /// <summary>
        /// Form_Closing update owner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (_id == 0 && !string.IsNullOrEmpty(this.comboBoxName.Text))
            {
                CqrImage? imgTest = null;
                try
                {
                    if (pictureBoxImage.Image != null)
                        imgTest = CqrImage.FromDrawingImage(pictureBoxImage.Image, pictureBoxImage.Tag?.ToString());
                }
                catch (Exception exi)
                {
                    Area23Log.LogStatic(exi);
                    imgTest = null;
                }
                if (Settings.Singleton.MyContact == null)
                {
                    Settings.Singleton.MyContact = new CqrContact()
                    {
                        ContactId = 0,
                        Name = this.comboBoxName.Text ?? string.Empty,
                        Email = this.textBoxEmail.Text ?? string.Empty,
                        Mobile = this.textBoxMobile.Text ?? string.Empty,
                        Address = this.textBoxAddress.Text ?? string.Empty                         
                    };
                    if (imgTest != null)
                        Settings.Singleton.MyContact.ContactImage = imgTest;
                }
                else
                {
                    if (!string.IsNullOrEmpty(comboBoxName.Text))
                        Settings.Singleton.MyContact.Name = comboBoxName.Text;
                    if (!string.IsNullOrEmpty(textBoxEmail.Text))
                        Settings.Singleton.MyContact.Email = textBoxEmail.Text;
                    if (!string.IsNullOrEmpty(textBoxMobile.Text))
                        Settings.Singleton.MyContact.Mobile = textBoxMobile.Text;
                    if (!string.IsNullOrEmpty(textBoxAddress.Text))
                        Settings.Singleton.MyContact.Address = textBoxAddress.Text;
                    if (imgTest != null)
                        Settings.Singleton.MyContact.ContactImage = imgTest;
                }
                AppDomain.CurrentDomain.SetData(Constants.MY_CONTACT, JsonConvert.SerializeObject(Settings.Singleton.MyContact));
                Settings.SaveSettings(Entities.Settings.Singleton);
            }
        }


        /// <summary>
        /// select combobox Name and display entry to the name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? currentSelectedName = (comboBoxName.SelectedItem != null) ? comboBoxName.SelectedItem.ToString() : null;
            if (!string.IsNullOrEmpty(currentSelectedName))
            {
                foreach (CqrContact contact in Entities.Settings.Singleton.Contacts)
                {
                    if (contact != null && !string.IsNullOrEmpty(contact.Name) && contact.Name.Equals(currentSelectedName))
                    {
                        this.Text = contact.ContactId + " " + contact.Name;
                        this.textBoxEmail.Text = contact.Email ?? string.Empty;
                        this.textBoxMobile.Text = contact.Mobile ?? string.Empty;
                        this.textBoxAddress.Text = contact.Address ?? string.Empty;
                        base64image = contact.ContactImage?.ImageBase64 ?? string.Empty;
                        if (!string.IsNullOrEmpty(base64image))
                        {
                            pictureBoxImage.Tag = contact.ContactImage?.ImageFileName;
                            this.pictureBoxImage.Image = base64image.Base64ToImage();
                        }
                        else
                        {
                            pictureBoxImage.Tag = $"ClickToUpload{contact.ContactId}.png";
                            using (MemoryStream memoryStream = new MemoryStream(Resources.ClickToUpload))
                            {
                                pictureBoxImage.Image = null;
                                pictureBoxImage.BackgroundImage = new Bitmap(memoryStream);
                            }
                        }
                    }
                }

            }
        }

        private void comboBoxName_TextUpdate(object sender, EventArgs e)
        {
            string? currentSelectedName = (comboBoxName.Text != null) ? comboBoxName.Text.ToString() : null;
            if (!string.IsNullOrEmpty(currentSelectedName))
            {
                foreach (CqrContact contact in Entities.Settings.Singleton.Contacts)
                {
                    if (contact != null && !string.IsNullOrEmpty(contact.Name) && contact.Name.Equals(currentSelectedName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.Text = contact.ContactId + " " + contact.Name;
                        this.comboBoxName.Text = contact.Name;                        
                        this.textBoxEmail.Text = contact.Email ?? string.Empty;
                        this.textBoxMobile.Text = contact.Mobile ?? string.Empty;
                        this.textBoxAddress.Text = contact.Address ?? string.Empty;
                        base64image = contact.ContactImage?.ImageBase64 ?? string.Empty;
                        if (!string.IsNullOrEmpty(base64image))
                        {
                            pictureBoxImage.Tag = contact.ContactImage?.ImageFileName;
                            this.pictureBoxImage.Image = contact.ContactImage?.ToDrawingBitmap();
                        }
                        else
                        {
                            pictureBoxImage.Tag = "ClickToUpload.png";
                            using (MemoryStream memoryStream = new MemoryStream(Resources.ClickToUpload))
                            {
                                pictureBoxImage.Image = new Bitmap(memoryStream);
                            }
                        }
                            
                        break;
                    }
                }

            }
        }


        /// <summary>
        /// Upload new Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Clicked(object sender, EventArgs e)
        {
            string fileName = "";
            FileOpenDialog.RestoreDirectory = true;
            FileOpenDialog.AddExtension = false;
            FileOpenDialog.CheckFileExists = true;
            FileOpenDialog.Filter = "Images (*.png, *.jpg, *.gif, *.bmp, *.exif)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|PNG (*.png)|*.png|GIF (*.gif)|*.gif|JPEG (*.jpeg)|*.jpeg|JPG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp";
            // FileOpenDialog.Filter = "Images|*.jpg|*.png|*.gif";
            DialogResult diaRes = FileOpenDialog.ShowDialog();
            if (diaRes == DialogResult.OK)
            {
                if (File.Exists(FileOpenDialog.FileName))
                {
                    CqrContact? cqrContact = Entities.Settings.Singleton.Contacts.Where(c => c.ContactId == _id).ToList().FirstOrDefault();
                    fileName = cqrContact?.Name?.Replace(" ", "");
                    fileName += ((string.IsNullOrEmpty(fileName)) ? _id : "") + Path.GetExtension(FileOpenDialog.FileName);
                    
                    byte[] bitmapBytes = System.IO.File.ReadAllBytes(FileOpenDialog.FileName);
                    base64image = Convert.ToBase64String(bitmapBytes, Base64FormattingOptions.None); 
                    // Base64.Encode(bitmapBytes);
                    Bitmap bmp = new Bitmap(FileOpenDialog.FileName);
                    int h = bmp.Size.Height;
                    int w = bmp.Size.Width;
                    if (h > 150)
                    {
                        float fh = (h / 150);
                        float fw = (w / fh);
                        Bitmap bitmap = ResizeImage(bmp, (int)fw, (int)150);
                        this.pictureBoxImage.Image = bitmap;
                    }
                    else
                        this.pictureBoxImage.Image = bmp;

                    
                    this.pictureBoxImage.Tag = fileName;
                }
            }
        }

        // <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    
    }
}
