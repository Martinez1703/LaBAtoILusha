using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        public int xn, yn, xk, yk;
        public List<Point> points = new List<Point>();
        Bitmap myBitmap; // объект Bitmap для вывода отрезка
        Color currentBorderColor; // текущий цвет отрезка
        Color fillColor; // цвет заливки

        public Form1()
        {
            InitializeComponent();
            fillColor = Color.White; // цвет заливки по умолчанию
            currentBorderColor = Color.Red; // начальный цвет отрезка
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Reset the bitmap and clear points
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Color newPixelColor = Color.FromArgb(247, 249, 239);
            for (int x = 0; x < myBitmap.Width; x++)
            {
                for (int y = 0; y < myBitmap.Height; y++)
                {
                    myBitmap.SetPixel(x, y, newPixelColor);
                }
            }
            pictureBox1.Image = myBitmap;
            points.Clear();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked || radioButton3.Checked)
            {
                xn = e.X;
                yn = e.Y;
                points.Add(new Point(xn, yn));
            }
            else
            {
                if (!radioButton1.Checked && !radioButton2.Checked && !radioButton3.Checked)
                {
                    MessageBox.Show("Вы не выбрали алгоритм отрисовки");
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked || radioButton3.Checked)
            {
                xk = e.X;
                yk = e.Y;
                points.Add(new Point(xk, yk));
                DrawLine(xn, yn, xk, yk); // Используем DrawLine для рисования линии
                xn = xk; // обновление стартовой точки на следующей линии
                yn = yk;
            }
        }

        private void DrawLine(int xStart, int yStart, int xEnd, int yEnd)
        {
            int Masss = int.Parse(numericUpDown1.Text);
            if (myBitmap == null) return;

            using (Graphics g = Graphics.FromImage(myBitmap))
            {
                Pen myPen = new Pen(currentBorderColor, Masss);
                g.DrawLine(myPen, xStart, yStart, xEnd, yEnd);
            }
            pictureBox1.Image = myBitmap;
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromHwnd(pictureBox1.Handle))
            {
                if (radioButton1.Checked)
                {
                    //рисуем прямоугольник
                    DrawLine(10, 10, 10, 110);
                    DrawLine(10, 10, 110, 10);
                    DrawLine(10, 110, 110, 110);
                    DrawLine(110, 10, 110, 110);
                    //рисуем треугольник
                    DrawLine(150, 10, 150, 200);
                    DrawLine(250, 50, 150, 200);
                    DrawLine(150, 10, 250, 150);
                }
                else if (radioButton2.Checked)
                {
                    //получаем растр созданного рисунка в mybitmap
                    myBitmap = pictureBox1.Image as Bitmap;

                    // задаем координаты затравки
                    xn = 160;
                    yn = 40;
                    // вызываем рекурсивную процедуру заливки с затравкой
                    FloodFill(xn, yn);
                }
            }
        }

        private void PictureBox1_MouseDownFill(object sender, MouseEventArgs e)
        {
            FloodFill(e.X, e.Y);
            pictureBox1.MouseDown -= PictureBox1_MouseDownFill;
        }

        private bool IsClosedContour()
        {
            if (points.Count < 3) // проверка полигона
                return false;

            Point firstPoint = points[0];
            Point lastPoint = points[points.Count - 1];

            //проверка последней точки
            return Math.Abs(firstPoint.X - lastPoint.X) < 9 && Math.Abs(firstPoint.Y - lastPoint.Y) < 9;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (IsClosedContour())
            {
                pictureBox1.MouseDown += PictureBox1_MouseDownFill;
                MessageBox.Show("Контур замкнут");
            }
            else
            {
                MessageBox.Show("Контур не замкнут!");
            }
        }

        private void FloodFill(int x, int y)
        {
            Color targetColor = myBitmap.GetPixel(x, y);
            if (targetColor.ToArgb() == fillColor.ToArgb()) return; // Если цвет уже заполнен, выходим

            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(new Point(x, y));

            while (pixels.Count > 0)
            {
                Point pt = pixels.Pop();

                // Проверка границ изображения
                if (pt.X < 0 || pt.X >= myBitmap.Width || pt.Y < 0 || pt.Y >= myBitmap.Height)
                    continue;

                // Пропустить, если цвет текущего пикселя не совпадает с целевым цветом
                if (myBitmap.GetPixel(pt.X, pt.Y).ToArgb() != targetColor.ToArgb())
                    continue;

                // Установить цвет пикселя на цвет заливки
                myBitmap.SetPixel(pt.X, pt.Y, fillColor);

                // Добавить соседние пиксели в стек
                pixels.Push(new Point(pt.X + 1, pt.Y)); // Right
                pixels.Push(new Point(pt.X - 1, pt.Y)); // Left
                pixels.Push(new Point(pt.X, pt.Y + 1)); // Down
                pixels.Push(new Point(pt.X, pt.Y - 1)); // Up
            }

            // Обновить pictureBox для отображения залитой области
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentBorderColor = colorDialog.Color; // Устанавливаем текущий цвет границы
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                fillColor = colorDialog.Color; // Сохраняем выбранный цвет заливки
            }
        }
    }
}
    