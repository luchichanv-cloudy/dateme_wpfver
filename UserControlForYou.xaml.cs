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
    /// Interaction logic for UserControlForYou.xaml
    /// </summary>
    public partial class UserControlForYou : UserControl
    {
        public UserControlForYou()
        {
            InitializeComponent();
        }
        public void display(int i)
        {
          
            SqlConnection sqlCon = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
            sqlCon.Open();
            DataSet ds = new DataSet();
         

            SqlDataAdapter sqa = new SqlDataAdapter("Select Image from ImageDatabase where UserId='" + MainWindow.ds[i] + "'", sqlCon);
            sqa.Fill(ds);
            sqlCon.Close();
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
            imagetrial.ImageSource = imageSource; ;

        } 
        int i = 0;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //get the list
           
            
            if (MainWindow.ds.Count>0)
            {
                display(i);
            }
            else
            {
                MessageBox.Show("There is no user suitable for you");
            }    



            //connect to sql

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            i++;
            if (MainWindow.ds != null && i<=MainWindow.ds.Count-1)
            {
                display(i);
            }
            else
            {
                MessageBox.Show("There is no user suitable for you");
            }
           
        }
        
        SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
        private void Liked_button_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            string query = "SELECT COUNT(1) FROM Relationship WHERE (User1ID=@ID1 AND User2ID=@ID2) OR (User1ID=@ID2 AND User2ID=@ID1)";
            SqlCommand sqlcmd = new SqlCommand(query, con);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            sqlcmd.Parameters.AddWithValue("@ID2", CurrentUser.ID);
            
            sqlcmd.Parameters.AddWithValue("@ID1", MainWindow.ds[i]);
            int bo = Convert.ToInt32(sqlcmd.ExecuteScalar());
            if (bo == 0)
            {
               
                SqlCommand sc = new SqlCommand("insert into Relationship values(@ID2, @ID1,  @match, 0)", con);
               

                sc.Parameters.AddWithValue("@ID2",CurrentUser.ID);
                sc.Parameters.AddWithValue("@ID1",MainWindow.ds[i]);
               
                sc.Parameters.AddWithValue("@match",0);
                sc.ExecuteNonQuery();
                MainWindow.dsshow[i] = true;
               
            }    
            else if(bo == 2)
            {
                
                SqlCommand sc = new SqlCommand("UPDATE Relationship SET Matched=1 WHERE User1ID=@ID1 AND User2ID=@ID2", con);



                sc.Parameters.AddWithValue("@ID2", CurrentUser.ID);
                sc.Parameters.AddWithValue("@ID1", MainWindow.ds[i]);
                SqlCommand scb = new SqlCommand("UPDATE Relationship SET Matched=1 WHERE User1ID=@User1 AND User2ID=@User2", con);



                scb.Parameters.AddWithValue("@ID2", CurrentUser.ID);
                scb.Parameters.AddWithValue("@ID1", MainWindow.ds[i]);
                

            }
            else if(bo == 1)
            {
                string q = "SELECT * FROM Relationship WHERE (User1ID=@ID1 AND User2ID=@ID2) OR (User1ID=@ID2 AND User2ID=@ID1)";
                
                SqlCommand sq = new SqlCommand(q, con);
                sq.Parameters.AddWithValue("@ID2", CurrentUser.ID);
                sq.Parameters.AddWithValue("@ID1", MainWindow.ds[i]);
                SqlDataReader Reader = sq.ExecuteReader();
                int a = -1; int b = -1; 
                if (Reader.Read()==true)
                {
                    a = Convert.ToInt32(Reader["User1ID"].ToString());
                    b = Convert.ToInt32(Reader["User2ID"].ToString());
                }
                Reader.Close();
               
                
               

              if (b == CurrentUser.ID)
                {
                    string room = CurrentUser.ID.ToString() + MainWindow.ds[i].ToString();
                    SqlCommand sc = new SqlCommand("insert into Relationship values(@ID2, @ID1, @match, @rooom)", con);
                    sc.Parameters.AddWithValue("@ID2", CurrentUser.ID);
                    sc.Parameters.AddWithValue("@ID1", MainWindow.ds[i]);

                    sc.Parameters.AddWithValue("@match", 1);
                    sc.Parameters.AddWithValue("@rooom", Convert.ToInt32(room));
                    sc.ExecuteScalar();
                    
                    SqlCommand sca = new SqlCommand("UPDATE Relationship SET Matched=1, chatroom_ID="+room+" WHERE User1ID=@ID1 AND User2ID=@ID2", con);



                    sca.Parameters.AddWithValue("@ID2", CurrentUser.ID);
                    sca.Parameters.AddWithValue("@ID1", MainWindow.ds[i]);
                   
                    sca.ExecuteScalar();
                    MessageBox.Show("It's a Match");
                    

                }
                
            }
            con.Close();

            
            if (MainWindow.ds != null &&  i< MainWindow.ds.Count-1 )
            {
                i++;
                display(i);
            }
            else
            {
                MessageBox.Show("There is no user suitable for you");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            i++;
            if (MainWindow.ds != null && i <= MainWindow.ds.Count - 1)
            {
                display(i);
            }
            else
            {
                MessageBox.Show("There is no user suitable for you");
            }
        }
    }
}
