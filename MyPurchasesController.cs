using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppinG_Cart.DB;
using ShoppinG_Cart.Models;
using ShoppinG_Cart.Models.ViewModel;

namespace ShoppinG_Cart.Controllers
{
    public class MyPurchasesController : Controller
    {
        private readonly Shopping_Cart_DBContext db;
        public MyPurchasesController(Shopping_Cart_DBContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {

            // var user = GetLogonUser();
            //var userId = "U02"; //user.Id
            var userId = HttpContext.Session.GetString("UserId");
            ViewData["username"] = HttpContext.Session.GetString("username");
            ViewData["count"] = HttpContext.Session.GetInt32("count");
            List<Order> orders = db.Orders.Where(m => m.UserId == userId).ToList(); //一个人有很多ORDER

            List<OrderDetail> userOrderDetails = new List<OrderDetail>();
            foreach (var order in orders) //一ORDER有很多物品,一个物品有可能有两个激活码
            {
                var tempOrderDetails = db.OrderDetails.Where(m => m.OrderId == order.OrderId).ToList();
                userOrderDetails.AddRange(tempOrderDetails);
            }

            List<ActivationCode> activationCodes = new List<ActivationCode>();
            foreach (var userOrder in userOrderDetails)
            {
                var tempActivationCode = db.ActivationCodes.Where(m => m.OrderId == userOrder.OrderId && m.ProductId == userOrder.ProductId).ToList();
                activationCodes.AddRange(tempActivationCode);
            }

            List<PurchaseHistoryViewModel> viewModels = new List<PurchaseHistoryViewModel>();
            foreach (var activationCode in activationCodes)
            {
                PurchaseHistoryViewModel viewModel = viewModels.FirstOrDefault(m => m.ProductId == activationCode.ProductId && m.OrderId == activationCode.OrderId);
                if (viewModel == null)
                {
                    var tempOrderDetail = db.OrderDetails.FirstOrDefault(m => m.OrderId == activationCode.OrderId && m.ProductId == activationCode.ProductId);
                    var tempProduct = db.Products.FirstOrDefault(m => m.ProductId == activationCode.ProductId);
                    var tempOrder = db.Orders.FirstOrDefault(m => m.OrderId == activationCode.OrderId);
                    var dateString = $"{tempOrder.OrderDate.Day}/{tempOrder.OrderDate.Month}/{tempOrder.OrderDate.Year}";
                    PurchaseHistoryViewModel tempViewModel = new PurchaseHistoryViewModel
                    {
                        OrderId = activationCode.OrderId,
                        ProductId = activationCode.ProductId,
                        ActivationCodeId = new List<string> { activationCode.ActivationCodeId },
                        ProductDescription = tempProduct.ProductDescription,
                        ProductPrice = tempProduct.ProductPrice,
                        ProductUrl = tempProduct.ImageURL,
                        ProductName = tempProduct.ProductName,
                        Orderdate = dateString,
                        Quantity = tempOrderDetail.Quantity
                    };
                    viewModels.Add(tempViewModel);
                }
                else
                {
                    viewModel.ActivationCodeId.Add(activationCode.ActivationCodeId);
                }
            }
            return View(viewModels);
        }
    }
}