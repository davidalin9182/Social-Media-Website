using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Website.ViewModels;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using Social_Media_Website.Helpers;
using Social_Media_Website.Repository;
using Lucene.Net.Search;

namespace Social_Media_Website.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IPostsRepository _postsRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;
        

        public FriendController(IFriendRepository friendRepository, IUserRepository userRepository, UserManager<AppUser> userManager, IPhotoService photoService,IProfileRepository profileRepository)
        {
            _friendRepository = friendRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _photoService = photoService;
            _profileRepository = profileRepository;
        }



        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            var friends = await _friendRepository.GetFriendsAsync(currentUser.Id);

            var friendIds = friends.Select(f => f.FriendId);

            List<AppUser> friendUsers;

            if (string.IsNullOrEmpty(searchTerm))
            {
                friendUsers = await _userRepository.GetAllUsers2();
            }
            else
            {
                friendUsers = await _userRepository.GetUserByName2(searchTerm);
            }

            var filteredUsers = friendUsers.Where(u => friendIds.Contains(u.Id)).ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(filteredUsers);
        }



        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var user = await _userRepository.GetUserById(id);
            if (user == null) return View("Error");
            var FollowingCount = await _friendRepository.GetFriendsCountAsync(id);
            var FolowersCount = await _friendRepository.GetFriendsWhoFollowUserCountAsync(id);
            var userPosts = await _profileRepository.GetAllUserPosts();
            var specificFriend = await _friendRepository.GetFriendUserAsync(currentUser.Id, id);
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


        public async Task<IActionResult> AddFriend(string friendId , string friendUsername)
        {
            if (string.IsNullOrEmpty(friendId))
            {
                return BadRequest("Friend ID cannot be null or empty.");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            var friend = await _userManager.FindByIdAsync(friendId);

            if (friend == null)
            {
                return NotFound("User not found.");
            }

            await _friendRepository.AddFriendAsync(currentUser.Id, friendId ,friendUsername);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            await _friendRepository.RemoveFriendAsync(currentUser.Id, friendId);

            return RedirectToAction(nameof(Index));
        }

    }
}
