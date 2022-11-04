using System.ComponentModel.DataAnnotations;

namespace LicenseManager.Models;

public class LicenseInfo
{
    [Required] public string? Key { get; set; }

    [Required] public Guid? ProductId { get; set; }
}