using Agencia.Web.Domain;

namespace Agencia.Web.Services;

public sealed record ReceiptUpload(string Nombre, byte[] Datos)
{
    public const int Limite = 2 * 1024 * 1024;

    public ComprobantePago Validar()
    {
        if (Datos.Length is 0 or > Limite) throw new ReglaException("El comprobante debe tener como máximo 2 MB.");
        var archivo = Path.GetFileName(Nombre.Replace('\\', '/'));
        var ext = Path.GetExtension(archivo).ToLowerInvariant();
        ReadOnlySpan<byte> bytes = Datos;
        var mime = ext switch
        {
            ".pdf" when bytes.StartsWith("%PDF-"u8) => "application/pdf",
            ".jpg" or ".jpeg" when bytes.StartsWith(new byte[] { 255, 216, 255 }) => "image/jpeg",
            ".png" when bytes.StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) => "image/png",
            _ => throw new ReglaException("El comprobante debe ser PDF, JPG o PNG.")
        };
        if (archivo.Length > 180 || archivo.Any(char.IsControl)) throw new ReglaException("El nombre del archivo no es válido.");
        return new() { Nombre = archivo, Tipo = mime, Datos = Datos };
    }
}
