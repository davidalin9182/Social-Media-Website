using Microsoft.AspNetCore.Mvc;
using Social_Media_Website.Data;
using Social_Media_Website.Models;
using Social_Media_Website.Interfaces;
using Social_Media_Website.ViewModels;
using System.Net.Mail;

namespace Social_Media_Website.Controllers
{
    public class AboutController : Controller
    {
        private readonly IPostsRepository _postsRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AboutController(IPostsRepository postsRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {

            _postsRepository = postsRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> Index1()
        {

            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateMail()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _postsRepository.GetUserById(currentUserId);
            var CreatePostsViewModel = new CreatePostsViewModel
            {
                AppUserId = currentUserId,
                UserName = user.UserName,
            };
            return PartialView("_CreateMail",CreatePostsViewModel);

        }
        [HttpPost]
        public async Task<IActionResult> CreateMail(CreatePostsViewModel PostsVM)
        {
            if (ModelState.IsValid)
            {
                if (PostsVM.Image != null)
                {
                    var result = await _photoService.AddPhotoAsync(PostsVM.Image);
                    var Posts = new Posts
                    {

                        PostName = PostsVM.UserName,
                        PostTitle = PostsVM.PostTitle,
                        PostContent = PostsVM.PostContent,
                        Image = result.Url.ToString(),
                        AppUserId = PostsVM.AppUserId,
                    };
                    _postsRepository.Add(Posts);
                    await SendEmailAsync(Posts);
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    var Posts = new Posts
                    {
                        PostProfileImage = PostsVM.ProfileImageUrl,
                        PostName = PostsVM.UserName,
                        PostTitle = PostsVM.PostTitle,
                        PostContent = PostsVM.PostContent,

                        AppUserId = PostsVM.AppUserId,
                    };
                    _postsRepository.Add(Posts);
                    await SendEmailAsync(Posts);
                    return RedirectToAction(nameof(Index));

                }

            }
            else
            {
                ModelState.AddModelError("", "Photo Upload Failed!");
            }
            return View(PostsVM);

        }

        private async Task SendEmailAsync(Posts Posts)
        {
            SmtpClient client = new SmtpClient
            {


                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,

                Credentials = new System.Net.NetworkCredential("caldararu.david.t9g@student.ucv.ro", "G4A95IASI"),



            };

            MailMessage message = new MailMessage("caldararu.david.t9g@student.ucv.ro", "davidalin9182@gmail.com");

            message.Subject = $"{Posts.PostTitle} from:{Posts.PostName} ";
            message.Body = $"{Posts.PostContent}{Posts.Image}";

            await client.SendMailAsync(message);
        }

    }
}
