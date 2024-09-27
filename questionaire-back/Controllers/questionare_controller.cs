using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using questionaire_back.models;
using questionaire_back.packages;
using System.Security.Claims;

namespace questionaire_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class questionare_controller :MainController
    {
        private readonly quest_package _questpack;

        public questionare_controller(quest_package quest_Package, IConfiguration configuration):base(configuration) 
        {
            _questpack = quest_Package;
        }


        [HttpPost("CreateAdmin")]

        public IActionResult CreateUser([FromBody] RegisterModel model)
        {
            try
            {
                var newcompany = _questpack.RegisterAdmin(model);
                if (newcompany)
                {
                    return Ok(new { message = "admin created successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to create admin.");
                }
            }
            catch (OracleException ex)
            {
                if (ex.Number == 20001)
                {
                    return StatusCode(20001, $"Oracle error occurred: {ex.Message}");
                }
                else
                {
                    return StatusCode(500, $"Oracle error occurred: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpPost("LoginAdmin")]
        public IActionResult LoginUser([FromBody] LoginModel model)
        {
            try
            {
                var user = _questpack.LoginUser(model);
                Console.WriteLine(user);
                if (user?.Name!= null)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("JWTID", Guid.NewGuid().ToString()),
                    };
                    var token = GenerateNewJsonWebToken(authClaims);
                    return Ok(new { message = "login was successfull", token });
                }
                else
                {
                    return StatusCode(500, "Failed to login.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpGet("GetAnswers/{id}")]
        public IActionResult GetAnswers(int id)
        {
            try
            {
                var answer = _questpack.GetAnswers(id);
                if (answer != null)
                {
                    return Ok(answer);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }
        [HttpGet("GetQuestions")]
        public IActionResult GetQuestions()
        {
            try
            {
                var quest = _questpack.GetQuestions();
                if (quest != null)
                {
                    return Ok(quest);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }
        [HttpGet("GetQuestion/{id}")]
        public IActionResult GetQuestion(int id)
        {
            try
            {
                var quest = _questpack.GetQuestion(id);
                if (quest != null)
                {
                    return Ok(quest);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            try
            {
                var user = _questpack.GetUsers();
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }
        [HttpPut("UpdateQuestions/{id}")]
        public IActionResult UpdateQuestions([FromBody] QuestionModel model,int id)
        {
            try
            {
                var quest = _questpack.UpdateQuestion(model,id);
                if (quest)
                {
                    return Ok(quest);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpPost("AddQuestion")]

        public IActionResult AddQuestions([FromBody] QuestionModel model)
        {
            try
            {
                var question = _questpack.AddQuestions(model);
                if (question)
                {
                    return Ok(new { message = "question added successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to add question.");
                }
            }
            catch (OracleException ex)
            {
                if (ex.Number == 20001)
                {
                    return StatusCode(20001, $"Oracle error occurred: {ex.Message}");
                }
                else
                {
                    return StatusCode(500, $"Oracle error occurred: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating question: {ex.Message}");
            }
        }
        [HttpPost("AddAnswers")]

        public IActionResult AddAnswers([FromBody] List<AnswerModel> models)
        {
            try
            {
                var answers = _questpack.AddAnswers(models);
                if (answers)
                {
                    return Ok(new { message = "answers added successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to add answers.");
                }
            }
            catch (OracleException ex)
            {
                if (ex.Number == 20001)
                {
                    return StatusCode(20001, $"Oracle error occurred: {ex.Message}");
                }
                else
                {
                    return StatusCode(500, $"Oracle error occurred: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating question: {ex.Message}");
            }
        }
    }
}
