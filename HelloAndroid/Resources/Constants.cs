using System;
using Android.Graphics;
using System.Collections.Generic;
using System.Json;

namespace TouriDroid
{
	public class Converter
	{
		public Color getOnlineStatusColor (int availability)
		{
			Color AvailableNowColor = Color.SeaGreen;
			Color AvailableLaterColor = Color.DarkOrange;
			Color NotAvailableForChatColor = Color.Red;
			switch (availability) 
			{

			case Constants.AvailableLaterValue:
				return AvailableLaterColor;

			case Constants.AvailableNowValue:
				return AvailableNowColor;

			case Constants.NotAvailableForChatValue:
			default:
				return NotAvailableForChatColor;
			}
		}

		public string getOnlineStatusString (int availability)
		{
			string status;
			switch (availability) 
			{

			case Constants.AvailableLaterValue:
				status = Constants.AvailableLaterString;
				break;

			case Constants.AvailableNowValue:
				status = Constants.AvailableNowString;
				break;

			case Constants.NotAvailableForChatValue:
				status = Constants.NotAvailableForChatString;
				break;

			default:
				status = Constants.NoValue;
				break;
			}
			return status;
		}



		public Guide parseOneGuideProfile(JsonValue json)
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
	public static class Constants
	{
		//Success, Fail values for function calls
		public const int SUCCESS=1;
		public const int FAIL=-1;

		//Main Tab positions
		public const int Main_Expertise_Tab=0;
		public const int Main_Chat_Tab=1;


		//URL values for WebApi calls
		public const string DEBUG_BASE_IP = "http://192.168.0.28";
		public const string DEBUG_BASE_PORT = "50467";
		public const string DEBUG_BASE_URL = DEBUG_BASE_IP + ":"+ DEBUG_BASE_PORT;//"http://192.168.0.28:50467";//"http://192.168.43.247:50467";//"http://192.168.0.12:50467";//"http://192.168.1.14:50467"; "http://192.168.1.189:50467"
		public const string URL_Get_All_Guides = "/api/guides";
		public const string URL_SearchGuides = "/api/guides/search?";
		public const string URL_Get_All_Expertises = "/api/expertises";
		public const string URL_MyGuideProfile = "/api/MyGuideProfile";
		public const string URL_AddGuideLocation = "/{0}/location";
		public const string URL_AddGuideExpertise = "/{0}/expertise";
		public const string URL_AddGuideLanguage = "/{0}/language";
		public const string URL_PutGuideNames = "/{0}/name";
		public const string URL_PutProfileImage = "/{0}/profileImage";
		public const string URL_PutGuideDescription = "/{0}/description";
		public const string URL_PutGuideSummary= "/{0}/summary";

		// WebApi Keys
		public const string Guide_WebAPI_Key_Username="username";
		public const string Guide_WebAPI_Key_FirstName="fName";
		public const string Guide_WebAPI_Key_LastName="lName";
		public const string Guide_WebAPI_Key_Description="description";
		public const string Guide_WebAPI_Key_GuideId="guideId";
		public const string Guide_WebAPI_Key_Address1="address1";
		public const string Guide_WebAPI_Key_Address2="address2";
		public const string Guide_WebAPI_Key_Phone="phone";
		public const string Guide_WebAPI_Key_LanguageList="languages";
		public const string Guide_WebAPI_Key_Language="language";
		public const string Guide_WebAPI_Key_LanguageId="languageId";
		public const string Guide_WebAPI_Key_LocationList="locationsServed";
		public const string Guide_WebAPI_Key_Location="location";
		public const string Guide_WebAPI_Key_Location_Id="locationId";
		public const string Guide_WebAPI_Key_Location_Lat="latitude";
		public const string Guide_WebAPI_Key_Location_long="longitude";
		public const string Guide_WebAPI_Key_LocationId="locationId";
		public const string Guide_WebAPI_Key_Availability ="availability";
		public const string Guide_WebAPI_Key_ExpertiseList ="expertises";
		public const string Guide_WebAPI_Key_Expertise = "expertise";
		public const string Guide_WebAPI_Key_ExpertiseId ="expertiseId";
		public const string Guide_WebAPI_Key_ProfileImageId ="profileImage";


		// Availability Values - this is stored in the Database for each guide to indicate availability
		public const int AvailableNowValue = 1;
		public const int AvailableLaterValue= 2;
		public const int NotAvailableForChatValue = 0;

