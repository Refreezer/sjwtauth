namespace SiJwtAuth.Application.Utils;

public interface ITimespanParser
{
    TimeSpan ParseExact(string text);
}