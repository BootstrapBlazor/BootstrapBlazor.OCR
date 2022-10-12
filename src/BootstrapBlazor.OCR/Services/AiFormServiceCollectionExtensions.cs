// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Ocr.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// AI Form 服务扩展类
    /// </summary>
    public static class AiFormServiceCollectionExtensions
    {

        /// <summary>
        /// 增加 AI Form 服务扩展类,<para></para>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="key"></param>
        /// <param name="url"></param> 
        /// <returns></returns>
        public static IServiceCollection AddAIFormExtensions(this IServiceCollection services,string key,string url)
        {
            services.AddTransient(sp => new AiFormService(key,url));
            return services;
        }

    }

}
