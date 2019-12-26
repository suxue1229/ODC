using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ODClient
{
    public partial class DispItem : UserControl
    {
        private String _id;
        public String ItemId
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        private String _name;
        public String ItemName
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
                DispName.Text = this._name;
            }
        }

        private Object _value;
        public Object ItemValue
        {
            get
            {
                return this._value;
            }
            set
            {
                this._servok = true;
                this._value = value;
                DispValue.Text = this._value.ToString();
                ResetTimer();
            }
        }

        private String _unit;
        public String ItemUnit
        {
            get
            {
                return this._unit;
            }
            set
            {
                this._unit = value;
                DispUnit.Text = String.IsNullOrEmpty(this._unit) ? String.Empty : String.Format("({0})", this._unit);
            }
        }

        private Int32 _quality;
        public Int32 Quality
        {
            get
            {
                return this._quality;
            }
            set
            {
                this._servok = true;
                this._quality = value;
                DispQuality.Invalidate();
            }
        }

        private Boolean _servok = true;
        public Boolean ServOK
        {
            set
            {
                this._servok = value;
                DispQuality.Invalidate();
            }
        }

        private System.Timers.Timer _updTimer;
        private void ResetTimer()
        {
            if (_updTimer == null)
            {
                _updTimer = new System.Timers.Timer(10000);
                _updTimer.Elapsed += _updTimer_Tick;
                _updTimer.AutoReset = false;
            }
            _updTimer.Stop();
            _updTimer.Start();
        }

        void _updTimer_Tick(object sender, EventArgs e)
        {
            this._servok = false;
            DispQuality.Invalidate();
        }

        public DispItem()
        {
            InitializeComponent();
            ItemUnit = String.Empty;
        }

        private Color[] QuColor = new Color[] { Color.Black, Color.Red, Color.Orange, Color.Green, Color.LimeGreen };

        private void DispQuality_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = DispQuality.ClientRectangle;
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            using (BufferedGraphics buff = context.Allocate(e.Graphics, rect))
            {
                using (Graphics graphic = buff.Graphics)
                {
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    graphic.Clear(DispQuality.BackColor);
                    Int32 colIndex = this._servok ? Math.Min(QuColor.Length, ((Quality >> 6) & 0x03) + 1) : 0;
                    Int32 colCount = this._servok ? Math.Min(5, ((Quality >> 6) & 0x03) + 2) : 1;
                    for (int i = 0; i < colCount; i++)
                    {
                        graphic.FillRectangle(new SolidBrush(QuColor[colIndex]),
                            (float)(rect.Width * 0.15), (float)(rect.Height * (0.95 - 0.14 - i * 0.19)), (float)(rect.Width * 0.7), (float)(rect.Height * 0.14));
                    }
                    for (int i = colCount; i < 5; i++)
                    {
                        graphic.DrawRectangle(new Pen(Color.Gray),
                            (float)(rect.Width * 0.2), (float)(rect.Height * (0.95 - 0.14 - i * 0.19)), (float)(rect.Width * 0.6), (float)(rect.Height * 0.14));
                    }
                    buff.Render(e.Graphics);
                }
            }
        }
    }
}
