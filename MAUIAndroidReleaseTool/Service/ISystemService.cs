using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIAndroidReleaseTool.Services
{
    public interface ISystemService
    {
        public Task<string> PickCsprojFileAsync();
        public void RunCMD(string cmd);
    }
}
