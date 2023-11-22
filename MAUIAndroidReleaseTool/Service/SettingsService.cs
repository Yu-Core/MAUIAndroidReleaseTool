using MAUIAndroidReleaseTool.Models;
using System.Text.Json;

namespace MAUIAndroidReleaseTool.Services
{
    public class SettingsService : ISettingsService
    {
        private Lazy<Dictionary<Setting, object>> _defalutSettings;

        private Dictionary<Setting, object> DefalutSettings => _defalutSettings.Value;

        public SettingsService(IStaticWebAssets staticWebAssets)
        {
            _defalutSettings = new(()=> staticWebAssets.ReadJsonAsync<Dictionary<Setting, object>>("json/default-settings.json").Result);
        }

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

        public Task<T> Get<T>(Setting type)
        {
            if (DefalutSettings[type] is JsonElement element)
            {
                DefalutSettings[type] = element.Deserialize<T>();
            }

            return Get(type, (T)DefalutSettings[type]);
        }

        public Task<T> Get<T>(Setting type, T defaultValue)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Get(key!, defaultValue);
        }

        public Task Save<T>(string key, T value)
        {
            Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }

        public Task Save<T>(Setting type, T value)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Save(key!, value);
        }
    }
}
