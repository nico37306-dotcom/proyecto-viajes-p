using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Agencia.Web.Services;

public sealed class ReglaException(string message) : Exception(message);

public static partial class Reglas
{
    public static DateOnly Hoy => DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4)).DateTime);
    public static bool English => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";
    public static string T(string es, string en) => English ? en : es;
    public static string Nombre(Enum valor) => TextoEnum().Replace(valor.ToString(), " ");
    public static decimal Redondear(decimal monto) => Math.Round(monto, 2, MidpointRounding.AwayFromZero);

    public static void Validar(object modelo)
    {
        var errores = new List<ValidationResult>();
        Validator.TryValidateObject(modelo, new ValidationContext(modelo), errores, true);
        if (errores.Count > 0) throw new ReglaException(string.Join(" ", errores.Select(e => e.ErrorMessage)));
        foreach (var prop in modelo.GetType().GetProperties())
        {
            if (prop.GetValue(modelo) is Enum valor && !Enum.IsDefined(valor.GetType(), valor))
                throw new ReglaException("La opción seleccionada no es válida.");
        }
    }

    [GeneratedRegex("(?<=[a-z])(?=[A-Z])")]
    private static partial Regex TextoEnum();
}
