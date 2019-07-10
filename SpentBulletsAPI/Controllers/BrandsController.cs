using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using SpentBulletsAPI.Models;

namespace SpentBulletsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public BrandsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        // GET: get all
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, brand FROM brands"; //sql string goes here
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Brand> brands = new List<Brand>();

                    while (reader.Read())
                    {
                        Brand brand = new Brand
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            brand = reader.GetString(reader.GetOrdinal("brand"))
                        };

                        brands.Add(brand);
                    }
                    reader.Close();

                    return Ok(brands);
                }
            }
        }

        [HttpPost]
        //POST
        public async Task<IActionResult> Post([FromBody] Brand brand) //RESEARCH NOTE -- not sure about FromBody; how does the data get passed from React's fetch calls to this server-side app
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO brands (brand) 
                                            OUTPUT Inserted.id
                                            VALUES (@Brand)";
                    cmd.Parameters.Add(new SqlParameter("@Brand", brand.brand));

                    int newId = (int)cmd.ExecuteScalar();
                    brand.Id = newId;
                    return CreatedAtRoute("GetBrand", new { id = newId }, brand); //RESEARCH NOTE -- not sure about CreatedAtRoute since not doing a single id get
                }
            }
        }

        //PUT: no put, not changing any of the data in here

        //DELETE: no delete, will screw up the stacks table
    }
}