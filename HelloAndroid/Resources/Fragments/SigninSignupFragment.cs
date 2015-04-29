
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TouriDroid
{
	public class SigninSignupFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_signinsignup, container, false);


			Button signUp = view.FindViewById<Button> (Resource.Id.buttonSignup);
			Button signIn = view.FindViewById<Button> (Resource.Id.buttonSignIn);

			signIn.Click += (object IntentSender, EventArgs e) => {
				var newFragment = new SignInFragment ();

				FragmentTransaction transaction = FragmentManager.BeginTransaction();
				transaction.Replace(Resource.Id.signinup_fragment_container, newFragment);

				transaction.Commit();
			};

			signUp.Click += (object IntentSender, EventArgs e) => {
				var newFragment = new SignupRegisterGuide ();

				FragmentTransaction transaction = FragmentManager.BeginTransaction();
				transaction.Replace(Resource.Id.signinup_fragment_container, newFragment);

				transaction.Commit();
			};


			return view;
		}
	}
}

