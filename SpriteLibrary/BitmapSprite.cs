using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SpriteLibrary
{
	public class BitmapSprite : Sprite
	{
		private Bitmap _bitmap;

		//Constructor
		public BitmapSprite(Bitmap bitmap)
		{
			Bitmap = bitmap;
		}

		//Прорисовка
		public override void Render(Graphics g)
		{
			g.DrawImage(Bitmap, X, Y, Width, Height);
		}

		public Bitmap Bitmap
		{
			get
			{
				return _bitmap;
			}
			set
			{
				_bitmap = value;
				if (_bitmap != null)
					Shape = new RectangleF(0, 0, _bitmap.Width, _bitmap.Height);
			}
		}

		//Вращение
		protected internal override void Process()
		{
			return; // заглушка
			Bitmap bmp = new Bitmap(_bitmap.Width, _bitmap.Height);
			Graphics gfx = Graphics.FromImage(bmp);
			gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
			gfx.RotateTransform(Angle);
			gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
			gfx.DrawImage(_bitmap, new Point(0, 0));
			gfx.Dispose();
			_bitmap = new Bitmap(bmp);
		}

	}
}

