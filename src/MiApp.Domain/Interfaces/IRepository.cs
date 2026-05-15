namespace MiApp.Domain.Interfaces;

/// <summary>
/// Repositorio genérico para operaciones CRUD básicas
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todos los elementos sin rastreo
    /// </summary>
    Task<IList<T>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Obtiene un elemento por ID
    /// </summary>
    Task<T?> GetByIdAsync(object id, CancellationToken ct = default);

    /// <summary>
    /// Agrega un nuevo elemento
    /// </summary>
    Task AddAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Actualiza un elemento
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Elimina un elemento
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Guarda los cambios en la BD
    /// </summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
