# 🎮 Tetris Game – C# Windows Forms Thesis Project

## 📖 Overview
This project is a **Tetris clone built with C# and Windows Forms** as part of a thesis project.  
It features core Tetris mechanics such as:
- Piece spawning, movement, and rotation
- Line clearing with a scoring system
- Game over detection
- Score saving to a log file (`scores.log`)
- Multiple forms (Form2 for score history, Form3 for the game itself)

The goal of this project is to demonstrate knowledge of **object-oriented programming, UI design in Windows Forms, and basic game development mechanics**.

---

## 🚀 Features
- **Playable Tetris clone** with WASD controls  
- **Scoring system** based on cleared lines:
  - 1 line = 100 points
  - 2 lines = 300 points
  - 3 lines = 500 points
  - 4 lines (Tetris) = 800 points
- **Persistent score saving** into `scores.log`
- **Score history window** (Form2) to display previous results
- **Game Over detection**

---

## 🎮 Controls
| Key | Action |
|-----|--------|
| **W** | Rotate piece |
| **A** | Move left |
| **D** | Move right |
| **S** | Move down faster |

---

## 🛠️ Technologies Used
- **Language:** C#  
- **Framework:** .NET (Windows Forms)  
- **IDE:** Visual Studio

---

## 📂 Project Structure
    /C_sharp_thesis
    │── Form2.cs         # Scoreboard window
    │── Form3.cs         # Game window
    │── TetrisGame.cs    # Game logic and mechanics
    │── scores.log       # Saved score history
    │── README.md        # Project documentation

---

## 📊 Scoring & Persistence
- Scores are updated dynamically during gameplay.  
- At the end of each game, the score is written into `scores.log`.  
- When **Form2** is opened, it reads from `scores.log` and displays all previous scores in a ListBox.

---

## 👨‍💻 Author
Developed by **Julius Tikkanen**
