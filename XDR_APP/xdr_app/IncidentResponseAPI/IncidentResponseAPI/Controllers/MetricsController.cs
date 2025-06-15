using IncidentResponseAPI.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using System.Collections.Generic;

namespace IncidentResponseAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly ILogger<MetricsController> _logger;
    private readonly SecurityMetricsService _metricsService;
    
    public MetricsController(ILogger<MetricsController> logger, SecurityMetricsService metricsService)
    {
        _logger = logger;
        _metricsService = metricsService;
    }

    [HttpGet]
    public ActionResult GetMetrics()
    {
        try
        {
            _logger.LogInformation("Fetching metrics");

            var metrics = new Dictionary<string, object>();
            using var stream = new MemoryStream();
            
            Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream).GetAwaiter().GetResult();
            stream.Position = 0;
            
            using var reader = new StreamReader(stream);
            var metricsText = reader.ReadToEnd();

            var parsedMetrics = ParseMetricsToText(metricsText);
            _logger.LogInformation("Parsing metrics");
    
            return Ok(parsedMetrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while fetching metrics ");
            return StatusCode(500, "Internal Server Error while fetching metrics");
        }
    }

    

    private Dictionary<string, Dictionary<string, double>> ParseMetricsToText(string metricsText)
    {
        var result = new Dictionary<string, Dictionary<string, double>>();

        using var reader = new StringReader(metricsText);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split(' ');
            if (parts.Length < 2)
                continue;

            var metricName = parts[0].Split('{')[0];
            if (double.TryParse(parts[^1], out double value))
            {
                var labels = "default";
                var labelStart = line.IndexOf('{');
                var labelEnd = line.IndexOf('}');
                if (labelStart > 0 && labelEnd > labelStart)
                {
                    labels = line.Substring(labelStart + 1, labelEnd - labelStart - 1);
                }

                if (!result.ContainsKey(metricName))
                {
                    result[metricName] = new Dictionary<string, double>();
                }

                result[metricName][labels] = value;
            }
        }

        return result;
    }
}
