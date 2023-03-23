// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.OCR.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using static BootstrapBlazor.AzureServices.TranslateResponse;

namespace BootstrapBlazor.AzureServices;

/// <summary>
/// 翻译
/// </summary>
public partial class TranslateService : BaseService<ReadResult>
{

    public string SubscriptionKey = "your Computer Vision subscription key";

    public string Endpoint = "https://api.cognitive.microsofttranslator.com";


    public TranslateService(IConfiguration? config)
    {
        if (config != null)
        {
            SubscriptionKey = config["AzureTranslateKey"] ?? "";
            Endpoint = config["AzureTranslateUrl"] ?? "";
        }
    }

    public TranslateService(string key, string url)
    {
        SubscriptionKey = key;
        Endpoint = url;
    }


    /// <summary>
    /// 翻译
    /// </summary>
    /// <param name="textToTranslate"></param>
    /// <param name="route"></param>
    /// <returns></returns>
    public async Task<List<Translation>?> Translate(string textToTranslate = "I would really like to drive your car around the block a few times!", string route = "/translate?api-version=3.0&to=es&to=en&to=fr&to=ca&to=zh-Hant")
    {
        try
        {
            //https://learn.microsoft.com/zh-cn/azure/cognitive-services/speech-service/language-support?tabs=speech-translation&WT.mc_id=DT-MVP-5005078

            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(Endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
                // location required if you're using a multi-service or regional (not global) resource.
                request.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                var result0 = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<TranslateResponse>>(result0);
                var result1 = result?.First()?.translations?.First()?.text;
                Console.WriteLine(result1);
                return result?.First()?.translations?.ToList();
            }
        }
        catch
        {
            return null;

        }
    }
}
    public enum TranslateLanguage
    {
        cn, es, en, fr, ca, zh_Hant
    }

    //[{"detectedLanguage":{"language":"en","score":1.0},"translations":[{"text":"PRODUCTNAME_6","to":"es"},{ "text":"PRODUCTNAME_6","to":"en"}]}]
    public class TranslateResponse
{

        public Detectedlanguage? detectedLanguage { get; set; }
        public Translation[]? translations { get; set; }
        public class Detectedlanguage
        {
            public string? language { get; set; }
            public float score { get; set; }
        }
        public class Translation
        {
            public string? text { get; set; }
            public string? to { get; set; }
        }
    }
 


