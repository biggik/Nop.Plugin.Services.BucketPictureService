using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.BucketPictureService.Services;

public partial class BucketPictureService : PictureService
{
#if (!NOP_47 && !NOP_48)
    private readonly INopFileProvider _fileProvider;
#endif
    public BucketPictureService(
        IDownloadService downloadService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        INopFileProvider fileProvider,
        IProductAttributeParser productAttributeParser,
#if (NOP_47 || NOP_48)
        IProductAttributeService productAttributeService,
#endif
        IRepository<Picture> pictureRepository,
        IRepository<PictureBinary> pictureBinaryRepository,
        IRepository<ProductPicture> productPictureRepository,
        ISettingService settingService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        MediaSettings mediaSettings)
        : base(downloadService,
               httpContextAccessor,
               logger,
               fileProvider,
               productAttributeParser,
#if (NOP_47 || NOP_48)
               productAttributeService,
#endif
               pictureRepository,
               pictureBinaryRepository,
               productPictureRepository,
               settingService,
               urlRecordService,
               webHelper,
               mediaSettings)
    {
#if (!NOP_47 && !NOP_48)
        _fileProvider = fileProvider;
#endif
    }

    /// <inheritdoc />
    protected override async Task DeletePictureThumbsAsync(Picture picture)
    {
        // Drill down the actual folder used
        var fullThumbPath = await GetThumbLocalPathAsync(picture.Id, "what.ever");

        var filter = $"{picture.Id:0000000}*.*";
        var currentFiles = _fileProvider.GetFiles(_fileProvider.GetDirectoryName(fullThumbPath), filter, false);
        foreach (var currentFileName in currentFiles)
        {
            var thumbFilePath = await GetThumbLocalPathAsync(picture.Id, currentFileName);
            _fileProvider.DeleteFile(thumbFilePath);
        }
    }

    /// <inheritdoc />
    protected override Task<string> GetThumbLocalPathAsync(string thumbFileName)
    {
        return GetThumbLocalPathAsync(IdFromName(thumbFileName), thumbFileName);
    }

    private Task<string> GetThumbLocalPathAsync(int pictureId, string thumbFileName)
    {
        var thumbsDirectoryPath = _fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath);

        // Put thumbs into 'buckets' - per 10k ids and 100 in each bucket within those
        var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(thumbFileName);
        if (pictureId >= 0 && fileNameWithoutExtension != null)
        {
            var subDirectoryName = SubDirectoryFromPictureId(pictureId);
            thumbsDirectoryPath = _fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath, subDirectoryName);
            _fileProvider.CreateDirectory(thumbsDirectoryPath);
        }

        var thumbFilePath = _fileProvider.Combine(thumbsDirectoryPath, thumbFileName);
        return Task.FromResult(thumbFilePath);
    }

    /// <inheritdoc />
    protected override Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
    {
        return GetThumbUrlAsync(IdFromName(thumbFileName), thumbFileName, storeLocation);
    }

    private async Task<string> GetThumbUrlAsync(int pictureId, string thumbFileName, string storeLocation = null)
    {
        var url = await GetImagesPathUrlAsync(storeLocation) + "thumbs/";

        var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(thumbFileName);
        if (pictureId >= 0 && fileNameWithoutExtension != null)
        {
            var subDirectoryName = SubDirectoryFromPictureId(pictureId, '/');
            url = url + subDirectoryName + "/";
        }

        url += thumbFileName;
        return url;
    }

    private static readonly Regex idRegex = new("(?<id>[0-9]+)_.*");
    private static int IdFromName(string thumbFileName)
    {
        var match = idRegex.Match(thumbFileName);
        if (match.Success)
        {
            var id = match.Groups["id"];
            if (id?.Value != null && int.TryParse(id.Value, out int foundId))
            {
                return foundId;
            }
        }
        return -1;
    }

    private static string SubDirectoryFromPictureId(int pictureId, char pathSeparator = '\\')
    {
        int first = pictureId - pictureId % 10000;
        int second = (pictureId - pictureId % 100) - first;
        first /= 1000;

        return $"{first}{pathSeparator}{second}";
    }
}
