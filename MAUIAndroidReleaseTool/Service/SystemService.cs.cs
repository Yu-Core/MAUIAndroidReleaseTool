using System.Diagnostics;

namespace MAUIAndroidReleaseTool.Services
{
    public class SystemService : ISystemService
    {
        public Task<string> PickCsprojFileAsync()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.xml" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/xml" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".csproj" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "public.xml" } }, // UTType values
                });

            PickOptions options = new()
            {
                FileTypes = customFileType,
            };
            return PickFileAsync(options, "csproj");
        }

        private static Task<string> PickFileAsync(PickOptions options, string suffixName)
        {
            string[] suffixNames = { suffixName };
            return PickFileAsync(options, suffixNames);
        }

        private async static Task<string> PickFileAsync(PickOptions options, string[] suffixNames)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    bool flag = false;
                    foreach (var suffixName in suffixNames)
                    {
                        if (result.FileName.EndsWith(suffixName, StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag)
                    {
                        return result.FullPath;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public void RunCMD(string cmd)
        {
#if WINDOWS
            Task.Run(() => {

            cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            
            Process process = new Process();

            process.StartInfo.FileName = "cmd.exe";

            process.StartInfo.Arguments = "/c " + cmd;

            process.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动

            process.StartInfo.CreateNoWindow = false; //是否在新窗口中启动该进程的值 (不显示程序窗口)
            
            process.Start();

            process.WaitForExit(); //等待程序执行完退出进程

            process.Close();
        });
#endif
        }
    }
}
