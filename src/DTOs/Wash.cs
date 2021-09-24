using System;
using System.Collections.Generic;
using System.Text;

namespace primer_problema.DTOs
{
	public class Wash
	{

		public Wash() { }

		public Wash(Wash x)
		{
			this.number = x.number;
			this.hoursToFinish = x.hoursToFinish;
			this.clothes = new List<Cloth>();
			foreach (var cloth in x.clothes)
			{
				this.clothes.Add(
					new Cloth()
					{
						hoursToClean = cloth.hoursToClean,
						number	 = cloth.number
					});
			}
		}

		public long number { get; set; }
		public long hoursToFinish { get; set; }
		public List<Cloth> clothes { get; set; }
	}
}
