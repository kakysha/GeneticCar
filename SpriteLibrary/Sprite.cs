using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SpriteLibrary
{
	//Абстрактный класс для всех спрайтов
	public abstract class Sprite
	{
		private static float[] _sin = new float[360];
		private static float[] _cos = new float[360];
		private int _facingAngle;
		private float _x;
		private float _y;
		private bool _dead;
		private RectangleF _shape = new RectangleF(-1, -1, -1, -1);
		private RectangleF _bounds = new RectangleF();
		private SpinType _spin;
		private int _spinSpeed;
		private SpinType _prevSpin;
		private PointF _velocity;
		private bool _movable = true;

		public static Random RND = new Random();

		public virtual PointF Velocity
		{
			get
			{
				return _velocity;
			}
			set
			{
				_velocity = value;
			}
		}
		public PointF Center
		{
			get
			{
				return new PointF(X + Width / 2, Y + Height / 2);
			}
		}
		public float Radius
		{
			get
			{
				return Width / 2;
			}
		}

		public RectangleF Shape
		{
			get
			{
				return _shape;
			}
			set
			{
				_shape = value;
			}
		}
		//Границы спрайта (с координатами)
		public RectangleF Bounds
		{
			get
			{
				_bounds.X = X + Shape.X;
				_bounds.Width = Shape.Width;
				_bounds.Y = Y + Shape.Y;
				_bounds.Height = Shape.Height;
				return _bounds;
			}
		}

		//Те же границы, только целого типа
		public Rectangle ClickBounds
		{
			get
			{
				return new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height);
			}
		}

		public int Width
		{
			get
			{
				return (int)Shape.Width;
			}
		}
		public int Height
		{
			get
			{
				return (int)Shape.Height;
			}
		}

		//Угол, под которым выводить спрайт
		public int Angle
		{
			get
			{
				return _facingAngle;
			}
			set
			{
				_facingAngle = value;
				while (_facingAngle >= 360)
					_facingAngle -= 360;
				while (_facingAngle < 0)
					_facingAngle += 360;
			}
		}

		public bool Dead
		{
			get
			{
				return _dead;
			}
			set
			{
				_dead = value;
			}
		}

		//Положение
		public float X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}
		public float Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		public PointF PositionF
		{
			get
			{
				return new PointF(_x, _y);
			}
			set
			{
				_x = value.X;
				_y = value.Y;
			}
		}

		//Вращение
		public SpinType Spin
		{
			get
			{
				return _spin;
			}
			set
			{
				_spin = value;
			}
		}
		public SpinType PrevSpin
		{
			get
			{
				return _prevSpin;
			}
			set
			{
				_prevSpin = value;
			}
		}
		public int SpinSpeed
		{
			get
			{
				return _spinSpeed;
			}
			set
			{
				_spinSpeed = value;
			}
		}
		public bool Movable
		{
			get
			{
				return _movable;
			}
			set
			{
				_movable = value;
			}
		}

		//Вычисляем синусы косинусы в консрукторе чтобы не делать это каждый кадр
		static Sprite()
		{
			for (int degree = 0; degree < 360; degree++)
			{
				_sin[degree] = (float)Math.Sin(DegToRad(degree));
				_cos[degree] = (float)Math.Cos(DegToRad(degree));
			}
		}

		public static float DegToRad(int degree)
		{
			return (float)((Math.PI / 180) * degree);
		}

		public static float Sin(int degree)
		{
			return _sin[degree];
		}
		public static float Cos(int degree)
		{
			return _cos[degree];
		}

		//Рандомный цвет
		public static Color ColorFromRange(Color startColor, Color endColor)
		{
			byte a = rndByte(startColor.A, endColor.A);
			byte r = rndByte(startColor.R, endColor.R);
			byte g = rndByte(startColor.G, endColor.G);
			byte b = rndByte(startColor.B, endColor.B);
			return Color.FromArgb(a, r, g, b);
		}

		//Убиение спрайта
		public void Kill()
		{
			_dead = true;
		}

		//вычисление угла вращения перед вычислением координат
		internal void PreProcess()
		{
			switch (_spin)
			{
				case SpinType.Clockwise:
					Angle += SpinSpeed;
					break;
				case SpinType.CounterClockwise:
					Angle -= SpinSpeed;
					break;
			}
		}

		//Прорисовка
		public abstract void Render(Graphics g);

		//Доп. процесс
		protected internal virtual void Process()
		{
		}

		//Для цвета
		private static byte rndByte(byte b1, byte b2)
		{
			if (b1 > b2)
			{
				byte temp = b1;
				b1 = b2;
				b2 = temp;
			}
			byte diff = (byte)(b2 - b1);
			return (byte)(RND.Next(diff) + b1);
		}

		public virtual bool IntersectsWith(Sprite s)
		{
			return this.ClickBounds.IntersectsWith(s.ClickBounds);
		}

		public void Start()
		{
			Movable = true;
		}

		public void Stop()
		{
			Movable = false;
		}

		#region Math Utils
		//Compute the dot product AB . AC
		public double DotProduct(PointF pointA, PointF pointB, PointF pointC)
		{
			PointF AB = new PointF(pointB.X - pointA.X, pointB.Y - pointA.Y);
			PointF BC = new PointF(pointC.X - pointB.X, pointC.Y - pointB.Y);
			double dot = AB.X * BC.X + AB.Y * BC.Y;

			return dot;
		}

		//Compute the cross product AB x AC
		public double CrossProduct(PointF pointA, PointF pointB, PointF pointC)
		{
			PointF AB = new PointF(pointB.X - pointA.X, pointB.Y - pointA.Y);
			PointF AC = new PointF(pointC.X - pointA.X, pointC.Y - pointA.Y);
			double cross = AB.X * AC.Y - AB.Y * AC.X;

			return cross;
		}

		//Compute the distance from A to B
		public double Distance(PointF pointA, PointF pointB)
		{
			double d1 = pointA.X - pointB.X;
			double d2 = pointA.Y - pointB.Y;

			return Math.Sqrt(d1 * d1 + d2 * d2);
		}
		#endregion
	}

	//Способы вращения спрайта
	public enum SpinType
	{
		None,
		Clockwise,
		CounterClockwise
	};
}




