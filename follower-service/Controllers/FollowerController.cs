using follower_service.Dtos;
using follower_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace follower_service.Controllers
{
    [ApiController]
    [Route("")]
    public class FollowerController : Controller
    {
        private readonly FollowerService followerService;

        public FollowerController(FollowerService followerService)
        {
            this.followerService = followerService;
        }

        [HttpGet("{id}/followers")]
        [Authorize]
        public IEnumerable<UserDTO> GetFollowers(string id)
        {
            return followerService.GetFollowersById(id).Select(x => new UserDTO
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
            });
        }

        [HttpGet("{id}/followers/{followerId}")]
        [Authorize]
        public FollowerDTO GetFollower(string id, string followerId)
        {
            var follower = followerService.GetById(followerId, id);

            return new FollowerDTO
            {
                FollowedUser = new UserDTO
                {
                    Id = follower.FollowedUserId,
                    DisplayName = follower.FollowedUser.DisplayName
                },
                FollowingUser = new UserDTO
                {
                    Id = follower.FollowingUserId,
                    DisplayName = follower.FollowingUser.DisplayName
                }
            };
        }


        [HttpDelete("{id}/followers")]
        [Authorize]
        public void RemoveFollower(string id)
        {
            var followingId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            followerService.removeFollower(followingId, id);
        }

        [HttpPost("{id}/followers")]
        [Authorize]
        public void AddFollower(string id)
        {
            var followingId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            followerService.AddFollower(followingId, id);
        }

    }
}
