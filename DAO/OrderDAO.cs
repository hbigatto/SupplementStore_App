using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Casestudy.APIHelpers;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.EntityFrameworkCore;


namespace Casestudy.DAL.DAO
{
    public class OrderDAO
    {
        private AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<int> AddOrder(int userid, OrderSelectionHelper[] selections)
        {
            int orderId = -1;

            using (_db)
            {
                // we need a transaction as multiple entities involved
                using (var _trans = await _db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        Order order = new Order();
                        order.UserId = userid;
                        order.OrderDate = System.DateTime.Now;
                        order.OrderAmount = 0.0M;


                        // calculate the totals and then add the order table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            order.OrderAmount += (decimal)(selection.item.CostPrice * selection.Qty);

                        }//end for each loop Orders table
                        await _db.Orders.AddAsync(order);
                        await _db.SaveChangesAsync();



                        // then add each item to the trayitems table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            OrderLineItem oItem = new OrderLineItem();
                            oItem.QtyOrdered = selection.Qty;
                            oItem.ProductId = selection.item.Id;
                            oItem.OrderId = order.Id;
                            oItem.SellingPrice += (selection.Qty * (decimal)selection.item.CostPrice);

                            if (oItem.QtyOrdered < selection.item.QtyOnHand)
                            {
                                selection.item.QtyOnHand -= oItem.QtySold;
                                oItem.QtySold = selection.Qty;
                                oItem.QtyOrdered = selection.Qty;
                                oItem.QtyBackOrdered = 0;
                            }

                            if (oItem.QtyOrdered == selection.item.QtyOnHand)
                            {
                                oItem.QtySold = selection.Qty;
                            }

                            if (oItem.QtyOrdered > selection.item.QtyOnHand)
                            {
                                //selection.item.QtyOnHand = 0;
                                oItem.QtyBackOrdered += (selection.Qty - selection.item.QtyOnHand);
                                oItem.QtySold += selection.item.QtyOnHand;
                                oItem.QtyOrdered = selection.Qty;
                                oItem.QtyBackOrdered = (selection.Qty - selection.item.QtyOnHand);


                            }


                            await _db.OrderLineItems.AddAsync(oItem);
                            await _db.SaveChangesAsync();


                        }//end for each loop OrderLineItems Table           

                        foreach (OrderSelectionHelper selection in selections)
                        {

                            OrderLineItem oItem = new OrderLineItem();
                            oItem.QtyOrdered = selection.Qty;
                            oItem.ProductId = selection.item.Id;
                            oItem.OrderId = order.Id;
                            oItem.SellingPrice += (selection.Qty * (decimal)selection.item.CostPrice);


                            if (oItem.QtyOrdered < selection.item.QtyOnHand)
                            {
                                selection.item.QtyOnHand -= selection.Qty;
                            }

                            else if (oItem.QtyOrdered == selection.item.QtyOnHand)
                            {
                                selection.item.QtyOnHand -= selection.Qty;
                            }

                            else if (oItem.QtyOrdered >= selection.item.QtyOnHand)
                            {
                                selection.item.QtyBackOrder += (selection.Qty - selection.item.QtyOnHand);
                                selection.item.QtyOnHand = 0;
                            }




                            _db.Products.Update(selection.item);
                            await _db.SaveChangesAsync();
                        }// end foreach loop Products Table



                        // test trans by uncommenting out these 3 lines
                        //int x = 1;
                        //int y = 0;
                        //x = x / y;
                        await _trans.CommitAsync();
                        orderId = order.Id;

                    }// end try block

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await _trans.RollbackAsync();
                    }
                }
            }
            return orderId;
        }//end AddTray

        public List<Product> GetBackOrder(OrderSelectionHelper[] selec)
        {

            //bool isBack = false;
            List<Product> products = new List<Product>();
            OrderLineItem oItemBool = new OrderLineItem();

            foreach (OrderSelectionHelper selection in selec)
            {


                oItemBool.QtyOrdered = selection.Qty;
                oItemBool.ProductId = selection.item.Id;


                if ((oItemBool.QtyOrdered > selection.item.QtyOnHand) && (selection.item.QtyOnHand != 0))
                {
                    //isBack = true;
                    products.Add(selection.item);
                }
                else if (oItemBool.QtyOrdered > selection.item.QtyOnHand && (selection.item.QtyOnHand == 0))
                {
                    products.Add(selection.item);
                }

            }

            return products;

        }//end getbackorder
    }
}


