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
using System.Windows.Forms;

namespace coin_crop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CoinCropModule cc = new CoinCropModule();
        string filePath = @"C:\Users\Jackson\Pictures\images\img_black_background\218b.tif";
        string fileName = "";
        string folderPath = "";
        

        public MainWindow()
        {
            InitializeComponent();
            imgWindow.Source = CvUtils.ToBitmapSource(cc.ProcessImg(filePath).Mat);
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog() //Creating an Open File Dialog object
            {
                Filter = "TIFF Files(*.tif)|*.tif|AllFiles(*.*)|*.*" //Sets the filter to show only .csv files or all files
            }; 

            bool fileSelected = Convert.ToBoolean(openDialog.ShowDialog());              //Opens the file explorer window, and notes if a file is selected.

            //If a file is selected...
            if (fileSelected)
            {
                filePath = openDialog.FileName;                 //...Store full path of the selected file...
                fileName = openDialog.SafeFileName;             //...Store just the name of the selected file...
                folderPath = getContainingFolder(filePath);     //...Store the path of the folder the file is stored in...
                tbFilePath.Text = filePath;
            }            
        }

        private string getContainingFolder(string filePath)
        {
            string[] folders = filePath.Split('\\');        //Fills the array with the folder names by splitting at each '\'
            string result = "";                             //Stores the folder path as it's being re-created

            //For all strings in the array 'folders' except the last...
            for (int i = 0; i < folders.Length - 1; i++)
            {
                //...Concatenate it into the result string.
                result = result + folders[i] + '\\';
            }
            return result;
        }

        private void bStart_Click(object sender, RoutedEventArgs e)
        {
            imgWindow.Source = CvUtils.ToBitmapSource(cc.ProcessImg(filePath).Mat);
        }

    }
}
