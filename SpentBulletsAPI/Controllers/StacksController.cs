﻿using System;
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
    public class StacksController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StacksController(IConfiguration config)
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
                    cmd.CommandText = @"SELECT s.id, s.amount, s.grain, s.notes, s.userId, u.username, u.email, s.caliberId, c.caliber, s.brandId, b.brand 
                                            FROM stacks s
                                            JOIN user u ON s.userId = u.id
                                            JOIN caliber c ON s.caliberId = c.id
                                            JOIN brand b ON s.brandId = b.id"; //sql string goes here
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Stack> stacks = new List<Stack>();

                    while (reader.Read())
                    {
                        Stack stack = new Stack
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            amount = reader.GetInt32(reader.GetOrdinal("amount")),
                            grain = reader.GetInt32(reader.GetOrdinal("grain")),
                            notes = reader.GetString(reader.GetOrdinal("notes")),
                            UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                            user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("userId")),
                                username = reader.GetString(reader.GetOrdinal("username")),
                                email = reader.GetString(reader.GetOrdinal("email"))
                            },
                            CaliberId = reader.GetInt32(reader.GetOrdinal("caliberId")),
                            caliber = new Caliber
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("caliberId")),
                                caliber = reader.GetString(reader.GetOrdinal("caliber"))
                            },
                            BrandId = reader.GetInt32(reader.GetOrdinal("brandId")),
                            brand = new Brand
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("brandId")),
                                brand = reader.GetString(reader.GetOrdinal("brand"))
                            }
                        };

                        stacks.Add(stack);
                    }
                    reader.Close();

                    return Ok(stacks);
                }
            }
        }

        //GET: get one
        [HttpGet("{id}", Name = "GetStack")]
        public async Task<IActionResult> Get([FromRoute] int id) // RESEARCH NOTE -- does FromRoute still work when crossing from React to C#
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.id, s.amount, s.grain, s.notes, s.userId, u.username, u.email, s.caliberId, c.caliber, s.brandId, b.brand 
                                            FROM stacks s
                                            JOIN user u ON s.userId = u.id
                                            JOIN caliber c ON s.caliberId = c.id
                                            JOIN brand b ON s.brandId = b.id
                                            WHERE s.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Stack stack = null;

                    while (reader.Read())
                    {
                        if (stack == null)
                        {
                            stack = new Stack
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                amount = reader.GetInt32(reader.GetOrdinal("amount")),
                                grain = reader.GetInt32(reader.GetOrdinal("grain")),
                                notes = reader.GetString(reader.GetOrdinal("notes")),
                                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                                user = new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("userId")),
                                    username = reader.GetString(reader.GetOrdinal("username")),
                                    email = reader.GetString(reader.GetOrdinal("email"))
                                },
                                CaliberId = reader.GetInt32(reader.GetOrdinal("caliberId")),
                                caliber = new Caliber
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("caliberId")),
                                    caliber = reader.GetString(reader.GetOrdinal("caliber"))
                                },
                                BrandId = reader.GetInt32(reader.GetOrdinal("brandId")),
                                brand = new Brand
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("brandId")),
                                    brand = reader.GetString(reader.GetOrdinal("brand"))
                                }
                            };
                        }
                    }
                    reader.Close();

                    return Ok(stack);
                }
            }
        }

        //POST: create new
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Stack stack)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO stacks (userId, caliberId, brandId, amount, grain, notes)
                                            OUTPUT Inserted.id
                                            VALUES (@UserId, @CaliberId, @BrandId, @Amount, @Grain, @Notes)";
                    cmd.Parameters.Add(new SqlParameter("@UserId", stack.UserId));
                    cmd.Parameters.Add(new SqlParameter("@CaliberId", stack.CaliberId));
                    cmd.Parameters.Add(new SqlParameter("@BrandId", stack.BrandId));
                    cmd.Parameters.Add(new SqlParameter("@Amount", stack.amount));
                    cmd.Parameters.Add(new SqlParameter("@Grain", stack.grain));
                    cmd.Parameters.Add(new SqlParameter("@Notes", stack.notes));

                    int newId = (int)cmd.ExecuteScalar();
                    stack.Id = newId;
                    return CreatedAtRoute("GetUser", new { id = newId }, stack);
                }
            }
        }

        //PUT: update
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Stack stack)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE stacks SET userId = @UserId, caliberId = @CaliberId, brandId = @BrandId, amount = @Amount, grain = @Grain, notes = @Notes WHERE id = @id";
                        cmd.Parameters.Add(new SqlParameter("@UserId", stack.UserId));
                        cmd.Parameters.Add(new SqlParameter("@CaliberId", stack.CaliberId));
                        cmd.Parameters.Add(new SqlParameter("@BrandId", stack.BrandId));
                        cmd.Parameters.Add(new SqlParameter("@Amount", stack.amount));
                        cmd.Parameters.Add(new SqlParameter("@Grain", stack.grain));
                        cmd.Parameters.Add(new SqlParameter("@Notes", stack.notes));

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
                if (!StackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE: delete one stack
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM stacks WHERE id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

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
                if (!StackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool StackExists(int id)
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