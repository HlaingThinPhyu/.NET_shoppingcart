using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using ShoppinG_Cart.DB;
using ShoppinG_Cart.Models;

namespace ShoppinG_Cart.Controllers
{

    public class ListController : Controller
    {
        protected Shopping_Cart_DBContext db;
        protected DBTester Tester;
        internal List<UserCart> selectedproducts = new List<UserCart>();
        public ListController(Shopping_Cart_DBContext db)
        {
            this.db = db;
            Tester = new DBTester(db);
        }

        public IActionResult StartSession()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                string sessionId = System.Guid.NewGuid().ToString();

                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddHours(1);

                Response.Cookies.Append("SessionId", sessionId, options);

                return RedirectToAction("Index");
            }

        }
        public IActionResult Index()
        {
            List<Product> products = Tester.Run();
            ViewData["products"] = products;
            HttpContext.Session.SetString("products", JsonConvert.SerializeObject(products));
            if (HttpContext.Session.GetString("selectedproducts") == null)
            {
                HttpContext.Session.SetString("selectedproducts", "");
                //List<UserCart> uc = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
                HttpContext.Session.SetInt32("count", 0);
                //int count = Convert.ToInt32(HttpContext.Session.GetInt32("count"));
                ViewData["count"] = 0;
            } else
            {
                List<UserCart> prods = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
                if (prods != null)
                {
                    ViewData["selectedproducts"] = prods;
                    int count = Convert.ToInt32(HttpContext.Session.GetInt32("count"));
                    ViewData["count"] = count;
                }
            }
           
           
            return View();
        }
        public IActionResult LoggedIn()
        {
           // string sessionId = Request.Cookies["SessionId"];
            ViewData["username"] = HttpContext.Session.GetString("username");
            List<Product> products = Tester.Run();
            ViewData["products"] = products;
            ViewData["count"] = HttpContext.Session.GetInt32("count");
            return View();
        }

        [HttpPost]
        public IActionResult Index(string SearchString)
        {
            if (SearchString != null)
            {
                List<Product> products = Tester.Find(SearchString);
                ViewData["products"] = products;
                return View();
            }
            else
            {
                List<Product> products = Tester.Run();
                ViewData["products"] = products;
                ViewData["count"] = HttpContext.Session.GetInt32("count");
                return View();
            }

        }
        public JsonResult AddToCart(string productId)
        {
            Product product = Tester.FindProduct(productId);
            List<UserCart> selprods = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
            if (selprods != null)
            {
                if (selprods.Any(cart => cart.ProductId == productId))
                {
                    //HttpContext.Session.SetString("selectedproducts", "");
                    UserCart prodobj = selprods.Single(cart => cart.ProductId == productId);
                    prodobj.ProductQuantity++;
                   // int count = Convert.ToInt32(ViewData["count"]);
                    //count++;
                    //ViewData["count"] = count;
                    prodobj.subTotal = prodobj.ProductQuantity * prodobj.ProductPrice;
                    HttpContext.Session.SetString("selectedproducts", JsonConvert.SerializeObject(selprods));
                } else
                {
                    UserCart prodobj = new UserCart();
                    prodobj.ProductId = product.ProductId;
                    prodobj.ProductName = product.ProductName;
                    prodobj.ProductQuantity = 1;
                    prodobj.ProductPrice = product.ProductPrice;
                    prodobj.ProductImage = product.ImageURL;
                    prodobj.subTotal = prodobj.ProductQuantity * prodobj.ProductPrice;
                    prodobj.ProductDescription = product.ProductDescription;
                    //if(HttpContext.Session.GetString("UserId") != null)
                    //{
                    //    prodobj.User.UserId = HttpContext.Session.GetString("UserId");
                    //}
                    if (JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts")) == null)
                    {
                        selectedproducts.Add(prodobj);
                        HttpContext.Session.SetString("selectedproducts", JsonConvert.SerializeObject(selectedproducts));
                    }
                    else
                    {
                        List<UserCart> selectedproducts = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
                        selectedproducts.Add(prodobj);
                        HttpContext.Session.SetString("selectedproducts", JsonConvert.SerializeObject(selectedproducts));
                    }
                }
                
            }
            else
            {
                    UserCart prodobj = new UserCart();
                    prodobj.ProductId = product.ProductId;
                    prodobj.ProductName = product.ProductName;
                    prodobj.ProductQuantity = 1;
                    prodobj.ProductPrice = product.ProductPrice;
                    prodobj.ProductImage = product.ImageURL;
                    prodobj.subTotal = prodobj.ProductQuantity*prodobj.ProductPrice;
                    //if(HttpContext.Session.GetString("UserId") != null)
                    //{
                    //    prodobj.User.UserId = HttpContext.Session.GetString("UserId");
                    //}
                    if (JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts")) == null)
                    {
                        selectedproducts.Add(prodobj);
                        HttpContext.Session.SetString("selectedproducts", JsonConvert.SerializeObject(selectedproducts));
                    }
                    else
                    {
                        List<UserCart> selectedproducts = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
                        selectedproducts.Add(prodobj);
                        HttpContext.Session.SetString("selectedproducts", JsonConvert.SerializeObject(selectedproducts));
                    }
                }
            //List<UserCart> selectedprods = JsonConvert.DeserializeObject<List<UserCart>>(HttpContext.Session.GetString("selectedproducts"));
            //ViewData["selectedproducts"] = selectedprods;
            //if (selectedprods.Count() != 0)
            //{
            //    ViewData["count"] = selectedprods.Count();
            //}
            int count = Convert.ToInt32(HttpContext.Session.GetInt32("count"));
            count++;
            HttpContext.Session.SetInt32("count", count);

            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(HttpContext.Session.GetString("products"));
                ViewData["products"] = products;
                //Session["cart"] = selectedproducts;
               // return View("Index");
                return Json(data: count); 
            }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Logout()
        {
            List<Product> products = Tester.Run();
            ViewData["products"] = products;
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Clear();
            ViewData["username"] = null;
            ViewData["count"] = null;
            return View("Index");
        }
    }
}