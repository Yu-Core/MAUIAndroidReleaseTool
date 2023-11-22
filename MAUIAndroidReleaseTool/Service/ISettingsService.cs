
using MAUIAndroidReleaseTool.Models;

namespace MAUIAndroidReleaseTool.Services
{
    public interface ISettingsService
    {
        Task<T> Get<T>(Setting type);
        Task<T> Get<T>(Setting type, T defaultValue);
        Task<T> Get<T>(string key, T defaultValue);
        Task Save<T>(Setting type, T value);
        Task Save<T>(string key, T value);
        Task<bool> ContainsKey(string key);
    }
}
