
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
		private DrawerLayout 	mDrawer;
		private ListView 		mDrawerList;
		//List<string> mDrawerItems = new List<string>();
		ActionBarDrawerToggle 	drawerToggle;
		private ArrayAdapter<string> mAdapterLanguages=null;
		private ArrayAdapter<string> mAdapterMainMenu=null;
		private ArrayAdapter<string> mAdapterExpertise=null;
		private SparseBooleanArray mLastSparseArrayLanguages;
		private SparseBooleanArray mLastSparseArrayExpertise;
		public List<String> 	availableLanguages = new List<String> ();
		private	Activity thisActivity = null;

		//Variables referenced by child fragments
		public GuideSearch 		mGuideSearch;
		public List<Guide> 	mGuideList;
		public string 			mPlace="";
		public string 			mExpertise="";
		public List<string> 	checkedLanguages = new List<String>();

		//SecondActivity_DrawerMainMenuId
		private static readonly string[] mDrawerItems = new []
		{
			Constants.DrawerOptionLanguage, "Contact Method", "Rates"
			//Constants.DrawerOptionLanguage, Constants.DrawerOptionExpertise, "Contact Method", "Rates"
		};

		//SecondActivity_SecondActivity_DrawerLanguageOptionsId
		private static readonly string[] mDrawerLanguages = new []
		{
			"Done", "Chinese","Spanish","English","Arabic","Hindi","Croatian","Portuguese","Russian","Japanese","German","Macedonian","Vietnamese","French","Korean","Tamil","Italian","Urdu"
		};

		private static readonly string[] mDrawerExpertise = new []
		{
			"Done", "All", "Restaurants", "Museums", "Nightlife", "Hot Spots", "Landmarks"
		};
			
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

		protected override void OnResume ()
		{


			base.OnResume ();
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
		//	drawerToggle.SyncState ();
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
		//	if (drawerToggle.OnOptionsItemSelected (item)) 
		//	{
		//		return (true);
		//	}
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
					// app icon in action bar clicked; go home
					//Intent intent = new Intent(this, HomeActivity.class);
					//intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
					//startActivity(intent);
				if (FragmentManager.BackStackEntryCount > 0) {
					FragmentManager.PopBackStack ();
				} else {
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
			}
		}
			

		private void DrawerListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
		{
			switch (itemClickEventArgs.Parent.Id)
			{
			case Constants.SecondActivity_DrawerMainMenuId:
				if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionLanguage)) {
					loadDrawerItems (Constants.SecondActivity_DrawerLanguageOptionsId);
				}else if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionExpertise)) {
					loadDrawerItems (Constants.SecondActivity_DrawerExpertiseOptionsId);
				}
				break;

			case Constants.SecondActivity_DrawerLanguageOptionsId:
				if (mDrawerLanguages [itemClickEventArgs.Position].Equals (Constants.DrawerOptionDone)) {
					//mDrawer.CloseDrawer (this.mDrawerList);
					loadDrawerItems (Constants.SecondActivity_DrawerMainMenuId);
				} else {
					mGuideSearch.languageList.Clear ();
					var sparseArray = mDrawerList.CheckedItemPositions;
					mLastSparseArrayLanguages = (SparseBooleanArray) sparseArray.Clone();
					for (var i = 0; i < sparseArray.Size(); i++ )
					{
//						string s = (sparseArray.KeyAt(i) + "=" + sparseArray.ValueAt(i) + ",");
						if (sparseArray.ValueAt (i) == true) {
							mGuideSearch.languageList.Add (mDrawerLanguages [sparseArray.KeyAt (i)]);
						}
						//	mDrawerList.SetItemChecked (itemClickEventArgs.Position, false);
					}
				//	mDrawerList.SetItemChecked (itemClickEventArgs.Position, true);
					GuideFragment gf = FragmentManager.FindFragmentById<GuideFragment> (Resource.Id.fragment_container);
					gf.RefineSearch (mGuideSearch);
				}
				break;

			case Constants.SecondActivity_DrawerExpertiseOptionsId:
				if (mDrawerLanguages [itemClickEventArgs.Position].Equals (Constants.DrawerOptionDone)) {
					loadDrawerItems (Constants.SecondActivity_DrawerMainMenuId);
				} else {
					mGuideSearch.expertiseList.Clear ();
					var sparseArray = mDrawerList.CheckedItemPositions;
					mLastSparseArrayExpertise = (SparseBooleanArray) sparseArray.Clone();
					for (var i = 0; i < sparseArray.Size(); i++ )
					{						
						if (sparseArray.ValueAt (i) == true) {
							mGuideSearch.expertiseList.Add (mDrawerExpertise [sparseArray.KeyAt (i)]);
						}
					}
					GuideFragment gf = FragmentManager.FindFragmentById<GuideFragment> (Resource.Id.fragment_container);
					gf.RefineSearch (mGuideSearch);					
				}
				break;
				
			}				
			//mDrawer.CloseDrawer (this.mDrawerList);
		}
			
		private void loadDrawerItems (int option)
		{
			switch (option) {

			case Constants.SecondActivity_DrawerMainMenuId:
				if (mAdapterMainMenu == null) {
					mAdapterMainMenu = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, mDrawerItems);
				}
				mDrawerList.ChoiceMode = Android.Widget.ChoiceMode.Single; 
				mDrawerList.Adapter = mAdapterMainMenu;
				mDrawerList.Id = Constants.SecondActivity_DrawerMainMenuId;
				break;

			case Constants.SecondActivity_DrawerLanguageOptionsId:
				if (mAdapterLanguages == null) {
					//	mAdapterDone = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, Constants.DrawerOptionDone);
					mAdapterLanguages = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItemMultipleChoice, mDrawerLanguages);
					//mAdapterLanguages = new ArrayAdapter<string> (this, Resource.Layout.menu_item, mDrawerLanguages);
				}
				mDrawerList.Adapter = mAdapterLanguages;
				mDrawerList.ChoiceMode = Android.Widget.ChoiceMode.Multiple; // Multiple
				mDrawerList.Id = Constants.SecondActivity_DrawerLanguageOptionsId;
				if (mLastSparseArrayLanguages != null) {
					for (int i = 0; i < mLastSparseArrayLanguages.Size(); i++ )
					{					
						if (mLastSparseArrayLanguages.ValueAt (i) == true) {
							mDrawerList.SetItemChecked (mLastSparseArrayLanguages.KeyAt (i), true);
						}
						//	mDrawerList.SetItemChecked (itemClickEventArgs.Position, false);
					}
				}
				break;
			case Constants.SecondActivity_DrawerExpertiseOptionsId:
				if (mAdapterExpertise == null) {
					mAdapterExpertise = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItemMultipleChoice, mDrawerExpertise);
				}
				mDrawerList.Adapter = mAdapterExpertise;
				mDrawerList.ChoiceMode = Android.Widget.ChoiceMode.Multiple; // Multiple
				mDrawerList.Id = Constants.SecondActivity_DrawerExpertiseOptionsId;
				if (mLastSparseArrayExpertise != null) {
					for (int i = 0; i < mLastSparseArrayExpertise.Size(); i++ )
					{					
						if (mLastSparseArrayExpertise.ValueAt (i) == true) {
							mDrawerList.SetItemChecked (mLastSparseArrayExpertise.KeyAt (i), true);
						}
						//	mDrawerList.SetItemChecked (itemClickEventArgs.Position, false);
					}
				}
				break;
			}
		}
	}
		
	public class PlacesAutoCompleteAdapter:ArrayAdapter,IFilterable
	{
		public List<string> resultList;
		protected Filter filter;


		public PlacesAutoCompleteAdapter(Context context, int textViewResourceId):base(context, textViewResourceId) {
			filter = new SuggestionsFilter (this);
			resultList = new List<string> ();

		}

		public override int Count {
			get {
				return resultList.Count;
			} 
		}

		public override Java.Lang.Object GetItem(int position) {
			return resultList [position];
		}

		public override Filter Filter {
			get {
				return filter;
			}
		}					
	}

	public class SuggestionsFilter : Filter
	{
		PlacesAutoCompleteAdapter a;

		public SuggestionsFilter (PlacesAutoCompleteAdapter adapter) : base() {
			a = adapter;
		}

		protected override Filter.FilterResults PerformFiltering (Java.Lang.ICharSequence constraint)
		{
			FilterResults filterResults  = new FilterResults();

			if (constraint != null) {
				a.resultList = autocomplete(constraint.ToString());

				Java.Lang.Object[] matchObjects = new Java.Lang.Object[a.resultList.Count];
				for (int i = 0; i < a.resultList.Count; i++) {
					matchObjects[i] = new Java.Lang.String(a.resultList[i]);
				}

				// Assign the data to the FilterResults
				filterResults.Values = matchObjects;
				filterResults.Count = matchObjects.Count();

			}
			return filterResults;
		}

		protected override void PublishResults (Java.Lang.ICharSequence constraint, Filter.FilterResults results)
		{
			a.NotifyDataSetChanged();
		}

		private static string LOG_TAG = "ExampleApp";
		private static string PLACES_API_BASE = "https://maps.googleapis.com/maps/api/place";
		private static string TYPE_AUTOCOMPLETE = "/autocomplete";
		private static string OUT_JSON = "/json";
		private static string API_KEY = "AIzaSyDPRsZJ3iQcO8PdUU1yCjFAKA7etzg7PPM";

		protected List<string> autocomplete(string input) 
		{
			List<string> resultList = null;

			HttpURLConnection conn = null;
			StringBuilder jsonResults = new StringBuilder();

			try {
				StringBuilder sb = new StringBuilder(PLACES_API_BASE + TYPE_AUTOCOMPLETE + OUT_JSON);
				sb.Append("?key=" + API_KEY);
				//    sb.Append("&components=country:uk");
				sb.Append("&input=" + URLEncoder.Encode(input, "utf8"));

				URL url = new URL(sb.ToString());
				conn = (HttpURLConnection) url.OpenConnection();
				InputStreamReader inS;
				inS = new InputStreamReader(conn.InputStream);


				// Load the results into a StringBuilder
				int read;
				char[] buff = new char[1024];
				while ((read = inS.Read(buff)) != -1) {
					jsonResults.Append(buff, 0, read);
				}
			} catch (MalformedURLException e) {
				Log.Error(LOG_TAG, "Error processing Places API URL", e);
				return resultList;
			} catch (System.IO.IOException e) {
				Log.Error(LOG_TAG, "Error connecting to Places API", e);
				return resultList;
			} finally {
				if (conn != null) {
					conn.Disconnect();
				}
			}

			try {
				// Create a JSON object hierarchy from the results
				JSONObject jsonObj = new JSONObject(jsonResults.ToString());
				JSONArray predsJsonArray = jsonObj.GetJSONArray("predictions");

				// Extract the Place descriptions from the results
				resultList = new List<string>();
				for (int i = 0; i < predsJsonArray.Length(); i++) {
					resultList.Add(predsJsonArray.GetJSONObject(i).GetString("description"));
				}
			} catch (JSONException e) {
				Log.Error(LOG_TAG, "Cannot process JSON results", e);
			}

			return resultList;
		}
	}
		
}

