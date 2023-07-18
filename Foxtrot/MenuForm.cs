using System.Drawing;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Foxtrot;
using System.Linq.Expressions;
using System.IO.Compression;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic;

public class MenuForm : Form
{
    const int cardHeight = 110;
    const int gap = 20;

    PictureBox pb;
    Bitmap bmp;
    Graphics g;
    Timer tm;

    public MenuForm()
    {
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.None;

        this.pb = new PictureBox();
        this.pb.Dock = DockStyle.Fill;
        this.Controls.Add(pb);

        this.tm = new Timer();
        this.tm.Interval = 20;

        this.KeyDown += (o, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
        };

        this.Load += (o, e) =>
        {
            this.bmp = new Bitmap(pb.Width, pb.Height);
            g = Graphics.FromImage(this.bmp);
            g.Clear(Color.White);
            this.pb.Image = bmp;

            Onstart();

            this.tm.Start();
        };

        tm.Tick += (o, e) =>
        {
            Frame();
            pb.Refresh();
        };

        this.pb.MouseMove += (o, e) =>
        {
            cursor = e.Location;
        };

        this.pb.MouseDown += (o, e) =>
        {
            isDown = true;
        };

        this.pb.MouseUp += (o, e) =>
        {
            isDown = false;
        };
    }

    int bgIndex = 0;
    int frameCount = 0;

    List<Card> cards = Card.GetAll();
    List<Card> defaultCards = Card.GetDefaults();

    List<FixedCardDeck> Decks = new List<FixedCardDeck>();

    List<CardBlock> blocks = new List<CardBlock>();
    CardBlock selected;
    bool isDown = false;

    List<ShopDeck> shops = new List<ShopDeck>();

    CompradoDeck compradoDeck = new();

    List<Image> background = new List<Image>();
    Point cursor = new Point(0, 0);

    Card coronga = Card.Coronga();

    Queue<DateTime> queue = new Queue<DateTime>();
    float fps = 30f;

    FixedCardBlock novaColuna = new();

    bool teste = true;


    void Onstart()
    {

        // background
        for (int i = 0; i < 8; i++)
            this.background.Add(
              Bitmap.FromFile(@$"img/b{i}.gif").GetThumbnailImage(Width, Height, null, IntPtr.Zero)
            );

        // fps
        queue.Enqueue(DateTime.Now);

        var shuffleDeck = this.cards
            .OrderBy(x => Random.Shared.Next())
            .ToArray();

        var Randola = 0;


        for (int i = 0; i < 7; i++)
        {
            novaColuna = new();

            for (int j = 0; j < i + 1; j++)
            {
                var card = shuffleDeck[Randola];
                novaColuna.Cards.Add(card);

                if (novaColuna.Cards.LastIndexOf(card) == i)
                {
                    card.Visible = true;
                }
                else
                    card.Visible = false;
                Randola++;
            }
            var offset = 158 * i;
            novaColuna.Location = new PointF(446 + offset, 20);
            blocks.Add(novaColuna);
        }


        for (int i = 0; i < 4; i++)
        {
            FixedCardDeck novoMonte = new();
            var offset = 130 * i;
            novoMonte.Location = new PointF(20, 20 + offset);
            novoMonte.Cards.Add(defaultCards.ElementAt(i));
            blocks.Add(novoMonte);
        }

        var Randola2 = Randola;

        ShopDeck shopDeck = new();
        shopDeck.Location = new PointF(1740, 20);

        shopDeck.Cards.Add(coronga);

        for (int i = 0; i < shuffleDeck.Length - Randola2; i++)
        {
            var card = shuffleDeck[Randola];
            card.Visible = false;
            shopDeck.Cards.Add(card);
            Randola++;
        }
        blocks.Add(shopDeck);


    }

    void Frame()
    {
        DrawBackground();


        foreach (var coluna in blocks)
        {
            if (coluna is FixedCardBlock)
            {
                if (coluna.Cards.Count > 1 && selected is null)
                {
                    coluna.Cards[^1].Visible = true;
                }
                if (coluna.Cards.Count == 1)
                {
                    coluna.Cards[0].Visible = true;
                }
                else
                {
                    continue;
                }
            }
            
        }

        if (selected is not null)
        {
            this.blocks.Remove(selected);
            this.blocks.Add(selected);
        }

        for (int i = 0; i < blocks.Count; i++)
        {
            var block = blocks[i];


            var cursorInBlock = block.Rect.Contains(cursor);

            if (!isDown && block is ShopDeck shop)
                shop.Selected = false;
            
            if (cursorInBlock && isDown && selected is null)
            {
                var selected = block.OnSelect(cursor);
                this.selected = selected;
            }

            if (block != selected && cursorInBlock && !isDown && selected is not null && block is not ShopDeck)
            {
                block.Cards.AddRange(selected.Cards);
                blocks.Remove(selected);
                selected = null;
                i--;
            }

            if (isDown && selected is not null)
                selected.OnMove(cursor);

            block.Draw(g);


        }

        if (!isDown)
            selected = null;

        DrawFps();

    }

    void DrawBackground()
    {
        var bg = this.background[bgIndex];

        if (frameCount % 6 == 0)
        {
            bgIndex++;
            if (bgIndex == 8)
                bgIndex = 0;
        }
        g.DrawImage(bg, Point.Empty);
    }

    void DrawFps()
    {
        frameCount++;
        var older = queue.Peek();
        var newer = DateTime.Now;

        queue.Enqueue(newer);
        if (queue.Count == 20)
            queue.Dequeue();

        var time = newer - older;
        fps = 20f / (float)time.TotalSeconds;

        var text = Math.Round(fps, 0).ToString();
        StringFormat format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        Font font = new Font(FontFamily.GenericMonospace, 20f);

        g.DrawString(text, font, Brushes.Yellow,
            new RectangleF(Width - 50, 0, 50, 50), format);
    }
}