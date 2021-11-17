using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace dateme_wpfver
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }
        private void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                App.Current.Windows[intCounter].Close();
        }
        private void Register_RForm_Button_Click(object sender, RoutedEventArgs e)
        {
            if(PasswordBox.Password!=PasswordConfirmBox.Password)
            {
                MessageBox.Show("The specified password is not match");
            }    
            else
            { 
            try
            {
                    // add user to user account database
                SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
                con.Open();

                string query = "INSERT INTO UserAccount VALUES(@Name, @Username, @Gender_ID, @Password, @Birthday, @Province, @Job ,@Status); SELECT SCOPE_IDENTITY() ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", NameLoginTextBox.Text);
                cmd.Parameters.AddWithValue("@Password", PasswordBox.Password);
                cmd.Parameters.AddWithValue("@Gender_ID", GenderComboBox.SelectedIndex);
                cmd.Parameters.AddWithValue("@Province", ((ComboBoxItem)ProvinceComboBox.SelectedItem).Content.ToString());
                cmd.Parameters.AddWithValue("@Birthday", BirthdayLoginDateBox.DisplayDate);
                cmd.Parameters.AddWithValue("@Username", UsernameLoginTextBox.Text);
                cmd.Parameters.AddWithValue("@Job", JobTextBox.Text);
                    cmd.Parameters.AddWithValue("@Status", 0);

                    int insertedID = Convert.ToInt32(cmd.ExecuteScalar());




                    // create default avatar of account 

                    string defaultphoto = "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";
                    byte[] data = Convert.FromBase64String(defaultphoto);
                   

                    SqlConnection sqlCon = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
                    sqlCon.Open();
                    SqlCommand sc = new SqlCommand("insert into ImageDatabase(UserId, Image) values(@n, @p)", sqlCon);


                    sc.Parameters.AddWithValue("@p", data);
                    sc.Parameters.AddWithValue("@n", insertedID);
                    sc.ExecuteNonQuery();

                    sqlCon.Close();
                    con.Close();

                    //init the interest database
                    SqlConnection conn = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
                    conn.Open();
                    SqlCommand sci = new SqlCommand("insert into InterestedValue  values(@a, @b, @c, @d)", conn);
                    sci.Parameters.AddWithValue("@a", insertedID);
                    sci.Parameters.AddWithValue("@b", 3);
                    sci.Parameters.AddWithValue("@c", 16);
                    sci.Parameters.AddWithValue("@d", 80);
                    sci.ExecuteNonQuery();
               
                    conn.Close();






                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            Login form1 = new Login();
            form1.Show();
                this.Close();
            }
        }
    }
}
