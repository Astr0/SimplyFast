using System;
using System.Collections.Generic;
using SF.Logging;

namespace SF.IoC
{
    internal class ModuleLoadLogger: ILogger
    {
        private readonly List<Action<ILogger>> _actions = new List<Action<ILogger>>();
        private Exception _fatal;

        public void Info(string message)
        {
            if (_fatal == null)
                _actions.Add(l => l.Info(message));
        }

        public void Debug(string message)
        {
            if (_fatal == null)
                _actions.Add(l => l.Debug(message));
        }

        public void Warning(string message)
        {
            if (_fatal == null)
                _actions.Add(l => l.Warning(message));
        }

        public void Error(string message, Exception ex = null)
        {
            if (_fatal == null)
                _actions.Add(l => l.Error(message, ex));
        }

        public void Fatal(string message, Exception ex = null)
        {
            if (_fatal == null)
            {
                _actions.Add(l => l.Fatal(message, ex));
                _fatal = new ApplicationException(message, ex);
            }
        }

        public void ThrowIfFatal()
        {
            if (_fatal != null)
                throw _fatal;
        }

        public void ReplayTo(ILogger logger)
        {
            foreach (var action in _actions)
            {
                action(logger);
            }
        }
    }
}