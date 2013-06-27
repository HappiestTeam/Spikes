using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Core.Diagnostics.Impl
{
    public class TraceLogger : ILogger
    {
        private const string MESSAGEFORMAT = "{0} - [{1}] - [{2}][{3}] {4} - {5}";
        private const string ERR = "ERR";
        private const string WARN = "WARN";
        private const string INFO = "INFO";
        private const string FATAL = "FATAL";
        private const string TRACE = "TRACE";
        private const string DEBUG = "DEBUG";

        #region ILogger Members

        public void Fatal(string className, object message)
        {
            this.Fatal(className, message, null);
        }

        public void Fatal(string className, object message, Exception exception)
        {
            System.Diagnostics.Trace.TraceError(String.Format(MESSAGEFORMAT, DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, FATAL, className,
                (message != null) ? message.ToString() : String.Empty,
                (exception != null) ? exception.ToString() : String.Empty));
            System.Diagnostics.Trace.Flush();
        }

        public void Error(string className, object message)
        {
            this.Error(className, message, null);
        }

        public void Error(string className, object message, Exception exception)
        {
            System.Diagnostics.Trace.TraceError(String.Format(MESSAGEFORMAT, DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, ERR, className,
                (message != null) ? message.ToString() : String.Empty,
                (exception != null) ? exception.ToString() : String.Empty));
            System.Diagnostics.Trace.Flush();
        }

        public void Warn(string className, object message)
        {
            this.Warn(className, message, null);
        }

        public void Warn(string className, object message, Exception exception)
        {
            System.Diagnostics.Trace.TraceWarning(String.Format(MESSAGEFORMAT, DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, WARN, className,
                (message != null) ? message.ToString() : String.Empty,
                (exception != null) ? exception.ToString() : String.Empty));
            System.Diagnostics.Trace.Flush();
        }

        public void Info(string className, object message)
        {
            this.Info(className, message, null);
        }

        public void Info(string className, object message, Exception exception)
        {
            System.Diagnostics.Trace.TraceInformation(String.Format(MESSAGEFORMAT, DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, INFO, className,
                (message != null) ? message.ToString() : String.Empty,
                (exception != null) ? exception.ToString() : String.Empty));
            System.Diagnostics.Trace.Flush();
        }

        public void Trace(string className, object message)
        {
            this.Trace(className, message, null);
        }

        public void Trace(string className, object message, Exception exception)
        {
            System.Diagnostics.Trace.TraceInformation(String.Format(MESSAGEFORMAT, DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, TRACE, className,
                (message != null) ? message.ToString() : String.Empty,
                (exception != null) ? exception.ToString() : String.Empty));
            System.Diagnostics.Trace.Flush();
        }

        public void Debug(string className, object message)
        {
            this.Debug(className, message, null);
        }

        public void Debug(string className, object message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(MESSAGEFORMAT, DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, DEBUG, className,
                (message != null) ? message.ToString() : String.Empty,
                (exception != null) ? exception.ToString() : String.Empty));
            System.Diagnostics.Debug.Flush();
        }

        #endregion
    }
}
