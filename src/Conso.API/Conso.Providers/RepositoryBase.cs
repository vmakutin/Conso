using AutoMapper;
using Conso.Providers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Conso.Providers
{
    public class RepositoryBase<TEntity, TDbContext> : IRepositoryBase<TEntity> 
        where TEntity : class
        where TDbContext : DbContext
    {
        protected TDbContext Context { get; }
        protected IMapper Mapper { get; }

        protected RepositoryBase(TDbContext context,
            IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public void Add(TEntity objModel)
        {
            Context.Set<TEntity>().Add(objModel);
            Context.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> objModel)
        {
            Context.Set<TEntity>().AddRange(objModel);
            Context.SaveChanges();
        }

        public TEntity? GetId(Guid id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity?> GetIdAsync(Guid id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where<TEntity>(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() =>
                Context.Set<TEntity>().Where<TEntity>(predicate));
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Task.Run(() => Context.Set<TEntity>());
        }

        public int Count()
        {
            return Context.Set<TEntity>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await Context.Set<TEntity>().CountAsync();
        }

        public void Update(TEntity objModel)
        {
            Context.Entry(objModel).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void Remove(TEntity objModel)
        {
            Context.Set<TEntity>().Remove(objModel);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
