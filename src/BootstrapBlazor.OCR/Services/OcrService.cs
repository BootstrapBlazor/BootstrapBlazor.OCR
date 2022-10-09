﻿// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Net;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace BootstrapBlazor.Ocr.Services
{

    public class OcrService
    {
        public OcrService(string key, string url)
        {
            subscriptionKey = key;
            endpoint = url;
        }

        //https://learn.microsoft.com/zh-cn/azure/cognitive-services/computer-vision/quickstarts-sdk/client-library?tabs=visual-studio&pivots=programming-language-csharp
        // Add your Computer Vision subscription key and endpoint
        string subscriptionKey = "your Computer Vision subscription key";
        string endpoint = "https://xxx.cognitiveservices.azure.com/";

        private const string READ_TEXT_URL_IMAGE = "https://freepos.es/uploads/demo/Doc/libai.jpg";

        private const string LOCAL_IMAGE = "C:\\Repos\\BootstrapBlazor.OCR\\src\\BootstrapBlazor.OCR\\wwwroot\\images\\";

        private const string READ_TEXT_LOCAL_IMAGE = LOCAL_IMAGE + "libai.jpg";
        private const string READ_TEXT_LOCAL_IMAGE1 = LOCAL_IMAGE + "printed_text.jpg";

        private const string ANALYZE_LOCAL_IMAGE = LOCAL_IMAGE + "celebrities.jpg";
        private const string DETECT_LOCAL_IMAGE = LOCAL_IMAGE + "objects.jpg";
        private const string DETECT_DOMAIN_SPECIFIC_LOCAL = LOCAL_IMAGE + "celebrities.jpg";

        // URL image used for analyzing an image (image of puppy)
        private const string ANALYZE_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png";
        // </snippet_analyze_url>
        // URL image for detecting objects (image of man on skateboard)
        private const string DETECT_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample9.png";
        // URL image for detecting domain-specific content (image of ancient ruins)
        private const string DETECT_DOMAIN_SPECIFIC_URL = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/ComputerVision/Images/landmark.jpg";

        /// <summary>
        /// 获得/设置 连接完成回调方法
        /// </summary>
        public Func<string, Task>? OnResult { get; set; }

        /// <summary>
        /// 获得/设置 错误回调方法
        /// </summary>
        public Func<string, Task>? OnError { get; set; }

        public async Task GetResult(string status)
        {
            try
            {
                Console.WriteLine(status);
                if (OnResult != null) await OnResult.Invoke(status);
            }
            catch (Exception e)
            {
                if (OnError != null) await OnError.Invoke(e.Message);
            }
        }

        string? msg = string.Empty;

        public async Task<List<string>> StartOcr(string url, Stream? image = null)
        {
            msg = "Azure Cognitive Services Computer Vision - .NET quickstart example";
            await GetResult(msg);
            Console.WriteLine();

            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            //// Analyze an image to get features and other properties.
            //await AnalyzeImageUrl(client, ANALYZE_URL_IMAGE);
            //// </snippet_main_calls>

            //await AnalyzeImageLocal(client, ANALYZE_LOCAL_IMAGE);

            //// Detect objects in an image.
            //await DetectObjectsUrl(client, DETECT_URL_IMAGE);
            //await DetectObjectsLocal(client, DETECT_LOCAL_IMAGE);

            //// Detect domain-specific content in both a URL image and a local image.
            //await DetectDomainSpecific(client, DETECT_DOMAIN_SPECIFIC_URL, DETECT_DOMAIN_SPECIFIC_LOCAL);

            //// Generate a thumbnail image from a URL and local image
            //await GenerateThumbnail(client, ANALYZE_URL_IMAGE, DETECT_LOCAL_IMAGE);

            // Extract text (OCR) from a local image using the Read API
            if (image != null)
            {
                var res1 = await ReadFileLocal(client, url ?? READ_TEXT_LOCAL_IMAGE, image);
                return res1;

            }
            else
            {
                // Extract text (OCR) from a URL image using the Read API
                var res = await ReadFileUrl(client, url ?? READ_TEXT_URL_IMAGE);
                return res;

            }

        }

        public ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint };
            return client;
        }

        public async Task<List<string>> ReadFileUrl(ComputerVisionClient client, string urlFile)
        {
            Console.WriteLine("----------------------------------------------------------");
            msg = "READ FILE FROM URL";
            await GetResult(msg);
            Console.WriteLine();

            // Read text from URL
            var textHeaders = await client.ReadAsync(urlFile);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            msg = $"Extracting text from URL file {Path.GetFileName(urlFile)}...";
            await GetResult(msg);
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
                msg = $"{results.Status}...";
                await GetResult(msg);
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var res = new List<string>();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            await GetResult("end");
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    res.Add($"{line.Text}");
                    Console.WriteLine($"{line.Text}");
                }
            }
            Console.WriteLine();
            return res;

        }


        /*
        * READ FILE - LOCAL
        */

        public async Task<List<string>> ReadFileLocal(ComputerVisionClient client, string localFile, Stream? image = null)
        {
            Console.WriteLine("----------------------------------------------------------");
            msg = "READ FILE FROM LOCAL";
            await GetResult(msg);
            Console.WriteLine();

            // Read text from URL
            var textHeaders = await client.ReadInStreamAsync(image ?? File.OpenRead(localFile));
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // <snippet_extract_response>
            // Retrieve the URI where the recognized text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            msg = $"Reading text from local file {Path.GetFileName(localFile)}...";
            await GetResult(msg);
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));
            // </snippet_extract_response>

            // <snippet_extract_display>
            // Display the found text.
            Console.WriteLine();
            var res = new List<string>();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            await GetResult("end");
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    res.Add($"{line.Text}");
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
            return res;
        }


        // <snippet_visualfeatures>
        /* 
         * ANALYZE IMAGE - URL IMAGE
         * Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult/gory content,
         * brands, celebrities, landmarks, color scheme, and image types.
         */
        public static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };
            // </snippet_visualfeatures>

            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // <snippet_analyze>
            // Analyze the URL image 
            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);

            // <snippet_describe>
            // Sunmarizes the image content.
            Console.WriteLine("Summary:");
            foreach (var caption in results.Description.Captions)
            {
                Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
            }
            Console.WriteLine();
            // </snippet_describe>

            // <snippet_categorize>
            // Display categories the image is divided into.
            Console.WriteLine("Categories:");
            foreach (var category in results.Categories)
            {
                Console.WriteLine($"{category.Name} with confidence {category.Score}");
            }
            Console.WriteLine();
            // </snippet_categorize>

            // <snippet_tags>
            // Image tags and their confidence score
            Console.WriteLine("Tags:");
            foreach (var tag in results.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence}");
            }
            Console.WriteLine();
            // </snippet_tags>

            // <snippet_objects>
            // Objects
            Console.WriteLine("Objects:");
            foreach (var obj in results.Objects)
            {
                Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                  $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
            }
            Console.WriteLine();
            // </snippet_objects>

            // <snippet_faces>
            // Faces
            Console.WriteLine("Faces:");
            foreach (var face in results.Faces)
            {
                Console.WriteLine($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                  $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                  $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
            }
            Console.WriteLine();
            // </snippet_faces>

            // <snippet_adult>
            // Adult or racy content, if any.
            Console.WriteLine("Adult:");
            Console.WriteLine($"Has adult content: {results.Adult.IsAdultContent} with confidence {results.Adult.AdultScore}");
            Console.WriteLine($"Has racy content: {results.Adult.IsRacyContent} with confidence {results.Adult.RacyScore}");
            Console.WriteLine($"Has gory content: {results.Adult.IsGoryContent} with confidence {results.Adult.GoreScore}");
            Console.WriteLine();
            // </snippet_adult>

            // <snippet_brands>
            // Well-known (or custom, if set) brands.
            Console.WriteLine("Brands:");
            foreach (var brand in results.Brands)
            {
                Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
                  $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
            }
            Console.WriteLine();
            // </snippet_brands>

            // <snippet_celebs>
            // Celebrities in image, if any.
            Console.WriteLine("Celebrities:");
            foreach (var category in results.Categories)
            {
                if (category.Detail?.Celebrities != null)
                {
                    foreach (var celeb in category.Detail.Celebrities)
                    {
                        Console.WriteLine($"{celeb.Name} with confidence {celeb.Confidence} at location {celeb.FaceRectangle.Left}, " +
                          $"{celeb.FaceRectangle.Top}, {celeb.FaceRectangle.Height}, {celeb.FaceRectangle.Width}");
                    }
                }
            }
            Console.WriteLine();
            // </snippet_celebs>

            // <snippet_landmarks>
            // Popular landmarks in image, if any.
            Console.WriteLine("Landmarks:");
            foreach (var category in results.Categories)
            {
                if (category.Detail?.Landmarks != null)
                {
                    foreach (var landmark in category.Detail.Landmarks)
                    {
                        Console.WriteLine($"{landmark.Name} with confidence {landmark.Confidence}");
                    }
                }
            }
            Console.WriteLine();
            // </snippet_landmarks>

            // <snippet_color>
            // Identifies the color scheme.
            Console.WriteLine("Color Scheme:");
            Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
            Console.WriteLine("Accent color: " + results.Color.AccentColor);
            Console.WriteLine("Dominant background color: " + results.Color.DominantColorBackground);
            Console.WriteLine("Dominant foreground color: " + results.Color.DominantColorForeground);
            Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
            Console.WriteLine();
            // </snippet_color>

            // <snippet_type>
            // Detects the image types.
            Console.WriteLine("Image Type:");
            Console.WriteLine("Clip Art Type: " + results.ImageType.ClipArtType);
            Console.WriteLine("Line Drawing Type: " + results.ImageType.LineDrawingType);
            Console.WriteLine();
            // </snippet_type>
            // </snippet_analyze>
        }
        /*
         * END - ANALYZE IMAGE - URL IMAGE
         */

        /*
       * ANALYZE IMAGE - LOCAL IMAGE
	     * Analyze local image. Extracts captions, categories, tags, objects, faces, racy/adult/gory content,
	     * brands, celebrities, landmarks, color scheme, and image types.
       */
        public static async Task AnalyzeImageLocal(ComputerVisionClient client, string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - LOCAL IMAGE");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            Console.WriteLine($"Analyzing the local image {Path.GetFileName(localImage)}...");
            Console.WriteLine();

            using (Stream analyzeImageStream = File.OpenRead(localImage))
            {
                // Analyze the local image.
                ImageAnalysis results = await client.AnalyzeImageInStreamAsync(analyzeImageStream, visualFeatures: features);

                // Sunmarizes the image content.
                if (null != results.Description && null != results.Description.Captions)
                {
                    Console.WriteLine("Summary:");
                    foreach (var caption in results.Description.Captions)
                    {
                        Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
                    }
                    Console.WriteLine();
                }

                // Display categories the image is divided into.
                Console.WriteLine("Categories:");
                foreach (var category in results.Categories)
                {
                    Console.WriteLine($"{category.Name} with confidence {category.Score}");
                }
                Console.WriteLine();

                // Image tags and their confidence score
                if (null != results.Tags)
                {
                    Console.WriteLine("Tags:");
                    foreach (var tag in results.Tags)
                    {
                        Console.WriteLine($"{tag.Name} {tag.Confidence}");
                    }
                    Console.WriteLine();
                }

                // Objects
                if (null != results.Objects)
                {
                    Console.WriteLine("Objects:");
                    foreach (var obj in results.Objects)
                    {
                        Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                          $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
                    }
                    Console.WriteLine();
                }

                // Detected faces, if any.
                if (null != results.Faces)
                {
                    Console.WriteLine("Faces:");
                    foreach (var face in results.Faces)
                    {
                        Console.WriteLine($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, {face.FaceRectangle.Top}, " +
                          $"{face.FaceRectangle.Left + face.FaceRectangle.Width}, {face.FaceRectangle.Top + face.FaceRectangle.Height}");
                    }
                    Console.WriteLine();
                }

                // Adult or racy content, if any.
                if (null != results.Adult)
                {
                    Console.WriteLine("Adult:");
                    Console.WriteLine($"Has adult content: {results.Adult.IsAdultContent} with confidence {results.Adult.AdultScore}");
                    Console.WriteLine($"Has racy content: {results.Adult.IsRacyContent} with confidence {results.Adult.RacyScore}");
                    Console.WriteLine($"Has gory content: {results.Adult.IsGoryContent} with confidence {results.Adult.GoreScore}");
                    Console.WriteLine();
                }

                // Well-known brands, if any.
                if (null != results.Brands)
                {
                    Console.WriteLine("Brands:");
                    foreach (var brand in results.Brands)
                    {
                        Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
                          $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
                    }
                    Console.WriteLine();
                }

                // Celebrities in image, if any.
                if (null != results.Categories)
                {
                    Console.WriteLine("Celebrities:");
                    foreach (var category in results.Categories)
                    {
                        if (category.Detail?.Celebrities != null)
                        {
                            foreach (var celeb in category.Detail.Celebrities)
                            {
                                Console.WriteLine($"{celeb.Name} with confidence {celeb.Confidence} at location {celeb.FaceRectangle.Left}, " +
                                  $"{celeb.FaceRectangle.Top},{celeb.FaceRectangle.Height},{celeb.FaceRectangle.Width}");
                            }
                        }
                    }
                    Console.WriteLine();
                }

                // Popular landmarks in image, if any.
                if (null != results.Categories)
                {
                    Console.WriteLine("Landmarks:");
                    foreach (var category in results.Categories)
                    {
                        if (category.Detail?.Landmarks != null)
                        {
                            foreach (var landmark in category.Detail.Landmarks)
                            {
                                Console.WriteLine($"{landmark.Name} with confidence {landmark.Confidence}");
                            }
                        }
                    }
                    Console.WriteLine();
                }

                // Identifies the color scheme.
                if (null != results.Color)
                {
                    Console.WriteLine("Color Scheme:");
                    Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
                    Console.WriteLine("Accent color: " + results.Color.AccentColor);
                    Console.WriteLine("Dominant background color: " + results.Color.DominantColorBackground);
                    Console.WriteLine("Dominant foreground color: " + results.Color.DominantColorForeground);
                    Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
                    Console.WriteLine();
                }

                // Detects the image types.
                if (null != results.ImageType)
                {
                    Console.WriteLine("Image Type:");
                    Console.WriteLine("Clip Art Type: " + results.ImageType.ClipArtType);
                    Console.WriteLine("Line Drawing Type: " + results.ImageType.LineDrawingType);
                    Console.WriteLine();
                }
            }
        }
        /*
         * END - ANALYZE IMAGE - LOCAL IMAGE
         */

        /* 
       * DETECT OBJECTS - URL IMAGE
       */
        public static async Task DetectObjectsUrl(ComputerVisionClient client, string urlImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("DETECT OBJECTS - URL IMAGE");
            Console.WriteLine();

            Console.WriteLine($"Detecting objects in URL image {Path.GetFileName(urlImage)}...");
            Console.WriteLine();
            // Detect the objects
            DetectResult detectObjectAnalysis = await client.DetectObjectsAsync(urlImage);

            // For each detected object in the picture, print out the bounding object detected, confidence of that detection and bounding box within the image
            Console.WriteLine("Detected objects:");
            foreach (var obj in detectObjectAnalysis.Objects)
            {
                Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                  $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
            }
            Console.WriteLine();
        }
        /*
         * END - DETECT OBJECTS - URL IMAGE
         */

        /*
         * DETECT OBJECTS - LOCAL IMAGE
         * This is an alternative way to detect objects, instead of doing so through AnalyzeImage.
         */
        public static async Task DetectObjectsLocal(ComputerVisionClient client, string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("DETECT OBJECTS - LOCAL IMAGE");
            Console.WriteLine();

            using (Stream stream = File.OpenRead(localImage))
            {
                // Make a call to the Computer Vision service using the local file
                DetectResult results = await client.DetectObjectsInStreamAsync(stream);

                Console.WriteLine($"Detecting objects in local image {Path.GetFileName(localImage)}...");
                Console.WriteLine();

                // For each detected object in the picture, print out the bounding object detected, confidence of that detection and bounding box within the image
                Console.WriteLine("Detected objects:");
                foreach (var obj in results.Objects)
                {
                    Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                      $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
                }
                Console.WriteLine();
            }
        }
        /*
         * END - DETECT OBJECTS - LOCAL IMAGE
         */

        /*
         * DETECT DOMAIN-SPECIFIC CONTENT
         * Recognizes landmarks or celebrities in an image.
         */
        public static async Task DetectDomainSpecific(ComputerVisionClient client, string urlImage, string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("DETECT DOMAIN-SPECIFIC CONTENT - URL & LOCAL IMAGE");
            Console.WriteLine();

            // Detect the domain-specific content in a URL image.
            DomainModelResults resultsUrl = await client.AnalyzeImageByDomainAsync("landmarks", urlImage);
            // Display results.
            Console.WriteLine($"Detecting landmarks in the URL image {Path.GetFileName(urlImage)}...");

            var jsonUrl = JsonConvert.SerializeObject(resultsUrl.Result);
            JObject resultJsonUrl = JObject.Parse(jsonUrl);
            if (resultJsonUrl["landmarks"].Any())
            {
                Console.WriteLine($"Landmark detected: {resultJsonUrl["landmarks"][0]["name"]} " +
                    $"with confidence {resultJsonUrl["landmarks"][0]["confidence"]}.");
            }
            Console.WriteLine();

            // Detect the domain-specific content in a local image.
            using (Stream imageStream = File.OpenRead(localImage))
            {
                // Change "celebrities" to "landmarks" if that is the domain you are interested in.
                DomainModelResults resultsLocal = await client.AnalyzeImageByDomainInStreamAsync("celebrities", imageStream);
                Console.WriteLine($"Detecting celebrities in the local image {Path.GetFileName(localImage)}...");
                // Display results.
                var jsonLocal = JsonConvert.SerializeObject(resultsLocal.Result);
                JObject resultJsonLocal = JObject.Parse(jsonLocal);
                if (resultJsonLocal["celebrities"].Any())
                {
                    Console.WriteLine($"Celebrity detected: {resultJsonLocal["celebrities"][0]["name"]} " +
                      $"with confidence {resultJsonLocal["celebrities"][0]["confidence"]}");
                }
            }
            Console.WriteLine();
        }
        /*
         * END - DETECT DOMAIN-SPECIFIC CONTENT
         */

        /*
         * GENERATE THUMBNAIL
         * Taking in a URL and local image, this example will generate a thumbnail image with specified width/height (pixels).
         * The thumbnail will be saved locally.
         */
        public static async Task GenerateThumbnail(ComputerVisionClient client, string urlImage, string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("GENERATE THUMBNAIL - URL & LOCAL IMAGE");
            Console.WriteLine();

            // Thumbnails will be saved locally in your bin\Debug\netcoreappx.x\ folder of this project.
            string localSavePath = @".";

            // URL
            Console.WriteLine("Generating thumbnail with URL image...");
            // Setting smartCropping to true enables the image to adjust its aspect ratio 
            // to center on the area of interest in the image. Change the width/height, if desired.
            Stream thumbnailUrl = await client.GenerateThumbnailAsync(60, 60, urlImage, true);

            string imageNameUrl = Path.GetFileName(urlImage);
            string thumbnailFilePathUrl = Path.Combine(localSavePath, imageNameUrl.Insert(imageNameUrl.Length - 4, "_thumb"));

            Console.WriteLine("Saving thumbnail from URL image to " + thumbnailFilePathUrl);
            using (Stream file = File.Create(thumbnailFilePathUrl)) { thumbnailUrl.CopyTo(file); }

            Console.WriteLine();

            // LOCAL
            Console.WriteLine("Generating thumbnail with local image...");

            using (Stream imageStream = File.OpenRead(localImage))
            {
                Stream thumbnailLocal = await client.GenerateThumbnailInStreamAsync(100, 100, imageStream, smartCropping: true);

                string imageNameLocal = Path.GetFileName(localImage);
                string thumbnailFilePathLocal = Path.Combine(localSavePath,
                        imageNameLocal.Insert(imageNameLocal.Length - 4, "_thumb"));
                // Save to file
                Console.WriteLine("Saving thumbnail from local image to " + thumbnailFilePathLocal);
                using (Stream file = File.Create(thumbnailFilePathLocal)) { thumbnailLocal.CopyTo(file); }
            }
            Console.WriteLine();
        }
        /*
         * END - GENERATE THUMBNAIL
         */
    }


    public class AiFormService
    {
        public AiFormService(string key, string url)
        {
            apiKey = key;
            endpoint = url;
        }

        /*
      This code sample shows Prebuilt Receipt operations with the Azure Form Recognizer client library. 

      To learn more, please visit the documentation - Quickstart: Form Recognizer C# client library SDKs
      https://docs.microsoft.com/en-us/azure/applied-ai-services/form-recognizer/quickstarts/try-v3-csharp-sdk
    */


        /*
          Remember to remove the key from your code when you're done, and never post it publicly. For production, use
          secure methods to store and access your credentials. For more information, see 
          https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-security?tabs=command-line%2Ccsharp#environment-variables-and-application-configuration
        */
        string endpoint = "YOUR_FORM_RECOGNIZER_ENDPOINT";
        string apiKey = "YOUR_FORM_RECOGNIZER_KEY";
        public async Task ReadFileUrl()
        {

            var credential = new AzureKeyCredential(apiKey);
            var client = new DocumentAnalysisClient(new Uri(endpoint), credential);

            //sample document
            Uri receiptUri = new Uri("https://raw.githubusercontent.com/Azure/azure-sdk-for-python/main/sdk/formrecognizer/azure-ai-formrecognizer/tests/sample_forms/receipt/contoso-receipt.png");

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-receipt", receiptUri);

            AnalyzeResult receipts = operation.Value;

            // To see the list of the supported fields returned by service and its corresponding types, consult:
            // https://aka.ms/formrecognizer/receiptfields

            foreach (AnalyzedDocument receipt in receipts.Documents)
            {
                if (receipt.Fields.TryGetValue("MerchantName", out DocumentField merchantNameField))
                {
                    if (merchantNameField.FieldType == DocumentFieldType.String)
                    {
                        string merchantName = merchantNameField.Value.AsString();

                        Console.WriteLine($"Merchant Name: '{merchantName}', with confidence {merchantNameField.Confidence}");
                    }
                }

                if (receipt.Fields.TryGetValue("TransactionDate", out DocumentField transactionDateField))
                {
                    if (transactionDateField.FieldType == DocumentFieldType.Date)
                    {
                        DateTimeOffset transactionDate = transactionDateField.Value.AsDate();

                        Console.WriteLine($"Transaction Date: '{transactionDate}', with confidence {transactionDateField.Confidence}");
                    }
                }

                if (receipt.Fields.TryGetValue("Items", out DocumentField itemsField))
                {
                    if (itemsField.FieldType == DocumentFieldType.List)
                    {
                        foreach (DocumentField itemField in itemsField.Value.AsList())
                        {
                            Console.WriteLine("Item:");

                            if (itemField.FieldType == DocumentFieldType.Dictionary)
                            {
                                IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                                if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                                {
                                    if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                    {
                                        string itemDescription = itemDescriptionField.Value.AsString();

                                        Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                    }
                                }

                                if (itemFields.TryGetValue("TotalPrice", out DocumentField itemTotalPriceField))
                                {
                                    if (itemTotalPriceField.FieldType == DocumentFieldType.Double)
                                    {
                                        double itemTotalPrice = itemTotalPriceField.Value.AsDouble();

                                        Console.WriteLine($"  Total Price: '{itemTotalPrice}', with confidence {itemTotalPriceField.Confidence}");
                                    }
                                }
                            }
                        }
                    }
                }

                if (receipt.Fields.TryGetValue("Total", out DocumentField totalField))
                {
                    if (totalField.FieldType == DocumentFieldType.Double)
                    {
                        double total = totalField .Value.AsDouble();

                        Console.WriteLine($"Total: '{total}', with confidence '{totalField.Confidence}'");
                    }
                }
            }
        }
    }
}
