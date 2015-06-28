
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
		UserPreferences mUserPreferences;
		List<string>    mCheckedLanguages;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.PreferencesLayout);

			mUserPreferences = new UserPreferences (this);
			// Create your application here
			ActionBar.SetDisplayHomeAsUpEnabled (true);

			SetupWithinDistance (); 
			SetupOffline ();
			SetupLanguagesTable ();

			Spinner spinner = (Spinner) FindViewById(Resource.Id.distanceSpinner);
			CheckBox showOfflineCB = (CheckBox) FindViewById(Resource.Id.Offlinecheckbox);
			Button apply = FindViewById<Button> (Resource.Id.applyFilter);
			apply.Click += (object IntentSender, EventArgs e) => {
				string distance = spinner.SelectedItem.ToString();
				mUserPreferences.SaveWithinDistance(distance);
				mUserPreferences.SaveLanguages(mCheckedLanguages);
				mUserPreferences.SaveShowOffline(showOfflineCB.Checked);

				Finish();
				var mainActivity = new Intent (this, typeof(MainActivity));
				mainActivity.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
				this.StartActivity(mainActivity);
			};
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

		private void SetupOffline()
		{
			CheckBox showOfflineCB = (CheckBox) FindViewById(Resource.Id.Offlinecheckbox);
			bool showOffline = mUserPreferences.GetShowOffline ();
			showOfflineCB.Checked = showOffline;
		}

		private void SetupWithinDistance()
		{
			Spinner spinner = (Spinner) FindViewById(Resource.Id.distanceSpinner);

			// Create an ArrayAdapter using the string array and a default spinner layout
			ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.distances, Android.Resource.Layout.SimpleSpinnerItem);
			// Specify the layout to use when the list of choices appears
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			// Apply the adapter to the spinner
			spinner.Adapter = adapter;

			SessionManager sm = new SessionManager (this);
			string searchDistance = mUserPreferences.GetWithinDistanceAsString ();

			int position = adapter.GetPosition(searchDistance);
			spinner.SetSelection (position);
		}

		private void SetupLanguagesTable()
		{
			TableLayout languagesTable = (TableLayout)FindViewById (Resource.Id.table_Languages);
			mCheckedLanguages = mUserPreferences.GetLanguages ();

			for (int i = 0; i < Constants.AvailableLanguages.Count; i++) {
				TableRow row = (TableRow)LayoutInflater.From (this).Inflate (Resource.Layout.language_tablerow, null);
				CheckBox c = row.FindViewById<CheckBox> (Resource.Id.languageCheck);
				c.Text = Constants.AvailableLanguages [i].Item2;

				if (mCheckedLanguages.Contains (Constants.AvailableLanguages [i].Item2)) {
					c.Checked = true;
				}

				c.Click += (object sender, EventArgs e) => {
					if (c.Checked==true)
					{
						mCheckedLanguages.Add(c.Text);
					}
					else
					{
						mCheckedLanguages.Remove(c.Text);
					}
				};
				languagesTable.AddView(row);
			}

			languagesTable.RequestLayout();

		}
	}
}

