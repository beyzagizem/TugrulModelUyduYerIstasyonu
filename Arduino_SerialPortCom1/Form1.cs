using Accord.Video.FFMPEG;
using Accord.Video.VFW;
using AForge.Video;
using AForge.Video.DirectShow;
using GMap.NET.MapProviders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Arduino_SerialPortCom1
{

    public partial class Form1 : Form
    {

        private float x = 0, y = 0, z = 0;
        private readonly bool cx = true, cy = true, cz = true;//gl control
        Color renk1 = Color.White, renk2 = Color.Red;
        private string currentTime = "00:00:00";//sayac
        private int sayac = 0;
        private int hour;
        private int minutes;
        private int seconds;
        private readonly SaveFileDialog saveAvi;
        private readonly AVIWriter AVIwriter = new AVIWriter();
        private readonly Bitmap video;
        private bool isrecording = false;
        private readonly string assemblyPath;
        private readonly string assemblyParentPath;
        private readonly string dosya_oku;
        private readonly VideoFileWriter FileWriter = new VideoFileWriter();
        private System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
        private System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
        private readonly Microsoft.Office.Interop.Excel.Application excelApp;
        private readonly Microsoft.Office.Interop.Excel._Worksheet workSheet;


        public Form1()
        {
            assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            assemblyParentPath = Path.GetDirectoryName(assemblyPath);
            dosya_oku = Directory.GetParent(Directory.GetParent(assemblyParentPath).ToString()).ToString() + "\\records\\Tugrul_B.xls";
            excelApp = new Microsoft.Office.Interop.Excel.Application();

            excelApp.Workbooks.Add();
            workSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.ActiveSheet;


            InitializeComponent();
            findConnectedPorts();
        }

        private string data;
        private string[] veriler;
        private int i = 0, j = 0;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = new VideoCaptureDevice();

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap video = (Bitmap)eventArgs.Frame.Clone();
            Size newSize = new Size((int)(video.Width * 0.675), (int)(video.Height * 0.711));
            Bitmap image = new Bitmap(video, newSize);
            //Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            liveCam.Image = image;
            if (isrecording)
            {

                FileWriter.WriteVideoFrame(video);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            try
            {
                timer1.Start();
                serialPort1.PortName = COMPort_cmbBox.Text;
                serialPort1.BaudRate = Convert.ToInt32(baudRate_cmbBox.Text);
                serialPort1.DtrEnable = true;
                if (serialPort1.IsOpen == false)
                {
                    serialPort1.Open();
                }

                button1.Visible = false;
                button9.Visible = true;
                ConnectedStatus.Visible = true;
                DisconnectStatus.Visible = false;
                if (videoSource.IsRunning)
                {
                    videoSource.Stop();
                    liveCam.Image = null;
                    liveCam.Invalidate();

                }
                else
                {
                    videoSource = new VideoCaptureDevice(videoDevices[chooseCameraBox.SelectedIndex].MonikerString);
                    videoSource.NewFrame += videoSource_NewFrame;
                    videoSource.Start();




                    Bitmap k = video;
                    int h = 480;
                    int w = 640;

                    string path = Directory.GetParent(Directory.GetParent(assemblyParentPath).ToString()).ToString() + "\\records\\camrecord.avi";
                    // AVIwriter.Open(path, w, h);
                    FileWriter.Open(path, w, h, 25, VideoCodec.Default, 5000000);
                    isrecording = true;





                }
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public void findConnectedPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                COMPort_cmbBox.Items.Add(port);
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {

            workSheet.Cells[1, "A"] = "TAKıM NO";
            workSheet.Cells[1, "B"] = "PAKET NO";
            workSheet.Cells[1, "C"] = "GÖNDERME ZAMANI";
            workSheet.Cells[1, "D"] = "BASINÇ1";
            workSheet.Cells[1, "E"] = "BASINÇ2";
            workSheet.Cells[1, "F"] = "YÜKSEKLİK1";
            workSheet.Cells[1, "G"] = "YÜKSEKLİK2";
            workSheet.Cells[1, "H"] = "İRTİFA FARKI";
            workSheet.Cells[1, "I"] = "İNİŞ HIZI";
            workSheet.Cells[1, "J"] = "SICAKLIK";
            workSheet.Cells[1, "K"] = "PİL GERİLİMİ";
            workSheet.Cells[1, "L"] = "GPS1 LATİTUDE";
            workSheet.Cells[1, "M"] = "GPS1 LONGTİTUDE";
            workSheet.Cells[1, "N"] = "GPS1 ALTİTUDE";
            workSheet.Cells[1, "O"] = "GPS2 LATİTUDE";
            workSheet.Cells[1, "P"] = "GPS2 LONGTİTUDE";
            workSheet.Cells[1, "Q"] = "GPS2 ALTİTUDE";
            workSheet.Cells[1, "R"] = "UYDU STATÜSÜ";
            workSheet.Cells[1, "S"] = "PITCH";
            workSheet.Cells[1, "T"] = "ROLL";
            workSheet.Cells[1, "U"] = "YAW";
            workSheet.Cells[1, "V"] = "DONÜŞ SAYISI";
            workSheet.Cells[1, "W"] = "VİDEO AKTARIM BİLGİSİ";




            dataGridView1.ColumnCount = 23;
            dataGridView1.RowCount = 100000;
            dataGridView1.Columns[0].Name = "TAKıM NO";
            dataGridView1.Columns[1].Name = "PAKET NO";
            dataGridView1.Columns[2].Name = "GÖNDERME ZAMANI";
            dataGridView1.Columns[3].Name = "BASINÇ1";
            dataGridView1.Columns[4].Name = "BASINÇ2";
            dataGridView1.Columns[5].Name = "YÜKSEKLİK1";
            dataGridView1.Columns[6].Name = "YÜKSEKLİK2";
            dataGridView1.Columns[7].Name = "İRTİFA FARKI";
            dataGridView1.Columns[8].Name = "İNİŞ HIZI";
            dataGridView1.Columns[9].Name = "SICAKLIK";
            dataGridView1.Columns[10].Name = "PİL GERİLİMİ";
            dataGridView1.Columns[11].Name = "GPS1 LATİTUDE";
            dataGridView1.Columns[12].Name = "GPS1 LONGTİTUDE";
            dataGridView1.Columns[13].Name = "GPS1 ALTİTUDE";
            dataGridView1.Columns[14].Name = "GPS2 LATİTUDE";
            dataGridView1.Columns[15].Name = "GPS2 LONGTİTUDE";
            dataGridView1.Columns[16].Name = "GPS2 ALTİTUDE";
            dataGridView1.Columns[17].Name = "UYDU STATÜSÜ";
            dataGridView1.Columns[18].Name = "PITCH";
            dataGridView1.Columns[19].Name = "ROLL";
            dataGridView1.Columns[20].Name = "YAW";
            dataGridView1.Columns[21].Name = "DONÜŞ SAYISI";
            dataGridView1.Columns[22].Name = "VİDEO AKTARIM BİLGİSİ";


            timer1.Start();
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                chooseCameraBox.Items.Add(device.Name);
            }
            videoSource = new VideoCaptureDevice();

            GL.ClearColor(Color.Pink);
            TimerXYZ.Interval = 1;


            series1 = chart1.Series.Add("Sıcaklık");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart2.Series.Add("Yükseklik2");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart3.Series.Add("Yükseklik1");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart4.Series.Add("Iniş hızı");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart5.Series.Add("Iritifa farki");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart6.Series.Add("Pil gerilimi");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart7.Series.Add("Basınç1");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart8.Series.Add("Basınç2");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series1 = chart9.Series.Add("Altitude");
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = Color.Black;
            series2 = chart9.Series.Add("Altitude2");
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = Color.Black;

            Uri result = new Uri("http://192.168.4.1", UriKind.Absolute);
            // (Uri)new WebBrowserUriTypeConverter().ConvertFromString("https://www.google.com/");
            webView21.Source = result;


        }

        private readonly object lockObject = new object();
        private readonly int counter = 0;
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {


            lock (lockObject)
            {
                try
                {
                    if (serialPort1.IsOpen && serialPort1 != null)
                    {
                        if (counter == 1)
                        {
                            TextWriter tw = new StreamWriter(dosya_oku);
                            tw.Write(" ");
                            tw.Close();
                        }


                        data = serialPort1.ReadLine();
                        if (data.Substring(0, 6) == "318842")
                        {
                            BeginInvoke(new EventHandler(displaydata));
                        }


                    }
                }
                catch (Exception)
                {

                    return;
                }

            }


        }

        private void displaydata(object sender, EventArgs e)
        {

            veriler = data.Split(',');
            for (int i = 0; i < veriler.Length; i++)
            {
                if (veriler[i] == null)
                {
                    veriler[i] = "0";
                }
            }

            workSheet.Cells[i + 2, "A"] = veriler[0];
            workSheet.Cells[i + 2, "B"] = veriler[1];
            workSheet.Cells[i + 2, "C"] = veriler[2];
            workSheet.Cells[i + 2, "D"] = veriler[3];
            workSheet.Cells[i + 2, "E"] = veriler[4];
            workSheet.Cells[i + 2, "F"] = veriler[5];
            workSheet.Cells[i + 2, "G"] = veriler[6];
            workSheet.Cells[i + 2, "H"] = veriler[7];
            workSheet.Cells[i + 2, "I"] = veriler[8];
            workSheet.Cells[i + 2, "J"] = veriler[9];
            workSheet.Cells[i + 2, "K"] = veriler[10];
            workSheet.Cells[i + 2, "L"] = veriler[11];
            workSheet.Cells[i + 2, "M"] = veriler[12];
            workSheet.Cells[i + 2, "N"] = veriler[13];
            workSheet.Cells[i + 2, "O"] = veriler[14];
            workSheet.Cells[i + 2, "P"] = veriler[15];
            workSheet.Cells[i + 2, "Q"] = veriler[16];
            workSheet.Cells[i + 2, "R"] = veriler[17];
            workSheet.Cells[i + 2, "S"] = veriler[18];
            workSheet.Cells[i + 2, "T"] = veriler[19];
            workSheet.Cells[i + 2, "U"] = veriler[20];
            workSheet.Cells[i + 2, "V"] = veriler[21];
            workSheet.Cells[i + 2, "W"] = veriler[22];

            dataGridView1.Rows[i].Cells[j].Value = veriler[0] + " "; j++;// Takım No
            dataGridView1.Rows[i].Cells[j].Value = veriler[1] + " "; j++;// Paket No 
            dataGridView1.Rows[i].Cells[j].Value = veriler[2] + " "; j++; // Gönderme Saati
            dataGridView1.Rows[i].Cells[j].Value = veriler[3] + " "; j++;// Basınç1
            dataGridView1.Rows[i].Cells[j].Value = veriler[4] + " "; j++;// Basınç2
            dataGridView1.Rows[i].Cells[j].Value = veriler[5] + " "; j++;// Yükseklik1 
            dataGridView1.Rows[i].Cells[j].Value = veriler[6] + " "; j++; // Yükseklik2
            dataGridView1.Rows[i].Cells[j].Value = veriler[7] + " "; j++;// İrtifa Farkı
            dataGridView1.Rows[i].Cells[j].Value = veriler[8] + " "; j++;// İniş Hızı 
            dataGridView1.Rows[i].Cells[j].Value = veriler[9] + " "; j++;// Sıcaklık
            dataGridView1.Rows[i].Cells[j].Value = veriler[10] + " "; j++;// Pil Gerilimi
            dataGridView1.Rows[i].Cells[j].Value = veriler[11] + " "; j++;// GPS1 Latitude
            dataGridView1.Rows[i].Cells[j].Value = veriler[12] + " "; j++;// GPS1 Longitude
            dataGridView1.Rows[i].Cells[j].Value = veriler[13] + " "; j++;//GPS1 Altitude 
            dataGridView1.Rows[i].Cells[j].Value = veriler[14] + " "; j++; //GPS2 Latitude
            dataGridView1.Rows[i].Cells[j].Value = veriler[15] + " "; j++; //GPS2 Longitude
            dataGridView1.Rows[i].Cells[j].Value = veriler[16] + " "; j++;// GPS2 Altitude
            dataGridView1.Rows[i].Cells[j].Value = veriler[17] + " "; j++;//uydu statüsü
            dataGridView1.Rows[i].Cells[j].Value = veriler[18] + " "; j++;//pitch
            dataGridView1.Rows[i].Cells[j].Value = veriler[19] + " "; j++;//roll
            dataGridView1.Rows[i].Cells[j].Value = veriler[20] + " "; j++;//yaw
            dataGridView1.Rows[i].Cells[j].Value = veriler[21] + " "; j++;//dönüş sayısı
            dataGridView1.Rows[i].Cells[j].Value = veriler[22] + " "; j++;//video aktarım bilgisi

            i++;
            j = 0;
            numberOfVeri.Text = i + "";
            dataGridView1.FirstDisplayedScrollingRowIndex = i - 6 > 0 ? i - 6 : i - 5 > 0 ? i - 5 : i - 4 > 0 ? i - 4 : i - 3 > 0 ? i - 3 : i - 2 > 0 ? i - 2 : i - 1;


            chart1.Series["Sıcaklık"].Points.AddXY(veriler[2], veriler[9]);
            chart2.Series["Yükseklik2"].Points.AddXY(veriler[2], veriler[6]);
            chart3.Series["Yükseklik1"].Points.AddXY(veriler[2], veriler[5]);
            chart4.Series["Iniş hızı"].Points.AddXY(veriler[2], veriler[8]);
            chart5.Series["Iritifa farki"].Points.AddXY(veriler[2], veriler[7]);
            chart6.Series["Pil gerilimi"].Points.AddXY(veriler[2], veriler[10]);
            chart7.Series["Basınç1"].Points.AddXY(veriler[2], veriler[3]);
            chart8.Series["Basınç2"].Points.AddXY(veriler[2], veriler[4]);
            chart9.Series["Altitude"].Points.AddXY(veriler[2], veriler[13]);
            chart9.Series["Altitude2"].Points.AddXY(veriler[2], veriler[16]);

            verileri_listele();
            //veriler = data.Split(',');
            var deneme = 
            
            x = (float)Convert.ToDouble(veriler[18].Replace(".", ",").ToString());
            Invoke(new Action(() => { textBox30.Text = veriler[18]; }));
            y = (float)Convert.ToDouble(veriler[19].Replace(".", ",").ToString());
            Invoke(new Action(() => { textBox29.Text = veriler[19]; }));
            z = (float)Convert.ToDouble(veriler[20].Replace(".", ",").ToString());
            Invoke(new Action(() => { textBox22.Text = veriler[20]; }));

            glControl1.Invalidate();

            File.AppendAllText(dosya_oku, data);

            Invoke(new Action(() => { GPS1Longtitude.Text = veriler[12]; }));
            Invoke(new Action(() => { GPS1Latitude.Text = veriler[11]; }));
            Invoke(new Action(() => { GPS1Altitude.Text = veriler[13]; }));
            Invoke(new Action(() => { GPS2Longtitude.Text = veriler[15]; }));
            Invoke(new Action(() => { GPS2Latitude.Text = veriler[14]; }));
            Invoke(new Action(() => { GPS2Altitude.Text = veriler[16]; }));
            Invoke(new Action(() => { upTime.Text = currentTime; }));

            if (!(string.IsNullOrEmpty(GPS1Latitude.Text) || string.IsNullOrEmpty(GPS1Longtitude.Text)))
            {
                MapTasiyici.MapProvider = GMapProviders.GoogleMap;
                double lat1 = Convert.ToDouble(veriler[11].Replace(".", ","));//Taşıyıcı Gps modülünden gelen verileri textbox a çekicez
                double lng1 = Convert.ToDouble(veriler[12].Replace(".", ","));
                Invoke(new Action(() => { MapTasiyici.Position = new GMap.NET.PointLatLng(lat1, lng1); }));
                Invoke(new Action(() => { MapTasiyici.MinZoom = 10; }));
                Invoke(new Action(() => { MapTasiyici.MaxZoom = 100; }));
                Invoke(new Action(() => { MapTasiyici.Zoom = 10; }));
            }


            if (!(string.IsNullOrEmpty(GPS2Latitude.Text) || string.IsNullOrEmpty(GPS2Longtitude.Text)))
            {
                MapGorev.MapProvider = GMapProviders.GoogleMap;
                float lat = (float) Convert.ToDouble(veriler[14].Replace(".", ",").ToString());//  Görev yükü Gps modülünden gelen verileri textbox a çekicez
                float lng = (float) Convert.ToDouble(veriler[15].Replace(".", ",").ToString());
                Invoke(new Action(() => { MapGorev.Position = new GMap.NET.PointLatLng(lat, lng); }));
                Invoke(new Action(() => { MapGorev.MinZoom = 10; }));
                Invoke(new Action(() => { MapGorev.MaxZoom = 100; }));
                Invoke(new Action(() => { MapGorev.Zoom = 15; }));
            }

            sayac++;
            hour = sayac / 3600;
            minutes = (sayac / 60) % 60;
            seconds = sayac % 60;
            currentTime = hour.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            /*
                        try
                        {

                            x = Convert.ToInt32(veriler[18]);
                            y = Convert.ToInt32(veriler[19]);
                            z = Convert.ToInt32(veriler[20]);
                            glControl1.Invalidate();
                            serialPort1.DiscardInBuffer();

                        }
                        catch
                        {

                        }
            */
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
                serialPort1.Dispose();
                serialPort1 = null;
            }


        }
        private void CloseSerialOnExit()
        {

            try
            {
                serialPort1.DtrEnable = false;
                serialPort1.RtsEnable = false;
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Thread CloseDown = new System.Threading.Thread(new System.Threading.ThreadStart(CloseSerialOnExit));
            CloseDown.Start();
            sayac = 0;
            hour = sayac / 3600;
            minutes = (sayac / 60) % 60;
            seconds = sayac % 60;
            currentTime = hour.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            Invoke(new Action(() => { upTime.Text = currentTime; }));
            button1.Visible = true;
            button9.Visible = false;
            ConnectedStatus.Visible = false;
            DisconnectStatus.Visible = true;
            videoSource.Stop();
            timer1.Stop();
            veriler = null;

            chart1.Series["Sıcaklık"].Points.Clear();
            chart2.Series["Yükseklik2"].Points.Clear();
            chart3.Series["Yükseklik1"].Points.Clear();
            chart4.Series["Iniş hızı"].Points.Clear();
            chart5.Series["Iritifa farki"].Points.Clear();
            chart6.Series["Pil gerilimi"].Points.Clear();
            chart7.Series["Basınç1"].Points.Clear();
            chart8.Series["Basınç2"].Points.Clear();
            chart9.Series["Altitude"].Points.Clear();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            isrecording = false;
            FileWriter.Close();
            videoSource.Stop();
            timer1.Stop();
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }

            // excelApp.Workbooks.Close();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Çıkmak istediğinize emin misiniz?", null,
                                             MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;

            }
            else
            {
                workSheet.SaveAs(Directory.GetParent(Directory.GetParent(assemblyParentPath).ToString()).ToString() + "\\records\\Tugrul_B.xls");
                excelApp.Workbooks.Close();
                isrecording = false;
                FileWriter.Close();
                videoSource.Stop();
                timer1.Stop();
                if (serialPort1.IsOpen == true)
                {
                    serialPort1.Close();
                }
            }
           
            //excelApp.Workbooks.Close();
        }


        private void verileri_listele()
        {

            switch (veriler[17])
            {
                case "0":
                    Invoke(new Action(() => { listView1.Items[0].ForeColor = Color.Green; }));
                    Invoke(new Action(() => { listView1.Items[1].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[2].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[3].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[4].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[5].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[6].ForeColor = Color.Black; }));

                    break;
                case "1":
                    Invoke(new Action(() => { listView1.Items[0].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[1].ForeColor = Color.Green; }));
                    Invoke(new Action(() => { listView1.Items[2].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[3].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[4].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[5].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[6].ForeColor = Color.Black; }));
                    break;
                case "2":
                    Invoke(new Action(() => { listView1.Items[0].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[1].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[2].ForeColor = Color.Green; }));
                    Invoke(new Action(() => { listView1.Items[3].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[4].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[5].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[6].ForeColor = Color.Black; }));
                    break;
                case "3":
                    Invoke(new Action(() => { listView1.Items[0].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[1].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[2].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[3].ForeColor = Color.Green; }));
                    Invoke(new Action(() => { listView1.Items[4].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[5].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[6].ForeColor = Color.Black; }));
                    break;
                case "4":
                    Invoke(new Action(() => { listView1.Items[0].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[1].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[2].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[3].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[4].ForeColor = Color.Green; }));
                    Invoke(new Action(() => { listView1.Items[5].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[6].ForeColor = Color.Black; }));
                    break;

                case "5":
                    Invoke(new Action(() => { listView1.Items[0].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[1].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[2].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[3].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[4].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[5].ForeColor = Color.Green; }));
                    Invoke(new Action(() => { listView1.Items[6].ForeColor = Color.Black; }));
                    break;

                case "6":
                    Invoke(new Action(() => { listView1.Items[0].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[1].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[2].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[3].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[4].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[5].ForeColor = Color.Black; }));
                    Invoke(new Action(() => { listView1.Items[6].ForeColor = Color.Green; }));
                    break;

                default:
                    break;

            }

        }


        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            float step = 1.0f;
            float topla = step;
            float radius = 5.0f;
            //float dikey1 = radius, dikey2 = -radius;
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            

            GL.Rotate(y, 1.0, 0.0, 0.0);//ÖNEMLİ
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(x, 0.0, 0.0, 1.0);

            silindir(step, topla, radius, 3, -5);
            koni(0.01f, 0.01f, radius, 3.0f, 3, 5);//Ust koni
            koni(0.01f, 0.01f, radius, 2.0f, -5.0f, -10.0f);//Alt koni
            silindir(0.01f, topla, 0.07f, 9, 3);// rotor      
            //Pervane(Yükseklik,Pervane Uzunluğu,Pervane Genişliği,Pervane açısı)
            silindir(0.01f, topla, 0.2f, 9, 9.3f);
            Pervane(9.0f, 7.0f, 0.3f, 0.3f);

            silindir(0.01f, topla, 0.2f, 7.3f, 7f);
            Pervane(7.0f, 7.0f, 0.3f, 0.3f);

            GL.Begin(BeginMode.Lines);

            GL.Color3(Color.FromArgb(250, 0, 0));
            GL.Vertex3(-1000, 0, 0);
            GL.Vertex3(1000, 0, 0);

            GL.Color3(Color.FromArgb(25, 150, 100));
            GL.Vertex3(0, 0, -1000);
            GL.Vertex3(0, 0, 1000);

            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(0, 1000, 0);
            GL.Vertex3(0, -1000, 0);

            GL.End();
            //GraphicsContext.CurrentContext.VSync = true;
            glControl1.SwapBuffers();
        }


      
        




        private void button8_Click(object sender, EventArgs e)
        {
            serialPort1.Write("44");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            serialPort1.Write("55");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            serialPort1.Write("66");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            serialPort1.Write("33");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            serialPort1.Write("22");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            isrecording = true;
        }

       
        private void button14_Click(object sender, EventArgs e)
        {
            serialPort1.Write("99");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            serialPort1.Write("00");
        }

       

        private void button16_Click(object sender, EventArgs e)
        {
            serialPort1.Write("66");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            serialPort1.Write("11");
        }

        private void textBox30_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {

        }

        private void MapTasiyici_Load(object sender, EventArgs e)
        {

        }

        private void MapGorev_Load(object sender, EventArgs e)
        {

        }

        private byte[] GetFileData(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                return ASCIIEncoding.ASCII.GetBytes(sr.ReadToEnd());
            }
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
           
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);//sonradan yazdık
        }
        private void renk_ataması(float step)
        {
            if (step < 45)
                GL.Color3(renk2);
            else if (step < 90)
                GL.Color3(renk1);
            else if (step < 135)
                GL.Color3(renk2);
            else if (step < 180)
                GL.Color3(renk1);
            else if (step < 225)
                GL.Color3(renk2);
            else if (step < 270)
                GL.Color3(renk1);
            else if (step < 315)
                GL.Color3(renk2);
            else if (step < 360)
                GL.Color3(renk1);
        }
        private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Quads);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 2) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 2) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey1, ciz1_y);
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            while (step <= 180)//ALT KAPAK
            {
                renk_ataması(step);

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
        }
        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Lines);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius1 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius1 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();

            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            GL.End();
        }
        private void Pervane(float yukseklik, float uzunluk, float kalinlik, float egiklik)
        {
            float radius = 10, angle = 45.0f;
            GL.Begin(BeginMode.Quads);

            GL.Color3(renk2);
            GL.Vertex3(uzunluk, yukseklik, kalinlik);
            GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik, kalinlik);

            GL.Color3(renk2);
            GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
            GL.Vertex3(-uzunluk, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, kalinlik);

            GL.Color3(renk1);
            GL.Vertex3(kalinlik, yukseklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, 0.0);//+
            GL.Vertex3(kalinlik, yukseklik, 0.0);//-

            GL.Color3(renk1);
            GL.Vertex3(kalinlik, yukseklik + egiklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, 0.0);
            GL.Vertex3(kalinlik, yukseklik + egiklik, 0.0);
            GL.End();

        }
    }
}
