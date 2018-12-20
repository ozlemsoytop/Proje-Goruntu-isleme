using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using Point = System.Drawing.Point;
using AForge.Math.Geometry;
using System.IO.Ports;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private FilterInfoCollection VideoCapTureDevices;
        private VideoCaptureDevice Finalvideo;

        int R;
        int G;
        int B;
        int pin;
        private void Form1_Load(object sender, EventArgs e)
        {
            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.DataSource = SerialPort.GetPortNames();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 20;//saniyede kaç görüntü alsın istiyorsanız. FPS
            Finalvideo.DesiredFrameSize = new Size(360, 360);//görüntü boyutları
            Finalvideo.Start();
        }
        private void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            image1.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox1.Image = image;

            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.CenterColor = new RGB(Color.FromArgb(R, G, B));
            filter.Radius = 100;
            filter.ApplyInPlace(image1);
            nesnebul(image1);
        }
        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            // grayscaling
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            image.UnlockBits(objectsData);
            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;

            foreach (Rectangle recs in rects)
            {
                Rectangle objectRect = rects[0];
                Graphics g = pictureBox1.CreateGraphics();
                using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                {
                    g.DrawRectangle(pen, objectRect);
                }
                int objectX = objectRect.X + (objectRect.Width / 2);
                int objectY = objectRect.Y + (objectRect.Height / 2);
                g.Dispose();

                if (objectX > 0 && objectX < 120 )
                  if ( objectY < 120)  
                {
                    pin = 1;
                }
                if (objectX > 120 && objectX < 240 )
                    if (objectY < 120)
                {
                        pin = 2;
                }
                if (objectX > 240 && objectX < 360 )
                    if (objectY < 120)
                { 
                    pin = 3;
                }
                if (objectX > 0 && objectX < 120 )
                    if (objectY > 120 && objectY < 240)
                {
                    pin = 4;
                }
                if (objectX > 120 && objectX < 240 )
                   if (objectY > 120 && objectY < 240)
                {
                    pin = 5;
                }
                if (objectX > 240 && objectX < 360 )
                    if (objectY > 120 && objectY < 240)
                {
                    pin = 6;
                }
                if (objectX > 0 && objectX < 120 )
                  if ( objectY > 240 && objectY < 360)
                {
                    pin = 7;
                }
                if (objectX > 120 && objectX < 240)
                    if (objectY > 240 && objectY < 360)
                {
                    pin = 8;
                }
                if (objectX > 240 && objectX < 360 )
                    if (objectY > 240 && objectY < 360)
                {
                    pin = 9;
                }
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write(pin.ToString());

                }
            }
        }
            private void trackBar1_Scroll(object sender, EventArgs e)
            {
                R = trackBar1.Value;
            }

            private void trackBar2_Scroll(object sender, EventArgs e)
            {
                G = trackBar2.Value;
            }

            private void trackBar3_Scroll(object sender, EventArgs e)
            {
                B = trackBar3.Value;
            }

            private void button2_Click(object sender, EventArgs e)
            {
                if (Finalvideo.IsRunning)
                {
                    Finalvideo.Stop();

                }
                Application.Exit();
            }

            private void button3_Click(object sender, EventArgs e)
            {
                serialPort1.BaudRate = 9600;
                serialPort1.PortName = comboBox2.SelectedItem.ToString();
                serialPort1.Open();

            }

            private void button4_Click(object sender, EventArgs e)
            {
                serialPort1.Close();
            }
        
    }
}
        
       