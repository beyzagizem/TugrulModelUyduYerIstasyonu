﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.IO;
using System.Net;

namespace Arduino_SerialPortCom1
{
	public partial class Form1 : Form
	{
		float x = 0, y = 0, z = 0;
		bool cx = true, cy = true, cz = true;//gl control
		string currentTime = "00:00:00";//sayac
		int sayac=0;
		int hour;
		int minutes;
		int seconds;

		System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();


		
		public Form1()
		{

			InitializeComponent();
			findConnectedPorts();
		}
		struct FtpSetting
        {
			public string Server { get; set; }	
			public string Name { get; set; }
			public string Password { get; set; }
			public string FileName { get; set; }
			public string FullName { get; set; }
        }
		FtpSetting _inputParameter;

		string data;
		string[] veriler;
		int i = 0, j = 0 ;
		private FilterInfoCollection videoDevices;
		private VideoCaptureDevice videoSource;


		void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			Bitmap originalBitmap = (Bitmap)eventArgs.Frame.Clone();
			Size newSize = new Size((int)(originalBitmap.Width * 0.3), (int)(originalBitmap.Height * 0.3));
			Bitmap image = new Bitmap(originalBitmap, newSize);
			//Bitmap image = (Bitmap)eventArgs.Frame.Clone();
			liveCam.Image = image;
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
			dataGridView1.ColumnCount = 23;
			dataGridView1.RowCount = 100000;
			dataGridView1.Columns[0].Name = "TAKIM NO";
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

		}

		Object lockObject = new object();


