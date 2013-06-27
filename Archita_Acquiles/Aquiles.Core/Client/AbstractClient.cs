using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Connection.Pooling;
using Aquiles.Core.Model;
using System.Globalization;

namespace Aquiles.Core.Client
{
    public abstract class AbstractClient : IClient
    {
        #region IClient Members

        public DateTime Created
        {
            get; 
            set;
        }

        public virtual IClientPool OwnerPool
        {
            get;
            set;
        }

        public abstract string KeyspaceName { get; set; }

        public virtual IEndpoint Endpoint
        {
            get;
            set;
        }

        public abstract void Open();

        public abstract void Close();

        public abstract bool IsOpen();

        public abstract Object Execute(Delegate executionBlock);

        public abstract string getClusterName();

        public abstract void OverrideConnectionConfig(ConnectionConfig config);
        
        public abstract void ResetToDefaultConnectionConfig();

        #region IDisposable Members
        protected bool disposed = false;
        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool forceDispose)
        {
            this.disposed = true;
            this.OwnerPool = null;
        }
        #endregion

        #endregion

        protected static string BuildExceptionMessage(Exception ex)
        {
            return BuildExceptionMessage(ex, null);
        }
        protected static string BuildExceptionMessage(Exception ex, string why)
        {
            if (String.IsNullOrEmpty(why))
            {
                return String.Format(CultureInfo.CurrentCulture, "Exception '{0}' during executing command. See inner exception for further details.", ex.Message);
            }
            else
            {
                return String.Format(CultureInfo.CurrentCulture, "Exception '{0}' during executing command: '{1}'. See inner exception for further details.", ex.Message, why);
            }
        }

        protected bool Invalid
        {
            get;
            set;
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", base.ToString(), this.GetHashCode());
        }
    }
}
