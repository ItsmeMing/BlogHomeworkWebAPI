using DataAccess.Blog.Entities;
using DataAccess.Blog.IServices;
using DataAccess.Blog.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogHomeworkWebAPI.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IBlogUnitOfWork _blogUnitOfWork;
        public OrderController(IBlogUnitOfWork blogUnitOfWork)
        {
            _blogUnitOfWork = blogUnitOfWork;
        }

			
        [HttpPost()]
        public async Task<ReturnData> CreateOrder([FromBody] CreateOrder order)
        {

            var returnData = new ReturnData();
            returnData = await _blogUnitOfWork._orderRepository.CreateOrder(order);
            _blogUnitOfWork.SaveChanges();
            return returnData;
        }
        
    }
}