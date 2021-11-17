using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace dateme_wpfver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();

            initlist(); //load nhung user da duoc match vao items
            SetupDataUpdateTimer();



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
            con.Open(); string info="";
            string query = "SELECT * FROM UserAccount WHERE UserId=@ID";
            SqlCommand sqlcmd = new SqlCommand(query, con);
            sqlcmd.CommandType = System.Data.CommandType.Text;
            sqlcmd.Parameters.AddWithValue("@ID", iduser);
            SqlDataReader oReader = sqlcmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
            string texttemp1 = "";
            if (oReader.Read())
            {
                texttemp1 = oReader["Username"].ToString();
                
                   
            }
            oReader.Close();
            con.Close();


            con.Open(); 
            string queryy = "SELECT * FROM Relationship WHERE User1ID=@ID1 AND User2ID=@ID2";
            SqlCommand sqll = new SqlCommand(queryy, con);
            sqll.CommandType = System.Data.CommandType.Text;
            sqll.Parameters.AddWithValue("@ID1", iduser);
            sqll.Parameters.AddWithValue("@ID2", CurrentUser.ID);
            SqlDataReader iReader = sqll.ExecuteReader(System.Data.CommandBehavior.SingleRow);
            
            if (iReader.Read())
            {
                string texttemp = iReader["chatroom_ID"].ToString();
                info = " Chat room ID: " + texttemp;

            }
            iReader.Close();
            con.Close();

            //add vao items
            items.Add(new TodoItem() { Title = texttemp1, ava = imageSource, RoomID=info });

        }
        private static DispatcherTimer _dataUpdateTimer = null;
        private void SetupDataUpdateTimer()
        {
            _dataUpdateTimer = new DispatcherTimer();
            _dataUpdateTimer.Tick += OnDataUpdateEvent; ;
            _dataUpdateTimer.Interval = TimeSpan.FromMilliseconds(10000);
            _dataUpdateTimer.Start();
        }

        private void OnDataUpdateEvent(object sender, EventArgs e)
        {
            upload();
            uploadforyouuser();
            Dispatcher.BeginInvoke(() =>
            {
                initlist();
            }
            );

            // ...      
        }
        ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();
        //  List<TodoItem> items = new List<TodoItem>();
        public void initlist()
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
                Addtolist(dsmatched[i]); //lay thong tin cua user id=dsmatched[i]  them vao items
            }

            icTodoList.ItemsSource = items;
            TabControl.Items.Refresh();

            
        }


        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                App.Current.Windows[intCounter].Close();
        }
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "HomeItem":
                    usc = new UseControlHome();
                    GridMain.Children.Add(usc);

                    break;
                case "ForyouItem":
                    //   usc = new UserControlCreate();
                    //  GridMain.Children.Add(usc);
                    usc = new UserControlForYou();
                    GridMain.Children.Add(usc);


                    break;

                case "EditProfileItem":
                    usc = new EditProfile();
                    GridMain.Children.Add(usc);
                    break;
                case "ChatItem":
                    chatwindow w2 = new chatwindow();
                    w2.Show();
                    break;
                default:
                    break;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }
        //CurrentUsername_Top.Text = "Tu Van";
        SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
        public void upload()
        {

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
                CurrentUsername_Top.Text = oReader["Username"].ToString();
                UserIconNameLeft.Text = oReader["Username"].ToString();
                CurrentUser.username= oReader["Username"].ToString();
            }
            oReader.Close();


            DataSet ds = new DataSet();
            SqlDataAdapter sqa = new SqlDataAdapter("Select Image from ImageDatabase where UserId= '" + CurrentUser.ID + "'", con);
            sqa.Fill(ds);

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
            // Up image len 
            Avaicon.ImageSource = imageSource;


            string querya = "SELECT * FROM InterestedValue WHERE UserId=@IDA";
            SqlCommand sql = new SqlCommand(querya, con);
            sql.CommandType = System.Data.CommandType.Text;
            sql.Parameters.AddWithValue("@IDA", CurrentUser.ID);
            SqlDataReader Reader = sql.ExecuteReader(System.Data.CommandBehavior.SingleRow);
            if (Reader.Read())
            {
                AgeFromtb.Text = Reader["AgeFrom"].ToString();
                AgeTotb.Text = Reader["AgeTo"].ToString();
                Gendercbbox.SelectedIndex = Convert.ToInt32(Reader["Gender"].ToString());
            }
            Reader.Close();
            con.Close();



            //update list 
        }
        public void _tab_load()
        {
            StackPanel newChild = new StackPanel();
            Matched_Tab.Content = newChild;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            upload();
            uploadforyouuser();
            //matched_tab_load();

        }

        private void ChatItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LogoutItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ForyouItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void HomeItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LogoutItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            CloseAllWindows();
            Login login = new Login();

            login.Show();
        }

        private void Exit_btt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void uploadforyouuser()
        {
            ds.Clear();
            dsshow.Clear();
            //theo yeu cau cua discovery setting
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string querya = "SELECT * FROM UserAccount WHERE (Gender_ID=@ID1 OR Gender_ID=@ID2 OR Gender_ID=@ID3) AND year(Birthday)<@year2 AND year(Birthday)>@year1 AND NOT UserId=@ID";
            SqlCommand sql = new SqlCommand(querya, con);
            sql.CommandType = System.Data.CommandType.Text;

            sql.Parameters.AddWithValue("@ID", CurrentUser.ID);
            if (Gendercbbox.SelectedIndex == 3)
            {
                sql.Parameters.AddWithValue("@ID1", 0);
                sql.Parameters.AddWithValue("@ID2", 1);
                sql.Parameters.AddWithValue("@ID3", 2);
            }
            else
            {
                sql.Parameters.AddWithValue("@ID1", Gendercbbox.SelectedIndex);
                sql.Parameters.AddWithValue("@ID2", Gendercbbox.SelectedIndex);
                sql.Parameters.AddWithValue("@ID3", Gendercbbox.SelectedIndex);
            }
            sql.Parameters.AddWithValue("@year2", DateTime.Now.Year - Convert.ToInt32(AgeFromtb.Text) + 1);
            sql.Parameters.AddWithValue("@year1", DateTime.Now.Year - Convert.ToInt32(AgeTotb.Text) - 1);
            SqlDataReader Reader = sql.ExecuteReader();


            while (Reader.Read())
            {


                ds.Add(Convert.ToInt32(Reader["UserId"].ToString()));
                dsshow.Add(false);



            }
            Reader.Close();

            //xoa nhung ten da match thanh cong




            con.Close();

        }
        private void DiscoverySettingbutton_Click(object sender, RoutedEventArgs e)
        {
            //cap nhat vao bang interested
            SqlConnection con = new SqlConnection(@"Server=DESKTOP-1885NSF;Database=DateMe_WPF;Integrated Security=True");
            con.Open();

            string query = "UPDATE InterestedValue SET AgeFrom = " + Convert.ToInt32(AgeFromtb.Text) + ", AgeTo=" + Convert.ToInt32(AgeTotb.Text) + ", Gender=" + Gendercbbox.SelectedIndex.ToString() +
                  " WHERE UserId =" + CurrentUser.ID.ToString();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteScalar();

            //cap nhat lai list danh sach 

            con.Close();
            uploadforyouuser();



        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) && e.SystemKey == Key.R)
            {
                upload();
                uploadforyouuser();
                initlist();
            }
        }

        private void AgeFromtb_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void AgeTotb_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        public static List<int> ds = new List<int>();
        public static List<bool> dsshow = new List<bool>();
        public static List<int> dsmatched = new List<int>();
        private void ForyouItem_Loaded(object sender, RoutedEventArgs e)
        {

        }
        
        private void UserIconNameLeft_Click(object sender, RoutedEventArgs e)
        {
        
        }
    }

    public class TodoItem
    {

        public string Title { get; set; }

        public ImageSource ava { get; set; }

        public string RoomID { get; set; }


    }
}
