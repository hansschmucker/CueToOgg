using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueToOgg
{
    public interface LogApplication
    {
        void FatalExit(string reason);
        void Exit();
        void Log(string message);
        void Alert(string message);
    }
}
