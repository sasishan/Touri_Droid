using System;
using Android.Graphics;

namespace HelloAndroid
{
	public static class Constants
	{
		//URL values for WebApi calls
		public const string DEBUG_BASE_URL = "http://192.168.0.12:50467";//"http://192.168.1.14:50467";
		public const string URL_Get_All_Guides = "/api/guides";
		public const string URL_SearchGuides = "/api/guides/search?";
		public const string URL_Get_All_Expertises = "/api/expertises";

		// WebApi Keys
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
		public const string AvailableNowString = "Available now";
		public const string AvailableLaterString = "Available later";
		public const string NotAvailableForChatString = "Not Available for Chat";

		public const string NoValue = "N/A";

		// Length of the About me summary in the list view of Guide Cardview
		public const int MaxDescriptionLengthInCard = 100;

		// Drawer Items and Identifiers - this is the order they're displayed
		public const int DrawerMainMenuId = 1;
		public const int DrawerLanguageOptionsId = 2;
		public const int DrawerExpertiseOptionsId = 3;

		public const string DrawerTitle = "Refine";
		public const string DrawerOptionLanguage = "Languages";
		public const string DrawerOptionExpertise = "Specialties";

		public const string DrawerOptionDone = "Done";

		//fragment types
		public const int GuideFragment=0;
		public const int ExpertiseFragment=1;
		public const int Uninitialized=-1;

		//image size for profile pictures
		public const int ProfileReqWidth=60;
		public const int ProfileReqHeight=60;
	}
}

