
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
	public class EditDescriptionFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.Guide_Description, container, false);

			EditText description =view.FindViewById<EditText> (Resource.Id.description);
			Button update= view.FindViewById<Button> (Resource.Id.buttonUpdate);
			update.Text = "Update";

			description.Text = ((EditGuideValueActivity)Activity).currentGuide.description;

			update.Click += (object IntentSender, EventArgs e) => {

				if (!description.Text.Equals(""))
				{					
					SessionManager sm = new SessionManager(view.Context);
					string token = sm.getAuthorizedToken();
					int guideId = sm.getGuideId();
					//post data
					Comms ca = new Comms();

					NameValueCollection parameters = new NameValueCollection ();
					parameters.Add (Constants.Guide_WebAPI_Key_Description, description.Text);
					string url = String.Format (Constants.URL_PutGuideDescription, guideId);

					JsonValue jsonResponse = ca.PostDataSync (Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile+url/*"/api/guides"*/, 
						parameters, token);

					//	var myActivity = (EditGuideValueActivity) this.Activity;
					Activity.Finish();

					Intent i = new Intent(view.Context, typeof(GuidingActivity));
					// Closing all the backstack Activities
					i.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
					this.StartActivity (i);

					//Activity.FragmentManager.PopBackStack();
					//ca.PostDataSync();
				}
				else
				{
					Toast.MakeText (view.Context, "Please enter a description", ToastLength.Long).Show();
				}
			};
			return view;
		}
	}
}

