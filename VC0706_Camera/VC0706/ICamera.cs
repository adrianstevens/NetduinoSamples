using System;
using Microsoft.SPOT;

namespace VC0706
{
    public interface ICamera
    {
        void TakePicture(string path);
        string GetVersion();
    }
}
