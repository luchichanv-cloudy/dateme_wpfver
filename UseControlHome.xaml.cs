using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for UseControlHome.xaml
    /// </summary>
    public partial class UseControlHome : UserControl
    {
        public UseControlHome()
        {
            InitializeComponent();
        }

        private void EditInformation_Btt_Click(object sender, RoutedEventArgs e)
        {
           
           
        }
        SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter sqa = new SqlDataAdapter("Select Image from ImageDatabase where UserId= '"+CurrentUser.ID+"'", con);
            sqa.Fill(ds);
            con.Close();
            byte[] data = (byte[])ds.Tables[0].Rows[0][0];

            //convert byte[] to memorystream to imagesource
            MemoryStream strm = new MemoryStream();

            strm.Write(data, 0, data.Length);

            strm.Position = 0;


            BitmapImage bi = new BitmapImage();

            bi.BeginInit();

            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = strm;
            imageSource.EndInit();

            // Up image len 
            HomeAvatar.ImageSource = imageSource; ;



            string query = "SELECT * FROM UserAccount WHERE UserId=@ID";
            SqlCommand sqlcmd = new SqlCommand(query, con);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            sqlcmd.Parameters.AddWithValue("@ID", CurrentUser.ID);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlDataReader oReader = sqlcmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
            if (oReader.Read())
            {
                NameTB.Text ="Name: "+ oReader["Name"].ToString();
                SocialMedia.Text = oReader["Job"].ToString();
                if (oReader["Gender_ID"].ToString() == "0")
                {
                    GenderTB.Text = "Female";
                }
                else if (oReader["Gender_ID"].ToString() == "1")
                {
                    GenderTB.Text = "Male";
                }
                else
                {
                    GenderTB.Text = "Non Binary";
                }
                AgeTB.Text = oReader["Birthday"].ToString();
                    

            }
            oReader.Close();
        }
    }
}
