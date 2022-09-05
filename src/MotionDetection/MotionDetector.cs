using OpenCvSharp;

namespace MotionDetection
{
    public class MotionDetector
    {
        private readonly BackgroundSubtractorMOG bg;

        public MotionDetector()
        {
            bg = BackgroundSubtractorMOG.Create(backgroundRatio: 0.8);
        }

        public bool IsMotionDetected(Mat frame)
        {
            Mat[] contours = Process(frame);

            if (HasMotion(contours))
            {
                return true;
            }

            return false;
        }

        public Mat[] Process(Mat frame)
        {
            using Mat bg_mask = new Mat();
            using Mat hierarchy = new Mat();

            frame.CopyTo(bg_mask);
            bg.Apply(bg_mask, bg_mask);

            Cv2.MorphologyEx(bg_mask, bg_mask, MorphTypes.Open, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)));
            Cv2.MedianBlur(bg_mask, bg_mask, 5);
            Cv2.FindContours(
                image: bg_mask,
                contours: out Mat[] contours,
                hierarchy: hierarchy,
                mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple
            );

            return contours;
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
