using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using questionaire_back.models;
using System.Data;

namespace questionaire_back.packages
{
    public class quest_package:main_package
    {
        private readonly string _connectionString;

        public quest_package(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }


        public bool RegisterAdmin(RegisterModel model)
        {
            using (OracleConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("pkg_add_questions_admin.proc_register_admin", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        string hashedPassword = HashPassword(model.Password);
                        
                        command.Parameters.Add("p_name", OracleDbType.Varchar2).Value = model.Name;
                        command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = model.UserName;
                        command.Parameters.Add("p_password", OracleDbType.Varchar2).Value = hashedPassword;
                        command.ExecuteNonQuery();

                        Console.WriteLine("admin created successfully.");
                        return true;
                    }
                }
                catch (OracleException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public TokenModel LoginUser(LoginModel model)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    using (var command = new OracleCommand("pkg_add_questions_admin.proc_login_admin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = model.UserName;
                        command.Parameters.Add("p_user_curs", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var hashedpassword = reader["password"].ToString();
                                Console.WriteLine(model.Password);

                                if (ValidatePassword(model.Password, hashedpassword))
                                {
                                    return RetrieveUserInfo(connection, model.UserName);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw;
            }
        }
        private TokenModel RetrieveUserInfo(OracleConnection connection, string username)
        {
            using (var command = new OracleCommand("pkg_add_questions_admin.proc_get_user_info", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username;
                command.Parameters.Add("p_user_curs", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TokenModel
                        {
                            UserName = reader["username"].ToString(),
                            Name = reader["name"].ToString()
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public bool AddQuestions(QuestionModel model)
        {
            using (OracleConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("pkg_add_questions_admin.proc_add_questions", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("p_question", OracleDbType.Varchar2).Value = model.Question;
                        command.Parameters.Add("p_answer", OracleDbType.Varchar2).Value = model.Answer;
                        command.Parameters.Add("p_required", OracleDbType.Int32).Value = model.Required;
                        command.ExecuteNonQuery();

                        Console.WriteLine("question added successfully.");
                        return true;
                    }
                }
                catch (OracleException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool AddAnswers(List<AnswerModel> models)
        {
            using (OracleConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    string json = JsonConvert.SerializeObject(models);
                    using (OracleCommand command = new OracleCommand("pkg_add_questions_admin.proc_add_answers", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        Console.WriteLine("JSON Payload: " + json);
                        command.Parameters.Add("p_json", OracleDbType.Clob).Value = json;

                        command.ExecuteNonQuery();

                        Console.WriteLine("answer added successfully.");
                        return true;
                    }
                }
                catch (OracleException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<GetAnsModel> GetAnswers(int id)
        {
            List<GetAnsModel> answers = new List<GetAnsModel>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_add_questions_admin.proc_get_answers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

                    OracleParameter cursorParameter = new OracleParameter
                    {
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cursorParameter);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var answer = new GetAnsModel
                            {
                                FullName = reader["fullname"].ToString(),
                                Question= reader["question"].ToString(),
                                Answer = reader["answer"].ToString(),
                            };
                            answers.Add(answer);
                        }
                    }
                }
            }
            return answers;
        }

        public List<GetQuestionModel> GetQuestions()
        {
            List<GetQuestionModel> questions = new List<GetQuestionModel>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_add_questions_admin.proc_get_questions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    OracleParameter cursorParameter = new OracleParameter
                    {
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cursorParameter);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var question = new GetQuestionModel
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Question = reader["question"].ToString(),
                                Answer= reader["answer"].ToString(),
                                Required= Convert.ToInt32(reader["required"])
                            };
                            questions.Add(question);
                        }
                    }
                }
            }
            return questions;
        }

        public GetQuestionModel GetQuestion(int id)
        {
            GetQuestionModel question =null;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_add_questions_admin.proc_get_question", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

                    OracleParameter cursorParameter = new OracleParameter
                    {
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cursorParameter);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            question = new GetQuestionModel
                            {
                                Question = reader["question"].ToString(),
                                Id = Convert.ToInt32(reader["id"]),
                                Answer = reader["answer"].ToString(),
                                Required = Convert.ToInt32(reader["required"])
                            };
                        }
                    }
                }
            }
            return question;
        }

        public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_add_questions_admin.proc_get_users", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    OracleParameter cursorParameter = new OracleParameter
                    {
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cursorParameter);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new UserModel
                            {
                                FullName = reader["fullname"].ToString(),
                                Id = Convert.ToInt32(reader["id"])
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        public bool UpdateQuestion(QuestionModel model, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_add_questions_admin.proc_update_question", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                    command.Parameters.Add("p_question", OracleDbType.Varchar2).Value = model.Question;
                    command.Parameters.Add("p_answer", OracleDbType.Varchar2).Value = model.Answer;
                    command.Parameters.Add("p_required", OracleDbType.Int32).Value = model.Required;
                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("question updated successfully.");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating user: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}
