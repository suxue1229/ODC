using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ODClient
{
    public partial class DispFrame : UserControl
    {
        private Boolean IsExpanded = true, IsMouseOver = false;

        private String _caption;
        public String Caption
        {
            get
            {
                return this._caption;
            }
            set
            {
                this._caption = value;
                UpdateCaption();
            }
        }

        private Dictionary<String, DispItem> _items = new Dictionary<String, DispItem>();

        public String[] Items
        {
            get
            {
                String[] keys = new String[this._items.Keys.Count];
                this._items.Keys.CopyTo(keys, 0);
                return keys;
            }
        }
        public DispFrame()
        {
            InitializeComponent();
            Caption = "DispFrame";
        }


        private void DispFrame_Load(object sender, EventArgs e)
        {
            this.Height = DispLayout.Height;
        }

        private void UpdateCaption()
        {
            DispLabel.Text = String.Format("{0}({1})", Caption, this._items.Count);
        }

        private void DispShow_MouseEnter(object sender, EventArgs e)
        {
            IsMouseOver = true;
        }

        private void DispShow_MouseLeave(object sender, EventArgs e)
        {
            IsMouseOver = false;
        }

        private void DispShow_MouseClick(object sender, MouseEventArgs e)
        {
            IsExpanded = !IsExpanded;
            DispShow.Invalidate();
            DispPanel.Visible = IsExpanded;
        }

        private void DispShow_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = DispShow.ClientRectangle;
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            using (BufferedGraphics buff = context.Allocate(e.Graphics, rect))
            {
                using (Graphics graphic = buff.Graphics)
                {
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    graphic.Clear(DispShow.BackColor);
                    if (IsExpanded)
                    {
                        if (IsMouseOver)
                        {
                            graphic.DrawLines(new Pen(Color.SkyBlue, 1.75F),
                                new Point[]{
                                        new Point((int)(rect.Width*0.65),(int)(rect.Height*0.35)),
                                        new Point((int)(rect.Width*0.65),(int)(rect.Height*0.65)),
                                        new Point((int)(rect.Width*0.4),(int)(rect.Height*0.65)),
                                        new Point((int)(rect.Width*0.65),(int)(rect.Height*0.35))
                                    });
                        }
                        else
                        {
                            graphic.FillPolygon(Brushes.Black,
                                   new Point[] { 
                                        new Point((int)(rect.Width*0.7),(int)(rect.Height*0.3)),
                                        new Point((int)(rect.Width*0.7),(int)(rect.Height*0.7)),
                                        new Point((int)(rect.Width*0.3),(int)(rect.Height*0.7))
                                    });
                        }
                    }
                    else
                    {
                        if (IsMouseOver)
                        {
                            graphic.DrawLines(new Pen(Color.SkyBlue, 1.75F),
                                   new Point[]{
                                        new Point((int)(rect.Width*0.4),(int)(rect.Height*0.3)),
                                        new Point((int)(rect.Width*0.4),(int)(rect.Height*0.7)),
                                        new Point((int)(rect.Width*0.6),(int)(rect.Height*0.5)),
                                        new Point((int)(rect.Width*0.4),(int)(rect.Height*0.3))
                                    });
                        }
                        else
                        {
                            graphic.DrawLines(new Pen(Color.Black, 1.75F),
                                new Point[]{
                                        new Point((int)(rect.Width*0.4),(int)(rect.Height*0.3)),
                                        new Point((int)(rect.Width*0.4),(int)(rect.Height*0.7)),
                                        new Point((int)(rect.Width*0.6),(int)(rect.Height*0.5)),
                                        new Point((int)(rect.Width*0.4),(int)(rect.Height*0.3))
                                    });
                        }
                    }
                    buff.Render(e.Graphics);
                }
            }
        }

        private void DispBanner_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = DispBanner.ClientRectangle;
            e.Graphics.Clear(DispBanner.BackColor);
            e.Graphics.DrawLine(new Pen(Color.Gray), new Point(3, rect.Height / 2), new Point(rect.Width - 6, rect.Height / 2));
        }

        private void DispLayout_SizeChanged(object sender, EventArgs e)
        {
            this.Height = DispLayout.Height;
        }

        private void DispFrame_SizeChanged(object sender, EventArgs e)
        {
            DispBanner.Invalidate();
        }

        public void AddItem(String ItemId, String ItemName)
        {
            AddItem(ItemId, ItemName, String.Empty);
        }

        public void AddItem(String ItemId, String ItemName, String ItemUnit)
        {
            if (!this._items.ContainsKey(ItemName))
            {
                DispItem item = new DispItem();
                item.Width = 160;
                item.ItemId = ItemId;
                item.ItemName = ItemName;
                item.ItemUnit = ItemUnit;
                item.BorderStyle = BorderStyle.FixedSingle;
                DispPanel.Controls.Add(item);
                this._items.Add(item.ItemId, item);
                UpdateCaption();
            }
        }

        public void SetItemValue(String ItemId, Object ItemValue)
        {
            if (this._items.ContainsKey(ItemId))
            {
                this._items[ItemId].ItemValue = ItemValue;
            }
        }

        public void SetItemValue(String ItemId, Object ItemValue, Int32 Quality)
        {
            if (this._items.ContainsKey(ItemId))
            {
                this._items[ItemId].ItemValue = ItemValue;
                this._items[ItemId].Quality = Quality;
            }
        }

        public void SetItemQuality(String ItemId, Int32 Quality)
        {
            if (this._items.ContainsKey(ItemId))
            {
                this._items[ItemId].Quality = Quality;
            }
        }
    }
}
