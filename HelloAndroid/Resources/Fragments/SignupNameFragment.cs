
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
using System.Collections.Specialized;
using System.Json;

namespace TouriDroid
{

	public class SignupNameFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.SignupName, container, false);

			EditText fname =view.FindViewById<EditText> (Resource.Id.fname);
			EditText lname =view.FindViewById<EditText> (Resource.Id.lname);
			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			next.Click += (object IntentSender, EventArgs e) => {

				if (!fname.Text.Equals("") && !fname.Text.Equals("") )
				{					
					((SignUpAsGuideActivity)Activity).newGuide.fName = fname.Text;
					((SignUpAsGuideActivity)Activity).newGuide.lName = lname.Text;		

					var newFragment = new SignupRegisterGuide ();

					FragmentTransaction transaction = FragmentManager.BeginTransaction();
					transaction.Replace(Resource.Id.signinup_fragment_container, newFragment);

					transaction.Commit();
				}
				else
				{
					Toast.MakeText (view.Context, "Please enter your first and last names", ToastLength.Long).Show();
				}
			};
			return view;
		}
	}
}

