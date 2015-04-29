
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

namespace TouriDroid
{
	[Activity (Label = "Log In or Sign Up")]			
	public class LoginOrSignupActivity : Activity
	{
		public Guide newGuide;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.SignInSignUp_layout);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			newGuide = new Guide ();
			//load the Guide Fragment
			var newFragment = new SigninSignupFragment ();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.signinup_fragment_container, newFragment);
			ft.Commit ();
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
	}
}

