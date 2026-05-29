using System.Text.RegularExpressions;
using MiApp.Domain.Exceptions;

namespace MiApp.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    private static readonly Regex _regex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainRuleException("Email cannot be empty.");

        if (!_regex.IsMatch(value))
            throw new DomainRuleException($"'{value}' is not a valid email address.");

        return new Email(value.ToLowerInvariant());
    }

    public bool Equals(Email? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Email e && Equals(e);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
