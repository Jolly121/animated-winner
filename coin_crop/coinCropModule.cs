using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Drawing;

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

        private Mat morphOpenKernel;
        private Mat morphCloseKernel;
        

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

            morphOpenKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(morphOpenKernelSize, morphOpenKernelSize), new Point(-1, -1));
            morphCloseKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(morphCloseKernelSize, morphCloseKernelSize), new Point(-1, -1));


        }
        public void UpdateCCM(
            bool uMC,
            bool uMO,
            bool uCA,
            bool uFC,
            bool iTH,
            double cAC,
            int tL,
            int mOKS,
            int mCKS
            )
        {
            useMorphClose = uMC;
            useMorphOpen = uMO;
            useContourApprox = uCA;
            useFindContours = uFC;
            inverseThreshold = iTH;

            contourApproxConstant = cAC;
            threshLevel = tL;
            morphCloseKernelSize = mOKS;
            morphOpenKernelSize = mCKS;

            morphOpenKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(morphOpenKernelSize, morphOpenKernelSize), new Point(-1, -1));
            morphCloseKernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(morphCloseKernelSize, morphCloseKernelSize), new Point(-1, -1));
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
            CvInvoke.Imwrite(@"C:\Users\aertho\Desktop\mask.tif", mask);

            if (!inverseThreshold)
                CvInvoke.Threshold(mask, mask, threshLevel, Byte.MaxValue, ThresholdType.Binary);
            else
                CvInvoke.Threshold(mask, mask, threshLevel, Byte.MaxValue, ThresholdType.BinaryInv);

            if (useMorphOpen)
                CvInvoke.MorphologyEx(mask, mask, MorphOp.Open, morphOpenKernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            if (useMorphClose)
                CvInvoke.MorphologyEx(mask, mask, MorphOp.Open, morphCloseKernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, new MCvScalar());

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

            CvInvoke.Imwrite(@"C:\Users\Jackson\Desktop\img.tif", img);
            return img;
        }

        public Image<Bgra, byte> ProcessImg(string imgPath)
        {
            Image<Bgra, byte> img = CvInvoke.Imread(imgPath, LoadImageType.AnyColor).ToImage<Bgra, byte>();
            return ProcessImg(img);
        }
    }
}
