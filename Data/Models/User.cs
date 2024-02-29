using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiJwtAuth.Data.Models;

[Table(nameof(User))]
public sealed class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [StringLength(25)] 
    public required string Username { get; set; }

    [StringLength(100)] 
    public required string PasswordHash { get; set; }

    [StringLength(60)] 
    public required string PasswordSalt { get; set; }

    [StringLength(100)] 
    public required string Email { get; set; }

    private bool Equals(User other)
    {
        return Id.Equals(other.Id) &&
               Username == other.Username &&
               PasswordHash == other.PasswordHash &&
               PasswordSalt == other.PasswordSalt &&
               Email == other.Email;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        
        return Equals((User)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Username, PasswordHash, PasswordSalt, Email);
    }
}