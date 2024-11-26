using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProcess2;

namespace CoinsActivity
{
    public partial class Form1 : Form
    {
        // Variables for counting different types of coins
        int fiveCentCoins = 0, tenCentCoins = 0, twentyFiveCentCoins = 0, onePesoCoins = 0, fivePesoCoins = 0;
        int coinCount = 0;
        Bitmap origImage;
        Bitmap coinImage;

        public Form1()
        {
            InitializeComponent();
            origImage = (Bitmap)pictureBox1.Image;
            initialize();
            this.Text = "Coins Activity";
        }

        private void initialize()
        {
            coinImage = (Bitmap)origImage.Clone();
            coinCount = 0;
            fiveCentCoins = 0;
            tenCentCoins = 0;
            twentyFiveCentCoins = 0;
            onePesoCoins = 0;
            fivePesoCoins = 0;
            numberOfCoins.Text = "";
        }

        // This function categorizes coins based on their pixel count (size)
        private void checkSizes(int pixelCount)
        {
            // Check for different coin sizes and categorize them
            if (pixelCount > 2000 && pixelCount < 3000)
            {
                fiveCentCoins++;
                coinCount++;
            }
            else if (pixelCount > 3000 && pixelCount < 4000 )
            {
                tenCentCoins++;
                coinCount++;
            }
            else if (pixelCount >= 4000 && pixelCount < 5000)
            {
                twentyFiveCentCoins++;
                coinCount++;
            }
            else if (pixelCount >= 5000 && pixelCount < 7000)
            {
                onePesoCoins++;
                coinCount++;
            }
            else if (pixelCount >= 7000)
            {
                fivePesoCoins++;
                coinCount++;
            }
        }

        // Check if the pixel is part of the coin (not white)
        private bool checkColor(Color pixelImage)
        {
            int red = pixelImage.R;
            int blue = pixelImage.B;
            int green = pixelImage.G;

            return red < 200 && blue < 200 && green < 200;
        }

        private async Task<int> PerformDFS(int startX, int startY, bool[,] visited, Bitmap coinImage)
        {
            // Directions: Left, Right, Up, Down
            int[] directionsX = { -1, 1, 0, 0 };  
            int[] directionsY = { 0, 0, -1, 1 }; 

            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(startX, startY));  
            visited[startY, startX] = true;  

            int pixelCount = 0;

            while (stack.Count > 0)
            {
                Point current = stack.Pop();
                int x = current.X;
                int y = current.Y;

                pixelCount++;

                for (int i = 0; i < 4; i++)  // Iterate through the 4 directions
                {
                    int newX = x + directionsX[i];
                    int newY = y + directionsY[i];

                    if (newX >= 0 && newX < coinImage.Width && newY >= 0 && newY < coinImage.Height)
                    {
                        Color neighborColor = coinImage.GetPixel(newX, newY);

                        if (!visited[newY, newX] && checkColor(neighborColor))
                        {
                            visited[newY, newX] = true;
                            coinImage.SetPixel(newX, newY,Color.Green);
                            stack.Push(new Point(newX, newY));
                        }
                    }
                }
            }
            //MessageBox.Show($"Pixel size {pixelCount}");
            pictureBox1.Image = coinImage;
            //await Task.Delay(100);
            return pixelCount;
        }


        private void pictureBox1_Click( object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            initialize();
            numberOfCoins.Text = "Converting image to grayscale...";

            BitmapFilter.GrayScale(coinImage);
            pictureBox1.Image = coinImage;

            bool[,] visited = new bool[coinImage.Height, coinImage.Width];

            numberOfCoins.Text = "Detecting Coins...";
            for (int y = 0; y < coinImage.Height; y++)              {
                for (int x = 0; x < coinImage.Width; x++)                  {
                    Color pixelColor = coinImage.GetPixel(x, y);

                    if (!visited[y, x] && checkColor(pixelColor))
                    {
                        int coinPixelCount = await PerformDFS(x, y, visited, coinImage);


                        checkSizes(coinPixelCount);
                    }
                }
            }
            int totalSum = (fiveCentCoins * 5) + (tenCentCoins * 10) + (twentyFiveCentCoins * 25) +
                           (onePesoCoins * 100) + (fivePesoCoins * 500);

            MessageBox.Show($"Total coins detected: {coinCount}", "Coin Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
            numberOfCoins.Text = $"Five Centavos: {fiveCentCoins}{Environment.NewLine}" +
                                 $"Ten Centavos: {tenCentCoins}{Environment.NewLine}" +
                                 $"Twenty-Five Centavos: {twentyFiveCentCoins}{Environment.NewLine}" +
                                 $"One peso: {onePesoCoins}{Environment.NewLine}" +
                                 $"Five pesos: {fivePesoCoins}{Environment.NewLine}" +
                                 $"Total Coins Detected = {coinCount}{Environment.NewLine}"+
                                 $"Total Sum: {totalSum/100.0:C2}";

        }

        private void numberOfCoins_TextChanged(object sender, EventArgs e)
        {
            // Optional: Implement if you want to show coin counts in a text box or handle user input
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set the form's start position to manual
            this.StartPosition = FormStartPosition.Manual;

            // Position the form at the top-right corner of the screen
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            this.Location = new Point(screenWidth - this.Width, 0);
        }
    }
}
