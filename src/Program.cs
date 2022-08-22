using OpenCvSharp;

namespace movement_detection
{
    class Program
    {
        static void Main()
        {
            VideoCapture capture = new VideoCapture(0);

            Mat frame1 = capture.RetrieveMat();
            Mat frame2 = capture.RetrieveMat();
            Point point = new Point(10, 20);

            while (capture.IsOpened())
            {
                Mat diff = new Mat();
                Mat gray = new Mat();
                Mat blur = new Mat();
                Mat thresh = new Mat();
                Mat dilated = new Mat();
                Mat hierarchy = new Mat();

                Cv2.Absdiff(frame1, frame2, diff);
                Cv2.CvtColor(diff, gray, ColorConversionCodes.BGR2GRAY);
                Cv2.GaussianBlur(gray, blur, new Size(5, 5), 0);
                Cv2.Threshold(blur, thresh, 20, 255, ThresholdTypes.Binary);
                Cv2.Dilate(thresh, dilated, null, iterations: 3);
                Cv2.FindContours(dilated, out Mat[] contours, hierarchy, mode: RetrievalModes.Tree, method: ContourApproximationModes.ApproxSimple);

                foreach (Mat contour in contours)
                {
                    Rect rect = Cv2.BoundingRect(contour);

                    if (Cv2.ContourArea(contour) < 900)
                    {
                        continue;
                    }

                    Cv2.Rectangle(frame1, rect, Scalar.Red, 1);
                    Cv2.PutText(frame1, "Motion detected", point, HersheyFonts.HersheySimplex, 1, Scalar.Red);
                }

                Cv2.ImShow("feed", frame1);

                frame1.Dispose();
                frame1 = frame2;
                frame2 = capture.RetrieveMat();

                diff.Dispose();
                gray.Dispose();
                blur.Dispose();
                thresh.Dispose();
                dilated.Dispose();
                hierarchy.Dispose();
                foreach(Mat contour in contours)
                {
                    contour.Dispose();
                }

                if (Cv2.WaitKey(40) == 27)
                    break;
            }

            capture.Dispose();
            frame2.Dispose();
        }
    }
}