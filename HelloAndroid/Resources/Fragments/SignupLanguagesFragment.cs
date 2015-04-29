
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
	public class SignupLanguagesFragment : Fragment
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
			List<string> languages = sf.BuildLanguagesTable (view, languagesTable, Resource.Layout.language_tablerow);

			languagesTable.RequestLayout();

			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			next.Click += (object sender, EventArgs e) => {
				if (languages.Count>0)
				{
					((SignUpAsGuideActivity)Activity).newGuide.languageList = languages;
					var newFragment = new SignupNameFragment ();

					FragmentTransaction transaction = FragmentManager.BeginTransaction();
					transaction.Replace(Resource.Id.signinup_fragment_container, newFragment);

					transaction.Commit();					
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

