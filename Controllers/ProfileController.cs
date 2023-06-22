using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Helpers;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using Social_Media_Website.Repository;
using Social_Media_Website.ViewModels;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Social_Media_Website.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPostsRepository _postsRepository;
        private readonly IFriendRepository _friendRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly SearchIndexer _searchIndexer;

        public ProfileController(IProfileRepository profileRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,SearchIndexer searchIndexer, IPostsRepository postsRepository, IFriendRepository friendRepository)
        {
            _userManager = userManager;
            _profileRepository = profileRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _searchIndexer = searchIndexer;
            _postsRepository = postsRepository;
            _friendRepository = friendRepository;
        }
        private void MapUserEdit(AppUser user,EditProfileViewModel editProfileVM/*,ImageUploadResult photoResult, ImageUploadResult photoResultCover*/)
        {
            user.Id = editProfileVM.Id;
            user.UserName = editProfileVM.UserName;
            user.Country = editProfileVM.Country;
            user.City = editProfileVM.City;
        }

        

        public async Task<IActionResult> Index()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _profileRepository.GetUserById(currentUserId);
            if (user == null) return View("Error");
            var FollowingCount = await _friendRepository.GetFriendsCountAsync(currentUserId);
            var FolowersCount = await _friendRepository.GetFriendsWhoFollowUserCountAsync(currentUserId);
            var userPosts = await _profileRepository.GetAllUserPosts();

            // Retrieve suggested users
            var suggestedUsers = await _friendRepository.GetFriendsOfFriendsAsync(currentUserId);

            var postViewModels = new List<PostsViewModel>();
            foreach (var post in userPosts)
            {
               

                // Get the comment count for the post
                var commentCount = await _postsRepository.GetCommentCount(post.Id);
                var comments = await _postsRepository.GetCommentsForPost(post.Id);
                // Get the like count for the specific post
                var likeCount = await _postsRepository.GetLikeCountByPostId(post.Id);
                // Retrieve the likes for the post
                var likes = await _postsRepository.GetLikesByPostId(post.Id);
                var isLikedByCurrentUser = currentUserId != null && likes.Any(l => l.UserId == currentUserId);
          
                var postViewModel = new PostsViewModel
                {
                    Id = post.Id,
                    PostTitle = post.PostTitle,
                    PostContent = post.PostContent,
                    AppUserId = post.AppUserId,
                    Image = post.Image,
                    DateCreated = post.DateCreated,
                    CommentsCount = commentCount,
                    LikesCount = likeCount,
                    Likes = likes,
                    IsLikedByCurrentUser = isLikedByCurrentUser,
                   
                };

                postViewModels.Add(postViewModel);
            }
          
            var profileViewModel = new ProfileViewModel()
            {
                Id = currentUserId,
                ProfileImageUrl = user.ProfileImageUrl ?? "/images/default_avatar.png",
                CoverUrl = user.CoverUrl ?? "/images/followbook_logo_2.png",
                UserName = user.UserName,
                Country = user.Country,
                City = user.City,
                Posts = postViewModels,
                Email = user.Email,
                FollowingCount = FollowingCount,
                FollowersCount = FolowersCount,
                SuggestedUsers = suggestedUsers,
                CreationDate =  user.CreationDate

            };

            return View(profileViewModel);
        }



        [HttpPost]
        public async Task<IActionResult> EditCoverImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
               
                var result = await _photoService.AddPhotoAsync(file);


                var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
                var user = await _profileRepository.GetUserById(currentUserId);
                user.CoverUrl = result.Url.ToString();
                _profileRepository.Update(user);

                return Ok(result.Url);
            }

            return BadRequest("No file was uploaded.");
        }

        
        [HttpPost]
        public async Task<IActionResult> EditProfileImage(IFormFile file)
        {
            

            if (file != null && file.Length > 0)
            {
                
                var result = await _photoService.AddPhotoAsync(file);

                var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
                var user = await _profileRepository.GetUserById(currentUserId);
                user.ProfileImageUrl = result.Url.ToString();
                _profileRepository.Update(user);

                return Ok(result.Url);
            }
            

            return BadRequest("File is null or empty");
        }

        [HttpGet]
        public async Task<IActionResult> EditUserProfile()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _profileRepository.GetUserById(currentUserId);
            if (user == null) return View("Error");
            var editProfileViewModel = new EditProfileViewModel()
            {
                Id = currentUserId,
                UserName = user.UserName,
                Country = user.Country,
                City = user.City,


            };
   
            return PartialView("_EditProfile", editProfileViewModel);
        }

      

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditProfileViewModel editProfileVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editProfileVM);
            }

            AppUser user = await _userManager.GetUserAsync(User);

      
            var userProfile = await _profileRepository.GetUserById(user.Id);

       
            user.UserName = editProfileVM.UserName;
            user.City = editProfileVM.City;
            user.Country = editProfileVM.Country;

            MapUserEdit(user, editProfileVM);
            _profileRepository.Update(user);

          
            await _userManager.ReplaceClaimAsync(user, User.FindFirst(ClaimTypes.Name), new Claim(ClaimTypes.Name, editProfileVM.UserName));

           
            await _signInManager.RefreshSignInAsync(user);

           
            _searchIndexer.UpdateUserProfile(userProfile.Id, user.UserName);

            return RedirectToAction("Index");
        }




        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentViewModel commentViewModel)
        {



            var comment = new Comment
            {
                PostId = commentViewModel.PostId,
                AppUserId = commentViewModel.AppUserId,
                AppUser = commentViewModel.AppUser,
                CommentContent = commentViewModel.CommentContent,
                DateCreated = DateTime.Now
            };

            _postsRepository.AddComment(comment);






            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        public async Task<IActionResult> LikePost(int postId)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var post = await _postsRepository.GetPostById(postId);

            if (post != null)
            {
               
                bool isLiked = _postsRepository.IsPostLikedByCurrentUser(post, currentUserId);

                if (isLiked)
                {
                    _postsRepository.RemoveLike(post, currentUserId);
                }
                else
                {
                    _postsRepository.AddLike(post, currentUserId);
                }

               
                return RedirectToAction(nameof(Index));
            }

           
            return NotFound();
        }

    


    }
}



