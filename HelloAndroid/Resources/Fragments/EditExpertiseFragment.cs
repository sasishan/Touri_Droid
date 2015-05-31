
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
	public class EditExpertiseFragment : Fragment
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
			List<Expertise> currentExpertises = ((EditGuideValueActivity)Activity).currentGuide.expertise;;
			TableLayout expertiseTable = (TableLayout)view.FindViewById (Resource.Id.table_Expertise);
			List<Expertise> selectedExpertises = sf.BuildExpertiseTable (view, expertiseTable, Resource.Layout.expertise_tablerow, 
				currentExpertises);

			expertiseTable.RequestLayout();

			Button update = view.FindViewById<Button> (Resource.Id.buttonNext);
			update.Text = "Update";

			update.Click += async (object sender, EventArgs e) => {
				if (selectedExpertises.Count>0)
				{
					SessionManager sm = new SessionManager(view.Context);
					string token = sm.getAuthorizedToken();
					int guideId = sm.getGuideId();

					Guide g = new Guide();
					g.guideId = guideId;
					foreach (Expertise exp in selectedExpertises)
					{
						g.expertise.Add(exp);
					}		

					int result =  Constants.FAIL;
					result = await sf.UpdateAllGuidesExpertises(token, g);
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
					Toast.MakeText (view.Context, "Please select at least one specialty", ToastLength.Long).Show();
				}
			};

			return view;
		}
	}
}

