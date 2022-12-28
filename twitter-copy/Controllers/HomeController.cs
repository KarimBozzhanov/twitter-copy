using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using twitter_copy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using twitter_copy.ViewModels;
using System.Security.Claims;

namespace twitter_copy.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;

        public HomeController(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.AddPosts.OrderByDescending(a => a.TimeOfCreating).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateModel createModel)
        {
            db.AddPosts.Add(new AddPosts { postText = createModel.postText, TimeOfCreating = createModel.Date_of_creating, UserName = User.Identity.Name});
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> postDetail(int? id)
        {
            if (id != null)
            {
                AddPosts post = await db.AddPosts.FirstOrDefaultAsync(p => p.Id == id);
                if (post != null)
                {
                    return View(post);
                }
            }
            return NotFound();
        }
        [HttpGet]
        [ActionName("postDelete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                AddPosts post = await db.AddPosts.FirstOrDefaultAsync(p => p.Id == id);
                if (post != null)
                {
                    return View();
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> postDelete(int? id)
        {
            if (id != null)
            {
                AddPosts post = new AddPosts { Id = id.Value };
                db.Entry(post).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> editPost(int? id)
        {
            if (id != null)
            {
                AddPosts post = await db.AddPosts.FirstOrDefaultAsync(p => p.Id == id);
                if (post != null)
                {
                    return View(post);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> editPost(AddPosts post)
        {
            db.AddPosts.Update(post);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.User.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Login);
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Некорректные логин или пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.User.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    db.User.Add(new User { Login = model.Login, Email = model.Email, Password = model.Password });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Login); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> allAccount()
        {
            return View(await db.User.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> editAccount(int? id)
        {
           if (id != null)
            {
                User user = await db.User.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    return View(user);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> editAccount(User user)
        {
            db.User.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("allAccount");
        }

        [HttpGet]
        [ActionName("deleteAccount")]
        public async Task<IActionResult> CondirmDeleteAccount(int? id)
        {
            if (id != null)
            {
                User user = await db.User.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    return View();
                }
            }
            return NotFound();
        }

        [HttpPost] 
        public async Task<IActionResult> deleteAccount(int? id)
        {
            if (id != null)
            {
                User user = new User { Id = id.Value };
                db.Entry(user).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("allAccount");
            }
            return NotFound();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
