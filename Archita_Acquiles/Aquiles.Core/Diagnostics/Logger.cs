using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Aquiles.Core.Diagnostics
{
    public class LoggerManager
    {
        public ILogger Instance
        {
            get;
            set;
        }

        private string identifier;
        public string Identifier
        {
            get { return this.identifier; }
            set { this.identifier = value; this.calculateFinalIdentifier(); }
        }

        private void calculateFinalIdentifier()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.clazz);
            if (this.identifier != null) 
            {
                sb.AppendFormat("_{0}", this.identifier); 
            }
            this._finalIdentifier = sb.ToString();
        } 

        private string clazz;
        private string _finalIdentifier;

        public LoggerManager(string clazz)
        {
            if (clazz == null)
            {
                throw new ArgumentNullException("clazz parameter cannot be null.");
            }
            this.clazz = clazz;
        }

        public static string BuildIdentifier(Type type)
        {
            return type.ToString();
        }

        public string StringFormatInvariantCulture(string pattern, params object[] obj)
        {
            return String.Format(CultureInfo.InvariantCulture, pattern, obj);
        }

        public void Fatal(object message)
        {
            if (this.Instance != null)
            {
                this.Instance.Fatal(this._finalIdentifier, message);
            }
        }

        public void Fatal(object message, Exception exception)
        {
            if (this.Instance != null)
            {
                this.Instance.Fatal(this._finalIdentifier, message, exception);
            }
        }

        public void Error(object message)
        {
            if (this.Instance != null)
            {
                this.Instance.Error(this._finalIdentifier, message);
            }
        }

        public void Error(object message, Exception exception)
        {
            if (this.Instance != null)
            {
                this.Instance.Error(this._finalIdentifier, message, exception);
            }
        }

        public void Warn(object message)
        {
            if (this.Instance != null)
            {
                this.Instance.Warn(this._finalIdentifier, message, null);
            }
        }

        public void Warn(object message, Exception exception)
        {
            if (this.Instance != null)
            {
                this.Instance.Warn(this._finalIdentifier, message, exception);
            }
        }

        public void Info(object message)
        {
            if (this.Instance != null)
            {
                this.Instance.Info(this._finalIdentifier, message);
            }
        }

        public void Info(object message, Exception exception)
        {
            if (this.Instance != null)
            {
                this.Instance.Info(this._finalIdentifier, message, exception);
            }
        }

        public void Trace(object message)
        {
            if (this.Instance != null)
            {
                this.Instance.Trace(this._finalIdentifier, message);
            }
        }

        public void Trace(object message, Exception exception)
        {
            if (this.Instance != null)
            {
                this.Instance.Trace(this._finalIdentifier, message, exception);
            }
        }

        public void Debug(object message)
        {
            if (this.Instance != null)
            {
                this.Instance.Debug(this._finalIdentifier, message, null);
            }
        }

        public void Debug(object message, Exception exception)
        {
            if (this.Instance != null)
            {
                this.Instance.Debug(this._finalIdentifier, message, exception);
            }
        }

        internal static LoggerManager CreateLoggerManager(Type type)
        {
            return new LoggerManager(LoggerManager.BuildIdentifier(type));
        }
    }
}
