namespace Application;

public interface IApiSeguridadService
{
    Task<string> ObtenerTokenSistema();
    Task<string> GetValueByKey(string key, bool almacenarCache = false);
    Task<bool> TienePermisoARecurso(IEnumerable<string> roles, string accion, string nombre);
}