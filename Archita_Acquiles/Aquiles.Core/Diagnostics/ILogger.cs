using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Core.Diagnostics
{
    /// <summary>
    /// Interface with the required methods to be able to log over Aquiles 
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Fatal information before crashing
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        void Fatal(string className, object message);
        /// <summary>
        /// Fatal information before crashing
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        void Fatal(string className, object message, Exception exception);
        /// <summary>
        /// Error information (this might be overcome)
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        void Error(string className, object message);
        /// <summary>
        /// Error information (this might be overcome)
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        void Error(string className, object message, Exception exception);
        /// <summary>
        /// Warning information (something went wrong, it was supposed to go right).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        void Warn(string className, object message);
        /// <summary>
        /// Warning information (something went wrong, it was supposed to go right).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        void Warn(string className, object message, Exception exception);
        /// <summary>
        /// information (something we must be aware of).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        void Info(string className, object message);
        /// <summary>
        /// information (something we must be aware of).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        void Info(string className, object message, Exception exception);
        /// <summary>
        /// Trace (something we migth consider useful on runtime).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        void Trace(string className, object message);
        /// <summary>
        /// Trace (something we migth consider useful on runtime).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        void Trace(string className, object message, Exception exception);
        /// <summary>
        /// Debug (something we migth consider useful on runtime).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        void Debug(string className, object message);
        /// <summary>
        /// Debug (something we migth consider useful on runtime).
        /// </summary>
        /// <param name="className">classname</param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        void Debug(string className, object message, Exception exception);
    }
}
