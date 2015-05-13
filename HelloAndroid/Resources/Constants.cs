using System;
using Android.Graphics;
using System.Collections.Generic;

namespace TouriDroid
{
	public static class Constants
	{
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

