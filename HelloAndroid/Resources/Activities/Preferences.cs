
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
using Android.Util;

namespace TouriDroid
{
	[Activity (Label = "Preferences")]			
	public class Preferences : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.PreferencesLayout);
			// Create your application here
			ActionBar.SetDisplayHomeAsUpEnabled (true);

			Button apply = FindViewById<Button> (Resource.Id.applyFilter);
			Spinner spinner = (Spinner) FindViewById(Resource.Id.distanceSpinner);

			// Create an ArrayAdapter using the string array and a default spinner layout
			ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this,
				Resource.Array.distances, Android.Resource.Layout.SimpleSpinnerItem);
			// Specify the layout to use when the list of choices appears
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			// Apply the adapter to the spinner
			spinner.Adapter = adapter;

			SessionManager sm = new SessionManager (this);
			string searchDistance = sm.getSearchDistance ();

			int position = adapter.GetPosition(searchDistance);
			spinner.SetSelection (position);

			Converter converter = new Converter();

			apply.Click += (object IntentSender, EventArgs e) => {

				string distance = spinner.SelectedItem.ToString();
				if (sm.isLoggedIn())
				{
					sm.setSearchDistance(distance);
				}
				Finish();
			};
		}

		private void SetupLanguagesTable()
		{
		}
	}
}

