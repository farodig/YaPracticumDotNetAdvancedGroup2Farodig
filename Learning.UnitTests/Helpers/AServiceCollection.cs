using LearningWebApi;
using LearningWebApi.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Learning.UnitTests.Helpers
{
    public abstract class AServiceCollection : IDisposable
    {
        public AServiceCollection()
        {
            var services = new ServiceCollection();

            var dbName = Guid.NewGuid().ToString();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(dbName));
            services.AddEventService();
            services.AddBookingService();

            ServiceProvider = services.BuildServiceProvider();
            Scope = ServiceProvider.CreateScope();

            var context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }

        protected void Initialize<T>(params IEnumerable<T> collection) where T : class
        {
            var context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            foreach (var item in collection)
            {
                context.Add(item);
            }
            context.SaveChanges();
        }

        protected T GetService<T>() where T : class
            => Scope.ServiceProvider.GetRequiredService<T>();

        protected T? GetHostedService<T>() where T : class, IHostedService
            => Scope.ServiceProvider.GetServices<IHostedService>()
            .OfType<T>()
            .FirstOrDefault();

        protected TService GetInitializedService<TService, T>(params IEnumerable<T> collection)
            where TService : class
            where T : class
        {
            Initialize(collection);
            return GetService<TService>();
        }

        protected (TService1, TService2) GetInitializedServices<TService1, TService2, T>(params IEnumerable<T> collection)
            where TService1 : class
            where TService2 : class
            where T : class
        {
            Initialize(collection);
            return (GetService<TService1>(), GetService<TService2>());
        }

        protected (TService1, TService2, TService3) GetInitializedServices<TService1, TService2, TService3, T>(params IEnumerable<T> collection)
            where TService1 : class
            where TService2 : class
            where TService3 : class
            where T : class
        {
            Initialize(collection);
            return (GetService<TService1>(), GetService<TService2>(), GetService<TService3>());
        }

        protected readonly ServiceProvider ServiceProvider;

        protected readonly IServiceScope Scope;

        #region IDisposable
        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Scope.Dispose();
                    ServiceProvider.Dispose();
                }

                _disposed = true;
            }
        }

        ~AServiceCollection()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}
