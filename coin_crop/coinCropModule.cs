using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;


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

        public Mat ProcessImg(Mat img)
        {
            
            Mat mask = new Mat(img.Height, img.Width, DepthType.Default, 3);

            if (shrink != 1)
                CvInvoke.Resize(img, img, new System.Drawing.Size(0, 0), fx: shrink, fy: shrink);

            CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray);
            if(!inverseThreshold)
                CvInvoke.Threshold(img, img, threshLevel, 255, ThresholdType.Binary);
            else
                CvInvoke.Threshold(img, img, threshLevel, 255, ThresholdType.BinaryInv);

            if (useMorphOpen)
                CvInvoke.MorphologyEx(img, img, MorphOp.Open, morphOpenKernel, new System.Drawing.Point(), 1, BorderType.Constant, new Emgu.CV.Structure.MCvScalar());

            if (useMorphClose)
                CvInvoke.MorphologyEx(img, img, MorphOp.Open, morphCloseKernel, new System.Drawing.Point(), 1, BorderType.Constant, new Emgu.CV.Structure.MCvScalar());

            if (useFindContours)
                CvInvoke.FindContours(img, contours, new Mat(), RetrType.Tree, ChainApproxMethod.ChainApproxSimple);



            return img;
        }

        public Mat ProcessImg(string imgPath)
        {
            Mat img = CvInvoke.Imread(imgPath, LoadImageType.AnyColor);

            return ProcessImg(img);
        }


        private Mat CropImage(Mat img)
        {

            return img;
        }
    }
}
