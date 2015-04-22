using System;
using System.Collections.Generic;

namespace TouriDroid
{
	public class GuideSearch
	{
		public int guideId { get; set; }
		public int availability { get; set; }

		public List<string> languageList = new List<string>();
		public List<string> placesServedList = new List<string>();
		public List<string> expertiseList = new List<string>();

		public GuideSearch ()
		{
		}
			
	}
}

