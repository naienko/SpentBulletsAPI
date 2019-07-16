using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
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

        public MySqlConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        // GET: get all
        public async Task<IActionResult> Get(string orderBy)
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    string dataQuery = "SELECT id, brand FROM brands";
                    string sortDataLimiter = " ORDER BY brand";

                    if (orderBy == "brand")
                    {
                        cmd.CommandText = dataQuery + sortDataLimiter;
                    }
                    else
                    {
                        cmd.CommandText = dataQuery;
                    }

                    MySqlDataReader reader = cmd.ExecuteReader();
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
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO brands (brand) 
                                            VALUES (@Brand)";
                    cmd.Parameters.Add(new MySqlParameter("@Brand", brand.brand));

                    cmd.ExecuteNonQuery();
                    int newId = (int)cmd.LastInsertedId;
                    brand.Id = newId;
                    return CreatedAtRoute("GetBrand", new { id = newId }, brand); //RESEARCH NOTE -- not sure about CreatedAtRoute since not doing a single id get
                }
            }
        }

        //PUT: no put, not changing any of the data in here

        //DELETE: no delete, will screw up the stacks table
    }
}