using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Json;
using Android.Graphics;

namespace TouriDroid
{
	public class Guide
	{
		public int guideId { get; set; }
		public string userName { get; set; }
		public string fName { get; set; }
		public string lName { get; set; }
		public string phone { get; set; }
		public string address1 { get; set; }
		public string address2 { get; set; }
		public string description { get; set; }
		public string summary { get; set; }
		public int availability { get; set; }
		public int profileImageId { get; set; }
		public Bitmap profileImage { get; set; }
		public JsonValue jsonText { get; set; }

		public List<string> languageList = new List<string>();
		public List<string> placesServedList = new List<string>();
		public List<GuideLanguage> languages = new List<GuideLanguage>();
		public List<Expertise> expertise = new List<Expertise>();

		public Guide ()
		{

		}
	}

	public class GuideLanguage
	{
		public int languageId { get; set; }
		public string language { get; set; }
		public string fluency { get; set; }

		public GuideLanguage ()
		{

		}
	}

	public class Expertise
	{
		public int expertiseId { get; set; }
		public string expertise { get; set; }
		public string description { get; set; }
		public int expertiseImageId { get; set; }
		public Bitmap expertiseImage { get; set; }
		public int numberOfGuides { get; set; }

		public Expertise ()
		{

		}
	}
}

