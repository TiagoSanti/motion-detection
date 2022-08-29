using OpenCvSharp;

namespace MotionDetection
{
    public class MotionDetector
    {
        private Point textLocation = new(10, 25);
        private readonly BackgroundSubtractorMOG bg;

        public MotionDetector()
        {
            bg = BackgroundSubtractorMOG.Create(backgroundRatio: 0.8);
        }

        public bool IsMotionDetected(Mat frame1, bool drawMotion)
        {
            bool motionDetected = false;

            using Mat processMat = new();
            Cv2.CopyTo(frame1, processMat);
            Mat[] contours = Process(processMat);

            if (HasMotion(contours))
            {
                motionDetected = true;

                if (drawMotion)
                {
                    DrawMotion(frame1, contours);
                }
            }

            return motionDetected;
        }

        public Mat[] Process(Mat frame)
        {
            using Mat bg_mask = new();
            frame.CopyTo(bg_mask);
            bg.Apply(bg_mask, bg_mask);
            Cv2.MorphologyEx(bg_mask, bg_mask, MorphTypes.Open, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)));
            Cv2.MedianBlur(bg_mask, bg_mask, 5);
            Cv2.ImShow("processing", bg_mask);

            Cv2.FindContours(bg_mask, out Mat[] contours, new Mat(), mode: RetrievalModes.External, method: ContourApproximationModes.ApproxSimple);

            return contours;
        }

        private void DrawMotion(Mat frame1, Mat[] contours)
        {
            foreach (Mat contour in contours)
            {
                if (Cv2.ContourArea(contour) > 900)
                {
                    Rect rect = Cv2.BoundingRect(contour);
                    Cv2.Rectangle(frame1, rect, Scalar.Red, 1);
                    Cv2.PutText(frame1, "Motion detected", this.textLocation, HersheyFonts.HersheySimplex, fontScale:1, Scalar.Red, thickness: 2);
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
    }
}
