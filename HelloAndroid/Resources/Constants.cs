using System;
using Android.Graphics;
using System.Collections.Generic;
using System.Json;
using Android.App;
using Android.Content;

namespace TouriDroid
{
	public static class Constants
	{
		public const string TOURI_VER = "v.0.1.06.16.15.2";
		public const string TOURI_TAG = "Touri";
		//Success, Fail values for function calls
		public const int SUCCESS=1;
		public const int FAIL=-1;
		public const string EmptyString = "";

		public const int MAX_SUMMARY_LENGTH=100;

		public const string DefaultLanguage="English";
		public const bool DefaultShowOffline=false;

		//Main Tab positions
		public const int Main_Expertise_Tab=0;
		public const int Main_Chat_Tab=1;
		public const string Search_Tab = "Local Expertise";
		public const string Chat_Tab = "Chat Messages";
		public const string Profile_Tab = "My Profile";
		public const string Bookings_Tab = "Bookings";

		public const string MessageIsRead = "Y";
		public const string MessageUnread = "N";

		public const int FULL_SIZE = 600;
		public const int MyResponseYes = 1;
		public const int MyResponseNo = 0;

		public const string DefaultSearchDistance = "50 KM";

		//URL values for WebApi calls
		//public const string DEBUG_BASE_IP = "http://192.168.0.13";
		//public const string DEBUG_BASE_IP = "http://54.69.185.48"; //PRODUCTION SYSTEM
		public const string DEBUG_BASE_IP = "http://192.168.0.28";

		public const string DEBUG_BASE_PORT = "50467";
		public const string DEBUG_BASE_URL = DEBUG_BASE_IP + ":"+ DEBUG_BASE_PORT;//"http://192.168.0.28:50467";//"http://192.168.43.247:50467";//"http://192.168.0.12:50467";//"http://192.168.1.14:50467"; "http://192.168.1.189:50467"
		public const string URL_Get_All_Guides = "/api/guides";
		public const string URL_SearchGuides = "/api/guides/search?";
		public const string URL_Get_All_Expertises = "/api/expertises";
		public const string URL_MyGuideProfile = "/api/MyGuideProfile";
		public const string URL_AddGuideLocation = "/{0}/location";
		public const string URL_PostAllGuideLocations = "/{0}/alllocations";
		public const string URL_PostAllGuideLanguages = "/{0}/alllanguages";
		public const string URL_PostAllGuideExpertises = "/{0}/allexpertises";
		public const string URL_AddGuideExpertise = "/{0}/expertise";
		public const string URL_AddGuideLanguage = "/{0}/language";
		public const string URL_PutGuideNames = "/{0}/name";
		public const string URL_PutProfileImage = "/{0}/profileImage";
		public const string URL_PutGuideDescription = "/{0}/description";
		public const string URL_PutGuideSummary= "/{0}/summary";
		public const string URL_MyMessages= "/api/messages";
		public const string URL_Query_LastMessageId= "?msgId=";

		//online offline
		public const string ONLINE_NOTIFICATIONS = "Chat Notifications are ON";
		public const string OFFLINE_NOTIFICATIONS = "Chat Notifications are OFF";

		// WebApi Keys
		public const string Guide_WebAPI_Key_Username="username";
		public const string Guide_WebAPI_Key_LastMessageSent="lastMessageSent";
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
		public const string Guide_WebAPI_Key_Summary="summary";

		//Chat messages json
		public const string Guide_WebAPI_Key_Msg_Id="id";
		public const string Guide_WebAPI_Key_FromUser="fromUser";
		public const string Guide_WebAPI_Key_ToUser="toUser";
		public const string Guide_WebAPI_Key_Message="message";
		public const string Guide_WebAPI_Key_Msg_TimeStampe="timeStamp";

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
		public const string DrawerOptionSwitchTravel = "Switch To Touri'ing";

		public const string DrawerOptionLogout = "Log Out";
		public const string DrawerOptionLoginOrSignUp = "Sign In or Sign Up";
		public const string MyPreferences = "Search Options";

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
		public const string selectedLanguages = "languages";

