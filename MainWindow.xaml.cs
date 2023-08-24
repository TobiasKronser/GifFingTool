using GifFingTool.Data;
using GifFingTool.Extensions;
using GifFingTool.GifEncoding;
using GifFingTool.Gui;
using GifFingTool.Gui.GlobalHooks;
using GifFingTool.Gui.Shortcuts;
using GifFingTool.Gui.Windows;
using GifFingTool.Gui.Windows.Controls;
using GifFingTool.IO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace GifFingTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SoundPlayer soundPlayer = new SoundPlayer();
        private TargetArea _targetArea = new TargetArea();
        private Rectangle _globalTargetArea = Screen.PrimaryScreen.Bounds;
        private TargetSelectionWindow TargetSelectionWindow;
        private GifImageList _CurrentImageList;
        private ShortcutManager _ShortcutManager;


        public MainWindow()
        {
            InitializeComponent();

            TargetSelectionWindow = new TargetSelectionWindow(_targetArea);
            _CurrentImageList = new GifImageList(PreviewImageDataGrid);

            _ShortcutManager = new ShortcutManager(this);

            KeyGroup shiftKey = KeyGroup.GetDefaultKeyGroup(Key.LeftShift);
            KeyGroup controlKey = KeyGroup.GetDefaultKeyGroup(Key.LeftCtrl);
            KeyGroup.TryCreateKeyGroup("shiftControl", "shiftControl", out KeyGroup shiftOrControl, Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl);
            
            _ShortcutManager.RegisterShortcut("Screenshot", "Take a screenshot of the currently targeted area.", TakeScreenshot, Key.G, true, false, shiftOrControl);
            _ShortcutManager.RegisterShortcut("Select Target", "Open the target selection window.", ShowTargetSelectWindow, Key.T, true, false, shiftOrControl);
            _ShortcutManager.RegisterShortcut("Undo", "Undo", Undo, Key.Z, true, false, controlKey);
            _ShortcutManager.RegisterShortcut("Redo", "Redo", Redo, Key.Y, true, false, controlKey);
            _ShortcutManager.RegisterShortcut("Save", "Open the Save-Dialog for the entire gif or the current image.", Save, Key.S, true, false, controlKey);
            _ShortcutManager.RegisterShortcut("Copy to clipboard", "Copy the current image to clipboard.", AttemptCopyToClipboard, Key.C, true, false, controlKey);
            _ShortcutManager.RegisterShortcut("Paste from clipboard", "If an image is contained in the clipboard, paste it.", AttemptPasteFromClipboard, Key.V, true, false, controlKey);
            _ShortcutManager.RegisterShortcut("Delete selected image", "Permanently delets a selected image.", AttemptDeleteSelectedImage, Key.Delete, true, false, new Key[0]);

            InterceptKeys.AssureTimeframeCount(1);
            InterceptKeys.RegisterHook(new TimedHook(TryEnableTimeframe, millisTimeframe: 250, ModifierKeys.None, Keys.LShiftKey)); //When LShift is pressed twice within 200ms, enable Timeframe 0 for 1000ms
            InterceptKeys.RegisterHook(new TimedHook(TryEnableTimeframe, millisTimeframe: 250, ModifierKeys.None, Keys.RShiftKey)); //When LShift is pressed twice within 200ms, enable Timeframe 0 for 1000ms
            InterceptKeys.RegisterHook(new TimeframeDependantHook(TakeScreenshot, timeframeIndex: 0, supressOtherHooks: true, ModifierKeys.Shift, Keys.G)); //When Timeframe 0 is enabled and G is pressed while Shift is being held down, call TakeScreenshot
            InterceptKeys.RegisterHook(new TimeframeDependantHook(ShowTargetSelectWindow, timeframeIndex: 0, supressOtherHooks: true, ModifierKeys.Shift, Keys.T)); //When Timeframe 0 is enabled and T is pressed while Shift is being held down, call ShowTargetSelectWindow
            InterceptKeys.RegisterHook(new TimeframeDependantHook(Undo, timeframeIndex: 0, supressOtherHooks: true, ModifierKeys.Shift, Keys.Z)); //When Timeframe 0 is enabled and T is pressed while Shift is being held down, call ShowTargetSelectWindow
            InterceptKeys.RegisterHook(new TimeframeDependantHook(Redo, timeframeIndex: 0, supressOtherHooks: true, ModifierKeys.Shift, Keys.Y));
            InterceptKeys.RegisterHook(new TimeframeDependantHook(Save, timeframeIndex: 0, supressOtherHooks: true, ModifierKeys.Shift, Keys.S));
            InterceptKeys.RegisterHook(new TimeframeDependantHook(AttemptCopyToClipboard, timeframeIndex: 0, supressOtherHooks: true, ModifierKeys.Shift, Keys.C));

            //InterceptKeys.RegisterHook(new Hook(Undo, ModifierKeys.Shift, Keys.Z)); //When Z is pressed while Shift is being held down, call Undo
            //InterceptKeys.RegisterHook(new Hook(Redo, ModifierKeys.Shift, Keys.Y));
            //InterceptKeys.RegisterHook(new Hook(Save, ModifierKeys.Shift, Keys.S));

            Toolbar.SetUpdateDisplayMethod(UpdateDisplay);
            Toolbar.SetUpdateListDisplayMethod(UpdateListDisplay);
        }

        private void AttemptCopyToClipboard()
        {
            if (_SelectedGifBitmap is null) return;
            System.Windows.Forms.Clipboard.SetImage(_SelectedGifBitmap.ModifiedImage);
        }
        private void AttemptPasteFromClipboard()
        {
            if (!System.Windows.Forms.Clipboard.ContainsImage()) return;
            GifBitmap gifBitmap = new GifBitmap(System.Windows.Forms.Clipboard.GetImage());
            _CurrentImageList.AddBitmap(gifBitmap);
        }
        private void AttemptDeleteSelectedImage()
        {
            if (_SelectedGifBitmap is null) return;
            _CurrentImageList.RemoveItem(_SelectedGifBitmap);
            //_SelectedGifBitmap = null;
        }

        private void TryEnableTimeframe()
        {
            if (this.IsActive) return;
            InterceptKeys.EnableTimeframe(timeframeIndex: 0, millisDuration: 1000);
        }

        private void UpdateListDisplay(GifBitmap gifbitmap)
        {
            _CurrentImageList.AttemptRefreshDisplays(gifbitmap);
        }
        private void UpdateDisplay(Bitmap bitmap)
        {
            Temp.printBitmap(SelectedImageDisplay, bitmap);
        }

        private void Undo()
        {
            Toolbar.Undo();
        }

        private void Redo()
        {
            Toolbar.Redo();
        }

        private void Save()
        {
            if (_CurrentImageList.IsEmpty()) return;
            SaveFileHelper.TrySave(_CurrentImageList.GetCurrentList(), _SelectedGifBitmap.ModifiedImage, false, out string _);
        }

        private void ShowTargetSelectWindow()
        {
            if (TargetSelectionWindow.Visibility == Visibility.Visible)
            {
                return;
            }
            _globalTargetArea = TargetSelectionWindow.ShowModalTargetSelect(Screen.AllScreens[0], _globalTargetArea);
        }

        private void TakeScreenshot()
        {
            soundPlayer.Stream = Properties.Resources.ScreenshotMade;
            GifBitmap gifBitmap = new GifBitmap(_targetArea.CreateScreenshot(false));
            soundPlayer.PlaySync();
            _CurrentImageList.AddBitmap(gifBitmap);
            PreviewImageDataGrid.SelectedItem = gifBitmap;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TargetSelectionWindow.Close();
            App.Current.Shutdown();
        }

        private GifBitmap _SelectedGifBitmap = null;
        private void PreviewImageDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count == 1)
            {
                _SelectedGifBitmap = (GifBitmap)e.AddedCells[0].Item;
                _SelectedGifBitmap.Refresh();
                Toolbar.SetNewTarget(_SelectedGifBitmap);
            } else
            {
                //TODO: Figure out how _CurrentImageList.Count is always 0
                //TargetSelectionWindow = TargetSelectionWindow;
                //if (_CurrentImageList.Count == 0)
                //{
                //    _SelectedGifBitmap = null;
                //    Toolbar.SetNewTarget(_SelectedGifBitmap);
                //}
            }
        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            TakeScreenshot();
        }

        private void SelectTargetAreaButton_Click(object sender, RoutedEventArgs e)
        {
            _globalTargetArea = TargetSelectionWindow.ShowModalTargetSelect(Screen.PrimaryScreen, _globalTargetArea);
        }


        #region ImageEditButtonEvents
        private void InvokeImageEditFunction(Action<int, int> actionToInvoke, System.Windows.Input.MouseEventArgs e)
        {
            if (_SelectedGifBitmap is null) return;

            System.Windows.Point position = e.GetPosition(SelectedImageDisplay);
            int x = (int)position.X;
            int y = (int)position.Y;
            if (x < 0 || y < 0 ||
                x >= _SelectedGifBitmap.ModifiedImage.Width ||
                y >= _SelectedGifBitmap.ModifiedImage.Height)
            {
                return;
            }
            actionToInvoke(x, y);
        }
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (_SelectedGifBitmap is null) return;
            Toolbar.ActiveTool.KeyDown(e.Key);
        }
        private void ImageEditButton_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        private void ImageEditButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            InvokeImageEditFunction(Toolbar.ActiveTool.MouseEnter, e);
        }

        private void ImageEditButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            InvokeImageEditFunction(Toolbar.ActiveTool.MouseLeave, e);
        }

        private void ImageEditButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            InvokeImageEditFunction(Toolbar.ActiveTool.MouseMove, e);
        }

        private void ImageEditButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InvokeImageEditFunction(Toolbar.ActiveTool.MouseLeftButtonDown, e);
        }

        private void ImageEditButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InvokeImageEditFunction(Toolbar.ActiveTool.MouseLeftButtonUp, e);
        }

        private void ImageEditButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            InvokeImageEditFunction(Toolbar.ActiveTool.MouseRightButtonDown, e);
        }

        private void ImageEditButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            InvokeImageEditFunction(Toolbar.ActiveTool.MouseRightButtonUp, e);
        }
        #endregion ImageEditButtonEvents


    }
}
