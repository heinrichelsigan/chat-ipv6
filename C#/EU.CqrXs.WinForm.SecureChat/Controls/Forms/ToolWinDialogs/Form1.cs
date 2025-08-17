using Area23.At.Framework.Core.Net.NameService;
using Area23.At.Framework.Core.Static;
using Area23.At.Framework.Core.Util;
using EU.CqrXs.WinForm.SecureChat.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IDataObject_Com = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace EU.CqrXs.WinForm.SecureChat.Controls.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int ListItemsCount;
        private int indexOfItemUnderMouseToDrag;
        private int indexOfItemUnderMouseToDrop;
        Size DragSize = new Size(9, 0);
        Rectangle DragBoxFromMouseDown = new Rectangle();
        System.Drawing.Point ScreenOffset = new System.Drawing.Point(0, 0);

        protected override void OnLoad(EventArgs e)
        {            
            base.OnLoad(e);
            this.ClientSize = new System.Drawing.Size(300, 268);
        }


        internal void ListDragSource_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            indexOfItemUnderMouseToDrag = ListDragSource.IndexFromPoint(e.X, e.Y);

            if (indexOfItemUnderMouseToDrag != ListBox.NoMatches)
            {

                // Remember the point where the mouse down occurred. The DragSize indicates
                // the size that the mouse can move before a drag event should be started.                
                DragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                DragBoxFromMouseDown = new Rectangle(new Point(e.X - (DragSize.Width / 2),
                                                               e.Y - (DragSize.Height / 2)), DragSize);
                PlaySoundFromResource("sound_laser");
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                DragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        internal void ListDragSource_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Reset the drag rectangle when the mouse button is raised.
            DragBoxFromMouseDown = Rectangle.Empty;
        }

        internal void ListDragSource_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (DragBoxFromMouseDown != Rectangle.Empty &&
                    !DragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Create custom cursornieightnes for the drag-and-drop operation.
                    try
                    {
                        byte[] icot = EU.CqrXs.WinForm.SecureChat.Properties.de.Resources.TransparentFormsIcon;
                        MemoryStream ms = new MemoryStream(icot);
                        Icon icon = new Icon(ms);
                        MyNoDropCursor = new Cursor(icon.Handle);
                        Bitmap bmp = EU.CqrXs.WinForm.SecureChat.Properties.de.Resources.CableWireCut;
                        MyNormalCursor = new Cursor(bmp.GetHicon());
                    }
                    catch
                    {
                        // An error occurred while attempting to load the cursors, so use
                        // standard cursors.
                        UseCustomCursorsCheck.Checked = false;
                    }
                    finally
                    {

                        // The screenOffset is used to account for any desktop bands 
                        // that may be at the top or left side of the screen when 
                        // determining when to cancel the drag drop operation.
                        ScreenOffset = SystemInformation.WorkingArea.Location;

                        // Proceed with the drag-and-drop, passing in the list item.                    
                        DragDropEffects dropEffect = ListDragSource.DoDragDrop(ListDragSource.Items[indexOfItemUnderMouseToDrag], DragDropEffects.All | DragDropEffects.Link);

                        // If the drag operation was a move then remove the item.
                        if (dropEffect == DragDropEffects.Move)
                        {
                            ListDragSource.Items.RemoveAt(indexOfItemUnderMouseToDrag);

                            // Selects the previous item in the list as long as the list has an item.
                            if (indexOfItemUnderMouseToDrag > 0)
                                ListDragSource.SelectedIndex = indexOfItemUnderMouseToDrag - 1;

                            else if (ListDragSource.Items.Count > 0)
                                // Selects the first item.
                                ListDragSource.SelectedIndex = 0;
                        }

                        // Dispose of the cursors since they are no longer needed.
                        if (MyNormalCursor != null)
                            MyNormalCursor.Dispose();

                        if (MyNoDropCursor != null)
                            MyNoDropCursor.Dispose();
                    }
                }
            }
        }

        internal void ListDragSource_GiveFeedback(object sender, System.Windows.Forms.GiveFeedbackEventArgs e)
        {
            // Use custom cursors if the check box is checked.
            if (UseCustomCursorsCheck.Checked)
            {

                // Sets the custom cursor based upon the effect.
                e.UseDefaultCursors = false;
                if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
                    Cursor.Current = MyNormalCursor;
                else
                    Cursor.Current = MyNoDropCursor;
            }
        }


        internal void ListDragTarget_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Determine whether string data exists in the drop data. If not, then
            // the drop effect reflects that the drop cannot occur.
            if (!e.Data.GetDataPresent(typeof(System.String)))
            {

                e.Effect = DragDropEffects.None;
                DropLocationLabel.Text = "None - no string data.";
                return;
            }

            // Set the effect based upon the KeyState.
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTL + ALT

                // Link drag-and-drop effect.
                e.Effect = DragDropEffects.Link;
            }
            else if ((e.KeyState & 32) == 32 &&
              (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {

                // ALT KeyState for link.
                e.Effect = DragDropEffects.Link;
            }
            else if ((e.KeyState & 4) == 4 &&
              (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {

                // SHIFT KeyState for move.
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.KeyState & 8) == 8 &&
              (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {

                // CTL KeyState for copy.
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {

                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

            // Get the index of the item the mouse is below. 

            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.

            indexOfItemUnderMouseToDrop =
                ListDragTarget.IndexFromPoint(ListDragTarget.PointToClient(new Point(e.X, e.Y)));

            // Updates the label text.
            if (indexOfItemUnderMouseToDrop != ListBox.NoMatches)
            {

                DropLocationLabel.Text = "Drops before item #" + (indexOfItemUnderMouseToDrop + 1);
            }
            else
            {
                DropLocationLabel.Text = "Drops at the end.";
            }
        }

        internal void ListDragTarget_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Ensure that the list item index is contained in the data.
            if (e.Data.GetDataPresent(typeof(System.String)))
            {

                Object item = (object)e.Data.GetData(typeof(System.String));

                // Perform drag-and-drop, depending upon the effect.
                if (e.Effect == DragDropEffects.Copy ||
                    e.Effect == DragDropEffects.Move)
                {

                    // Insert the item.
                    if (indexOfItemUnderMouseToDrop != ListBox.NoMatches)
                        ListDragTarget.Items.Insert(indexOfItemUnderMouseToDrop, item);
                    else
                        ListDragTarget.Items.Add(item);

                    PlaySoundFromResource("sound_push");

                    if (ListDragTarget.Items.Count >= ListItemsCount)
                    {
                        PlaySoundFromResource("sound_hammer");
                    }
                }
            }
            // Reset the label text.
            DropLocationLabel.Text = "None";
        }

        internal void ListDragSource_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            // Cancel the drag if the mouse moves off the form.
            if (sender is ListBox lb)
            {

                Form f = lb.FindForm();

                // Cancel the drag if the mouse moves off the form. The screenOffset
                // takes into account any desktop bands that may be at the top or left
                // side of the screen.
                if (((Control.MousePosition.X - ScreenOffset.X) < f.DesktopBounds.Left) ||
                    ((Control.MousePosition.X - ScreenOffset.X) > f.DesktopBounds.Right) ||
                    ((Control.MousePosition.Y - ScreenOffset.Y) < f.DesktopBounds.Top) ||
                    ((Control.MousePosition.Y - ScreenOffset.Y) > f.DesktopBounds.Bottom))
                {

                    PlaySoundFromResource("sound_warning");
                    // e.Action = DragAction.Cancel;
                }
            }
        }

        internal void ListDragTarget_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Reset the label text.
            DropLocationLabel.Text = "ListDragTarget_DragEnter";
        }

        internal void ListDragTarget_DragLeave(object sender, System.EventArgs e)
        {
            // Reset the label text.
            DropLocationLabel.Text = "ListDragTarget_DragLeave";
        }



        internal void DragNDropDemo_FormClosed(object sender, FormClosedEventArgs e)
        {
            PlaySoundFromResource("glasses");
            Application.Exit();
        }

        private IDropTargetHelper ddHelper = (IDropTargetHelper) new DragDropHelper();

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            Point p = Cursor.Position;
            Win32Point wp;
            wp.x = p.X;
            wp.y = p.Y;
            
            ddHelper.Drop(e.Data as IDataObject_Com, ref wp, (int)e.Effect);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            Point p = Cursor.Position;
            Win32Point wp;
            wp.x = p.X;
            wp.y = p.Y;

            ddHelper.DragEnter(this.Handle, e.Data as IDataObject_Com, ref wp, (int)e.Effect);
        }

        private void Form1_DragLeave(object sender, EventArgs e)
        {
            ddHelper.DragLeave();
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            Point p = Cursor.Position;
            Win32Point wp;
            wp.x = p.X;
            wp.y = p.Y;

            ddHelper.DragOver(ref wp, (int)e.Effect);
        }

        #region Media Methods

        /// <summary>
        /// PlaySoundFromResource - plays a sound embedded in application ressource file
        /// </summary>
        /// <param name="soundName">unique qualified name for sound</param>
        protected static bool PlaySoundFromResource(string soundName)
        {
            bool played = false;
            if (true)
            {
                byte[] soundBytes = (byte[])EU.CqrXs.WinForm.SecureChat.Properties.Resources.ResourceManager.GetObject(soundName);

                if (soundBytes != null && soundBytes.Length > 0)
                {
                    try
                    {
                        // Place the data into a stream
                        using (MemoryStream ms = new MemoryStream(soundBytes))
                        {
                            // Construct the sound player
                            SoundPlayer player = new SoundPlayer(ms);
                            player.Play();
                            played = true;
                        }
                    }
                    catch (Exception exSound)
                    {
                        Area23Log.LogOriginMsgEx("Form1", $"PlaySoundFromResource(string soundName = {soundName})", exSound);
                        played = false;
                    }
                    //fixed (byte* bufferPtr = &bytes[0])
                    //{
                    //    System.IO.UnmanagedMemoryStream ums = new UnmanagedMemoryStream(bufferPtr, bytes.Length);
                    //    SoundPlayer player = new SoundPlayer(ums);                        
                    //    player.Play();
                    //}
                }
            }

            return played;
        }



        protected virtual async Task<bool> PlaySoundFromResourcesAsync(string soundName)
        {
            return await Task<bool>.Run<bool>(() => (PlaySoundFromResource(soundName)));
        }

        #endregion Media Methods
    }
}
