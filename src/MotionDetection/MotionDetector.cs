using OpenCvSharp;

namespace MotionDetection
{
    public class MotionDetector
    {
        private Point textLocation = new Point(10, 25);
        private BackgroundSubtractorMOG bg;

        public MotionDetector()
        {
            bg = BackgroundSubtractorMOG.Create(backgroundRatio: 0.8);
        }

        public bool IsMotionDetected(Mat frame1, bool drawMotion)
        {
            bool motionDetected = false;

            using (Mat processMat = new())
            {
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
        }

        public Mat[] Process(Mat frame)
        {
            using (Mat bg_mask1 = new())
            {
                frame.CopyTo(bg_mask1);
                bg.Apply(bg_mask1, bg_mask1);

                bg.Apply(frame, frame);

                Cv2.MorphologyEx(bg_mask1, bg_mask1, MorphTypes.Open, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)));

                Cv2.MedianBlur(bg_mask1, bg_mask1, 5);
                Cv2.ImShow("processing", bg_mask1);

                Cv2.FindContours(bg_mask1, out Mat[] contours, new Mat(), mode: RetrievalModes.External, method: ContourApproximationModes.ApproxSimple);

                return contours;
            }
        }

        private void DrawMotion(Mat frame1, Mat[] contours)
        {
            foreach (Mat contour in contours)
            {
                if (Cv2.ContourArea(contour) > 250)
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
