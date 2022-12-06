## Blazor 光学字符识别 (OCR) 组件

### 示例

https://www.blazor.zone/ocr

https://blazor.app1.es/ocr

## 使用方法:

1. nuget包

    ```BootstrapBlazor.OCR```

2. _Imports.razor 文件 或者页面添加 添加组件库引用

    ```@using BootstrapBlazor.Components```

3. Program.cs 文件添加

    ```
    builder.Services.AddOcrExtensions("YourAzureCvKey", "YourAzureCvEndpoint");
    ```

4. Razor页面

    Razor  
    <https://github.com/densen2014/Densen.Extensions/blob/master/Demo/DemoShared/Pages/OcrPage.razor>

    ```
    @using BootstrapBlazor.Components
    
    <OCR ShowUI="true" ShowUI_Capture="true" Debug="true" OnResult="OnResult" />

    @code{
        List<string> res { get; set; }
        private Task OnResult(List<string> res)
        {
            this.res = res;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
 
    ```


2. 更多信息请参考

    Bootstrap 风格的 Blazor UI 组件库
基于 Bootstrap 样式库精心打造，并且额外增加了 100 多种常用的组件，为您快速开发项目带来非一般的感觉

    <https://www.blazor.zone>

    <https://www.blazor.zone/ocr>

----

## Blazor OCR component

### Demo

https://www.blazor.zone/ocr

https://blazor.app1.es/ocr

## Instructions:

1. NuGet install pack 

    `BootstrapBlazor.OCR`

2. _Imports.razor or Razor page

   ```
   @using BootstrapBlazor.Components
   ```
   
3. Program.cs

    ```
    builder.Services.AddOcrExtensions("YourAzureCvKey", "YourAzureCvEndpoint");
    ```
 
4. Razor page

    Razor  
    <https://github.com/densen2014/Densen.Extensions/blob/master/Demo/DemoShared/Pages/OcrPage.razor>

    ```
    @using BootstrapBlazor.Components
    
    <OCR ShowUI="true" ShowUI_Capture="true" Debug="true" OnResult="OnResult" />

    @code{
        List<string> res { get; set; }
        private Task OnResult(List<string> res)
        {
            this.res = res;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
    
    ```
    

2.  More informations

    Bootstrap style Blazor UI component library
Based on the Bootstrap style library, it is carefully built, and 100 a variety of commonly used components have been added to bring you an extraordinary feeling for rapid development projects

    <https://www.blazor.zone>

    <https://www.blazor.zone/ocr>


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
