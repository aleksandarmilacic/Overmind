using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overmind.Interfaces
{
    public interface IGameWindowCapture
    {
        string CaptureGameWindowAsBase64();
    }
}
