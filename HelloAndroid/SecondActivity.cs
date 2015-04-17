
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


namespace HelloAndroid
{
	[Activity (Label = "Nativus", Theme = "@style/Theme.AppCompat")]			
	public class SecondActivity : ActionBarActivity
	{
		private DrawerLayout mDrawer;
		private ListView mDrawerList;
		//List<string> mDrawerItems = new List<string>();
		ActionBarDrawerToggle drawerToggle;
		private ArrayAdapter<string> mAdapterLanguages=null;
		private ArrayAdapter<string> mAdapterMainMenu=null;
		private ArrayAdapter<string> mAdapterExpertise=null;
		private GuideSearch mGuideSearch;
		private SparseBooleanArray mLastSparseArrayLanguages;
		private SparseBooleanArray mLastSparseArrayExpertise;
		public string mPlace="";
		public string mExpertise="";

		private static string[] placeSuggestions = {
			"Toronto", "Sao Paulo", "Rio de Janeiro",
			"Bahia", "Mato Grosso", "Minas Gerais",
			"Tocantins", "Rio Grande do Sul"
		};


		//private MyActionBarDrawerToggle drawerToggle;

		private static readonly string[] mDrawerItems = new []
		{
			Constants.DrawerOptionLanguage, "Contact Method", "Rates"
			//Constants.DrawerOptionLanguage, Constants.DrawerOptionExpertise, "Contact Method", "Rates"
		};

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

			//@remove
			mPlace = Intent.GetStringExtra ("location") ?? "";
			mExpertise = Intent.GetStringExtra ("expertise") ?? "";
			// Create your application here
			SetContentView (Resource.Layout.Second);


			mGuideSearch = new GuideSearch ();
			//mGuideSearch.placesServedList.Add ("Toronto, ON, Canada");

			mDrawer = this.FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			mDrawerList = this.FindViewById<ListView> (Resource.Id.left_drawer);

			//mDrawerList.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, mDrawerItems);
			loadDrawerItems (Constants.DrawerMainMenuId);
			//mDrawerList.Id = Constants.DrawerMainMenuId;
			drawerToggle = new ActionBarDrawerToggle (this, mDrawer,Resource.String.drawer_open, Resource.String.drawer_close);

			mDrawer.SetDrawerListener (drawerToggle);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);
			SupportActionBar.Title = mExpertise;				
			mDrawerList.ItemClick += DrawerListOnItemClick;

			//var places = new string [] { "Toronto", "Halifax", "Vancouver", "New York"};
			//mSearchListView = FindViewById<ListView> (Resource.Id.SearchListview);
			//mSearchAdapter = new SimpleAdapter (this, Android.Resource.Layout.SimpleListItem1, null, from, to);
			//mSearchListAdapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, places);
			//mSearchListView.Adapter = mSearchListAdapter;

			var newFragment = new GuideFragment ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.fragment_container, newFragment);
			ft.Commit ();
		}
			
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			drawerToggle.SyncState ();
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (drawerToggle.OnOptionsItemSelected (item)) 
			{
				return (true);
			}

			return base.OnOptionsItemSelected (item);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_guide, menu);

			var item = menu.FindItem (Resource.Id.search);
			View v = (View) MenuItemCompat.GetActionView (item);
			AutoCompleteTextView searchPlaces = (AutoCompleteTextView) v.FindViewById (Resource.Id.search_places);

			//AutoCompleteTextView searchPlaces = (AutoCompleteTextView)FindViewById (Resource.Id.search_places);

			PlacesAutoCompleteAdapter pacAdapter = new PlacesAutoCompleteAdapter (this, Android.Resource.Layout.SimpleListItem1);
			searchPlaces.Adapter = pacAdapter;
			searchPlaces.ItemClick += searchPlaces_ItemClick;

			//@remove
			mPlace = Intent.GetStringExtra ("location") ?? "";
			string expertise = Intent.GetStringExtra ("expertise") ?? "";
			searchPlaces.Text = mPlace;
			mGuideSearch.placesServedList.Clear ();
			mGuideSearch.placesServedList.Add (mPlace);
			mGuideSearch.expertiseList.Add (expertise);
			GuideFragment gf = FragmentManager.FindFragmentById<GuideFragment> (Resource.Id.fragment_container);
			gf.RefineSearch(mGuideSearch);

			//ArrayAdapter<String> adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, placeSuggestions);
			//searchPlaces.Adapter = adapter;

			//var item = menu.FindItem (Resource.Id.action_search);
			//var searchView = MenuItemCompat.GetActionView (item);
			//mSearchView = searchView.JavaCast<SearchView> ();
			//mSearchView.SetOnQueryTextListener ();
			//mSearchView.QueryTextChange+=(s,else)=> mSearchListAdapter.F

			//SearchManager searchManager = (SearchManager)GetSystemService (Context.SearchService);
			//SearchView searchView = (SearchView)menu.FindItem (Resource.Id.action_search).ActionView;
			//ComponentName n = new ComponentName ("component.android.);
			//searchView.SetSearchableInfo(searchManager.GetSearchableInfo(SecondActivity));
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

			//	string place = 
			} else {
				//@todo
			}
		}
			

		private void DrawerListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
		{
			//Fragment fragment = null;
	//		mDrawerList.SetItemChecked (itemClickEventArgs.Position, true);
//			SupportActionBar.Title = mDrawerItems [itemClickEventArgs.Position];

			switch (itemClickEventArgs.Parent.Id)
			{
			case Constants.DrawerMainMenuId:
				if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionLanguage)) {
					loadDrawerItems (Constants.DrawerLanguageOptionsId);
				}else if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionExpertise)) {
					loadDrawerItems (Constants.DrawerExpertiseOptionsId);
				}
				break;

			case Constants.DrawerLanguageOptionsId:
				if (mDrawerLanguages [itemClickEventArgs.Position].Equals (Constants.DrawerOptionDone)) {
					//mDrawer.CloseDrawer (this.mDrawerList);
					loadDrawerItems (Constants.DrawerMainMenuId);
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

			case Constants.DrawerExpertiseOptionsId:
				if (mDrawerLanguages [itemClickEventArgs.Position].Equals (Constants.DrawerOptionDone)) {
					loadDrawerItems (Constants.DrawerMainMenuId);
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

			case Constants.DrawerMainMenuId:
				if (mAdapterMainMenu == null) {
					mAdapterMainMenu = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, mDrawerItems);
				}
				mDrawerList.ChoiceMode = Android.Widget.ChoiceMode.Single; 
				mDrawerList.Adapter = mAdapterMainMenu;
				mDrawerList.Id = Constants.DrawerMainMenuId;
				break;

			case Constants.DrawerLanguageOptionsId:
				if (mAdapterLanguages == null) {
					//	mAdapterDone = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, Constants.DrawerOptionDone);
					mAdapterLanguages = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItemMultipleChoice, mDrawerLanguages);
				}
				mDrawerList.Adapter = mAdapterLanguages;
				mDrawerList.ChoiceMode = Android.Widget.ChoiceMode.Multiple; // Multiple
				mDrawerList.Id = Constants.DrawerLanguageOptionsId;
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
			case Constants.DrawerExpertiseOptionsId:
				if (mAdapterExpertise == null) {
					mAdapterExpertise = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItemMultipleChoice, mDrawerExpertise);
				}
				mDrawerList.Adapter = mAdapterExpertise;
				mDrawerList.ChoiceMode = Android.Widget.ChoiceMode.Multiple; // Multiple
				mDrawerList.Id = Constants.DrawerExpertiseOptionsId;
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
			//	sb.Append("&components=country:uk");
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

