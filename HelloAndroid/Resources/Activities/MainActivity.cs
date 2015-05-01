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

namespace TouriDroid
{
	//[Activity (Label = "HelloAndroid", MainLauncher = true, Icon = "@drawable/icon")]
	/* This is the Activity that displays all the expertises and is the Main Activity */
	[Activity (Label = "Touri", MainLauncher = true, Theme = "@style/Theme.AppCompat")]			
	public class MainActivity : ActionBarActivity, Android.Support.V7.App.ActionBar.ITabListener
	{
		private DrawerLayout 	mDrawer;
		private ListView		mDrawerList;
		ActionBarDrawerToggle 	drawerToggle;
		public string 			mPlace="";
		public Expertise 		mExpertise;
		private GuideSearch 	mGuideSearch;
		protected int 			currentFragment = Constants.Uninitialized;

		private List<string> mDrawerItems = new List<string>
		{
		};

		//Each fragment that associates with this Activity should set it's type here
		//within the fragment code itself (in OnCreateView)
		//This allows the activity to call actionbar functions for the right fragment
		//type is defined in Constants class
		public void setCurrentFragment(int type)
		{
			currentFragment = type;
		}

		//This allows fragment to get the search bar place value from the activity
		public string getPlace()
		{
			return mPlace;
		}

		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature(WindowFeatures.ActionBar);
			base.OnCreate (bundle);
			SessionManager sessionManager = new SessionManager (this);

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

			if (sessionManager.isLoggedIn ()) {
				//mDrawerItems.Add ("Logout");
				if (sessionManager.isGuide ()) {
					mDrawerItems.Add (Constants.DrawerOptionSwitchGuide);
				} else {
					mDrawerItems.Add ("Favourite Guides");
				}
				mDrawerItems.Add (Constants.DrawerOptionLogout);
			} else {
				mDrawerItems.Add(Constants.DrawerOptionBeAGuide);
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

			var newFragment = new ExpertiseFragment ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.main_fragment_container, newFragment);
			ft.Commit ();

		}	

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_guide, menu);

			//set up the action bar menu's search 
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

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			drawerToggle.SyncState ();
		}
						
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
				
		//if the search action bar has a value added
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
				} 
				else if (currentFragment == Constants.GuideFragment) 
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
		}
			
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (drawerToggle.OnOptionsItemSelected (item)) 
			{
				return (true);
			}

			switch (item.ItemId) {
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
			
	}		
}