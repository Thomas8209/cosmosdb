namespace CosmosDb.Configuration;

public class CosmosDbOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public string ContainerName { get; set; } = string.Empty;
}
