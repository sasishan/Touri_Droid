
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
	public class EditSummaryFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.Guide_summary, container, false);

			EditText summary =view.FindViewById<EditText> (Resource.Id.summary);
			TextView count =view.FindViewById<TextView> (Resource.Id.charCount);
			Button update= view.FindViewById<Button> (Resource.Id.buttonUpdate);
			update.Text = "Update";

			summary.Text = ((EditGuideValueActivity)Activity).currentGuide.summary;
			count.Text=summary.Text.Length.ToString()+" chars";

			summary.AfterTextChanged+= (sender, e) => {
				int length = summary.Text.Length;
				count.Text=length.ToString()+" chars";

				if (length>=Constants.MAX_SUMMARY_LENGTH)
				{
					string subSum = summary.Text.Substring(0, length);
//					summary.Text = subSum;
					count.Text+=" MAX REACHED";
				}
				//mTextView.setText(string..valueOf(s.length()));
			};

			update.Click += (object IntentSender, EventArgs e) => {

				if (!string.IsNullOrEmpty(summary.Text) && !string.IsNullOrWhiteSpace(summary.Text))
				{					
					SessionManager sm = new SessionManager(view.Context);
					string token = sm.getAuthorizedToken();
					int guideId = sm.getGuideId();
					//post data
					Comms ca = new Comms();

					NameValueCollection parameters = new NameValueCollection ();
					parameters.Add (Constants.Guide_WebAPI_Key_Summary, summary.Text);
					string url = String.Format (Constants.URL_PutGuideSummary, guideId);

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

