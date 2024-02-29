using Microsoft.IdentityModel.Protocols.Configuration;

namespace SiJwtAuth.Application.Utils.Impl;

public class TimespanParser : ITimespanParser
{
    public TimeSpan ParseExact(string text)
    {
        var lex = text[^1];
        if (!double.TryParse(text[..^1], out var value))
            throw new InvalidConfigurationException("Bad lifetime config text");

        return lex switch
        {
            'd' => TimeSpan.FromDays(value),
            'h' => TimeSpan.FromHours(value),
            'm' => TimeSpan.FromMinutes(value),
            'f' => TimeSpan.FromMilliseconds(value),
            'z' => TimeSpan.FromTicks((long)value),
            's' => TimeSpan.FromSeconds(value),
            _ => throw new ArgumentOutOfRangeException(nameof(text))
        };
    }
}