		private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{

            lock (lockObject)
            {
                try
                {
					if (serialPort1.IsOpen && serialPort1 != null)
					{
						data = serialPort1.ReadLine();
						this.BeginInvoke(new EventHandler(displaydata));
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
			
			dataGridView1.Rows[i].Cells[j].Value = veriler[0]+ " "; j++;// Takım No
			dataGridView1.Rows[i].Cells[j].Value = veriler[1]+ " "; j++;// Paket No 
			dataGridView1.Rows[i].Cells[j].Value = veriler[2]+ " "; j++; // Gönderme Saati
			dataGridView1.Rows[i].Cells[j].Value = veriler[3]+" "; j++;// Basınç1
			dataGridView1.Rows[i].Cells[j].Value = veriler[4] +" "; j++;// Basınç2
			dataGridView1.Rows[i].Cells[j].Value = veriler[5] +" "; j++;// Yükseklik1 
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
			
			
			


			chart1.Series["Sıcaklık"].Points.AddXY(veriler[2], veriler[9]);
			chart2.Series["Yükseklik2"].Points.AddXY(veriler[2], veriler[9]);
			chart3.Series["Yükseklik1"].Points.AddXY(veriler[2], veriler[9]);
			chart4.Series["Iniş hızı"].Points.AddXY(veriler[2], veriler[9]);
			chart5.Series["Iritifa farki"].Points.AddXY(veriler[2], veriler[9]);
			chart6.Series["Pil gerilimi"].Points.AddXY(veriler[2], veriler[9]);
			chart7.Series["Basınç1"].Points.AddXY(veriler[2], veriler[3]);
			chart8.Series["Basınç2"].Points.AddXY(veriler[2], veriler[4]);

			verileri_listele();
			//veriler = data.Split(',');
			x = float.Parse(veriler[18]);
			Invoke(new Action(() => { textBox22.Text = veriler[18]; }));
			y = float.Parse(veriler[19]);
			Invoke(new Action(() => { textBox30.Text = veriler[19]; }));
			z = float.Parse(veriler[20]);
			Invoke(new Action(() => { textBox29.Text = veriler[20]; }));

			glControl1.Invalidate();



			Invoke(new Action(() => { GPS1Longtitude.Text = veriler[12]; }));
			Invoke(new Action(() => { GPS1Latitude.Text = veriler[11]; }));
			Invoke(new Action(() => { GPS1Altitude.Text = veriler[13]; }));
			Invoke(new Action(() => { GPS2Longtitude.Text = veriler[15]; }));
			Invoke(new Action(() => { GPS2Latitude.Text = veriler[14]; }));
			Invoke(new Action(() => { GPS2Altitude.Text = veriler[16]; }));
			Invoke(new Action(() => { upTime.Text = currentTime; }));

			if (!(String.IsNullOrEmpty(GPS1Latitude.Text) || String.IsNullOrEmpty(GPS1Longtitude.Text)))
			{
				MapTasiyici.MapProvider = GMapProviders.GoogleMap;
				double lat1 = Convert.ToDouble(veriler[11].Replace(".", ","));//Taşıyıcı Gps modülünden gelen verileri textbox a çekicez
				double lng1 = Convert.ToDouble(veriler[12].Replace(".", ","));
				Invoke(new Action(() => { MapTasiyici.Position = new GMap.NET.PointLatLng(lat1, lng1); }));
				Invoke(new Action(() => { MapTasiyici.MinZoom = 10; }));
				Invoke(new Action(() => { MapTasiyici.MaxZoom = 100; }));
				Invoke(new Action(() => { MapTasiyici.Zoom = 10; }));
			}


			if (!(String.IsNullOrEmpty(GPS2Latitude.Text) || String.IsNullOrEmpty(GPS2Longtitude.Text)))
			{
				MapGorev.MapProvider = GMapProviders.GoogleMap;
				double lat = Convert.ToDouble(veriler[14].Replace(".", ","));//  Görev yükü Gps modülünden gelen verileri textbox a çekicez
				double lng = Convert.ToDouble(veriler[15].Replace(".", ","));
				Invoke(new Action(() => { MapGorev.Position = new GMap.NET.PointLatLng(lat, lng); }));
				Invoke(new Action(() => { MapGorev.MinZoom = 10; }));
				Invoke(new Action(() => { MapGorev.MaxZoom = 100; }));
				Invoke(new Action(() => { MapGorev.Zoom = 10; }));
			}
			sayac++;
			hour = sayac / 3600;
			minutes = (sayac / 60) % 60;
			seconds = sayac % 60;
			currentTime = hour.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");

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
			sayac =0;
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
			veriler =null;

			chart1.Series["Sıcaklık"].Points.Clear();
			chart2.Series["Yükseklik2"].Points.Clear();
			chart3.Series["Yükseklik1"].Points.Clear();
			chart4.Series["Iniş hızı"].Points.Clear();
			chart5.Series["Iritifa farki"].Points.Clear();
			chart6.Series["Pil gerilimi"].Points.Clear();
			chart7.Series["Basınç1"].Points.Clear();
			chart8.Series["Basınç2"].Points.Clear();

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			videoSource.Stop();
			timer1.Stop();
			if (serialPort1.IsOpen == true)
			{
				serialPort1.Close();
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (MessageBox.Show("Çıkmak istediğinize emin misiniz?", null,
				MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Cancel = true;

			}
			

			videoSource.Stop();
			timer1.Stop();
			if (serialPort1.IsOpen == true)
			{
				serialPort1.Close();
			}
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
			float dikey1 = radius, dikey2 = -radius;
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

			GL.Rotate(x, 1.0, 0.0, 0.0);//ÖNEMLİ
			GL.Rotate(z, 0.0, 1.0, 0.0);
			GL.Rotate(y, 0.0, 0.0, 1.0);

			silindir(step, topla, radius, 3, -5);
			silindir(0.01f, topla, 0.5f, 9, 9.7f);
			silindir(0.01f, topla, 0.1f, 5, dikey1 + 5);
			koni(0.01f, 0.01f, radius, 3.0f, 3, 5);
			koni(0.01f, 0.01f, radius, 2.0f, -5.0f, -10.0f);
			Pervane(9.0f, 11.0f, 0.2f, 0.5f);

			GL.Begin(BeginMode.Lines);

			GL.Color3(Color.FromArgb(250, 0, 0));
			GL.Vertex3(-30.0, 0.0, 0.0);
			GL.Vertex3(30.0, 0.0, 0.0);


			GL.Color3(Color.FromArgb(0, 0, 0));
			GL.Vertex3(0.0, 30.0, 0.0);
			GL.Vertex3(0.0, -30.0, 0.0);

			GL.Color3(Color.FromArgb(0, 0, 250));
			GL.Vertex3(0.0, 0.0, 30.0);
			GL.Vertex3(0.0, 0.0, -30.0);

			GL.End();
			//GraphicsContext.CurrentContext.VSync = true;
			glControl1.SwapBuffers();
		}

      

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
			string FileName=((FtpSetting)e.Argument).FileName;
			string name = ((FtpSetting)e.Argument).Name;
			string password = ((FtpSetting)e.Argument).Password;
			string server = ((FtpSetting)e.Argument).Server;
			string fullname = ((FtpSetting)e.Argument).FullName;

			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", server, FileName)));
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials=new NetworkCredential(name, password);
			Stream ftpstream =request.GetRequestStream();
			FileStream fs = File.OpenRead(Name);
			byte[] buffer = new byte[1024];
			double total=(double)fs.Length;
			int bytesRead = 0;
			double read = 0;
			do
			{
                if (!backgroundWorker.CancellationPending)
                {
					bytesRead=fs.Read(buffer, 0, buffer.Length);//
					ftpstream.Write(buffer, 0, bytesRead);
					read+=(double)bytesRead;

					double percentage=read/total*100;
					backgroundWorker.ReportProgress((int)percentage);
                }
			}
			while (bytesRead != 0);
			fs.Close();	
			ftpstream.Close();
		}

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
			
			progressBar1.Value = e.ProgressPercentage;
			progressBar1.Update();

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
			
        }

        private void button5_Click(object sender, EventArgs e)
        {
			using(OpenFileDialog ofd=new OpenFileDialog() { Multiselect=false,ValidateNames=true,Filter= "All files| *.*" })
            {
				if(ofd.ShowDialog() == DialogResult.OK)
                {
					 FileInfo fi = new FileInfo(ofd.FileName);
					_inputParameter.Name=textBoxName.Text;
					_inputParameter.Password = textBoxPass.Text;
					_inputParameter.Server = textBoxServer.Text;
					_inputParameter.FullName = fi.FullName;
					_inputParameter.FileName = fi.Name;


				}
            }
        }

        

        private void glControl1_Load(object sender, EventArgs e)
        {
			GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			GL.Enable(EnableCap.DepthTest);//sonradan yazdık
		}
		private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
		{
			float eski_step = 0.1f;
			GL.Begin(BeginMode.Quads);//Y EKSEN CIZIM DAİRENİN
			while (step <= 360)
			{
				if (step < 45)
					GL.Color3(Color.FromArgb(255, 0, 0));
				else if (step < 90)
					GL.Color3(Color.FromArgb(255, 255, 255));
				else if (step < 135)
					GL.Color3(Color.FromArgb(255, 0, 0));
				else if (step < 180)
					GL.Color3(Color.FromArgb(255, 255, 255));
				else if (step < 225)
					GL.Color3(Color.FromArgb(255, 0, 0));
				else if (step < 270)
					GL.Color3(Color.FromArgb(255, 255, 255));
				else if (step < 315)
					GL.Color3(Color.FromArgb(255, 0, 0));
				else if (step < 360)
					GL.Color3(Color.FromArgb(255, 255, 255));


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
				if (step < 45)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 90)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 135)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 180)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 225)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 270)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 315)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 360)
					GL.Color3(Color.FromArgb(250, 250, 200));


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
				if (step < 45)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 90)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 135)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 180)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 225)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 270)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 315)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 360)
					GL.Color3(Color.FromArgb(250, 250, 200));

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
				if (step < 45)
					GL.Color3(1.0, 1.0, 1.0);
				else if (step < 90)
					GL.Color3(1.0, 0.0, 0.0);
				else if (step < 135)
					GL.Color3(1.0, 1.0, 1.0);
				else if (step < 180)
					GL.Color3(1.0, 0.0, 0.0);
				else if (step < 225)
					GL.Color3(1.0, 1.0, 1.0);
				else if (step < 270)
					GL.Color3(1.0, 0.0, 0.0);
				else if (step < 315)
					GL.Color3(1.0, 1.0, 1.0);
				else if (step < 360)
					GL.Color3(1.0, 0.0, 0.0);


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
				if (step < 45)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 90)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 135)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 180)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 225)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 270)
					GL.Color3(Color.FromArgb(250, 250, 200));
				else if (step < 315)
					GL.Color3(Color.FromArgb(255, 1, 1));
				else if (step < 360)
					GL.Color3(Color.FromArgb(250, 250, 200));


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

			GL.Color3(Color.Red);
			GL.Vertex3(uzunluk, yukseklik, kalinlik);
			GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
			GL.Vertex3(0.0, yukseklik + egiklik, -kalinlik);
			GL.Vertex3(0.0, yukseklik, kalinlik);

			GL.Color3(Color.Red);
			GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
			GL.Vertex3(-uzunluk, yukseklik, -kalinlik);
			GL.Vertex3(0.0, yukseklik, -kalinlik);
			GL.Vertex3(0.0, yukseklik + egiklik, kalinlik);

			GL.Color3(Color.White);
			GL.Vertex3(kalinlik, yukseklik, -uzunluk);
			GL.Vertex3(-kalinlik, yukseklik + egiklik, -uzunluk);
			GL.Vertex3(-kalinlik, yukseklik + egiklik, 0.0);//+
			GL.Vertex3(kalinlik, yukseklik, 0.0);//-

			GL.Color3(Color.White);
			GL.Vertex3(kalinlik, yukseklik + egiklik, +uzunluk);
			GL.Vertex3(-kalinlik, yukseklik, +uzunluk);
			GL.Vertex3(-kalinlik, yukseklik, 0.0);
			GL.Vertex3(kalinlik, yukseklik + egiklik, 0.0);
			GL.End();

		}
		
	}
}