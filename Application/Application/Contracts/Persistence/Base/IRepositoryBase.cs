namespace Application
{
    using Domain;

    public interface IRepositoryBase<T> where T : BaseDomainModel
    {
        void AddEntity(T entity);
        void UpdateEntity(T entity);
    }
}
