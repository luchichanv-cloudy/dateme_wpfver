using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dateme_wpfver
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : UserControl
    {
        public EditProfile()
        {
            InitializeComponent();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
            con.Open();

            string query = "UPDATE UserAccount SET Name = '" +NameLoginTextBox.Text +"', Username = '"+UsernameLoginTextBox.Text+ "', Password = '"+PasswordBox.Password+"'" +
                    ", Gender_ID=" +GenderComboBox.SelectedIndex.ToString()+
                    ", Province = '" + ((ComboBoxItem)ProvinceComboBox.SelectedItem).Content.ToString()+ 
                    "', Birthday= '" +BirthdayLoginDateBox.DisplayDate+ "' , Job = '"+JobTextBox.Text+"' WHERE UserId =" +CurrentUser.ID.ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteScalar();
            con.Close();
        }

        private void UploadAva_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select image";
            ofd.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
         "Portable Network Graphic (*.png)|*.png";
            if (ofd.ShowDialog() == true)
            {
               
                ImageSource imgsource = new BitmapImage(new Uri( ofd.FileName));
                Ava.Source = imgsource;
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open,
FileAccess.Read);

                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, System.Convert.ToInt32(fs.Length));

                fs.Close();

                SqlConnection sqlCon = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
                sqlCon.Open();
                SqlCommand sc = new SqlCommand("UPDATE ImageDatabase SET Image=@p WHERE UserID="+CurrentUser.ID.ToString(), sqlCon);


                sc.Parameters.AddWithValue("@p", data);
                sc.Parameters.AddWithValue("@n", CurrentUser.ID);
                sc.ExecuteNonQuery();
                sqlCon.Close();
                CurrentUser.upload = true;
            };

        }

       
    }
}
