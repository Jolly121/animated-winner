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
        private bool useGaussianFilter;
        private bool inverseThreshold;
        private bool useErode;

        private double contourApproxConstant; //Reduces the number of points in the contour.
        private double threshLevel;            
        private int morphOpenKernelSize;    //Size of the morph Kernel
        private int morphCloseKernelSize;
        private int gaussianFilterSize;
        private int erodeKernelSize;
        private Mat morphOpenKernel;
        private Mat morphCloseKernel;
        private Mat erodeKernel;
        

        public CoinCropModule()
        {
            useMorphClose = true;
            useMorphOpen = true;
            useContourApprox = true;
            useFindContours = true;
            useErode = true;
            contourApproxConstant = 0.000015;
            threshLevel = 10;
            inverseThreshold = false;
            morphCloseKernelSize = 23;
            morphOpenKernelSize = 23;
            gaussianFilterSize = 23;
            erodeKernelSize = 3;
            InitializeKernels();
        }

        public void UpdateCCM(
            bool uMC,
            bool uMO,
            bool uCA,
            bool uFC,
            bool iTH,
            bool uGF,
            bool uME,
            double cAC,
            int tL,
            int mOKS,
            int mCKS,
            int gFS,
            int mEKS
            )
        {
            useMorphClose = uMC;
            useMorphOpen = uMO;
            useContourApprox = uCA;
            useFindContours = uFC;
            inverseThreshold = iTH;
            useGaussianFilter = uGF;
            useErode = uME;

            contourApproxConstant = cAC;
            threshLevel = tL;
            morphCloseKernelSize = mCKS;
            morphOpenKernelSize = mOKS;
            gaussianFilterSize = gFS;
            erodeKernelSize = mEKS;

            InitializeKernels();
        }

        public Image<Bgra, byte> ProcessImg(Image<Bgra, byte> img)
        {
            Image<Bgra, byte> maskBgra;
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Image<Gray, byte> mask, maskTemp;
            VectorOfPoint largestContour;
            System.Drawing.Rectangle croppingRectangle;
            
            mask = new Image<Gray, byte>(img.Width, img.Height, new Gray(255));
            maskBgra = new Image<Bgra, byte>(img.Width, img.Height, new Bgra(0, 0, 0, 0));

            CvInvoke.CvtColor(img, mask, ColorConversion.Bgra2Gray);
            
            if (useGaussianFilter)
                CvInvoke.GaussianBlur(mask, mask, new System.Drawing.Size(gaussianFilterSize, gaussianFilterSize), 0);

            if (!inverseThreshold)
                CvInvoke.Threshold(mask, mask, threshLevel, Byte.MaxValue, ThresholdType.Binary);
            else
                CvInvoke.Threshold(mask, mask, threshLevel, Byte.MaxValue, ThresholdType.BinaryInv);

            if (useMorphOpen)
                CvInvoke.MorphologyEx(mask, mask, MorphOp.Open, 
                    morphOpenKernel, 
                    new System.Drawing.Point(-1,-1), 1,
                    BorderType.Default, new MCvScalar());

            if (useMorphClose)
                CvInvoke.MorphologyEx(mask, mask, MorphOp.Close, 
                    morphCloseKernel, new System.Drawing.Point(-1,-1), 1,
                    BorderType.Default, new MCvScalar());

            if (useErode)
                CvInvoke.MorphologyEx(mask, mask, MorphOp.Erode, 
                    erodeKernel, new System.Drawing.Point(-1,-1), 1,
                    BorderType.Default, new MCvScalar());

            maskTemp = mask.Clone();
            CvInvoke.FindContours(maskTemp, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            largestContour = CvUtils.BiggestContour(contours);
            croppingRectangle = CvInvoke.BoundingRectangle(largestContour);

            if (useFindContours)
            {
                if (useContourApprox)
                {
                    double epsillon = contourApproxConstant * CvInvoke.ArcLength(largestContour, true);
                    CvInvoke.ApproxPolyDP(largestContour, largestContour, epsillon, true);
                }
                maskBgra.Draw(largestContour.ToArray(), new Bgra(255,255,255,255), -1);
            }
            else
            {
                CvInvoke.CvtColor(mask, maskBgra, ColorConversion.Gray2Bgra); // might not create transparent background
            }

            CvInvoke.BitwiseAnd(img, maskBgra, img);
            img.ROI = croppingRectangle;

            return img;
        }

        public Image<Bgra, byte> ProcessImg(string imgPath)
        {
            Image<Bgra, byte> img = CvInvoke.Imread(imgPath, LoadImageType.AnyColor).ToImage<Bgra, byte>();
            return ProcessImg(img);
        }

        private void InitializeKernels() {
            morphOpenKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, 
                new System.Drawing.Size(morphOpenKernelSize, morphOpenKernelSize), 
                new System.Drawing.Point(morphOpenKernelSize/2, morphOpenKernelSize/2));
            morphCloseKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, 
                new System.Drawing.Size(morphCloseKernelSize, morphCloseKernelSize), 
                new System.Drawing.Point(morphCloseKernelSize/2, morphCloseKernelSize/2));
            erodeKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, 
                new System.Drawing.Size(erodeKernelSize, erodeKernelSize), 
                new System.Drawing.Point(erodeKernelSize/2, erodeKernelSize/2));
        }
    }
}
