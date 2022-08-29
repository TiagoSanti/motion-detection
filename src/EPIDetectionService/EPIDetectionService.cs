using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using RestSharp;
using Newtonsoft.Json;

namespace EPIDetection
{
    public class PPERecognitionApiClient
    {
        private readonly string _apiURL = "http://127.0.0.1:5000/";
        private readonly RestClient _restClient;

        public PPERecognitionApiClient()
        {
            _restClient = new RestClient(_apiURL);
        }

        public static string BitmapToBase64(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();

            return Convert.ToBase64String(byteImage);
        }

        public static RestRequest AddImageToRequest(RestRequest request, Bitmap bitmap)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            string base64Image = BitmapToBase64(bitmap);
            content.Add("ImageData", base64Image);
            request.AddJsonBody(JsonConvert.SerializeObject(content));
            return request;
        }

        public static List<dynamic> ManageResponse(RestResponse response)
        {
            if (response == null)
            {
                throw new Exception("Null response from API");
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseContent = response.Content;
                    List<dynamic> detectionResult = JsonConvert.DeserializeObject<List<dynamic>>(responseContent);

                    return detectionResult;
                }
                else
                {
                    if (response.StatusDescription == null)
                    {
                        Console.WriteLine("Null response from EPI detection");
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong: " + response.StatusDescription.ToString());
                    }
                    return null;
                }
            }
        }

        public List<dynamic> MakeDetectionRequestAsync(Bitmap bitmap)
        {
            RestRequest request = new RestRequest(@"detect/", Method.Post);
            request = AddImageToRequest(request, bitmap);
            RestResponse detectionResponse = _restClient.Execute<List<dynamic>>(request);

            return ManageResponse(detectionResponse);
        }


        public void ProcessAfterRecognizePPE(Bitmap bitmap, List<dynamic> result)
        {
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                foreach (dynamic identifiedObject in result)
                {
                    PPELocations location = new PPELocations(
                        (int)identifiedObject[2],
                        (int)identifiedObject[3],
                        (int)identifiedObject[4],
                        (int)identifiedObject[5],
                        (float)identifiedObject[6],
                        (string)identifiedObject[0]);

                    
                    DrawFaceRectangleOnIdentifiedPPE(location, gr, true);
                }
            }
        }

        public static void DrawFaceRectangleOnIdentifiedPPE(PPELocations location, Graphics gr, bool showName)
        {
            Rectangle rect = new Rectangle(
                    location.X,
                    location.Y,
                    location.Width,
                    location.Height
                );

            using (Pen thick_pen = new Pen(Color.Blue, 2))
            {
                gr.DrawRectangle(thick_pen, rect);
            }
            
            if(showName)
                DrawStringFloatFormat(gr, location.Name+" "+location.Condifence.ToString("0.00"), rect.X, rect.Y - 23);
        }

        public static void DrawStringFloatFormat(Graphics graphic, string text, float x, float y)
        {
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Red);

            graphic.DrawString(text, drawFont, drawBrush, x, y);
        }

        public static Bitmap Resize(Bitmap original, Size size)
        {
            return new Bitmap(original, size);
        }

    }
}
