using Microsoft.Win32;
using object_automatisch_aanmaken;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        private byte leds = 0b00000000;
        private byte druk = 0b00000000;
        private byte data = 0b00000000;
        public int i = 0;
        public int value = 0;
        public int count = 0;
        DispatcherTimer timer = new DispatcherTimer();
        private List<Gebruiker> objectList = new List<Gebruiker>();
        string voornaam;
        string achternaam;
        string leeftijd;
        string username;
        string pasword;
        DispatcherTimer timer2 = new DispatcherTimer();
        bool load = false;


        public MainWindow()
        {
            InitializeComponent();
            _serialPort = new SerialPort();
            _serialPort.DataReceived += _serialPort_DataReceived;
            cbxComPorts.Items.Add("None");
            foreach (string s in SerialPort.GetPortNames())
                cbxComPorts.Items.Add(s);
            StartTimer2();


        }

        private void Username_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_Username.Text))
            {
                txt_Username.Text = "Username";
            }
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            lbl_Status.Visibility = Visibility.Collapsed;

            if (txt_Username.Text == "Username")
            {
                txt_Username.Text = "";


            }
        }

        private void txt_Pasword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_Password.Text))
            {
                txt_Password.Text = "Password";
            }
        }

        private void txt_Pasword_GotFocus(object sender, RoutedEventArgs e)
        {

            if (txt_Password.Text == "Password")
            {
                txt_Password.Text = "";
            }
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            load = false;
            if (txt_Username.Text == "Username")
            {
                MessageBox.Show("Vul uw gebruikernaam in.");
                return;
            }
            string inputUsername = txt_Username.Text;
            if (CheckUserExists(inputUsername))
            {
                lbl_Status.Visibility = Visibility.Collapsed;
                string inputPassword = txt_Password.Text;
                Gebruiker userToLogin = objectList.FirstOrDefault(gebruiker => gebruiker.Username.Equals(inputUsername, StringComparison.OrdinalIgnoreCase));

                if (userToLogin != null && userToLogin.Pasword.Equals(inputPassword))
                {
                    canvas_Inloggen.Visibility = Visibility.Collapsed;
                    i = 0;
                    value =0;
                    timer.Interval = TimeSpan.FromMilliseconds(100);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    btn_inloggen.Visibility = Visibility.Collapsed;
                    btn_aanmelden.Visibility = Visibility.Collapsed;
                    LijnA.Visibility = Visibility.Collapsed;
                    LijnI.Visibility = Visibility.Collapsed;

                    lbl_naam.Content = userToLogin.Username;
                    userV.Content = $"Voornaam: {userToLogin.FirstName}";
                    userA.Content = $"Achternaam: {userToLogin.LastName}";
                    userL.Content = $"Leeftijd: {userToLogin.Age}";
                    userU.Content = $"Username: {userToLogin.Username}";
                    userP.Content = $"Paswoord: {userToLogin.Pasword}";
                }
                else
                {
                    MessageBox.Show("Foutieve wachtwoord.");
                    return;
                }

            }
            else
            {
                lbl_Status.Visibility = Visibility.Visible;
            }
        }

        private bool CheckUserExists(string username)
        {
            return objectList.Any(gebruiker => gebruiker.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            canvas_load.Visibility = Visibility.Visible;
            lbl_prsbr.Content = $"{value}%";
            prsbar.Value = value;
            value = value + 10;
            if (value >= 120)
            {
                i = 0;
                value = 0;
                canvas_load.Visibility = Visibility.Collapsed;
                canvas_Start.Visibility = Visibility.Visible;
                lbl_com.Visibility = Visibility.Visible;
                timer.Stop();
                load = true;
            }
            

            if (load == true)
            {
                i = 0;
                value = 0;
                count ++;
                canvas_load.Visibility = Visibility.Collapsed;
                canvas_Start.Visibility = Visibility.Visible;
                lbl_com.Content = 
                lbl_com.Visibility = Visibility.Visible;
                timer.Stop();
                

            }
            
        }

       
        

        private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            lbl_com.Visibility = Visibility.Visible;
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                if (cbxComPorts.SelectedItem.ToString() == "None")
                {
                    lbl_com.Visibility = Visibility.Visible;
                    camvas_Cntrols.Visibility = Visibility.Collapsed;
                }

                if (cbxComPorts.SelectedItem.ToString() != "None" && cbxComPorts.SelectedItem.ToString() != " ")
                {
                    _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
                    if (!PortExists(_serialPort.PortName))
                    {
                        lbl_com.Content = $"Deze COM-Poort is niet beschikbaar! ";
                        camvas_Cntrols.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        lbl_com.Visibility = Visibility.Collapsed;
                        camvas_Cntrols.Visibility = Visibility.Visible;
                        _serialPort.Open();

                    }
                    

                }
                


            }
        }

        private void ComboBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            canvas_load.Visibility = Visibility.Collapsed;
            foreach (string s in SerialPort.GetPortNames())
            {
                if (!cbxComPorts.Items.Contains(s))
                {
                    cbxComPorts.Items.Add(s);
                }
            }
               
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((_serialPort != null) && _serialPort.IsOpen)
            {
                _serialPort.Write(new byte[] { 0 }, 0, 1);
                _serialPort.Dispose();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            canvas_Inloggen.Visibility = Visibility.Visible;
            canvas_Start.Visibility = Visibility.Collapsed;
            camvas_Cntrols.Visibility= Visibility.Collapsed;
            canvas_User_info.Visibility = Visibility.Collapsed;
            canvas_load.Visibility = Visibility.Collapsed;
            btn_inloggen.Visibility = Visibility.Visible;
            btn_aanmelden.Visibility = Visibility.Visible;
            LijnA.Visibility = Visibility.Collapsed;
            LijnI.Visibility = Visibility.Visible;

            Color myColor = (Color)ColorConverter.ConvertFromString("#3AA1DB"); 
            BorderBrush = new SolidColorBrush(myColor);
            BorderThickness = new Thickness(2);

        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider1.Value == 1) { BOL1g.Visibility = Visibility.Visible; BOL1r.Visibility = Visibility.Collapsed; leds |= 0b00000001; }
            if (slider2.Value == 1) { BOL2g.Visibility = Visibility.Visible; BOL2r.Visibility = Visibility.Collapsed; leds |= 0b00000010; }
            if (slider3.Value == 1) { BOL3g.Visibility = Visibility.Visible; BOL3r.Visibility = Visibility.Collapsed; leds |= 0b00000100; }
            if (slider4.Value == 1) { BOL4g.Visibility = Visibility.Visible; BOL4r.Visibility = Visibility.Collapsed; leds |= 0b00001000; }
            if (slider5.Value == 1) { BOL5g.Visibility = Visibility.Visible; BOL5r.Visibility = Visibility.Collapsed; leds |= 0b00010000; }

            if (slider1.Value == 0) { BOL1g.Visibility = Visibility.Collapsed; BOL1r.Visibility = Visibility.Visible; leds &= 0b11111110; }
            if (slider2.Value == 0) { BOL2g.Visibility = Visibility.Collapsed; BOL2r.Visibility = Visibility.Visible; leds &= 0b11111101; }
            if (slider3.Value == 0) { BOL3g.Visibility = Visibility.Collapsed; BOL3r.Visibility = Visibility.Visible; leds &= 0b11111011; }
            if (slider4.Value == 0) { BOL4g.Visibility = Visibility.Collapsed; BOL4r.Visibility = Visibility.Visible; leds &= 0b11110111; }
            if (slider5.Value == 0) { BOL5g.Visibility = Visibility.Collapsed; BOL5r.Visibility = Visibility.Visible; leds &= 0b11101111; }
            

            _serialPort_DataSend(leds);
           
            
        }

        private void canvas_User_info_MouseLeave(object sender, MouseEventArgs e)
        {
            canvas_User_info.Visibility = Visibility.Collapsed;
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            canvas_User_info.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            canvas_Inloggen.Visibility = Visibility.Visible;
            canvas_Start.Visibility = Visibility.Collapsed;
            camvas_Cntrols.Visibility = Visibility.Collapsed;
            canvas_User_info.Visibility = Visibility.Collapsed;
            canvas_load.Visibility = Visibility.Collapsed;
            txt_Password.Text = "Password";
            txt_Username.Text = "Username";


            btn_inloggen.Visibility = Visibility.Visible;
            btn_aanmelden.Visibility = Visibility.Visible;
            canvas_Aanmelden.Visibility = Visibility.Collapsed;
            LijnA.Visibility = Visibility.Collapsed;
            LijnI.Visibility = Visibility.Visible;

            txt_voornaam.Text = "";
            txt_achternaam.Text = "";
            txt_leeftijd.Text = "";
            txt_username.Text = "";
            txt_pasword.Text = "";

            lbl_com.Visibility = Visibility.Visible;
            cbxComPorts.SelectedItem = "None";

        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string receivedText = _serialPort.ReadLine().Trim();


            // Splits de ontvangen string op basis van het kommateken
            string[] waarden = receivedText.Split(',');

            if (waarden.Length == 2)
            {

                if (int.TryParse(waarden[0], out int waarde1))
                {
                    Dispatcher.Invoke(new Action<int>(UpdateLeds), waarde1);
                }

                if (int.TryParse(waarden[1], out int waarde2))
                {
                    Dispatcher.Invoke(new Action<double>(UpdateADC), waarde2);
                }
            }



        }

        private void UpdateADC(double value)
        {

            double procent = (value * 1000) / 2510;
            ADC_Value.Content = $"{procent:0.00}% ({value})";
            levelProgressBar.Value = procent;
        }

        private void UpdateLeds(int text)
        {


            switch (text)
            {
                case 1:
                    {
                        KNOP_1A.Visibility = Visibility.Visible;
                        KNOP_2A.Visibility = Visibility.Collapsed;
                        KNOP_3A.Visibility = Visibility.Collapsed;
                        KNOP_4A.Visibility = Visibility.Collapsed;

                    }
                    break;
                case 2:
                    {

                        KNOP_2A.Visibility = Visibility.Visible;
                        KNOP_1A.Visibility = Visibility.Collapsed;
                        KNOP_3A.Visibility = Visibility.Collapsed;
                        KNOP_4A.Visibility = Visibility.Collapsed;

                    }
                    break;
                case 3:
                    {

                        KNOP_3A.Visibility = Visibility.Visible;
                        KNOP_1A.Visibility = Visibility.Collapsed;
                        KNOP_2A.Visibility = Visibility.Collapsed;
                        KNOP_4A.Visibility = Visibility.Collapsed;

                    }
                    break;
                case 4:
                    {

                        KNOP_4A.Visibility = Visibility.Visible;
                        KNOP_1A.Visibility = Visibility.Collapsed;
                        KNOP_3A.Visibility = Visibility.Collapsed;
                        KNOP_2A.Visibility = Visibility.Collapsed;
                    }
                    break;
                default:
                    {
                        KNOP_1A.Visibility = Visibility.Collapsed;
                        KNOP_2A.Visibility = Visibility.Collapsed;
                        KNOP_3A.Visibility = Visibility.Collapsed;
                        KNOP_4A.Visibility = Visibility.Collapsed;
                    }
                    break;

            }
        }

        static bool PortExists(string portName)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                if (port == portName)
                {
                    return true; // De opgegeven poortnaam komt overeen met een beschikbare poort
                }
            }
            return false; // De opgegeven poortnaam komt niet overeen met een beschikbare poort
        }

        private void _serialPort_DataSend(byte leds)
        {
           
            Debug.WriteLine(leds);
            _serialPort.Write(new byte[] { leds}, 0, 1);
            

        }

        private void btn_buz_Click(object sender, RoutedEventArgs e)
        {
            
            leds |= 0b10000000;
            _serialPort_DataSend(leds);
            leds &= 0b01111111;

        }

        private void btn_rechts_Click(object sender, RoutedEventArgs e)
        {
            leds &= 0b00011111;
            leds |= 0b00100000;
            _serialPort_DataSend(leds);
            
        }

        private void btn_Links_Click(object sender, RoutedEventArgs e)
        {
            leds &= 0b00011111;
            leds |= 0b01000000;
            _serialPort_DataSend(leds);
           
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            leds &= 0b00011111;
            _serialPort_DataSend(leds);
            
        }

        private void btn_change_icoon_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Afbeeldingsbestanden|*.jpg;*.png;*.gif;*.bmp|Alle bestanden|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                

                try
                {
                    
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.EndInit();


                    imageBrush.ImageSource = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij het laden van de afbeelding: " + ex.Message);
                }
            }
        }

        private void btn_inloggen_Click(object sender, RoutedEventArgs e)
        {
            lbl_Status.Visibility = Visibility.Collapsed;
            LijnA.Visibility = Visibility.Collapsed;
            LijnI.Visibility = Visibility.Visible;
            canvas_Inloggen.Visibility = Visibility.Visible;
            canvas_Aanmelden.Visibility = Visibility.Collapsed;
            txt_Username.Text = "Username";
            txt_Password.Text = "Password";
        }

        private void btn_aanmelden_Click(object sender, RoutedEventArgs e)
        {
            LijnA.Visibility = Visibility.Visible;
            LijnI.Visibility = Visibility.Collapsed;
            canvas_Inloggen.Visibility = Visibility.Collapsed;
            canvas_Aanmelden.Visibility = Visibility.Visible;

            txt_voornaam.Text = "";
            txt_achternaam.Text = "";
            txt_leeftijd.Text = "";
            txt_username.Text = "";
            txt_pasword.Text = "";
        }

        private void btn_singUP_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_voornaam.Text) || string.IsNullOrEmpty(txt_achternaam.Text) || string.IsNullOrEmpty(txt_leeftijd.Text) || string.IsNullOrEmpty(txt_username.Text) || string.IsNullOrEmpty(txt_pasword.Text))
            {
                MessageBox.Show("Vul alle velden in AUB.");
                return; // Stop met verder verwerken als de TextBox leeg is

            }
            int age;

            if (!int.TryParse(txt_leeftijd.Text, out age))
            {
                MessageBox.Show("Voer alstublieft een geldig getal in voor leeftijd.");
                return; // Stop met verder verwerken als de ingevoerde waarde geen geldig getal is
            }

            if (age < 0 || age > 100)
            {
                MessageBox.Show("Leeftijd moet tussen 0 en 100 liggen.");
                return; // Stop met verder verwerken als de leeftijd buiten het gewenste bereik ligt
            }


            voornaam = txt_voornaam.Text;
            achternaam = txt_achternaam.Text;
            leeftijd = txt_leeftijd.Text;
            username = txt_username.Text;
            pasword = txt_pasword.Text;

            if (CheckUsernameExists(username) || txt_username.Text == "Username")
            {
                MessageBox.Show("Deze gebruikersnaam bestaat al.");
                return; // Stop met het maken van het object als de gebruikersnaam al bestaat
            }

            Gebruiker nieuwObject = new Gebruiker(voornaam, achternaam, leeftijd, pasword, username);
            objectList.Add(nieuwObject);

            MessageBox.Show("Uw bent aanmelding was succesvol");


            cmbx_gebruikers.Items.Add(nieuwObject.Username);
            txt_voornaam.Text = "";
            txt_achternaam.Text = "";
            txt_leeftijd.Text = "";
            txt_username.Text = "";
            txt_pasword.Text = "";
        }

        private bool CheckUsernameExists(string username)
        {
            return objectList.Any(gebruiker => gebruiker.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        private void StartTimer2()
        {
            timer2.Interval = TimeSpan.FromSeconds(1);

            timer2.Tick += Timer_Tick2;
            timer2.Start();
        }
        private void Timer_Tick2(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            CultureInfo culture = new CultureInfo("nl-NL"); // Specificeer Nederlands (Nederland) cultuurinfo voor Europese notatie

            string formattedDateTime = currentTime.ToString("dd/MM/yyyy HH:mm:ss", culture);
            lbl_time.Content = formattedDateTime;
        }


    }    
}
