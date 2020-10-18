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
        private static int[,] unitArr = new int[64, 64];
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
                        Console.WriteLine("Connected");
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
        public static byte[] ReceiveCallback()
        {
            var buffer = new List<byte>();
            while(_clientSocket.Available > 0)
            {
                var currByte = new Byte[1];
                var byteCounter = _clientSocket.Receive(currByte, currByte.Length, SocketFlags.None);

                if (byteCounter.Equals(1))
                {
                    buffer.Add(currByte[0]);
                }
            }
            return buffer.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextBox username = this.username;
            if (username.Text.Length > 0)
            {
                StartConnect(username.Text);
                byte[] someArr = ReceiveCallback();
                Console.WriteLine("sOME ABYTE ARR " + someArr.Length);
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