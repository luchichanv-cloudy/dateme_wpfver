using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace dateme_wpfver
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void dragMe(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {

            }
        }
        private void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                App.Current.Windows[intCounter].Close();
        }
       
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
                if (con.State == System.Data.ConnectionState.Closed)
                {
                    con.Open();
                }    
                string query = "SELECT * FROM UserAccount WHERE Username=@Username AND Password=@Password";
                SqlCommand sqlcmd = new SqlCommand(query, con);
                
                sqlcmd.Parameters.AddWithValue("@Username", UsernameLoginTextBox.Text);
                sqlcmd.Parameters.AddWithValue("@Password", PasswordBox.Password);
                int Count = Convert.ToInt32(sqlcmd.ExecuteScalar());


              
                if (Count>0)
                {
                    SqlDataReader reader = sqlcmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                    if(reader.Read())
                    {
                        CurrentUser.ID = Convert.ToInt32(reader["UserId"].ToString());
                    }
                    reader.Close();
                    string temp = "UPDATE UserAccount SET Status=1 WHERE UserId=" + CurrentUser.ID;
                    SqlCommand cmd= new SqlCommand(temp, con);
                    cmd.ExecuteScalar();
                    CloseAllWindows();
                    MainWindow window1 = new MainWindow();
                    window1.Show();
                }    
                else
                {
                    MessageBox.Show("Username or Password isn't correct ");                                                                                 
                }    
            }                                    
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
            
           
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        { 
            CloseAllWindows();
           
            Register window1 = new Register();
            this.Close();
            window1.Show();
           
            
            
        }

        private void UsernameLoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
