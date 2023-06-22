using Microsoft.AspNetCore.Mvc;
using Social_Media_Website.Data;
using Social_Media_Website.Models;
using Social_Media_Website.Interfaces;
using Social_Media_Website.ViewModels;
using X.PagedList;
using Social_Media_Website.Repository;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
//using Social_Media_Website.Migrations;

namespace Social_Media_Website.Controllers
{
    public class PostsController : Controller
    {

        private readonly IPostsRepository _postsRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFriendRepository _friendRepository;
        private object _userRepository;

        public PostsController(IPostsRepository postsRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor,IFriendRepository friendRepository)
        {

            _postsRepository = postsRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
            _friendRepository = friendRepository;
        }

        public async Task<IActionResult> Index()
        {
            

            var posts = await _postsRepository.GetAll();

            // Create a list to store the view models
            var postViewModels = new List<PostsViewModel>();
        

            foreach (var post in posts)
            {
                var user = await _postsRepository.GetUserById(post.AppUserId);

                // Get the comment count for the post
                var commentCount = await _postsRepository.GetCommentCount(post.Id);
                var comments = await _postsRepository.GetCommentsForPost(post.Id);
                // Get the like count for the specific post
                var likeCount = await _postsRepository.GetLikeCountByPostId(post.Id);
                // Retrieve the likes for the post
                var likes = await _postsRepository.GetLikesByPostId(post.Id);
                bool isLikedByCurrentUser =false;
                if (User.Identity.IsAuthenticated)
                {
                    var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
                    isLikedByCurrentUser = currentUserId != null && likes.Any(l => l.UserId == currentUserId);

                }
                else
                {
                    isLikedByCurrentUser = false;
                }

                var postitem = await _postsRepository.GetPostById(post.Id);
                postitem.Likes = likes;
                postitem.LikesCount = likeCount;
                postitem.Comments = comments;
                postitem.CommentsCount = commentCount;
                var postViewModel = new PostsViewModel
                {
                    Id = post.Id,
                    PostTitle = post.PostTitle,
                    PostContent = post.PostContent,
                    Image = post.Image,
                    UserName = user.UserName,
                    AppUserId = user.Id,
                    ProfileImageUrl = user.ProfileImageUrl ?? "/images/default_avatar.png",
                    CommentsCount = commentCount,
                    LikesCount = likeCount,
                    Likes = likes,
                    IsLikedByCurrentUser = isLikedByCurrentUser,
                    DateCreated= post.DateCreated,
                };

                postViewModels.Add(postViewModel);
            }

          
            var AllPosts = postViewModels.ToList();

            return View(AllPosts);
        }




        public async Task<IActionResult> GetSuggestedUsers()
        {

            var suggestedUsers = await _friendRepository.GetMostFollowedUsersAsync(10); 

            var suggestedUsersViewModel = new SugestedUsersViewModel
            {
                SuggestedUsers = suggestedUsers
            };

            return PartialView("_SuggestedUsers", suggestedUsersViewModel);
        }


        public async Task<IActionResult> FollowedPostsPartial()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            // Get the followed posts from the repository or service
            var followedPosts = await _postsRepository.GetFollowedPosts(currentUserId); 

            var followedpostsVM = new List<PostsViewModel>();

            foreach (var post in followedPosts)
            {
                var user = await _postsRepository.GetUserById(post.AppUserId);

                // Get the comment count for the post
                var commentCount = await _postsRepository.GetCommentCount(post.Id);
                var comments = await _postsRepository.GetCommentsForPost(post.Id);
                // Get the like count for the specific post
                var likeCount = await _postsRepository.GetLikeCountByPostId(post.Id);
                // Retrieve the likes for the post
                var likes = await _postsRepository.GetLikesByPostId(post.Id);
                bool isLikedByCurrentUser = false;
                if (User.Identity.IsAuthenticated)
                {
                   
                    isLikedByCurrentUser = currentUserId != null && likes.Any(l => l.UserId == currentUserId);

                }
                else
                {
                    isLikedByCurrentUser = false;
                }

                // Create an instance of FollowedPostsViewModel
                var postviewModel = new PostsViewModel
                {
                    Id = post.Id,
                    PostTitle = post.PostTitle,
                    PostContent = post.PostContent,
                    Image = post.Image,
                    UserName = user.UserName,
                    AppUserId = user.Id,
                    ProfileImageUrl = user.ProfileImageUrl ?? "/images/default_avatar.png",
                    CommentsCount = commentCount,
                    LikesCount = likeCount,
                    Likes = likes,
                    IsLikedByCurrentUser = isLikedByCurrentUser,
                    DateCreated = post.DateCreated,

                };

                followedpostsVM.Add(postviewModel);
            }

            var AllFollowedPosts = followedpostsVM.ToList();
            // Return the partial view with the followed posts
            return PartialView("_FollowedPosts", AllFollowedPosts);
        }





        [HttpGet]
        public async Task<IActionResult> CreateComment(int postId)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var user = await _postsRepository.GetUserById(currentUserId);
            var CreateCommentViewModel = new CreateCommentViewModel
            {
                PostId = postId,
                AppUserId = currentUserId,
                AppUser = user,
                ProfileImageUrl = user.ProfileImageUrl ?? "/images/default_avatar.png",
                UserName = user.UserName,
                DateCreated = DateTime.Now
            };

            return PartialView("_CreateComment", CreateCommentViewModel);
          
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

        [HttpGet]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _postsRepository.GetCommentByIdAsync(id);
            if (comment == null) return View("Error");
            var CommentVM = new EditCommentViewModel
            {

                CommentContent = comment.CommentContent,
                AppUserId = comment.AppUserId,
                DateCreated = DateTime.Now

            };

