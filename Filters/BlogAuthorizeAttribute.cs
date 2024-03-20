using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace BlogHomeworkWebAPI.Filters
{
    public class BlogAuthorizeAttribute : TypeFilterAttribute
    {
        public BlogAuthorizeAttribute() : base(typeof(BlogAuthorizeActionFilter))
        {
        }
    }

    public class BlogAuthorizeActionFilter : IAsyncAuthorizationFilter
    {
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