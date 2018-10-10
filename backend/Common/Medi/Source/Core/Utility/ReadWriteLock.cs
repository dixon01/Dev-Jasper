// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadWriteLock.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadWriteLock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Utility
{
    using System;
    using System.Threading;

    /// <summary>
    /// Wrapper for <see cref="ReaderWriterLock"/> that allows to use
    /// the <code>using</code> statement for releasing a lock.
    /// </summary>
    public partial class ReadWriteLock
    {
        private class Releaser : IDisposable
        {
            private readonly ThreadStart task;

            public Releaser(ThreadStart task)
            {
                this.task = task;
            }

            public void Dispose()
            {
                this.task();
            }
        }
    }
}
