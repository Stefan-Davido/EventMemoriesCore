namespace EventMemoriesServices.DTOs
{
    public class ConfigurationDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int NumberValue { get; set; }
    }

    public class CreateConfigurationDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int NumberValue { get; set; }
    }

    public class UpdateConfigurationDto
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public int? NumberValue { get; set; }
    }
}
