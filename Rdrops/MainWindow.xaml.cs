using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Rdrops
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(16) };
        DispatcherTimer timer2 = new() { Interval = TimeSpan.FromMilliseconds(400) };
        private readonly List<Circle> circles = new List<Circle>();
        Random r = new();


        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
            timer2.Tick += Timer2_Tick;
            timer.Start();
            timer2.Start();
        }

        private void Timer2_Tick(object? sender, EventArgs e)
        {
            lock (circles)
                circles.Add(new Circle() { center = new System.Drawing.Point((int)(r.NextDouble() * Image.ActualWidth), ((int)(r.NextDouble() * Image.ActualHeight))) });
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap((int)Canvas.ActualWidth, (int)Canvas.ActualHeight);
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(System.Drawing.Color.CornflowerBlue);

                for (int i = 0; i < circles.Count - 1; i++)
                {
                    Circle c = circles[i];
                    if (c.center.X + c.radius > Image.ActualWidth || c.center.X - c.radius < 0 ||
                        c.center.Y + c.radius > Image.ActualHeight || c.center.Y - c.radius < 0)
                    {
                        circles.Remove(c);
                    }
                    else
                    { // new SolidBrush(System.Drawing.Color.DarkBlue) // FILL ELLPISE
                        gfx.DrawEllipse(new System.Drawing.Pen(new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue)), c.center.X, c.center.Y, ++c.radius, c.radius);
                    }
                }
            }

            Image.Source = BmpImageFromBmp(bmp);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lock (circles)
                circles.Add(new Circle() { center = new System.Drawing.Point((int)e.GetPosition(Image).X, (int)e.GetPosition(Image).Y) });
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e) => Render();
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) => Render();

        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void Render()
        {
            using (var bmp = new Bitmap((int)Canvas.ActualWidth, (int)Canvas.ActualHeight))
            using (var gfx = Graphics.FromImage(bmp))
            {

                Image.Source = BmpImageFromBmp(bmp);
            }
        }

        class Circle
        {
            public System.Drawing.Point center;
            public int radius;
        }


    }
}
