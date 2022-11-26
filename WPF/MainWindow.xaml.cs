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
using System.Globalization;
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
        List<Brush> ColorList;
        List<string> ColorsList;
        axis Axis;

        point_type StartPoint = new point_type(300, 250);

        int cur_graphic = 0;
        public int Cur_graphic
        {
            get
            { return cur_graphic; } 
            set 
            { 
                cur_graphic = value; 
                data_grid1.DataContext = Graphics[cur_graphic].Points; 
                ComboBox1.SelectedIndex = Graphics[cur_graphic].Color; 
            } 
        }

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
            def_gr.Color = 0;
            Graphics.Add(def_gr);

            ColorsList = new List<string>();
            ColorsList.Add("Azure");
            ColorsList.Add("DarkSalmon");
            ColorsList.Add("Khaki");
            ColorsList.Add("MediumTurquoise");
            ColorsList.Add("PaleVioletRed");

            ColorList = new List<Brush>();
            ColorList.Add(Brushes.Azure);
            ColorList.Add(Brushes.DarkSalmon);
            ColorList.Add(Brushes.Khaki);
            ColorList.Add(Brushes.MediumTurquoise);
            ColorList.Add(Brushes.PaleVioletRed);

            Axis = new axis();

            InitializeComponent();
            List_Box1.DataContext = Graphics;
            ComboBox1.DataContext = ColorsList;
            ComboBox1.SelectedIndex = Graphics[cur_graphic].Color;
            data_grid1.DataContext = Graphics[cur_graphic].Points;

            canvas1.Background = Brushes.LightSlateGray;
            canvas1.Focus();
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
                    data.Name = openFileDialog.FileName.Split('\\').Last();
                    data.IsSelected = 1;
                    data.Color = (Graphics.Last().Color + 1) % 5;
                    // цвет
                    Graphics.Add(data);
                    Cur_graphic = Graphics.Count - 1;

                }
            }

            Draw();

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

        private void List_Box1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cur_graphic = (sender as ListBox).SelectedIndex;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem i)
                i.IsSelected = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Graphics[cur_graphic].Color = (sender as ComboBox).SelectedIndex;
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
            Draw();
        }

        private void canvas1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Left: 
                    StartPoint.X -= 10; 
                    break;
                case Key.Right:
                    StartPoint.X += 10;
                    break;
                case Key.Up:
                    StartPoint.Y -= 10;
                    break;
                case Key.Down:
                    StartPoint.Y += 10;
                    break;
                case Key.PageUp:
                    Axis.Cur_step_x += 5;
                    Axis.Cur_step_y += 5;
                    break;
                case Key.PageDown:
                    Axis.Cur_step_x -= 5;
                    Axis.Cur_step_y -= 5;
                    break;
                default: 
                    return;
            }
            Draw();
            e.Handled = true;
        }

        private void canvas1_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Canvas).Focus();
        }

        private Point WindowPoint(double x_real, double y_real, double ScalCoefx, double ScalCoefy, double dx, double dy)
        {
            Point res = new Point(x_real, y_real);
            res.X *= ScalCoefx;
            res.Y *= ScalCoefy;
            res.X = res.X + dx;
            res.Y = -(res.Y + dy);
            return res;
        }
        private void Draw()
        {
            canvas1.Children.Clear();
            
            double margin = 30;      
            
            double rightBoard = canvas1.ActualWidth - margin;
            double downBoard = canvas1.ActualHeight - margin;
            double lxGlobal = StartPoint.X;
            double lyGlobal = StartPoint.Y;
            if (StartPoint.X > rightBoard)
                lxGlobal = rightBoard;
            else if (StartPoint.X < margin)
                lxGlobal = margin;

            if (StartPoint.Y > downBoard)
                lyGlobal = downBoard;
            else if (StartPoint.Y < margin)
                lyGlobal = margin;

            // draw ox
            point_type min_x = new point_type(margin, lyGlobal);
            point_type max_x = new point_type(rightBoard, lyGlobal);
            Axis.PrepareAxisX(min_x, max_x, StartPoint);
            canvas1.Children.Add(Axis.Axis_line_x);
            foreach(var el in Axis.Tic_lables_x)
            {
                canvas1.Children.Add(el);
            }
            foreach (var el in Axis.Tic_lines_x)
            {
                canvas1.Children.Add(el);
            }

            // draw oy
            point_type min_y = new point_type(lxGlobal, margin);
            point_type max_y = new point_type(lxGlobal, downBoard);
            Axis.PrepareAxisY(min_y, max_y, StartPoint);
            canvas1.Children.Add(Axis.Axis_line_y);
            foreach (var el in Axis.Tic_lables_y)
            {
                canvas1.Children.Add(el);
            }
            foreach (var el in Axis.Tic_lines_y)
            {
                canvas1.Children.Add(el);
            }

            // draw graphic
            double ScalCoefx = Axis.Scale_x, ScalCoefy = Axis.Scale_y;
            double x_min_real = (margin - StartPoint.X) / ScalCoefx;
            double x_max_real = (rightBoard - StartPoint.X) / ScalCoefx;
            double y_min_real = -(downBoard - StartPoint.Y) / ScalCoefy;
            double y_max_real = -(margin - StartPoint.Y) / ScalCoefy;
            int skip_point; // -1 no point, 0 point was add, 1 left, 2 right, 3 top, 4 bottom
            for (int i = 0; i < Graphics.Count; i++)
            {
                if (Graphics[i].IsSelected == 1)
                {
                    skip_point = -1;
                    Point p_pre = new Point();
                    Polyline line = new Polyline();
                    line.StrokeThickness = 2;
                    // цвет
                    line.Stroke = ColorList[Graphics[i].Color];
                    for (int j = 0; j < Graphics[i].Points.Count; j++)
                    {
                        if (Graphics[i].Points[j].X < x_min_real)
                        {
                            skip_point = 1;
                            continue;
                        }
                        if(Graphics[i].Points[j].X > x_max_real)
                        {
                            // добавить пересечение с правой границей
                            if(Graphics[i].GetVal(x_max_real) > y_max_real)
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_max_real, Graphics[i].Points[j].X), y_max_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                            else if(Graphics[i].GetVal(x_max_real) < y_min_real)
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_min_real, Graphics[i].Points[j].X), y_min_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                            else
                                line.Points.Add(WindowPoint(x_max_real, Graphics[i].GetVal(x_max_real), ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                            
                            break;
                        }
                        if (Graphics[i].Points[j].Y < y_min_real)
                        {
                            if (skip_point == 0)
                            {
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_min_real, Graphics[i].Points[j].X), y_min_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                canvas1.Children.Add(line);
                                line = new Polyline();
                                line.StrokeThickness = 2;
                                // цвет
                                line.Stroke = ColorList[Graphics[i].Color];
                            }
                            if(skip_point == 3)
                            {
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_max_real, Graphics[i].Points[j].X), y_max_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_min_real, Graphics[i].Points[j].X), y_min_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                canvas1.Children.Add(line);
                                line = new Polyline();
                                line.StrokeThickness = 2;
                                // цвет
                                line.Stroke = ColorList[Graphics[i].Color];
                            }
                            skip_point = 4;
                            continue;
                        }
                        if (Graphics[i].Points[j].Y > y_max_real)
                        {
                            if(skip_point == 0)
                            {
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_max_real, Graphics[i].Points[j].X), y_max_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));

                                canvas1.Children.Add(line);
                                line = new Polyline();
                                line.StrokeThickness = 2;
                                // цвет
                                line.Stroke = ColorList[Graphics[i].Color];
                            }
                            if(skip_point == 4)
                            {
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_min_real, Graphics[i].Points[j].X), y_min_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_max_real, Graphics[i].Points[j].X), y_max_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));

                                canvas1.Children.Add(line);
                                line = new Polyline();
                                line.StrokeThickness = 2;
                                // цвет
                                line.Stroke = ColorList[Graphics[i].Color];
                            }
                            skip_point = 3;

                            continue;
                        }

                        Point p = new Point(Graphics[i].Points[j].X, Graphics[i].Points[j].Y);

                        p.X *= ScalCoefx;
                        p.Y *= ScalCoefy;
                        p.X += StartPoint.X;
                        p.Y = StartPoint.Y - p.Y;

                        // добавить пересечение с границей
                        switch(skip_point)
                        {
                            case 1: // left
                                if (Graphics[i].GetVal(x_min_real) > y_max_real)
                                    line.Points.Add(WindowPoint(Graphics[i].GetArg(y_max_real, Graphics[i].Points[j].X), y_max_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                else if (Graphics[i].GetVal(x_min_real) < y_min_real)
                                    line.Points.Add(WindowPoint(Graphics[i].GetArg(y_min_real, Graphics[i].Points[j].X), y_min_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                else
                                    line.Points.Add(WindowPoint(x_min_real, Graphics[i].GetVal(x_min_real), ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                break;
                            case 3: // top
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_max_real, Graphics[i].Points[j].X), y_max_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                break;
                            case 4: // bottom
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_min_real, Graphics[i].Points[j].X), y_min_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                break;
                            default:
                                break;
                        }
                        line.Points.Add(p);
                        skip_point = 0;

                    }
                    // добавить пересечение с границей
                    if (line.Points.Count != 0)
                    {
                        switch (skip_point)
                        {
                            case 3: // top
                                
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_max_real, (line.Points.Last().X - StartPoint.X) / ScalCoefx), y_max_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                break;
                            case 4: // bottom
                                line.Points.Add(WindowPoint(Graphics[i].GetArg(y_min_real, (line.Points.Last().X - StartPoint.X) / ScalCoefx), y_min_real, ScalCoefx, ScalCoefy, StartPoint.X, -StartPoint.Y));
                                break;
                            default:
                                break;
                        }
                        canvas1.Children.Add(line);
                    }
                }
            }
        }

        private void canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Axis.Cur_step_x += e.Delta;
            Axis.Cur_step_y += e.Delta;
            Draw();
        }
    }
    public class axis
    {
        double size_of_marks = 5;
        double min_step = 40;
        double max_step = 400;

        double hx = 40;
        double hy = 40;
        double scale_x = 1; // сколько 1 в hx_px по х
        double scale_y = 1; // сколько 1 в hy_px по y


        public double Min_step { get { return min_step; } }
        public double Max_step { get { return max_step; } }

        public double Cur_step_x
        {
            get
            {
                return hx;
            }
            set
            {
                if (value <= Min_step)
                {
                    hx = Max_step;
                    scale_x *= max_step / min_step;
                }
                else if (value >= Max_step)
                {
                    hx = Min_step;
                    scale_x /= max_step / min_step;
                }
                else
                {
                    hx = value;
                }
            }
        }
        public double Scale_x // на сколько надо умножить 1, чтобы получить оконные координаты 1
        {
            get
            {
                return hx / scale_x;
            }
        }
        public Line Axis_line_x { get; set; }
        public List<Line> Tic_lines_x { get; set; }
        public List<TextBlock> Tic_lables_x { get; set; }
        public double Cur_step_y
        {
            get
            {
                return hy;
            }
            set
            {
                if (value <= Min_step)
                {
                    hy = Max_step;
                    scale_y *= max_step / min_step;
                }
                else if (value >= Max_step)
                {
                    hy = Min_step;
                    scale_y /= max_step / min_step;
                }
                else
                {
                    hy = value;
                }
            }
        }
        public double Scale_y // на сколько надо умножить 1, чтобы получить оконные координаты 1
        {
            get
            {
                return  hy / scale_y;
            }
        }
        public Line Axis_line_y { get; set; }
        public List<Line> Tic_lines_y { get; set; }
        public List<TextBlock> Tic_lables_y { get; set; }


        public void PrepareAxisX(point_type min_point, point_type max_point, point_type start_point)
        {
            Axis_line_x = new Line();
            Axis_line_x.X1 = min_point.X;
            Axis_line_x.Y1 = min_point.Y;
            Axis_line_x.X2 = max_point.X;
            Axis_line_x.Y2 = max_point.Y;
            Axis_line_x.StrokeThickness = 1;
            Axis_line_x.Stroke = Brushes.Black;

            Tic_lines_x = new List<Line>();
            Tic_lables_x = new List<TextBlock>();

            point_type loc_start = new point_type(start_point);
            double label_1 = 0;
            double cur_h = hx;
            if (hx / min_step >= 2)
            {
                cur_h = hx / 2;
            }
            if (hx / min_step >= 5)
            {
                cur_h = hx / 5;
            }
            while (loc_start.X < min_point.X)
            {
                loc_start.X += cur_h;
                label_1 += (scale_x / hx * cur_h);
            }
            while (loc_start.X - cur_h > min_point.X)
            {
                loc_start.X -= cur_h;
                label_1 -= (scale_x / hx * cur_h);
            }

            int i = 0;
            double label_start = label_1;
            for (double xi = loc_start.X; xi <= max_point.X; xi = loc_start.X + i * cur_h, label_1 = label_start + i * (scale_x / hx * cur_h))
            {
                Line px_i = new Line();
                px_i.X1 = xi;
                px_i.Y1 = Axis_line_x.Y1 + size_of_marks;
                px_i.X2 = xi;
                px_i.Y2 = Axis_line_x.Y1 - size_of_marks;
                px_i.StrokeThickness = 1;
                px_i.Stroke = Brushes.Black;
                Tic_lines_x.Add(px_i);

                TextBlock text = new TextBlock();

                if (Math.Abs(label_1 / cur_h) < 1e-14)
                {
                    label_1 = 0;
                    Canvas.SetLeft(text, xi - 2 * size_of_marks);
                }
                else
                {
                    Canvas.SetLeft(text, xi - size_of_marks);
                }
                text.Text = label_1.ToString("0.##e0");
                text.FontSize = 10;
                Canvas.SetTop(text, Axis_line_x.Y1 + size_of_marks);
                Tic_lables_x.Add(text);
                i++;
            }
        }
        public void PrepareAxisY(point_type min_point, point_type max_point, point_type start_point)
        {
            Axis_line_y = new Line();
            Axis_line_y.X1 = min_point.X;
            Axis_line_y.Y1 = min_point.Y;
            Axis_line_y.X2 = max_point.X;
            Axis_line_y.Y2 = max_point.Y;
            Axis_line_y.StrokeThickness = 1;
            Axis_line_y.Stroke = Brushes.Black;

            Tic_lines_y = new List<Line>();
            Tic_lables_y = new List<TextBlock>();

            point_type loc_start = new point_type(start_point);
            double label_1 = 0;
            double cur_h = hy;
            if (hy / min_step >= 2)
            {
                cur_h = hy / 2;
            }
            if (hy / min_step >= 5)
            {
                cur_h = hy / 5;
            }
            while (loc_start.Y > max_point.Y)
            {
                loc_start.Y -= cur_h;
                label_1 += (scale_y / hy * cur_h);
            }
            while (loc_start.Y + cur_h <= max_point.Y)
            {
                loc_start.Y += cur_h;
                label_1 -= (scale_y / hy * cur_h);
            }

            int i = 0;
            double label_start = label_1;
            for (double yi = loc_start.Y; yi >= min_point.Y; yi = loc_start.Y - i * cur_h, label_1 = label_start + i * (scale_y / hy * cur_h))
            {
                Line px_i = new Line();
                px_i.X1 = Axis_line_y.X1 - size_of_marks;
                px_i.Y1 = yi;
                px_i.X2 = Axis_line_y.X1 + size_of_marks;
                px_i.Y2 = yi;
                px_i.StrokeThickness = 1;
                px_i.Stroke = Brushes.Black;
                Tic_lines_y.Add(px_i);

                if (Math.Abs(label_1) > 1e-14)
                {
                    TextBlock text = new TextBlock();
                    text.Text = label_1.ToString("0.##e0");
                    text.FontSize = 10;

                    Canvas.SetTop(text, yi - 2 * size_of_marks);
                    Canvas.SetLeft(text, Axis_line_y.X1 + 2 * size_of_marks);
                    Tic_lables_y.Add(text);
                }
                    i++;
            }
        }
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
        public point_type(point_type p)
        {
            X = p.X;
            Y = p.Y;
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
        int _IsSelected = 1;
        public int IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnPropertyChanged();
            }
        }

        public int Color { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double GetVal(double x)
        {
            double res = 0;
            for(int i = 1; i < Points.Count; i++)
            {
                if (x >= Points[i - 1].X && x <= Points[i].X)
                {
                    res = (Points[i].Y - Points[i - 1].Y) / (Points[i].X - Points[i - 1].X) * (x - Points[i - 1].X) + Points[i - 1].Y;
                    break;
                }
            }
            return res;
        }
        public double GetArg(double y)
        {
            double res = 0;
            for (int i = 1; i < Points.Count; i++)
            {
                if (y >= Points[i - 1].Y && y <= Points[i].Y)
                {
                    res = (Points[i].X - Points[i - 1].X) / (Points[i].Y - Points[i - 1].Y) * (y - Points[i - 1].Y) + Points[i - 1].X;
                    break;
                }
            }
            return res;
        }
        public double GetArg(double y, double x)
        {
            double res = 0;
            int i;
            for (i = 1; i < Points.Count; i++)
            {
                if (x >= Points[i - 1].X && x <= Points[i].X)
                {
                    res = (Points[i].X - Points[i - 1].X) / (Points[i].Y - Points[i - 1].Y) * (y - Points[i - 1].Y) + Points[i - 1].X;
                    break;
                }
            }
            if (i == Points.Count)
                i = 0;
            return res;
        }
    }
}

