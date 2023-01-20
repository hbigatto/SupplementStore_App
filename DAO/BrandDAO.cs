using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;


namespace Casestudy.DAL.DAO
{
    public class BrandDAO
    {
        private AppDbContext _db;
        public BrandDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<Brand>> GetAll()
        {
            return await _db.Brands.ToListAsync<Brand>();
        }
    }
}
