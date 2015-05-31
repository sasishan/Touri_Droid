
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
using Android.Graphics;



namespace TouriDroid
{
	[Activity (Label = "Guide", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class GuideProfileActivity : Activity
	{
		Guide thisGuide;
		string guideId;
		ProgressBar progress;
		string userName;
		string fName;
		string lName;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.GuideProfile);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			progress = FindViewById<ProgressBar> (Resource.Id.progressBar);

			guideId = Intent.GetStringExtra ("GuideId") ?? "Data not available";
			userName = Intent.GetStringExtra ("UName") ?? "Data not available";
			fName = Intent.GetStringExtra ("FName") ?? "Data not available";
			lName = Intent.GetStringExtra ("LName") ?? "Data not available";
			string description = Intent.GetStringExtra ("Description") ?? "Data not available";
			string languages = Intent.GetStringExtra ("Languages") ?? "Data not available";
			string expertise = Intent.GetStringExtra ("Expertise") ?? "Data not available";
			string availability = Intent.GetStringExtra ("Availability") ?? "Data not available";

			loadProfile (guideId, fName, lName, description,languages,expertise );

			Button button = FindViewById<Button>(Resource.Id.ChatButton);

			button.Click += (o, e) => {
				//if not logged in
				SessionManager sm = new SessionManager(this);
				if (sm.isLoggedIn()==false)
				{
					Toast.MakeText(this, "Please sign in to chat", ToastLength.Short).Show();
				}
				else
				{
					string myUsername = sm.getEmail();
					if (myUsername.Equals(thisGuide.userName))
					{
						Toast.MakeText(this, "You can't chat with yourself", ToastLength.Short).Show();
					}
					else
					{
						var chatActivity = new Intent (this, typeof(ActiveChat));
						chatActivity.PutExtra ("TargetGuideId", guideId);
						chatActivity.PutExtra ("TargetUserName", thisGuide.userName);
						chatActivity.PutExtra ("TargetFirstName", thisGuide.fName);
						chatActivity.PutExtra ("TargetLastName", thisGuide.lName);
						this.StartActivity(chatActivity);						
					}
				}
			};
		}
		 
		public override bool OnKeyDown(Android.Views.Keycode keyCode, Android.Views.KeyEvent e) {
			if (keyCode == Android.Views.Keycode.Back  && e.RepeatCount == 0) {

				Finish();
				return true;
			}
			return base.OnKeyDown(keyCode, e);
		}

		public override bool OnKeyUp(Android.Views.Keycode keyCode, Android.Views.KeyEvent e) {
			if (keyCode == Android.Views.Keycode.Back  && e.RepeatCount == 0) {

				Finish();
				return true;
			}
			return base.OnKeyUp(keyCode, e);
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

		private async void loadProfile(string guideId, string fName, string lName,string description, string languages, string expertise)
		{
			TextView gName = this.FindViewById<TextView> (Resource.Id.guide_name);
			TextView gAbout = this.FindViewById<TextView> (Resource.Id.about);
			TextView gExpertise = this.FindViewById<TextView> (Resource.Id.expertise);
			TextView gLanguages = this.FindViewById<TextView> (Resource.Id.languages);
			ImageView photo = this.FindViewById<ImageView> (Resource.Id.guide_photo);

			//photo.SetImageResource(Resource.Drawable.placeholder_photo);

			Comms ca = new Comms ();
			String url = Constants.DEBUG_BASE_URL + "/api/guides/" + guideId;
			Converter converter = new Converter ();

			progress.Visibility = ViewStates.Visible;
			var json = await ca.getWebApiData (url, null);
			thisGuide = converter.parseOneGuideProfile (json);

			if (thisGuide.profileImageId == Constants.Uninitialized) {
				photo.SetImageResource(Resource.Drawable.placeholder_photo);
			} else {
				url= Constants.DEBUG_BASE_URL + "/api/images/"+ thisGuide.profileImageId;

				Bitmap image = (Bitmap) await ca.getScaledImage (url, Constants.ProfileReqWidth, Constants.ProfileReqHeight);
				thisGuide.profileImage = image;
				photo.SetImageBitmap(thisGuide.profileImage);
			}

			progress.Visibility = ViewStates.Gone;

			TextView availability = this.FindViewById<TextView> (Resource.Id.availability);
			availability.Text = converter.getOnlineStatusString(thisGuide.availability);
			availability.SetTextColor(converter.getOnlineStatusColor(thisGuide.availability));

			gName.Text = thisGuide.fName+" " +thisGuide.lName;
			gAbout.Text = thisGuide.description;

			foreach (string l in thisGuide.languageList) {
				gLanguages.Text += "• " + l + "\r\n";	 
			}

			foreach (Expertise e in thisGuide.expertise) {
				gExpertise.Text +="• " + e.expertise+ "\r\n";	 
			}
	
			// set the titles to visible so it appears at the same times after loading
			TextView holders = this.FindViewById<TextView> (Resource.Id.languages_text);
			holders.Visibility = ViewStates.Visible;
			holders = this.FindViewById<TextView> (Resource.Id.expertise_text);
			holders.Visibility = ViewStates.Visible;
			holders = this.FindViewById<TextView> (Resource.Id.about_text);
			holders.Visibility = ViewStates.Visible;
			//gLanguages.Text = g.languages;
			//gExpertise.Text = expertise;
		}
	
	}
}

