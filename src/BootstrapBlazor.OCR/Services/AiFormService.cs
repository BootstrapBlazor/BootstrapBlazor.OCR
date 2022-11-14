// ********************************** 
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
using BootstrapBlazor.OCR.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace BootstrapBlazor.Ocr.Services
{

    public class AiFormService : BaseService<AnalyzedDocument>
    {
        public AiFormService() { }
        public AiFormService(string key, string url)
        {
            SubscriptionKey = key;
            Endpoint = url;
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
        public string Endpoint = "YOUR_FORM_RECOGNIZER_ENDPOINT";
        public string SubscriptionKey = "YOUR_FORM_RECOGNIZER_KEY";

        /// <summary>
        /// 转换 BrowserFileStream 到 MemoryStream
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<Stream> CopyStream(Stream input)
        {
            try
            {
            if (input.GetType().Name== "BrowserFileStream")
            {
                var output = new MemoryStream();
                byte[] buffer = new byte[16 * 1024];
                int read;
                while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }
                return output;
            }
            else
            {
                return input;
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<string>> AnalyzeDocument(string? url = null, Stream? image = null, string modelId = "prebuilt-receipt")
        {
            msg = "AnalyzeDocument start ";
            await GetStatus(msg);

            var credential = new AzureKeyCredential(SubscriptionKey);
            var client = new DocumentAnalysisClient(new Uri(Endpoint), credential);


            AnalyzeDocumentOperation operation;

            if (image != null)
            {
                var ms = await CopyStream(image);
                operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, ms);
            }
            else
            {
                //sample document
                Uri receiptUri = new Uri(url ?? "https://raw.githubusercontent.com/Azure/azure-sdk-for-python/main/sdk/formrecognizer/azure-ai-formrecognizer/tests/sample_forms/receipt/contoso-receipt.png");
                operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, receiptUri);
            }

            AnalyzeResult receipts = operation.Value;

            // To see the list of the supported fields returned by service and its corresponding types, consult:
            // https://aka.ms/formrecognizer/receiptfields

            await GetResult(receipts.Documents.ToList());
            var res = new List<string>();
            foreach (AnalyzedDocument receipt in receipts.Documents)
            {
                if (receipt.Fields.TryGetValue("MerchantName", out DocumentField merchantNameField))
                {
                    if (merchantNameField.FieldType == DocumentFieldType.String)
                    {
                        string merchantName = merchantNameField.Value.AsString();

                        msg = $"Merchant Name: '{merchantName}', with confidence {merchantNameField.Confidence}";
                        await GetStatus(msg);
                        res.Add(msg);

                    }
                }

                if (receipt.Fields.TryGetValue("TransactionDate", out DocumentField transactionDateField))
                {
                    if (transactionDateField.FieldType == DocumentFieldType.Date)
                    {
                        DateTimeOffset transactionDate = transactionDateField.Value.AsDate();

                        msg = $"Transaction Date: '{transactionDate}', with confidence {transactionDateField.Confidence}";
                        await GetStatus(msg);
                        res.Add(msg);

                    }
                }

                if (receipt.Fields.TryGetValue("Items", out DocumentField itemsField))
                {
                    if (itemsField.FieldType == DocumentFieldType.List)
                    {
                        foreach (DocumentField itemField in itemsField.Value.AsList())
                        {
                            msg = "Item:";
                            await GetStatus(msg);
                            res.Add(msg);

                            if (itemField.FieldType == DocumentFieldType.Dictionary)
                            {
                                IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                                if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                                {
                                    if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                    {
                                        string itemDescription = itemDescriptionField.Value.AsString();

                                        msg = $"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}";
                                        await GetStatus(msg);
                                        res.Add(msg);

                                    }
                                }

                                if (itemFields.TryGetValue("TotalPrice", out DocumentField itemTotalPriceField))
                                {
                                    if (itemTotalPriceField.FieldType == DocumentFieldType.Double)
                                    {
                                        double itemTotalPrice = itemTotalPriceField.Value.AsDouble();

                                        msg = $"  Total Price: '{itemTotalPrice}', with confidence {itemTotalPriceField.Confidence}";
                                        await GetStatus(msg);
                                        res.Add(msg);

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
                        double total = totalField.Value.AsDouble();

                        msg = $"Total: '{total}', with confidence '{totalField.Confidence}'";
                        await GetStatus(msg);
                        res.Add(msg);

                    }
                }
            }
            return res;
        }
    }
}

