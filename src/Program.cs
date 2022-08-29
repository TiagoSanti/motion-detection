using OpenCvSharp;

namespace movement_detection.src
{
    class Program
    {
        static void Main()
        {
            MotionDetector _detector = new();

            Task task = Task.Run(() => InitDetection(
                windowTitle: "webcam",
                source: 0,
                detector: _detector,
                drawMotion: true,
                detectEPI: true)
            );

            task.Wait();
        }

        private static void InitDetection(MotionDetector detector, string windowTitle, dynamic source, bool drawMotion, bool detectEPI)
        {
            using VideoCapture videoCapture = new VideoCapture(source);
            using Mat frameCopy = new();

            while (videoCapture.IsOpened())
            {
                using Mat frame = videoCapture.RetrieveMat();
                if (!frame.Empty())
                {
                    frame.CopyTo(frameCopy);

                    detector.IsMotionDetected(frameCopy, drawMotion);

                    Cv2.ImShow(windowTitle, frameCopy);

                }

                if (Cv2.WaitKey(40) == 'q')
                {
                    break;
                }
            }

            Cv2.DestroyWindow(windowTitle);
        }
    }
}