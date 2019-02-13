using System;
using SimplyFast.Log.Messages;

namespace SimplyFast.Log
{
    public interface IOutput : IDisposable
    {
        void Log(IMessage message);
    }
}