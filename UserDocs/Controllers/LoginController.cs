using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NHibernate;
using NHibernate.Criterion;
using UserDocs.Models;
using UserDocs.Core;

namespace UserDocs.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new UserLoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserLoginModel userLoginModel)
        {
            if (string.IsNullOrEmpty(userLoginModel.Username) || 
                (string.IsNullOrEmpty(userLoginModel.Password)))
            {
                ModelState.AddModelError("Username", "Заполните все поля!");
                return View(userLoginModel);
            }

            UserModel user = UserModel.Load(userLoginModel.Username, userLoginModel.Password);
            if (user == null)
            {
                ModelState.AddModelError("Username", "Неверные логин или пароль!");
                return View(userLoginModel);
            }
            Context.Instance.CurrentUser = user;
            FormsAuthentication.SetAuthCookie(userLoginModel.Username, true);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new UserRegisterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserRegisterModel userRegisterModel)
        {
            if (string.IsNullOrEmpty(userRegisterModel.Username) || 
                string.IsNullOrEmpty(userRegisterModel.Password) || 
                string.IsNullOrEmpty(userRegisterModel.PasswordCheck))
            {
                ModelState.AddModelError("Username", "Заполните все поля!");
                return View(userRegisterModel);
            }

            if (userRegisterModel.Password != userRegisterModel.PasswordCheck)
            {
                ModelState.AddModelError("", "Пароли не совпадают!");
                return View(userRegisterModel);
            }

            if (UserModel.Load(userRegisterModel.Username) != null)
            {
                ModelState.AddModelError("Username", "Пользователь с таким именем уже зарегистрирован в системе!");
                return View(userRegisterModel);
            }

            ITransaction tr = Context.Instance.Session.BeginTransaction();
            UserModel user = new UserModel() { Username = userRegisterModel.Username, Password = userRegisterModel.Password };
            try
            {
                Context.Instance.Session.Save(user);
                tr.Commit();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже зарегистрирован в системе!");
                return View(userRegisterModel);
            }
            Context.Instance.CurrentUser = user;
            FormsAuthentication.SetAuthCookie(userRegisterModel.Username, true);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}