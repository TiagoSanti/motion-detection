using OpenCvSharp;
using OpenCvSharp.Extensions;
using EPIDetection;
using System.Drawing;

namespace MotionDetection
{
    class Program
    {
        static void Main()
        {
            MotionDetector _detector = new MotionDetector();
            PPERecognitionApiClient _ppeRecognitionApiClient = new();

            Task task = Task.Run(() => InitDetection(
                ppeRecognitionApiClient: _ppeRecognitionApiClient,
                windowTitle: "webcam",
                source: 0,
                detector: _detector,
                drawMotion: true,
                detectEPI: true)
            );

            Task.WaitAll(task);
        }

        private static void InitDetection(MotionDetector detector, PPERecognitionApiClient ppeRecognitionApiClient, string windowTitle, dynamic source, bool drawMotion, bool detectEPI)
        {
            using VideoCapture videoCapture = new VideoCapture(source);
            using Mat frameCopy = new();

            while (videoCapture.IsOpened())
            {
                using Mat frame = videoCapture.RetrieveMat();
                if (!frame.Empty())
                {
                    frame.CopyTo(frameCopy);

                    if (detector.IsMotionDetected(frameCopy, drawMotion) && detectEPI)
                    {
                        Bitmap bitmapFrame = frame.ToBitmap();
                        bitmapFrame = PPERecognitionApiClient.Resize(bitmapFrame, new System.Drawing.Size(640, 480));
                        List<dynamic> result = ppeRecognitionApiClient.MakeDetectionRequestAsync(bitmapFrame);

                        if (result != null)
                        {
                            if (result.Count > 0)
                            {
                                ppeRecognitionApiClient.ProcessAfterRecognizePPE(bitmapFrame, result);
                                Mat epiDetectionResult = BitmapConverter.ToMat(bitmapFrame);
                                Cv2.ImShow(windowTitle + " | EPI result", epiDetectionResult);
                            }
                        }
                        
                        bitmapFrame.Dispose();
                    }
                    else
                    {
                        try
                        {
                            Cv2.DestroyWindow(windowTitle + " | EPI result");
                        }
                        catch { }
                    }

                    frameCopy.Resize(new OpenCvSharp.Size(frameCopy.Width / 2, frameCopy.Height / 2));
                    Cv2.ImShow(windowTitle, frameCopy);

                }

                if (Cv2.WaitKey(40) == 'q')
                {
                    break;
                }
            }

            //Cv2.DestroyWindow(windowTitle + " | EPI result");
            Cv2.DestroyWindow(windowTitle);
        }
    }
}