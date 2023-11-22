using System.Text.Json;

namespace MAUIAndroidReleaseTool.Services
{
    public class StaticWebAssets : IStaticWebAssets
    {
        private readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<T> ReadJsonAsync<T>(string relativePath)
        {
            var contents = await ReadContentAsync(relativePath).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(contents, JsonSerializerOptions) ?? throw new($"{relativePath} deserialize fail");
        }

        public async Task<string> ReadContentAsync(string relativePath)
        {
            string path = $"wwwroot/{relativePath}";
            bool exists = await FileSystem.AppPackageFileExistsAsync(path).ConfigureAwait(false);
            if (!exists)
            {
                throw new Exception($"not find {path}");
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync(path).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
