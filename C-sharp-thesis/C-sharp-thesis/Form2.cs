using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace C_sharp_thesis
{
    public partial class Form2 : Form
    {
        private ListBox scoreListBox;

        public Form2()
        {
            InitializeComponent();
            InitializeScoreList(); // Call the method to initialize the score list
        }

        private void InitializeScoreList()
        {
            // Create a ListBox to display the scores
            scoreListBox = new ListBox()
            {
                Location = new Point(270, 130),
                Size = new Size(200, 400)
            };

            // Define the file path
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scores.log");

            // Check if the file exists
            if (File.Exists(filePath))
            {
                string[] scores = File.ReadAllLines(filePath); // Read all lines from the file
                foreach (string score in scores)
                {
                    scoreListBox.Items.Add(score); // Add scores to the ListBox
                }
            }
            else
            {
                scoreListBox.Items.Add("No scores yet!"); // Display a message if no scores are available
            }

            // Add the ListBox to the form
            Controls.Add(scoreListBox);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // Close Form2
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
 
        }
    }
}
