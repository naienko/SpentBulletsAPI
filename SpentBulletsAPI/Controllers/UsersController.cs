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
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsersController(IConfiguration config)
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

        //GET: get all
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = ""; //sql string goes here
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<User> users = new List<User>();

                    while (reader.Read())
                    {
                        User user = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            username = reader.GetString(reader.GetOrdinal("username")),
                            password = reader.GetString(reader.GetOrdinal("password")),
                            email = reader.GetString(reader.GetOrdinal("email")),
                            display_name = reader.GetString(reader.GetOrdinal("display_name")),
                            role = reader.GetString(reader.GetOrdinal("role"))
                        };

                        users.Add(user);
                    }
                    reader.Close();

                    return Ok(users);
                }
            }
        }

        //GET: get one
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> Get([FromRoute] int id) // RESEARCH NOTE -- does FromRoute still work when crossing from React to C#
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    User user = null;

                    while (reader.Read())
                    {
                        if (user == null)
                        {
                            user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                username = reader.GetString(reader.GetOrdinal("username")),
                                password = reader.GetString(reader.GetOrdinal("password")),
                                email = reader.GetString(reader.GetOrdinal("email")),
                                display_name = reader.GetString(reader.GetOrdinal("display_name")),
                                role = reader.GetString(reader.GetOrdinal("role"))
                            };
                        }
                    }
                    reader.Close();

                    return Ok(user);
                }
            }
        }

        //POST: create new



        //PUT: update
        //DELETE: should I let a user delete their account?
    }
}