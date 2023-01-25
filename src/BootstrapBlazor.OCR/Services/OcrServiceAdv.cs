// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.OCR.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BootstrapBlazor.Ocr.Services
{

    public partial class OcrService : BaseService<ReadResult>
    {
        // 创建一个列表，定义要从图像中提取的特征。 
        public List<VisualFeatureTypes?> Features = new List<VisualFeatureTypes?>()
        {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
        };

        /// <summary>
        /// 分析图像 - URL 图像
        /// <para>分析图像以获得特征和其他属性</para>
        ///<para></para>分析 URL 图片。提取标题、类别、标签、对象、面孔、色情/成人/血腥内容，
        ///<para></para>品牌、名人、地标、配色方案和图像类型。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="imageUrl"></param> 
        /// <returns></returns>
        public async Task<List<string>> AnalyzeImageUrl(string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"分析图像 - URL {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();

            //分析 URL 图片
            ImageAnalysis results = await Client.AnalyzeImageAsync(imageUrl, visualFeatures: Features);

            var res = AnalyzeResults(results);
            
            return res;
        }


        /// <summary>
        ///分析图像 - 本地图像
        /// <para>分析图像以获得特征和其他属性</para>
        ///<para></para>分析本地图像。提取标题、类别、标签、对象、面孔、色情/成人/血腥内容，
        ///<para></para>品牌、名人、地标、配色方案和图像类型。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="localImage"></param>
        /// <returns></returns>
        public async Task<List<string>> AnalyzeImageLocal(string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"分析本地图像 {Path.GetFileName(localImage)}...");
            Console.WriteLine();

            using (Stream analyzeImageStream = File.OpenRead(localImage))
            {
                // 分析本地图像。
                ImageAnalysis results = await Client.AnalyzeImageInStreamAsync(analyzeImageStream, visualFeatures: Features);

                var res = AnalyzeResults(results);
                
                return res;
            }
        }
        
        public List<string> AnalyzeResults(ImageAnalysis results)
        {
            var res = new List<string>();
            
            // 对图像内容进行概述。
            if (null != results.Description && null != results.Description.Captions)
            {
                Console.WriteLine("概括:");
                foreach (var caption in results.Description.Captions)
                {
                    Console.WriteLine($"{caption.Text} 可信度 {caption.Confidence}");
                }
                Console.WriteLine();
            }

            // 显示图像被划分的类别。
            if (null != results.Categories)
            {
                Console.WriteLine("类别:");
                foreach (var category in results.Categories)
                {
                    Console.WriteLine($"{category.Name} 可信度 {category.Score}");
                }
                Console.WriteLine();
            }
            
            // 图像标签及其置信度得分
            if (null != results.Tags)
            {
                Console.WriteLine("标签:");
                foreach (var tag in results.Tags)
                {
                    Console.WriteLine($"{tag.Name} {tag.Confidence}");
                }
                Console.WriteLine();
            }

            // 对象
            if (null != results.Objects)
            {
                Console.WriteLine("对象:");
                foreach (var obj in results.Objects)
                {
                    Console.WriteLine($"{obj.ObjectProperty} 可信度 {obj.Confidence} 位置 {obj.Rectangle.X}, " +
                      $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
                }
                Console.WriteLine();
            }

            // 面孔
            if (null != results.Faces)
            {

                Console.WriteLine("面孔:");
                foreach (var face in results.Faces)
                {
                    Console.WriteLine($"面孔 {face.Gender} 年龄 {face.Age} 位置 {face.FaceRectangle.Left}, " +
                      $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                      $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
                }
                Console.WriteLine();
            }

            // 成人或不雅内容
            if (null != results.Adult)
            {

                Console.WriteLine("成人或不雅内容:");
                Console.WriteLine($"有成人内容: {results.Adult.IsAdultContent} 可信度 {results.Adult.AdultScore}");
                Console.WriteLine($"有不雅内容: {results.Adult.IsRacyContent} 可信度 {results.Adult.RacyScore}");
                Console.WriteLine($"有血腥内容: {results.Adult.IsGoryContent} 可信度 {results.Adult.GoreScore}");
                Console.WriteLine();
            }

            // 知名品牌
            if (null != results.Brands)
            {
                Console.WriteLine("知名品牌:");
                foreach (var brand in results.Brands)
                {
                    Console.WriteLine($"商标 {brand.Name} 可信度 {brand.Confidence} 位置 {brand.Rectangle.X}, " +
                      $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
                }
                Console.WriteLine();
            }

            // 名人形象
            if (null != results.Categories)
            {
                Console.WriteLine("名人:");
                foreach (var category in results.Categories)
                {
                    if (category.Detail?.Celebrities != null)
                    {
                        foreach (var celeb in category.Detail.Celebrities)
                        {
                            Console.WriteLine($"{celeb.Name} 可信度 {celeb.Confidence} 位置 {celeb.FaceRectangle.Left}, " +
                              $"{celeb.FaceRectangle.Top}, {celeb.FaceRectangle.Height}, {celeb.FaceRectangle.Width}");
                        }
                    }
                }
                Console.WriteLine();
            }

            // 图像中的热门地标（如果有）。
            if (null != results.Categories)
            {
                Console.WriteLine("地标:");
                foreach (var category in results.Categories)
                {
                    if (category.Detail?.Landmarks != null)
                    {
                        foreach (var landmark in category.Detail.Landmarks)
                        {
                            Console.WriteLine($"{landmark.Name} 可信度 {landmark.Confidence}");
                        }
                    }
                }
                Console.WriteLine();
            }

            //标识配色方案。
            if (null != results.Color)
            {
                Console.WriteLine("配色方案:");
                Console.WriteLine("是黑或白: " + results.Color.IsBWImg);
                Console.WriteLine("强调色: " + results.Color.AccentColor);
                Console.WriteLine("主背景色: " + results.Color.DominantColorBackground);
                Console.WriteLine("主前景颜色: " + results.Color.DominantColorForeground);
                Console.WriteLine("主色调: " + string.Join(",", results.Color.DominantColors));
                Console.WriteLine();
            }

            // 检测图像类型。
            if (null != results.ImageType)
            {
                Console.WriteLine("图像类型:");
                Console.WriteLine("剪贴画类型: " + results.ImageType.ClipArtType);
                Console.WriteLine("画线类型: " + results.ImageType.LineDrawingType);
                Console.WriteLine();
            }

            return res;

        }

        /// <summary>
        /// 检测图像中的对象 - URL 图像
        /// </summary>
        /// <param name="client"></param>
        /// <param name="urlImage"></param>
        /// <returns></returns>
        public async Task DetectObjectsUrl(string urlImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("检测对象 - URL 图像");
            Console.WriteLine();

            Console.WriteLine($"检测 URL 图像中的对象 {Path.GetFileName(urlImage)}...");
            Console.WriteLine();
            // 检测物体
            DetectResult detectObjectAnalysis = await Client.DetectObjectsAsync(urlImage);

            // 对于图片中的每个检测到的对象，打印出检测到的边界对象、该检测的置信度和图像中的边界框
            Console.WriteLine("检测对象:");
            foreach (var obj in detectObjectAnalysis.Objects)
            {
                Console.WriteLine($"{obj.ObjectProperty} 可信度 {obj.Confidence} 位置 {obj.Rectangle.X}, " +
                  $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 检测图像中的对象 - 本地图像
        ///<para></para> 这是检测对象的替代方法，而不是通过分析图像进行检测.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="localImage"></param>
        /// <returns></returns>
        public async Task DetectObjectsLocal(string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("检测对象 - 本地图像");
            Console.WriteLine();

            using (Stream stream = File.OpenRead(localImage))
            {
                // 使用本地文件调用计算机视觉服务
                DetectResult results = await Client.DetectObjectsInStreamAsync(stream);

                Console.WriteLine($"检测本地图像中的物体 {Path.GetFileName(localImage)}...");
                Console.WriteLine();

                // 对于图片中的每个检测到的对象，打印出检测到的边界对象、该检测的置信度和图像中的边界框
                Console.WriteLine("检测对象:");
                foreach (var obj in results.Objects)
                {
                    Console.WriteLine($"{obj.ObjectProperty} 可信度 {obj.Confidence} 位置 {obj.Rectangle.X}, " +
                      $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 检测 URL 图像和本地图像中的域特定内容
        /// <para></para>检测特定领域的内容
        ///<para></para> 识别图像中的地标或名人。
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="urlImage">检测域特定 URL</param>
        /// <param name="localImage">检测域特定本地文件</param>
        /// <returns></returns>
        public async Task DetectDomainSpecific(string urlImage, string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("检测域特定内容 - URL 和本地图像");
            Console.WriteLine();

            // 检测 URL 图像中特定于域的内容。
            DomainModelResults resultsUrl = await Client.AnalyzeImageByDomainAsync("landmarks", urlImage);
            // 显示结果。
            Console.WriteLine($"检测 URL 图像中的地标 {Path.GetFileName(urlImage)}...");

            var jsonUrl = JsonConvert.SerializeObject(resultsUrl.Result);
            JObject resultJsonUrl = JObject.Parse(jsonUrl);
            if (resultJsonUrl["landmarks"].Any())
            {
                Console.WriteLine($"检测到地标: {resultJsonUrl["landmarks"][0]["name"]} " +
                    $"可信度 {resultJsonUrl["landmarks"][0]["confidence"]}.");
            }
            Console.WriteLine();

            // 检测本地图像中特定于域的内容。
            using (Stream imageStream = File.OpenRead(localImage))
            {
                // 如果您感兴趣，请将“名人”更改为“地标”。
                DomainModelResults resultsLocal = await Client.AnalyzeImageByDomainInStreamAsync("celebrities", imageStream);
                Console.WriteLine($"在本地图像中检测名人 {Path.GetFileName(localImage)}...");
                // 显示结果。
                var jsonLocal = JsonConvert.SerializeObject(resultsLocal.Result);
                JObject resultJsonLocal = JObject.Parse(jsonLocal);
                if (resultJsonLocal["celebrities"].Any())
                {
                    Console.WriteLine($"检测到名人: {resultJsonLocal["celebrities"][0]["name"]} " +
                      $"可信度 {resultJsonLocal["celebrities"][0]["confidence"]}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 从 URL 和本地图像生成缩略图
        /// <para></para>接受 URL 和本地图像，此示例将生成具有指定宽度/高度（像素）的缩略图。
        /// <para></para>缩略图将保存在本地。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="urlImage"></param>
        /// <param name="localImage"></param>
        /// <returns></returns>
        public async Task GenerateThumbnail(string urlImage, string localImage)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("生成缩略图 - URL 和本地图片");
            Console.WriteLine();

            // 缩略图将本地保存在此项目的 bin\Debug\netcoreappx.x\ 文件夹中。
            string localSavePath = @".";

            // URL
            Console.WriteLine("生成带有 URL 图片的缩略图...");
            // 将 smart Cropping 设置为 true 可使图像调整其纵横比
            // 以图像中感兴趣的区域为中心。如果需要，更改宽度/高度。
            Stream thumbnailUrl = await Client.GenerateThumbnailAsync(60, 60, urlImage, true);

            string imageNameUrl = Path.GetFileName(urlImage);
            string thumbnailFilePathUrl = Path.Combine(localSavePath, imageNameUrl.Insert(imageNameUrl.Length - 4, "_thumb"));

            Console.WriteLine("将缩略图从 URL 图像保存到 " + thumbnailFilePathUrl);
            using (Stream file = File.Create(thumbnailFilePathUrl)) { thumbnailUrl.CopyTo(file); }

            Console.WriteLine();

            // LOCAL
            Console.WriteLine("使用本地图像生成缩略图...");

            using (Stream imageStream = File.OpenRead(localImage))
            {
                Stream thumbnailLocal = await Client.GenerateThumbnailInStreamAsync(100, 100, imageStream, smartCropping: true);

                string imageNameLocal = Path.GetFileName(localImage);
                string thumbnailFilePathLocal = Path.Combine(localSavePath,
                        imageNameLocal.Insert(imageNameLocal.Length - 4, "_thumb"));
                // 保存到文件
                Console.WriteLine("将本地图像的缩略图保存到 " + thumbnailFilePathLocal);
                using (Stream file = File.Create(thumbnailFilePathLocal)) { thumbnailLocal.CopyTo(file); }
            }
            Console.WriteLine();
        }
        
    }
}

