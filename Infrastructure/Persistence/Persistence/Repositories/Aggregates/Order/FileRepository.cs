namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class FileRepository : RepositoryBase<File>, IFileRepository
{
    public FileRepository(ApplicationDbContext context) : base(context) { }

}
