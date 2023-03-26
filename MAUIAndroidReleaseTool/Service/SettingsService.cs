
namespace MAUIAndroidReleaseTool.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly Dictionary<SettingType, dynamic> Settings = new()
        {
            {SettingType.Path,"" },
            {SettingType.Runtime," -r android-arm64" },
            {SettingType.Password,"" },
            {SettingType.Framework," -f:net7.0-android" },
            {SettingType.Trimmed," -p:PublishTrimmed=true" },
            {SettingType.SelfContained," --sc" },
        };
        private Dictionary<SettingType, object> DefalutSettings = new();

        public Task<bool> ContainsKey(string key)
        {
            var result = Preferences.Default.ContainsKey(key);
            return Task.FromResult(result);
        }

        public Task<T> Get<T>(string key, T defaultValue)
        {
            var result = Preferences.Default.Get(key, defaultValue);
            return Task.FromResult(result);
        }

        public async Task<dynamic> Get(SettingType type)
        {
            var defaultValue = Settings[type];
            return await Get(type, defaultValue);
        }

        public Task<T> Get<T>(SettingType type)
        {
            var defaultValue = Settings[type];
            return Get(type, defaultValue);
        }

        public Task<T> Get<T>(SettingType type, T defaultValue)
        {
            var key = Enum.GetName(typeof(SettingType), type);
            return Get(key!, defaultValue);
        }

        public T GetDefault<T>(SettingType type)
        {
            return (T)DefalutSettings[type];
        }

        public async Task InitDefault<T>(SettingType type)
        {
            T value = await Get<T>(type);
            DefalutSettings.Add(type, value!);
        }

        public Task Save<T>(string key, T value)
        {
            Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }

        public Task Save<T>(SettingType type, T value)
        {
            var key = Enum.GetName(typeof(SettingType), type);
            return Save(key!, value);
        }
    }
}
