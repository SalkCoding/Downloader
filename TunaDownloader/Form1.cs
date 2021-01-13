using MaterialSkin.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Media;
using System.Net;
using System.Windows.Forms;

namespace TunaDownloader
{
    public partial class Form1 : MaterialForm
    {
        private WebClient client;
        private bool isDownloading = false;
        private readonly Uri uri = new Uri(@"https://speed.hetzner.de/100MB.bin");
        private readonly string defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\resourcepacks";

        public Form1()
        {
            InitializeComponent();
            folderTextBox.Text = defaultDirectory;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (isDownloading)
            {
                MessageBox.Show("이미 다운로드가 진행 중입니다!", "오류!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string fileLocation = string.Format(@"{0}\{1}.zip", folderTextBox.Text, fileNameTextBox.Text);
            client = new WebClient();
            client.DownloadProgressChanged += DownloadProgressChanged;
            client.DownloadFileCompleted += DownloadCompleted;
            client.DownloadFileAsync(uri, fileLocation);
            isDownloading = true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (!isDownloading)
                return;

            client.DownloadFileCompleted -= DownloadCompleted;
            client.CancelAsync();
            client.Dispose();
            percentageLabel.Text = "0%";
            downloadProgressBar.Value = 0;
            isDownloading = false;
            SystemSounds.Beep.Play();
        }

        private void FolderTextBox_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = defaultDirectory;
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    folderTextBox.Text = dialog.FileName;
                }
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int percentage = e.ProgressPercentage;
            downloadProgressBar.Value = percentage;
            percentageLabel.Text = string.Format("{0:0.##}%", (double)e.BytesReceived / (double)e.TotalBytesToReceive * 100);
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            client.Dispose();
            isDownloading = false;
            SystemSounds.Beep.Play();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
            {
                client.CancelAsync();
                client.Dispose();
            }
        }
    }
}
