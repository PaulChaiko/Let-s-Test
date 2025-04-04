using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace Let_s_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Test
    {
        public int id { get; set; }
        public string test_name { get; set; }
        public int test_size { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string user_login { get; set; }
        public string user_pass { get; set; }
    }

    public class UserTest
    {
        public int user_id { get; set; }
        public int test_id { get; set; }
        public int score { get; set; }

    }




    public partial class MainWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        int ActUser;
        List<int> ActTests = new List<int>();

       // private string connectionString = @"Server = DESERT_ROSE\SQLEXPRESS;Database=TestDatabase;Trusted_Connection=True; TrustServerCertificate=True;";
        List<Test> TList = new List<Test>();
        List<User> UList = new List<User>();
        List<UserTest> UTList = new List<UserTest>();
        
        public MainWindow()
        {
            InitializeComponent();
            // var TList = new List<Test>();
            //var UList = new List<User>();
            //var UTList = new List<UserTest>();
            TList = FillTests(TList);
            UList = FillUsers(UList);
            UTList = FillUserTests(UTList);
            FillUsersBox(UList, UTList, TList);
            FillTestsBox(TList);

            //var _Testing = new TestingWindow(this);
            // _Testing.Show();




        }

        private void RegB_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text != "" && PassBox.Text != "")
            {
                string l = LoginBox.Text;
                string p = PassBox.Text;
                bool q = false;

                foreach (User u in UList) if (u.user_login == l) q = true;
                if (q) MessageBox.Show("Такой пользователь уже существует!");
                else
                {
                    string query = "INSERT INTO Users (user_login, user_pass) VALUES (@user_login, @user_pass)";


                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();



                        using (var command = new SqlCommand(query, connection))
                        {

                            command.Parameters.AddWithValue("@user_login", l);
                            command.Parameters.AddWithValue("@user_pass", p);
                            command.ExecuteNonQuery();

                        }
                    }

                    UList = FillUsers(UList);
                    FillUsersBox(UList, UTList, TList);

                    Hello.Text = $"Привет, {l}";
                    StartB.IsEnabled = true;
                }

            }



        }

        private void AutB_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text != "" && PassBox.Text != "")
            {
                string l = LoginBox.Text;
                string p = PassBox.Text;
                bool lq = false;
                foreach (User u in UList)
                {
                    if (u.user_login == l)
                    {
                        lq = true;
                        if (u.user_pass != p)
                        {
                            MessageBox.Show("Неверный пароль!");
                        }
                        else
                        {
                            Hello.Text = $"Привет, {u.user_login}";
                            StartB.IsEnabled = true;
                            ActUser = u.id;
                            

                        }
                    }
                }

                if (!lq) MessageBox.Show("Такой пользователь не обнаружен!");

            }
        }

        private void StartB_Click(object sender, RoutedEventArgs e)
        {
            if (ChosenTest.Items.Count > 0)
            {
                ActTests.Clear();
                foreach (var ct in ChosenTest.Items)
                {
                    foreach (Test t in TList)
                    {
                        if (ct.ToString().Contains(t.test_name)) ActTests.Add(t.id);
                    }
                }

                var _Testing = new TestingWindow(ActTests, ActUser);
                _Testing.ShowDialog();
            }
        }

        private void AddB_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
        }

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
            string query = "SELECT user_id, test_id, score FROM UserTests";

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

        void FillUsersBox(List<User> _UList, List<UserTest> _UTList, List<Test> _TList)
        {
            int ii = 0;
            foreach (var user in _UList) ii++;

            for (int i = 0; i < ii; i++)
            {
                Results.Text += _UList[i].user_login;
                //Results.Text += "\n";
                foreach (var utest in _UTList)
                {
                    string A="";
                    string B="";

                    foreach (var test in _TList) { if (test.id == utest.test_id) A = test.test_name; B = test.test_size.ToString(); }
                    

                    if (utest.user_id == _UList[i].id) Results.Text += $"\n {A}  {utest.score}/{B}";


                }

                Results.Text += "\n";
                Results.Text += "///////////////////////////";
                Results.Text += "\n\n";

            }

        }

        void FillTestsBox(List<Test> _TList)
        {
            int ii = 0;
            string muffin;
            foreach (var t in _TList)
            {
                muffin = $"{t.test_name} ({t.test_size} вопросов)";
                TestList.Items.Add(muffin);

            }



        }

        private void TestList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = listBox.SelectedItem;
            if (!ChosenTest.Items.Contains(selectedItem.ToString())) ChosenTest.Items.Add(selectedItem.ToString());

        }

        private void ChosenTest_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = listBox.SelectedItem;
            listBox.Items.Remove(selectedItem.ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                @"Вы действительно хотите закрыть приложение?",
                "Подтверждение закрытия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true; // Отменяем закрытие приложения
            }
            else

            MessageBox.Show(@"Авторы:
1. Перепечаев Роман Романович
2. Поляков Андрей Сергеевич
3. Полякова Мария Валерьевна
4. Сагаровский Александр Геннадиевич
5. Трегубенко Виктория Игоревна
6. Томилов Антон Игоревич
7. Чайко Павел Александрович
8. Чекеренда Юрий
9. Янко Никита Федорович");
        }



    }
}