using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiJwtAuth.Dao.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Username {get; set;}
    public required string PasswordHash { get; set; } 
    public required string Email { get; set; }
}