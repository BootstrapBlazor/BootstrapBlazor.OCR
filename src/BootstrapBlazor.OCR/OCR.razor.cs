// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Ocr.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BootstrapBlazor.Components;

/// <summary>
/// OCR 组件
/// </summary>
public partial class OCR
{
    public string url { get; set; } = "https://freepos.es/uploads/demo/Doc/libai.jpg";
    string? log;
    string? log2;
    [Inject] OcrService? OcrService { get; set; }

    /// <summary>
    /// 获得/设置 识别完成回调方法,返回 ReadResult
    /// </summary>
    [Parameter]
    public Func<List<ReadResult>, Task>? OnReadResult { get; set; }

    /// <summary>
    /// 获得/设置 识别完成回调方法,返回 string
    /// </summary>
    [Parameter]
    public Func<List<string>, Task>? OnResult { get; set; }

    /// <summary>
    /// 获得/设置 Url识别按钮文字 默认为 执行识别
    /// </summary>
    [Parameter]
    [NotNull]
    public string? OcrUrlButtonText { get; set; } = "执行识别";

    /// <summary>
    /// 获得/设置 拍照识别按钮文字 默认为 拍照
    /// </summary>
    [Parameter]
    [NotNull]
    public string? OcrCaptureButtonText { get; set; } = "拍照";

    /// <summary>
    /// 获得/设置 文件识别按钮文字 默认为 文件
    /// </summary>
    [Parameter]
    [NotNull]
    public string? OcrButtonText { get; set; } = "文件";


    /// <summary>
    /// 获得/设置 显示内置UI
    /// </summary>
    [Parameter]
    public bool ShowUI { get; set; }

    /// <summary>
    /// 获得/设置 显示内置UI
    /// </summary>
    [Parameter]
    public bool ShowUI_Capture { get; set; } = true;

    /// <summary>
    /// 获得/设置 显示内置识别URL的UI
    /// </summary>
    [Parameter]
    public bool ShowUI_Url { get; set; }

    /// <summary>
    /// 获得/设置 显示log
    /// </summary>
    [Parameter]
    public bool Debug { get; set; } 
    
    protected string? uploadstatus;
    long maxFileSize = 1024 * 1024 * 15;
    public List<ReadResult>? Results { get; set; }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                OcrService!.OnResult = OnResult1;
                OcrService!.Result = OnResult;
                OcrService.OnStatus = OnStatus;
                OcrService.OnError = OnError1;
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }
    protected async Task OnChange(InputFileChangeEventArgs e)
    {
        int i = 0;
        var selectedFiles = e.GetMultipleFiles(100);
        foreach (var item in selectedFiles)
        {
            i++;
            await OnSubmit(item);
            uploadstatus += Environment.NewLine + $"[{i}]: " + item.Name;
        }
    }

    protected async Task OnSubmit(IBrowserFile efile)
    {
        if (efile == null) return;
        try
        {
            using var stream = efile.OpenReadStream(maxFileSize);
            var res = await OcrService!.StartOcr(image: stream);
            //if (OnResult != null) await OnResult.Invoke(res);
            if (Debug)
            {
                log = "";
                res.ForEach(a => log += a + Environment.NewLine);
                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            log += "Error:" + e.Message + Environment.NewLine;
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    } 
    private void oninput(ChangeEventArgs e)
    {
        url = e.Value?.ToString()??"";
    }
    
    /// <summary>
    /// 识别文字
    /// </summary>
    public virtual async Task GetOCR()
    {
        try
        {
            var res = await OcrService!.StartOcr(url);
            //if (OnResult != null) await OnResult.Invoke(res);
            if (Debug)
            {
                log = "";
                res.ForEach(a => log += a + Environment.NewLine);
                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            log += "Error:" + e.Message + Environment.NewLine;
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }     

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    
    private async Task OnResult1(List<ReadResult> models)
    {
        this.Results = models;
        if (OnReadResult != null) await OnReadResult.Invoke(models);
        StateHasChanged();
    }
    private Task OnStatus(string message)
    {
        this.log2 = message;
        StateHasChanged();
        return Task.CompletedTask;
    }
    private Task OnError1(string message)
    {
        this.log2 = message;
        StateHasChanged();
        return Task.CompletedTask;
    }

}
