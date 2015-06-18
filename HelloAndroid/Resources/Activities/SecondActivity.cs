
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
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Java.Net;
using Java.IO;
using Android.Util;
using Org.Json;


namespace TouriDroid
{
	[Activity (Label = "Nativus", Theme = "@style/Theme.AppCompat", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]			
	public class SecondActivity : ActionBarActivity
	{
		public List<String> 	availableLanguages = new List<String> ();
		private	Activity thisActivity = null;

		//Variables referenced by child fragments
		public GuideSearch 		mGuideSearch;
		public List<Guide> 		mGuideList;
		public string 			mPlace="";
		public string 			mExpertise="";
		public List<string> 	checkedLanguages = new List<String>();

		protected override void OnCreate (Bundle savedInstanceState)
		{
			RequestWindowFeature(WindowFeatures.ActionBar);
			base.OnCreate (savedInstanceState);

			//if (savedInstanceState != null)
			//return;

			setDefaultValues();
		
			SetContentView (Resource.Layout.Second);

			thisActivity = this;
			mGuideSearch = new GuideSearch ();
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);
			SupportActionBar.Title = mExpertise;

			if (savedInstanceState == null) {
				//load the Guide Fragment
				var newFragment = new GuideFragment ();
				var ft = FragmentManager.BeginTransaction ();
				ft.Add (Resource.Id.fragment_container, newFragment, "GuideFragment");
				ft.Commit ();				
			}

		}
			
		private void setDefaultValues()
		{
			//@todo get default
			checkedLanguages.Add ("English");	

			//Get the selected location and expertise from the Main activity
			// Set by ExpertiseFragment

			mPlace = Intent.GetStringExtra (Constants.selectedLocation) ?? "";
			mExpertise = Intent.GetStringExtra (Constants.selectedExpertise) ?? "";
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
					// app icon in action bar clicked; go home
					//Intent intent = new Intent(this, HomeActivity.class);
					//intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
					//startActivity(intent);
				if (FragmentManager.BackStackEntryCount > 0) {
					Log.Debug (Constants.TOURI_TAG, "popping backstack which has " + FragmentManager.BackStackEntryCount + " entries");
					FragmentManager.PopBackStack ();
				} else {
					Log.Debug (Constants.TOURI_TAG, "Finishing second activity");
					Finish ();
				}
				return true;
				//base.OnBackPressed();
				//return base.OnOptionsItemSelected (item);
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_guide, menu);
			return base.OnCreateOptionsMenu(menu);

		} 

		private void searchPlaces_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (sender != null) {
				string place = ((AutoCompleteTextView)sender).Text;
				mGuideSearch.placesServedList.Clear ();
				mGuideSearch.placesServedList.Add (place);
				GuideFragment gf = FragmentManager.FindFragmentById<GuideFragment> (Resource.Id.fragment_container);
				gf.RefineSearch(mGuideSearch);
				 
			} else {
				//@todo
				Log.Debug(Constants.TOURI_TAG, "sender was null");
			}
		}
	}
}

