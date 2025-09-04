using Area23.At.Framework.Core.Cache;
using Area23.At.Framework.Core.Cqr.Msg;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Entities;
using EU.CqrXs.WinForm.SecureChat.Properties;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
// using static QRCoder.Core.PayloadGenerator.SwissQrCode;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{

    /// <summary>
    /// ContactSettings is Form to Edit <see cref="CqrContact"/>
    /// </summary>
    partial class ContactSettings : Dialog
    {
        private int _id = 1;
        bool firstReg = false;
        private string base64image = string.Empty;
        private System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(ContactSettings));
        public ContactSettings() : base()
        {
            InitializeComponent();
            pictureBoxImage.Image = null;
            if (EU.CqrXs.WinForm.SecureChat.Properties.fr.Resources.Click2UploadBackground != null)
                pictureBoxImage.BackgroundImage = EU.CqrXs.WinForm.SecureChat.Properties.fr.Resources.Click2UploadBackground;
            pictureBoxImage.Tag = Constants.IMAGE_UPLOAD_CLICK + Constants.IMAGE_UPLOAD_EXTENSION;

            using (MemoryStream ms = new MemoryStream(Resources.WinFormAboutDialog))
            {
                this.logoPictureBox.Image = new Bitmap(ms);
            }
            pictureBoxImage.SizeMode = PictureBoxSizeMode.AutoSize;

            if (Settings.Singleton != null)
                this.checkBoxRegister.Checked = Settings.Singleton.RegisterUser;
            firstReg = MemoryCache.CacheDict.GetValue<bool>(Constants.APP_FIRST_REG);
            if (!firstReg)
                this.checkBoxRegister.Enabled = false;

        }

        public ContactSettings(string name, int id = 1) : this()
        {
            this.Text = name;
            _id = id;
            if (Settings.Singleton != null)
            {
                if (_id == 0 && Settings.Singleton.MyContact != null)
                {
                    if (Settings.Singleton.MyContact.ContactId < 0)
                    {
                        _id = 0;
                        Settings.Singleton.MyContact.ContactId = 0;
                        Settings.SaveSettings(Settings.Singleton);
                    }
                    else
                    {
                        this.Text = Settings.Singleton.MyContact.ContactId + " " + Settings.Singleton.MyContact.Name;
                        this.comboBoxName.Text = Settings.Singleton.MyContact.Name;
                        this.textBoxEmail.Text = Settings.Singleton.MyContact.Email;
                        this.textBoxMobile.Text = Settings.Singleton.MyContact.Mobile;
                        this.textBoxAddress.Text = Settings.Singleton.MyContact.Address;
                        base64image = "";
                        if (Entities.Settings.Singleton.MyContact.ContactImage != null &&
                            Entities.Settings.Singleton.MyContact.ContactImage.ImageData != null &&
                            Entities.Settings.Singleton.MyContact.ContactImage.ImageData.Length > 0)
                        {
                            base64image = Convert.ToBase64String(Entities.Settings.Singleton.MyContact.ContactImage.ImageData);
                        }                        
                        if (!string.IsNullOrEmpty(base64image))
                        {
                            this.pictureBoxImage.Image = base64image.Base64ToImage();
                            pictureBoxImage.Tag = Entities.Settings.Singleton.MyContact.ContactImage?.ImageFileName;
                        }
                    }
                }
                else if (_id > 0 && Entities.Settings.Singleton.Contacts.Count > 0)
                {
                    foreach (CContact contact in Entities.Settings.Singleton.Contacts)
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
                    foreach (CContact contact in Entities.Settings.Singleton.Contacts)
                    {
                        if (contact != null && contact.ContactId == currentId && currentId > 0)
                        {
                            contact.Name = this.comboBoxName.Text ?? string.Empty; // 
                            contact.Email = this.textBoxEmail.Text ?? string.Empty; //
                            contact.Mobile = this.textBoxMobile.Text ?? string.Empty; //
                            contact.Address = this.textBoxAddress.Text ?? string.Empty;
                            if (pictureBoxImage.Image != null)
                                contact.ContactImage = CImage.FromDrawingImage(this.pictureBoxImage.Image, pictureBoxImage.Tag.ToString());

                            foundContact = true;
                            break;
                        }
                    }
                    if (!foundContact)
                    {
                        currentId = Entities.Settings.Singleton.Contacts.Count + 1;
                        Entities.Settings.Singleton.Contacts.Add(
                            new CContact()
                            {
                                ContactId = currentId,
                                Name = this.comboBoxName.Text ?? string.Empty,
                                Email = this.textBoxEmail.Text ?? string.Empty,
                                Mobile = this.textBoxMobile.Text ?? string.Empty,
                                Address = this.textBoxAddress.Text ?? string.Empty,
                                ContactImage = CImage.FromDrawingImage(this.pictureBoxImage.Image, pictureBoxImage.Tag.ToString())
                            });
                    }
                }
                Settings.Singleton.RegisterUser = this.checkBoxRegister.Checked;
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
                CImage? imgTest = null;
                try
                {
                    if (pictureBoxImage.Image != null)
                        imgTest = CImage.FromDrawingImage(pictureBoxImage.Image, pictureBoxImage.Tag?.ToString());
                }
                catch (Exception exi)
                {
                    Area23Log.LogOriginMsgEx("ContactSettings", "Form_Closing(...)", exi);                    
                    imgTest = null;
                }
                if (Settings.Singleton.MyContact == null)
                {
                    Settings.Singleton.MyContact = new CContact()
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
                MemoryCache.CacheDict.SetValue<CContact>(Constants.APP_MY_CONTACT, Settings.Singleton.MyContact);                
                Settings.Singleton.RegisterUser = this.checkBoxRegister.Checked;
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
                foreach (CContact contact in Entities.Settings.Singleton.Contacts)
                {
                    if (contact != null && !string.IsNullOrEmpty(contact.Name) && contact.Name.Equals(currentSelectedName))
                    {
                        this.Text = contact.ContactId + " " + contact.Name;
                        this.textBoxEmail.Text = contact.Email ?? string.Empty;
                        this.textBoxMobile.Text = contact.Mobile ?? string.Empty;
                        this.textBoxAddress.Text = contact.Address ?? string.Empty;
                        base64image = Convert.ToBase64String(contact.ContactImage.ImageData);
                        if (!string.IsNullOrEmpty(base64image))
                        {
                            pictureBoxImage.Tag = contact.ContactImage?.ImageFileName;
                            this.pictureBoxImage.Image = base64image.Base64ToImage();
                            this.pictureBoxImage.BackgroundImage = null;
                        }
                        else
                        {
                            pictureBoxImage.Tag = Constants.IMAGE_UPLOAD_CLICK + contact.ContactId + Constants.IMAGE_UPLOAD_EXTENSION;
                            if (Properties.fr.Resources.Click2UploadBackground != null)
                            {
                                pictureBoxImage.Image = null;
                                pictureBoxImage.BackgroundImage = Properties.fr.Resources.Click2UploadBackground;
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
                foreach (CContact contact in Entities.Settings.Singleton.Contacts)
                {
                    if (contact != null && !string.IsNullOrEmpty(contact.Name) && contact.Name.Equals(currentSelectedName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.Text = contact.ContactId + " " + contact.Name;
                        this.comboBoxName.Text = contact.Name;
                        this.textBoxEmail.Text = contact.Email ?? string.Empty;
                        this.textBoxMobile.Text = contact.Mobile ?? string.Empty;
                        this.textBoxAddress.Text = contact.Address ?? string.Empty;
                        base64image = Convert.ToBase64String(contact.ContactImage.ImageData);
                        if (!string.IsNullOrEmpty(base64image))
                        {
                            pictureBoxImage.Tag = contact.ContactImage?.ImageFileName;
                            this.pictureBoxImage.Image = contact.ContactImage?.ToDrawingBitmap();
                            this.pictureBoxImage.BackgroundImage = null;
                        }
                        else
                        {
                            pictureBoxImage.Tag = Constants.IMAGE_UPLOAD_CLICK + contact.ContactId + Constants.IMAGE_UPLOAD_EXTENSION;
                            this.pictureBoxImage.BackgroundImage = Properties.fr.Resources.Click2UploadBackground;
                            this.pictureBoxImage.Image = null;
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
                    CContact? cContact = Entities.Settings.Singleton.Contacts.Where(c => c.ContactId == _id).ToList().FirstOrDefault();
                    fileName = cContact?.Name?.Replace(" ", "");
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
