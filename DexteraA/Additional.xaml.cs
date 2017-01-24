using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PCBDiploma;

namespace PCBDiploma
{
    /// <summary>
    /// Interaction logic for Additional.xaml
    /// </summary>
    public partial class Additional : Window
    {
        public Additional()
        {
            InitializeComponent();
        }

        
        private void btnOK_OnClick(object sender, RoutedEventArgs e)
        {


	        var mainWindow = this.Owner as MainWindow;
            //var mainView = new View();
             //mainView.X = ValueX();
           
	        if (mainWindow != null)
	        {
				mainWindow.X = ValueX();
				mainWindow.Y = ValueY();
            }
            //if (mainView != null)
            //{
            //    mainView.X = ValueX();
            //    mainView.Y = ValueY();
            //    mainView.DrawGrid();
            //}
            
			
            this.Close();
        }

		//public int[] ValuesXY()
        //{
        //    int X = 100, Y = 100;
        //    X = Convert.ToInt32(sldX.Value);
        //    Y = Convert.ToInt32(sldY.Value);
        //    int[] XY = {X, Y};
        //    return XY;
        //}
        public int ValueX()
        {
            int X =  Convert.ToInt32(sldX.Value);
            return X;
        }

        public int ValueY()
        {
            int Y = Convert.ToInt32(sldY.Value);
            return Y;
        }
        private void sldY_MouseMove(object sender, MouseEventArgs e)
        {
            ValY.Content = Convert.ToInt32(sldY.Value);

        }

        private void sldX_MouseMove(object sender, MouseEventArgs e)
        {
            ValX.Content = Convert.ToInt32(sldX.Value);
        }


    }
}
