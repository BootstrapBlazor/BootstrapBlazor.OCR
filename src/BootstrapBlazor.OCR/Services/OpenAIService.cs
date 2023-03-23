// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.OCR.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;

namespace BootstrapBlazor.OpenAI.Services;


public partial class OpenAIService : BaseService<ReadResult>
{

    public string SubscriptionKey = "your Computer Vision subscription key";

    public string Endpoint = "https://xxx.cognitiveservices.azure.com/";

    //public Stream? Stream { get; set; }
    public string? Tempfilename { get; set; }

    /// <summary>
    /// 获得/设置 识别完成回调方法,结果为string集合
    /// </summary>
    public Func<List<string>, Task>? Result { get; set; }

    public OpenAIService(IConfiguration? config)
    {
        if (config != null)
        {
            SubscriptionKey = config["AzureOpenAIKey"] ?? "";
            Endpoint = config["AzureOpenAIUrl"] ?? "";
        }
    }

    public OpenAIService(string key, string url)
    {
        SubscriptionKey = key;
        Endpoint = url;
    }

    /// <summary>
    /// 从本地文件或者流提取文本
    /// </summary>
    /// <param name="client"></param>
    /// <param name="localFile"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async Task<string> OcrLocal(string localFile, Stream? stream = null)
    {
        Console.WriteLine("----------------------------------------------------------");
        msg = stream != null ? "从流提取文本" : "从本地文件提取文本";
        await GetStatus(msg);
        Console.WriteLine();

       
        return msg;
    }


}