            return PartialView("_EditComment", CommentVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditComment(int id, EditCommentViewModel commentVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit comment");
                return PartialView("_EditComment", commentVM);
            }

            var userComment = await _postsRepository.GetCommentByIdAsyncNoTracking(id);

            if (userComment == null)
            {
                return View("Error");
            }
           
            
                var Comment = new Comment
                {
                    Id = id,
                    AppUserId = commentVM.AppUserId,
                    CommentContent = commentVM.CommentContent,
                    DateCreated = DateTime.Now

                };
                _postsRepository.UpdateComment(Comment);
            



            return RedirectToAction("Index", "Profile");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var Detail = await _postsRepository.GetCommentByIdAsync(id);
            if (Detail == null) return View("Error");

            return PartialView("_DeleteComment", Detail);
        }

        [HttpPost, ActionName("DeleteComment")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var CommentDetails = await _postsRepository.GetCommentByIdAsync(id);

            if (CommentDetails == null)
            {
                return View("Error");
            }

            _postsRepository.DeleteComment(CommentDetails);
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


        [HttpGet]
        public async Task<IActionResult> GetCommentsForPost(int postId)
        {
            var comments = await _postsRepository.GetCommentsForPost(postId);

            return PartialView("_CommentSection", comments);
        }

      


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _postsRepository.GetUserById(currentUserId);
            var CreatePostsViewModel = new CreatePostsViewModel 
            { 
                AppUserId = currentUserId ,
                ProfileImageUrl = user.ProfileImageUrl ?? "/images/default_avatar.png", 
                UserName = user.UserName,
                DateCreated = DateTime.Now
            };
            
            return PartialView("_Create", CreatePostsViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostsViewModel PostsVM)
        {
            if (ModelState.IsValid)
            {
                
                if (PostsVM.Image != null)
                {
                    var result = await _photoService.AddPhotoAsync(PostsVM.Image);
                    var Posts = new Posts
                    {
                        PostProfileImage = PostsVM.ProfileImageUrl ?? "/images/default_avatar.png",
                        PostName = PostsVM.UserName,
                        PostTitle = PostsVM.PostTitle,
                        PostContent = PostsVM.PostContent,
                        Image = result.Url.ToString(),              
                        AppUserId = PostsVM.AppUserId,
                        DateCreated = DateTime.Now
                    };
                    _postsRepository.Add(Posts);
                    return RedirectToAction(nameof(Index));
                }
                else
                {

                    var Posts = new Posts
                    {
                        PostProfileImage = PostsVM.ProfileImageUrl ?? "/images/default_avatar.png",
                        PostName = PostsVM.UserName,
                        PostTitle = PostsVM.PostTitle,
                        PostContent = PostsVM.PostContent,

                        AppUserId = PostsVM.AppUserId,
                        DateCreated = DateTime.Now
                    };
                    _postsRepository.Add(Posts);
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                ModelState.AddModelError("", "Photo Upload Failed!");
            }
        
            return PartialView("_Edit", PostsVM);
        }

      
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Posts = await _postsRepository.GetByIdAsync(id);
            if (Posts == null) return View("Error");
            var PostsVM = new EditPostsViewModel
            {

                PostTitle = Posts.PostTitle,
                PostContent = Posts.PostContent,
                AppUserId = Posts.AppUserId,
                URL = Posts?.Image?.ToString(),
                DateCreated = DateTime.Now

            };

            return PartialView("_Edit", PostsVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditPostsViewModel PostsVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit post");
                return PartialView("_Edit", PostsVM);
            }

            var userPosts = await _postsRepository.GetByIdAsyncNoTracking(id);

            if (userPosts == null)
            {
                return View("Error");
            }
            if (PostsVM.Image != null)
            {
                var photoResult = await _photoService.AddPhotoAsync(PostsVM.Image);

                if (photoResult.Error != null)
                {
                    ModelState.AddModelError("Image", "Photo upload failed");
                    return View(PostsVM);
                }

                var Posts = new Posts
                {
                    Id = id,
                    PostTitle = PostsVM.PostTitle,
                    PostContent = PostsVM.PostContent,
                    AppUserId = PostsVM.AppUserId,
                    Image = photoResult.Url.ToString(),
                    DateCreated = DateTime.Now
                };
                _postsRepository.Update(Posts);
            }
            else if(PostsVM.Image == null && userPosts.Image == null)
            {
                var Posts = new Posts
                {
                    Id = id,
                    PostTitle = PostsVM.PostTitle,
                    PostContent = PostsVM.PostContent,
                    AppUserId = PostsVM.AppUserId,
                    DateCreated = DateTime.Now

                };
                _postsRepository.Update(Posts);
            }
            else
            {
                
                var Posts = new Posts
                {
                    Id = id,
                    PostTitle = PostsVM.PostTitle,
                    PostContent = PostsVM.PostContent,
                    AppUserId = PostsVM.AppUserId,
                    Image = userPosts.Image,
                    DateCreated = DateTime.Now

                };
                _postsRepository.Update(Posts);
            }

            if (!string.IsNullOrEmpty(userPosts.Image))
            {
                _ = _photoService.DeletePhotoAsync(userPosts.Image);
            }


            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var Detail = await _postsRepository.GetByIdAsync(id);
            if (Detail == null) return View("Error");
            
            return PartialView("_Delete", Detail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePosts(int id)
        {
            var PostsDetails = await _postsRepository.GetByIdAsync(id);

            if (PostsDetails == null)
            {
                return View("Error");
            }

            _postsRepository.Delete(PostsDetails);
            return RedirectToAction(nameof(Index));

        }




        public async Task<IActionResult> Detail(int id)
        {
            Posts Posts = await _postsRepository.GetByIdAsync(id);
            return Posts == null ? NotFound() : View(Posts);

        }
    }
}

