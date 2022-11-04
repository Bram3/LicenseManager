using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseManager.Models;

public class LicenseNew
{
    [Required] public Guid? ProductId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Column(TypeName = "Date")]
    public DateTime? Expires { get; set; }
}