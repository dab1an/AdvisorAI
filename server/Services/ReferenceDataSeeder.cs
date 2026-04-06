namespace server.Services;
public class ReferenceDataSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReferenceDataSeeder> _logger;
    private const string ReferenceDataFolder = "ReferenceData";

    public ReferenceDataSeeder(IServiceProvider serviceProvider, ILogger<ReferenceDataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string folderPath = Path.Combine(AppContext.BaseDirectory, ReferenceDataFolder);
        if (!Directory.Exists(folderPath))
        {
            _logger.LogWarning(
                "Reference data folder not found at {FolderPath}. Skipping embedding.",
                folderPath);
            return;
        }

        string[] markdownFiles = Directory.GetFiles(folderPath, "*.md");
        if (markdownFiles.Length == 0)
        {
            _logger.LogInformation("No markdown files found in {FolderPath}.", folderPath);
            return;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();
        IEmbeddingService embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();
        int embeddedCount = 0;
        int skippedCount = 0;

        foreach (string filePath in markdownFiles)
        {
            string fileName = Path.GetFileName(filePath);
            try
            {
                bool alreadyExists = await embeddingService.ReferenceDocumentExistsAsync(fileName);
                if (alreadyExists)
                {
                    _logger.LogInformation("Already embedded, skipping: {FileName}", fileName);
                    skippedCount++;
                    continue;
                }
                string content = await File.ReadAllTextAsync(filePath, cancellationToken);
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning("Skipping empty file: {FileName}", fileName);
                    continue;
                }
                await embeddingService.UpsertReferenceDocumentAsync(fileName, content);
                _logger.LogInformation("Embedded reference document: {FileName}", fileName);
                embeddedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process reference document: {FileName}", fileName);
            }
        }
        _logger.LogInformation(
            "Reference data seeding complete. Embedded: {Embedded}, Skipped: {Skipped}",
            embeddedCount, skippedCount);
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}