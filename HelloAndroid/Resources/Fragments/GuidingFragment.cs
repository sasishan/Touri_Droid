
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
using System.Threading.Tasks;

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
			((GuidingActivity)Activity).currentGuide=null;

			//do the image and details separately for better responsiveness
			loadMyProfileDetails (view);
			loadMyProfileImage (view);
			return view;
		}

		public async void loadMyProfileImage(View view)
		{
			//once the profile is loaded (including the image url) this variable is set
			while ( Activity!=null && ((GuidingActivity)Activity).currentGuide==null )
			{
				await Task.Delay (1000);
			}

			if (Activity == null) {
				return;
			}
			Guide myProfile = ((GuidingActivity)Activity).currentGuide;
			string imageUrl;
			Comms ca = new Comms ();

			if (myProfile.profileImageId == Constants.Uninitialized) {
				myProfile.profileImage = null;
			} else {
				imageUrl= Constants.DEBUG_BASE_URL + "/api/images/"+ myProfile.profileImageId;

				Bitmap image = (Bitmap) await ca.getScaledImage (imageUrl,  Constants.ProfileReqWidth, Constants.ProfileReqHeight);
				myProfile.profileImage = image;
			}

			ImageView photo = view.FindViewById<ImageView> (Resource.Id.guide_photo);
			if (myProfile.profileImage == null) {
				photo.SetImageResource (Resource.Drawable.placeholder_photo);
			} else {
				photo.SetImageBitmap (myProfile.profileImage);
			}

			//add a listener for the change photo icon
			ImageView changePhoto = view.FindViewById<ImageView> (Resource.Id.camera);
			changePhoto.Click += (sender, e) => 
			{
				var selectImage = new Intent (Activity, typeof(ImageSelectActivity));
				selectImage.PutExtra (Constants.guideId, myProfile.guideId);

				this.StartActivity(selectImage);
			};
		}

		public async void loadMyProfileDetails(View view)
		{
			List<string> profileRows = new List<string> ();
			SessionManager sm = new SessionManager (view.Context);
			string token = sm.getAuthorizedToken ();

			Comms ca = new Comms ();
			string url = Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile;

			var json = await ca.getWebApiData (url, token);

			if (json == null) {
				Toast.MakeText(view.Context, "Not authorized, try signing in again", ToastLength.Short).Show ();
				Activity.Finish ();
				return;
			}
			Converter converter = new Converter ();
			Guide myProfile = converter.parseOneGuideProfile (json);//parseGuideProfiles (json);

			Switch online = view.FindViewById<Switch> (Resource.Id.toggleChatOn);
			if (myProfile.availability == Constants.AvailableNowValue) {
				online.Checked = true;
			}

			if (myProfile == null) {
				return;
			}

		//	string imageUrl;
			if (myProfile.profileImageId == Constants.Uninitialized) {
				myProfile.profileImage = null;
			} else {
		//		imageUrl= Constants.DEBUG_BASE_URL + "/api/images/"+ myProfile.profileImageId;

		//		Bitmap image = (Bitmap) await ca.getScaledImage (imageUrl,  Constants.ProfileReqWidth, Constants.ProfileReqHeight);
		//		myProfile.profileImage = image;
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
			TextView editLocations = view.FindViewById<TextView> (Resource.Id.editLocations);
			TextView editExpertise = view.FindViewById<TextView> (Resource.Id.editExpertise);
			TextView editLanguages = view.FindViewById<TextView> (Resource.Id.editLanguages);

			TableLayout expertiseTableLayout = view.FindViewById<TableLayout> (Resource.Id.table_Expertise);
			SupportFunctions sf = new SupportFunctions ();

			sf.BuildSelectedExpertiseTable (view, expertiseTableLayout, Resource.Layout.selectedExpertise_layout, myProfile.expertise);

			userName.Text = myProfile.userName;
			guideName.Text = myProfile.fName + " " + myProfile.lName;
			aboutMe.Text = myProfile.description;
			shortAboutme.Text = myProfile.summary;
			foreach (LocationWrapper l in myProfile.placesServedList) {
				locations.Text += "• "+l.location+"\r\n" ;
			}

			foreach (string l in myProfile.languageList) {
				languages.Text += "• "+l+"\r\n" ;
			}

			photo.SetImageResource (Resource.Drawable.placeholder_photo);
			/*if (myProfile.profileImage == null) {
				photo.SetImageResource (Resource.Drawable.placeholder_photo);
			} else {
				photo.SetImageBitmap (myProfile.profileImage);
			}*/	

			online.Click += (o, e) => {
				// Perform action on clicks
				if (online.Checked)
				{
					//stop any existing service and restart it
					Activity.StopService (new Intent (Activity, typeof(ChatService)));
					Activity.StartService (new Intent (Activity, typeof(ChatService)));
					Toast.MakeText(view.Context, "Online", ToastLength.Short).Show ();
				}
				else
				{
					Activity.StopService (new Intent (Activity, typeof(ChatService)));
					Toast.MakeText(view.Context, "Offline", ToastLength.Short).Show ();
				}
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
				var editGuide = new Intent (Activity, typeof(EditGuideValueActivity));
				editGuide.PutExtra (Constants.guideSummary, myProfile.summary);	
				editGuide.PutExtra (Constants.Action, Constants.Action_EditSummary);	

				this.StartActivity (editGuide);
			};

			editExpertise.Click += (sender, e) => 
			{
				var editGuide = new Intent (Activity, typeof(EditGuideValueActivity));
				string stringExpertiseIds = GetStringExpertiseIds(myProfile.expertise);
				editGuide.PutExtra (Constants.Action, Constants.Action_EditExpertise);	
				editGuide.PutExtra (Constants.selectedExpertise, stringExpertiseIds);	
				this.StartActivity (editGuide);
			};

			editLanguages.Click += (sender, e) => 
			{
				string stringLanguageIds = GetStringLanguageIds(myProfile.languages);
				var editGuide = new Intent (Activity, typeof(EditGuideValueActivity));
				editGuide.PutExtra (Constants.selectedLanguages, stringLanguageIds);	
				editGuide.PutExtra (Constants.Action, Constants.Action_EditLanguages);	

				this.StartActivity (editGuide);	
			};

			editLocations.Click += (sender, e) => 
			{
				string stringLocation = GetStringLocationsList(myProfile.placesServedList);
				var editGuide = new Intent (Activity, typeof(EditGuideValueActivity));
				editGuide.PutExtra (Constants.selectedLocation, stringLocation);	
				editGuide.PutExtra (Constants.Action, Constants.Action_EditLocations);	

				this.StartActivity (editGuide);				
			};

			//we've loaded all the details so set the current profile - this is used to load the image async
			if (Activity != null) {
				((GuidingActivity)Activity).currentGuide = myProfile;
			}
		}

		private string GetStringExpertiseIds(List<Expertise> expList)
		{
			string expIdString = "";
			foreach (Expertise exp in expList) {
				expIdString += exp.expertiseId.ToString() + Constants.separator;
			}

			if (expIdString.Length > 1) {
				expIdString = expIdString.Remove (expIdString.Length - 1);
			}

			return expIdString;
		}

		private string GetStringLanguageIds(List<GuideLanguage> langList)
		{
			string langIdString = "";
			foreach (GuideLanguage gl in langList) {
				langIdString += gl.languageId.ToString() + Constants.separator;
			}
			if (langIdString.Length > 1) {
				langIdString = langIdString.Remove (langIdString.Length - 1);
			}

			return langIdString;
		}

		private string GetStringLocationsList(List<LocationWrapper> lwList)
		{
			string locationString = "";
			foreach (LocationWrapper lw in lwList) {
				locationString += lw.location + Constants.separator;
			}
			if (locationString.Length > 1) {
				locationString = locationString.Remove (locationString.Length - 1);
			}

			return locationString;
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
	}
}

