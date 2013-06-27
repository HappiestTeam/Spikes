using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace DownloadFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = @"http://www.thereforesystems.com/wp-content/uploads/2008/08/image35.png";
            // Create an instance of WebClient
            WebClient client = new WebClient();
            // Hookup DownloadFileCompleted Event
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

            // Start the download and copy the file to c:\temp
            client.DownloadFileAsync(new Uri(url), @"c:\temp\image35.png");

        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("File downloaded");
            string file = @"c:\temp\image35.png";

            FileInfo oFileInfo = new FileInfo(file);
            MessageBox.Show("My File's Name: \"" + oFileInfo.Name + "\"");
            DateTime dtCreationTime = oFileInfo.CreationTime;
            MessageBox.Show("Date and Time File Created: " + dtCreationTime.ToString());
            MessageBox.Show("myFile Extension: " + oFileInfo.Extension);
            MessageBox.Show("myFile total Size: " + oFileInfo.Length.ToString());
            MessageBox.Show("myFile filepath: " + oFileInfo.DirectoryName);
            MessageBox.Show("My File's Full Name: \"" + oFileInfo.FullName + "\"");

        }
    }
}
