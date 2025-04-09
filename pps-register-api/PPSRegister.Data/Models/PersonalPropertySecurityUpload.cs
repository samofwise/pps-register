using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPSRegister.Data.Models;
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

    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}