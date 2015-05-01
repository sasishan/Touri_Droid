using System;
using Android.Widget;
using Android.Views;

using System.Collections.Generic;
using Android.Content;

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
		public const String KeyToken = "token";
		public const String KeyGuideId = "guideId";

		public SessionManager (Context pContext)
		{
			context = pContext;
			pref = context.GetSharedPreferences (PrefName, FileCreationMode.Private);
			editor = pref.Edit ();
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
		public SupportFunctions ()
		{
		}

		//Creates a list of languages that can be checked
		public List<string> BuildLanguagesTable(View view, TableLayout languagesTable, int tableRow)
		{			
			List<string> checkedLanguages = new List<string> ();

			for (int i = 0; i < Constants.AvailableLanguages.Length; i++) {
				TableRow row = (TableRow)LayoutInflater.From (view.Context).Inflate (tableRow, null);
				CheckBox c = row.FindViewById<CheckBox> (Resource.Id.languageCheck);
				c.Text = Constants.AvailableLanguages [i];

				c.Click += (object sender, EventArgs e) => {
					if (c.Checked==true)
					{
						checkedLanguages.Add(c.Text);
					}
					else
					{
						checkedLanguages.Remove(c.Text);
					}
				};
				languagesTable.AddView(row);
			}

			return checkedLanguages;
		}

		//Creates expertise images and puts them in a 3x3 table
		//images are held in Constants
		public List<string> BuildExpertiseTable(View view, TableLayout expertiseTable, int tableRow)
		{			
			List<string> selectedExpertises = new List<string> ();
			for (int i = 0; i < Constants.ExpertiseImages.Count; i = i + 3) {
				TableRow row = (TableRow)LayoutInflater.From (view.Context).Inflate (tableRow, null);

				ImageButton []buttonArray = new ImageButton[3];

				buttonArray[0] = row.FindViewById<ImageButton> (Resource.Id.exp1);
				buttonArray[1] = row.FindViewById<ImageButton> (Resource.Id.exp2);
				buttonArray[2] = row.FindViewById<ImageButton> (Resource.Id.exp3);

				for (int j = 0; j < buttonArray.Length; j++) {
					ImageButton b = buttonArray[j];
					int downImage = Constants.ExpertiseImages [i + j].Item2;
					int upImage = Constants.ExpertiseImages [i + j].Item1;
					string value = Constants.ExpertiseImages[i+j].Item3;
					b.SetImageResource (upImage);

					b.Click += (object sender, EventArgs e) => {
						if (!b.Selected)
						{						
							b.SetImageResource (downImage);
							b.Selected=true;
							selectedExpertises.Add(value);
						
						}
						else
						{
							b.SetImageResource (upImage);
							b.Selected=false;
							selectedExpertises.Remove(value);
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

