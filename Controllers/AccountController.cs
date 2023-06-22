using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Website.Data;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using Social_Media_Website.ViewModels;
using Social_Media_Website.Helpers;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Social_Media_Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly SearchIndexer _searchtIndexer;
        private readonly Lucene.Net.Store.Directory _directory;


        public AccountController(UserManager<AppUser> userManager,SearchIndexer searchIndexer, IWebHostEnvironment env,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context)

        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _searchtIndexer = searchIndexer;
            var indexPath = Path.Combine(env.WebRootPath, "search_index_directory");
            _directory = new SimpleFSDirectory(new DirectoryInfo(indexPath));
        }


      

        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if (user != null)
            {
                //User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    //Password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Posts");
                    }
                }
                //Password is incorrect
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginViewModel);
            }
            //User not found
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

       
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress,
                CreationDate = DateTime.UtcNow,
                ProfileImageUrl = "/images/default_avatar.png",

            };

            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            // Add the product to the index
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);



            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var doc = new Document();
                doc.Add(new Field("Id", newUser.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("UserName", newUser.UserName, Field.Store.YES, Field.Index.ANALYZED));
                writer.AddDocument(doc);

                writer.Optimize();
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Posts");
        }



    }
}