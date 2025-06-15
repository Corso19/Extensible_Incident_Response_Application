using System.Collections.Concurrent;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Implementations;
using IncidentResponseAPI.Services.Interfaces;
using Prometheus;

namespace IncidentResponseAPI.Orchestrators;

public class SensorsOrchestrator : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SensorsOrchestrator> _logger;
    private readonly ConcurrentQueue<SensorsModel> _sensorsQueue;
    private readonly SemaphoreSlim _semaphore;
    private const int MaxConcurrentSensors = 5;
    private const int DelayBetweenSensorRuns = 20;
    private CancellationTokenSource _orchestratorCts;
    private readonly SecurityMetricsService _metricsService;

    public bool IsRunning { get; set; }

    public SensorsOrchestrator(IServiceScopeFactory scopeFactory, ILogger<SensorsOrchestrator> logger
        , SecurityMetricsService metricsService)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _sensorsQueue = new ConcurrentQueue<SensorsModel>();
        _semaphore = new SemaphoreSlim(MaxConcurrentSensors);
        _orchestratorCts = new CancellationTokenSource();
        IsRunning = false;
        _metricsService = metricsService;
    }

    public void EnqueueSensor(SensorsModel sensor)
    {
        _sensorsQueue.Enqueue(sensor);
        ProcessQueue();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SensorsOrchestrator initializing metrics at startup...");

        // Reset all sensor metrics to ensure we start from a clean state
        var sensorTypes = new[] { "MicrosoftEmail", "MicrosoftTeams", "MicrosoftSharePoint" };
        foreach (var sensorType in sensorTypes)
        {
            _metricsService.ActiveSensors.WithLabels(sensorType).Set(0);
        }

        // Initialize metrics with currently enabled sensors
        using (var scope = _scopeFactory.CreateScope())
        {
            var sensorsRepository = scope.ServiceProvider.GetRequiredService<ISensorsRepository>();
            var enabledSensors = await sensorsRepository.GetAllEnabledAsync();

            foreach (var sensor in enabledSensors)
            {
                _metricsService.ActiveSensors.WithLabels(sensor.Type).Inc();
                _logger.LogInformation("Initialized metric for active {SensorType} sensor", sensor.Type);
            }
        }

        // Call the base implementation to start the background service
        await base.StartAsync(cancellationToken);
    }

    private async void ProcessQueue()
    {
        IsRunning = true;
        while (_sensorsQueue.Count > 0)
        {
            await _semaphore.WaitAsync();
            if (_sensorsQueue.TryDequeue(out var sensor))
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var sensorsService = scope.ServiceProvider.GetRequiredService<ISensorsService>();
                            while (!_orchestratorCts.Token.IsCancellationRequested)
                            {
                                var freshSensorDto = await sensorsService.GetByIdAsync(sensor.SensorId);
                                if (freshSensorDto == null)
                                {
                                    _logger.LogError("Sensor {SensorId} not found", sensor.SensorId);
                                    _metricsService.SensorErrors.WithLabels(sensor.Type, "not_found").Inc();
                                    break;
                                }

                                if (!freshSensorDto.isEnabled)
                                {
                                    _logger.LogInformation("Sensor {SensorId} is disabled", sensor.SensorId);
                                    break;
                                }

                                //update activity sensors gauge when starting a sensor
                                _metricsService.ActiveSensors.WithLabels(sensor.Type).Inc();

                                var freshSensor = new SensorsModel
                                {
                                    SensorId = freshSensorDto.SensorId,
                                    SensorName = freshSensorDto.SensorName,
                                    Type = freshSensorDto.Type,
                                    Configuration = freshSensorDto.Configuration,
                                    isEnabled = freshSensorDto.isEnabled,
                                    CreatedAt = freshSensorDto.CreatedAt,
                                    LastRunAt = freshSensorDto.LastRunAt,
                                    NextRunAfter = freshSensorDto.NextRunAfter,
                                    LastError = freshSensorDto.LastError,
                                    RetrievalInterval = freshSensorDto.RetrievalInterval,
                                    LastEventMarker = freshSensorDto.LastEventMarker
                                };

                                try
                                {
                                    using (var timer = _metricsService.EventProcessingTime.WithLabels(sensor.Type)
                                               .NewTimer())
                                    {
                                        await sensorsService.RunSensorAsync(freshSensor, _orchestratorCts.Token);
                                    }

                                    //increment successful sensor runs counter
                                    _metricsService.SensorRuns.WithLabels(sensor.Type, sensor.SensorName).Inc();

                                    _logger.LogInformation("Sensor {SensorId} run completed", sensor.SensorId);
                                }
                                catch (Exception ex)
                                {
                                    //track sensor errors
                                    _metricsService.SensorErrors.WithLabels(sensor.Type, "execution_error").Inc();
                                    _logger.LogError(ex, "Error running sensor {SensorId}", sensor.SensorId);
                                }
                                finally
                                {
                                    //decrement active sensors when done
                                    _metricsService.ActiveSensors.WithLabels(sensor.Type).Dec();
                                }

                                // Re-enqueue the sensor if still enabled
                                _sensorsQueue.Enqueue(sensor);

                                await Task.Delay(TimeSpan.FromSeconds(DelayBetweenSensorRuns), _orchestratorCts.Token);
                            }
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!IsRunning)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var sensorsService = scope.ServiceProvider.GetRequiredService<ISensorsService>();

                    var enabledSensors = await sensorsService.GetAllEnabledAsync();
                    foreach (var sensor in enabledSensors)
                    {
                        _sensorsQueue.Enqueue(sensor);
                    }

                    if (_sensorsQueue.Any())
                    {
                        IsRunning = true;
                        ProcessQueue();
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Log graceful shutdown
            _logger.LogInformation("Sensor orchestrator stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in orchestrator background service");
        }
    }
}