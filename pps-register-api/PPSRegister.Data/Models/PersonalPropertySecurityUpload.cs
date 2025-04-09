using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PPSRegister.Data.Models;

[Index(nameof(FileName), nameof(ClientId), IsUnique = true, Name = "IX_PersonalPropertySecurityUpload_FileName_ClientId")]
public class PersonalPropertySecurityUpload
{
    [Key]
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int? Submitted { get; set; }
    public int? Invalid { get; set; }
    public int? Processed { get; set; }
    public int? Updated { get; set; }
    public int? Added { get; set; }

    [Required]
    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}