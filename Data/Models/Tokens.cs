using System.ComponentModel.DataAnnotations;

namespace SiJwtAuth.Data.Models;

public sealed class Tokens
{
    [Key] 
    public Guid Id { get; set; }

    [StringLength(25)] 
    public string? Username { get; set; }

    [StringLength(100)] 
    public string? Email { get; set; }

    [StringLength(500)] 
    public string? AccessToken { get; set; }

    [StringLength(64)] 
    public string? RefreshToken { get; set; }

    private bool Equals(Tokens other)
    {
        return Id.Equals(other.Id) && Username == other.Username && Email == other.Email &&
               AccessToken == other.AccessToken && RefreshToken == other.RefreshToken;
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
        
        if (obj.GetType() != GetType())
        {
            return false;
        }
        
        return Equals((Tokens)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Username, Email, AccessToken, RefreshToken);
    }
}