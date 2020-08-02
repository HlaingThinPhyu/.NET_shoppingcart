using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppinG_Cart.DB;
using System.Web;

namespace ShoppinG_Cart.Controllers
{
    public class LoginController : Controller
    {
        readonly Shopping_Cart_DBContext db = new Shopping_Cart_DBContext();
        protected DBTester Tester;
        public LoginController(Shopping_Cart_DBContext db)
        {
            this.db = db;
            Tester = new DBTester(db);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult UserExists(string username, string hashpassword)
        {
            var found = db.Users.Where(model => model.UserName == username && model.Password == hashpassword).FirstOrDefault();
            if (found == null)
            {
                return Json(data: 0);
            }
            else
            {
                HttpContext.Session.SetString("username", found.FullName);
                HttpContext.Session.SetString("UserId", found.UserId);
                return Json(data: 1);
            }

        }

    }
}