using ConSurvBackend.Core.Services;
using GRYLibrary.Core.Misc.CustomDisposables;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Tests.TestUtilities
{
    internal class PersistenceDisposable : CustomDisposable
    {
        public IPersistence Persistence { get; private set; }
        public PersistenceDisposable(IPersistence persistence, ISet<IDisposable> disposables) : base(() =>
        {
            persistence.Dispose();
            foreach (IDisposable disposable in disposables)
            {
                disposable.Dispose();
            }
        })
        {
            this.Persistence = persistence;
        }
    }
}
