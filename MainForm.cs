using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class MainForm : Form
    {
        private Graphics _gfx;
        private Pen _penArr = new Pen(Color.Silver);
        private Pen _penExp = new Pen(Color.Red);
        private Pen _penSine = new Pen(Color.Green);
        private Font _font = new Font(FontFamily.GenericSansSerif, 12);
        private int[] _arr;
        private int[] _arrExp;
        private int[] _arrSine;
        private Size _panelSize;
        private double _smoothFact = 0.01; ///< Smoothing factor (0 < a < 1).

        public MainForm()
        {
            InitializeComponent();
            _panelSize = panel1.Size;
        }

        private void PanelClear()
        {
            _gfx.Clear(Color.White);
        }

        private void PanelDrawArray<T>(Pen pen, IList<T> arr) where T: IConvertible
        {
            int zero = panel1.Size.Height / 2;

            var points = new Point[arr.Count];

            for (int i = 0; i < arr.Count; i++)
            {
                points[i].X = i;
                points[i].Y = arr[i].ToInt32(NumberFormatInfo.CurrentInfo) + zero;
            }

            _gfx.DrawLines(pen, points);
        }

        private void Draw()
        {
            /// Updating size of the panel.
            if ((_gfx == null) || (_panelSize != panel1.Size))
            {
                _panelSize = panel1.Size;

                if (_gfx != null)
                {
                    _gfx.Dispose();
                }

                _gfx = panel1.CreateGraphics();
            }

            var rnd = new Random();
            int maxValue = panel1.Height / 4;
            
            int exp = 0;

            _arr = new int[panel1.Size.Width];
            _arrExp = new int[panel1.Size.Width];
            _arrSine = new int[panel1.Size.Width];

            for (int i = 0; i < _arr.Length; i++)
            {
                int noise = rnd.Next(150);

                var angle = Math.PI * i / 180 / 2;
                var sv = (int)(Math.Sin(angle) * maxValue);

                _arrSine[i] = sv;
                _arr[i] = rnd.Next(sv - noise, sv + noise);
                
                exp = (int)(_smoothFact * _arr[i] + (1 - _smoothFact) * exp);
                _arrExp[i] = exp;
            }

            PanelClear();
            PanelDrawArray(_penSine, _arrSine);
            PanelDrawArray(_penArr, _arr);
            PanelDrawArray(_penExp, _arrExp);

            _gfx.DrawString("Initial Sine", _font, _penSine.Brush, 0, 0);
            _gfx.DrawString("Sine with noise", _font, _penArr.Brush, 0, _font.GetHeight());
            _gfx.DrawString("Sine with exponential smoothing", _font, _penExp.Brush, 0, 2 * _font.GetHeight());
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            _smoothFact = trackBar1.Value / 100.0;
            label1.Text = $"Smoothing factor(0 < a < 1): {_smoothFact:G}";
            Draw();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            TrackBar1_Scroll(this, EventArgs.Empty);
        }

        private void Panel1_SizeChanged(object sender, EventArgs e)
        {
            TrackBar1_Scroll(this, EventArgs.Empty);
        }
    }
}
