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
	/* This is the Activity that displays all the expertises and is the Main Activity */
	[Activity (Label="Touri", MainLauncher = true, Theme = "@style/Theme.AppCompat", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]			
	public class MainActivity : ActionBarActivity,  ILocationListener, Android.Support.V7.App.ActionBar.ITabListener
	{

		private LocationManager mLocationManager = null;
		public Location 		mCurrentLocation;
		private SessionManager  mSessionManager;

		private DrawerLayout 	mDrawer;
		private ListView		mDrawerList;
		ActionBarDrawerToggle 	drawerToggle;
		public Expertise 		mExpertise;
		private GuideSearch 	mGuideSearch;
		protected int 			currentFragment = Constants.Uninitialized;
		private	AutoCompleteTextView mSearchPlaces;
		IMenu					mSearchMenu;
		Type					mCurrentFragment;
		public string 			mPlace="";

		private List<string> mDrawerItems = new List<string>
		{
			//blank initially
		};

		//Initialize all the globals for this activity
		private void initializeGlobals()
		{
			Log.Debug (Constants.TOURI_TAG, "initializeGlobals");
			mSessionManager = new SessionManager (this);
			mLocationManager = GetSystemService (LocationService) as LocationManager;
			mGuideSearch = new GuideSearch ();
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			RequestWindowFeature(WindowFeatures.ActionBar);
			base.OnCreate (savedInstanceState);
			SupportActionBar.NavigationMode = (int) ActionBarNavigationMode.Tabs;

			// Set our view from the "main" layout resource
			//SetContentView (Resource.Layout.Second);
			SetContentView (Resource.Layout.Main);

			Log.Info (Constants.TOURI_TAG, "Version= " + Constants.TOURI_VER);
			initializeGlobals ();

			if (mSessionManager.isLoggedIn () == true) {
				//bind to the chat service
				//this way we don't have to invoke multiple connections
				//	var chatServiceIntent = new Intent (this, typeof(ChatService));
				//	MainChatServiceConnection chatServiceConnection = new MainChatServiceConnection (this);

				//	bool connected = BindService (chatServiceIntent, chatServiceConnection, Bind.AutoCreate);
			}

			int returnVal = configureDrawer ();
			if (returnVal == Constants.FAIL) {
				Log.Debug (Constants.TOURI_TAG, "Failed to configure the left drawer");
				return;
			}

			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);

			//fire up the fragment
			string selectedTab = Constants.Search_Tab;
			if (savedInstanceState == null) {
				Bundle extras = Intent.Extras;
				Boolean callChat = false;

				if(extras != null) {
					callChat= extras.GetBoolean("CallChat");
				}

				if (callChat) {
					var newFragment = new ChatListFragment ();
					var ft = FragmentManager.BeginTransaction ();
					ft.Add (Resource.Id.main_fragment_container, newFragment);
					mCurrentFragment = typeof(ChatListFragment);
					ft.Commit ();
					selectedTab = Constants.Chat_Tab;
				} else {
					var newFragment = new ExpertiseFragment ();
					var ft = FragmentManager.BeginTransaction ();
					ft.Add (Resource.Id.main_fragment_container, newFragment);
					mCurrentFragment = typeof(ExpertiseFragment);
					ft.Commit ();					
				}

			}

			returnVal = configureTabs (selectedTab);
			if (returnVal == Constants.FAIL) {
				Log.Debug (Constants.TOURI_TAG, "Failed to configure the tabs");
			}
		}	

		//Set up the menu, including the search bar in the menu
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_guide, menu);

			//set up the action bar menu's search 
			var item = menu.FindItem (Resource.Id.search);
			mSearchMenu = menu;
			View v = (View) MenuItemCompat.GetActionView (item);

			mSearchPlaces = (AutoCompleteTextView) v.FindViewById (Resource.Id.search_places);

			PlacesAutoCompleteAdapter pacAdapter = new PlacesAutoCompleteAdapter (this, Android.Resource.Layout.SimpleListItem1);
			mSearchPlaces.Adapter = pacAdapter;
			mSearchPlaces.ItemClick += searchPlaces_ItemClick;

			//See if we can speed things up by looking for the last location held in memory
			//if the person was logged in, the location would be saved in the session manager
			if (mSessionManager == null) {
				mSessionManager = new SessionManager (this);
			}

			string lastLocation = "";
			//	if (mSessionManager.isLoggedIn ()) {
			lastLocation = mSessionManager.getLastLocation ();

			// only waste time if it's not blank
			if (!lastLocation.Equals ("")) {
				setMyPlace (lastLocation);
			} else {
				setMyPlace ("Markham, Ontario, Canada");
			}
			//	}

			return base.OnCreateOptionsMenu(menu);
		} 

		//sets the top label to a value - usually place in our case
		private void SetActivityLabel(string place)
		{
			char[] splits = {',' };
			string[] placeArray= mSearchPlaces.Text.Split (splits, 3);

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

					ExpertiseFragment ef = FragmentManager.FindFragmentById<ExpertiseFragment> (Resource.Id.main_fragment_container);

					if (ef==null || mSearchMenu == null) {
						Log.Debug (Constants.TOURI_TAG, "search menu is null");
						return;
					}
					ef.loadExpertises ();

					var item = mSearchMenu.FindItem (Resource.Id.search);
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

					if (gf==null) {
						Log.Debug (Constants.TOURI_TAG, "search menu is null");
						return;
					}

					gf.RefineSearch(mGuideSearch);
				}					

			} else {
				//@todo
				Log.Debug(Constants.TOURI_TAG, "search click's sender was null");
				Finish();
			}
		}

		//handle selections on the drawer list
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

				sm.logoutUser();
				StopService (new Intent (this, typeof(ChatService)));

				//Logger logger = new Logger ();

				//logger.LogOut (sm, this);
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
				Intent i = new Intent (this, typeof(GuidingActivity));
				// Closing all the Activities
				i.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
				this.StartActivity (i);
			}
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (drawerToggle.OnOptionsItemSelected (item)) 
			{
				return (true);
			}

			switch (item.ItemId) 
			{
			case Resource.Id.search:
				mSearchPlaces.Text = "";
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
			//turn off the location manager since we only need it at startup
			mLocationManager.RemoveUpdates (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			//string Provider = LocationManager.GpsProvider;

			if (mPlace.Equals ("")) {
				Log.Debug (Constants.TOURI_TAG, "OnResume - mPlace is empty");
				//string Provider = LocationManager.NetworkProvider;
			
				Criteria locationCriteria = new Criteria();

				locationCriteria.Accuracy = Accuracy.Coarse;
				locationCriteria.PowerRequirement = Power.Low;

				string provider = mLocationManager.GetBestProvider (locationCriteria, true);

				if (provider != null)
				{
					mLocationManager.RequestLocationUpdates (provider, 0, 0, this);
				}
				else
				{
					setMyPlace ("Chicago, Illinois, USA");
					Log.Info(Constants.TOURI_TAG, "No location providers available");
				}
			}
		}

		private string reverseFindLocation (double lati, double longi)
		{
			string mPlace = "Toronto, ON, Canada";
			var g = new Geocoder (this);

			Log.Debug (Constants.TOURI_TAG, "reverseFindLocation - lat="+lati.ToString() + ", long="+longi.ToString());
			try
			{
				IList<Address> address =  g.GetFromLocation (lati, longi, 1);
				if (address.Count > 0) {
					mPlace = (address [0].SubLocality ?? (address [0].Locality ?? "")) + ", " + (address [0].AdminArea ?? "") + ", " + (address [0].CountryName ?? "");
				}
			}
			catch (Exception e) {
				Log.Debug (Constants.TOURI_TAG, "reverseFindLocation - Exception: "+e.Message);
			}
			return mPlace;
		}

		public void OnLocationChanged (Location location)
		{
			Log.Debug (Constants.TOURI_TAG, "OnLocationChanged");
			mCurrentLocation = location;

			mLocationManager.RemoveUpdates (this);
			string currentPlace = reverseFindLocation (location.Latitude, location.Longitude);
			setMyPlace (currentPlace);

			if (mSessionManager != null) {
				mSessionManager.setCurrentLatitudeLongitude ((float)location.Latitude, (float)location.Longitude);
				mSessionManager.setCurrentLocation (currentPlace);
			}
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
			mSearchPlaces.Text = address;
			SetActivityLabel (mPlace);
			mGuideSearch.placesServedList.Clear ();
			mGuideSearch.placesServedList.Add (mPlace);
		}

		//Set up the left drawer for this activity 
		//Return SUCCESS or FAIL
		private int configureDrawer ()
		{
			if (mSessionManager == null) {
				return Constants.FAIL;
			}

			if (mSessionManager.isLoggedIn ()) {
				//mDrawerItems.Add ("Logout");
				if (mSessionManager.isGuide ()) {
					mDrawerItems.Add (Constants.DrawerOptionSwitchGuide);
				} else {
					mDrawerItems.Add ("Favourite Guides");
				}
				mDrawerItems.Add (Constants.MyPreferences);

				//this is the last line in the drawer
				TextView drawerFooter = this.FindViewById<TextView> (Resource.Id.drawer_bottom_text1);
				drawerFooter.Text = "Signed in as " + mSessionManager.getEmail ();

				mDrawerItems.Add (Constants.DrawerOptionLogout);

			} else {
				mDrawerItems.Add(Constants.DrawerOptionBeAGuide);
				//drawerFooter.Text = Constants.DrawerOptionLoginOrSignUp;
				mDrawerItems.Add (Constants.DrawerOptionLoginOrSignUp);
			}

			mDrawer = this.FindViewById<DrawerLayout> (Resource.Id.main_drawer_layout);
			mDrawerList = this.FindViewById<ListView> (Resource.Id.main_left_drawer);	

			mDrawerList.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, mDrawerItems.ToArray());
			drawerToggle = new ActionBarDrawerToggle (this, mDrawer,Resource.String.drawer_open, Resource.String.drawer_close);

			mDrawer.SetDrawerListener (drawerToggle);
			mDrawerList.ItemClick += DrawerListOnItemClick;

			return Constants.SUCCESS;
		}

		//Set up the left drawer for this activity 
		private int configureTabs (string selectedTab)
		{
			Boolean selected = false;
			try
			{
				Android.Support.V7.App.ActionBar.Tab tab = SupportActionBar.NewTab ();
				tab.SetText (Constants.Search_Tab);
				tab.SetTabListener(this);
				if (selectedTab.Equals(tab.Text))
				{
					selected = true;
				}
				SupportActionBar.AddTab(tab, selected);
				selected = false;

				tab =  SupportActionBar.NewTab ();
				tab.SetTabListener(this);
				tab.SetText (Constants.Chat_Tab);
				if (selectedTab.Equals(tab.Text))
				{
					selected = true;
				}

				SupportActionBar.AddTab (tab, selected);

			}
			catch (Exception e) {
				Log.Debug (Constants.TOURI_TAG, e.InnerException.ToString ());
				return Constants.FAIL;
			}

			return Constants.SUCCESS;
		}
	}		
}