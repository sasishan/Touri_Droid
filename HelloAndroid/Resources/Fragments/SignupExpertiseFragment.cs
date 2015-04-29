
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
	public class SignupExpertiseFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.SignupExpertise, container, false);

			SupportFunctions sf = new SupportFunctions ();

			// build out the expertises table
			TableLayout expertiseTable = (TableLayout)view.FindViewById (Resource.Id.table_Expertise);
			List<string> expertises = sf.BuildExpertiseTable (view, expertiseTable, Resource.Layout.expertise_tablerow);

			expertiseTable.RequestLayout();

			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			next.Click += (object sender, EventArgs e) => {
				if (expertises.Count>0)
				{
					List<Expertise> expertiseList = new List<Expertise>();
					foreach(string expertiseString in expertises)
					{
						Expertise exp = new Expertise();
						exp.expertise = expertiseString;
						expertiseList.Add(exp);
					}

					((SignUpAsGuideActivity)Activity).newGuide.expertise = expertiseList;
					var newFragment = new SignupLanguagesFragment ();

					FragmentTransaction transaction = FragmentManager.BeginTransaction();
					transaction.Replace(Resource.Id.signinup_fragment_container, newFragment);

					transaction.Commit();					
				}
				else
				{
					Toast.MakeText (view.Context, "Please select at least one specialty", ToastLength.Long).Show();
				}
			};

			return view;
		}
	}
}

