using System;

namespace Epsiloner
{
    /// <summary>
    /// Disposable object abstract implementation.
    /// </summary>
    public abstract class DisposableObject : IDisposable, IDisposableStatus
    {
        /// <inheritdoc />
        public bool IsDisposed { get; private set; }


        /// <inheritdoc />
        public bool IsDisposing { get; private set; }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DisposableObject()
        {
            IsDisposing = true;
            if (IsDisposed)
                return;

            DisposeUnmanagedResources();
            IsDisposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            IsDisposing = true;
            if (!IsDisposed)
            {
                DisposeManagedResources();
                DisposeUnmanagedResources();
                IsDisposed = true;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes managed resources, implement if needed.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
        }

        /// <summary>
        /// Disposes un-managed resources, implement if needed.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }

        /// <summary>
        /// Dispose strict validator with exception.
        /// </summary>
        protected void CheckNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}