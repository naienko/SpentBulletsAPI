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
                    cmd.CommandText = "SELECT id, username, password, email, display_name, role FROM users"; //sql string goes here
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<User> users = new List<User>();

                    while (reader.Read())
                    {
                        User user = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
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
                    cmd.CommandText = @"SELECT id, username, password, email, display_name, role 
                                            FROM users
                                            WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    User user = null;

                    while (reader.Read())
                    {
                        if (user == null)
                        {
                            user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
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

        //GET: get single user with all stacks?

        //POST: create new
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO users (username, password, email, display_name, role)
                                            OUTPUT Inserted.id
                                            VALUES (@Username, @Password, @Email, @DisplayName, @Role)";
                    cmd.Parameters.Add(new SqlParameter("@Username", user.username));
                    cmd.Parameters.Add(new SqlParameter("@Password", user.password));
                    cmd.Parameters.Add(new SqlParameter("@Email", user.email));
                    cmd.Parameters.Add(new SqlParameter("@DisplayName", user.display_name));
                    cmd.Parameters.Add(new SqlParameter("@Role", user.role));

                    int newId = (int)cmd.ExecuteScalar();
                    user.Id = newId;
                    return CreatedAtRoute("GetUser", new { id = newId }, user);
                }
            }
        }

        //PUT: update
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] User user)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE users SET username = @Username,
                                                password = @Password, email = @Email, display_name = @DisplayName,
                                                role = @Role WHERE id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Username", user.username));
                        cmd.Parameters.Add(new SqlParameter("@Password", user.password));
                        cmd.Parameters.Add(new SqlParameter("@Email", user.email));
                        cmd.Parameters.Add(new SqlParameter("@DisplayName", user.display_name));
                        cmd.Parameters.Add(new SqlParameter("@Role", user.role));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE: should I let a user delete their account?

        private bool UserExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}