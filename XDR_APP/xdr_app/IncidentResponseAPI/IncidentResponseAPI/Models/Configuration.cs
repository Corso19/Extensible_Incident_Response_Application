using System.ComponentModel.DataAnnotations;

namespace IncidentResponseAPI.Models;

public class Configuration
{
    [Required]
    [RegularExpression(@"^[0-9a-fA-F-]{36}$", ErrorMessage = "Invalid TenantId format.")]
    public string TenantId { get; set; }

    [Required]
    [RegularExpression(@"^[0-9a-fA-F-]{36}$", ErrorMessage = "Invalid ApplicationId format.")]
    public string ApplicationId { get; set; }

    [Required(ErrorMessage = "Client secret is required.")]
    [StringLength(100, ErrorMessage = "Client secret must not exceed 100 characters.")]
    public string ClientSecret { get; set; }
}