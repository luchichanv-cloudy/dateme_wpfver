using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Guna.UI2.WinForms.Suite.Descriptions;

namespace dateme_wpfver
{
    /// <summary>
    /// Interaction logic for chatwindow.xaml
    /// </summary>
    public partial class chatwindow : Window
    {
        public chatwindow()
        {
            InitializeComponent();
        }
        public SqlCommand cmd = new SqlCommand();
        public SqlDataAdapter adtr;
        public SqlDataReader dr;
        public DataSet ds;
        SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
        private DispatcherTimer _dataUpdateTimer;

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            
            con.Open();

            string querya = "INSERT INTO MessageChat(room_id, user_id, message, date, user_name)  VALUES(@roomid, @userid, @message, @date, @username)";
            SqlCommand cmdd = new SqlCommand(querya, con);
            cmdd.Parameters.AddWithValue("@roomid", Chatroomidtb.Text);
            cmdd.Parameters.AddWithValue("@userid", CurrentUser.ID);
            cmdd.Parameters.AddWithValue("@message", Send_Text.Text);
            cmdd.Parameters.AddWithValue("@date", DateTime.Now);
            cmdd.Parameters.AddWithValue("@username", CurrentUser.username);
            cmdd.ExecuteNonQuery();

            Send_Text.Text = "";
            AllMessageLists();
            con.Close();
            
        }
       /* private void SetupDataUpdateTimer()
        {
            _dataUpdateTimer = new DispatcherTimer();
            _dataUpdateTimer.Tick += OnDataUpdateEvent; ;
            _dataUpdateTimer.Interval = TimeSpan.FromMilliseconds(10000);
            _dataUpdateTimer.Start();
        }
        private void OnDataUpdateEvent(object sender, EventArgs e)
        {
            AllMessageLists();

            // ...      
        }*/
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*  Thread sohbet = new Thread(new ThreadStart(AllMessageLists));
              sohbet.IsBackground = true;
              sohbet.Start();*/
            /*  logId = Properties.Settings.Default.id;
              logUsername = Properties.Settings.Default.username;
              logFullname = Properties.Settings.Default.fullname;
              logStatus = Properties.Settings.Default.status;*/
            
        }
        private void AllMessageLists()
        {
            
                try
                {

                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd = new SqlCommand("SELECT * FROM MessageChat  WHERE room_id="+Chatroomidtb.Text + " ORDER BY date ASC ", con);
                    cmd.ExecuteNonQuery();
                 
                    SqlDataReader dr = cmd.ExecuteReader();
                    txtAll.Text = "";
                    while (dr.Read())
                    {
                        string temp = dr["date"].ToString();
                        string temp1 = dr["user_name"].ToString();
                        string temp2 = dr["message"].ToString();
                        txtAll.Text += "(" + temp + " / " + temp1 + ") :  " + temp2 + "\n\n";
                        
                    }
                    txtAll.SelectionStart = txtAll.Text.Length;
                    txtAll.ScrollToEnd();
                    cmd.Dispose();
                    dr.Close();
               
                    Thread.Sleep(500);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            
        }
    }
}
