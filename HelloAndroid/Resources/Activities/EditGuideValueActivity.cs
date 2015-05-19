
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

