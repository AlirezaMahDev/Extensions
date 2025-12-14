using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions.Node;

partial class NodeWorker<TNodeService, TNodeServiceOptions>(
    TNodeService nodeService,
    IServiceProvider serviceProvider,
    IOptionsMonitor<TNodeServiceOptions> optionsMonitor,
    ILogger<NodeWorker<TNodeService, TNodeServiceOptions>> logger)
    : BackgroundService
    where TNodeService : INodeService
    where TNodeServiceOptions : NodeOptions
{
    private readonly Process _process = new();
    private readonly string _name = typeof(TNodeService).Name;

    [LoggerMessage(LogLevel.Information, "{message}")]
    private partial void LogInformation(string message);

    [LoggerMessage(LogLevel.Warning, "{message}")]
    private partial void LogWarning(string message);

    [LoggerMessage(LogLevel.Error, "{message}")]
    private partial void LogError(string message);

    private readonly Channel<NodeTaskRequest> _channel = nodeService.Channel;

    private readonly TNodeService _nodeService = serviceProvider.GetRequiredService<TNodeService>();
    private readonly TNodeServiceOptions _options = optionsMonitor.CurrentValue;

    private readonly ConcurrentDictionary<Guid, NodeTaskRequest> _requests = new();

    public override void Dispose()
    {
        base.Dispose();
        _process.Kill(true);
        _process.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = (await _nodeService.InitializeAsync(stoppingToken).ConfigureAwait(false))?.ToString();
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        _process.StartInfo = new("node", $"{_options.Assembly.GetName().Name!}.js {config}")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardInputEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            CreateNoWindow = false,
            UseShellExecute = false
        };

        LogInformation($"start process {
            _name
        }: {
            workingDirectory
        } {
            _process.StartInfo.FileName
        } {
            _process.StartInfo.Arguments
        }");

        _process.Start();

        await Task.WhenAll(
            CheckAsync(stoppingToken),
            ErrorAsync(stoppingToken),
            ReadAsync(stoppingToken),
            WriteAsync(stoppingToken)
        );
    }

    private async Task CheckAsync(CancellationToken cancellationToken = default)
    {
        LogInformation($"start check {_name}");
        NodeTaskRequest nodeTaskRequest = new("check",
            JsonSerializer.SerializeToElement(true, NodeDefaults.JsonSerializerOptions));
        await _channel.Writer.WriteAsync(nodeTaskRequest, cancellationToken);
        var nodeTaskResponse = await nodeTaskRequest.TaskCompletionSource.Task;
        LogInformation($"end check {_name}: {nodeTaskResponse}");
    }

    private async Task WriteAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var nodeTaskRequest in _channel.Reader.ReadAllAsync(cancellationToken)
                           .ConfigureAwait(false))
        {
            _requests[nodeTaskRequest.Id] = nodeTaskRequest;

            var write = JsonSerializer.Serialize(nodeTaskRequest, NodeDefaults.JsonSerializerOptions);
            await _process.StandardInput.WriteLineAsync($"request>{write}");
            LogInformation($"process {_name} request: {write}");
        }
    }

    private async Task ReadAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var read = await _process.StandardOutput.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(read))
            {
                continue;
            }

            const string logPrefix = "log>";
            if (read.StartsWith(logPrefix))
            {
                LogWarning($"process {_name} log: {read}");
                continue;
            }

            const string responsePrefix = "response>";
            if (read.StartsWith(responsePrefix))
            {
                LogInformation($"process {_name} response: {read}");

                var nodeTaskResponse = JsonSerializer.Deserialize<NodeTaskResponse>(
                    read[responsePrefix.Length..],
                    NodeDefaults.JsonSerializerOptions
                )!;

                if (_requests.TryRemove(nodeTaskResponse.Id, out var nodeTask))
                {
                    if (nodeTaskResponse.Success)
                    {
                        nodeTask.TaskCompletionSource.SetResult(
                            nodeTaskResponse
                        );
                    }
                    else
                    {
                        nodeTask.TaskCompletionSource.SetException(
                            new Exception(nodeTaskResponse.Error?.ToString())
                        );
                    }
                }
            }
        }
    }

    private async Task ErrorAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var read = await _process.StandardError.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(read))
            {
                continue;
            }

            LogError(read);
        }
    }
}