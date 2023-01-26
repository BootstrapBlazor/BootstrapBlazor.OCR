// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Ocr.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.Extensions.Hosting;
#nullable disable

Console.WriteLine("Hello, Azure!");

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Encoding.GetEncoding(65001);

IServiceProvider ServiceProvider=null;
IConfiguration Config = null;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.AddUserSecrets<Program>();
        Config = configuration.Build();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTransient<OcrService>();
        services.AddTransient<AiFormService>();
        services.AddOptions();
        ServiceProvider = services.BuildServiceProvider();
    })
    .Build();


var apiKey = Config["AzureCvKey"];
Console.WriteLine($"AzureCvKey: {apiKey}");

var OcrService = ServiceProvider.GetService<OcrService>();




string READ_TEXT_URL_IMAGE = "https://freepos.es/uploads/demo/Doc/libai.jpg";

string LOCAL_IMAGE = "C:\\Repos\\BootstrapBlazor.OCR\\src\\BootstrapBlazor.OCR\\wwwroot\\images\\";

string READ_TEXT_LOCAL_IMAGE = LOCAL_IMAGE + "libai.jpg";

string READ_TEXT_LOCAL_IMAGE1 = LOCAL_IMAGE + "printed_text.jpg";

string ANALYZE_LOCAL_IMAGE = LOCAL_IMAGE + "celebrities.jpg";

string DETECT_LOCAL_IMAGE = LOCAL_IMAGE + "objects.jpg";

string DETECT_DOMAIN_SPECIFIC_LOCAL = LOCAL_IMAGE + "celebrities.jpg";

OcrService.OnError = OnError1;

// 用于OCR的图片（李白的诗句）
//var res = await OcrService!.StartOcr(READ_TEXT_URL_IMAGE);
//res.ForEach(Console.WriteLine);

// 用于分析图像的 URL 图像（小狗的图像）
string ANALYZE_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png";

// 用于检测物体的URL图像（滑板上的人的图像）
string DETECT_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample9.png";

// 用于检测特定领域内容的 URL 图片（古遗址图片）
string DETECT_DOMAIN_SPECIFIC_URL = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/ComputerVision/Images/landmark.jpg";

//分析图像
//await OcrService!.AnalyzeImage(ANALYZE_URL_IMAGE);
await OcrService!.AnalyzeImage(localImage: ANALYZE_LOCAL_IMAGE);

//检测图像中的对象 - URL 图像
//await OcrService!.DetectObjects(DETECT_URL_IMAGE);
await OcrService!.DetectObjects(localImage: DETECT_LOCAL_IMAGE);

//检测图像中的地标或名人
//await OcrService!.DetectDomainSpecific(DETECT_DOMAIN_SPECIFIC_URL);
await OcrService!.DetectDomainSpecific(localImage: DETECT_DOMAIN_SPECIFIC_LOCAL);



Task OnError1(string message)
{
    Console.WriteLine(message);
    return Task.CompletedTask;
}

//await host.RunAsync();
