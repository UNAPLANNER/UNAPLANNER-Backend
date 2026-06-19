using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace UNAPLANNER_API.Infrastructure.Firebase;

public class FirebaseNotificationSender : INotificationSender
{
    private readonly FirebaseMessaging _firebaseMessaging;
    private readonly ILogger<FirebaseNotificationSender> _logger;

    public FirebaseNotificationSender(
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger<FirebaseNotificationSender> logger)
    {
        _logger = logger;
        TryInitializeFirebase(configuration, environment);
        _firebaseMessaging = FirebaseMessaging.DefaultInstance;
    }
// This method sends a notification to a single device token and returns true if successful.
    public async Task<bool> SendAsync(string token, NotificationPayload payload)
    {
        try
        {
            var messageId = await _firebaseMessaging.SendAsync(BuildMessage(token, payload));
            _logger.LogInformation("Notificación enviada con ID {MessageId}", messageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación");
            return false;
        }
    }
// This method sends a notification to multiple device tokens and returns the count of successful deliveries.
    public async Task<int> SendToMultipleAsync(IEnumerable<string> tokens, NotificationPayload payload)
    {
        var tokensList = tokens.ToList();
        if (tokensList.Count == 0)
        {
            _logger.LogWarning("No se proporcionaron tokens para enviar notificaciones");
            return 0;
        }

        var results = await Task.WhenAll(tokensList.Select(token => SendAsync(token, payload)));
        var successCount = results.Count(success => success);
        _logger.LogInformation(
            "Notificaciones enviadas a {SuccessCount}/{TotalCount} dispositivos",
            successCount,
            tokensList.Count);

        return successCount;
    }

    private static Message BuildMessage(string token, NotificationPayload payload) =>
        new()
        {
            Token = token,
            Notification = new Notification
            {
                Title = payload.Title,
                Body = payload.Body
            },
            Data = payload.Data
        };

// This method attempts to initialize the Firebase Admin SDK, logging any issues encountered during the process.
    private void TryInitializeFirebase(IConfiguration configuration, IHostEnvironment environment)
    {
        try
        {
            if (FirebaseApp.DefaultInstance is not null)
                return;

            var serviceAccountPath = ResolveServiceAccountPath(configuration, environment);
            if (string.IsNullOrWhiteSpace(serviceAccountPath))
            {
                _logger.LogWarning("Firebase:ServiceAccountPath no está configurado");
                return;
            }

            if (!File.Exists(serviceAccountPath))
            {
                _logger.LogWarning(
                    "Archivo de cuenta de servicio de Firebase no encontrado en {ServiceAccountPath}",
                    serviceAccountPath);
                return;
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(serviceAccountPath)
            });

            _logger.LogInformation(
                "Firebase Admin SDK inicializado correctamente usando {ServiceAccountPath}",
                serviceAccountPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar Firebase");
        }
    }
// This method resolves the service account path, allowing for both absolute and relative paths.
    private static string? ResolveServiceAccountPath(
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var configuredPath = configuration.GetValue<string>("Firebase:ServiceAccountPath");
        if (string.IsNullOrWhiteSpace(configuredPath))
            return null;

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(environment.ContentRootPath, configuredPath);
    }
}
