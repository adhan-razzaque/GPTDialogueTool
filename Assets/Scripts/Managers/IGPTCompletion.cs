using System;

namespace Managers
{
    public interface IGPTCompletion
    {
        public void Execute(string prompt, Action<string> responseHandler, bool storeMessage = false);
    }
}