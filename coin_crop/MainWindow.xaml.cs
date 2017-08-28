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
        string filePath = "";
        string fileName = "";
        string folderPath = "";
        

        public MainWindow()
        {
            InitializeComponent();
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
                imgWindow.Source = CvUtils.ToBitmapSource(cc.ProcessImg(filePath).Mat);
            }            
        }

        private void UpdateParameters()
        {
            bool uMC = (bool)cbUseMorphClose.IsChecked;
            bool uMO = (bool)cbUseMorphOpen.IsChecked;
            bool uCA = (bool)cbUseContorApprox.IsChecked;
            bool uFC = (bool)cbUseFindContours.IsChecked;
            bool iTH = (bool)cbInverseThreshold.IsChecked;
            bool uGF = (bool)cbUseGaussianFilter.IsChecked;
            bool uME = (bool)cbUseErode.IsChecked;
            double cAC;
            int tL;
            int mOKS;
            int mCKS;
            int gFS;
            int mEKS;

            try
            {
                cAC = Convert.ToDouble(tbContourApprox.Text);
                tL = Convert.ToInt32(tbThreshLevel.Text);
                mOKS = Convert.ToInt32(tbMorphOpenKernelSize.Text);
                mCKS = Convert.ToInt32(tbMorphCloseKernelSize.Text);
                gFS = Convert.ToInt32(tbGaussKernelSize.Text);
                mEKS = Convert.ToInt32(tbErodeKernelSize.Text);
                cc.UpdateCCM(
                uMC,
                uMO,
                uCA,
                uFC,
                iTH,
                uGF,
                uME,
                cAC,
                tL,
                mOKS,
                mCKS,
                gFS,
                mEKS
                );
            }
            catch(Exception e)
            {
                //TODO
                System.Windows.MessageBox.Show("Error - " + e.Message);
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
            IImage img;
            if (String.IsNullOrEmpty(filePath))
                return;
            UpdateParameters();
            img = cc.ProcessImg(filePath);
            imgWindow.Source = CvUtils.ToBitmapSource(img);
            CvUtils.SaveImage(img, @"C:\Users\aertho\Desktop\img.tiff", filePath);
        }

        private void tbContourApprox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
