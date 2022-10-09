// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Ocr.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                OcrService.OnResult = OnResult1;
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
            var res = await OcrService!.StartOcr(url, stream);
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
        url = e.Value.ToString();
    }
    
    /// <summary>
    /// 识别文字
    /// </summary>
    public virtual async Task GetOCR()
    {
        try
        {
            var res = await OcrService!.StartOcr(url);
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
  
    private Task OnResult1(string message)
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
