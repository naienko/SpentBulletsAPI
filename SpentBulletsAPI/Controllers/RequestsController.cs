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
    public class RequestsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public RequestsController(IConfiguration config)
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
        public async Task<IActionResult> Get(int? userId)
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    string dataQuery = @"SELECT r.id, r.name, r.typeId, r.userId, u.username, u.email, r.about  
                                            FROM requests r
                                            JOIN users u ON r.userId = u.id";

                    cmd.CommandText = dataQuery;

                    MySqlDataReader reader = cmd.ExecuteReader();
                    List<Request> requests = new List<Request>();

                    while (reader.Read())
                    {
                        Request request = new Request
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            name = reader.GetString(reader.GetOrdinal("name")),
                            TypeId = reader.GetInt32(reader.GetOrdinal("typeId")),
                            UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                            user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("userId")),
                                username = reader.GetString(reader.GetOrdinal("username")),
                                email = reader.GetString(reader.GetOrdinal("email"))
                            },
                            about = reader.GetString(reader.GetOrdinal("about"))
                        };

                        requests.Add(request);
                    }
                    reader.Close();

                    return Ok(requests);
                }
            }
        }

        //GET: get one
        [HttpGet("{id}", Name = "GetRequest")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    string dataQuery = @"SELECT r.id, r.name, r.typeId, r.userId, u.username, u.email, r.about  
                                            FROM requests r
                                            JOIN users u ON r.userId = u.id
                                            WHERE r.id = @id";

                    cmd.CommandText = dataQuery;

                    MySqlDataReader reader = cmd.ExecuteReader();

                    Request request = null;

                    while (reader.Read())
                    {
                        if (request == null)
                        {
                            request = new Request
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                name = reader.GetString(reader.GetOrdinal("name")),
                                TypeId = reader.GetInt32(reader.GetOrdinal("typeId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                                user = new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("userId")),
                                    username = reader.GetString(reader.GetOrdinal("username")),
                                    email = reader.GetString(reader.GetOrdinal("email"))
                                },
                                about = reader.GetString(reader.GetOrdinal("about"))
                            };
                        }
                    }
                    reader.Close();

                    return Ok(request);
                }
            }
        }

        //POST: create new
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Request request)
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO requests (name, typeId, userId, about)
                                            VALUES (@Name, @TypeId, @UserId, @About)";
                    cmd.Parameters.Add(new MySqlParameter("@Name", request.name));
                    cmd.Parameters.Add(new MySqlParameter("@TypeId", request.TypeId));
                    cmd.Parameters.Add(new MySqlParameter("@UserId", request.UserId));
                    cmd.Parameters.Add(new MySqlParameter("@About", request.about));

                    cmd.ExecuteNonQuery();
                    int newId = (int)cmd.LastInsertedId;
                    request.Id = newId;
                    return CreatedAtRoute("GetRequest", new { id = newId }, request);
                }
            }
        }

        //PUT: update -- NO UPDATE
        
        //DELETE: delete one request
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (MySqlConnection conn = Connection)
                {
                    conn.Open();
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM requests WHERE id = @id";
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
                if (!RequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool RequestExists(int id)
        {
            using (MySqlConnection conn = Connection)
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, name, typeId, userId, about 
                                        FROM requests WHERE id = @id";
                    cmd.Parameters.Add(new MySqlParameter("@id", id));

                    MySqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}