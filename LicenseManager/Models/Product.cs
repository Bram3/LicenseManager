using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Models;

[Index(nameof(Name), IsUnique = true)]
public class Product
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    public string? OwnerId { get; set; }
}