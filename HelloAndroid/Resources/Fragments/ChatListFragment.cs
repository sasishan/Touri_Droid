
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
using Android.Graphics;
using Java.IO;
using System.IO;

namespace TouriDroid
{
	public class ChatListFragment : Fragment
	{
		DataManager mDm;
		Converter	mConverter;
		View 		mView;
		SessionManager mSm;
		List<ChatUser> mUsers;
		ChatUserAdapter mAdapter;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			mConverter = new Converter ();

			// Create your fragment here
		}

		private void loadScreen()
		{
			SupportFunctions sf = new SupportFunctions ();

			string myUsername = "";
			if (mSm.isLoggedIn() == true) {
				myUsername= mSm.getEmail();
				loadMyMessages (mSm, sf);
			}

			mUsers = mDm.GetUsersWhoSentMeMessages (myUsername);
			if (mUsers == null) {
				Log.Debug ("ChatListFragment", "Unable to use DB");
				Activity.Finish ();
			}

			TextView noMsgs = mView.FindViewById<TextView> (Resource.Id.nomsgs);
			var messages = mView.FindViewById<ListView> (Resource.Id.Messages);
			var adapter = new ChatUserAdapter(Activity, mUsers);

			messages.Adapter = adapter;
			if (messages.Count > 0) {
				noMsgs.Visibility = ViewStates.Gone;
			}
		}
			
		public override void OnResume ()
		{
			base.OnResume ();
			loadScreen ();
			//LoadGuideThumbnails();

			//string Provider = LocationManager.GpsProvider;

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			mView = inflater.Inflate(Resource.Layout.ChatList, container, false);

			var messages = mView.FindViewById<ListView> (Resource.Id.Messages);
			//	var adapter = new ArrayAdapter<ChatUser> (Activity, Android.Resource.Layout.SimpleListItem1, users);
			mDm = new DataManager ();
			mDm.SetContext (mView.Context);
			mSm = new SessionManager (mView.Context);

			mUsers = new List<ChatUser> ();// mDm.GetUsersWhoSentMeMessages (myUsername);
			mAdapter = new ChatUserAdapter(Activity, mUsers);
			messages.Adapter = mAdapter;

			SupportFunctions sf = new SupportFunctions ();
			messages.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				var chatActivity = new Intent (mView.Context, typeof(ActiveChat));

				chatActivity.PutExtra ("TargetUserName", mUsers.ElementAt(e.Position).UserName);
				chatActivity.PutExtra ("TargetFirstName", mUsers.ElementAt(e.Position).UserName);
				chatActivity.PutExtra ("TargetGuideId", mUsers.ElementAt(e.Position).UserId.ToString());
				chatActivity.PutExtra ("TargetLastName", "");

				string imageName = mUsers.ElementAt(e.Position).UserName;
				if (!sf.FileExistsInStorage(imageName))
				{
					SaveImageFromBitmap(imageName, mUsers.ElementAt(e.Position).profileImage);
				}

				chatActivity.PutExtra ("ImageName", imageName);

				this.StartActivity(chatActivity);
			};
			return mView;
		}

		public String SaveImageFromBitmap(String imageName, Bitmap bitmap) {

			var documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			string filePath = System.IO.Path.Combine (documentsPath, imageName);

			try {
				ByteArrayOutputStream bytes = new ByteArrayOutputStream();
				FileStream fio = new System.IO.FileStream (filePath, FileMode.Create);
				bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, fio);
				fio.Write(bytes.ToByteArray(),0,bytes.Size());
				fio.Close();
			//	FileOutputStream fo = new FileOutputStream(imageName); //openFileOutput(fileName, Context.MODE_PRIVATE);
			//	fo.Write(bytes.ToByteArray());
				// remember close file output
			//	fo.Close();
			} catch (Exception e) {
				//;
				filePath = null;
			}
			return filePath;
		}

		public async void loadMyMessages(SessionManager sm, SupportFunctions sf)
		{
			string token = sm.getAuthorizedToken ();
			int newMessages = await sf.GetMyMessages(token, mDm, sm);
			return;
		}

	}
}

