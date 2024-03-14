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
		public PostController(IPostServices postServices)
		{
			_postServices = postServices;
		}

			
		[HttpGet()]
		public async Task<ActionResult> Post_Getlist([FromQuery] string? post_id, [FromQuery] string? category_id)
		{

			var list = new List<Post>();
			if (post_id == null && category_id == null)
			{
				list = await _postServices.GetPosts(null, null);
			}
			else if (post_id != null && category_id == null)
			{
				list = await _postServices.GetPosts(Int32.Parse(post_id), null);
			}
			else if (post_id == null  || category_id != null)
			{
				list = await _postServices.GetPosts(null, Int32.Parse(category_id));
			}
			return Ok(list);
		}

		[HttpPost]
		public async Task<ActionResult> Post_Create([FromBody] CreatePost post)
		{
			try
			{
				var returnData = new ReturnData();
				if (post == null)
				{
					return BadRequest();
				}

				returnData = await _postServices.CreatePost(post);

				return StatusCode(201);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		[HttpPut("{post_id:int}")]
		public async Task<ActionResult> Post_Update(int post_id, [FromBody] CreatePost post)
		{
			var returnData = new ReturnData();
			returnData = await _postServices.UpdatePost(post_id, post);
			return Ok(returnData);
		}


		[HttpDelete("{post_id:int}")]
		public async Task<ActionResult> Post_Delete(int post_id)
		{
			var returnData = new ReturnData();
			returnData = await _postServices.DeletePost(post_id);
			return Ok(returnData);
		}
	}
}
