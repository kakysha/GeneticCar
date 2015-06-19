using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeneticLibrary;


namespace SpriteLibrary
{
	//Движок
	public class Engine
	{
		private int _dt = 10; //time delta, to calculate pixels per second
		private bool _detectCollision = true;
		private Random rnd = new Random();
		private Size _surfaceSize;
		private int _offSetX;
		private int _offSetY;
		private int _ms = 0;
		public int distanceCovered;
		private int _prevDistance;
		private double desiredWheelDist;

		private GeneticPopulation _population = null;
		private int currentIndividualIdx = -1;
		private int desiredHeight = 0;

		private List<Sprite> _spriteList = new List<Sprite>(); //all sprites for rendering
		private List<Sprite> _objectsList = new List<Sprite>(); //car objects
		private List<SurfaceSprite> _surfaceSprites = new List<SurfaceSprite>(); //surface
		private PolygonSprite _frame;
		private PolygonSprite _weight;

		public List<Sprite> ObjectsList
		{
			get
			{
				return _objectsList;
			}
		}

		//Лист спрайтов в движке
		public IList<Sprite> Sprites
		{
			get
			{
				return _spriteList;
			}
		}

		public Size SurfaceSize
		{
			get
			{
				return _surfaceSize;
			}
			set
			{
				_surfaceSize = value;
			}
		}

		public int OffSetX
		{
			get
			{
				return _offSetX;
			}
			set
			{
				_offSetX = value;
			}
		}

		public int OffSetY
		{
			get
			{
				return _offSetY;
			}
			set
			{
				_offSetY = value;
			}
		}

		//Добавление спрайта в движок
		public void AddSprite(Sprite sprite)
		{
			if (sprite.Shape.X == -1)
				throw new InvalidOperationException("Sprite's Shape must be set before adding to SpriteEngine");

			InitializeSprite(sprite);
			lock (_spriteList)
			{
				_spriteList.Add(sprite);
			}
		}

		public void AddObject(Sprite sprite)
		{
			lock (ObjectsList)
			{
				ObjectsList.Add(sprite);
			}
			AddSprite(sprite);
		}

		private void AddSurfacePart(SurfaceSprite ss)
		{
			_surfaceSprites.Add(ss);
			AddSprite(ss);
		}

		//Удаление спрайта из движка
		public void RemoveSprite(Sprite sprite)
		{
			sprite.Kill();
		}

		//Очищение движка от всех спрайтов
		public void Clear()
		{
			lock (_spriteList)
				foreach (Sprite sprite in _spriteList)
					sprite.Kill();
		}

		//Пемещение спрайтов
		public void MoveSprites()
		{
			//Вызов метода перемещения для каждого спрайта в движке
			lock (_spriteList)
			{
				foreach (Sprite sprite in Sprites)
				{
					sprite.PreProcess();
					sprite.Process();
					MoveSprite(sprite);
				}
				var _w1 = ObjectsList[1];
				var _w2 = ObjectsList[0];
				_frame.PositionF = _w1.Center;
				_frame.Angle = (int)(Math.Atan((_w2.Center.Y - _w1.Center.Y) / (_w2.Center.X - _w1.Center.X)) / Math.PI * 180);
				float diffy = Math.Abs((float)Math.Cos((double)_frame.Angle / 180 * Math.PI) * desiredHeight);
				float diffx = (float)Math.Sin((double)_frame.Angle / 180 * Math.PI) * desiredHeight;
				_weight.PositionF = new PointF((float)(_w2.Center.X + _w1.Center.X) / 2 + diffx, (float)(_w2.Center.Y + _w1.Center.Y) / 2 - diffy);
				_weight.Angle = _frame.Angle;
			}
		}

		private void MoveSprite(Sprite sprite, bool onlyMove = false)
		{
			if (!sprite.Movable)
				return;
			sprite.X += sprite.Velocity.X / _dt;
			if (typeof(BitmapSprite) == sprite.GetType()) //car
			{
				if (!onlyMove)
				{
					sprite.Velocity = new PointF(sprite.Velocity.X, (float)(sprite.Velocity.Y + 9.8 / _dt));
				}
				sprite.Y += sprite.Velocity.Y / _dt;
			}
		}

