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
    public class CalibersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CalibersController(IConfiguration config)
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
                    cmd.CommandText = ""; //sql string goes here
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Caliber> calibers = new List<Caliber>();

                    while (reader.Read())
                    {
                        Caliber caliber = new Caliber
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            caliber = reader.GetString(reader.GetOrdinal("caliber"))
                        };

                        calibers.Add(caliber);
                    }
                    reader.Close();

                    return Ok(calibers);
                }
            }
        }

        [HttpPost]
        //POST
        public async Task<IActionResult> Post([FromBody] Caliber caliber) //RESEARCH NOTE -- not sure about FromBody; how does the data get passed from React's fetch calls to this server-side app
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "";
                    cmd.Parameters.Add(new SqlParameter("@Caliber", caliber.caliber));

                    int newId = (int)cmd.ExecuteScalar();
                    caliber.Id = newId;
                    return CreatedAtRoute("GetCaliber", new { id = newId }, caliber); //RESEARCH NOTE -- not sure about CreatedAtRoute since not doing a single id get
                }
            }
        }

        //PUT: no put, not changing any of the data in here

        //DELETE: no delete, will screw up the stacks table
    }
}