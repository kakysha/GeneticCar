using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpriteLibrary
{
	class SurfaceSprite : PolygonSprite
	{
		public static float XSpeed = 0;

		public override PointF Velocity
		{
			get
			{
				return new PointF(XSpeed, base.Velocity.Y);
			}
			set
			{
				base.Velocity = value;
			}
		}

		public SurfaceSprite(params PointF[] points)
			: base(points)
		{
			if (points.Count() != 2)
				throw new ArgumentException("Argument must containt 2 Points");
		}

		public PointF Normal
		{
			get
			{
				var x1 = _points[0].X;
				var x2 = _points[1].X;
				var y1 = _points[0].Y;
				var y2 = _points[1].Y;
				var length = (float)Distance(_points[0], _points[1]);
				return new PointF((y2 - y1) / length, -(x2 - x1) / length);
			}
		}

		public float DistanceToPoint(PointF p)
		{
			var pointA = new PointF(X + _points[0].X, Y + _points[0].Y);
			var pointB = new PointF(X + _points[1].X, Y + _points[1].Y);
			double dot1 = DotProduct(pointA, pointB, p);
			if (dot1 > 0)
				return (float)Distance(pointB, p);

			double dot2 = DotProduct(pointB, pointA, p);
			if (dot2 > 0)
				return (float)Distance(pointA, p);
			return (float)(-CrossProduct(pointA, pointB, p) / Distance(pointA, pointB));
		}
	}
}
