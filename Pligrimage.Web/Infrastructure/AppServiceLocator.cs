using Microsoft.Extensions.DependencyInjection;
using System;

namespace Pligrimage.Web.Infrastructure
{
    /// <summary>
    /// Provides a static accessor to the DI container.
    /// Used only by legacy static extension methods (IdentityExtensions)
    /// that cannot receive injected dependencies.
    /// </summary>
    public static class AppServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public static IServiceScope CreateScope()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException(
                    "AppServiceLocator has not been initialized. " +
                    "Call AppServiceLocator.Initialize(app.Services) in Program.cs.");

            return _serviceProvider.CreateScope();
        }
    }
}
