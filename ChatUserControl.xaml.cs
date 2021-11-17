

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
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
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;

namespace dateme_wpfver
{
    /// <summary>
    /// Interaction logic for ChatUserControl.xaml
    /// </summary>
    /// 
    public partial class ChatUserControl : UserControl
    {
        public ChatUserControl()
        {
            InitializeComponent();
           
        }
        ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();
        SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                con.Close();
                
                string querya = "Insert Into message(time,fullname,msg) Values ('"
                  + DateTime.Now.ToLongTimeString() + "','"
                  + CurrentUser.username + "','"
                  + Send_Text.Text + "')";
                SqlCommand sc = new SqlCommand(querya, con);
                con.Open();
                sc.ExecuteNonQuery();
                sc.Dispose();
                con.Close();
                Send_Text.Text = "";


            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
        }
        public void InitOnlineUser()
        {

            items.Clear();

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            List<int> dsmatched = new List<int>();
            //  con.Open();
            string query = "SELECT * FROM Relationship WHERE User1ID=@ID AND Matched=1";
            SqlCommand sqlcmd = new SqlCommand(query, con);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            sqlcmd.Parameters.AddWithValue("@ID", CurrentUser.ID);
            SqlDataReader oReader = sqlcmd.ExecuteReader();
            while (oReader.Read())
            {
                int x = Convert.ToInt32(oReader["User2ID"].ToString());
                dsmatched.Add(x); // luu lai id cua cac user  matched
            }
            oReader.Close();


            for (int i = 0; i < dsmatched.Count; i++)
            {
                string queri = "SELECT COUNT(1) FROM UserAccount WHERE UserId=@ID AND Status=1";
                SqlCommand cm = new SqlCommand(queri, con);
                cm.Parameters.AddWithValue("@ID", dsmatched[i]);
                int count = (int)cm.ExecuteScalar();
                if (count >0)
                {
                    Addtolist(dsmatched[i]);
                }
                else
                {
                    dsmatched.Remove(dsmatched[i]);
                }
               //lay thong tin cua user id=dsmatched[i]  them vao items
            }

            icTodoList.ItemsSource = items;
            ListViewUser.Items.Refresh();
        }
        public void Addtolist(int iduser)
        {
            //lay image avatar
            DataSet ds = new DataSet();
            SqlDataAdapter sqa = new SqlDataAdapter("Select Image from ImageDatabase where UserId= '" + iduser + "'", con);
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
            //


            //lay username cua doituong
            con.Open();
            string query = "SELECT * FROM UserAccount WHERE UserId=@ID";
            SqlCommand sqlcmd = new SqlCommand(query, con);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            sqlcmd.Parameters.AddWithValue("@ID", iduser);
            SqlDataReader oReader = sqlcmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
            string texttemp = "";
            if (oReader.Read())
            {
                texttemp = oReader["Username"].ToString();

            }
            oReader.Close();
            con.Close();

            //add vao items
            items.Add(new TodoItem() { Title = texttemp, ava = imageSource });

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitOnlineUser();
            this.Dispatcher.Invoke(() =>
            {
                Thread sohbet = new Thread(new ThreadStart(AllMessageLists));
                sohbet.IsBackground = true;
                sohbet.Start();
            });
           
        }
        private void AllMessageLists()
        {
            while (true)
            {
                try
                {
                    con.Close();
                    con.Open();
                   
                    SqlCommand sql= new SqlCommand ("SELECT * FROM message ORDER BY id ASC",con);

                    sql.ExecuteNonQuery();
                   SqlDataReader dr = sql.ExecuteReader();
                    txtAll.Text = "";
                    while (dr.Read())
                    {
                        txtAll.Text += "(" + dr["time"].ToString() + " / " + dr["fullname"].ToString() + ") :  " + dr["msg"].ToString() + "\n\n";
                    }
                    txtAll.SelectionStart = txtAll.Text.Length;
                    txtAll.ScrollToLine(txtAll.GetLineIndexFromCharacterIndex(txtAll.SelectionStart));
                    con.Dispose();
                    con.Close();
                    InitOnlineUser();
                    Thread.Sleep(500);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        public class TodoItem
        {

            public string Title { get; set; }

            public ImageSource ava { get; set; }



        }
    }
}

