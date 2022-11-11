//using LiveCharts;
//using LiveCharts.Defaults;
//using LiveCharts.Wpf;
//<lvc:CartesianChart Series="{Binding SeriesCollection}" LegendLocation="Right" Margin="10,78,250.6,59" Loaded="Graf_Loaded"/>

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;


namespace WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Graphic> Graphics;

        double dx = 30, dy = 30;
        Point StartPoint = new Point(0, 0);//new Point(2 * size_of_marks, canvas1.ActualHeight - 5 * size_of_marks);

        double ScalCoefx = 1, ScalCoefy = 1;

        int cur_graphic = 0;
        public int Cur_graphic { get { return cur_graphic; } set { cur_graphic = value; data_grid1.DataContext = Graphics[cur_graphic].Points; } }
        //ObservableCollection<ObservableCollection<Point>> Grafics;
        //ObservableCollection<Point> Grafic_cur;

        public MainWindow()
        {
            cur_graphic = 0;
            Graphics = new ObservableCollection<Graphic>();
            Graphic def_gr = new Graphic();
            def_gr.Points = new ObservableCollection<point_type>();

            def_gr.Points.Add(new point_type(1, 1));
            def_gr.Points.Add(new point_type(2, 2));
            def_gr.Points.Add(new point_type(3, 3));
            def_gr.Points.Add(new point_type(4, 4));

            def_gr.Name = "default";

            def_gr.IsSelected = 1;

            Graphics.Add(def_gr);

            InitializeComponent();
            data_grid1.DataContext = Graphics[cur_graphic].Points;
            List_Box1.DataContext = Graphics;
            canvas1.Background = Brushes.LightSlateGray;


            //canvas1.Width = 600;
            //canvas1.Height = 360;
            StartPoint = new Point(2 * 5, canvas1.MinHeight - 5 * 5);

            //Draw();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // save
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                var fileStream = saveFileDialog.OpenFile();
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    foreach (var p in Graphics[cur_graphic].Points)
                    {
                        writer.WriteLine(p.X.ToString() + '\t' + p.Y.ToString());
                        
                    }
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e) // load
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    var data = new Graphic();
                    data.Points = new ObservableCollection<point_type>();
                    while (reader.Peek() != -1)
                    {
                        string s = reader.ReadLine();
                        var splitted = s.Split('\t', ' ');
                        if (splitted.Length > 1 && !String.IsNullOrWhiteSpace(s))
                        {
                            data.Points.Add(new point_type(double.Parse(splitted[0]), double.Parse(splitted[1])));
                        }
                    }
                    data.Name = openFileDialog.FileName;
                    data.IsSelected = 1;
                    Graphics.Add(data);
                    Cur_graphic = Graphics.Count - 1;

                }
            }


        }

        private void Button_Click_2(object sender, RoutedEventArgs e) // add point
        {
            double x = Graphics[cur_graphic].Points[Graphics[cur_graphic].Points.Count - 1].X * 1.1;
            Graphics[cur_graphic].Points.Add(new point_type(x, x));

            Draw();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // delete point
        {
            int cur_point = data_grid1.SelectedIndex;
            if (cur_point == -1)
            {
                cur_point = 0;
            }
            Graphics[cur_graphic].Points.RemoveAt(cur_point);

        }

        private void Draw()
        {
            canvas1.Children.Clear();

            //Line l1 = new Line();
            //l1.X1 = 100;
            //l1.Y1 = 100;
            //l1.X2 = 300;
            //l1.Y2 = 300;
            //l1.StrokeThickness = 5;
            //l1.Stroke = Brushes.Black;
            //
            //canvas1.Children.Add(l1);

            // draw ox
            double size_of_marks = 5;
            double margin = 2 * size_of_marks;
            StartPoint.X = 300;

            double rightBord = canvas1.ActualWidth - 3 * margin;
            double downBoard = canvas1.ActualHeight - 3 * margin;
            double lxGlobal = StartPoint.X < rightBord ? StartPoint.X : rightBord;
            double lyGlobal = StartPoint.Y < downBoard ? StartPoint.Y : downBoard;

            Line l = new Line();
            l.X1 = lxGlobal;
            l.Y1 = lyGlobal;
            l.X2 = canvas1.ActualWidth - margin;
            l.Y2 = lyGlobal;
            l.StrokeThickness = 1;
            l.Stroke = Brushes.Black;
            canvas1.Children.Add(l);

            double value1 = 0;
            TextBlock text1 = new TextBlock();
            text1.Text = value1.ToString();
            Canvas.SetLeft(text1, lxGlobal + margin / 2);
            Canvas.SetTop(text1, lyGlobal + margin / 2);
            canvas1.Children.Add(text1);

            // по х в положительном направлении
            for (double xi = lxGlobal, i = 0; xi <= canvas1.ActualWidth - margin /*|| StartPoint.X - xi - 5 > 0*/; xi += dx, i++)
            {
                Line px_i = new Line();
                px_i.X1 = xi;
                px_i.Y1 = lyGlobal + size_of_marks;
                px_i.X2 = xi;
                px_i.Y2 = lyGlobal - size_of_marks;
                px_i.StrokeThickness = 1;
                px_i.Stroke = Brushes.Black;
                canvas1.Children.Add(px_i);

                if (i != 0)
                {
                    double value = i * ScalCoefx;
                    TextBlock text = new TextBlock();
                    text.Text = value.ToString();
                    Canvas.SetLeft(text, xi - size_of_marks);
                    Canvas.SetTop(text, lyGlobal + size_of_marks);
                    canvas1.Children.Add(text);
                }
            }


            // по х в отрицательном направлении

            

            Line l2 = new Line();
            l2.X1 = margin;
            l2.Y1 = lyGlobal;
            l2.X2 = lxGlobal;
            l2.Y2 = lyGlobal;
            l2.StrokeThickness = 1;
            l2.Stroke = Brushes.Black;
            canvas1.Children.Add(l2);

            
            // по х в положительном направлении
            for (double xi = l2.X2, i = 0; xi > margin; xi -= dx, i++)
            {
                Line px_i = new Line();
                px_i.X1 = xi;
                px_i.Y1 = lyGlobal + size_of_marks;
                px_i.X2 = xi;
                px_i.Y2 = lyGlobal - size_of_marks;
                px_i.StrokeThickness = 1;
                px_i.Stroke = Brushes.Black;
                canvas1.Children.Add(px_i);

                if (i != 0)
                {
                    double value = -i * ScalCoefx;
                    TextBlock text = new TextBlock();
                    text.Text = value.ToString();
                    Canvas.SetLeft(text, xi - size_of_marks);
                    Canvas.SetTop(text, StartPoint.Y + size_of_marks);

                    canvas1.Children.Add(text);
                }

            }



            // draw oy

            //StartPoint.X += 30;


            Line l1 = new Line();
            l1.X1 = lxGlobal;
            l1.Y1 = lyGlobal;
            l1.X2 = lxGlobal; /*canvas1.ActualHeight - 2 * size_of_marks;*/
            l1.Y2 = 2 * size_of_marks; /*canvas1.ActualHeight;*/
            l1.StrokeThickness = 1;
            l1.Stroke = Brushes.Black;
            canvas1.Children.Add(l1);

            // по y в положительном направлении

            for (double yi = lyGlobal, i = 0; yi >= margin /*|| StartPoint.Y - yi - 5 > 0*/; yi -= dy, i++)
            {
                Line px_i = new Line();
                px_i.X1 = lxGlobal - size_of_marks;
                px_i.Y1 = yi;
                px_i.X2 = lxGlobal + size_of_marks;
                px_i.Y2 = yi;
                px_i.StrokeThickness = 1;
                px_i.Stroke = Brushes.Black;
                canvas1.Children.Add(px_i);
                if (i != 0)
                {
                    double value = i * ScalCoefx;
                    TextBlock text = new TextBlock();
                    text.Text = value.ToString();
                    Canvas.SetBottom(text, canvas1.ActualHeight - yi - size_of_marks);
                    Canvas.SetLeft(text, lxGlobal + margin);

                    canvas1.Children.Add(text);
                }
            }



            // по y в отрицательном направлении


            Line l3 = new Line();
            l3.X1 = lxGlobal;
            l3.Y1 = lyGlobal;
            l3.X2 = lxGlobal; /*canvas1.ActualHeight - 2 * size_of_marks;*/
            l3.Y2 = canvas1.ActualHeight - margin; /*canvas1.ActualHeight;*/
            l3.StrokeThickness = 1;
            l3.Stroke = Brushes.Black;
            canvas1.Children.Add(l3);

            for (double yi = lyGlobal, i = 0; yi <= canvas1.ActualHeight - margin /*|| StartPoint.Y - yi - 5 > 0*/; yi += dy, i++)
            {
                Line px_i = new Line();
                px_i.X1 = lxGlobal - size_of_marks;
                px_i.Y1 = yi;
                px_i.X2 = lxGlobal + size_of_marks;
                px_i.Y2 = yi;
                px_i.StrokeThickness = 1;
                px_i.Stroke = Brushes.Black;
                canvas1.Children.Add(px_i);
                if (i != 0)
                {
                    double value = -i * ScalCoefx;
                    TextBlock text = new TextBlock();
                    text.Text = value.ToString();
                    Canvas.SetBottom(text, canvas1.ActualHeight - yi - size_of_marks);
                    Canvas.SetLeft(text, lxGlobal + margin);

                    canvas1.Children.Add(text);
                }
            }

            // draw graphic
            for (int i = 0; i < Graphics.Count; i++)
            {
                Polyline line = new Polyline();
                line.StrokeThickness = 2;
                line.Stroke = Brushes.Red;
                for (int j = 0; j < Graphics[i].Points.Count; j++)
                {
                    Point p = new Point(Graphics[i].Points[j].X, Graphics[i].Points[j].Y);
              
                    p.X *= ScalCoefx* dx;
                    p.Y *= ScalCoefy* dy;
                    p.X += StartPoint.X;
                    p.Y = StartPoint.Y - p.Y;

                    line.Points.Add(p);
                }
                canvas1.Children.Add(line);
            }
        }

        /*public SeriesCollection SeriesCollection { get; set; }

        public string[] Labels { get; set; }

        public Func<double, string> YFormatter { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SeriesCollection = new SeriesCollection
         {
            new LineSeries
            {
                  Values = new ChartValues<ObservablePoint>
                  {
                     new ObservablePoint(0,10),
                     new ObservablePoint(2,11),
                     new ObservablePoint(3,12),
                     new ObservablePoint(4,13),
                     new ObservablePoint(6,14)
                  },
                  Title = "graph1",
                  Fill = Brushes.Transparent,
                  PointGeometrySize = 10,
                  PointGeometry = DefaultGeometries.Square
            },


            new LineSeries
            {
                  Values = new ChartValues<ObservablePoint>
                  {
                     new ObservablePoint(0,5),
                     new ObservablePoint(2,5),
                     new ObservablePoint(3,3),
                     new ObservablePoint(4,15),
                     new ObservablePoint(6,20)
                  },
                  Title = "graph2",
                  Stroke = Brushes.Red,
                  Fill = Brushes.Transparent,
                  PointGeometrySize = 10,

            }



         };

            YFormatter = value => value.ToString("C");
            DataContext = this;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы (* .txt)| *.txt|Все файлы (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            ofd.ShowDialog();
            if (ofd.FileName != "") //Проверка на выбор файла
            {
                using (var reader = File.OpenText(ofd.FileName))
                {
                    //Пропусть первую строку в файле
                    // if (!reader.EndOfStream)
                    // reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();


                    }

                }
            }
            else MessageBox.Show("Файл не найден");
        }

        private void Graf_Loaded(object sender, RoutedEventArgs e)
        {

        }*/




    }

    public class point_type : INotifyPropertyChanged
    {
        public double X { get; set; }
        public double Y { get; set; }


        public point_type(double x, double y)
        {
            X = x;
            Y = y;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Graphic : INotifyPropertyChanged
    {
        ObservableCollection<point_type> _points;
        public ObservableCollection<point_type> Points 
        {
            get { return _points; }
            set
            {
                _points = value;
                OnPropertyChanged();
            }
        }
        public string Name { get; set; }
        public int IsSelected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

