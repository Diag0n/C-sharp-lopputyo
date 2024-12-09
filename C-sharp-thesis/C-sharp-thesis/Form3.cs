using System;
using System.Drawing;
using System.Windows.Forms;

namespace C_sharp_thesis
{
    public partial class Form3 : Form
    {
        private TetrisGame tetrisGame;
        private PictureBox pictureBox1;

        public Form3()
        {
            InitializeComponent();
            KeyPreview = true;
            InitializeGameUI();
            tetrisGame = new TetrisGame(pictureBox1);
        }

        private void InitializeGameUI()
        {
            pictureBox1 = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(200, 400),
                BorderStyle = BorderStyle.FixedSingle
            };

            button1 = new Button
            {
                Text = "Palaa",
                Location = new Point(220, 10),
                Size = new Size(80, 30)
            };

            button1.Click += Button1_Click;

            Controls.Add(pictureBox1);
            Controls.Add(button1);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            tetrisGame.HandleKeyDown(e.KeyCode);
        }
    }

    public class TetrisGame
    {
        private readonly int tileSize = 20;
        private int[,] currentPiece;
        private Point piecePosition;
        private readonly PictureBox pictureBox;
        private readonly Bitmap gameBitmap;

        private readonly int[,] IBlock = new int[4, 4]
        {
        { 0, 0, 0, 0 },
        { 1, 1, 1, 1 },
        { 0, 0, 0, 0 },
        { 0, 0, 0, 0 }
        };

        public TetrisGame(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            this.gameBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            InitGame();
        }

        private void InitGame()
        {
            currentPiece = IBlock;
            piecePosition = new Point(3, 0);
            Timer timer = new Timer { Interval = 500 };
            timer.Tick += (s, e) => MovePiece(0, 1);
            timer.Start();
            DrawGame();
        }

        public void HandleKeyDown(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    MovePiece(-1, 0);
                    break;
                case Keys.Right:
                    MovePiece(1, 0);
                    break;
                case Keys.Down:
                    MovePiece(0, 1);
                    break;
                case Keys.Up:
                    RotatePiece();
                    break;
            }
        }

        private void MovePiece(int dx, int dy)
        {
            piecePosition.X += dx;
            piecePosition.Y += dy;
            DrawGame();
        }

        private void RotatePiece()
        {
            int size = currentPiece.GetLength(0);
            int[,] rotatedPiece = new int[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    rotatedPiece[x, size - 1 - y] = currentPiece[y, x];
                }
            }

            currentPiece = rotatedPiece;
            DrawGame();
        }


        private void DrawGame()
        {
            using (Graphics g = Graphics.FromImage(gameBitmap))
            {
                g.Clear(Color.Black);

                for (int y = 0; y < currentPiece.GetLength(0); y++)
                {
                    for (int x = 0; x < currentPiece.GetLength(1); x++)
                    {
                        if (currentPiece[y, x] == 1)
                        {
                            g.FillRectangle(Brushes.Cyan,
                                (piecePosition.X + x) * tileSize,
                                (piecePosition.Y + y) * tileSize,
                                tileSize, tileSize);
                            g.DrawRectangle(Pens.Black,
                                (piecePosition.X + x) * tileSize,
                                (piecePosition.Y + y) * tileSize,
                                tileSize, tileSize);
                        }
                    }
                }
            }


            pictureBox.Image = (Image)gameBitmap.Clone();
        }
    }
}