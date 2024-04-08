using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPI.Model;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly MyDbContext context;

        public UserController(MyDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ep = context.empapi.ToList();
            JsonResult r = new JsonResult(ep);
            //var jsondata = JsonConvert.SerializeObject(r);
            return Ok(ep);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var emp = await context.empapi.FirstOrDefaultAsync(e => e.Id == id);
                if (emp == null)
                {
                    return NotFound();
                }
                return Ok(emp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(Emp ep)
        {
            var emp = new Emp()
            {
                Id = ep.Id,
                name = ep.name,
                lastname = ep.lastname,
                departement = ep.departement
            };
            context.empapi.Add(emp);
            context.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var emp = await context.empapi.FirstOrDefaultAsync(e => e.Id == id);
                if (emp == null)
                {
                    return NotFound();
                }
                context.empapi.Remove(emp);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}
