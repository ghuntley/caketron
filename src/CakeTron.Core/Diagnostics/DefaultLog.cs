using System;

namespace CakeTron.Core.Diagnostics
{
    internal sealed class DefaultLog : ILog
    {
        public void Write(LogLevel level, string format, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
