using System.Text.Json.Serialization;

namespace VacationPlanner.Models;

public class ImportPayload
{
    [JsonPropertyName("employees")]
    public List<ImportEmployee> Employees { get; set; } = [];
    [JsonPropertyName("customers")]
    public List<ImportCustomer> Customers { get; set; } = [];
    [JsonPropertyName("projects")]
    public List<ImportProject> Projects { get; set; } = [];
}

public class ImportEmployee
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("jobTitle")] public string JobTitle { get; set; } = "";
}

public class ImportCustomer
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
}

public class ImportProject
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("customerId")] public string CustomerId { get; set; } = "";
    [JsonPropertyName("period")] public ImportPeriod Period { get; set; } = new();
    [JsonPropertyName("assignedEmployeeIds")] public List<string> AssignedEmployeeIds { get; set; } = [];
}

public class ImportPeriod
{
    [JsonPropertyName("start")] public DateOnly Start { get; set; }
    [JsonPropertyName("end")]   public DateOnly End   { get; set; }
}