using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppinG_Cart.DB;
using ShoppinG_Cart.Models;

namespace ShoppinG_Cart.Controllers
{
    public class ViewCartController : Controller
    {
        protected Shopping_Cart_DBContext dbcontext;
        protected DBTester dBTester;
        public ViewCartController(Shopping_Cart_DBContext dbcontext)
        {
            this.dbcontext = dbcontext;
            dBTester = new DBTester(dbcontext);
        }
        public IActionResult Index()
        {
            ViewData["username"] = HttpContext.Session.GetString("username");
            ViewData["count"] = HttpContext.Session.GetInt32("count");
            List<UserCart> selectedproducts = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
            ViewData["selectedproducts"] = selectedproducts;
            //List<Cart_Model> items = dBTester.CartItems();
            //ViewData["cart"] = items;
            //ViewBag.cart = iter;
            if (selectedproducts != null)
            {
                ViewData["selectedproducts"] = selectedproducts;
                int count = Convert.ToInt32(HttpContext.Session.GetInt32("count"));
                if (selectedproducts.Count > 0)
                {
                    return View();
                }
                else
                {
                    if (HttpContext.Session.GetString("UserId") == null)
                    {
                        return RedirectToAction("Index", "List");
                    }
                    else
                    {
                        return RedirectToAction("LoggedIn", "List");
                    }
                }
            }
            else
            {
                if(HttpContext.Session.GetString("UserId") == null)
                {
                    return RedirectToAction("Index", "List");
                } else
                {
                    return RedirectToAction("LoggedIn", "List");
                }
            }
        }
        public IActionResult Quantity([FromBody] Object obj)
        {
           // int count = Convert.ToInt32(HttpContext.Session.GetInt32("count"));
            UserCart cart_model = System.Text.Json.JsonSerializer.Deserialize<UserCart>(obj.ToString());
            //dBTester.updateCart(cart_model);
            List<UserCart> selectedproducts = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
            UserCart prod = selectedproducts.Single(model => model.ProductId == cart_model.ProductId);
            if (prod != null)
            {
                prod.ProductQuantity = cart_model.ProductQuantity;
                prod.subTotal = cart_model.subTotal;
                HttpContext.Session.SetString("selectedproducts", JsonConvert.SerializeObject(selectedproducts));

                return Json(new
                {
                    success = true,
                });
            } else
            {
                return Json(new
                {
                    success = false,
                });
            }
        }
        public JsonResult Delete(string id, int qty)
        {
            //string result = dBTester.DeleteItems(id);
            int count = Convert.ToInt32(HttpContext.Session.GetInt32("count"));
            count = count - qty;
            if (count < 0)
                count = 0;
            HttpContext.Session.SetInt32("count", count);
            ViewData["count"] = count;
            List<UserCart> selectedproducts = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
            UserCart prod = selectedproducts.Single(model => model.ProductId == id);
            if (prod != null)
            {
                selectedproducts.Remove(prod);
                HttpContext.Session.SetString("selectedproducts", JsonConvert.SerializeObject(selectedproducts));
                return Json(new
                {
                    success = true,
                    //newUrl = Url.Action("Index", "ViewCart")
                });
            }
            else
            {
                return Json(new
                {
                    success = false
                });
            }

            //return RedirectToAction("Index");
            //if (result == "success")
            //    return Json(new
            //    {
            //        success = true,
            //        //newUrl = Url.Action("Index", "ViewCart")
            //    });

        }

        public IActionResult AddOrder(string TotalAmt)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return Json(new
                {
                    success = false
                });
            }
            else
            {
                string UserId = HttpContext.Session.GetString("UserId");
                List<UserCart> selectedproducts = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
                string result = dBTester.AddtoOrder(TotalAmt, UserId, selectedproducts);
                if (result == "success")
                {
                    HttpContext.Session.SetString("selectedproducts", "");
                    HttpContext.Session.SetInt32("count", 0);
                    return Json(new
                    {
                        success = true
                    });
                }
                   
                else
                    return Json(new
                    {
                        success = false
                    });
            }
            
        }


    }
}