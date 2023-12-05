﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;


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
        DispatcherTimer timer = new DispatcherTimer();
        Gebruiker gebruiker1 = new Gebruiker();
     
        public MainWindow()
        {
            InitializeComponent();
            _serialPort = new SerialPort();
            _serialPort.DataReceived += _serialPort_DataReceived;
            cbxComPorts.Items.Add("None");
            foreach (string s in SerialPort.GetPortNames())
                cbxComPorts.Items.Add(s);

            gebruiker1.Voornaam = "Voornaam: Ramsey";
            gebruiker1.Achternaam = "Naam: Carpentier";
            gebruiker1.Username = "Username: Ramsey_Carpentier";
            gebruiker1.Geboortedatum = "Geboortedatum: 21/06/2005";
            gebruiker1.Email = "E-mail: ramsey.carpentier@student.vives.be";
            gebruiker1.Paswoord = "Paswoord: Vives123";

            user2.Content = gebruiker1.Voornaam;
            user1.Content = gebruiker1.Achternaam;
            user3.Content = gebruiker1.Username;
            user4.Content = gebruiker1.Geboortedatum;
            user5.Content = gebruiker1.Email;
            user6.Content = gebruiker1.Paswoord;
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
            lbl_user_inc.Visibility = Visibility.Collapsed;
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
            lbl_pass_inc.Visibility = Visibility.Collapsed;
            if (txt_Password.Text == "Password")
            {
                txt_Password.Text = "";
            }
        }

        private  void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            string username = txt_Username.Text;
            string password = txt_Password.Text;
            if ((username == "Ramsey_Carpentier" || username == "Thibobeer" || username == "Michel" || username == "Floflo") && password == "Vives123")
            {
                canvas_Inloggen.Visibility = Visibility.Collapsed;
                
                lbl_naam.Content = username;

                
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Tick += Timer_Tick;
                timer.Start();



            }

            

            else
            {
                if ((username != "Ramsey_Carpentier" && username != "Thibobeer" && username != "Michel" && username != "Floflo"))
                    lbl_user_inc.Visibility = Visibility.Visible;


                if (password != "Vives123")
                    lbl_pass_inc.Visibility = Visibility.Visible;
            }


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
                canvas_load.Visibility = Visibility.Collapsed;
                canvas_Start.Visibility = Visibility.Visible;
                lbl_com.Visibility = Visibility.Visible;
                timer.Stop();
                Thread.Sleep(1000);
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
                    bitmap.DecodePixelHeight = 80; 
                    bitmap.DecodePixelWidth = 80; 
                    bitmap.EndInit();

                    
                    lbl_start2.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij het laden van de afbeelding: " + ex.Message);
                }
            }
        }
    }    
}
