using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;

namespace IntegrationTests.Helpers
{
    /// <summary>
    /// Реализация контейнера postgresql без докера,
    /// на моём пок есть проблемы с docker,
    /// есть ноут с докером, но у него походу винч сдох (к слову о китайских ноутах)
    /// тестировать буду на ней
    /// </summary>
    public class TestPostgeSqlContaner(string connectionString) : IDatabaseContainer
    {
        private readonly string connectionString = connectionString;

        public string GetConnectionString()
        {
            return connectionString;
        }

        public async Task StartAsync(CancellationToken ct = default)
        {
            await new ValueTask();
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            await new ValueTask();
        }

        #region Это всё мы не используем)
        public DateTime CreatedTime => throw new NotImplementedException();

        public DateTime StartedTime => throw new NotImplementedException();

        public DateTime StoppedTime => throw new NotImplementedException();

        public DateTime PausedTime => throw new NotImplementedException();

        public DateTime UnpausedTime => throw new NotImplementedException();

        public ILogger Logger => throw new NotImplementedException();

        public string Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string IpAddress => throw new NotImplementedException();

        public string MacAddress => throw new NotImplementedException();

        public string Hostname => throw new NotImplementedException();

        public IImage Image => throw new NotImplementedException();

        public TestcontainersStates State => throw new NotImplementedException();

        public TestcontainersHealthStatus Health => throw new NotImplementedException();

        public long HealthCheckFailingStreak => throw new NotImplementedException();

        ILogger IContainer.Logger => throw new NotImplementedException();
#pragma warning disable CS0414
        public event EventHandler Creating = null!;
        public event EventHandler Starting = null!;
        public event EventHandler Stopping = null!;
        public event EventHandler Pausing = null!;
        public event EventHandler Unpausing = null!;
        public event EventHandler Created = null!;
        public event EventHandler Started = null!;
        public event EventHandler Stopped = null!;
        public event EventHandler Paused = null!;
        public event EventHandler Unpaused = null!;
#pragma warning restore CS0414

        public Task ConnectAsync(string network, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task ConnectAsync(INetwork network, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task CopyAsync(byte[] fileContent, string filePath, uint uid = 0, uint gid = 0, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task CopyAsync(string source, string target, uint uid = 0, uint gid = 0, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task CopyAsync(DirectoryInfo source, string target, uint uid = 0, uint gid = 0, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task CopyAsync(FileInfo source, string target, uint uid = 0, uint gid = 0, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString(ConnectionMode connectionMode = ConnectionMode.Host)
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString(string name, ConnectionMode connectionMode = ConnectionMode.Host)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetExitCodeAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public ushort GetMappedPublicPort()
        {
            throw new NotImplementedException();
        }

        public ushort GetMappedPublicPort(int containerPort)
        {
            throw new NotImplementedException();
        }

        public ushort GetMappedPublicPort(string containerPort)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<ushort, ushort> GetMappedPublicPorts()
        {
            throw new NotImplementedException();
        }

        public Task PauseAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UnpauseAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
