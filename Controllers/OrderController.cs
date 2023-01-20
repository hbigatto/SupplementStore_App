using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.APIHelpers;
using Microsoft.AspNetCore.Authorization;
using Casestudy.Helpers;


namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        AppDbContext _ctx;
        public OrderController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            string retVal = "";
            List<Product> myList = new List<Product>();
            try
            {

                CustomerDAO cDao = new CustomerDAO(_ctx);
                Customer orderOwner = await cDao.GetByEmail(helper.email);
                OrderDAO oDao = new OrderDAO(_ctx);
                int orderId = await oDao.AddOrder(orderOwner.Id, helper.selections);

                myList = oDao.GetBackOrder(helper.selections);

                if (orderId > 0)
                {
                    retVal = "Order " + orderId + " saved! ";
                    foreach (Product p in myList)
                    {
                        retVal += " " + p.ProductName + " was back ordered.";
                    }
                }
                else
                {
                    retVal = "Order not saved";
                }

            }
            catch (Exception ex)
            {
                retVal = "Order not saved " + ex.Message;
            }
            return retVal;
        }//end index method
    }
}



//-------------------------------------------------------
