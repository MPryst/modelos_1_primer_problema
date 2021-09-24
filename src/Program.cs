using primer_problema.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace primer_problema
{
	class Program
	{
		private const string PHISYCALL_INPUT_FILE = "";// complete before running the program
		private const string PHISYCALL_OUTPUT = ""; // complete before running the program
		private const int NUMBER_TO_CALCULATE = 11;

		private static Dictionary<Tuple<long, long>, Boolean> exceptions = new Dictionary<Tuple<long, long>, bool>();
		private static List<Cloth> clothes = new List<Cloth>();
		private static List<Wash> washes = new List<Wash>();
		private static List<Wash> partialWashes = new List<Wash>();
		static void Main(string[] args)
		{
			string inputText = File.ReadAllText(PHISYCALL_INPUT_FILE);
			foreach (string line in inputText.Split('\n'))
				InterpretLine(line);

			InitializeClothes();
			CleanClothes();			
		}

		private static void InitializeClothes()
		{
			foreach (var element in exceptions)
			{
				var first = clothes.Where(x => x.number == element.Key.Item1).FirstOrDefault();				
				first.numberOfRestrictions++;				
			}
		}

		private static void InterpretLine(string line)
		{
			var characters = line.Split(" ");

			if (characters[0] == "e")
				AddExceptionRule(long.Parse(characters[1]), long.Parse(characters[2]));

			if (characters[0] == "n")
				CreateCloth(long.Parse(characters[1]), long.Parse(characters[2]));
		}

		private static void CreateCloth(long number, long hoursToClean)
		{
			clothes.Add(new Cloth()
			{
				number = number,
				hoursToClean = hoursToClean
			});
		}

		private static void AddExceptionRule(long A, long B)
		{
			// Both are kept because they're not guaranteed 
			if (!exceptions.ContainsKey(Tuple.Create(A, B)))
				exceptions.Add(Tuple.Create(A, B), true);

			if (!exceptions.ContainsKey(Tuple.Create(B, A)))
				exceptions.Add(Tuple.Create(B, A), true);
		}

		private static void CleanClothes()
		{
			SortClothes();
			partialWashes = washes;
			List<IEnumerable<int>> permutations = GetPermutations(Enumerable.Range(1, NUMBER_TO_CALCULATE), NUMBER_TO_CALCULATE).ToList();


			var currentWashList = washes.Select(x => new Wash(x)).ToList();
			var best = currentWashList.Select(x => new Wash(x)).ToList();
			long hoursCurrent = 0;
			long hoursbest = 99999;
			var bestElement = permutations[0];
			foreach (var element in permutations)
			{
				for (int i = 0; i < clothes.Count - NUMBER_TO_CALCULATE; i++)
				{
					washes = washes.OrderByDescending(x => x.clothes.Count).ToList();
					if (!AddClothToWashes(clothes[i]))
						AddWash(clothes[i], washes.Count() + 1);
				}

				currentWashList = GetWashingOrder(element.ToList(), NUMBER_TO_CALCULATE);
				hoursCurrent = HoursToWash(currentWashList);


				if (hoursCurrent < hoursbest)
				{
					best = currentWashList.ToList();
					hoursbest = hoursCurrent;
					bestElement = element;
				}
				washes.Clear();
			}

			SortClothes();
			for (int i = 0; i < clothes.Count - NUMBER_TO_CALCULATE; i++)
			{
				washes = washes.OrderByDescending(x => x.clothes.Count).ToList();
				if (!AddClothToWashes(clothes[i]))
					AddWash(clothes[i], washes.Count() + 1);
			}

			washes = GetWashingOrder(bestElement.ToList(), NUMBER_TO_CALCULATE);
			WriteWashedClothes();
		}

		private static long HoursToWash(List<Wash> currentWashList)
		{
			long hourstoWash = 0;
			foreach (var element in currentWashList)
			{
				hourstoWash += element.hoursToFinish;
			}
			return hourstoWash;
		}

		private static List<Wash> GetWashingOrder(List<int> elements, int offset)
		{
			SortClothes();
			for (int i = 0; i < elements.Count(); i++)
			{
				int index = clothes.Count() - offset + elements[i] - 1;
				if (!AddClothToWashes(clothes[index]))
					AddWash(clothes[index], washes.Count() + 1);

			}
			return washes;
		}

		public static void SortClothes()
		{
			clothes = clothes.OrderByDescending(x => x.numberOfRestrictions).ToList();
		}

		private static void AddWash(Cloth cloth, long numberOfWash = 1)
		{
			washes.Add(new Wash()
			{
				number = numberOfWash,
				hoursToFinish = cloth.hoursToClean,
				clothes = new List<Cloth>() { cloth }
			});
		}

		private static bool AddClothToWashes(Cloth cloth)
		{
			foreach (var wash in washes)
			{
				if (CanAddClothToWas(cloth, wash))
				{
					wash.clothes.Add(cloth);
					if (cloth.hoursToClean > wash.hoursToFinish)
					{
						wash.hoursToFinish = cloth.hoursToClean;
					}
					return true;
				}
			}
			return false;
		}

		private static bool CanAddClothToWas(Cloth cloth, Wash wash)
		{
			foreach (var washCloth in wash.clothes)
			{
				if (exceptions.ContainsKey(Tuple.Create(cloth.number, washCloth.number)))
					return false;
			}
			return true;
		}

		private static void WriteWashedClothes()
		{
			using StreamWriter file = new StreamWriter(PHISYCALL_OUTPUT);
			foreach (var wash in washes)
			{
				foreach (var cloth in wash.clothes)
				{
					file.WriteLine($"{cloth.number} {wash.number}");
				}
			}
		}

		static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
		{
			if (length == 1) return list.Select(t => new T[] { t });

			return GetPermutations(list, length - 1)
				.SelectMany(t => list.Where(e => !t.Contains(e)),
					(t1, t2) => t1.Concat(new T[] { t2 }));
		}
	}
}
