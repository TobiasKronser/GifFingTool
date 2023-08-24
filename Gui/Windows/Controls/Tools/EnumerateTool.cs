using GifFingTool.Data.BitmapModification;
using GifFingTool.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class EnumerateTool : ToolBaseV2
    {
        private bool _drag = false;
        private int? _targetX = 0;
        private int _targetY = 0;

        public int Number => _number;
        private int _number = 1;
        private string _string => _number.ToString();
        private protected override IBitmapModifyStep BitmapModifyStep => GetModifyStepOrNull();

        public float FontSize { get => _fontSize; }
        private float _fontSize = 12;

        public FontFamily FontFamily { get => _fontFamily; }
        private FontFamily _fontFamily = FontFamily.GenericSansSerif;

        public bool Bold { get => _bold; }
        private bool _bold = false;
        public bool Italic { get => _italic; }
        private bool _italic = false;
        public bool Underline { get => _underline; }
        private bool _underline = false;
        public Color Color { get => _color; }
        private Color _color = Color.BlueViolet;

        private Brush _brush = new SolidBrush(Color.BlueViolet);
        private Font _font;

        private void CompileFont()
        {
            FontStyle fontStyle = FontStyle.Regular;
            if (_bold) fontStyle |= FontStyle.Bold;
            if (_italic) fontStyle |= FontStyle.Italic;
            if (_underline) fontStyle |= FontStyle.Underline;

            _font = new Font(_fontFamily, _fontSize, fontStyle);
        }

        public void SetFont(FontFamily fontFamily, float fontSize, bool bold, bool italic, bool underline, Color color)
        {
            _fontFamily = fontFamily;
            _fontSize = fontSize;
            _bold = bold;
            _italic = italic;
            _underline = underline;
            _font = null;

            _brush = new SolidBrush(color);
            Redraw();
        }
        public void SetFont(FontFamily fontFamily, float fontSize, bool bold, bool italic, bool underline)
        {
            _fontFamily = fontFamily;
            _fontSize = fontSize;
            _bold = bold;
            _italic = italic;
            _underline = underline;
            _font = null;

            Redraw();
        }

        public EnumerateTool(GifBitmapEditingContext editingContext) : base(editingContext)
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
            EditingContext.ProcessToolOperationDone();
            _drag = true;
            _targetX = x;
            _targetY = y;
            Redraw();
            SetNumber(_number + 1);
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
            _targetX = x;
            _targetY = y;
            Redraw();
            //SetNumber(_number + 1);
        }

        public override void MouseRightButtonUp(int x, int y)
        {

        }

        public override void Reset()
        {
            _targetX = null;
        }

        public void SetNumber(int number)
        {
            _number = number;
            InvokeConfigChangedNotifyEvent();
        }

        private void Redraw()
        {
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

            switch (key)
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
            if (!(_targetX is int targetX)) return null;

            using (Bitmap bitmap = new Bitmap(EditingContext.Bitmap.Width, EditingContext.Bitmap.Height))
            {
                int unmodifiedPixelColor = bitmap.GetPixel(0, 0).ToArgb();
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(_string, _font, _brush, targetX, _targetY);
                }

                return BmpModColorsPixels.FromBitmap(bitmap, unmodifiedPixelColor, EditingContext.GifBitmap.RotateFlipState);
            }
        }
    }
}
