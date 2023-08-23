using GifFingTool.Data;
using GifFingTool.Data.BitmapModification;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class TextTool : ToolBaseV2
    {
        private static readonly KeyConverter keyConverter = new KeyConverter();

        private bool _drag = false;
        private int? _targetX = 0;
        private int _targetY = 0;
        private string _string = null;
        private protected override IBitmapModifyStep BitmapModifyStep => GetModifyStepOrNull();

        public float FontSize { get => _fontSize; }
        public FontFamily FontFamily { get => _fontFamily; }
        public bool Bold { get => _bold; }
        public bool Italic { get => _italic; }
        public bool Underline { get => _underline; }
        public bool Strikeout { get => _strikeout; }
        public Color Color { get => _color; }

        private float _fontSize = 12;
        private FontFamily _fontFamily = FontFamily.GenericSansSerif;
        private bool _bold = false;
        private bool _underline = false;
        private bool _strikeout = false;
        private bool _italic = false;
        private Color _color = Color.BlueViolet;

        private Brush _brush = new SolidBrush(Color.BlueViolet);
        private Font _font;

        public Action InitCaptureKeyboardInput = () => { };

        private void CompileFont()
        {
            FontStyle fontStyle = FontStyle.Regular;
            if (_bold) fontStyle |= FontStyle.Bold;
            if (_italic) fontStyle |= FontStyle.Italic;
            if (_underline) fontStyle |= FontStyle.Underline;
            if (_strikeout) fontStyle |= FontStyle.Strikeout;
            
            _font = new Font(_fontFamily, _fontSize, fontStyle);
        }

        public void SetFont(FontFamily fontFamily, float fontSize, bool bold, bool italic, bool underline, bool strikeout, Color color)
        {
            _fontFamily = fontFamily;
            _fontSize = fontSize;
            _bold = bold;
            _italic = italic;
            _underline = underline;
            _strikeout = strikeout;
            _font = null;

            _brush = new SolidBrush(color);
            Redraw();
        }
        public void SetFont(FontFamily fontFamily, float fontSize, bool bold, bool italic, bool underline, bool strikeout)
        {
            _fontFamily = fontFamily;
            _fontSize = fontSize;
            _bold = bold;
            _italic = italic;
            _underline = underline;
            _strikeout = strikeout;
            _font = null;

            Redraw();
        }

        public TextTool(GifBitmapEditingContext editingContext) : base(editingContext)
        {

        }

        public void SetBold(bool value)
        {
            _bold = value;
            _font = null;
            Redraw();
        }

        public void SetItalic(bool value)
        {
            _italic = value;
            _font = null;
            Redraw();
        }

        public void SetUnderline(bool value)
        {
            _underline = value;
            _font = null;
            Redraw();
        }

        public void SetStrikeout(bool value)
        {
            _strikeout = value;
            _font = null;
            Redraw();
        }

        public void SetColor(Color color)
        {
            _brush = new SolidBrush(color);
            Redraw();
        }

        public void SetFontFamily(FontFamily fontFamily)
        {
            _fontFamily = fontFamily;
            _font = null;
            Redraw();
        }

        public void SetFontSize(int value)
        {
            _fontSize = value;
            _font = null;
            Redraw();
        }

        public override void MouseClick(int x, int y)
        {
        }

        public override void MouseEnter(int x, int y)
        {
            _drag = false;
        }

        public override void MouseLeave(int x, int y)
        {
            _drag = false;
        }

        public override void MouseLeftButtonDown(int x, int y)
        {
            _drag = true;
            _targetX = x;
            _targetY = y;
            Redraw();
            InitCaptureKeyboardInput();
        }

        public override void MouseLeftButtonUp(int x, int y)
        {
            _drag = false;
        }

        public override void MouseMove(int x, int y)
        {
            if (!_drag) return;
            _targetX = x;
            _targetY = y;
            Redraw();
        }

        public override void MouseRightButtonDown(int x, int y)
        {
            Reset();
            _targetX = x;
            _targetY = y;
            Redraw();
        }

        public override void MouseRightButtonUp(int x, int y)
        {

        }

        public override void Reset()
        {
            _string = null;
            _targetX = null;
        }

        public void SetString(string str)
        {
            _string = str;
            InvokeConfigChangedNotifyEvent();
            Redraw();
        }

        private void Redraw()
        {
            if (_string is null) return;
            if (!(_targetX is int x)) return;
            if (_font is null) CompileFont();

            Bitmap newBitmap = new Bitmap(EditingContext.GifBitmap.ModifiedImage);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(_string, _font, _brush, x, _targetY);
            }
            EditingContext.Bitmap = newBitmap;
        }

        public override void KeyDown(Key key)
        {
            if (_targetX is null) return;

            bool controlHeldDown = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            bool shiftHeldDown = (Keyboard.Modifiers & ModifierKeys.Shift) > 0;

            int stepSize = shiftHeldDown ? 10 : 1;

            switch(key)
            {
                case Key.Up:
                    _targetY = controlHeldDown ? 0 : _targetY - stepSize;
                    Redraw();
                    break;
                case Key.Down:
                    _targetY = controlHeldDown ? EditingContext.Bitmap.Height - (int)Math.Ceiling(_fontSize) : _targetY + stepSize;
                    Redraw();
                    break;
                case Key.Left:
                    _targetX = controlHeldDown ? 0 : _targetX - stepSize;
                    Redraw();
                    break;
                case Key.Right:
                    _targetX = controlHeldDown ? EditingContext.Bitmap.Width - (int)Math.Ceiling(_fontSize) : _targetX + stepSize;
                    Redraw();
                    break;

                //case Key.B:
                //    SetBold(!_bold);
                //    break;
                //case Key.I:
                //    SetItalic(!_italic);
                //    break;
                //case Key.S:
                //    SetStrikeout(!_strikeout);
                //    break;
                //case Key.U:
                //    SetUnderline(!_underline);
                //    break;

            }
        }

        private unsafe IBitmapModifyStep GetModifyStepOrNull()
        {
            if (!(_string is string drawString)) return null;
            if (!(_targetX is int targetX)) return null;

            using (Bitmap bitmap = new Bitmap(EditingContext.Bitmap.Width, EditingContext.Bitmap.Height))
            {
                int unmodifiedPixelColor = bitmap.GetPixel(0, 0).ToArgb();
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(drawString, _font, _brush, targetX, _targetY);
                }

                return BmpModColorsPixels.FromBitmap(bitmap, unmodifiedPixelColor, EditingContext.GifBitmap.RotateFlipState);
            }
        }
    }
}
