// // namespace IncidentResponseAPI.Services.Implementations;
// //
// // using Prometheus;
// //
// // public class SecurityMetricsService
// // {
// //     //sensor metrics
// //     public readonly Counter SensorRuns;
// //     public readonly Counter SensorErrors;
// //     public readonly Gauge ActiveSensors;
// //     
// //     //Incident metrics
// //     public readonly Counter IncidentsDetected;
// //     public readonly Histogram EventProcessingTime;
// //
// //     public SecurityMetricsService()
// //     {
// //         SensorRuns = Metrics.CreateCounter("ir_sensor_runs_total", "Number of sensor executions",
// //             new CounterConfiguration { LabelNames = new[] { "sensor_type", "sensor_name" } });
// //         
// //         SensorErrors = Metrics.CreateCounter("ir_sensor_errors_total", 
// //             "Number of sensor errors",
// //             new CounterConfiguration { LabelNames = new[] { "sensor_type", "sensor_name" } });
// //
// //         ActiveSensors = Metrics.CreateGauge("ir_active_sensors",
// //             "Number of currently active sensors",
// //             new GaugeConfiguration { LabelNames = new[] { "sensor_type" }});
// //         
// //         IncidentsDetected = Metrics.CreateCounter("ir_incidents_detected_total",
// //             "Number of security incidents detected",
// //             new CounterConfiguration { LabelNames = new[] { "severity", "type" }});
// //             
// //         EventProcessingTime = Metrics.CreateHistogram("ir_event_processing_seconds",
// //             "Time taken to process events",
// //             new HistogramConfiguration { LabelNames = new[] { "event_type" }});
// //     }
// // }
//
// using Prometheus;
//
// namespace IncidentResponseAPI.Services.Implementations
// {
//     public class SecurityMetricsService
//     {
//         // Sensor metrics
//         public readonly Counter SensorRuns;
//         public readonly Counter SensorErrors;
//         public readonly Gauge ActiveSensors;
//
//         // Event processing metrics
//         public readonly Counter EventsProcessed;
//         public readonly Counter EventProcessingErrors;
//         public readonly Histogram EventProcessingTime;
//
//         // Incident metrics
//         public readonly Counter IncidentsDetected;
//         public readonly Counter IncidentsByType;
//         public readonly Counter IncidentsBySeverity;
//
//         // Email metrics
//         public readonly Counter EmailsProcessed;
//         public readonly Counter EmailsWithAttachments;
//         public readonly Counter SuspiciousAttachmentsDetected;
//
//         public SecurityMetricsService()
//         {
//             // Initialize sensor metrics
//             SensorRuns = Metrics.CreateCounter(
//                 "ir_sensor_runs_total",
//                 "Number of sensor execution runs",
//                 new CounterConfiguration
//                 {
//                     LabelNames = new[] { "sensor_type", "sensor_name" }
//                 });
//
//             SensorErrors = Metrics.CreateCounter(
//                 "ir_sensor_errors_total",
//                 "Number of sensor execution errors",
//                 new CounterConfiguration
//                 {
//                     LabelNames = new[] { "sensor_type", "sensor_name" }
//                 });
//
//             ActiveSensors = Metrics.CreateGauge(
//                 "ir_active_sensors",
//                 "Number of currently active sensors",
//                 new GaugeConfiguration
//                 {
//                     LabelNames = new[] { "sensor_type" }
//                 });
//
//             // Initialize event processing metrics
//             EventsProcessed = Metrics.CreateCounter(
//                 "events_processed_total",
//                 "Total number of events processed");
//
//             EventProcessingErrors = Metrics.CreateCounter(
//                 "event_processing_errors_total",
//                 "Total number of event processing errors");
//
//             EventProcessingTime = Metrics.CreateHistogram(
//                 "ir_event_processing_seconds",
//                 "Time taken to process events",
//                 new HistogramConfiguration
//                 {
//                     LabelNames = new[] { "sensor_type" }
//                 });
//
//             // Initialize incident metrics
//             IncidentsDetected = Metrics.CreateCounter(
//                 "ir_incidents_detected_total",
//                 "Number of security incidents detected",
//                 new CounterConfiguration
//                 {
//                     LabelNames = new[] { "severity", "type" }
//                 });
//
//             IncidentsByType = Metrics.CreateCounter(
//                 "incidents_by_type_total",
//                 "Number of incidents by type",
//                 new CounterConfiguration
//                 {
//                     LabelNames = new[] { "incident_type" }
//                 });
//
//             IncidentsBySeverity = Metrics.CreateCounter(
//                 "incidents_by_severity_total",
//                 "Number of incidents by severity level",
//                 new CounterConfiguration
//                 {
//                     LabelNames = new[] { "severity" }
//                 });
//
//             // Initialize email-specific metrics
//             EmailsProcessed = Metrics.CreateCounter(
//                 "emails_processed_total",
//                 "Total number of emails processed");
//
//             EmailsWithAttachments = Metrics.CreateCounter(
//                 "emails_with_attachments_total",
//                 "Number of emails containing attachments");
//
//             SuspiciousAttachmentsDetected = Metrics.CreateCounter(
//                 "suspicious_attachments_total",
//                 "Number of suspicious attachments detected");
//         }
//     }
// }

