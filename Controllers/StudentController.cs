using Quiz_management_system.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace Quiz_management_system.Controllers
{
    public class StudentController : Controller
    {


        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
         
        
        void connectionString()
        {
            con.ConnectionString = @"Data Source=DESKTOP-12BDF78\SQLEXPRESS;Initial Catalog=databasequiz;Integrated Security=True";
        }
        public IActionResult s_dashboard()
        {
            connectionString();
            DataTable dt = new DataTable();
            category_chose ch = new category_chose();
            string register_id = HttpContext.Session.GetString("register_id");

            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string query = "select * FROM user_result where register_id = '" + register_id + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.Fill(dt);
            }

            con.Close();
            if (dt.Rows.Count == 0)
            {
                connectionString();
                SqlConnection sqlcon = new SqlConnection(con.ConnectionString);
                string query = "insert into user_result(register_id )values(@register_id)";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                cmd.Parameters.AddWithValue("@register_id", register_id);
                cmd.ExecuteNonQuery();
                sqlcon.Close();
            }
            return View();
        }
        public IActionResult categoryChose()
        {
            
            connectionString();
            DataTable dt = new DataTable();
            string register_id = HttpContext.Session.GetString("register_id");

            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string query = "select * FROM user_result where register_id = '" + register_id + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.Fill(dt);
                
            }
            con.Close();
            return View(dt);
        }
        public IActionResult Biggner()
        {

            return View();
            
        }

        public IActionResult Intermidiate()
        {
            return View();
        }
        public IActionResult Difficult()
        {
            return View();

        }
        public IActionResult Instruction(int course_id)
        {
            ViewBag.ID = course_id;
            HttpContext.Session.SetString("course_id", course_id.ToString());

            string register_id = HttpContext.Session.GetString("register_id");
            DataTable dt = new DataTable();
            connectionString();
            Add_QuestionModel Question = new Add_QuestionModel();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {

                con.Open();
                string query = "SELECT top 10 question_id FROM question Where course_id = '"+ course_id + "' ORDER BY NEWID() ";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.Fill(dt);
            }

            Random ran = new Random();
            string paper_id = "KIET-" + ran.Next();
            string u_id = "DC-" + ran.Next(); // DC = double check

            HttpContext.Session.SetString("paper_id", paper_id.ToString());
            HttpContext.Session.SetString("u_id", u_id.ToString());

            int question_no = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                question_no++;
                Question.question_id = dt.Rows[i][0].ToString();
                SqlConnection sqlcon = new SqlConnection(con.ConnectionString);
                string query = "Insert into infoquizs(paper_id,register_id,question_id,u_id,question_no)values(@paper_id,@register_id ,@question_id,@u_id, @question_no)";
                SqlCommand cmd = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                cmd.Parameters.AddWithValue("@paper_id", paper_id);
                cmd.Parameters.AddWithValue("@register_id", register_id);
               cmd.Parameters.AddWithValue("@question_id", Question.question_id);
                cmd.Parameters.AddWithValue("@question_no", question_no);
                cmd.Parameters.AddWithValue("@u_id", u_id);

                cmd.ExecuteNonQuery();
                sqlcon.Close();
            }

            return View("Instruction");
        }

        public IActionResult Attempt_Quiz()
        {
            string user_name = HttpContext.Session.GetString("user_name");
            string course_id = HttpContext.Session.GetString("course_id");
            string register_id = HttpContext.Session.GetString("register_id");
            string u_id = HttpContext.Session.GetString("u_id");
            string paper_id = HttpContext.Session.GetString("paper_id");
            DataTable dt = new DataTable();

            category_chose category_Chose = new category_chose();
          

            connectionString();

            DataTable dt2 = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                
                con.Open();
                string query = "SELECT top 1 a.id,a.paper_id, a.register_id, a.u_id , b.question,b.option1,b.option2, b.option3,b.option4, b.question_id, b.answer, a.question_no from infoquizs a inner join question b on a.question_id = b.question_id WHERE register_id = '"+register_id+"' AND u_id = '"+u_id+"' ";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.Fill(dt2);

            }

            con.Close();
           

            int question_no = Convert.ToInt32(dt2.Rows[0][11].ToString());
            
                 if (question_no == 10)
            {
                connectionString();
                DataTable sd = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
                {
                    con.Open();
                    string query = "select count(ua_id) as total_ids, score from user_attempt where register_id = '"+ register_id +"' and paper_id = '"+paper_id+"' and u_id = '"+u_id+"' group by score";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                    sda.Fill(sd);


                    string score = sd.Rows[0][0].ToString();
                    string score1 = sd.Rows[0][1].ToString();

                    string score2 = sd.Rows[1][0].ToString();
                    string score3 = sd.Rows[1][1].ToString();

                    HttpContext.Session.SetString("wrong_answers", score.ToString());
                    HttpContext.Session.SetString("right_answers", score2.ToString());

                    con.Close();
                }

                 string score_2 = HttpContext.Session.GetString("right_answers");
                int real_score_to = Convert.ToInt32(score_2);
                HttpContext.Session.SetInt32("right_answers22", real_score_to);

                connectionString();
                DataTable sd1 = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
                {
                    //
                    con.Open();
                    string query = "select a.category_name,b.course_name from category a inner join courses b on a.category_id = b.category_id where course_id = '"+ course_id +"'";
                    SqlDataAdapter sda1 = new SqlDataAdapter(query, sqlcon);
                    sda1.Fill(sd1);
                    con.Close();

                    string category_name = sd1.Rows[0][0].ToString();

                    string course_name = sd1.Rows[0][1].ToString();
                    HttpContext.Session.SetString("course_name", course_name.ToString());
                    string course_names = HttpContext.Session.GetString("course_name");



                    if (course_id == "1" && real_score_to >= 5)
                    {
                        category_Chose.biggner_course1 = "1";
                        connectionString();
                        con.Open();
                        string query2 = "update user_result set biggner_course1 = @biggner_course1 where register_id = '" +register_id+ "' ";
                        SqlCommand cmd = new SqlCommand(query2, con);
                        cmd.Parameters.AddWithValue("@biggner_course1", category_Chose.biggner_course1);
                        cmd.ExecuteNonQuery();

                        DataTable sdt = new DataTable();
                        string query3 = "Select count(id) as total_record from user_result where register_id = '"+register_id+"' AND biggner_course1 = '1' AND biggner_course2 = '1' ";
                        SqlDataAdapter sda = new SqlDataAdapter(query3, sqlcon);
                        sda.Fill(sdt);
                        string total_record = sdt.Rows[0][0].ToString();
                        int real_total = Convert.ToInt32(total_record);

                        if (real_total > 0)
                        {
                            string query4 = "update user_result set biggner_is_pass = @biggner_is_pass where register_id = '" + register_id + "' ";
                            SqlCommand cmd1 = new SqlCommand(query4, con);
                            cmd1.Parameters.AddWithValue("@biggner_is_pass", '1');
                            cmd1.ExecuteNonQuery();
                        }


                        if (real_score_to >=5)
                        {
                            ViewData["Message"] = " pass !";
                        }

                    }
                    else if (course_id == "2" && real_score_to >= 5)
                    {
                        category_Chose.biggner_course2 = "1";
                        connectionString();
                        con.Open();
                        string query2 = "update user_result set biggner_course2 = @biggner_course2  where register_id = '" + register_id + "' ";
                        SqlCommand cmd = new SqlCommand(query2, con);
                        cmd.Parameters.AddWithValue("@biggner_course2", category_Chose.biggner_course2);
                        cmd.ExecuteNonQuery();

                        DataTable sdt = new DataTable();
                        string query3 = "Select count(id) as total_record from user_result where register_id = '" + register_id + "' AND biggner_course1 = '1' AND biggner_course2 = '1' ";
                        SqlDataAdapter sda = new SqlDataAdapter(query3, sqlcon);
                        sda.Fill(sdt);
                        string total_record = sdt.Rows[0][0].ToString();
                        int real_total = Convert.ToInt32(total_record);

                        if (real_total > 0)
                        {
                            string query4 = "update user_result set biggner_is_pass = @biggner_is_pass where register_id = '" + register_id + "' ";
                            SqlCommand cmd1 = new SqlCommand(query4, con);
                            cmd1.Parameters.AddWithValue("@biggner_is_pass", '1');
                            cmd1.ExecuteNonQuery();
                        }
                    }
                    else if (course_id == "3" && real_score_to >= 5)
                    {
                        category_Chose.intermidiate_course1 = "1";
                        connectionString();
                        con.Open();
                        string query2 = "update user_result set intermidiate_course1 = @intermidiate_course1 where register_id = '" + register_id + "' " ;
                        SqlCommand cmd = new SqlCommand(query2, con);
                        cmd.Parameters.AddWithValue("@intermidiate_course1", category_Chose.intermidiate_course1);
                        cmd.ExecuteNonQuery();

                    }
                    else if (course_id == "4" && real_score_to >= 5)
                    { 
                        category_Chose.intermidiate_course2 = "1";
                        connectionString();
                        con.Open();
                        string query2 = "update user_result set intermediate_course2 = @intermediate_course2 where register_id = '" + register_id + "' ";
                        SqlCommand cmd = new SqlCommand(query2, con);
                        cmd.Parameters.AddWithValue("@intermediate_course2", category_Chose.intermidiate_course2);
                        cmd.ExecuteNonQuery();

                        DataTable sdt = new DataTable();
                        string query3 = "Select count(id) as total_record from user_result where register_id = '" + register_id + "' AND intermediate_course2 = '1' AND intermediate_course2 = '1' ";
                        SqlDataAdapter sda = new SqlDataAdapter(query3, sqlcon);
                        sda.Fill(sdt);
                        string total_record = sdt.Rows[0][0].ToString();
                        int real_total = Convert.ToInt32(total_record);

                        if (real_total > 0)
                        {
                            string query4 = "update user_result set intermidiate_is_pass = @intermidiate_is_pass where register_id = '" + register_id + "' ";
                            SqlCommand cmd1 = new SqlCommand(query4, con);
                            cmd1.Parameters.AddWithValue("@intermidiate_is_pass", '1');
                            cmd1.ExecuteNonQuery();
                        }
                    }
                    else if (course_id == "5" && real_score_to >= 5)
                    { 
                        category_Chose.difficult_course1 = "1";
                        connectionString();
                        con.Open();
                        string query2 = "update user_result set difficult_course1 = @difficult_course1 where register_id = '" + register_id + "' ";
                        SqlCommand cmd = new SqlCommand(query2, con);
                        cmd.Parameters.AddWithValue("@difficult_course1", category_Chose.difficult_course1);
                        cmd.ExecuteNonQuery();

                        DataTable sdt = new DataTable();
                        string query3 = "Select count(id) as total_record from user_result where register_id = '" + register_id + "' AND difficult_course1 = '1' AND difficult_course2 = '1' ";
                        SqlDataAdapter sda = new SqlDataAdapter(query3, sqlcon);
                        sda.Fill(sdt);
                        string total_record = sdt.Rows[0][0].ToString();
                        int real_total = Convert.ToInt32(total_record);

                        if (real_total > 0)
                        {
                            string query4 = "update user_result set difficult_is_pass = @difficult_is_pass where register_id = '" + register_id + "' ";
                            SqlCommand cmd1 = new SqlCommand(query4, con);
                            cmd1.Parameters.AddWithValue("@difficult_is_pass", '1');
                            cmd1.ExecuteNonQuery();
                        }
                    }
                    else if (course_id == "6" && real_score_to >= 5)
                    {
                        category_Chose.difficult_course2 = "1";
                        connectionString();
                        con.Open();
                        string query2 = "update user_result set difficult_course2 = @difficult_course2 where register_id = '" + register_id + "' ";
                        SqlCommand cmd = new SqlCommand(query2, con);
                        cmd.Parameters.AddWithValue("@difficult_course2", category_Chose.difficult_course2);
                        cmd.ExecuteNonQuery();

                        DataTable sdt = new DataTable();
                        string query3 = "Select count(id) as total_record from user_result where register_id = '" + register_id + "' AND difficult_course1 = '1' AND difficult_course2 = '1' ";
                        SqlDataAdapter sda = new SqlDataAdapter(query3, sqlcon);
                        sda.Fill(sdt);
                        string total_record = sdt.Rows[0][0].ToString();
                        int real_total = Convert.ToInt32(total_record);

                        if (real_total > 0)
                        {
                            string query4 = "update user_result set difficult_is_pass = @difficult_is_pass where register_id = '" + register_id + "' ";
                            SqlCommand cmd1 = new SqlCommand(query4, con);
                            cmd1.Parameters.AddWithValue("@difficult_is_pass", '1');
                            cmd1.ExecuteNonQuery();
                        }
                    }

                }
              
               return RedirectToAction("Result");

            }



            else
            {
                return View(dt2);
            }




        }
        [HttpPost]
        public IActionResult Attempt_Quiz2(Add_QuestionModel ad)
        {

            string user_answer = ad.option1.ToString();
            string question_id = ad.question_id.ToString();
            string answer = ad.answer.ToString();
            string register_id = HttpContext.Session.GetString("register_id");
            string u_id = HttpContext.Session.GetString("u_id");
            string paper_id = HttpContext.Session.GetString("paper_id");

            if (user_answer == answer)
            {
                ad.score = 1;
            }
            else
            {
                ad.score = 0;
            }

            

            connectionString();
            SqlConnection sqlcon = new SqlConnection(con.ConnectionString);
            SqlCommand cmd = new SqlCommand("Insertattempt", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            sqlcon.Open();
            cmd.Parameters.AddWithValue("@register_id", register_id);
            cmd.Parameters.AddWithValue("@question_id", question_id);
            cmd.Parameters.AddWithValue("@user_answer", ad.option1);
            cmd.Parameters.AddWithValue("@score", ad.score);
            cmd.Parameters.AddWithValue("@paper_id", paper_id);
            cmd.Parameters.AddWithValue("@u_id", u_id);
            cmd.ExecuteNonQuery();
            sqlcon.Close();

            connectionString();
            SqlConnection scon = new SqlConnection(con.ConnectionString);
            string query2 = "DELETE FROM infoquizs where question_id = @question_id AND register_id = @register_id";
            SqlCommand command = new SqlCommand(query2, scon);
            scon.Open();
            command.Parameters.AddWithValue("@question_id", question_id);
            command.Parameters.AddWithValue("@register_id", register_id);
            command.ExecuteNonQuery();

            scon.Close();
            return RedirectToAction("Attempt_Quiz");

           

        }

        public IActionResult Result()
        {
            string course_id = HttpContext.Session.GetString("course_id");
            category_chose cate = new category_chose();
            Add_QuestionModel add_QuestionModel = new Add_QuestionModel();

            string register_id = HttpContext.Session.GetString("register_id");
            string user_name = HttpContext.Session.GetString("user_name");

            Add_QuestionModel Question = new Add_QuestionModel();
            DataTable dt = new DataTable();
            connectionString();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string query = "SELECT top 10 a.question_id, a.user_answer, a.score , b.question, b.option1, b.option2, b.option3, b.option4, b.answer from user_attempt a inner join question b on a.question_id = b.question_id where register_id = '" + register_id + "' and course_id = '"+course_id+"' ";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.Fill(dt);
            }

            con.Close();
            return View(dt);
        }
          
        }



    }