		public const string Action = "Action";
		public const string Action_EditName = "Edit Name";
		public const string Action_EditDescription = "Edit About Me";	
		public const string Action_EditLocations= "Edit Locations";	
		public const string Action_EditLanguages = "Edit Languages";	
		public const string Action_EditSummary = "Edit Summary";	
		public const string Action_EditExpertise = "Edit Expertise";	

		public const char separator = '|';

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
		//Up image, down image, expertise name, expertise ID (matches Expertise table in DB)
		//don't use the '&' sign in names
		public static List<Tuple<int,int, string, int, int>> ExpertiseImages = new List<Tuple<int,int, string, int, int>> {					
			Tuple.Create(Resource.Drawable.expCool_64, Resource.Drawable.expCool_64_pressed, "Local Life", 1, Resource.Drawable.expCool_32_pressed),
			Tuple.Create(Resource.Drawable.expRestaurant_64, Resource.Drawable.expRestaurant_64_pressed, "Restaurants and Food", 4, Resource.Drawable.expRestaurant_32_pressed),
			Tuple.Create(Resource.Drawable.expMuseum_64, Resource.Drawable.expMuseum_64_pressed, "Art and Museums", 5, Resource.Drawable.expMuseum_32_pressed),
			Tuple.Create(Resource.Drawable.expDJ_64, Resource.Drawable.expDJ_64_pressed, "Happening Now", 6, Resource.Drawable.expDJ_32_pressed),
			Tuple.Create(Resource.Drawable.expTrekking_64, Resource.Drawable.expTrekking_64_pressed, "Outdoors", 7, Resource.Drawable.expTrekking_32_pressed),
			Tuple.Create(Resource.Drawable.expDancing_64, Resource.Drawable.expDancing_64_pressed, "Bars and Lounges", 2, Resource.Drawable.expDancing_32_pressed)
		};
	}

	public class Logger 
	{
		public int LogOut(SessionManager sm, Activity a)
		{
			sm.logoutUser();
			a.StopService (new Intent (a, typeof(ChatService)));

			return Constants.SUCCESS;
		}

		public int LogIn(SessionManager sm, Activity a)
		{
			return Constants.SUCCESS;
		//	sm.createLoginSession();
		//	a.StopService (new Intent (this, typeof(ChatService)));
		}

	}

	public class Converter
	{
		
		public string ConvertWithinDistance(string distance)
		{
			string[] words = distance.Split(' ');
			if (words != null) {
				return words [0];
			} else {
				return null;
			}
		}

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

		public ChatMessage parseOneChatMessage(JsonValue json)
		{
			if (json == null) {
				return null;
			}

			ChatMessage cm = new ChatMessage ();

			JsonValue values = json;

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_FromUser)) {
				string fromUser = values [Constants.Guide_WebAPI_Key_FromUser];
				cm.FromUser = fromUser;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_ToUser)) {
				string toUser = values [Constants.Guide_WebAPI_Key_ToUser];
				cm.ToUser = toUser;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Message)) {
				string message = values [Constants.Guide_WebAPI_Key_Message];
				cm.Message = message;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Msg_TimeStampe)) {
				string timestamp = values [Constants.Guide_WebAPI_Key_Msg_TimeStampe];
				cm.Msgtimestamp = timestamp;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Msg_Id)) {
				cm.ID = values [Constants.Guide_WebAPI_Key_Msg_Id];
			}

			return cm;
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

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_LastMessageSent)) {
				string lastMessageSent = values [Constants.Guide_WebAPI_Key_LastMessageSent];
				g.lastMessageSent = lastMessageSent;
			} else {
				g.lastMessageSent = "N/A";
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Username)) {
				string userName = values [Constants.Guide_WebAPI_Key_Username];
				g.userName= userName;
			}

			if (values.ContainsKey (Constants.Guide_WebAPI_Key_Summary)) {
				string summary = values [Constants.Guide_WebAPI_Key_Summary];
				g.summary= summary;
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

					//@todo use languages instead
					g.languageList.Add (l [Constants.Guide_WebAPI_Key_Language]);

					GuideLanguage gl = new GuideLanguage ();
					gl.language = l [Constants.Guide_WebAPI_Key_Language];
					gl.languageId = l [Constants.Guide_WebAPI_Key_LanguageId];
					g.languages.Add (gl);
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