using Prometheus;

public class SecurityMetricsService
{
    public Counter SensorRuns { get; private set; }
    public Counter SensorErrors { get; private set; }
    public Gauge ActiveSensors { get; private set; }
    public Counter EventsProcessed { get; private set; }
    public Counter EventProcessingErrors { get; private set; }
    public Histogram EventProcessingTime { get; private set; }
    public Counter IncidentsDetected { get; private set; }
    public Counter IncidentsByType { get; private set; }
    public Counter IncidentsBySeverity { get; private set; }

    public SecurityMetricsService()
    {
        // Sensor metrics
        SensorRuns = Metrics.CreateCounter(
            "ir_sensor_runs_total",
            "Number of sensor execution runs",
            new CounterConfiguration { 
                LabelNames = new[] { "sensor_type", "sensor_name" } 
            });

        SensorErrors = Metrics.CreateCounter(
            "ir_sensor_errors_total",
            "Number of sensor execution errors",
            new CounterConfiguration { 
                LabelNames = new[] { "sensor_type", "sensor_name" } 
            });

        ActiveSensors = Metrics.CreateGauge(
            "ir_active_sensors",
            "Number of currently active sensors",
            new GaugeConfiguration { 
                LabelNames = new[] { "sensor_type" } 
            });

        // Event processing metrics
        EventsProcessed = Metrics.CreateCounter(
            "ir_events_processed_total",
            "Total number of events processed",
            new CounterConfiguration { 
                LabelNames = new[] { "sensor_type" } 
            });

        EventProcessingErrors = Metrics.CreateCounter(
            "ir_event_processing_errors_total",
            "Number of event processing errors",
            new CounterConfiguration { 
                LabelNames = new[] { "sensor_type", "error_type" } 
            });

        EventProcessingTime = Metrics.CreateHistogram(
            "ir_event_processing_seconds",
            "Time taken to process events",
            new HistogramConfiguration {
                LabelNames = new[] { "sensor_type" },
                Buckets = new[] { .1, .5, 1, 2.5, 5, 10, 30 }
            });

        // Incident metrics
        IncidentsDetected = Metrics.CreateCounter(
            "ir_incidents_detected_total",
            "Number of security incidents detected",
            new CounterConfiguration {
                LabelNames = new[] { "severity", "type" }
            });

        IncidentsByType = Metrics.CreateCounter(
            "ir_incidents_by_type_total",
            "Total number of incidents grouped by type",
            new CounterConfiguration {
                LabelNames = new[] { "type", "sensor_type" }
            });

        IncidentsBySeverity = Metrics.CreateCounter(
            "ir_incidents_by_severity_total",
            "Total number of incidents grouped by severity level",
            new CounterConfiguration {
                LabelNames = new[] { "severity", "sensor_type" }
            });
    }
}