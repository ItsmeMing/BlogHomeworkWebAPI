using DataAccess.Blog.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace BlogHomeworkWebAPI.Filters
{
	public class BlogAuthorizeAttribute : TypeFilterAttribute
	{
		private readonly string _functionCode;
		private readonly string _permission;

		public BlogAuthorizeAttribute(string functionCode, string permission) : base(typeof(BlogAuthorizeActionFilter))
		{
			Arguments = new object[] { functionCode, permission };
		}
	}

	public class BlogAuthorizeActionFilter : IAsyncAuthorizationFilter
	{
		private readonly string _functionCode;
		private readonly string _permission;
		private readonly IBlogUnitOfWork _blogUnitOfWork;

		public BlogAuthorizeActionFilter(string functionCode, string permission, IBlogUnitOfWork blogUnitOfWork)
		{
			_functionCode = functionCode;
			_permission = permission;
			_blogUnitOfWork = blogUnitOfWork;

		}

		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			var identity = context.HttpContext.User.Identity as ClaimsIdentity;
			if (identity != null)
			{
				var userClaims = identity.Claims;
				if (userClaims.Count() > 0)
				{
					var userId = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

					if (userId == null || userId == "")
					{
						context.HttpContext.Response.ContentType = "application/json";
						context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
						context.Result = new JsonResult(new
						{
							Code = HttpStatusCode.Unauthorized,
							Message = "Vui lòng đăng nhập để thực hiện chức năng này "
						});

						return;
					}

					var function = await _blogUnitOfWork._userRepository.GetFunctionByCode(_functionCode);
					if (function == null || function.FunctionId <= 0)
					{
						context.HttpContext.Response.ContentType = "application/json";
						context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
						context.Result = new JsonResult(new
						{
							Code = HttpStatusCode.Unauthorized,
							Message = "Function ko tồn tại"
						});

						return;
					}

					var userFunction = await _blogUnitOfWork._userRepository.UserFunction_GetRole(Int32.Parse(userId), function.FunctionId);
					if (userFunction == null || userFunction.UserId <= 0)
					{
						context.HttpContext.Response.ContentType = "application/json";
						context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
						context.Result = new JsonResult(new
						{
							Code = HttpStatusCode.Unauthorized,
							Message = "UserFunction ko tồn tại"
						});

						return;
					}

					switch (_permission)
					{
						case "Get":
							if (userFunction.IsView == 0)
							{
								context.HttpContext.Response.ContentType = "application/json";
								context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
								context.Result = new JsonResult(new
								{
									Code = HttpStatusCode.Unauthorized,
									Message = "User ko có quyền view"
								});
								return;
							}
							break;
						case "Delete":
							if (userFunction.IsDelete == 0)
							{
								context.HttpContext.Response.ContentType = "application/json";
								context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
								context.Result = new JsonResult(new
								{
									Code = HttpStatusCode.Unauthorized,
									Message = "User ko có quyền xóa"
								});
								return;
							}
							break;
						default:
							break;
					}
				}
				else
				{
					context.HttpContext.Response.ContentType = "application/json";
					context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					context.Result = new JsonResult(new
					{
						Code = HttpStatusCode.Unauthorized,
						Message = "Vui lòng đăng nhập để thực hiện chức năng này "
					});

					return;
				}
			}
		}
	}
}