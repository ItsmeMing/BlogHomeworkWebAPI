using DataAccess.Blog.Entities;
using DataAccess.Blog.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogHomeworkWebAPI.Controllers
{
	[Route("posts")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private IPostServices _postServices;
		public PostController(IPostServices postServices) // TIÊM 
		{
			_postServices = postServices;
		}

			
		[HttpGet()]
		public async Task<ActionResult> Post_Getlist([FromQuery] string? post_id)
		{

			var list = new List<Post>();
			if (post_id == null)
			{
				list = await _postServices.GetPosts(null);
			}
			else
			{
				list = await _postServices.GetPosts(Int32.Parse(post_id)); 
			}
			return Ok(list);
		}
	}
}
