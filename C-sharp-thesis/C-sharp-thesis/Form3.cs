﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace C_sharp_thesis
{
    public partial class Form3 : Form
    {
        private TetrisGame tetrisGame;
        private PictureBox pictureBox1;
        private Label scoreLabel; // Label to display the score
        public int score; // Variable to hold the score

        public Form3()
        {
            InitializeComponent();
            KeyPreview = true; // Enables listening to keystrokes
            InitializeGameUI();
            tetrisGame = new TetrisGame(pictureBox1, this);
        }

        private void InitializeGameUI()
        {
            // Setup for the game area
            pictureBox1 = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(400, 800), // Increased game area size
                BorderStyle = BorderStyle.FixedSingle
            };

            // Return button
            Button returnButton = new Button
            {
                Text = "Return",
                Location = new Point(420, 1200),
                Size = new Size(80, 30)
            };

            scoreLabel = new Label
            {
                AutoSize = false,
                Text = "Score: 0",
                Location = new Point(420, 50),
                Size = new Size(240, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White // Set the text color to white
            };

            Controls.Add(returnButton);
            Controls.Add(pictureBox1);
            Controls.Add(scoreLabel);
        }

        public void UpdateScore(int points)
        {
            score += points; // Add the points to the current score
            RefreshUI(); // Refresh the score display
        }

        private void RefreshUI()
        {
            scoreLabel.Text = $"Score: {score}"; // Update the score label with the new score
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes the current form
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            tetrisGame.HandleKeyDown(e.KeyCode); // Passes key events to the game
        }
    }

    public class TetrisGame
    {
        private readonly int tileSize = 40; // Size of each tile in pixels
        private readonly int boardWidth = 10; // Game area width in tiles
        private readonly int boardHeight = 20; // Game area height in tiles
        private int[,] board; // Represents the game grid
        private int[,] currentPiece; // Currently active piece
        private Point piecePosition; // Current position of the active piece
        private readonly PictureBox pictureBox;
        private readonly Bitmap gameBitmap;
        private Timer gameTimer;
        private bool gameOver = false; // Tracks whether the game is over
        private Form3 form; // Reference to the main form for updating score

        // Piece shapes (I, O, T)
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

        public TetrisGame(PictureBox pictureBox, Form3 form)
        {
            this.pictureBox = pictureBox;
            this.gameBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            this.form = form; // Store reference to the main form
            InitGame();
        }

        private void InitGame()
        {
            board = new int[boardHeight, boardWidth]; // Initialize the game grid
            SpawnNewPiece(); // Spawn the first piece

            // Setup the game timer
            gameTimer = new Timer { Interval = 500 }; // Timer interval in milliseconds
            gameTimer.Tick += (s, e) => MovePiece(0, 1); // Move the piece down every tick
            gameTimer.Start();

            DrawGame(); // Draw the initial game state
        }

        public void HandleKeyDown(Keys keyCode)
        {
            // Handles key presses for controlling the game
            if (gameOver) return; // Do not process input if the game is over

            switch (keyCode)
            {
                case Keys.A:
                    MovePiece(-1, 0); // Move left
                    break;
                case Keys.D:
                    MovePiece(1, 0); // Move right
                    break;
                case Keys.S:
                    MovePiece(0, 1); // Move down faster
                    break;
                case Keys.W:
                    RotatePiece(); // Rotate the piece
                    break;
            }
        }

        private void SpawnNewPiece()
        {
            if (gameOver) return; // Do not spawn a piece if the game is over

            var random = new Random();
            currentPiece = pieces[random.Next(pieces.Length)]; // Select a random piece
            piecePosition = new Point(boardWidth / 2 - currentPiece.GetLength(1) / 2, 0); // Initial position

            // Check if the piece can be placed; if not, game over
            if (!CanPlacePiece(piecePosition.X, piecePosition.Y, currentPiece))
            {
                gameTimer.Stop(); // Stop the game timer
                gameOver = true; // Mark the game as over

                // Save score to file
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scores.log");
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"Score: {form.score}"); // Write score to file
                    Console.WriteLine($"Saved score: {form.score}"); // Debug print
                }

                MessageBox.Show("Game Over!"); // Display game over message
                return;
            }
        }   

            private bool CanPlacePiece(int x, int y, int[,] piece)
        {
            // Checks if a piece can be placed at a specified position
            for (int row = 0; row < piece.GetLength(0); row++)
            {
                for (int col = 0; col < piece.GetLength(1); col++)
                {
                    if (piece[row, col] == 1)
                    {
                        int newX = x + col;
                        int newY = y + row;

                        // Check boundaries
                        if (newX < 0 || newX >= boardWidth || newY >= boardHeight)
                        {
                            return false;
                        }

                        // Check for collisions
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
            // Moves the active piece if possible
            if (gameOver) return; // Do not move if the game is over

            int newX = piecePosition.X + dx;
            int newY = piecePosition.Y + dy;

            if (CanPlacePiece(newX, newY, currentPiece))
            {
                piecePosition.X = newX;
                piecePosition.Y = newY;
                DrawGame(); // Redraw the game
            }
            else if (dy > 0) // If the piece cannot move down
            {
                PlacePiece(); // Place the piece on the board
                ClearFullRows(); // Clear completed rows
                SpawnNewPiece(); // Spawn a new piece
            }
        }

        private void RotatePiece()
        {
            // Rotates the current piece clockwise
            int size = currentPiece.GetLength(0);
            var rotatedPiece = new int[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    rotatedPiece[x, size - 1 - y] = currentPiece[y, x];
                }
            }

            if (CanPlacePiece(piecePosition.X, piecePosition.Y, rotatedPiece))
            {
                currentPiece = rotatedPiece; // Apply the rotation if valid
            }

            DrawGame();
        }

        private void PlacePiece()
        {
            // Locks the current piece onto the game board
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
            int clearedRows = 0; // Count how many rows were cleared

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
                    clearedRows++; // Increment cleared rows count

                    // Shift all rows above downward
                    for (int y = row; y > 0; y--)
                    {
                        for (int col = 0; col < boardWidth; col++)
                        {
                            board[y, col] = board[y - 1, col];
                        }
                    }

                    // Clear the top row
                    for (int col = 0; col < boardWidth; col++)
                    {
                        board[0, col] = 0;
                    }
                }
            }

            if (clearedRows > 0)
            {
                int points = CalculatePoints(clearedRows); // Calculate score based on rows cleared
                form.UpdateScore(points); // Update the score on the main form
            }
        }

        private int CalculatePoints(int rowsCleared)
        {
            // Scoring logic based on the number of rows cleared
            switch (rowsCleared)
            {
                case 1: return 100; // 1 row = 100 points
                case 2: return 300; // 2 rows = 300 points
                case 3: return 500; // 3 rows = 500 points
                case 4: return 800; // 4 rows (Tetris) = 800 points
                default: return 0; // No points
            }
        }

        private void DrawGame()
        {
            // Renders the game state
            using (Graphics g = Graphics.FromImage(gameBitmap))
            {
                g.Clear(Color.Black); // Clear the game area

                // Draw the locked pieces
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

                // Draw the active piece
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

            pictureBox.Image = (Image)gameBitmap.Clone(); // Update the PictureBox
        }
    }
}
