using DataAccess.Blog.Entities;
using DataAccess.Blog.IServices;
using DataAccess.Blog.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogHomeworkWebAPI.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IBlogUnitOfWork _blogUnitOfWork;
        public ProductController(IBlogUnitOfWork blogUnitOfWork)
        {
            _blogUnitOfWork = blogUnitOfWork;
        }

			
        [HttpGet()]
        public async Task<ActionResult> Product_Getlist([FromQuery] string? product_id)
        {

            var list = new List<Product>();
            list = await _blogUnitOfWork._productRepository.GetProducts(product_id);
            return Ok(list);
        }
    }
}