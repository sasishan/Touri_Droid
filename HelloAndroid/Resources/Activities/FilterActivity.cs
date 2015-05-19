
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TouriDroid
{
	[Activity (Label = "FilterActivity", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]	
	public class FilterActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Second);

			if (savedInstanceState == null) {
				//load the Guide Fragment
				var newFragment = new FilterFragment ();
				var ft = FragmentManager.BeginTransaction ();
				ft.Add (Resource.Id.fragment_container, newFragment);
				ft.Commit ();				
			}
			// Create your application here
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
				Finish ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}
	}
}

