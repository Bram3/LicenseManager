using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseManager.Models;

public class License
{
    public Guid Id { get; set; }

    public string? Key { get; set; }

    public Guid? ProductId { get; set; }

    [DataType(DataType.Date)]
    [Column(TypeName = "Date")]
    public DateTime? Expires { get; set; }
}