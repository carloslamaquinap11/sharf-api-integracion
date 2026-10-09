namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ClientRepository : RepositoryBase<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context) { }
    public async Task<Client?> GetClientByCode(string code, bool asNoTracking = false)
    {
        var query = context.Client.Where(x => x.Code.ToLower() == code.ToLower() && x.IsActive);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}
