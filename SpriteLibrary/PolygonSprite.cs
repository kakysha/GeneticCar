﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SpriteLibrary
{
	public class PolygonSprite : Sprite
	{
		protected PointF[] _points;
		private PointF[] _drawnPoints;
		private PointF[] _unrotated;
		private Color _color = Color.Red;
		private int _width = 1;
		private bool _filled = false;
		private Color _fillColor = Color.Empty;
		private int _lastAngle;

		public PointF[] 
			Points
		{
			get
			{
				return _points;
			}
			set
			{
				_points = value;

				//Вычисление формы спрайта
				float x1 = 0;
				float y1 = 0;
				float x2 = 0;
				float y2 = 0;
				foreach (PointF pt in _points)
				{
					if (pt.X < x1)
						x1 = pt.X;
					if (pt.X > x2)
						x2 = pt.X;
					if (pt.Y < y1)
						y1 = pt.Y;
					if (pt.Y > y2)
						y2 = pt.Y;
				}
				Shape = new RectangleF(x1, y1, x2 - x1, y2 - y1);
			}
		}

		public Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}

		public int LineWidth
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public bool IsFilled
		{
			get
			{
				return _filled;
			}
			set
			{
				_filled = value;
			}
		}

		public Color FillColor
		{
			get
			{
				return _fillColor;
			}
			set
			{
				_fillColor = value;
			}
		}

		public PolygonSprite(params PointF[] points)
		{
			Points = points;
			_drawnPoints = new PointF[_points.Length];
			_unrotated = new PointF[_points.Length];
			_points.CopyTo(_unrotated, 0);
		}

		//Процесс вычисления координат для вращения
		protected internal override void Process()
		{
			//Вращение 
			if (Angle != _lastAngle)
			{
				float sin = Sprite.Sin(Angle);
				float cos = Sprite.Cos(Angle);
				_lastAngle = Angle;
				for (int p = 0; p < _points.Length; p++)
				{
					_points[p].X = _unrotated[p].X * cos - _unrotated[p].Y * sin;
					_points[p].Y = _unrotated[p].Y * cos + _unrotated[p].X * sin;
				}

				//Заного вычисляем размеры спрайта и его форму
				Points = _points;
			}
		}

		public override bool IntersectsWith(Sprite s)
		{
			if (!(s is PolygonSprite))
				return base.IntersectsWith(s);
			else
			{
				if (Bounds.IntersectsWith(s.Bounds))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		//Прорисовка
		public override void Render(Graphics g)
		{
			//Прорисовка точек
			for (int pt = 0; pt < _points.Length; pt++)
			{
				_drawnPoints[pt].X = _points[pt].X + X; //TODO: SURFACE OFFSET!
				_drawnPoints[pt].Y = _points[pt].Y + Y; //TODO: SURFACE OFFSET!
			}

			//Заполнение
			if (_filled)
			{
				Brush brush = new SolidBrush(_fillColor);
				using (brush)
				{
					g.FillPolygon(brush, _drawnPoints);
				}
			}

			//Прорисовка границы
			Pen pen = new Pen(_color, _width);
			using (pen)
			{
				g.DrawPolygon(pen, _drawnPoints);
				//g.DrawRectangle(pen, ClickBounds);
			}
		}
	}

}
