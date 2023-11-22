namespace MAUIAndroidReleaseTool.Services
{
    public interface ISystemService
    {
        public Task<string> PickCsprojFileAsync();

        public void RunCMD(string cmd);
    }
}
