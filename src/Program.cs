using OpenCvSharp;
using System.Threading.Tasks;

namespace MotionDetection
{
    class Program
    {
        static void Main()
        {
            MotionDetector _detector = new MotionDetector();

            Task task = Task.Run(() => InitDetection(
                windowTitle: "task",
                source: 0,
                detector: _detector,
                drawMotion: true,
                showAllSteps: true)
            );

            Task task2 = Task.Run(() => InitDetection(
                windowTitle: "task2",
                source: @"rtsp://centro.inovacao:Dvr!9Covid@192.168.23.34:8082/cam/realmonitor?channel=2&subtype=0",
                detector: _detector,
                drawMotion: true,
                showAllSteps: false)
            );

            Task.WaitAll(task, task2);
        }

        private static void InitDetection(string windowTitle, dynamic source, MotionDetector detector, bool drawMotion, bool showAllSteps)
        {
            VideoCapture videoCapture = new VideoCapture(source);

            Mat frame1 = videoCapture.RetrieveMat();
            Mat frame2 = videoCapture.RetrieveMat();

            while (videoCapture.IsOpened())
            {
                _ = detector.IsMotionDetected(frame1, frame2, drawMotion, showAllSteps);

                Cv2.ImShow(windowTitle, frame1);

                frame1.Dispose();
                frame1 = frame2;
                frame2 = videoCapture.RetrieveMat();

                if (Cv2.WaitKey(40) == 27)
                {
                    detector.FinishDetection();
                    break;
                }
            }

            Cv2.DestroyWindow(windowTitle);
            videoCapture.Dispose();
            frame2.Dispose();
        }
    }
}