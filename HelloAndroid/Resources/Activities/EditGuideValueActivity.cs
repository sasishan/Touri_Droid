
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

namespace TouriDroid
{
	[Activity (Label = "Edit Guide", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]	
	public class EditGuideValueActivity : Activity
	{
		public Guide currentGuide;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.SignInSignUp_layout);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);
			currentGuide = new Guide ();
			string action = Intent.GetStringExtra (Constants.Action) ?? "Data not available";
			this.Title = action;
			Fragment newFragment=null;

			//Use the guide object and fill out the values that are current depending on the action
			//the appropriate fragment will reference the right variables in current guide depending on whats 
			//being changed
			if (action.Equals (Constants.Action_EditName)) {
				string fName = Intent.GetStringExtra (Constants.guideFirstName) ?? "Data not available";
				string lName = Intent.GetStringExtra (Constants.guideLastName) ?? "Data not available";

				currentGuide.fName = fName;
				currentGuide.lName = lName;
				//load the Guide Fragment
				newFragment = new EditNameFragment ();
			}
			else if (action.Equals(Constants.Action_EditDescription))
			{
				string description = Intent.GetStringExtra (Constants.guideDescription) ?? "Data not available";
				currentGuide.description = description;

				newFragment = new EditDescriptionFragment ();
			}
			else if (action.Equals(Constants.Action_EditLocations))
			{
				string listOfLocations = Intent.GetStringExtra (Constants.selectedLocation) ?? "";
				if (!string.IsNullOrEmpty (listOfLocations)) {
					string[] splitLocations = listOfLocations.Split (Constants.separator);
					foreach (string s in splitLocations) {
						LocationWrapper locItem = new LocationWrapper ();

						//for the edit of locations we only need the location string
						locItem.location = s;
						currentGuide.placesServedList.Add (locItem);
					}
				}

				newFragment = new EditCitiesFragment ();
			}
			else if (action.Equals(Constants.Action_EditLanguages))
			{
				string listOfLanguages = Intent.GetStringExtra (Constants.selectedLanguages) ?? "";
				if (!string.IsNullOrEmpty (listOfLanguages)) {
					string[] splitLanguages = listOfLanguages.Split (Constants.separator);
					foreach (string s in splitLanguages) {
						GuideLanguage langItem = new GuideLanguage ();

						//for the edit of languages we only need the language Id
						langItem.languageId = Convert.ToInt32(s);
						currentGuide.languages.Add (langItem);
					}
				}

				newFragment = new EditLanguagesFragment ();
			}
			else if (action.Equals(Constants.Action_EditExpertise))
			{
				string listOfExpertises = Intent.GetStringExtra (Constants.selectedExpertise) ?? "";
				if (!string.IsNullOrEmpty (listOfExpertises)) {
					string[] splitExpertises = listOfExpertises.Split (Constants.separator);
					foreach (string s in splitExpertises) {
						Expertise expertiseItem = new Expertise ();

						//for the edit of languages we only need the language Id
						expertiseItem.expertiseId = Convert.ToInt32(s);
						currentGuide.expertise.Add (expertiseItem);
					}
				}

				newFragment = new EditExpertiseFragment();
			}


			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.signinup_fragment_container, newFragment);

			ft.Commit ();	
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

