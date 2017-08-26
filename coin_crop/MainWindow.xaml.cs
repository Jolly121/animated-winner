using System;
using System.Collections.Generic;
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
using System.Runtime.InteropServices;

using Emgu.CV;

namespace coin_crop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string filePath = @"C:\Users\aertho\PycharmProjects\coincrop\images\black_background\218b.tif";
            InitializeComponent();
            CoinCropModule cc = new CoinCropModule();


            imgWindow.Source = CvUtils.ToBitmapSource(cc.ProcessImg(filePath).Mat);
                
            //cc.ProcessImg(filePath);
            
        }
    }
}
