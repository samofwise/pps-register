using System.ComponentModel.DataAnnotations;

namespace PPSRegister.Data.Models;

public class Client
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
