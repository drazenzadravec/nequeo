using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Drawing.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Rectangle rectangle;
            RectangleF rectangleF;

            Nequeo.Drawing.Image image = new Nequeo.Drawing.Image();
            Bitmap bitmap = image.Create(300, 300);
            Graphics graphics = image.CreateGraphics(ref bitmap, out rectangle, out rectangleF);

            // Format the string in the center of the image.
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            //stringFormat.LineAlignment = StringAlignment.Center;

            FontFamily fontFamily = new System.Drawing.FontFamily("Times New Roman");
            GraphicsPath graphicsPath = image.CreateText(ref graphics, "Drazen Zadravec", 50,50,100, 50, stringFormat, fontFamily);
            
            Brush brushBack = new SolidBrush(Color.White);
            Brush brush = new SolidBrush(Color.Black);

            graphics.FillRectangle(brushBack, rectangleF);
            graphics.FillPath(brush, graphicsPath);

            ImageFormat imageFormat = ImageFormat.Png;

            System.IO.Stream file = System.IO.File.Create(@"c:\temp\image\text.png");
            image.Save(bitmap, file, imageFormat);
            file.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Nequeo.Drawing.Captcha captcha = new Nequeo.Drawing.Captcha();
            FontFamily fontFamily = new System.Drawing.FontFamily("Times New Roman");
            Bitmap bitmap = captcha.GenerateImage("Drazen Zadravec", 300, 300, fontFamily);
            bitmap.Save(@"c:\temp\image\Captcha.png", ImageFormat.Png);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Nequeo.Drawing.Image image = new Nequeo.Drawing.Image();
            System.Drawing.Bitmap file = image.CaptureScreen();
            System.IO.FileStream stream = System.IO.File.Create(@"C:\Temp\Capture\image.jpg");
            image.Save(file, stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            stream.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Nequeo.Drawing.Pdf.Document doc = new Nequeo.Drawing.Pdf.Document();
            System.IO.FileStream stream = System.IO.File.Open(@"C:\Temp\Examples.pdf", System.IO.FileMode.Open);
            System.IO.StreamWriter extract = new System.IO.StreamWriter(@"C:\Temp\Examples_Extract.txt");
            StringBuilder text = doc.ExtractText(stream, encoding: Nequeo.Text.EncodingType.ASCII);
            extract.Write(text.ToString());
            stream.Close();
            extract.Close();
        }
    }
}
