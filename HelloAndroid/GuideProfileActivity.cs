
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



namespace HelloAndroid
{
	[Activity (Label = "GuideProfileActivity")]			
	public class GuideProfileActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.GuideProfile);

			string guideId = Intent.GetStringExtra ("GuideId") ?? "Data not available";
			string fName = Intent.GetStringExtra ("FName") ?? "Data not available";
			string lName = Intent.GetStringExtra ("LName") ?? "Data not available";
			string description = Intent.GetStringExtra ("Description") ?? "Data not available";
			string languages = Intent.GetStringExtra ("Languages") ?? "Data not available";
			string expertise = Intent.GetStringExtra ("Expertise") ?? "Data not available";
			string availability = Intent.GetStringExtra ("Availability") ?? "Data not available";

			loadProfile (guideId, fName, lName, description,languages,expertise );
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

		private void loadProfile(string guideId, string fName, string lName,string description, string languages, string expertise)
		{
			TextView gName = this.FindViewById<TextView> (Resource.Id.guide_name);
			TextView gAbout = this.FindViewById<TextView> (Resource.Id.about);
			TextView gExpertise = this.FindViewById<TextView> (Resource.Id.expertise);
			TextView gLanguages = this.FindViewById<TextView> (Resource.Id.languages);
			ImageView photo = this.FindViewById<ImageView> (Resource.Id.guide_photo);
			photo.SetImageResource(Resource.Drawable.placeholder_photo);

			gName.Text = fName+" " +lName;
			gAbout.Text = description;
			gLanguages.Text = languages;
			gExpertise.Text = expertise;
		}
	
	}
}

