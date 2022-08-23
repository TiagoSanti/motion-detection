using OpenCvSharp;

namespace MotionDetection
{
    public class MotionDetector
    {
        private Mat diff;
        private Mat gray;
        private Mat blur;
        private Mat thresh;
        private Mat dilated;
        private Mat hierarchy;
        private Mat[] contours;
        private Point textLocation = new Point(10, 20);
        private Size resizeSize = new Size();
        private Dictionary<string, Mat> processingMats;

        public MotionDetector()
        {
            this.diff = new Mat();
            this.gray = new Mat();
            this.blur = new Mat();
            this.thresh = new Mat();
            this.dilated = new Mat();
            this.hierarchy = new Mat();
            this.contours = Array.Empty<Mat>();
            this.resizeSize = Size.Zero;
            this.processingMats = new Dictionary<string, Mat>();
        }

        public bool IsMotionDetected(Mat frame1, Mat frame2, bool drawMotion, bool showAllSteps)
        {
            bool motionDetected = false;
            this.contours = GetContours(frame1, frame2);
            
            if (HasMotion(contours))
            {
                motionDetected = true;

                if (drawMotion)
                {
                    DrawMotion(frame1, contours);
                }
            }

            if (showAllSteps)
            {
                if (this.resizeSize.Equals(Size.Zero))
                {
                    this.resizeSize.Width = frame1.Width / 2;
                    this.resizeSize.Height = frame1.Height / 2;
                }

                ShowAllSteps();
            }

            return motionDetected;
        }

        private void ShowAllSteps()
        {
            this.processingMats["1 - diff"] = diff.Resize(resizeSize);
            this.processingMats["2 - gray"] = gray.Resize(this.resizeSize);
            this.processingMats["3 - blur"] = blur.Resize(resizeSize);
            this.processingMats["4 - thresh"] = thresh.Resize(resizeSize);
            this.processingMats["5 - dilated"] = dilated.Resize(resizeSize);

            foreach (string key in processingMats.Keys)
            {
                Cv2.ImShow(key, processingMats[key]);
                processingMats[key].Dispose();
            }
        }

        private void DrawMotion(Mat frame1, Mat[] contours)
        {
            foreach (Mat contour in contours)
            {
                if (Cv2.ContourArea(contour) > 900)
                {
                    Rect rect = Cv2.BoundingRect(contour);
                    Cv2.Rectangle(frame1, rect, Scalar.Red, 1);
                    Cv2.PutText(frame1, "Motion detected", this.textLocation, HersheyFonts.HersheySimplex, 1, Scalar.Red);
                }
            }
        }

        private static bool HasMotion(Mat[] contours)
        {
            foreach (Mat contour in contours)
            {
                if (Cv2.ContourArea(contour) > 900)
                {
                    return true;
                }
            }

            return false;
        }

        private Mat[] GetContours(Mat frame1, Mat frame2)
        {
            Cv2.Absdiff(frame1, frame2, diff);
            Cv2.CvtColor(diff, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, blur, new Size(5, 5), 0);
            Cv2.Threshold(blur, thresh, 20, 255, ThresholdTypes.Binary);
            Cv2.Dilate(thresh, dilated, null, iterations: 3);
            Cv2.FindContours(dilated, out Mat[] contours, hierarchy, mode: RetrievalModes.Tree, method: ContourApproximationModes.ApproxSimple);

            return contours;
        }

        public void DisposeAllMats()
        {
            this.diff.Dispose();
            this.gray.Dispose();
            this.blur.Dispose();
            this.thresh.Dispose();
            this.dilated.Dispose();
            this.hierarchy.Dispose();
            foreach (Mat contour in this.contours)
                contour.Dispose();
        }

        public void FinishDetection()
        {
            foreach (string title in this.processingMats.Keys)
            {
                try
                {
                    Cv2.DestroyWindow(title);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
