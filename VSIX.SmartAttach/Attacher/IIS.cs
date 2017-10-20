using System;
using System.Linq;
using Microsoft.Web.Administration;

namespace GeeksAddin.Attacher
{
    internal class IIS : IDisposable
    {
        ServerManager ServerManager;

        public IIS()
        {
            try
            {
                ServerManager = new ServerManager();
            }
            catch
            {
                ServerManager = null;
            }
        }

        public bool CanUseIIS => ServerManager != null;

        public string GetPhysicalPath(string poolName)
        {
            if (!CanUseIIS)
                return null;

            try
            {
                var applications = ServerManager.Sites.SelectMany(s => s.Applications);
                var application = applications.FirstOrDefault(app => String.Compare(app.ApplicationPoolName, poolName, ignoreCase: true) == 0);
                if (application == null) return null;

                return application.VirtualDirectories["/"].PhysicalPath;
            }
            catch { }

            return null;
        }

        #region IDisposable Members

        bool disposed = false;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                disposed = true;
            }
        }

        ~IIS()
        {
            Dispose(disposing: false);
        }
        #endregion
    }
}
