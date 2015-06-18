
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
using System.Collections.Specialized;
using System.Json;

namespace TouriDroid
{
	public class EditCitiesFragment : Fragment
	{
	   	protected List<string> mLocations;
	   	protected LinearLayout mCityList;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.SignupCities, container, false);
			mLocations = new List<string> ();
			Toast toast = new Toast(view.Context);

			AutoCompleteTextView searchPlaces = (AutoCompleteTextView) view.FindViewById (Resource.Id.search_places);

			PlacesAutoCompleteAdapter pacAdapter = new PlacesAutoCompleteAdapter (view.Context, Android.Resource.Layout.SimpleListItem1);
			searchPlaces.Adapter = pacAdapter;

			Button addCity = view.FindViewById<Button> (Resource.Id.addCity);
			Button update = view.FindViewById<Button> (Resource.Id.buttonNext);
			update.Text = "Update";

			mCityList = (LinearLayout) view.FindViewById (Resource.Id.dynamicSelections);

			if ( ((EditGuideValueActivity)Activity).currentGuide.placesServedList==null )
			{
				Log.Debug (Constants.TOURI_TAG, "Error getting current guide");
				Activity.Finish();
				return view;
			}

			List<LocationWrapper> selectedLocs = ((EditGuideValueActivity)Activity).currentGuide.placesServedList;
			foreach (LocationWrapper l in selectedLocs)
			{
				AddLocation(l.location);
			}

			addCity.Click += (object IntentSender, EventArgs e) => {

				Boolean duplicate =false;

				if (string.IsNullOrEmpty(searchPlaces.Text) || string.IsNullOrWhiteSpace(searchPlaces.Text))
				{
					Toast.MakeText (view.Context, "Location cannot be blank", ToastLength.Short).Show();
					return;
				}
					
				foreach (string s in mLocations)
				{
					if (s.Equals(searchPlaces.Text))
					{
						duplicate=true;
					}
				}
				if (!duplicate)
				{
					AddLocation(searchPlaces.Text);


					//cityList.Text+=searchPlaces.Text+"\r\n";
				}
				else
				{
					Toast.MakeText (view.Context, "Location already added", ToastLength.Long).Show();
				}
				searchPlaces.Text="";
			};

			update.Click += async (object IntentSender, EventArgs e) =>  {
				if (mLocations.Count>0)
				{
					SessionManager sm = new SessionManager(view.Context);
					string token = sm.getAuthorizedToken();
					int guideId = sm.getGuideId();

					Guide g = new Guide();
					g.guideId = guideId;
					foreach (string l in mLocations)
					{
						LocationWrapper lw = new LocationWrapper();
						lw.location = l;
						g.placesServedList.Add(lw);
					}		

					SupportFunctions sf = new SupportFunctions();

					int result =  Constants.FAIL;
					result = await sf.UpdateAllGuidesLocations(token, g);
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
					Toast.MakeText (view.Context, "Please add at least one location", ToastLength.Long).Show();
				}
			};

			return view;
		}			


		private int AddLocation(string location)
		{
			int result = Constants.SUCCESS;

			mLocations.Add(location);

			for (;;)
			{
				View cityRow = LayoutInflater.From(Activity).Inflate(Resource.Layout.RowwithDeleteLayout, null);

				TextView city = cityRow.FindViewById<TextView> (Resource.Id.item);
				ImageView remove = cityRow.FindViewById<ImageView> (Resource.Id.remove);

				if (city==null || remove==null)
				{
					result = Constants.FAIL;
					break;
				}

				remove.Click += (object sender, EventArgs events) => 
				{
					mLocations.Remove(city.Text);
					mCityList.RemoveView(cityRow);
				};
				city.Text = location;
				mCityList.AddView(cityRow);
				break;
			}
			return result;
		}
	}
}

