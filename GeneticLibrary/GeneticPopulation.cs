using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticLibrary
{
	public class GeneticPopulation
	{
		Random rnd = new Random((int)DateTime.Now.TimeOfDay.TotalSeconds);
		private List<GeneticCar> individuals = new List<GeneticCar>();
		private int _finishedCount = 0;
		private readonly int _generationSize;
		private readonly double _crossoverRate;
		private readonly double _mutationRate;
		public event EventHandler PopulationFinished;

		public List<GeneticCar> Individuals
		{
			get
			{
				return individuals;
			}
		}

		public GeneticPopulation(int size, double crossover, double mutation)
		{
			this._generationSize = size;
			this._crossoverRate = crossover;
			this._mutationRate = mutation;
		}

		public GeneticPopulation GenerateNewPopulation()
		{
			GeneticPopulation gp = new GeneticPopulation(_generationSize, _crossoverRate, _mutationRate);
			while (gp.Individuals.Count < _generationSize)
			{
				gp.AddIndividual(GenerateNewIndividual());
			}
			return gp;
		}

		public void FullfillRandomPopulation()
		{
			while (Individuals.Count < _generationSize)
			{
				AddIndividual(GenerateRandomIndividual());
			}
		}

		public void AddIndividual(GeneticCar car)
		{
			if (Individuals.Count == _generationSize)
				return;
			Individuals.Add(car);
			car.Finished += FinishHandler;
		}

		private GeneticCar GenerateNewIndividual()
		{
			GeneticCar car = new GeneticCar();

			var fatherInd = SelectParent();
			GeneticCar father = Individuals[fatherInd];
			var motherInd = SelectParent(fatherInd);
			GeneticCar mother = Individuals[motherInd];

			car.Length = CrossoverInt(father.Length, mother.Length);
			car.FrontWheel = CrossoverInt(father.FrontWheel, mother.FrontWheel);
			car.RearWheel = CrossoverInt(father.RearWheel, mother.RearWheel);
			car.Height = CrossoverInt(father.Height, mother.Height);

			car.Length = MutateInt(car.Length);
			car.FrontWheel = MutateInt(car.FrontWheel);
			car.RearWheel = MutateInt(car.RearWheel);
			car.Height = MutateInt(car.Height);

			Console.WriteLine(String.Format
			("New offspring from Father={0} Mother={1} : Length={2} Front={3} Rear={4}",
			fatherInd, motherInd, car.Length, car.FrontWheel, car.RearWheel));

			return car;
		}
		private GeneticCar GenerateRandomIndividual()
		{
			GeneticCar car = new GeneticCar();

			car.Length = rnd.Next(GeneticCar.LENGTH_MIN, GeneticCar.LENGTH_MAX);
			car.RearWheel = rnd.Next(GeneticCar.REARWHEEL_MIN, GeneticCar.REARWHEEL_MAX);
			car.FrontWheel = rnd.Next(GeneticCar.FRONTWHEEL_MIN, GeneticCar.FRONTWHEEL_MAX);
			car.Height = rnd.Next(GeneticCar.HEIGHT_MIN, GeneticCar.HEIGHT_MAX);

			return car;
		}

		private int SelectParent(int ind = -1)
		{
			int i;
			do
			{
				i = 0;
				var r = rnd.NextDouble();
				foreach (GeneticCar car in Individuals) //fix for offset
				{
					if (car.Fitness <= 0)
						r += rnd.Next(0, 1) * 0.01;
				}
				while ((Individuals[i].RelativeFitness < r) && i < Individuals.Count - 1)
				{
					r -= Individuals[i].RelativeFitness;
					i++;
				}
			} while (i == ind);
			return i;
		}

		private void GenerateRelativeFitnesses()
		{
			int sum = Individuals.Sum(car => car.Fitness > 0 ? car.Fitness : 0);
			Individuals.ForEach(delegate(GeneticCar car)
			{
				car.RelativeFitness = car.Fitness > 0 ? (double)car.Fitness / sum : 0.01;
				Console.WriteLine(car.Fitness + "->" + car.RelativeFitness);
			});
		}

		#region Crossover
		private int CrossoverInt(int a, int b)
		{
			string A = Convert.ToString(a, 2).PadLeft(10, '0');
			string B = Convert.ToString(b, 2).PadLeft(10, '0');
			char[] Cc = new char[10];
			for (int i = 0; i < A.Length; i++)
			{
				if (rnd.NextDouble() <= _crossoverRate)
					Cc[i] = B[i];
				else
					Cc[i] = A[i];
			}
			string C = new string(Cc);
			return Convert.ToInt32(C, 2);
		}
		#endregion

		#region Mutation
		private int MutateInt(int a)
		{
			string A = Convert.ToString(a, 2).PadLeft(10, '0');
			char[] Cc = new char[10];
			for (int i = 0; i < A.Length; i++)
			{
				if (rnd.NextDouble() <= _mutationRate)
					Cc[i] = A[i] == '1' ? '0' : '1';
				else
					Cc[i] = A[i];
			}
			string C = new string(Cc);
			return Convert.ToInt32(C, 2);
		}
		#endregion

		private void FinishHandler(object sender, EventArgs args)
		{
			_finishedCount++;
			if (_finishedCount == _generationSize)
			{
				Console.WriteLine(String.Format("Population Finished. Avg. Fitness={0,2}", Individuals.Sum(car => car.Fitness > 0 ? car.Fitness : 0) / _generationSize));
				GenerateRelativeFitnesses();
				if (PopulationFinished != null)
					PopulationFinished(this, EventArgs.Empty);
			}
		}
	}
}
