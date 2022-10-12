// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Azure.AI.FormRecognizer.DocumentAnalysis;
using BootstrapBlazor.Ocr.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BootstrapBlazor.Components;

/// <summary>
/// AI表格识别 AI Form 组件
/// </summary>
public partial class AiForm
{
    public string url { get; set; } = "https://freepos.es/uploads/demo/Doc/ticket.jpg";
    string? log;
    string? log2;
    [Inject] AiFormService? AiFormService { get; set; }

    /// <summary>
    /// 获得/设置 识别完成回调方法,返回 ReadResult
    /// </summary>
    [Parameter]
    public Func<List<AnalyzedDocument>, Task>? OnReadResult { get; set; }

    /// <summary>
    /// 获得/设置 识别完成回调方法,返回 string
    /// </summary>
    [Parameter]
    public Func<List<string>, Task>? OnResult { get; set; }

    /// <summary>
    /// 获得/设置 识别按钮文字 默认为 识别文字
    /// </summary>
    [Parameter]
    [NotNull]
    public string? OcrButtonText { get; set; } = "执行识别";


    /// <summary>
    /// 获得/设置 显示内置UI
    /// </summary>
    [Parameter]
    public bool ShowUI { get; set; }

    /// <summary>
    /// 获得/设置 显示log
    /// </summary>
    [Parameter]
    public bool Debug { get; set; } 
    
    protected string? uploadstatus;
    long maxFileSize = 1024 * 1024 * 15;
    public List<AnalyzedDocument>? Results { get; set; }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                AiFormService!.OnResult = OnResult1;
                AiFormService.OnStatus = OnStatus;
                AiFormService.OnError = OnError1;
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
            var res = await AiFormService!.ReadFileUrl(image: stream);
            log = "";
            res.ForEach(a => log += a + Environment.NewLine);
            StateHasChanged();
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
        StateHasChanged();
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
            var res = await AiFormService!.ReadFileUrl(url);
            if (OnResult != null) await OnResult.Invoke(res);
            log = "";
            res.ForEach(a => log += a + Environment.NewLine);
            StateHasChanged();
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }     

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }
  
    private async Task OnResult1(List<AnalyzedDocument> models)
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
