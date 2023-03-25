## Blazor 光学字符识别(OCR)/ 翻译/AI表格识别 组件 

### 示例

https://blazor.app1.es/ocr

https://blazor.app1.es/aiform

https://blazor.app1.es/translate

## 使用方法:

1. nuget包

    ```BootstrapBlazor.OCR```

2. _Imports.razor 文件 或者页面添加 添加组件库引用

    ```@using BootstrapBlazor.Components```

3. Program.cs 文件添加

    ```
    builder.Services.AddTransient<OcrService>();
    builder.Services.AddTransient<AiFormService>();
    builder.Services.AddTransient<TranslateService>();
    ```

4. Key

    `appsettings.json`或者其他配置文件添加配置

    ```
      "AzureCvKey": "AzureCvKey",
      "AzureCvUrl": "https://xxx.cognitiveservices.azure.com/",
      "AzureAiFormKey": "AzureAiFormKey",
      "AzureAiFormUrl": "https://xxx.cognitiveservices.azure.com/",
      "AzureTranslateKey": "AzureTranslateKey",
      "AzureTranslateUrl": "https://api.cognitive.microsofttranslator.com",

    ```

5. Razor页面


    [OcrPage.razor](https://github.com/densen2014/Densen.Extensions/blob/master/Demo/DemoShared/Pages/OcrPage.razor)

    [AiFormPage.razor](https://github.com/densen2014/Densen.Extensions/blob/master/Demo/DemoShared/Pages/AiFormPage.razor)

    [TranslatePage.razor](https://github.com/densen2014/Densen.Extensions/blob/master/Demo/DemoShared/Pages/TranslatePage.razor)

     Razor  

   ```
    @using BootstrapBlazor.Components
    
    <OCR ShowUI="true" ShowUI_Capture="true" Debug="true" OnResult="OnResult" />
    <AiForm ShowUI="true" Debug="true" OnReadResult="OnResult2" /> 
    <Translate /> 

    @code{
        List<string> res { get; set; }
        private Task OnResult(List<string> res)
        {
            this.res = res;
            StateHasChanged();
            return Task.CompletedTask;
        }

        List<AnalyzedDocument>? models { get; set; }
        private Task OnResult2(List<AnalyzedDocument> models)
        {
            this.models = models;
            StateHasChanged();
            return Task.CompletedTask;
        }

    }
 
    ```


2. 更多信息请参考

    Bootstrap 风格的 Blazor UI 组件库
基于 Bootstrap 样式库精心打造，并且额外增加了 100 多种常用的组件，为您快速开发项目带来非一般的感觉

    <https://www.blazor.zone>


----
#### 更新历史

v7.1.0 
- 添加翻译组件

v6.1.2 
- 添加AI表格识别组件 


---
#### Blazor 组件

[条码扫描 ZXingBlazor](https://www.nuget.org/packages/ZXingBlazor#readme-body-tab)
[![nuget](https://img.shields.io/nuget/v/ZXingBlazor.svg?style=flat-square)](https://www.nuget.org/packages/ZXingBlazor) 
[![stats](https://img.shields.io/nuget/dt/ZXingBlazor.svg?style=flat-square)](https://www.nuget.org/stats/packages/ZXingBlazor?groupby=Version)

[图片浏览器 Viewer](https://www.nuget.org/packages/BootstrapBlazor.Viewer#readme-body-tab)
  
[条码扫描 BarcodeScanner](Densen.Component.Blazor/BarcodeScanner.md)
   
[手写签名 Handwritten](Densen.Component.Blazor/Handwritten.md)

[手写签名 SignaturePad](https://www.nuget.org/packages/BootstrapBlazor.SignaturePad#readme-body-tab)

[定位/持续定位 Geolocation](https://www.nuget.org/packages/BootstrapBlazor.Geolocation#readme-body-tab)

[屏幕键盘 OnScreenKeyboard](https://www.nuget.org/packages/BootstrapBlazor.OnScreenKeyboard#readme-body-tab)

[百度地图 BaiduMap](https://www.nuget.org/packages/BootstrapBlazor.BaiduMap#readme-body-tab)

[谷歌地图 GoogleMap](https://www.nuget.org/packages/BootstrapBlazor.Maps#readme-body-tab)

[蓝牙和打印 Bluetooth](https://www.nuget.org/packages/BootstrapBlazor.Bluetooth#readme-body-tab)

[PDF阅读器 PdfReader](https://www.nuget.org/packages/BootstrapBlazor.PdfReader#readme-body-tab)

[文件系统访问 FileSystem](https://www.nuget.org/packages/BootstrapBlazor.FileSystem#readme-body-tab)

[光学字符识别 OCR](https://www.nuget.org/packages/BootstrapBlazor.OCR#readme-body-tab)

[电池信息/网络信息 WebAPI](https://www.nuget.org/packages/BootstrapBlazor.WebAPI#readme-body-tab)

#### AlexChow

[今日头条](https://www.toutiao.com/c/user/token/MS4wLjABAAAAGMBzlmgJx0rytwH08AEEY8F0wIVXB2soJXXdUP3ohAE/?) | [博客园](https://www.cnblogs.com/densen2014) | [知乎](https://www.zhihu.com/people/alex-chow-54) | [Gitee](https://gitee.com/densen2014) | [GitHub](https://github.com/densen2014)


![ChuanglinZhou](https://user-images.githubusercontent.com/8428709/205942253-8ff5f9ca-a033-4707-9c36-b8c9950e50d6.png)
