using System;
using System.Net;
using System.IO;
using System.Json;
using System.Threading.Tasks;

using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Views;
using System.Net.Http;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Util;



namespace HelloAndroid
{
	//[Activity (Label = "HelloAndroid", MainLauncher = true, Icon = "@drawable/icon")]
	[Activity (Label = "Touri", MainLauncher = true, Theme = "@style/Theme.AppCompat")]			
	public class MainActivity : ActionBarActivity, Android.Support.V7.App.ActionBar.ITabListener
	{
		private DrawerLayout mDrawer;
		private ListView mDrawerList;
		ActionBarDrawerToggle drawerToggle;
		public string mPlace="";
		public Expertise mExpertise;
		private GuideSearch mGuideSearch;
		protected int currentFragment=Constants.Uninitialized;

		public void OnTabReselected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Optionally refresh/update the displayed tab.
			//Log.Debug(Tag, "The tab {0} was re-selected.", tab.Text);
		}

		public void OnTabSelected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Display the fragment the user should see
			//Log.Debug(Tag, "The tab {0} has been selected.", tab.Text);
		}

		public void OnTabUnselected(Android.Support.V7.App.ActionBar.Tab  tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Save any state in the displayed fragment.
			//Log.Debug(Tag, "The tab {0} as been unselected.", tab.Text);
		}

		void AddTabToActionBar(int labelResourceId, int iconResourceId)
		{

		}
			
		public void setCurrentFragment(int type)
		{
			currentFragment = type;
		}

		public string getPlace()
		{
			return mPlace;
		}

		public FragmentManager getCurrentFragmentManager()
		{
			return FragmentManager;
		}

		private static readonly string[] mDrawerItems = new []
		{
			"Settings", "Profile"
		};
						
		public override bool OnKeyUp(Android.Views.Keycode keyCode, Android.Views.KeyEvent e) {			
			if (keyCode == Android.Views.Keycode.Back && e.RepeatCount == 0) {
				if (FragmentManager.BackStackEntryCount > 0) {
					FragmentManager.PopBackStack ();
					return true;
				} else {
					return base.OnKeyUp (keyCode, e); 
				}
			} else {
				return base.OnKeyUp (keyCode, e); 
			}

		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			drawerToggle.SyncState ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature(WindowFeatures.ActionBar);
			base.OnCreate (bundle);


			SupportActionBar.NavigationMode = (int) ActionBarNavigationMode.Tabs;
			// Set our view from the "main" layout resource
			//SetContentView (Resource.Layout.Second);
			SetContentView (Resource.Layout.Main);
			Android.Support.V7.App.ActionBar.Tab tab = SupportActionBar.NewTab ();
			tab.SetText ("Search");
			tab.SetTabListener(this);
			SupportActionBar.AddTab (tab);

			tab =  SupportActionBar.NewTab ();
			tab.SetTabListener(this);
			tab.SetText ("Chat");

			SupportActionBar.AddTab (tab);

			mDrawer = this.FindViewById<DrawerLayout> (Resource.Id.main_drawer_layout);
			mDrawerList = this.FindViewById<ListView> (Resource.Id.main_left_drawer);

			mDrawerList.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, mDrawerItems);
			//loadDrawerItems (Constants.DrawerMainMenuId);
			//mDrawerList.Id = Constants.DrawerMainMenuId;
			drawerToggle = new ActionBarDrawerToggle (this, mDrawer,Resource.String.drawer_open, Resource.String.drawer_close);

			mDrawer.SetDrawerListener (drawerToggle);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);

			mGuideSearch = new GuideSearch ();

			var newFragment = new ExpertiseFragment ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.main_fragment_container, newFragment);
			ft.Commit ();

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

			//@remove replace with get current location
			searchPlaces.Text = "Toronto, ON, Canada";
			mPlace = searchPlaces.Text;

			return base.OnCreateOptionsMenu(menu);

		} 

		private void searchPlaces_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (sender != null) {

				if (currentFragment == Constants.ExpertiseFragment) 
				{
					string place = ((AutoCompleteTextView)sender).Text;
					mPlace = place;
					string url = Constants.DEBUG_BASE_URL + "/api/expertises/search?locs="+place;

					//mGuideSearch.placesServedList.Clear ();
					//mGuideSearch.placesServedList.Add (place);
					ExpertiseFragment ef = FragmentManager.FindFragmentById<ExpertiseFragment> (Resource.Id.main_fragment_container);
					ef.loadExpertises (url);					
				} else if (currentFragment == Constants.GuideFragment) 
				{
					string place = ((AutoCompleteTextView)sender).Text;
					mGuideSearch.placesServedList.Clear ();
					mGuideSearch.placesServedList.Add (place);
					GuideFragment gf = FragmentManager.FindFragmentById<GuideFragment> (Resource.Id.main_fragment_container);
					gf.RefineSearch(mGuideSearch);
				}					

			} else {
				//@todo
			}
		}

			
		public async void loadExpertises(string url)
		{
			CallAPI ca = new CallAPI();

			//List<Guide> myGuides = new List<Guide> ();
			url = Constants.DEBUG_BASE_URL + Constants.URL_Get_All_Expertises;
				
			/****** Uncomment when webservice is running *****/
			var json = await ca.getWebApiData(url);
			int imageId = parseExpertises(json);

			url = Constants.DEBUG_BASE_URL + "/api/images/"+imageId;

			Bitmap image = (Bitmap) await ca.getImage (url);

			ImageView expertiseImage = FindViewById<ImageView> (Resource.Id.expertise_image);
			expertiseImage.SetImageBitmap (image);
			//mGuideList = myGuides;
			//mAdapter.NotifyDataSetChanged ();
			//			mAdapter = new RecyclerAdapter (mGuideList, this.Activity);
			//		mRecyclerView.SetAdapter (mAdapter);
			//RecyclerView.ItemDecoration itemDecoration =new DividerItemDecoration(this, DividerItemDecoration.VERTICAL_LIST);
			//new DividerItemDecoration

		}

		private int parseExpertises(JsonValue json)
		{
			int imageId=0;

			for (int i = 0; i < json.Count; i++) {

				JsonValue values = json [i];
				if (values.ContainsKey ("expertise")) {
					string expertise = values ["expertise"];
					imageId = values ["imageId"];
				}					
			}
			return imageId;

		}
			
	}


	public class CallAPIOld
	{
		public CallAPIOld()
		{
		}

		public async void loadGuideProfiles(TextView text, List<Guide> guideList)
		{
			string url ="http://192.168.0.12:50467/api/guides";
			CallAPI ca = new CallAPI();

			JsonValue json = await ca.getWebApiData(url);
			parseGuideProfiles(json, text, guideList);
			text.Text = guideList.Count + " guide profiles loaded";


		}

		private void parseGuideProfiles(JsonValue json, TextView text, List<Guide> gL)
		{
			for (int i = 0; i < json.Count; i++) {
				Guide g = new Guide ();
				JsonValue values = json [i];
				if (values.ContainsKey ("fName")) {
					string fName = values ["fName"];

					g.fName = fName;
					g.guideId = values ["guideId"];

				}
				if (values.ContainsKey ("lName")) {
					string lName = values ["lName"];
					g.lName= lName;
				}
				if (values.ContainsKey ("languages")) {
					JsonValue temp = values ["languages"];
					for (int j=0; j<temp.Count;j++)
					{
						JsonValue l = temp[j];
						g.languageList.Add (l ["language"]);
					}

					if (values.ContainsKey ("languages")) {
					}
					string lName = values ["lName"];
					g.lName= lName;
				}
				gL.Add (g);
			}
		}

		public async Task<JsonValue> getGuideProfiles (string url)
		{
			// Create an HTTP web request using the URL:
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
			request.ContentType = "application/json";
			request.Method = "GET";

			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync ())
			{
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream ())
				{
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run (() => JsonObject.Load (stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

					// Return the JSON document:
					return jsonDoc;
				}
			}
		}
	}


	/*public class CallAPI: AsyncTask
	{	
		protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] parms)
		{
			var request = HttpWebRequest.Create(string.Format(@"http://192.168.0.12:50467/api/guides"));
			request.ContentType = "application/json";
			request.Method = "GET";
			string resultToDisplay = "";

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				if (response.StatusCode != HttpStatusCode.OK)
					Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					if(string.IsNullOrWhiteSpace(content)) {
						resultToDisplay= "Response contained empty body...";
					}
					else {
						resultToDisplay = string.Format ("Response Body: \r\n {0}", content);
					}

					//Assert.NotNull(content);
				}
			}

			return resultToDisplay;
		}

		protected override void OnPostExecute(Java.Lang.Object result)
		{
		}

	} // end CallAPI*/
}


