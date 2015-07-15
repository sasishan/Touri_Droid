
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
	public class ChatListFragment : Fragment
	{
		DataManager mDm;
		Converter mConverter;
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			mConverter = new Converter ();

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			var view = inflater.Inflate(Resource.Layout.ChatList, container, false);

			SessionManager sm = new SessionManager (view.Context);
			SupportFunctions sf = new SupportFunctions ();
			mDm = new DataManager ();
			mDm.SetContext (view.Context);

			string myUsername = "";
			if (sm.isLoggedIn() == true) {
				myUsername= sm.getEmail();
				loadMyMessages (sm, sf);
			}

			List<ChatUser> users = mDm.GetUsersWhoSentMeMessages (myUsername);
			if (users == null) {
				Log.Debug ("ChatListFragment", "Unable to use DB");
				Activity.Finish ();
				return view;
			}

			var messages = view.FindViewById<ListView> (Resource.Id.Messages);

		//	var adapter = new ArrayAdapter<ChatUser> (Activity, Android.Resource.Layout.SimpleListItem1, users);
			var adapter = new ChatUserAdapter(Activity, users);
			messages.Adapter = adapter;

			TextView noMsgs = view.FindViewById<TextView> (Resource.Id.nomsgs);
			if (messages.Count > 0) {
				noMsgs.Visibility = ViewStates.Gone;
			}

			messages.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				var chatActivity = new Intent (view.Context, typeof(ActiveChat));

				chatActivity.PutExtra ("TargetUserName", users.ElementAt(e.Position).UserName);
				chatActivity.PutExtra ("TargetFirstName", "");
				chatActivity.PutExtra ("TargetLastName", "");
				this.StartActivity(chatActivity);
			};
			return view;
		}

		public async void loadMyMessages(SessionManager sm, SupportFunctions sf)
		{
			string token = sm.getAuthorizedToken ();
			int newMessages = await sf.GetMyMessages(token, mDm);
			return;
		}

	}
}

