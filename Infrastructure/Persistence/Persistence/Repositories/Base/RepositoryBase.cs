namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseDomainModel
{
    protected readonly ApplicationDbContext context;

    public RepositoryBase(ApplicationDbContext context)
    {
        this.context = context;
    }
    public void AddEntity(T entity)
    {
        context.Set<T>().Add(entity);
    }

    public void UpdateEntity(T entity)
    {
        context.Set<T>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
    }
}