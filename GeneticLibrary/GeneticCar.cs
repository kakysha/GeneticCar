using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticLibrary
{
	public class GeneticCar
	{
		public static int FRONTWHEEL_MAX = 100;
		public static int FRONTWHEEL_MIN = 20;
		public static int REARWHEEL_MAX = 100;
		public static int REARWHEEL_MIN = 20;
		public static int LENGTH_MAX = 260;
		public static int LENGTH_MIN = 60;
		public static int HEIGHT_MIN = 1;
		public static int HEIGHT_MAX = 60;

		private int _frontWheel;

		public int FrontWheel
		{
			get
			{
				return _frontWheel;
			}
			set
			{
				_frontWheel = value < FRONTWHEEL_MIN ? FRONTWHEEL_MIN : value > FRONTWHEEL_MAX ? FRONTWHEEL_MAX : value;
			}
		}
		private int _rearWheel;

		public int RearWheel
		{
			get
			{
				return _rearWheel;
			}
			set
			{
				_rearWheel = value < REARWHEEL_MIN ? REARWHEEL_MIN : value > REARWHEEL_MAX ? REARWHEEL_MAX : value;
			}
		}
		private int _length;

		public int Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value < LENGTH_MIN ? LENGTH_MIN : value > LENGTH_MAX ? LENGTH_MAX : value;
			}
		}
		private int _fitness;

		public int Fitness
		{
			get
			{
				return _fitness;
			}
			set
			{
				_fitness = value;
				if (Finished != null)
					Finished(this, EventArgs.Empty);
			}
		}

		private double _relativeFitness;

		public double RelativeFitness
		{
			get
			{
				return _relativeFitness;
			}
			set
			{
				_relativeFitness = value;
			}
		}

		private int _height;

		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value < HEIGHT_MIN ? HEIGHT_MIN : value > HEIGHT_MAX ? HEIGHT_MAX : value;
			}
		}

		public event EventHandler Finished;
	}
}
