
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
using Android.Graphics;
using Android.Database;
using Android.Net;

namespace TouriDroid
{
	public class GuidingFragment : Fragment
	{
		View myView;
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.GuideEditProfile_fragment, container, false);
			myView = view;
			loadMyProfile (view);
			return view;
		}

		public async void loadMyProfile(View view)
		{
			List<string> profileRows = new List<string> ();
			SessionManager sm = new SessionManager (view.Context);
			string token = sm.getAuthorizedToken ();

			Comms ca = new Comms ();
			string url = Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile;
			var json = await ca.getWebApiData (url, token);

			if (json == null) {
				Toast.MakeText(view.Context, "Not authorized", ToastLength.Short).Show ();
				Activity.Finish ();
				return;
			}
			Guide myProfile = parseGuideProfiles (json);
			((GuidingActivity)Activity).currentGuide = myProfile;

			Switch online = view.FindViewById<Switch> (Resource.Id.toggleChatOn);

			if (myProfile.availability == Constants.AvailableNowValue) {
				online.Checked = true;
			}

			if (myProfile == null) {
				return;
			}

			string imageUrl;
			if (myProfile.profileImageId == Constants.Uninitialized) {
				myProfile.profileImage = null;
			} else {
				imageUrl= Constants.DEBUG_BASE_URL + "/api/images/"+ myProfile.profileImageId;

				Bitmap image = (Bitmap) await ca.getScaledImage (imageUrl,  Constants.ProfileReqWidth, Constants.ProfileReqHeight);
				myProfile.profileImage = image;
			}
			
		/*	profileRows.Add ("Photo TBD");
			profileRows.Add ("Name: " + myProfile.fName + " " + myProfile.lName);
			profileRows.Add ("Address: "+ myProfile.address1 );
			profileRows.Add ("Email: TBD");
			profileRows.Add ("Phone: " + myProfile.phone);
			profileRows.Add ("Touring Expertise" );
			profileRows.Add ("Languages" );
			profileRows.Add ("Touring Locations" );
			profileRows.Add ("Description: " + myProfile.description);
			profileRows.Add ("One line summary: TBD");

			var guideProfile = view.FindViewById<ListView> (Resource.Id.GuideProfileRows);

			var adapter = new ArrayAdapter<string> (Activity, Android.Resource.Layout.SimpleListItem1, profileRows);
			guideProfile.Adapter = adapter;*/

			TextView guideName = view.FindViewById<TextView> (Resource.Id.guideName);
			TextView userName = view.FindViewById<TextView> (Resource.Id.guideUsername);
			TextView aboutMe = view.FindViewById<TextView> (Resource.Id.aboutme);
			TextView shortAboutme = view.FindViewById<TextView> (Resource.Id.shortAboutme);
			TextView locations = view.FindViewById<TextView> (Resource.Id.locations);
			TextView languages = view.FindViewById<TextView> (Resource.Id.languages);
			ImageView photo = view.FindViewById<ImageView> (Resource.Id.guide_photo);
			ImageView changePhoto = view.FindViewById<ImageView> (Resource.Id.camera);

			TextView editName = view.FindViewById<TextView> (Resource.Id.editGuideName);
			TextView editAboutMe = view.FindViewById<TextView> (Resource.Id.editAboutme);
			TextView editShortAboutMe = view.FindViewById<TextView> (Resource.Id.editShortAboutme);
			TextView editLocations = view.FindViewById<TextView> (Resource.Id.locations);
			TextView editLanguages = view.FindViewById<TextView> (Resource.Id.languages);

			userName.Text = myProfile.userName;
			guideName.Text = myProfile.fName + " " + myProfile.lName;
			aboutMe.Text = myProfile.description;
			shortAboutme.Text = myProfile.description;
			foreach (LocationWrapper l in myProfile.placesServedList) {
				locations.Text += "• "+l.location+"\r\n" ;
			}

			foreach (string l in myProfile.languageList) {
				languages.Text += "• "+l+"\r\n" ;
			}

			if (myProfile.profileImage == null) {
				photo.SetImageResource (Resource.Drawable.placeholder_photo);
			} else {
				photo.SetImageBitmap (myProfile.profileImage);
			}	

			online.Click += (o, e) => {
				// Perform action on clicks
				if (online.Checked)
				{
					Activity.StartService (new Intent (Activity, typeof(ChatService)));
					Toast.MakeText(view.Context, "Online", ToastLength.Short).Show ();
				}
				else
				{
					Activity.StopService (new Intent (Activity, typeof(ChatService)));
					Toast.MakeText(view.Context, "Offline", ToastLength.Short).Show ();
				}
			};

			changePhoto.Click += (sender, e) => 
			{
				var selectImage = new Intent (Activity, typeof(ImageSelectActivity));
				selectImage.PutExtra (Constants.guideId, myProfile.guideId);

				this.StartActivity(selectImage);
			};

			editName.Click += (sender, e) => 
			{
				var editGuide = new Intent (Activity, typeof(EditGuideValueActivity));
				editGuide.PutExtra (Constants.guideFirstName, myProfile.fName);
				editGuide.PutExtra (Constants.guideLastName, myProfile.lName);	
				editGuide.PutExtra (Constants.Action, Constants.Action_EditName);	

				this.StartActivity (editGuide);
			};

			editAboutMe.Click += (sender, e) => 
			{
				var editGuide = new Intent (Activity, typeof(EditGuideValueActivity));
				editGuide.PutExtra (Constants.guideDescription, myProfile.description);	
				editGuide.PutExtra (Constants.Action, Constants.Action_EditDescription);	

				this.StartActivity (editGuide);
			};

			editShortAboutMe.Click += (sender, e) => 
			{
			};

			editLanguages.Click += (sender, e) => 
			{
			};

			editLocations.Click += (sender, e) => 
			{
			};
		}

		public void onToggleClicked(View view) {
			// Is the toggle on?
			Boolean on = ((ToggleButton) view).Checked;

			if (on) {
				Activity.StartService (new Intent (Activity, typeof(ChatService)));

			} else {
				// Disable vibrate
				Activity.StopService (new Intent (Activity, typeof(ChatService)));
			}
		}

		public Guide parseGuideProfiles(JsonValue json)
		{
			if (json == null) {
				return null;
			}

			Guide g = new Guide ();
			g.jsonText = json;

			JsonValue values = json;
			if (values.ContainsKey (Constants.Guide_WebAPI_Key_FirstName)) {
				string fName = values [Constants.Guide_WebAPI_Key_FirstName];

				g.fName = fName;
				g.guideId = values [Constants.Guide_WebAPI_Key_GuideId];

			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Username)) {
				string userName = values [Constants.Guide_WebAPI_Key_Username];
				g.userName= userName;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_LastName)) {
				string lName = values [Constants.Guide_WebAPI_Key_LastName];
				g.lName= lName;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Address1)) {
				g.address1 = values [Constants.Guide_WebAPI_Key_Address1];
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Address2)) {
				g.address2 = values [Constants.Guide_WebAPI_Key_Address2];
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Description)) {
				g.description = values [Constants.Guide_WebAPI_Key_Description];
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Availability)) {
				g.availability = values [Constants.Guide_WebAPI_Key_Availability];
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_ProfileImageId)) {
				g.profileImageId = values [Constants.Guide_WebAPI_Key_ProfileImageId];
			} else {
				g.profileImageId = Constants.Uninitialized;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_LanguageList)) {
				JsonValue temp = values [Constants.Guide_WebAPI_Key_LanguageList];
				for (int j=0; j<temp.Count;j++)
				{
					JsonValue l = temp[j];
					g.languageList.Add (l [Constants.Guide_WebAPI_Key_Language]);
					//@todo get languageId too
				}
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_LocationList)) {
				JsonValue temp = values [Constants.Guide_WebAPI_Key_LocationList];
				for (int j=0; j<temp.Count;j++)
				{
					JsonValue l = temp[j];
					LocationWrapper lw = new LocationWrapper ();
					lw.location = l [Constants.Guide_WebAPI_Key_Location];
					lw.longitude = l [Constants.Guide_WebAPI_Key_Location_long];
					lw.latitude = l [Constants.Guide_WebAPI_Key_Location_Lat];
					lw.locationId = l [Constants.Guide_WebAPI_Key_Location_Id];
					g.placesServedList.Add (lw);
					//@todo 
				}
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_ExpertiseList)) {
				JsonValue temp = values [Constants.Guide_WebAPI_Key_ExpertiseList];
				for (int j=0; j<temp.Count;j++)
				{
					JsonValue l = temp[j];
					g.expertise.Add (new Expertise() {expertise=l [Constants.Guide_WebAPI_Key_Expertise], expertiseId=l [Constants.Guide_WebAPI_Key_ExpertiseId]});
					//@todo 
				}
			}
			return g;
		}


	}
}

