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
using Android.Content;
using Android.Views.InputMethods;
using Android.Locations;
using Android.Gms.Maps.Model;

namespace TouriDroid
{
	//[Activity (Label = "HelloAndroid", MainLauncher = true, Icon = "@drawable/icon")]
	/* This is the Activity that displays all the expertises and is the Main Activity */
	[Activity (Label="Touri", MainLauncher = true, Theme = "@style/Theme.AppCompat", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]			
	public class MainActivity : ActionBarActivity,  ILocationListener, Android.Support.V7.App.ActionBar.ITabListener
	{
		private LocationManager _locationManager = null;
		public Location 		_currentLocation;

		private DrawerLayout 	mDrawer;
		private ListView		mDrawerList;
		ActionBarDrawerToggle 	drawerToggle;
		public Expertise 		mExpertise;
		private GuideSearch 	mGuideSearch;
		protected int 			currentFragment = Constants.Uninitialized;
		private	AutoCompleteTextView searchPlaces;
		IMenu					searchMenu;
		private SessionManager sessionManager;
		Type					mCurrentFragment;
		public string 			mPlace="";

		private List<string> mDrawerItems = new List<string>
		{
			//blank initially
		};


		protected override void OnCreate (Bundle savedInstanceState)
		{
			RequestWindowFeature(WindowFeatures.ActionBar);
			base.OnCreate (savedInstanceState);

			_locationManager = GetSystemService (LocationService) as LocationManager;

			sessionManager = new SessionManager (this);

			SupportActionBar.NavigationMode = (int) ActionBarNavigationMode.Tabs;

			// Set our view from the "main" layout resource
			//SetContentView (Resource.Layout.Second);
			SetContentView (Resource.Layout.Main);

			TextView drawerFooter = this.FindViewById<TextView> (Resource.Id.drawer_bottom_text1);

			if (sessionManager.isLoggedIn ()) {
				//mDrawerItems.Add ("Logout");
				if (sessionManager.isGuide ()) {
					mDrawerItems.Add (Constants.DrawerOptionSwitchGuide);
				} else {
					mDrawerItems.Add ("Favourite Guides");
				}
				mDrawerItems.Add (Constants.MyPreferences);
				drawerFooter.Text = "Signed in as " + sessionManager.getEmail ();
				mDrawerItems.Add (Constants.DrawerOptionLogout);

			} else {
				mDrawerItems.Add(Constants.DrawerOptionBeAGuide);
				//drawerFooter.Text = Constants.DrawerOptionLoginOrSignUp;
				mDrawerItems.Add (Constants.DrawerOptionLoginOrSignUp);
			}

			mDrawer = this.FindViewById<DrawerLayout> (Resource.Id.main_drawer_layout);
			mDrawerList = this.FindViewById<ListView> (Resource.Id.main_left_drawer);

			mDrawerList.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, mDrawerItems.ToArray());
			//loadDrawerItems (Constants.DrawerMainMenuId);
			//mDrawerList.Id = Constants.DrawerMainMenuId;
			drawerToggle = new ActionBarDrawerToggle (this, mDrawer,Resource.String.drawer_open, Resource.String.drawer_close);

			mDrawer.SetDrawerListener (drawerToggle);
			mDrawerList.ItemClick += DrawerListOnItemClick;

			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);

			mGuideSearch = new GuideSearch ();

			if (savedInstanceState == null) {
				var newFragment = new ExpertiseFragment ();
				var ft = FragmentManager.BeginTransaction ();
				ft.Add (Resource.Id.main_fragment_container, newFragment);
				ft.Commit ();
			}
			mCurrentFragment = typeof(ExpertiseFragment);

			Android.Support.V7.App.ActionBar.Tab tab = SupportActionBar.NewTab ();
			tab.SetText ("Search");
			tab.SetTabListener(this);
			SupportActionBar.AddTab (tab);

			tab =  SupportActionBar.NewTab ();
			tab.SetTabListener(this);
			tab.SetText ("Chat");

			SupportActionBar.AddTab (tab);
		}	

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_guide, menu);

			//set up the action bar menu's search 
			var item = menu.FindItem (Resource.Id.search);
			searchMenu = menu;
			View v = (View) MenuItemCompat.GetActionView (item);

			searchPlaces = (AutoCompleteTextView) v.FindViewById (Resource.Id.search_places);
			//AutoCompleteTextView searchPlaces = (AutoCompleteTextView)FindViewById (Resource.Id.search_places);

			PlacesAutoCompleteAdapter pacAdapter = new PlacesAutoCompleteAdapter (this, Android.Resource.Layout.SimpleListItem1);
			searchPlaces.Adapter = pacAdapter;
			searchPlaces.ItemClick += searchPlaces_ItemClick;

			// See if we can speed things up by looking for the last location held in memory
			string lastLocation = "";
			SessionManager sm = new SessionManager (this);
			if (sessionManager.isLoggedIn ()) {
				lastLocation = sm.getLastLocation ();

				// only waste time if it's not blank
				if (!lastLocation.Equals ("")) {
					setMyPlace (lastLocation);
				}
			}

			return base.OnCreateOptionsMenu(menu);

		} 

		//sets the top label to a valeu - usually place in our case
		private void SetActivityLabel(string place)
		{
			char[] splits = {',' };
			string[] placeArray= searchPlaces.Text.Split (splits, 3);

			if (placeArray.Length > 1) {
				this.Title = placeArray [0] + ", " + placeArray [1];
			} else {
				this.Title = placeArray [0] ;
			}
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			drawerToggle.SyncState ();
		}
						
		//if the search action bar has a value added
		private void searchPlaces_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (sender != null) {
				if (mCurrentFragment == typeof(ExpertiseFragment)) 
				{
					string place = ((AutoCompleteTextView)sender).Text;
					setMyPlace (place);
					//string url = Constants.DEBUG_BASE_URL + "/api/expertises/search?locs="+place;

					ExpertiseFragment ef = FragmentManager.FindFragmentById<ExpertiseFragment> (Resource.Id.main_fragment_container);
					ef.loadExpertises ();	

					var item = searchMenu.FindItem (Resource.Id.search);
					item.CollapseActionView ();
					//ef.View.RequestFocus ();
					InputMethodManager imm = (InputMethodManager) GetSystemService(Activity.InputMethodService);
					imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
										
				} 
				else if (mCurrentFragment == typeof(GuideFragment) )
				{
					string place = ((AutoCompleteTextView)sender).Text;

					setMyPlace (place);
					GuideFragment gf = FragmentManager.FindFragmentById<GuideFragment> (Resource.Id.main_fragment_container);
					gf.RefineSearch(mGuideSearch);
				}					

			} else {
				//@todo
			}
		}

		private void DrawerListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
		{
			//Be a guide - the user is going to register as a guide
			if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionBeAGuide)) {
				mDrawer.CloseDrawers ();
				this.StartActivity (typeof(SignUpAsGuideActivity));
			}
			//Log out and refresh the screen
			else if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionLogout)) 
			{
				SessionManager sm = new SessionManager (this);
				sm.logoutUser ();
				mDrawer.CloseDrawers ();

				Intent i = new Intent (this, typeof(MainActivity));
				// Closing all the Activities
				i.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
				this.StartActivity (i);
			} 
			//Show the options to register or sign in as a guide
			else if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionLoginOrSignUp)) 
			{
				mDrawer.CloseDrawers ();
				this.StartActivity (typeof(LoginOrSignupActivity));
			}
			else if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionSwitchGuide)) {
				mDrawer.CloseDrawers ();
				this.StartActivity (typeof(GuidingActivity));
			}
		}
			
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (drawerToggle.OnOptionsItemSelected (item)) 
			{
				return (true);
			}

			switch (item.ItemId) {
			case Resource.Id.search:
				searchPlaces.Text = "";
				return base.OnOptionsItemSelected (item);
			case Android.Resource.Id.Home:
				Finish ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}

		// TAB Code
		public void OnTabReselected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Optionally refresh/update the displayed tab.
			//Log.Debug(Tag, "The tab {0} was re-selected.", tab.Text);
		}

		public void OnTabChanged (string tabId)
		{
		}

		public void OnTabSelected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Tab0 = main, Tab1 = chat
			// Display the Chat fragment

			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			if (tab.Position == Constants.Main_Chat_Tab) {
				var newFragment = new ChatListFragment ();

				if (mCurrentFragment != typeof( ChatListFragment)) {
					mCurrentFragment = typeof( ChatListFragment);
					transaction.Replace (Resource.Id.main_fragment_container, newFragment);
				}
			} else if (tab.Position == Constants.Main_Expertise_Tab) {
				var newFragment = new ExpertiseFragment ();
				if (mCurrentFragment != typeof(ExpertiseFragment)) {
					mCurrentFragment = typeof( ExpertiseFragment);
					transaction.Replace (Resource.Id.main_fragment_container, newFragment);			
				}
			}						
			transaction.Commit ();
		}

		public void OnTabUnselected(Android.Support.V7.App.ActionBar.Tab  tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Save any state in the displayed fragment.
			//Log.Debug(Tag, "The tab {0} as been unselected.", tab.Text);
			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			Fragment currentFrag= FragmentManager.FindFragmentById(Resource.Id.main_fragment_container);

			transaction.Remove (currentFrag);
			transaction.Commit ();

		}

		void AddTabToActionBar(int labelResourceId, int iconResourceId)
		{

		}

		//Locationmanager Listener code
		protected override void OnPause ()
		{
			base.OnPause ();
			_locationManager.RemoveUpdates (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			//string Provider = LocationManager.GpsProvider;

			if (mPlace.Equals ("")) {
				string Provider = LocationManager.NetworkProvider;

				if (_locationManager.IsProviderEnabled (Provider)) {
					_locationManager.RequestLocationUpdates (Provider, 0, 0, this);
				} else {
					Log.Info ("loc", Provider + " is not available. Does the device have location services enabled?");
				}
			}
		}

		private string reverseFindLocation (double lati, double longi)
		{
			string mPlace = "Toronto, ON, Canada";
			var g = new Geocoder (this);

			IList<Address> address = g.GetFromLocation (lati, longi,1);
			if (address.Count > 0) {
				mPlace = (address [0].SubLocality ?? (address [0].Locality ?? "")) + ", " + (address [0].AdminArea ?? "") + ", " + (address [0].CountryName ?? "");
			}
			return mPlace;
		}

		public void OnLocationChanged (Location location)
		{
			_currentLocation = location;
			sessionManager.setCurrentLatitudeLongitude ((float) location.Latitude, (float) location.Longitude);
			//LatLng loc = new LatLng (_currentLocation.Latitude, _currentLocation.Longitude);
			//setMapLocation (loc, "Your Location", "", BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueCyan));
			//setCameraLocation(loc);
			//markGuides ();
			_locationManager.RemoveUpdates (this);
			string mPlace = reverseFindLocation (location.Latitude, location.Longitude);
			setMyPlace (mPlace);
			//GuideFragment gf = Activity.F<GuideFragment> (Resource.Id.main_fragment_container);
		//	gf.RefineSearch(mGuideSearch);

		//	address[0].

		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}


		/*Initializer functions */

		//Each fragment that associates with this Activity should set it's type here
		//within the fragment code itself (in OnCreateView)
		//This allows the activity to call actionbar functions for the right fragment
		//type is defined in Constants class
		public void setCurrentFragment(Type type)
		{
			mCurrentFragment = type;
		}

		//This allows fragment to get the search bar place value from the activity
		public string getPlace()
		{
			return mPlace;
		}

		public void setMyPlace(string address)
		{
			mPlace = address;
			searchPlaces.Text = address;
			SetActivityLabel (mPlace);
			mGuideSearch.placesServedList.Clear ();
			mGuideSearch.placesServedList.Add (mPlace);
		}

	}		
}