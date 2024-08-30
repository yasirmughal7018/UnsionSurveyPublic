using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;

namespace UnionSurvey.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly SurveyUnionContext _context;
        public readonly DbSet<T> _dbSet;

        public Repository(SurveyUnionContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.ToList();
        public IQueryable<T> GetAllAsync() => _dbSet.AsQueryable<T>();
        public T GetById(int id) => _dbSet.Find(id)!;
        public async Task<T> GetByIdAsync(int id) => (await _dbSet.FindAsync(id))!;
        public async Task<T> GetByIdAsync(string id) => (await _dbSet.FindAsync(id))!;
        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }
        public async Task AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
