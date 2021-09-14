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
		private static List<WashedCloth> washedClothes = new List<WashedCloth>();
		static void Main(string[] args)
		{
			string text = System.IO.File.ReadAllText(PHISYCALL_INPUT_FILE);
			foreach (string line in text.Split('\n'))
			{
				InterpretLine(line);
			}
			Clean();
			GetWashedClothes();
			WriteWashedClothes();
		}

		private static void InterpretLine(string line)
		{
			var characters = line.Split(" ");

			if (characters[0] == "e")							
				AddException(long.Parse(characters[1]), long.Parse(characters[2]));

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

		private static void AddException(long A, long B)
		{
			// Both are kept because they're not guaranteed 
			if (!exceptions.ContainsKey(Tuple.Create(A, B)))
				exceptions.Add(Tuple.Create(A, B), true);

			if (!exceptions.ContainsKey(Tuple.Create(B, A)))
				exceptions.Add(Tuple.Create(B, A), true);			
		}

		private static void Clean()
		{
			foreach (var cloth in clothes)
			{
				if (!washes.Any())
				{
					washes.Add(new Wash()
					{
						number = 1,
						clothes = new List<Cloth>() { cloth }
					});
				}
				else
				{
					if (!AddClothToWashes(cloth))
					{
						washes.Add(new Wash()
						{
							number = washes.Count() + 1,
							clothes = new List<Cloth>() { cloth }
						});
					}
				}
			}
		}

		private static bool AddClothToWashes(Cloth cloth)
		{
			foreach (var wash in washes)
			{
				if (CanAddClothToWas(cloth, wash))
				{
					wash.clothes.Add(cloth);
					return true;
				}
			}
			return false;
		}

		private static bool CanAddClothToWas(Cloth cloth, Wash wash)
		{
			foreach (var element in wash.clothes)
			{
				if (exceptions.ContainsKey(Tuple.Create(cloth.number, element.number)))
				{
					return false;
				}
			}
			return true;
		}

		private static void GetWashedClothes()
		{
			foreach (var wash in washes)
			{
				foreach (var cloth in wash.clothes)
				{
					washedClothes.Add(new WashedCloth()
					{
						number = cloth.number,
						numberOfWash = wash.number
					});
				}
			}
		}

		private static void WriteWashedClothes()
		{
			using StreamWriter file = new StreamWriter(PHISYCALL_OUTPUT);

			foreach (var washedCloth in washedClothes)
			{
				file.WriteLine($"{washedCloth.number} {washedCloth.numberOfWash}");
			}
		}
	}
}
