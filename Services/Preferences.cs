using Agencia.Web.Domain;

namespace Agencia.Web.Services;

public sealed class Preferences(IHttpContextAccessor accessor)
{
    public const string CurrencyCookie = "agencia.moneda";
    public Moneda Currency { get; } = accessor.HttpContext?.Request.Cookies[CurrencyCookie] == "USD" ? Moneda.USD : Moneda.BOB;
    public static string LocalPath(string? path) =>
        path is { Length: > 0 } && path[0] == '/' &&
        (path.Length == 1 || path[1] is not ('/' or '\\')) && !path.Any(char.IsControl)
            ? path : "/";
    public static string Money(decimal value, Moneda moneda) => $"{(moneda == Moneda.BOB ? "Bs" : "USD")} {value:N2}";
}
