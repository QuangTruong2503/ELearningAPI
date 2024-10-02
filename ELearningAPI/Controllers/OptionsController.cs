﻿using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public OptionsController(ELearningDbContext context)
        {
            _context = context;
        }
        // GET: api/<OptionsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OptionsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OptionsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<OptionsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OptionsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}