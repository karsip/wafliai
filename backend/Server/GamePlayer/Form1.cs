using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GamePlayer
{
    public partial class Form1 : Form
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static bool connected = false;
        private static void StartConnect(string username)
        {
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 100);

                    if (_clientSocket.Connected)
                    {
                        connected = true;
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }
        } 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextBox username = this.username;
            if (username.Text.Length > 0)
            {
                StartConnect(username.Text);
                if (connected)
                {
                    this.Hide();
                    Battle battleForm = new Battle(username.Text, _clientSocket);
                    battleForm.ShowDialog();
                }
            }
            else
            {
                string message = "Username cannot be empty!";
                MessageBox.Show(message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}