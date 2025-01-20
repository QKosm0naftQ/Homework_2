using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Local_User
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        string selectedFilePathPhoto;
        HttpClient client;
        private async void bt_enterPort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new HttpClient() { BaseAddress = new Uri($"http://localhost:{this.tb_port.Text}") };
                HttpResponseMessage tmp_respons = await client.PostAsync("api/Galleries/upload", null);
                if (tmp_respons.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    this.bt_enterPort.IsEnabled = false;
                    this.tb_port.IsEnabled = false;
                    this.bt_selectPicture.IsEnabled = true;
                    this.bt_sendPhoto.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Error port status code:"+tmp_respons.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bt_selectPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Виберіть зображення",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePathPhoto = openFileDialog.FileName;
            }
        }

        private async void bt_sendPhoto_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = File.ReadAllBytes(selectedFilePathPhoto);
            string base64ЗPhoto = Convert.ToBase64String(imageBytes);

            var uploadData = new { Photo = $"data:image/jpeg;base64,{base64ЗPhoto}" };
            var content = new StringContent(JsonSerializer.Serialize(uploadData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("api/Galleries/upload", content);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var imageUrl = JsonDocument.Parse(responseData).RootElement.GetProperty("image").GetString();

                string fullUrl = $"http://localhost:{this.tb_port.Text}{imageUrl}";
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(fullUrl);
                bitmapImage.EndInit();
                this.picture.Source = bitmapImage;

            }
        }
    }
}