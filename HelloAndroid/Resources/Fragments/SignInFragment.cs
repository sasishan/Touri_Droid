﻿
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
	public class SignInFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public async void Login(View view, string username, string password)
		{
			LoginService ls = new LoginService ();

			String token = await ls.Login (username, password);

			if (token!=null) {
				SessionManager sessionManager = new SessionManager (view.Context);
				sessionManager.createLoginSession (username, token);

				Toast.MakeText (view.Context, "Successfully logged in", ToastLength.Long).Show ();

				Intent i = new Intent(view.Context, typeof(MainActivity));
				// Closing all the Activities
				i.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
				this.StartActivity (i);
			
			} else {
				Toast.MakeText (view.Context, "Error logging in", ToastLength.Long).Show ();
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.SignIn, container, false);
			SessionManager sm = new SessionManager (view.Context);


			EditText username =view.FindViewById<EditText> (Resource.Id.username);
			EditText password =view.FindViewById<EditText> (Resource.Id.password);
			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			string email = sm.getEmail ();
			if (email != null) {
				username.Text = email;				
			}

			next.Click += (object IntentSender, EventArgs e) => {

				if (!username.Text.Equals("") && !password.Text.Equals("") )
				{					
					if (Constants.isValidEmail(username.Text))
					{
						Login(view, username.Text, password.Text);
					}
					else
					{
						Toast.MakeText (view.Context, "Enter a valid email address", ToastLength.Long).Show();
					}						
				}
				else
				{
					Toast.MakeText (view.Context, "Please enter a valid email address and password", ToastLength.Long).Show();
				}
			};				

			return view;
		}
	}
}

