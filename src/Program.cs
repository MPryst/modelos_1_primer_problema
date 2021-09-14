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

		private static Dictionary<Tuple<long, long>, Boolean> exceptions = new Dictionary<Tuple<long, long>, bool>();
		private static List<Cloth> clothes = new List<Cloth>();
		private static List<Wash> washes = new List<Wash>();
		static void Main(string[] args)
		{
			string inputText = File.ReadAllText(PHISYCALL_INPUT_FILE);
			foreach (string line in inputText.Split('\n'))
				InterpretLine(line);

			CleanClothes();
			WriteWashedClothes();
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
			clothes = clothes.OrderBy(x => x.hoursToClean).ToList();
			foreach (var cloth in clothes)
			{
				washes = washes.OrderBy(x => x.hoursToFinish).ToList();
				if (!AddClothToWashes(cloth))
					AddWash(cloth, washes.Count() + 1);
			}
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
	}
}
