using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlineSelection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private Bitmap OutlineSelection( Bitmap Im)// Метод выделения контуров
        {
            int h = Im.Height, w = Im.Width;
            progressBar1.Visible = true;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = h * w;
            progressBar1.Step = 1;
            for (int i = 0; i < w - 2; i++)// проход по изображению и наложение маски
                for (int j = 0; j < h - 2; j++)
                {
                    float Gx = 0, Gy = 0;
                    if (radioButton1.Checked)
                    {// Градиенты x и y по оператору Прюитт
                        Gx = -Im.GetPixel(i, j).GetBrightness() - Im.GetPixel(i + 1, j).GetBrightness() - Im.GetPixel(i + 2, j).GetBrightness() +
                           Im.GetPixel(i, j + 2).GetBrightness() + Im.GetPixel(i + 1, j + 2).GetBrightness() + Im.GetPixel(i + 2, j + 2).GetBrightness();
                        Gy = -Im.GetPixel(i, j).GetBrightness() - Im.GetPixel(i, j + 1).GetBrightness() - Im.GetPixel(i, j + 2).GetBrightness() +
                           Im.GetPixel(i, j + 2).GetBrightness() + Im.GetPixel(i + 1, j + 2).GetBrightness() + Im.GetPixel(i + 2, j + 2).GetBrightness();
                    }
                    else if (radioButton2.Checked)
                    {
                        // Градиенты x и y по оператору Собеля
                        Gx = -Im.GetPixel(i, j).GetBrightness() - 2 * Im.GetPixel(i + 1, j).GetBrightness() - Im.GetPixel(i + 2, j).GetBrightness() +
                           Im.GetPixel(i, j + 2).GetBrightness() + 2 * Im.GetPixel(i + 1, j + 2).GetBrightness() + Im.GetPixel(i + 2, j + 2).GetBrightness();
                        Gy = -Im.GetPixel(i, j).GetBrightness() - 2 * Im.GetPixel(i, j + 1).GetBrightness() - Im.GetPixel(i, j + 2).GetBrightness() +
                           Im.GetPixel(i, j + 2).GetBrightness() + 2 * Im.GetPixel(i + 1, j + 2).GetBrightness() + Im.GetPixel(i + 2, j + 2).GetBrightness();
                    }

                    else if (radioButton3.Checked)
                    {// Градиенты x и y по оператору Шарра
                        Gx = -3 * Im.GetPixel(i, j).GetBrightness() - 10 * Im.GetPixel(i + 1, j).GetBrightness() - 3 * Im.GetPixel(i + 2, j).GetBrightness() +
                          3 * Im.GetPixel(i, j + 2).GetBrightness() + 10 * Im.GetPixel(i + 1, j + 2).GetBrightness() + 3 * Im.GetPixel(i + 2, j + 2).GetBrightness();
                        Gy = -3 * Im.GetPixel(i, j).GetBrightness() - 10 * Im.GetPixel(i, j + 1).GetBrightness() - 3 * Im.GetPixel(i, j + 2).GetBrightness() +
                           3 * Im.GetPixel(i, j + 2).GetBrightness() + 10 * Im.GetPixel(i + 1, j + 2).GetBrightness() + 3 * Im.GetPixel(i + 2, j + 2).GetBrightness();
                    }
                    
                    if (radioButton4.Checked)// пороговая фильтрация
                    {
                        double border = trackBar1.Value/10000;
                        int G = Convert.ToInt32(Math.Sqrt(Math.Pow(Gx, 2) + Math.Pow(Gy, 2)));
                        if (G > border)
                            Im.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        else Im.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                    else if (radioButton5.Checked)// вычисление нового цвета на основе полученного градиента
                    {
                        double multiplier = trackBar1.Value;
                        int G = Convert.ToInt32(Math.Sqrt(Math.Pow(Gx, 2) + Math.Pow(Gy, 2))*multiplier)%255;
                        Im.SetPixel(i, j, Color.FromArgb(G, G, G));//установка нового цвета пикселя
                    }
                    progressBar1.PerformStep();
                }
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            return Im;
        }

        private void UIMethodChange() {//изменение значений ползунка для регулирования заданного метода
            if (radioButton4.Checked)
            {
                textBox1.Text = "0";
                textBox3.Text = "1";
                trackBar1.Minimum = 0;
                trackBar1.Maximum = 10000;
                trackBar1.TickFrequency = 1250;
            }
            else if (radioButton5.Checked)
            {
                textBox1.Text = "0";
                textBox3.Text = "256";
                trackBar1.Maximum = 256;
                trackBar1.Minimum = 0;
                trackBar1.TickFrequency = 16;
            }
        }

        private void Form1_Load(object sender, EventArgs e)//Установка заглушки при загрузке формы
        {
            ImBox.Image= Image.FromFile("C:\\PracticePics\\Stub.jpg");
            UIMethodChange();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)//Загрузка изображения
        {
            openFileDialog1.Filter = "Jpg|*.jpg|Jpeg|*.jpeg";
           if ( openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImBox.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)//Вызов метода выделения контуров
        {
            ImBox.Image = OutlineSelection((ImBox.Image as Bitmap));
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)//Сохранение изображения
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                ImBox.Image.Save(saveFileDialog1.FileName);
        }

        private void выйтиToolStripMenuItem_Click(object sender, EventArgs e)// выход
        {
            this.Close();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)// обновление интерфейса при смене метода
        {
            UIMethodChange();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)// обновление интерфейса при смене метода
        {
            UIMethodChange();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)// отображение текущего выбранного на ползунке значения
        {
            if (radioButton4.Checked)
                textBox2.Text = Convert.ToString(Convert.ToDouble(trackBar1.Value) / 10000);
            else textBox2.Text = Convert.ToString(trackBar1.Value);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)// ввод значения пользователем
        {
            if (textBox2.Text != "")
            {
                if (radioButton4.Checked)
                {
                    if (Convert.ToInt32(Convert.ToDouble(textBox2.Text) * 10000) <= trackBar1.Maximum)
                        trackBar1.Value = Convert.ToInt32(Convert.ToDouble(textBox2.Text) * 10000);
                }
                else if (radioButton5.Checked)
                        if (Convert.ToInt32(textBox2.Text) <= trackBar1.Maximum)
                            trackBar1.Value = Convert.ToInt32(textBox2.Text);
            }
        }
    }
}
