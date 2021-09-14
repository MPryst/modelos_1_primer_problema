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
		private static long clothesNumber;
		private static long exceptionsNumber;
		private static Dictionary<Tuple<long, long>, Boolean> exceptions = new Dictionary<Tuple<long, long>, bool>();
		private static List<Cloth> clothes;
		private static List<WashedCloth> washedClothes = new List<WashedCloth>();
		private static List<Wash> washes = new List<Wash>();
		static void Main(string[] args)
		{
			string text = System.IO.File.ReadAllText(PHISYCALL_INPUT_FILE);
			foreach (string line in text.Split('\n'))
			{
				ParseLine(line);
			}
			clothes = clothes.OrderByDescending(c => c.hoursToClean).ToList();
			Clean();
			PrintWashes();
		}

		private static void ParseLine(string line)
		{
			var characters = line.Split(" ");
			if (characters[0] == "c")
				return;

			if (characters[0] == "p")
			{
				clothesNumber = long.Parse(characters[2]);
				exceptionsNumber = long.Parse(characters[3]);
				clothes = new List<Cloth>();
			}

			if (characters[0] == "e")
			{
				if (!exceptions.ContainsKey(Tuple.Create(long.Parse(characters[1]), long.Parse(characters[2]))))
				{
					exceptions.Add(Tuple.Create(long.Parse(characters[1]), long.Parse(characters[2])), true);
				}
				if (!exceptions.ContainsKey(Tuple.Create(long.Parse(characters[2]), long.Parse(characters[1]))))
				{
					exceptions.Add(Tuple.Create(long.Parse(characters[2]), long.Parse(characters[1])), true);
				}
			}

			if (characters[0] == "n")
			{
				clothes.Add(new Cloth()
				{
					number = long.Parse(characters[1]),
					hoursToClean = long.Parse(characters[2])
				});
			}
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

		public class Cloth
		{
			public long number { get; set; }
			public long hoursToClean { get; set; }
		}

		public class WashedCloth
		{
			public long number { get; set; }
			public long numberOfWash { get; set; }
		}

		public class Wash
		{
			public long number { get; set; }
			public List<Cloth> clothes { get; set; }
		}

		private static void PrintWashes()
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
			washedClothes.OrderBy(x => x.number);

			using StreamWriter file = new StreamWriter(PHISYCALL_OUTPUT);

			foreach (var washedCloth in washedClothes)
			{
				file.WriteLine($"{washedCloth.number} {washedCloth.numberOfWash}");
				Console.WriteLine($"{washedCloth.number} {washedCloth.numberOfWash}");
			}
		}
	}
}
