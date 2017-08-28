using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using ImageMagick;

namespace coin_crop
{
    static class CvUtils
    {
        static public Matrix<double> Ones(int size)
        {
            Matrix<double> output = new Matrix<double>(size,size);
            for(int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    output.Data[i, j] = 1;

            return output;
        }



        static public VectorOfPoint BiggestContour(VectorOfVectorOfPoint contours)
        {
            int largestContourIndex = 0;
            double largestArea = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                double a = CvInvoke.ContourArea(contours[i], false);  //  Find the area of contour
                if (a > largestArea)
                {
                    largestArea = a;
                    largestContourIndex = i;                //Store the index of largest contour
                }

            }
            return contours[largestContourIndex];
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop
                  .Imaging.CreateBitmapSourceFromHBitmap(
                  ptr,
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        public static int SaveImage(IImage img, String path, String orgPath)
        {
            ImageProfile imageProfile;
            CvInvoke.Imwrite(path, img);
            try
            {
                using (MagickImage orgImage = new MagickImage(orgPath))
                {
                    imageProfile = orgImage.GetProfile("ICC");
                }
                using (MagickImage newImage = new MagickImage(path))
                {
                    newImage.AddProfile(imageProfile);
                    newImage.Write(path);
                }
            } catch(MagickBlobErrorException)
            {
                throw new System.Exception("Cannot save file.");
            }
            return 0;
        }

        internal static double DetermineThreshold(Image<Gray, byte> img)
        {
            double threshhold = 0;
            float smallest = float.MaxValue;
            Mat hist = new Mat();
            
            using (VectorOfMat vm = new VectorOfMat())
            {
                vm.Push(img.Mat);
                float[] ranges = new float[] { 0.0f, 256.0f };
                CvInvoke.CalcHist(vm, new int[] { 0 }, null, hist, new int[] { 256 }, ranges, false);
            }
            for (int i = 5;i < 50; ++i)
            {
                if (hist.GetValue(0,i) < smallest)
                {
                    smallest = hist.GetValue(0, i);
                    threshhold = i;
                }
            }
            return threshhold;
        }
    }
}
