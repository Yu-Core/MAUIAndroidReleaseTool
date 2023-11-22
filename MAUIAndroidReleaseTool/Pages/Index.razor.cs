using Masa.Blazor;
using MAUIAndroidReleaseTool.Models;
using MAUIAndroidReleaseTool.Services;
using Microsoft.AspNetCore.Components;

namespace MAUIAndroidReleaseTool.Pages
{
    public partial class Index
    {
        private ReleaseModel Release { get; set; } = new();

        private List<SelectItem> Frameworks = new();

        private List<SelectItem> Runtimes = new();

        private List<SelectItem> Trimmeds = new();

        private string DefaultKeystoreFileName = "myapp.keystore";

        [Inject]
        private IStaticWebAssets StaticWebAssets { get; set; } = default!;

        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;

        [Inject]
        private ISystemService SystemService { get; set; } = default!;

        [Inject]
        private IPopupService PopupService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadDefaultSelectItemsAsync();
            await LoadSettingsAsync();
            await base.OnInitializedAsync();
        }

        private string CMD => $"dotnet publish -c:Release -p:AndroidPackageFormat=apk{Release.Runtime}{Release.Framework}{Release.Trimmed}{KeystoreCommand}";

        private string KeystoreFileName => Path.GetFileName(Release.KeystorePath);

        private string KeystoreCommand
            => string.IsNullOrWhiteSpace(Release.KeystorePath) ? "" : $" -p:AndroidKeyStore=true -p:AndroidSigningKeyStore={KeystoreFileName} -p:AndroidSigningKeyAlias=key -p:AndroidSigningKeyPass={Release.Password} -p:AndroidSigningStorePass={Release.Password}";

        private async Task LoadDefaultSelectItemsAsync()
        {
            Frameworks = await StaticWebAssets.ReadJsonAsync<List<SelectItem>>("json/frameworks.json");
            Runtimes = await StaticWebAssets.ReadJsonAsync<List<SelectItem>>("json/runtimes.json");
            Trimmeds = await StaticWebAssets.ReadJsonAsync<List<SelectItem>>("json/trimmeds.json");
        }

        private async Task LoadSettingsAsync()
        {
            Release.Path = await SettingsService.Get<string>(Setting.Path);
            Release.Runtime = await SettingsService.Get<string>(Setting.Runtime);
            Release.Framework = await SettingsService.Get<string>(Setting.Framework);
            Release.Trimmed = await SettingsService.Get<string>(Setting.Trimmed);
            Release.Password = await SettingsService.Get<string>(Setting.Password);

            await SetKeystore();
        }
        private async Task SetKeystore()
        {
            if (Directory.Exists(Release.Path))
            {
                DirectoryInfo TheFolder = new DirectoryInfo(Release.Path);
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                {
                    if (NextFile.Name.Substring(NextFile.Name.LastIndexOf(".")) == ".keystore")
                    {
                        Release.KeystorePath = NextFile.FullName;
                        return;
                    }
                }
                Release.KeystorePath = string.Empty;
                await PopupService.EnqueueSnackbarAsync("请创建密钥存储文件");
            }
        }

        private async Task PickCsprojFile()
        {
            var path = await SystemService.PickCsprojFileAsync();
            if (string.IsNullOrEmpty(path))
                return;
            Release.Path = Path.GetDirectoryName(path);
            await SetKeystore();
            await SettingsService.Save(Setting.Path, Release.Path);
        }

        private void CreateKeystore()
        {
            string keyName = Path.Combine(Release.Path, DefaultKeystoreFileName);
            string cmd = $"keytool -genkey -v -keystore {keyName} -alias key -keyalg RSA -keysize 2048 -validity 10000";
            SystemService.RunCMD(cmd);
        }

        private async Task ReleaseItemChanged(Setting type, string value)
        {
            await SettingsService.Save(type, value);
        }

        private async Task ReleaseStart()
        {
            await SettingsService.Save(Setting.Password, Release.Password);
            string cmd = $"cd {Release.Path}&{CMD}";
            SystemService.RunCMD(cmd);
        }
    }
}
