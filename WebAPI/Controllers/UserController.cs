﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPI.Model;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly MyDbContext context;

        public UserController(MyDbContext context,IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //get all data empapi table
            var ep = context.empapi.ToList();
            JsonResult r = new JsonResult(ep);
            //var jsondata = JsonConvert.SerializeObject(r);
            return Ok(ep);
        }
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //Get by according to id 
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(Emp ep)
        {
            //data insert in empapi table
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

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Sign(login ep)
        {
            //login user then  name find empapi table 
            var emp = await context.empapi.FirstOrDefaultAsync(e => e.name == ep.Name); 
            if (emp == null)
            {
                return NotFound();
            }
            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub,configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                 new Claim("UserId",ep.Name),
                  new Claim("Password",ep.Password),
            };
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audiance"],
                claims,               
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signIn
                );
            string tokenvalue=new JwtSecurityTokenHandler().WriteToken(Token);
            return Ok(new { token = tokenvalue});
            //return Ok(emp);
        }
     
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> Put(int id, Emp ep)
        {
            // Update the properties of the existing employee
            var emp = await context.empapi.FindAsync(id);

            if (emp == null)
            {
                return NotFound();
            }
            
            emp.Id = emp.Id;
            emp.name = ep.name;
            emp.lastname = ep.lastname;
            emp.departement = ep.departement;
            context.empapi.Update(emp);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //delete by according to id
            try
            {
                var emps = new Emp();
                var emp = await context.empapi.FirstOrDefaultAsync(e => e.Id == id);
                if (emp == null)
                {
                    return NotFound();
                }
                emps.name=emp.name;
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