		//Удаление мертвых спрайтов
		public void RemoveDeadSprites()
		{
			lock (_spriteList)
			{
				for (int i = 0; i < Sprites.Count; i++)
				{
					if (Sprites[i].Dead)
					{
						DeleteSprite(Sprites[i]);
					}
				}
			}
		}

		//Удаление перед убийством
		internal void DeleteSprite(Sprite sprite)
		{
			_spriteList.Remove(sprite);
		}

		protected void InitializeSprite(Sprite sprite)
		{

		}

		//Обработка столкновения спрайтов
		public void PerformSelfCollisionDetection()
		{
			lock (ObjectsList)
			{
				Sprite o1 = ObjectsList[0];
				Sprite o2 = ObjectsList[1];
				var dist = (float)o2.Distance(o1.Center, o2.Center);
				var relVect = new PointF(Math.Abs(o2.Center.X - o1.Center.X) / dist, Math.Abs(o2.Center.Y - o1.Center.Y) / dist);
				var relDist = dist - (float)desiredWheelDist;
				var relVelX = (o2.Velocity.X - relDist - o1.Velocity.X) * relVect.X / 2;
				var relVelY = (o2.Velocity.Y - relDist - o1.Velocity.Y) * relVect.Y / 2;
				o1.Velocity = new PointF(o1.Velocity.X + relVelX, o1.Velocity.Y + relVelY);
				o2.Velocity = new PointF(o2.Velocity.X - relVelX, o2.Velocity.Y - relVelY);

				for (int i = 0; i < ObjectsList.Count; i++)
				{
					Sprite s1 = ObjectsList[i];
					for (int k = 0; k < _surfaceSprites.Count; k++)
					{
						var s2 = _surfaceSprites[k];
						if ((s2.Bounds.Right >= s1.X && s2.X <= s1.Bounds.Right && s1.Bounds.Bottom > s2.Bounds.Top) &&
							s2.DistanceToPoint(s1.Center) < s1.Radius &&
							(s1.Velocity.X * s2.Normal.X + s1.Velocity.Y * s2.Normal.Y) < 0)
						{
							var newVx = s1.Velocity.X - 1.4 * s2.Normal.X * (s1.Velocity.X * s2.Normal.X + s1.Velocity.Y * s2.Normal.Y);
							var newVy = s1.Velocity.Y - 1.4 * s2.Normal.Y * (s1.Velocity.X * s2.Normal.X + s1.Velocity.Y * s2.Normal.Y);
							s1.Velocity = new PointF(s2.Normal.X, s2.Normal.Y);
							//SurfaceSprite.XSpeed = -s2.Normal.X;
							while (s2.DistanceToPoint(s1.Center) <= s1.Radius)
							{

								MoveSprite(s1, true);
								//MoveSurface();
							}
							s1.Velocity = new PointF((float)(1 + newVx), (float)newVy);
							//SurfaceSprite.XSpeed = -30 - (o1.Velocity.X + o2.Velocity.X) / 2;
							s2.Color = Color.Green;
						}
					}
				}
			}
		}

		public void DrawPath()
		{
			if (_surfaceSprites.Count == 0 || _surfaceSprites.Last().Bounds.Right <= _surfaceSize.Width)
			{
				float startx = _surfaceSprites.Count() > 0 ? _surfaceSprites.Last().Bounds.Right : 0;
				float starty = _surfaceSprites.Count() > 0 ?
					  _surfaceSprites.Last().Points.Last().Y + _surfaceSprites.Last().Y
					: _surfaceSize.Height - 30;
				PointF[] points = new PointF[2];
				points[0] = new PointF(0, 0);
				points[1] = new PointF(rnd.Next(20, 50), rnd.Next(
					starty - 30 < SurfaceSize.Height / 2 ? (int)(SurfaceSize.Height / 2 - starty) : -30,
					starty + 30 > SurfaceSize.Height ? SurfaceSize.Height - (int)starty : 30));
				SurfaceSprite ss = new SurfaceSprite(points);
				ss.X = startx + 1;
				ss.Y = starty - 1;
				ss.Velocity = new PointF(-100, 0);
				AddSurfacePart(ss);
			}
		}

		public void Animate()
		{

			MoveSprites();

			var diff = ObjectsList[0].X - SurfaceSize.Width / 2;
			distanceCovered += (int)diff;
			foreach (Sprite s in Sprites)
			{
				s.X -= diff;
			}

			//Обработка столкнвоений
			if (_detectCollision)
				PerformSelfCollisionDetection();

			//Удаляем на всякий случай мертвые спрайты
			RemoveDeadSprites();

			//дорисовываем дорогу
			DrawPath();

			CheckStop();
		}

