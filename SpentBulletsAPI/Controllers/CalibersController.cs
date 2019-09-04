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
    public class CalibersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CalibersController(IConfiguration config)
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
                    string dataQuery = "SELECT id, caliber FROM calibers";
                    string sortDataLimiter = " ORDER BY caliber";

                    if (orderBy == "caliber")
                    {
                        cmd.CommandText = dataQuery + sortDataLimiter;
                    }
                    else
                    {
                        cmd.CommandText = dataQuery;
                    }

                    MySqlDataReader reader = cmd.ExecuteReader();
                    List<Caliber> calibers = new List<Caliber>();

                    while (reader.Read())
                    {
                        Caliber caliber = new Caliber
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            caliber = reader.GetString(reader.GetOrdinal("caliber"))
                        };

                        calibers.Add(caliber);
                    }
                    reader.Close();

                    return Ok(calibers);
                }
            }
        }

        //GET: get one
        [HttpGet("{id}", Name = "GetCaliber")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, caliber FROM caliber
                                                WHERE id = @id";
                    cmd.Parameters.Add(new MySqlParameter("@id", id));

                    MySqlDataReader reader = cmd.ExecuteReader();

                    Caliber caliber = null;

                    while (reader.Read())
                    {
                        if (caliber == null)
                        {
                            caliber = new Caliber
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                caliber = reader.GetString(reader.GetOrdinal("caliber"))
                            };
                        }
                    }
                    reader.Close();

                    return Ok(caliber);
                }
            }
        }

        [HttpPost]
        //POST
        public async Task<IActionResult> Post([FromBody] Caliber caliber) //RESEARCH NOTE -- not sure about FromBody; how does the data get passed from React's fetch calls to this server-side app
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO calibers (caliber) 
                                            VALUES (@caliber)";
                    cmd.Parameters.Add(new MySqlParameter("@Caliber", caliber.caliber));

                    cmd.ExecuteNonQuery();
                    int newId = (int)cmd.LastInsertedId;
                    caliber.Id = newId;
                    return CreatedAtRoute("GetCaliber", new { id = newId }, caliber);
                }
            }
        }

        //PUT: no put, not changing any of the data in here

        //DELETE: no delete, will screw up the stacks table
    }
}