using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpriteLibrary;
using System.Threading;
using System.IO;
using ZedGraph;

namespace GACar
{
	public partial class Form1 : Form
	{
		Engine engine = new Engine();
		bool active = false;
		private Random rnd = new Random(DateTime.Now.Millisecond);
		private Thread _thrdAnimate;
		private int _lastSecond = -1;
		private int _compareSecond;
		private int _actualFPS;
		private DateTime _dtStamp;
		private int _frames = 0;
		private DateTime _nextFrameTime;
		private TimeSpan _animationSpan = new TimeSpan(0, 0, 0, 0, 10);
		private Bitmap _buffer = new Bitmap(1, 1);
		private int _width, _height;

		public Form1()
		{
			InitializeComponent();
			_width = this.ClientRectangle.Width;
			_height = this.ClientRectangle.Height;
			engine.SurfaceSize = new Size(_width, _height);
			engine.OffSetX = 0;
			engine.OffSetY = 0;
			engine.Restart();

			this.DoubleBuffered = true;

			active = true;
			//Создание потока для анимации
			_thrdAnimate = new Thread(AnimateProc);
			_thrdAnimate.IsBackground = true;

			//Устанавливаем время следующего обновления кадра
			_nextFrameTime = DateTime.Now;

			//Активация потока
			_thrdAnimate.Start();

			//DrawGraphs();
		}

		protected void AnimateProc()
		{
			while (active)
			{
				//Рендеринг кадра
				if (DateTime.Now > _nextFrameTime)
				{
					_nextFrameTime = DateTime.Now + _animationSpan;

					engine.Animate();

					//Перерисовка формы
					Invalidate();
					//force_chart.Invalidate();
				}
				else
					Thread.Sleep(5);
			}
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			//Изменяет размер буфера
			if (_buffer.Width != _width)
				_buffer = new Bitmap(_width, _height);

			try
			{
				//Создаем графический объект из буфера
				Graphics grBuffer = Graphics.FromImage(_buffer);
				grBuffer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

				//Создаем графический объект из рабочей области
				Graphics grSurface = e.Graphics;

				using (grBuffer)
				{
					grBuffer.Clear(Color.Transparent);
					//Прорисовка спрайтов
					Rectangle viewport = new Rectangle(engine.OffSetX, engine.OffSetY, _width, _height);

					lock (engine.Sprites)
					{
						foreach (Sprite sprite in engine.Sprites)
							if (sprite.Bounds.IntersectsWith(viewport))
							{
								sprite.Render(grBuffer);
								/*if (engine.ObjectsList.Count > 1)
									grBuffer.DrawLine(new Pen(Color.Red), engine.ObjectsList[0].Center, engine.ObjectsList[1].Center);
								 */
							}
							else if (sprite.Bounds.Right < 0)
							{
								//sprite.Dead = true;
							}
					}


					//Копируем буфер в рабочую область
					grSurface.DrawImage(_buffer, 0, 0);

					//Прошла секунда, высчитываем фпс
					_dtStamp = DateTime.Now;
					_compareSecond = _dtStamp.Second;
					if (_compareSecond != _lastSecond)
					{
						//выводим фпс
						if (_lastSecond != -1)
							_actualFPS = _frames;
						fpsLabel.Text = _actualFPS.ToString();
						_frames = 1;
						_lastSecond = _compareSecond;

						distanceLabel.Text = engine.distanceCovered.ToString();
					}
					else
						_frames++;
				}
			}
			/*catch {

			}*/
			finally
			{

			}
		}

		private void DrawGraphs()
		{
			//GraphPane forcePane = force_chart.GraphPane;
			//forcePane.Title.Text = "Эпюры сил";
			//forcePane.Legend.IsVisible = false;
			//forcePane.XAxis.Title.IsVisible = false;
			//forcePane.XAxis.Scale.FontSpec.Size = 32;
			//forcePane.XAxis.Scale.MaxAuto = false;
			//forcePane.XAxis.Scale.Max = 568;
			//forcePane.XAxis.Scale.MinAuto = false;
			//forcePane.YAxis.Scale.MaxAuto = true;
			//forcePane.YAxis.Title.IsVisible = false;
			//forcePane.YAxis.Scale.FontSpec.Size = 32;
			//forcePane.AxisChange();

			//engine.forcePane = forcePane;
		}

		private void Form1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 'r')
				engine.Restart();
		}
	}
}