using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;

using Microsoft.AspNetCore.Authorization;

namespace Casestudy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        AppDbContext _db;
        public BrandController(AppDbContext context)
        {
            _db = context;
        }
        public async Task<ActionResult<List<Brand>>> Index()
        {
            BrandDAO dao = new BrandDAO(_db);
            List<Brand> allBrands = await dao.GetAll();
            return allBrands;
        }
    }
}
