using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Ioc
{
    internal class IDSServiceScope : IServiceScope, IAsyncDisposable
    {
        private bool _disposed;
        private readonly AutofacServiceProvider _serviceProvider;

       
        public IDSServiceScope(ILifetimeScope lifetimeScope)
        {
         
            _serviceProvider = new AutofacServiceProvider(lifetimeScope);
        }
        public IServiceProvider ServiceProvider => _serviceProvider;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (disposing)
            {
                _serviceProvider.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;
                await _serviceProvider.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
