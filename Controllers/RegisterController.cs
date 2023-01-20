using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Cryptography;
using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;

using Microsoft.AspNetCore.Authorization;

namespace Casestudy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        AppDbContext _db;
        public RegisterController(AppDbContext context)
        {
            _db = context;
        }//end constructor

        [AllowAnonymous]
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<CustomerHelper>> Index(CustomerHelper helper)
        {
            CustomerDAO dao = new CustomerDAO(_db);
            Customer already = await dao.GetByEmail(helper.email);
            if (already == null)
            {
                HashSalt hashSalt = GenerateSaltedHash(64, helper.password);
                helper.password = ""; // don’t need the string anymore
                Customer dbCustomer = new Customer();
                dbCustomer.Firstname = helper.firstname;
                dbCustomer.Lastname = helper.lastname;
                dbCustomer.Email = helper.email;
                dbCustomer.Hash = hashSalt.Hash;
                dbCustomer.Salt = hashSalt.Salt;
                dbCustomer = await dao.Register(dbCustomer);
                if (dbCustomer.Id > 0)
                    helper.token = "customer registered";
                else
                    helper.token = "customer registration failed";
            }
            else
            {
                helper.token = "customer registration failed - email already in use";
            }
            return helper;
        }//end index method

        private static HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            var provider = new RNGCryptoServiceProvider();
            // Fills an array of bytes with a cryptographically strong sequence of random nonzero values.
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            // a password, salt, and iteration count, then generates a binary key
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            HashSalt hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }//end generatesaltedHash method

        public class HashSalt
        {
            public string Hash { get; set; }
            public string Salt { get; set; }
        }

    }//end class



}//end namespace
