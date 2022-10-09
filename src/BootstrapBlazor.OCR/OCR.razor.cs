// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Ocr.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazor.Components;

/// <summary>
/// OCR 组件
/// </summary>
public partial class OCR : IAsyncDisposable
{
    string url { get; set; } = "https://freepos.es/uploads/demo/Doc/libai.jpg";
    string log;
    string log2;
    [Inject] IJSRuntime? JS { get; set; }
    [Inject] OcrService? OcrService { get; set; }
    private IJSObjectReference? module;
    private DotNetObjectReference<OCR>? InstanceOcr { get; set; }

    /// <summary>
    /// UI界面元素的引用对象
    /// </summary>
    public ElementReference OcrElement { get; set; }

    /// <summary>
    /// 获得/设置 识别按钮文字 默认为 识别文字
    /// </summary>
    [Parameter]
    [NotNull]
    public string? PrintButtonText { get; set; } = "执行识别";

    /// <summary>
    /// 获得/设置 OcrOption
    /// </summary>
    [Parameter]
    public OcrOption Opt { get; set; } = new OcrOption(); 

    /// <summary>
    /// 获得/设置 状态更新回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnUpdateStatus { get; set; }

    /// <summary>
    /// 获得/设置 错误更新回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnUpdateError { get; set; }

    /// <summary>
    /// 可用已配对设备列表
    /// </summary>
    public List<string>? Devices;

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

    /// <summary>
    /// 获得/设置 设备名称
    /// </summary>
    [Parameter]
    public string? Devicename { get; set; }
    
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
            //await module!.InvokeVoidAsync("ocrFunction", InstanceOcr, OcrElement, Opt, "write", Commands);
            StateHasChanged();
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
        StateHasChanged();
    }
    
    /// <summary>
    /// 打印
    /// </summary>
    public virtual async Task Print()
    {
        try
        {
            var res = await OcrService!.StartOcr(url);
            log = "";
            res.ForEach(a => log += a + Environment.NewLine);
            //await module!.InvokeVoidAsync("ocrFunction", InstanceOcr, OcrElement, Opt, "write", Commands);
            StateHasChanged();
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        InstanceOcr?.Dispose();
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }

    /// <summary>
    /// 连接完成回调方法
    /// </summary>
    /// <param name="opt"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetResult(OcrOption opt,string status)
    {
        try
        {
            Opt = opt;
            if (OnResult != null) await OnResult.Invoke($"{opt.Devicename}{status}");
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 获得/设置 连接完成回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnResult { get; set; }

    /// <summary>
    /// 获取已配对设备回调方法
    /// </summary>
    /// <param name="devices"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetDevices(List<string>? devices)
    {
        try
        {
            Devices = devices;
            if (OnGetDevices != null) await OnGetDevices.Invoke(devices);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 获得/设置 获取已配对设备回调方法
    /// </summary>
    [Parameter]
    public Func<List<string>?, Task>? OnGetDevices { get; set; }

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }
 
    /// <summary>
    /// 状态更新回调方法
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task UpdateStatus(string status)
    {
        if (OnUpdateStatus != null) await OnUpdateStatus.Invoke(status);
    }

    /// <summary>
    /// 错误更新回调方法
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task UpdateError(string status)
    {
        if (OnUpdateError != null) await OnUpdateError.Invoke(status);
    }
 

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
