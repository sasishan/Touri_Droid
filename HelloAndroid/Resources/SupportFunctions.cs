using System;
using Android.Widget;
using Android.Views;

using System.Collections.Generic;
using Android.Content;
using System.Collections.Specialized;
using System.Json;
using Android.Util;
using Org.Json;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Android.Graphics;

namespace TouriDroid
{
	public class RegisterModel
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
	}

	public class SessionManager 
	{
		//Shared preferences
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		Context context;


		private const String PrefName = "TouriPref";
		private const String IsGuide = "IsGuide";
		private const String IsLoggedIn = "IsLoggedIn";
		public const String KeyEmail = "email";
		public const String KeyCurrentLatitude = "latitude";
		public const String KeyCurrentLongitude = "longitude";
		public const String KeyCurrentLastLocation = "lastPlace";
		public const String KeyToken = "token";
		public const String KeyGuideId = "guideId";

		public SessionManager (Context pContext)
		{
			context = pContext;
			pref = context.GetSharedPreferences (PrefName, FileCreationMode.Private);
			editor = pref.Edit ();
		}

		public void setCurrentLatitudeLongitude (float lati, float longi)
		{
			editor.PutFloat (KeyCurrentLatitude, lati);
			editor.PutFloat (KeyCurrentLongitude, longi);
			editor.Commit ();
		}

		public void setCurrentLocation (string location)
		{
			editor.PutString (KeyCurrentLastLocation, location);
			editor.Commit ();
		}

		public string getLastLocation()
		{
			return pref.GetString (KeyCurrentLastLocation, "");
		}

		public float getCurrentLatitude()
		{
			return pref.GetFloat (KeyCurrentLatitude, Constants.Uninitialized);
		}

		public float getCurrentLongitude()
		{
			return pref.GetFloat (KeyCurrentLongitude, Constants.Uninitialized);
		}

		public void createLoginSession (String email, String token, Boolean pIsGuide, int guideId)
		{
			editor.PutBoolean (IsLoggedIn, true);
			editor.PutBoolean (IsGuide, pIsGuide);
			editor.PutString (KeyEmail, email);
			editor.PutString (KeyToken, token);
			editor.PutInt (KeyGuideId, guideId);

			editor.Commit ();
		}

		public String getEmail()
		{
			String email = pref.GetString (KeyEmail, null);

			return email;
		}

		public bool isGuide()
		{
			return pref.GetBoolean (IsGuide, false);
		}

		public int getGuideId()
		{
			int guideId=Constants.Uninitialized;
			if (isGuide ()) {
				guideId = pref.GetInt (KeyGuideId, Constants.Uninitialized);
			} 

			return guideId;
		}

		public String getAuthorizedToken()
		{
			String token = pref.GetString (KeyToken, null);

			return token;
		}

		public void checkLogin()
		{
			if (!this.isLoggedIn ()) {
				Intent i = new Intent(context, typeof(LoginOrSignupActivity));
				i.AddFlags (ActivityFlags.ClearTop);

				i.SetFlags (ActivityFlags.NewTask);

				context.StartActivity (i);
			}
		}

		public void logoutUser()
		{
			editor.Remove (KeyToken);
			editor.PutBoolean (IsLoggedIn, false);
			editor.Remove (IsGuide);
			editor.Remove (KeyGuideId);
			editor.Commit();

			//editor.Clear ();
		//	editor.Commit();
		}

		public Boolean isLoggedIn()
		{
			return pref.GetBoolean (IsLoggedIn, false);
		}
	}

	public class SupportFunctions
	{
		private Converter mConverter = null;

		public SupportFunctions ()
		{
			mConverter = new Converter ();
		}

		public async Task<int> GetMyMessages(string accessToken, DataManager dm)
		{
			if (dm == null) {
				return 0;
			}

			string url = Constants.DEBUG_BASE_URL + Constants.URL_MyMessages;
			Comms comms = new Comms();

			var json = await comms.getWebApiData(url, accessToken);
			if (json==null)
			{
				//no need to do anything more
				return 0;
			}

			for (int i = 0; i < json.Count; i++) {
				ChatMessage cm = mConverter.parseOneChatMessage (json[i]);

				if (cm == null) {
					continue;
				}

				//this is not a response from the current user
				cm.MyResponse=Constants.MyResponseNo;
				//add it straight to the DB
				dm.AddMessage(cm);
			}

			return json.Count;
		}



		public async Task<int> UpdateAllGuidesLanguages( string token, Guide guide)
		{
			int result = Constants.SUCCESS;

			Comms ca = new Comms ();
			if (guide == null) {
				Log.Debug (Constants.TOURI_TAG, "Guide is null!");
				return Constants.FAIL;
			}

			string url = Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile +	String.Format (Constants.URL_PostAllGuideLanguages, guide.guideId);

			string data = "";
			foreach (GuideLanguage l in guide.languages) {
				data += "langIds=" + l.languageId.ToString() + "&";
			}
			data = data.Remove (data.Length - 1);

			//@todo make this async later
			JsonValue jsonResponse = await ca.PostDataSyncUsingUri (url, data, token);
			if (jsonResponse == null) {
				result = Constants.FAIL;
			}

			return result;
		}

		public async Task<int> UpdateAllGuidesExpertises( string token, Guide guide)
		{
			int result = Constants.SUCCESS;

			Comms ca = new Comms ();
			if (guide == null) {
				Log.Debug (Constants.TOURI_TAG, "Guide is null!");
				return Constants.FAIL;
			}

			string url = Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile +	String.Format (Constants.URL_PostAllGuideExpertises, guide.guideId);

			string data = "";
			foreach (Expertise e in guide.expertise) {
				data += "expIds=" + e.expertiseId.ToString() + "&";
			}
			data = data.Remove (data.Length - 1);

			//@todo make this async later
			JsonValue jsonResponse = await ca.PostDataSyncUsingUri (url, data, token);
			if (jsonResponse == null) {
				result = Constants.FAIL;
			}

			return result;
		}


		public async Task<int> UpdateAllGuidesLocations( string token, Guide guide)
		{
			int result = Constants.SUCCESS;

			NameValueCollection parameters = new NameValueCollection ();
			Comms ca = new Comms ();

			if (guide == null) {
				Log.Debug (Constants.TOURI_TAG, "Guide is null!");
				return Constants.FAIL;
			}

			string url = Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile +	String.Format (Constants.URL_PostAllGuideLocations, guide.guideId);

			string data = "";
			foreach (LocationWrapper l in guide.placesServedList) {
				data += "locs=" + l.location + "&";
			}
			data = data.Remove (data.Length - 1);

			//@todo make this async later
			JsonValue jsonResponse = await ca.PostDataSyncUsingUri (url, data, token);
			if (jsonResponse == null) {
				result = Constants.FAIL;
			}

			return result;
		}

		//Creates a list of languages that can be checked
		public List<GuideLanguage> BuildLanguagesTable(View view, TableLayout languagesTable, int tableRow, 
			List<GuideLanguage> currentlySelectedLanguageIds)
		{			
			List<GuideLanguage> checkedLanguages = new List<GuideLanguage> ();

			//copy the ids into an int list to enable quick searches
			List<int> selectedIds = new List<int> ();
			if (currentlySelectedLanguageIds.Count > 0) {
				foreach (GuideLanguage gl in currentlySelectedLanguageIds) {
					selectedIds.Add(gl.languageId);
				}
			}

			for (int i = 0; i < Constants.AvailableLanguages.Count; i++) {
				TableRow row = (TableRow)LayoutInflater.From (view.Context).Inflate (tableRow, null);
				CheckBox c = row.FindViewById<CheckBox> (Resource.Id.languageCheck);
				c.Text = Constants.AvailableLanguages [i].Item2;
				c.Id = Constants.AvailableLanguages [i].Item1;

				if (currentlySelectedLanguageIds.Count > 0) {
					if (selectedIds.Contains (c.Id)) {
						c.Checked = true;

						GuideLanguage gl = new GuideLanguage();
						gl.language = c.Text;
						gl.languageId = c.Id;
						checkedLanguages.Add(gl);
					}
				}

				c.Click += (object sender, EventArgs e) => {
					if (c.Checked==true)
					{
						GuideLanguage gl = new GuideLanguage();
						gl.language = c.Text;
						gl.languageId = c.Id;
						checkedLanguages.Add(gl);
					}
					else
					{
						GuideLanguage gl = new GuideLanguage();
						gl.language = c.Text;
						gl.languageId = Convert.ToInt32(c.Tag);
						for (int k=0; k<checkedLanguages.Count;k++)
						{
							if (checkedLanguages[k].languageId==c.Id)
							{
								checkedLanguages.RemoveAt(k);
								break;
							}
						}

					}
				};
				languagesTable.AddView(row);
			}

			return checkedLanguages;
		}

		public int BuildSelectedExpertiseTable(View view, TableLayout tableLayout, int expertiseLayout, 
			List<Expertise> selectedExpertises)
		{			
			int result = Constants.SUCCESS;
			for (;;) {

				if (tableLayout == null || view == null) {
					result = Constants.FAIL;
					break;
				}

				//Show in a table view and add rows once we hit the max columns in a row
				int maxColumns = 3;
				int currentColumn = 0;
				ImageView expImage=null;
				TextView expText = null;
				TableRow row = (TableRow)LayoutInflater.From (view.Context).Inflate (expertiseLayout, null);
				for (int i = 0; i < selectedExpertises.Count; i++) {
					// create a new row 
					if (currentColumn >= maxColumns) {
						currentColumn = 0;
					}

					for (int j = 0; j < Constants.ExpertiseImages.Count; j++) {
						if (selectedExpertises [i].expertise.Equals (Constants.ExpertiseImages [j].Item3)) {
							int downImage = Constants.ExpertiseImages [j].Item2;
							string value = Constants.ExpertiseImages[j].Item3;

							//this code assumes theres 3 columns!
							if (currentColumn == 0) {
								expImage= row.FindViewById<ImageView> (Resource.Id.exp1);
								expText = row.FindViewById<TextView> (Resource.Id.exp1_text);
							} else if (currentColumn == 1) {
								expImage = row.FindViewById<ImageView> (Resource.Id.exp2);
								expText = row.FindViewById<TextView> (Resource.Id.exp2_text);
							} else {
								expImage = row.FindViewById<ImageView> (Resource.Id.exp3);
								expText = row.FindViewById<TextView> (Resource.Id.exp3_text);
							}

							expImage.SetImageResource (downImage);
							expText.Text = value;
							break;

						}
					}
					if (currentColumn == (maxColumns-1)) {
						tableLayout.AddView (row);
						row = (TableRow)LayoutInflater.From (view.Context).Inflate (expertiseLayout, null);
						expImage = null;
						expText = null;
					}
					currentColumn++;
				}

				//if expimage is null that means there's no values to add
				if (expImage != null) {
					tableLayout.AddView (row);
				}
				break;
			}//main FOR

			return result;		
		}

		//Creates expertise images and puts them in a 3x3 table
		//images are held in Constants
		public List<Expertise> BuildExpertiseTable(View view, TableLayout expertiseTable, int tableRowLayout, List<Expertise> alreadySelectedExps)
		{			
			List<Expertise> selectedExpertises = new List<Expertise> ();
			List<int> alreadySelectedExpIds = new List<int> ();
			if (alreadySelectedExps.Count > 0) {
				foreach (Expertise e in alreadySelectedExps) {
					alreadySelectedExpIds.Add (e.expertiseId); //use a list of ints for faster and simple search
				}
			}

			for (int i = 0; i < Constants.ExpertiseImages.Count; i = i + 3) {
				TableRow row = (TableRow)LayoutInflater.From (view.Context).Inflate (tableRowLayout, null);

				ImageButton []buttonArray = new ImageButton[3];
				TextView 	[]textArray = new TextView[3];

				buttonArray[0] = row.FindViewById<ImageButton> (Resource.Id.exp1);
				textArray[0] = row.FindViewById<TextView> (Resource.Id.exp1_text);
				buttonArray[1] = row.FindViewById<ImageButton> (Resource.Id.exp2);
				textArray[1] = row.FindViewById<TextView> (Resource.Id.exp2_text);
				buttonArray[2] = row.FindViewById<ImageButton> (Resource.Id.exp3);
				textArray[2] = row.FindViewById<TextView> (Resource.Id.exp3_text);

				for (int j = 0; j < buttonArray.Length; j++) {
					ImageButton b = buttonArray[j];

					int downImage = Constants.ExpertiseImages [i + j].Item2;
					int upImage = Constants.ExpertiseImages [i + j].Item1;
					string value = Constants.ExpertiseImages[i+j].Item3;
					int expId = Constants.ExpertiseImages[i+j].Item4;
					textArray [j].Text = value;

					if (alreadySelectedExpIds.Contains (expId)) {
						b.SetImageResource (downImage);
						b.Selected = true;
						Expertise expert = new Expertise();
						expert.expertise = value;
						expert.expertiseId = expId;
						selectedExpertises.Add(expert);		

					} else {
						b.SetImageResource (upImage);
					}

					b.Click += (object sender, EventArgs e) => {
						if (!b.Selected)
						{						
							b.SetImageResource (downImage);
							b.Selected=true;
							Expertise expert = new Expertise();
							expert.expertise = value;
							expert.expertiseId = expId;
							selectedExpertises.Add(expert);						
						}
						else
						{
							b.SetImageResource (upImage);
							b.Selected=false;
							for (int c=0; c<selectedExpertises.Count;c++)
							{
								if (selectedExpertises[c].expertiseId==expId)
								{									
									selectedExpertises.RemoveAt(c);
									break;
								}
							}

						}
					};					
				}								

				//	expertiseTable.AddView (blankRow);
				expertiseTable.AddView (row);
			}
			return selectedExpertises;		
		}
	}
}