		//Avaialibtility value mapping strings - this is done in the app based on DB value
		public const string AvailableNowString = "Online for chat";
		public const string AvailableLaterString = "Available later";
		public const string NotAvailableForChatString = "Not online";

		public const string NoValue = "N/A";

		// Length of the About me summary in the list view of Guide Cardview
		public const int MaxDescriptionLengthInCard = 100;

		// Drawer Items and Identifiers - this is the order they're displayed
		public const int MainActivity_DrawerMainMenuId = 1;
		public const string DrawerOptionBeAGuide = "Become A Guide";
		public const string DrawerOptionSwitchGuide = "Switch To Guiding";
		public const string DrawerOptionSwitchTravel = "Switch To Travelling";

		public const string DrawerOptionLogout = "Log Out";
		public const string DrawerOptionLoginOrSignUp = "Sign In or Sign Up";
		public const string MyPreferences = "My Preferences";

		public const int REQUEST_IMAGE =0;

		public const int SecondActivity_DrawerMainMenuId = 2;
		public const int SecondActivity_DrawerLanguageOptionsId = 3;
		public const int SecondActivity_DrawerExpertiseOptionsId = 4;

		public const string DrawerTitle = "Refine";
		public const string DrawerOptionLanguage = "Languages";
		public const string DrawerOptionExpertise = "Specialties";

		public const string DrawerOptionDone = "Done";

		//fragment types
		public const int GuideFragment=0;
		public const int ExpertiseFragment=1;
		public const int Uninitialized=-1;

		//image size for guide listing
		public const int GuideListingReqWidth=60;
		public const int GuideListingReqHeight=60;

		//image size for guide profile 
		public const int ProfileReqWidth=200;
		public const int ProfileReqHeight=200;

		//Second Activity passed in values
		public const string selectedLocation = "location";
		public const string selectedExpertise = "expertise";
		public const string guideFirstName = "fName";
		public const string guideLastName = "lName";
		public const string guideId= "guideId";
		public const string guideDescription = "description";
		public const string guideSummary = "summary";

		public const string Action = "Action";
		public const string Action_EditName = "Edit Name";
		public const string Action_EditDescription = "Edit Description";	


		//Google API Constants
		public static string PLACES_API_BASE = "https://maps.googleapis.com/maps/api/place";
		public static string TYPE_AUTOCOMPLETE = "/autocomplete";
		public static string OUT_JSON = "/json";


		public static bool isValidEmail(String target) {
			if (target == null) 
			{
				return false;
			}
			return Android.Util.Patterns.EmailAddress.Matcher(target).Matches();
		}

//	public static readonly string[] AvailableLanguages = new []
//		{
//			"Chinese","Spanish","English","Arabic","Hindi","Croatian","Portuguese","Russian","Japanese","German","Macedonian",
//			"Vietnamese","French","Korean","Tamil","Italian","Urdu"
//		};

		public static int DefaultProfileId = 1;

		//This must match the Languages table
		//@todo pull dynamically
		public static List<Tuple<int, string>> AvailableLanguages = new List<Tuple<int, string>> {
			Tuple.Create(1, "English"),
			Tuple.Create(2, "French"),
			Tuple.Create(3, "Tamil"),
			Tuple.Create(4, "Macedonian"),
			Tuple.Create(5, "Croatian"),
			Tuple.Create(6, "Turkish"),
			Tuple.Create(7, "Italian")

		};

		//Expertise images
		//This must match the Expertises table
		//@todo pull dynamically
		public static List<Tuple<int,int, string, int>> ExpertiseImages = new List<Tuple<int,int, string, int>> {
					Tuple.Create(Resource.Drawable.bar48, Resource.Drawable.bar48_pressed, "Hot Spots", 1),
					Tuple.Create(Resource.Drawable.cup54, Resource.Drawable.cup54, "Local life", 6),
					Tuple.Create(Resource.Drawable.camera, Resource.Drawable.camera, "Landmarks", 5),
					Tuple.Create(Resource.Drawable.lunch4, Resource.Drawable.lunch4, "Restaurants", 4),
					Tuple.Create(Resource.Drawable.hiking48, Resource.Drawable.hiking48_pressed, "Outdoors", 7),
					Tuple.Create(Resource.Drawable.museum37, Resource.Drawable.museum37, "Museums", 2)
		};

	}
}

