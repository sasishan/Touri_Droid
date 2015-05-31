
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

namespace TouriDroid
{
	public class EditLanguagesFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.SignupLanguages, container, false);

			SupportFunctions sf = new SupportFunctions ();

			// build out the expertises table
			TableLayout languagesTable = (TableLayout)view.FindViewById (Resource.Id.table_Languages);

			List<GuideLanguage> selectedLanguages = ((EditGuideValueActivity)Activity).currentGuide.languages;

			List<GuideLanguage> languages = sf.BuildLanguagesTable (view, languagesTable, Resource.Layout.language_tablerow, selectedLanguages);

			languagesTable.RequestLayout();

			Button update = view.FindViewById<Button> (Resource.Id.buttonNext);
			update.Text = "Update";

			update.Click += async (object sender, EventArgs e) => {
				if (languages.Count>0)
				{
					SessionManager sm = new SessionManager(view.Context);
					string token = sm.getAuthorizedToken();
					int guideId = sm.getGuideId();

					Guide g = new Guide();
					g.guideId = guideId;
					foreach (GuideLanguage lang in languages)
					{
						g.languages.Add(lang);
					}		

					int result =  Constants.FAIL;
					result = await sf.UpdateAllGuidesLanguages(token, g);
					if (result!=Constants.SUCCESS)
					{
						Toast.MakeText (view.Context, "Could not update locations", ToastLength.Long).Show();
					}

					Activity.Finish();
					Intent i = new Intent(view.Context, typeof(GuidingActivity));
					// Closing all the backstack Activities
					i.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask); 
					this.StartActivity (i);					
				}
				else
				{
					Toast.MakeText (view.Context, "Please select at least one language", ToastLength.Long).Show();
				}
			};

			return view;
		}
	}
}