		private void CheckStop()
		{
			for (int k = 0; k < _surfaceSprites.Count; k++)
			{
				if (_surfaceSprites[k].DistanceToPoint(_weight.PositionF) <= 0)
				{
					Restart();
					return;
				}
			}
			if ((int)DateTime.Now.TimeOfDay.TotalSeconds / 5 > _ms)
			{
				if (distanceCovered / 10 <= _prevDistance)
				{
					Restart();
					return;
				}
				_prevDistance = distanceCovered / 10;
				_ms = (int)DateTime.Now.TimeOfDay.TotalSeconds / 5;
			}
		}

		public void Restart()
		{
			#region Genetic Algorithm
			if (currentIndividualIdx > -1)
			{
				Console.WriteLine(String.Format("Individual {0} finished : {1}", currentIndividualIdx, distanceCovered));
				_population.Individuals[currentIndividualIdx].Fitness = distanceCovered;
			}

			if (_population == null)
			{
				_population = new GeneticPopulation(5, 0.4, 0.05);
				_population.FullfillRandomPopulation();
				_population.PopulationFinished += PopulationFinishedHandler;
			}
			GeneticCar gCar;
			if (currentIndividualIdx < _population.Individuals.Count - 1)
			{
				currentIndividualIdx++;
				gCar = _population.Individuals[currentIndividualIdx];
			}
			else
				return; //will be invoked from Population
			#endregion

			var diff = _surfaceSprites.Count > 0 ? _surfaceSprites[0].X : 0;
			foreach (Sprite s in Sprites)
			{
				s.X -= diff;
			}
			_prevDistance = distanceCovered = 0;

			foreach (Sprite s in ObjectsList)
			{
				DeleteSprite(s);
			}
			_objectsList.Clear();

			Bitmap bmp = new Bitmap("Resources/wheel.png");
			BitmapSprite bs = new BitmapSprite(bmp);

			bs.Spin = SpinType.Clockwise;
			bs.SpinSpeed = 2;
			bs.PositionF = new PointF(SurfaceSize.Width / 2, SurfaceSize.Height - 300 - gCar.FrontWheel / 2);
			bs.Velocity = new PointF(0, 0);
			bs.Shape = new RectangleF(0, 0, gCar.FrontWheel, gCar.FrontWheel);

			BitmapSprite bs2 = new BitmapSprite(bmp);

			bs2.Spin = SpinType.Clockwise;
			bs2.SpinSpeed = 2;
			bs2.PositionF = new PointF(SurfaceSize.Width / 2 - gCar.Length, SurfaceSize.Height - 300 - gCar.RearWheel / 2);
			bs2.Velocity = new PointF(0, 0);
			bs2.Shape = new RectangleF(0, 0, gCar.RearWheel, gCar.RearWheel);

			desiredWheelDist = bs.Distance(bs.Center, bs2.Center);
			desiredHeight = gCar.Height;

			if (_frame != null)
			{
				RemoveSprite(_frame);
				RemoveSprite(_weight);
			}


			// добавляем раму автомобиля
			_frame = new PolygonSprite(new PointF[] {
				new PointF(0, 0),
				new PointF((float)desiredWheelDist, 0),
				new PointF((float)desiredWheelDist, -desiredHeight),
				new PointF(0, -desiredHeight)
			});
			//добавляем полезную нагрузку
			_weight = new PolygonSprite(new PointF[] {
				new PointF(0, 0),
				new PointF(0, -10),
				new PointF(10, -10),
				new PointF(10, 0)
			});
			_weight.IsFilled = true;
			_weight.FillColor = Color.CadetBlue;

			AddSprite(_frame);
			AddSprite(_weight);
			AddObject(bs);
			AddObject(bs2);

			_ms = (int)DateTime.Now.TimeOfDay.TotalSeconds / 5 + 1;

			Console.WriteLine("Restart");
		}

		private void PopulationFinishedHandler(object sender, EventArgs args)
		{
			var oldP = (GeneticPopulation)sender;
			_population = oldP.GenerateNewPopulation();
			_population.PopulationFinished += PopulationFinishedHandler;
			currentIndividualIdx = -1;
			Restart();
		}
	}
}

