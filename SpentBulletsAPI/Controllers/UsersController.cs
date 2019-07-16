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
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsersController(IConfiguration config)
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

        //GET: get all
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, username, password, email, display_name, role FROM users"; //MySql string goes here
                    MySqlDataReader reader = cmd.ExecuteReader();
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
        public async Task<IActionResult> Get([FromRoute] int id, string username, string password) // RESEARCH NOTE -- does FromRoute still work when crossing from React to C#
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    if (username != null)
                    {
                        cmd.CommandText = @"SELECT id, username, password, email, display_name, role 
                                                FROM users
                                                WHERE username = @username";
                        cmd.Parameters.Add(new MySqlParameter("@username", username));
                    } else if (username != null && password != null)
                    {
                        cmd.CommandText = @"SELECT id, username, password, email, display_name, role 
                                                FROM users
                                                WHERE username = @username AND password = @password";
                        cmd.Parameters.Add(new MySqlParameter("@username", username));
                        cmd.Parameters.Add(new MySqlParameter("@password", password));
                    } else
                    {
                        cmd.CommandText = @"SELECT id, username, password, email, display_name, role 
                                                FROM users
                                                WHERE id = @id";
                        cmd.Parameters.Add(new MySqlParameter("@id", id));
                    }

                    MySqlDataReader reader = cmd.ExecuteReader();

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
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO users (username, password, email, display_name, role)
                                            VALUES (@Username, @Password, @Email, @DisplayName, @Role)";
                                            //OUTPUT INSERTED.id 
                    cmd.Parameters.Add(new MySqlParameter("@Username", user.username));
                    cmd.Parameters.Add(new MySqlParameter("@Password", user.password));
                    cmd.Parameters.Add(new MySqlParameter("@Email", user.email));
                    cmd.Parameters.Add(new MySqlParameter("@DisplayName", user.display_name));
                    cmd.Parameters.Add(new MySqlParameter("@Role", user.role));

                    cmd.ExecuteNonQuery();
                    int newId = (int)cmd.LastInsertedId;
                    //int newId = (int)cmd.ExecuteScalar();
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
                using (MySqlConnection conn = Connection)
                {
                    conn.Open();
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE users SET username = @Username,
                                                password = @Password, email = @Email, display_name = @DisplayName,
                                                role = @Role WHERE id = @id";
                        cmd.Parameters.Add(new MySqlParameter("@Username", user.username));
                        cmd.Parameters.Add(new MySqlParameter("@Password", user.password));
                        cmd.Parameters.Add(new MySqlParameter("@Email", user.email));
                        cmd.Parameters.Add(new MySqlParameter("@DisplayName", user.display_name));
                        cmd.Parameters.Add(new MySqlParameter("@Role", user.role));
                        cmd.Parameters.Add(new MySqlParameter("@id", id));

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
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT username, password, email, role, display_name FROM users WHERE id = @id";
                    cmd.Parameters.Add(new MySqlParameter("@id", id));

                    MySqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}