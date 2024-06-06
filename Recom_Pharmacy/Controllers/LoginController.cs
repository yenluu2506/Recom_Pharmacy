using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using HieuThuoc.Models;
using Recom_Pharmacy;
using Recom_Pharmacy.Models;
using System.Threading.Tasks;
using Vonage.Messaging;
using Vonage;
using System.Configuration;
using Vonage.Request;

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
		public ActionResult Register(FormCollection collection, string email)
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
                        var existingUser = db.KHACHHANGs.SingleOrDefault(a => a.Username == userName);
                        var existingEmail = db.KHACHHANGs.SingleOrDefault(a => a.EMAIL == Email);
                        var existingSDT = db.KHACHHANGs.SingleOrDefault(a => a.SDT == phoneNumber);

                        if (existingUser != null)
                        {
                            ModelState.AddModelError("", "Tài khoản đã tồn tại!");
                            return View();
                        }

                        if (existingEmail != null)
                        {
                            ModelState.AddModelError("", "Email đã tồn tại!");
                            return View();
                        }
                        if (existingSDT != null)
                        {
                            ModelState.AddModelError("", "Số điện thoại đã tồn tại!");
                            return View();
                        }
                        // Generate verification code
                        string verificationCode = Guid.NewGuid().ToString();
                        //string phoneVerificationCode = new Random().Next(100000, 999999).ToString();

                        // Create callback URL
                        string callbackUrl = Url.Action("VerifyEmail", "Login", new { code = verificationCode }, protocol: Request.Url.Scheme);

                        // Send verification email
                        SendVerificationEmail(Email, callbackUrl);
                        //send verification phone
                        //SendVerificationSms(phoneNumber, phoneVerificationCode);

                        // Temporarily store user data (e.g., in session or a temporary table)
                        TempData["RegisterData"] = new
                        {
                            Username = userName,
                            Password = passWord,
                            Name = name,
                            Birthday = Birthday,
                            Email = Email,
                            Address = address,
                            Gender = Gender,
                            PhoneNumber = phoneNumber,
                            VerificationCode = verificationCode,
                            ExpiryDate = DateTime.Now.AddMinutes(3)
                            //PhoneVerificationCode = phoneVerificationCode
                        };

                        return RedirectToAction("VerifyEmail");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản đã tồn tại !");
                        return View();
                    }
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

        private void SendVerificationEmail(string email, string callbackUrl)
        {
            var fromAddress = new MailAddress("thienlinh0112000@gmail.com", "Pharmacy");
            var toAddress = new MailAddress(email);
            const string fromPassword = "yxvkzrvlohrojeco";
            const string subject = "Email Verification";
            string body = $"Please verify your email by clicking the following link: {callbackUrl}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        //private async Task SendVerificationSms(string phoneNumber, string verificationCode)
        //{
        //    // Lấy thông tin cấu hình từ appSettings
        //    var apiKey = ConfigurationManager.AppSettings["NexmoApiKey"];
        //    var apiSecret = ConfigurationManager.AppSettings["NexmoApiSecret"];
        //    var fromPhoneNumber = ConfigurationManager.AppSettings["NexmoPhoneNumber"];

        //    // Tạo cấu hình cho Vonage client
        //    var credentials = Credentials.FromApiKeyAndSecret(apiKey, apiSecret);
        //    var client = new VonageClient(credentials);

        //    // Tạo yêu cầu gửi SMS
        //    var request = new SendSmsRequest
        //    {
        //        To = phoneNumber,
        //        From = fromPhoneNumber,
        //        Text = $"Mã xác nhận của bạn là: {verificationCode}"
        //    };

        //    // Gửi SMS và lấy phản hồi
        //    var response = await client.SmsClient.SendAnSmsAsync(request);

        //    if (response.Messages[0].Status != "0")
        //    {
        //        throw new Exception($"Failed to send SMS: {response.Messages[0].ErrorText}");
        //    }
        //}
        public ActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyEmail(string code)
        {
            var registerData = TempData["RegisterData"] as dynamic;
            if (registerData != null && (registerData.VerificationCode == code))
            {
                // Save the user to the database
                //KHACHHANG cs = new KHACHHANG
                //{
                //    Username = registerData.Username,
                //    Passwords = registerData.Password,
                //    TENKH = registerData.Name,
                //    EMAIL = registerData.Email,
                //    NGAYSINH = registerData.Birthday,
                //    DIACHI = registerData.Address,
                //    GIOITINH = registerData.Gender,
                //    SDT = registerData.PhoneNumber
                //};
                //db.KHACHHANGs.Add(cs);
                //db.SaveChanges();

                //ViewBag.Message = "Email verified successfully!";
                //return RedirectToAction("Login", "Login");
                if (DateTime.Now <= registerData.ExpiryDate)
                {
                    // Mã xác nhận hợp lệ và chưa hết hạn
                    // Thêm người dùng vào cơ sở dữ liệu và chuyển hướng
                    KHACHHANG cs = new KHACHHANG
                    {
                        Username = registerData.Username,
                        Passwords = registerData.Password,
                        TENKH = registerData.Name,
                        EMAIL = registerData.Email,
                        NGAYSINH = registerData.Birthday,
                        DIACHI = registerData.Address,
                        GIOITINH = registerData.Gender,
                        SDT = registerData.PhoneNumber
                    };
                    db.KHACHHANGs.Add(cs);
                    db.SaveChanges();

                    ViewBag.Message = "Email verified successfully!";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    ViewBag.ErrorMessage = "Mã xác nhận đã hết hạn. Vui lòng yêu cầu gửi lại mã.";
                    ViewBag.ShowResendButton = true; // Hiển thị nút "Gửi lại mã"
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Mã xác nhận không chính xác hoặc đã hết hạn. Vui lòng nhập lại.";
                // Tạo một đối tượng mới với mã xác nhận mới và thời gian hết hạn mới
                var newRegisterData = new
                {
                    Username = registerData.Username,
                    Password = registerData.Password,
                    Name = registerData.Name,
                    Birthday = registerData.Birthday,
                    Email = registerData.Email,
                    Address = registerData.Address,
                    Gender = registerData.Gender,
                    PhoneNumber = registerData.PhoneNumber,
                    VerificationCode = Guid.NewGuid().ToString(),
                    ExpiryDate = DateTime.Now.AddMinutes(3) // Ví dụ: hết hạn sau 30 phút
                };

                // Cập nhật lại TempData với đối tượng mới
                TempData["RegisterData"] = newRegisterData;

                ViewBag.ShowResendButton = true; // Hiển thị nút "Gửi lại mã"
            } 
            return View();
        }
        public ActionResult ResendVerificationEmail()
        {
            var registerData = TempData["RegisterData"] as dynamic;
            if (registerData != null)
            {
                // Cập nhật lại mã xác nhận và thời gian hết hạn mới
                registerData.VerificationCode = Guid.NewGuid().ToString();
                registerData.ExpiryDate = DateTime.Now.AddMinutes(3); // Ví dụ: hết hạn sau 30 phút

                // Cập nhật lại TempData
                TempData["RegisterData"] = registerData;

                // Gửi email mới
                string newCallbackUrl = Url.Action("VerifyEmail", "Login", new { code = registerData.VerificationCode }, protocol: Request.Url.Scheme);
                SendVerificationEmail(registerData.Email, newCallbackUrl);

                ViewBag.Message = "Mã xác nhận mới đã được gửi qua email.";
                return RedirectToAction("VerifyEmail");
            }
            else
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
            }
            return View("VerifyEmail");
        }
        public ActionResult VerifyEmailConfirm()
        {
            return View();
        }

        public ActionResult Forgotpassword()
		{
			return View();
		}
		[HttpPost]

		public ActionResult Forgotpassword(FormCollection fc, string code, ForgotPasswordViewModel model)
		{
            if (ModelState.IsValid)
            {
                var user = db.KHACHHANGs.SingleOrDefault(x => x.EMAIL == model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // Tạo mật khẩu mới ngẫu nhiên
                string newPassword = GenerateRandomPassword();

                // Cập nhật mật khẩu mới trong cơ sở dữ liệu (bạn nên mã hóa mật khẩu này)
                user.Passwords = newPassword;
                db.SaveChanges();

                // Gửi email chứa mật khẩu mới
                SendNewPasswordEmail(user.EMAIL, newPassword);
                return RedirectToAction("ForgotPasswordConfirmation", "Login");
            }

            return View(model);
        }

        private string GeneratePasswordResetToken(string email)
        {
            string token = Guid.NewGuid().ToString();
            MvcApplication.Cache.Set(token, email, DateTimeOffset.Now.AddHours(1)); // Token expires in 1 hour
            return token;
        }
        private void SendNewPasswordEmail(string email, string newPassword)
        {
            string subject = "Your New Password";
            string body = $"Your new password is: {newPassword}. Please log in and change your password.";

            using (var message = new MailMessage("thienlinh0112000@gmail.com", email))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential("thienlinh0112000@gmail.com", "yxvkzrvlohrojeco");
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
        }
        public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}
        private string GenerateRandomPassword(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult ResetPassword(string userId, string code)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId))
            {
                return View("Error");
            }

            var model = new ResetPasswordViewModel { Code = code, UserName = userId };
            return View(model);
        }
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = MvcApplication.Cache.Get(model.Code) as string;
            if (email == null)
            {
                return View("Error");
            }

            var user = db.KHACHHANGs.SingleOrDefault(x => x.EMAIL == email && x.ID.ToString() == model.UserName);
            if (user == null)
            {
                return View("Error");
            }

            // Mã hóa mật khẩu mới trước khi lưu vào cơ sở dữ liệu
            user.Passwords = model.NewPassword; // Bạn có thể thêm mã hóa mật khẩu tại đây
            db.SaveChanges();

            // Xóa token sau khi sử dụng
            MvcApplication.Cache.Remove(model.Code);

            return RedirectToAction("ResetPasswordConfirmation", "Login");
        }
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
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
		public ActionResult ChangepasswordConfirm()
		{
			return View();
		}

        public static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
	}
}