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
            KeyPreview = true; // Enables listening to keystrokes
            InitializeGameUI();
            tetrisGame = new TetrisGame(pictureBox1);
        }

        private void InitializeGameUI()
        {
            // Gamespace
            pictureBox1 = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(400, 800),
                BorderStyle = BorderStyle.FixedSingle
            };

            // return-button
            Button returnButton = new Button
            {
                Text = "Return",
                Location = new Point(220, 10),
                Size = new Size(80, 30)
            };

            returnButton.Click += Button1_Click;

            Controls.Add(returnButton);

            Controls.Add(pictureBox1);
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            tetrisGame.HandleKeyDown(e.KeyCode);
        }
    }

    public class TetrisGame
    {
        private readonly int tileSize = 20; // Square size in pixels
        private readonly int boardWidth = 10; // Game area width
        private readonly int boardHeight = 20; // Game area height
        private int[,] board; // Game area
        private int[,] currentPiece; // Current piece
        private Point piecePosition; // Current piece position
        private readonly PictureBox pictureBox;
        private readonly Bitmap gameBitmap;
        private Timer gameTimer;

        // Piece shape
        private readonly int[][,] pieces = new int[][,]
        {
            new int[4, 4] // I-piece
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            new int[2, 2] // O-piece
            {
                { 1, 1 },
                { 1, 1 }
            },
            new int[3, 3] // T-piece
            {
                { 0, 1, 0 },
                { 1, 1, 1 },
                { 0, 0, 0 }
            }
        };

        public TetrisGame(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            this.gameBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            InitGame();
        }

        private void InitGame()
        {
            board = new int[boardHeight, boardWidth];
            SpawnNewPiece();

            gameTimer = new Timer { Interval = 500 };
            gameTimer.Tick += (s, e) => MovePiece(0, 1);
            gameTimer.Start();

            DrawGame();
        }

        public void HandleKeyDown(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.A:
                    MovePiece(-1, 0);
                    break;
                case Keys.D:
                    MovePiece(1, 0);
                    break;
                case Keys.S:
                    MovePiece(0, 1);
                    break;
                case Keys.W:
                    RotatePiece();
                    break;
            }
        }


        private void SpawnNewPiece()
        {
            var random = new Random();
            currentPiece = pieces[random.Next(pieces.Length)];
            piecePosition = new Point(boardWidth / 2 - currentPiece.GetLength(1) / 2, 0);

            if (!CanPlacePiece(piecePosition.X, piecePosition.Y, currentPiece))
            {
                gameTimer.Stop();
                MessageBox.Show("Game Over!");
                return;
            }

        }



        private bool CanPlacePiece(int x, int y, int[,] piece)
        {
            for (int row = 0; row < piece.GetLength(0); row++)
            {
                for (int col = 0; col < piece.GetLength(1); col++)
                {
                    if (piece[row, col] == 1)
                    {
                        int newX = x + col;
                        int newY = y + row;

                        // Makes sure that piece is inside game area
                        if (newX < 0 || newX >= boardWidth || newY >= boardHeight)
                        {
                            return false;
                        }

                        // Makes sure that piece doesn't touch other pieces
                        if (board[newY, newX] == 1)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }



        private void MovePiece(int dx, int dy)
        {
            int newX = piecePosition.X + dx;
            int newY = piecePosition.Y + dy;

            if (CanPlacePiece(newX, newY, currentPiece))
            {
                piecePosition.X = newX;
                piecePosition.Y = newY;
                DrawGame();
            }
            else if (dy > 0)
            {
                PlacePiece(); 
                ClearFullRows();
                SpawnNewPiece();
                DrawGame();
            }
        }



        private void RotatePiece()
        {
            int size = currentPiece.GetLength(0);
            var rotatedPiece = new int[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    rotatedPiece[x, size - 1 - y] = currentPiece[y, x];
                }
            }

            // Checks if piece is able to rotate
            if (CanPlacePiece(piecePosition.X, piecePosition.Y, rotatedPiece))
            {
                currentPiece = rotatedPiece;
                DrawGame();
            }
        }




        private void PlacePiece()
        {
            for (int row = 0; row < currentPiece.GetLength(0); row++)
            {
                for (int col = 0; col < currentPiece.GetLength(1); col++)
                {
                    if (currentPiece[row, col] == 1)
                    {
                        int boardX = piecePosition.X + col;
                        int boardY = piecePosition.Y + row;
                        board[boardY, boardX] = 1;
                    }
                }
            }
        }

        private void ClearFullRows()
        {
            for (int row = 0; row < boardHeight; row++)
            {
                bool isFull = true;

                for (int col = 0; col < boardWidth; col++)
                {
                    if (board[row, col] == 0)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                {
                    for (int y = row; y > 0; y--)
                    {
                        for (int col = 0; col < boardWidth; col++)
                        {
                            board[y, col] = board[y - 1, col];
                        }
                    }

                    for (int col = 0; col < boardWidth; col++)
                    {
                        board[0, col] = 0;
                    }
                }
            }
        }

        private void DrawGame()
        {
            using (Graphics g = Graphics.FromImage(gameBitmap))
            {
                g.Clear(Color.Black);

                // Draws game area
                for (int row = 0; row < boardHeight; row++)
                {
                    for (int col = 0; col < boardWidth; col++)
                    {
                        if (board[row, col] == 1)
                        {
                            g.FillRectangle(Brushes.Gray, col * tileSize, row * tileSize, tileSize, tileSize);
                            g.DrawRectangle(Pens.Black, col * tileSize, row * tileSize, tileSize, tileSize);
                        }
                    }
                }

                // Draws current piece
                for (int row = 0; row < currentPiece.GetLength(0); row++)
                {
                    for (int col = 0; col < currentPiece.GetLength(1); col++)
                    {
                        if (currentPiece[row, col] == 1)
                        {
                            g.FillRectangle(Brushes.Cyan,
                                (piecePosition.X + col) * tileSize,
                                (piecePosition.Y + row) * tileSize,
                                tileSize, tileSize);
                            g.DrawRectangle(Pens.Black,
                                (piecePosition.X + col) * tileSize,
                                (piecePosition.Y + row) * tileSize,
                                tileSize, tileSize);
                        }
                    }
                }
            }

            pictureBox.Image = (Image)gameBitmap.Clone();
        }
    }
}
