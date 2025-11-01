using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.BucketPictureService.Services;

public class NopStartup : INopStartup
{
    public int Order => 16384;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
#if NOP_49
        services.AddScoped<IThumbService, BucketThumbPictureService>();
#else
        // Override the default picture service
        services.AddScoped<IPictureService, BucketPictureService>();
#endif
    }
}
