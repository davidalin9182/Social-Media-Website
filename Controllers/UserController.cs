using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Website.ViewModels;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using Social_Media_Website.Helpers;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;
using Social_Media_Website.Repository;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Social_Media_Website.Controllers
{
    public class UserController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPostsRepository _postsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendRepository _friendRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;
        private readonly SearchIndexer _searchIndexer;
        private readonly Lucene.Net.Store.Directory _directory;

        public UserController(IUserRepository userRepository, IFriendRepository friendRepository, UserManager<AppUser> userManager, SearchIndexer searchIndexer, IPhotoService photoService, IWebHostEnvironment env, IProfileRepository profileRepository, IPostsRepository postsRepository)
        {
            _userRepository = userRepository;
            _friendRepository = friendRepository;
            _userManager = userManager;
            _photoService = photoService;
            _searchIndexer = searchIndexer;
            var indexPath = Path.Combine(env.WebRootPath, "search_index_directory");
            _directory = new SimpleFSDirectory(new DirectoryInfo(indexPath));
            _profileRepository = profileRepository;
            _postsRepository = postsRepository;
        }


        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friends = await _friendRepository.GetFriendsAsync(currentUser.Id);
            var friendIds = friends.Select(f => f.FriendId).ToList();

            if (currentUser == null)
            {
                return NotFound();
            }

            IEnumerable<AppUser> users;

            if (string.IsNullOrEmpty(searchTerm))
            {
                users = await _userRepository.GetAllUsers();
            }
            else
            {
                users = await _userRepository.GetUserByName(searchTerm);
            }



            // Filter friends
            users= users.Where(u => !friendIds.Contains(u.Id));


            // Filter out the current user
            users = users.Where(u => u.Id != currentUser.Id).ToList();


            var result = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfileImageUrl = user.ProfileImageUrl ?? "/images/default_avatar.png",
                CoverUrl = user.CoverUrl ?? "followbook_logo_2.png",
                Country = user.Country,
                City = user.City,
  
            }).ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friends = await _friendRepository.GetFriendsAsync(currentUser.Id);
            var friendIds = friends.Select(f => f.FriendId).ToList();
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction("Index");
            }

            var users = _userRepository.GetUserByName(searchTerm).Result;
       
            users = users.Where(u => !friendIds.Contains(u.Id));


            users = users.Where(u => u.Id != currentUser.Id).ToList();

            return View("Index", users);
        }


        //[HttpGet]
        //public async Task<IActionResult> TypeAhead(string term)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);
        //    var friends = await _friendRepository.GetFriendsAsync(currentUser.Id);
        //    var friendNames = friends.Select(f => f.FriendUsername).ToList();
        //    var currentUserName = currentUser.UserName;
        //    using (var searcher = new IndexSearcher(_directory, true))
        //    {
        //        var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        //        var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "UserName" }, analyzer);
        //        parser.DefaultOperator = QueryParser.Operator.OR;

        //        var query = parser.Parse(term + "*");
        //        var hits = searcher.Search(query, 5).ScoreDocs;

        //        var suggestions = hits
        //            .Select(hit => searcher.Doc(hit.Doc).Get("UserName"))
        //            .Where(suggestion => !friendNames.Contains(suggestion) && !currentUserName.Contains(suggestion)) // Filter out friends' usernames
        //            .ToList();



        //        return Json(suggestions);
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> TypeAheadFriends(string term)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);
        //    var friends = await _friendRepository.GetFriendsAsync(currentUser.Id);
        //    var friendNames = friends.Select(f => f.FriendUsername).ToList();

        //    using (var searcher = new IndexSearcher(_directory, true))
        //    {
        //        var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        //        var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "UserName" }, analyzer);
        //        parser.DefaultOperator = QueryParser.Operator.OR;

        //        var query = parser.Parse(term + "*");
        //        var hits = searcher.Search(query, 5).ScoreDocs;
      

        //        var suggestions = hits
        //            .Select(hit => searcher.Doc(hit.Doc).Get("UserName"))
        //            .Where(suggestion => friendNames.Contains(suggestion)) // Filter out friends' usernames
        //            .ToList();

        //        return Json(suggestions);
        //    }
        //}

      

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var friends = await _friendRepository.GetFriendsAsync(currentUser.Id);
           
            var user = await _userRepository.GetUserById(id);
            var userPosts = await _profileRepository.GetAllUserPosts();

            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var friendIds = friends.Select(f => f.FriendId).ToList();
            var specificFriend = await _friendRepository.GetFriendUserAsync(currentUser.Id, id);
            var FollowingCount = await _friendRepository.GetFriendsCountAsync(id);
            var FolowersCount = await _friendRepository.GetFriendsWhoFollowUserCountAsync(id);
          
            // Retrieve suggested users
            var suggestedUsers = await _friendRepository.GetFriendsOfFriendsAsync(id);
            bool isFriend;
            if (user.UserName == specificFriend?.UserName) 
            {
                isFriend = true;
            }
            else
            {
                isFriend = false;
            }
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
                var isLikedByCurrentUser = id != null && likes.Any(l => l.UserId == id);

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

            var userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfileImageUrl = user.ProfileImageUrl ?? "/images/default_avatar.png",
                CoverUrl = user.CoverUrl ?? "/images/followbook_logo_2.png",
                Country = user.Country,
                City = user.City,
                Posts = postViewModels,
                Email = user.Email,
                FollowingCount = FollowingCount,
                FollowersCount = FolowersCount,
                SuggestedUsers = suggestedUsers,
                CreationDate = user.CreationDate,
                IsFriend = isFriend,
            };
            return View(userDetailViewModel);
        }

       
        

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            var editMV = new EditProfileViewModel()
            {
                ProfileImageUrl = user.ProfileImageUrl,
                CoverUrl = user.CoverUrl,
            };
            return View(editMV);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditProfile", editVM);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            if (editVM.Image != null) // only update profile image
            {
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                var photoResultCover = await _photoService.AddPhotoAsync(editVM.CoverImage);

                if (photoResult.Error != null)
                {
                    ModelState.AddModelError("Image", "Failed to upload image");
                    return View("EditProfile", editVM);
                }
                if (photoResultCover.Error != null)
                {
                    ModelState.AddModelError("Image", "Failed to upload image");
                    return View("EditProfile", editVM);
                }

                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    _ = _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }

                user.ProfileImageUrl = photoResult.Url.ToString();
                editVM.ProfileImageUrl = user.ProfileImageUrl;
                user.CoverUrl = photoResult.Url.ToString();
                editVM.CoverUrl = user.CoverUrl;

                await _userManager.UpdateAsync(user);

                return View(editVM);
            }


            await _userManager.UpdateAsync(user);

            return RedirectToAction("Detail", "User", new { user.Id });
        }
    }
}