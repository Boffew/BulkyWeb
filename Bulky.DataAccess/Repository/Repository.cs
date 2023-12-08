using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDBContext _db;
		internal DbSet<T> dbset;
		public Repository(ApplicationDBContext db)
		{
			_db = db;
			this.dbset = _db.Set<T>();
			//so now instead of _db.catecories.toList() = dbset.toList()
		}
		public void Add(T entity)
		{
			dbset.Add(entity);
		}

		public T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
		{
			IQueryable<T> query = dbset;
			query = query.Where(filter);
			//Add eager loading logic, using include
			//Can write include statement like this context.People.Include("School").ToList() so that why the below code works
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var include in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(include);
				}
			}
			return query.FirstOrDefault();
		}

		public IEnumerable<T> GetAll(string? includeProperties = null)
		{

			IQueryable<T> query = dbset;
			//Add eager loading logic, using include
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var include in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(include);
				}
			}
			return query.ToList();
		}

		public void Remove(T entity)
		{
			dbset?.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			dbset?.RemoveRange(entities);
		}
	}
}
