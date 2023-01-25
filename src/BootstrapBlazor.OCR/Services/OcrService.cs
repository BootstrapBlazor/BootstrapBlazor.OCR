// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.OCR.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;

namespace BootstrapBlazor.Ocr.Services
{

    public partial class OcrService : BaseService<ReadResult>
    {
        public string? LocalFilePath;
        
        //https://learn.microsoft.com/zh-cn/azure/cognitive-services/computer-vision/quickstarts-sdk/client-library?tabs=visual-studio&pivots=programming-language-csharp
        // 添加您的计算机视觉订阅密钥和端点
        
        public string SubscriptionKey = "your Computer Vision subscription key";
        
        public string Endpoint = "https://xxx.cognitiveservices.azure.com/";

        private const string READ_TEXT_URL_IMAGE = "https://freepos.es/uploads/demo/Doc/libai.jpg";

        private const string LOCAL_IMAGE = "C:\\Repos\\BootstrapBlazor.OCR\\src\\BootstrapBlazor.OCR\\wwwroot\\images\\";

        private const string READ_TEXT_LOCAL_IMAGE = LOCAL_IMAGE + "libai.jpg";
        
        private const string READ_TEXT_LOCAL_IMAGE1 = LOCAL_IMAGE + "printed_text.jpg";

        private const string ANALYZE_LOCAL_IMAGE = LOCAL_IMAGE + "celebrities.jpg";
        
        private const string DETECT_LOCAL_IMAGE = LOCAL_IMAGE + "objects.jpg";
        
        private const string DETECT_DOMAIN_SPECIFIC_LOCAL = LOCAL_IMAGE + "celebrities.jpg";

        // 用于分析图像的 URL 图像（小狗的图像）
        private const string ANALYZE_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png";
        
        // 用于检测物体的URL图像（滑板上的人的图像）
        private const string DETECT_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample9.png";
        
        // 用于检测特定领域内容的 URL 图片（古遗址图片）
        private const string DETECT_DOMAIN_SPECIFIC_URL = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/ComputerVision/Images/landmark.jpg";

        /// <summary>
        /// 获得/设置 识别完成回调方法,结果为string集合
        /// </summary>
        public Func<List<string>, Task>? Result { get; set; }
        
        public OcrService(IConfiguration? config)
        {
            if (config != null)
            {
                SubscriptionKey = config["AzureCvKey"] ?? "";
                Endpoint = config["AzureCvUrl"] ?? "";
            }
        }

        public OcrService(string localFilePath)
        {
            LocalFilePath = localFilePath;
        }

        public OcrService(string key, string url, string localFilePath)
        {
            LocalFilePath = localFilePath;
            SubscriptionKey = key;
            Endpoint = url;
        }

        public ComputerVisionClient Client { get => Client ?? OcrService.Authenticate(Endpoint, SubscriptionKey); }
            
        /// <summary>
        /// 转换和提取结果
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        async Task GetResult(List<string> models)
        {
            try
            {
                Console.WriteLine(models);
                if (Result != null) await Result.Invoke(models);
            }
            catch (Exception e)
            {
                if (OnError != null) await OnError.Invoke(e.Message);
            }
        }
        
        /// <summary>
        /// 从图像中提取文本 (OCR)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<List<string>> StartOcr(string? url = null, Stream? image = null)
        {
            msg = "Ocr start";
            await GetStatus(msg);
            Console.WriteLine();
            
            if (image != null)
            {
#if (IOS || MACCATALYST)
                var ms = await CopyStream(image);
                var res1 = await ReadFileLocal(client, url ?? READ_TEXT_LOCAL_IMAGE, ms);
#else
                if (LocalFilePath != null)
                {
                    var tempfilename = Path.Combine(LocalFilePath, "temp.jpg");
                    await using FileStream fs = new(tempfilename, FileMode.Create);
                    await image.CopyToAsync(fs);
                    var res1 = await ReadFileLocal(Client, tempfilename);
                    return res1;

                }
                else
                {
                    var res1 = await ReadFileLocal(Client, url ?? READ_TEXT_LOCAL_IMAGE, image);
                    return res1;
                }

#endif

            }
            else
            {
                // 使用读取 API 从 URL 图像中提取文本 (OCR)
                var res = await ReadFileUrl(Client, url ?? READ_TEXT_URL_IMAGE);
                return res;

            }

        }

        /// <summary>
        /// 初始化 ComputerVisionClient 类的新实例
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
            return client;
        }

        /// <summary>
        /// 从 URL 提取文本
        /// </summary>
        /// <param name="client"></param>
        /// <param name="urlFile"></param>
        /// <returns></returns>
        public async Task<List<string>> ReadFileUrl(ComputerVisionClient client, string urlFile)
        {
            Console.WriteLine("----------------------------------------------------------");
            msg = "从 URL 提取文本";
            await GetStatus(msg);
            Console.WriteLine();

            // 从 URL 读取文本
            var textHeaders = await client.ReadAsync(urlFile);
            // 请求后，获取操作位置（操作ID）
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // 从 Operation-Location 标头中检索将存储提取的文本的 URI。
            // 我们只需要 ID 而不是完整的 URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // 提取文本
            ReadOperationResult results;
            msg = $"从 URL 文件中提取文本 {Path.GetFileName(urlFile)}...";
            await GetStatus(msg);
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
                msg = $"{results.Status}...";
                await GetStatus(msg);
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // 显示找到的文本。
            Console.WriteLine();
            var res = new List<string>();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            await GetStatus("end");
            await GetResult(textUrlFileResults.ToList());
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    res.Add($"{line.Text}");
                    Console.WriteLine($"{line.Text}");
                }
            }
            Console.WriteLine();
            await GetResult(res);
            return res;

        }


        /// <summary>
        /// 从本地文件或者流提取文本
        /// </summary>
        /// <param name="client"></param>
        /// <param name="localFile"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<List<string>> ReadFileLocal(ComputerVisionClient client, string localFile, Stream? image = null)
        {
            Console.WriteLine("----------------------------------------------------------");
            msg = image!=null ? "从流提取文本" : "从本地文件提取文本";
            await GetStatus(msg);
            Console.WriteLine();

            // 从 URL 读取文本
            var textHeaders = await client.ReadInStreamAsync(image ?? File.OpenRead(localFile));
            // 请求后，获取操作位置（操作 operation ID）
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // <snippet_extract_response>
            // 从 Operation-Location 标头中检索将存储已识别文本的 URI。
            // 我们只需要 ID 而不是完整的 URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // 提取文本
            ReadOperationResult results;
            msg = image != null ? "从流提取文本" : $"从文件 {Path.GetFileName(localFile)} 提取文本...";
            await GetStatus(msg);
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));
            // </snippet_extract_response>

            // <snippet_extract_display>
            // 显示找到的文本。
            Console.WriteLine();
            var res = new List<string>();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            await GetStatus("end");
            await GetResult(textUrlFileResults.ToList());
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    res.Add($"{line.Text}");
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
            await GetResult(res);
            return res;
        }


    }
}

