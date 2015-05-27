
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
using System.Json;
using System.Threading.Tasks;

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

			ProgressBar progress = (ProgressBar) view.FindViewById(Resource.Id.progressBar);

			progress.Visibility = ViewStates.Visible;
			String token = await ls.Login (username, password);
			progress.Visibility = ViewStates.Gone;

			if (token!=null) {
				Comms ca = new Comms ();

				//check if there is a guide record for this username
				String url = Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile;// "/api/guides?username=" + username;

				Boolean isGuide = false;
				int guideId = Constants.Uninitialized;
				progress.Visibility = ViewStates.Visible;

				JsonValue response = await ca.getWebApiData (url, token);

				progress.Visibility = ViewStates.Gone;
				if (response != null) {
					//not a guide
					if (response.ContainsKey (Constants.Guide_WebAPI_Key_GuideId)) {						
						guideId = response [Constants.Guide_WebAPI_Key_GuideId];
						isGuide = true;					
				//		mChatIntent = new Intent (Activity, typeof(ChatService));
				//		mChatIntent.SetData ();
					}
				}

				Logger logger = new Logger ();
				SessionManager sessionManager = new SessionManager (view.Context);
				sessionManager.createLoginSession (username, token, isGuide, guideId );

				await startChatListener ();

				Toast.MakeText (view.Context, "Successfully logged in", ToastLength.Long).Show ();

				Intent i = null;
				if (isGuide) {
					i = new Intent(view.Context, typeof(GuidingActivity));
				} else {
					i = new Intent(view.Context, typeof(MainActivity));
				}

				// Closing all the backstack Activities
				i.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
				this.StartActivity (i);
			
			} else {
				Toast.MakeText (view.Context, "Username or password was incorrect", ToastLength.Long).Show ();
			}
		}

		private async Task startChatListener()
		{
			//start the chat service
			var chatIntent = new Intent (Activity, typeof(ChatService));
			Activity.StartService (chatIntent);

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

