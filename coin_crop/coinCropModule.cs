using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.Structure;

namespace coin_crop
{
    class CoinCropModule
    {
        private bool useMorphOpen;          //If morphing is used to process the image
        private bool useMorphClose;

        private bool useFindContours;       //If countours are used to process the image
        private bool useContourApprox;  
        private double contourApproxConstant; //Reduces the number of points in the contour.

        private double threshLevel;            
        private bool inverseThreshold;

        private int morphOpenKernelSize;    //Size of the morph Kernel
        private int morphCloseKernelSize;

        private double shrink;


        private Matrix<double> morphOpenKernel;
        private Matrix<double> morphCloseKernel;
        

        public CoinCropModule()
        {
            useMorphClose = true;
            useMorphOpen = true;
            useContourApprox = true;
            useFindContours = true;
            contourApproxConstant = 0.000015;

            threshLevel = 10;

            inverseThreshold = false;

            morphCloseKernelSize = 10;
            morphOpenKernelSize = 10;

            shrink = 1;

            morphOpenKernel = CvUtils.Ones(morphOpenKernelSize);
            morphCloseKernel = CvUtils.Ones(morphCloseKernelSize);

           
        }

        public Image<Rgb, byte> ProcessImg(Image<Rgb, byte> img)
        {
            Image<Rgb, byte> maskRgb;
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Image<Gray, byte> mask, maskTemp;
            int largestContourIndex = 0;

            
            // Mat mask = new Mat(img.Height, img.Width, DepthType.Default, 3);


            //if (shrink != 1)
               // CvInvoke.Resize(img, img, new System.Drawing.Size(0, 0), fx: shrink, fy: shrink);
            
            mask = new Image<Gray, byte>(img.Width, img.Height, new Gray(255));
            maskRgb = new Image<Rgb, byte>(img.Width, img.Height, new Rgb(255, 255, 255));

            CvInvoke.CvtColor(img, mask, ColorConversion.Bgr2Gray);
            
            if (!inverseThreshold)
                CvInvoke.Threshold(mask, mask, threshLevel, 255, ThresholdType.Binary);
            else
                CvInvoke.Threshold(mask, mask, threshLevel, 255, ThresholdType.BinaryInv);

            

            if (useMorphOpen)
                CvInvoke.MorphologyEx(mask, mask, MorphOp.Open, morphOpenKernel, new System.Drawing.Point(), 0, BorderType.Constant, new MCvScalar());

            if (useMorphClose)
                CvInvoke.MorphologyEx(mask, mask, MorphOp.Open, morphCloseKernel, new System.Drawing.Point(), 0, BorderType.Constant, new MCvScalar());

            

            maskTemp = mask.Clone();
            
        

            CvInvoke.FindContours(maskTemp, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            largestContourIndex = CvUtils.BiggestContour(contours);

           
            //CvInvoke.DrawContours(maskTemp, contours, largestContourIndex, new MCvScalar(255), thickness: 10);
            

            
            
            if (useFindContours)
            {
                mask = new Image<Gray, byte>(img.Width, img.Height, new Gray(255));
                mask.Draw(contours[largestContourIndex].ToArray(), new Gray(0), -1);
                CvInvoke.Imshow("mt", mask);
                //img_mod = mask;
            }
            
            CvInvoke.CvtColor(mask, maskRgb, ColorConversion.Gray2Bgr);
            
            CvInvoke.Add(img, maskRgb, img);
            CvInvoke.Imshow("img", img);



            return img;
        }

        public Image<Rgb, byte> ProcessImg(string imgPath)
        {
            Image<Rgb, byte> img = CvInvoke.Imread(imgPath, LoadImageType.AnyColor).ToImage<Rgb, byte>();
            
            return ProcessImg(img);
        }


        private Mat CropImage(Mat img)
        {

            return img;
        }
    }
}
