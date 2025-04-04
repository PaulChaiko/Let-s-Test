using Azure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Let_s_Test
{
    public class Answer
    {
        public int id { get; set; }
        public int test_id { get; set; }
        public int  question_id { get; set; }
        public string answer_text { get; set; }
        public bool is_correst { get; set; }
    }
    public class Question
    {
        public int id { get; set; }
        public int test_id { get; set; }
        public string question_text { get; set; }
    }


    public partial class TestingWindow : Window
    {
        List<Test> TList = new List<Test>();
        List<User> UList = new List<User>();
        List<UserTest> UTList = new List<UserTest>();

        List<Question> QList = new List<Question>();
        List <Question> QofOne = new List<Question>();


        List<Answer> AList = new List<Answer>();
        List<Answer> AofOne = new List<Answer>();
        int TestsCount;
        int _UserID;
        List<int> _AT = new List<int>();
        private string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        int ActUser;
        int Q = 0;
        int QCount = 0;
        int T = 0;
        

        List <int> Scores = new List<int>();


        public TestingWindow(List<int> AT, int AU)
        {
            InitializeComponent();

            TList = FillTests(TList);
            UList = FillUsers(UList);
            UTList = FillUserTests(UTList);
            QList = FillQuestions(QList);

            ActUser = AU;

            foreach (int i in AT) Scores.Add(0);

            TestsCount = AT.Count;

            _UserID = AU;
            _AT = AT;
            AList=Fill(AList);
        



            foreach (Question QQQ in QList)
            {
                if (QQQ.test_id == _AT[T]) QofOne.Add(QQQ);
            }

            QCount=QofOne.Count;

            QBox.Text = QofOne[Q].question_text;

            foreach (Answer AAA in AList)
            {
                if (AAA.question_id == QofOne[Q].id) AofOne.Add(AAA);

            }

            T1.Text = AofOne[0].answer_text;
            T2.Text = AofOne[1].answer_text;
            T3.Text = AofOne[2].answer_text;
            T4.Text = AofOne[3].answer_text;





        }


        public List<Answer> Fill(List<Answer> _AList)
        {
            string query = "SELECT id, test_id, question_id, answer_text, is_correct FROM Answers";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _AList.Add(new Answer
                            {
                                id = reader.GetInt32(0),
                                test_id = reader.GetInt32(1),
                                question_id = reader.GetInt32(2),
                                answer_text = reader.GetString(3),
                                is_correst = reader.GetBoolean(4)


                            });

                        }
                    }

                }
            }
            return _AList;
        }
        private void B1_Click(object sender, RoutedEventArgs e)
        {
            GiveAnswer(0);
        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            GiveAnswer(1);
        }

        private void B3_Click(object sender, RoutedEventArgs e)
        {
            GiveAnswer(2);
        }

        private void B4_Click(object sender, RoutedEventArgs e)
        {
            GiveAnswer(3);
        }

        public void GiveAnswer (int A)
        {

            if (AofOne[A].is_correst) Scores[T]++;
            Q++;

            if(Q==QCount)
            {
                T++;
                Q = 0;
                if (T == TestsCount) { TheEnd(); return; }

                QofOne.Clear();
                foreach (Question QQQ in QList)
                {
                    if (QQQ.test_id == _AT[T]) QofOne.Add(QQQ);
                }

                QCount = QofOne.Count;


            }

            QBox.Text = QofOne[Q].question_text;
            AofOne.Clear();

            foreach (Answer AAA in AList)
            {
                if (AAA.question_id == QofOne[Q].id) AofOne.Add(AAA);

            }

            T1.Text = AofOne[0].answer_text;
            T2.Text = AofOne[1].answer_text;
            T3.Text = AofOne[2].answer_text;
            T4.Text = AofOne[3].answer_text;




        }

        public void TheEnd()
        {
            this.Close();
            string INFO="";
            for (int i = 0; i < TestsCount; i++)
            {
                foreach (Test TTT in TList)
                {
                    if (TTT.id == _AT[i])
                    {
                        INFO += TTT.test_name;
                        INFO += " ";
                        INFO += Scores[i].ToString();
                        INFO += " из ";
                        INFO += TTT.test_size.ToString();
                        INFO += "\n";
                    }
                }
            }
                MessageBox.Show(INFO);

                for (int j = 0; j < TestsCount; j++)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                   
                        connection.Open();
                        string sql = "INSERT INTO UserTests (user_id, test_id, score) VALUES (@user_id, @test_id, @score)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            
                            command.Parameters.AddWithValue("@user_id", ActUser);
                            command.Parameters.AddWithValue("@test_id", _AT[j]);
                            command.Parameters.AddWithValue("@score", Scores[j]);

                            
                            command.ExecuteNonQuery();

                        }

                    }
                }

            

        }

        //public void ASK()
        //{
        //   // QBox.Text=
            
        //}

        List<Test> FillTests(List<Test> _TList)
        {
            string query = "SELECT id, test_name, test_size FROM Tests";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _TList.Add(new Test
                            {
                                id = reader.GetInt32(0),
                                test_name = reader.GetString(1),
                                test_size = reader.GetInt32(2)

                            });

                        }
                    }

                }
            }
            return _TList;

        }
        List<User> FillUsers(List<User> _UList)
        {
            string query = "SELECT id, user_login, user_pass FROM Users";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _UList.Add(new User
                            {
                                id = reader.GetInt32(0),
                                user_login = reader.GetString(1),
                                user_pass = reader.GetString(2)


                            });

                        }
                    }

                }
            }


            return _UList;
        }
        List<UserTest> FillUserTests(List<UserTest> _UTList)
        {
            string query = "SELECT test_id, user_id, score FROM UserTests";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _UTList.Add(new UserTest
                            {
                                user_id = reader.GetInt32(0),
                                test_id = reader.GetInt32(1),
                                score = reader.GetInt32(2)


                            });

                        }
                    }

                }
            }


            return _UTList;
        }

        List<Question> FillQuestions(List<Question> _QTList)
        {
            string query = "SELECT id, test_id, question_text FROM Questions";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _QTList.Add(new Question
                            {
                                id = reader.GetInt32(0),
                                test_id = reader.GetInt32(1),
                                question_text = reader.GetString(2)


                            });

                        }
                    }

                }
            }


            return _QTList;
        }

    }
}
