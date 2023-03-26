using Masa.Blazor;
using MAUIAndroidReleaseTool.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace MAUIAndroidReleaseTool.Pages
{
    public partial class Index
    {
        public class ReleaseModel
        {
            [Required]
            public string Path { get; set; }
            [Required]
            public string Framework { get; set; }
            [Required]
            public string Runtime { get; set; }
            [Required]
            public string Password { get; set; }
            public string Trimmed { get; set; }
            [Required]
            public string SelfContained { get; set; }
            [Required]
            public string KeystorePath { get; set; }
        }
        private ReleaseModel Release { get; set; } = new();
        private string CMD => $"dotnet publish -c:Release{Release.Runtime}{Release.Framework}{Release.Trimmed}{Release.SelfContained}{Keystore}";
        private string Keystore => $" /p:AndroidSigningKeyPass={Release.Password} /p:AndroidSigningStorePass={Release.Password}";
        private List<SelectItem> Frameworks = new()
        {
            new(".NET 7"," -f:net7.0-android" ),
            new(".NET 6"," -f:net6.0-android" )
        };
        private List<SelectItem> Runtimes = new()
        {
            new("arm64"," -r android-arm64" ),
            new ("x64"," -r android-x64" )
        };
        private List<SelectItem> SelfContaineds = new()
        {
            new("包含.NET运行时"," --sc" ),
            new ("不包含.NET运行时"," --no-self-contained" )
        };
        private List<SelectItem> Trimmeds = new()
        {
            new("剪裁"," -p:PublishTrimmed=true" ),
            new ("不剪裁","" )
        };
        private class SelectItem
        {
            public SelectItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public string Text { get; set; }
            public string Value { get; set; }
        }

        [Inject]
        private ISettingsService SettingsService { get; set; }
        [Inject]
        private ISystemService SystemService { get; set; }
        [Inject]
        private IPopupService PopupService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            Release.Path = await SettingsService.Get(SettingType.Path);
            Release.Runtime = await SettingsService.Get(SettingType.Runtime);
            Release.SelfContained = await SettingsService.Get(SettingType.SelfContained);
            Release.Framework = await SettingsService.Get(SettingType.Framework);
            Release.Trimmed = await SettingsService.Get(SettingType.Trimmed);
            Release.Password = await SettingsService.Get(SettingType.Password);

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
            await SettingsService.Save(SettingType.Path, Release.Path);
        }

        private void CreateKeystore()
        {
            string keyName = Path.Combine(Release.Path, "myapp.keystore");
            string cmd = $"keytool -genkey -v -keystore {keyName} -alias key -keyalg RSA -keysize 2048 -validity 10000";
            SystemService.RunCMD(cmd);
        }

        private async Task ReleaseItemChanged(SettingType type,string value)
        {
            await SettingsService.Save(type, value);
        }

        private async Task ReleaseStart()
        {
            await SettingsService.Save(SettingType.Password, Release.Password);
            string cmd = $"cd {Release.Path}&{CMD}";
            SystemService.RunCMD(cmd);
        }
    }
}
