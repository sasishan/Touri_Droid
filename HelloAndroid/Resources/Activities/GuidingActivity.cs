
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
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace TouriDroid
{
	[Activity (Label = "Guiding", MainLauncher = true, Theme = "@style/Theme.AppCompat")]		
	public class GuidingActivity : ActionBarActivity, Android.Support.V7.App.ActionBar.ITabListener
	{
		private DrawerLayout 	mDrawer;
		private ListView		mDrawerList;
		ActionBarDrawerToggle 	drawerToggle;
		public Guide			currentGuide;

		private List<string> mDrawerItems = new List<string>
		{
		};

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			SupportActionBar.NavigationMode = (int) ActionBarNavigationMode.Tabs;

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			Android.Support.V7.App.ActionBar.Tab tab = SupportActionBar.NewTab ();
			tab.SetText ("My Profile");
			tab.SetTabListener(this);
			SupportActionBar.AddTab (tab);

			tab =  SupportActionBar.NewTab ();
			tab.SetTabListener(this);
			tab.SetText ("Chat");
			SupportActionBar.AddTab (tab);

			SessionManager sessionManager = new SessionManager (this);
			TextView drawerFooter = this.FindViewById<TextView> (Resource.Id.drawer_bottom_text1);

			if (sessionManager.isLoggedIn ()) {
				//mDrawerItems.Add ("Logout");
				mDrawerItems.Add (Constants.DrawerOptionSwitchTravel);
				mDrawerItems.Add (Constants.DrawerOptionLogout);
				drawerFooter.Text = "Signed in as " + sessionManager.getEmail ();
			} else {
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

//			var newFragment = new GuidingFragment ();
//			var ft = FragmentManager.BeginTransaction ();
//			ft.Add (Resource.Id.main_fragment_container, newFragment);
//			ft.Commit ();
		}

		private void DrawerListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
		{
			//Log out and refresh the screen
			if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionLogout)) 
			{
				SessionManager sm = new SessionManager (this);
				sm.logoutUser ();
				mDrawer.CloseDrawers ();

				Intent i = new Intent (this, typeof(MainActivity));
				// Closing all the Activities
				i.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
				this.StartActivity (i);
			} 
			else if (mDrawerItems [itemClickEventArgs.Position].Equals (Constants.DrawerOptionSwitchTravel)) {
				mDrawer.CloseDrawers ();
				this.StartActivity (typeof(MainActivity));
			}
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

			switch (item.ItemId) {
			case Android.Resource.Id.Home:
				Finish ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}

		public void OnTabReselected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Optionally refresh/update the displayed tab.
		}

		public void OnTabSelected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			if (tab.Position == Constants.Main_Chat_Tab) {
				var newFragment = new ChatListFragment ();
				transaction.Replace (Resource.Id.main_fragment_container, newFragment);
			} else if (tab.Position == Constants.Main_Expertise_Tab) {
				var newFragment = new GuidingFragment ();
				transaction.Replace (Resource.Id.main_fragment_container, newFragment);			
			}						
			transaction.Commit ();
		}

		public void OnTabUnselected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			// Save any state in the displayed fragment.
			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			Fragment currentFrag= FragmentManager.FindFragmentById(Resource.Id.main_fragment_container);

			transaction.Remove (currentFrag);
			transaction.Commit ();
		}

	}
}

