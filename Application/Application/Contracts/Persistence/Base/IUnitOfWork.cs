namespace Application
{
    using Domain;
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryBase<TEntity> Repository<TEntity>() where TEntity : BaseDomainModel;
        Task<int> Complete(bool executeDomainEvent = true);
        void ClearTrackedChanges();
    }
}
