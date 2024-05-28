using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using HieuThuoc.Models;
using Recom_Pharmacy.Models;

namespace HieuThuoc.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        RecomPharmacyEntities db = new RecomPharmacyEntities();
		
		public ActionResult Login()
		{
			return View();

		}
		[HttpPost]
		public ActionResult Login(FormCollection collection)
        {
			
			var userName = collection["userName"];
			var passWord = collection["passWord"];
			//passWord = Encryption.ComputeHash(passWord, "SHA512", GetBytes("acc"));
		
			KHACHHANG cs = db.KHACHHANGs.SingleOrDefault(n => n.Username == userName && n.Passwords == passWord);
				//Customer cs = db.Customers.SingleOrDefault(n => n.Username == userName && n.Passwords == passWord);
				if (cs != null)
				{
					
					Session["usr"] = cs;
					return RedirectToAction("Index", "UserInterface");
				}
				else
					ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng");
			//}
			return View();
			
		}
		
		public ActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public ActionResult Register(FormCollection collection)
		{
			try
			{
				string userName = collection["Username"];
				string passWord = collection["Password"];
				string conFirmPassWord = collection["ConfirmPassword"];
				string name = collection["Name"];
				DateTime Birthday;
				string Email = collection["Email"];
				string address = collection["Address"];
				string Gender = collection["Gender"];
				string phoneNumber = collection["PhoneNumber"];
                ViewBag.Username = userName;
                ViewBag.Password = passWord;
                ViewBag.ConfirmPassword = conFirmPassWord;
                ViewBag.Name = name;
                ViewBag.Email = Email;
                ViewBag.Address = address;
                ViewBag.Gender = Gender;
                ViewBag.PhoneNumber = phoneNumber;
                ViewBag.Birthday = collection["Birthday"];
                if (!DateTime.TryParse(collection["Birthday"], out Birthday))
                {
                    ModelState.AddModelError("Birthday", "Please enter a valid Birthday");
                }
                if (userName != null && passWord == conFirmPassWord)
				{
			
				
						var tem = db.KHACHHANGs.SingleOrDefault(a => a.Username == userName);
						if (tem == null)
						{
							KHACHHANG cs = new KHACHHANG();
							cs.Username = userName;
							cs.Passwords = passWord;
							cs.TENKH = name;
							cs.EMAIL = Email;
							cs.NGAYSINH = Birthday;
							cs.DIACHI = address;
							cs.GIOITINH = Gender;
							cs.SDT = phoneNumber;
							db.KHACHHANGs.Add(cs);
							db.SaveChanges();
						}
			
					else
					{
						ModelState.AddModelError("", "Tài khoản đã tồn tại !");
						return View();
					}
					return RedirectToAction("Login", "Login");
				}
				else
				{
					ViewBag.Confirm = "Mật khẩu không trùng khớp";
				}

				
			}
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError("", validationError.ErrorMessage);
                    }
                }
            }
			return View();
        }
		public ActionResult Forgotpassword()
		{
			if (Session["usr"] == null)
			{
				return RedirectToAction("Login", "Login");
			}
			var ac = ((KHACHHANG)Session["usr"]);

			return View(new AccountClientEntity(ac));
		}
		[HttpPost]

		public ActionResult Forgotpassword(FormCollection fc)
		{
			var ac = ((KHACHHANG)Session["usr"]);

			if (Session["usr"] != null)
			{
				string userName = fc["userName"].ToString();
				string pass = fc["pass"].ToString();
				string newpass = fc["newpass"].ToString();
				string repass = fc["repass"].ToString();
				var temp = db.KHACHHANGs.SingleOrDefault(x => x.Username == userName && x.Passwords == pass);
				if (temp != null && pass != "" && newpass != pass && newpass != "" && newpass == repass)
				{
					temp.Passwords = fc["newpass"];
					db.SaveChanges();
					Session["usr"] = temp;
					return RedirectToAction("Profile", "AuraStore");

				}



			}
			else
			{
				return RedirectToAction("Index", "UserInterface");
			}
			ModelState.AddModelError("", "Không thể thay đổi mật khẩu");
			return View(new AccountClientEntity(ac));
		}

		public ActionResult Changepassword()
		{
			if (Session["usr"] == null)
			{
				return RedirectToAction("Login", "Login");
			}
			var ac = ((KHACHHANG)Session["usr"]);

			return View(new AccountClientEntity(ac));
		}
		[HttpPost]

		public ActionResult Changepassword(FormCollection fc)
		{
			//string userName = collection["Username"];
			//string passWord = collection["Password"];
			//string conFirmPassWord = collection["ConfirmPassword"];
			//string name = collection["Name"];
			//var Birthday = String.Format("{0:MM/dd/yyyy}", collection["Birthday"]);
			//string Email = collection["Email"];
			//string address = collection["Address"];
			//int Gender = Convert.ToInt32(collection["Gender"]);
			//string phoneNumber = collection["PhoneNumber"];
			var ac = ((KHACHHANG)Session["usr"]);

			if (Session["usr"] != null)
			{
				string userName = fc["userName"].ToString();
				string pass = fc["pass"].ToString();
				string newpass = fc["newpass"].ToString();
				string repass = fc["repass"].ToString();
				//pass = Encryption.ComputeHash(pass, "SHA512", GetBytes("acc"));
				//newpass = Encryption.ComputeHash(newpass, "SHA512", GetBytes("acc"));
				//repass = Encryption.ComputeHash(repass, "SHA512", GetBytes("acc"));
				var temp = db.KHACHHANGs.SingleOrDefault(x => x.Username == userName && x.Passwords == pass);
				if (temp != null && pass != "" && newpass != pass && newpass != "" && newpass == repass)
				{
					temp.Passwords = fc["newpass"];
					db.SaveChanges();
					Session["usr"] = temp;
					return RedirectToAction("Profile", "UserInterface");

				}
			}
			else
			{
				return RedirectToAction("Index", "UserInterface");
			}
			ModelState.AddModelError("", "Không thể thay đổi mật khẩu..");
			return View(new AccountClientEntity(ac));
		}
		public static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
	}
}