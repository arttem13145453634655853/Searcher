using GMap.NET.MapProviders;
using Microsoft.VisualBasic.Logging;
using System.Net.Http.Headers;
using System;
using System.Security.Policy;
using Microsoft.VisualBasic.ApplicationServices;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Stend
{
    public partial class Searcher : Form
    {
        const string m_url = "https://a6a6-192-162-250-97.ngrok-free.app/process-images";
        const string m_source = "../../../../source/";

        HttpClient m_client;
        public Searcher()
        {
            InitializeComponent();

            m_client = new HttpClient();

            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.MapProvider = GMapProviders.GoogleSatelliteMap;
            gMapControl1.Position = new GMap.NET.PointLatLng(35, 139);
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            gMapControl1.ShowCenter = false;
            gMapControl1.ShowTileGridLines = false;

            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 30;
            gMapControl1.Zoom = 15;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String lasStr = textBox1.Text;
            String lngStr = textBox2.Text;
            if (lasStr == "" || lngStr == "")
                return;

            double las = Convert.ToDouble(lasStr);
            double lng = Convert.ToDouble(textBox2.Text);

            gMapControl1.Position = new GMap.NET.PointLatLng(las, lng);

            gMapControl1.Zoom = 15;
        }

        private async void gMapControl1_Scroll(object sender, ScrollEventArgs e)
        {
            double currentZoom = gMapControl1.Zoom;

            if (currentZoom < 6)
                gMapControl1.Zoom = 6;

            if (currentZoom > 49)
                gMapControl1.Zoom = 49;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new MultipartFormDataContent())
                {

                    string imagePath1 = m_source + "map_im.jpg";
                    string imagePath2 = m_source + "satelite_im.jpg";

                    var mapImageContent = new ByteArrayContent(File.ReadAllBytes(imagePath1));
                    mapImageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                    form.Add(mapImageContent, "map_image", imagePath1);

                    var satelliteImageContent = new ByteArrayContent(File.ReadAllBytes(imagePath2));
                    satelliteImageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                    form.Add(satelliteImageContent, "satellite_image", imagePath2);

                    HttpResponseMessage response = await m_client.PostAsync(m_url, form);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] resultContent = await response.Content.ReadAsByteArrayAsync();
                        File.WriteAllBytes(m_source + "result.jpg", resultContent);
                        Console.WriteLine("Результат успешно сохранён в 'result.jpg'");
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Ошибка: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = true;
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string outputFilePath = m_source + (trackBar1.Value == 1 ? "map_im.jpg" : "satelite_im.jpg");
            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            File.Copy(openFileDialog1.FileName, outputFilePath);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var bitmap = new Bitmap(gMapControl1.Width, gMapControl1.Height - 20);

            Graphics graphics = Graphics.FromImage(bitmap as Image);
            graphics.CopyFromScreen(this.Location.X + 21, this.Location.Y + 50, 0, 0, bitmap.Size);

            Image im = bitmap;
            im.Save(m_source + (trackBar1.Value == 1 ? "map_im.jpg" : "satelite_im.jpg"));
        }
    }
} 