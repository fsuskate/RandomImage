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

namespace RandomImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new DataViewModel();
            (DataContext as DataViewModel).Retries = 10;

            (DataContext as DataViewModel).DoImgurAuth();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as DataViewModel).RandomImage();
        }
    }
}
