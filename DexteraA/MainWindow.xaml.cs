using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace PCBDiploma
{
    public partial class MainWindow
    {
        private StartFinish _status;
        public int _check = 4;
        public int X { get; set; }
        public int Y { get; set; }

        public Saving save;
        
 
       
        public MainWindow()
        {
            this.Activated += this.OnActivated;
            InitializeComponent();
           //_status = StartFinish.Finish;

            View.Main = this;
            save= new Saving();
         
			
        }

		private void OnActivated(object sender, EventArgs eventArgs)
		{
		   
			this.Activated -= this.OnActivated;
			
			var additional = new Additional
			{
				Owner = Window.GetWindow(this)
			};

			additional.ShowDialog();
         
		    View.X = X;
		    View.Y = Y;
            View.DrawGrid();
		}

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (View.Start == null)
            {
                MessageBox.Show("Введите начальную точку!");
                return;
            }

            if (View.Finish == null)
            {
                MessageBox.Show("Введите конечную точку!");
                return;
            }

            View.StartStop(_check);

            //if (_status == GameStatus.Stoped)
            //{
            //    StartPauseButton.Content = "Pause";
            //    _status = GameStatus.Started;
            //    View.StartStop(_check);
            //}
            //else if (_status == GameStatus.Started)
            //{
            //    StartPauseButton.Content = "Start";
            //    _status = GameStatus.Stoped;
            //    View.StartStop(_check);
            //}


        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            StartPauseButton.Content = "Start";
            //_status = GameStatus.Stoped;
            View.Clear();

            StartPauseButton.IsEnabled = true;
        }

        private void RadioA_Checked(object sender, RoutedEventArgs e)
        {
            _check = 1;
        }

        private void RadioB_Checked(object sender, RoutedEventArgs e)
        {
            _check = 2;
        }

        private void RadioC_Checked(object sender, RoutedEventArgs e)
        {
            _check = 3;
        }

        private void RadioD_Checked(object sender, RoutedEventArgs e)
        {
            _check = 4;
        }
        private void RadioE_Checked(object sender, RoutedEventArgs e)
        {
            _check = 5;
        }
        #region DELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETEDELETE
        //private void Import(object sender, RoutedEventArgs e)
        //{
        //    var openFileDialog = new OpenFileDialog();
        //    openFileDialog.ShowDialog();

        //    using (var fs = new FileStream(openFileDialog.FileName, FileMode.Open))
        //    {
        //        var sr = new StreamReader(fs);
        //        string[] points;

        //        while (!sr.EndOfStream)
        //        {
        //            points = sr.ReadLine().Split(' ');

        //            View.DrawBall(new Point(int.Parse(points[0]), int.Parse(points[1])), Brushes.Red, false);

        //        }
        //    }
        //}
      
        //private void ImportConnected(object sender, RoutedEventArgs e)
        //{
        //    var startFinishList = new List<StartFinish>();

        //    using (var fs = new FileStream("pointsConnected.txt", FileMode.Open))
        //    {
        //        var sr = new StreamReader(fs);

        //        string[] points;

        //        while (!sr.EndOfStream)
        //        {
        //            points = sr.ReadLine().Split(' ');

        //            var start = new Point(int.Parse(points[0]), int.Parse(points[1]));
        //            var finish = new Point(int.Parse(points[2]), int.Parse(points[3]));

        //            startFinishList.Add(new StartFinish { Start = start, Finish = finish });
        //        }
        //    }

        //    View.BulkImport(startFinishList);
        //}
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          // View.save.Save();
            using (var fs = new FileStream("points.txt", FileMode.Open))
            {
                var sr = new StreamReader(fs);

                string[] points;



                while (!sr.EndOfStream)
                {
                    points = sr.ReadLine().Split(' ');

                    View.DrawBall(new Point(int.Parse(points[0]), int.Parse(points[1])), Brushes.Red, false);

                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var startFinishList = new List<StartFinish>();

            using (var fs = new FileStream("pointsConnected.txt", FileMode.Open))
            {
                var sr = new StreamReader(fs);

                string[] points;

                while (!sr.EndOfStream)
                {
                    points = sr.ReadLine().Split(' ');

                    var start = new Point(int.Parse(points[0]), int.Parse(points[1]));
                    var finish = new Point(int.Parse(points[2]), int.Parse(points[3]));

                    startFinishList.Add(new StartFinish { Start = start, Finish = finish });
                }
            }

            View.BulkImport(startFinishList);
            //var newsave = Saving.GetSaving();
            //View.Clear();
            //var vi = new View(View.X, View.Y, newsave.Criteria, newsave.Points, newsave.Walls);
            //View.Main.X = View.X;
            //View.Main.Y = View.Y;
            //View.Cells = newsave.Points;
            //foreach(int crit in View.Criteria)
            //    View.Criteria[crit] = View.Criteria[crit];
            //View.Start = newsave.Points[0];
            //View.Finish = newsave.Points[1];
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Руководоство пользователя:\nДля проектирования печатной платы выберите на монтажном пространстве ");
        }
        private void ImportConnected(object sender, RoutedEventArgs e)
        {
            var startFinishList = new List<StartFinish>();

            using (var fs = new FileStream("pointsConnected.txt", FileMode.Open))
            {
                var sr = new StreamReader(fs);

                string[] points;

                while (!sr.EndOfStream)
                {
                    points = sr.ReadLine().Split(' ');

                    var start = new Point(int.Parse(points[0]), int.Parse(points[1]));
                    var finish = new Point(int.Parse(points[2]), int.Parse(points[3]));

                    startFinishList.Add(new StartFinish { Start = start, Finish = finish });
                }
            }

            View.BulkImport(startFinishList);
        }

    }
}

