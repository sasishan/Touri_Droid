
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
using Android.Database;

namespace TouriDroid
{
	[Activity (Label = "Image Select", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]	
	public class ImageSelectActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ImageSelect_Layout);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);
			string guideId = Intent.GetStringExtra (Constants.guideId) ?? "Data not available";

			var imageIntent = new Intent ();
			imageIntent.SetType ("image/*");
			imageIntent.SetAction (Intent.ActionGetContent);
			StartActivityForResult (Intent.CreateChooser (imageIntent, "Select photo"), Constants.REQUEST_IMAGE);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (resultCode == Result.Ok) {

				var imageView = FindViewById<ImageView> (Resource.Id.guide_photo);
				imageView.SetImageURI (data.Data);

				Button accept = FindViewById<Button> (Resource.Id.acceptButton);
				Button cancel = FindViewById<Button> (Resource.Id.cancelButton);

				accept.Click += (object sender, EventArgs e) => {
					Android.Net.Uri selectedImage = data.Data;

					string filePath = GetPathToImage (selectedImage);
					if (filePath==null)
					{
						Toast.MakeText(this, "There was an error getting the image", ToastLength.Short).Show();
					}
					else
					{
						SessionManager sm = new SessionManager (this);
						string token = sm.getAuthorizedToken ();
						int guideId = sm.getGuideId ();
						string url = String.Format (Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile + Constants.URL_PutProfileImage, guideId);
						Comms ca = new Comms ();

						string response = ca.PostFile (url, filePath, token);
						//string response = ca.ScaleAndUploadPic(url, filePath, token,Constants.FULL_SIZE, Constants.FULL_SIZE);
						if (response==null)
						{
							Toast.MakeText(this, "There was an error uploading the image", ToastLength.Short).Show();
						}
						else
						{
							Toast.MakeText(this, "Image uploaded successfully.", ToastLength.Short).Show();
						}
					}

					Finish ();
					Intent i = new Intent (this, typeof(GuidingActivity));
					// Closing all the Activities
					i.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
					this.StartActivity (i);
				};

				cancel.Click += (object sender, EventArgs e) => {
					Finish ();
				};
			} else {
				Finish ();
			}
		}

		private string GetPathToImage(Android.Net.Uri uri)
		{
			string path = null;
			string document = uri.Path;
			string docId = document.Substring (document.LastIndexOf (":") + 1);

			// The projection contains the columns we want to return in our query.
			string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Id };
			string selection = Android.Provider.MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
			using (ICursor cursor = ContentResolver.Query(Android.Provider.MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] {docId}, null))
			{
				if (cursor != null)
				{
					int columnIndex = cursor.GetColumnIndexOrThrow (Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
					cursor.MoveToFirst();
					path = cursor.GetString(columnIndex);
				}
			}

			return path;

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
	}
}

