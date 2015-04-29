
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
	public class SignupCitiesFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.SignupCities, container, false);
			List<string> locations = new List<string> ();
			Toast toast = new Toast(view.Context);
			ToastLength duration = ToastLength.Short;

			AutoCompleteTextView searchPlaces = (AutoCompleteTextView) view.FindViewById (Resource.Id.search_places);

			PlacesAutoCompleteAdapter pacAdapter = new PlacesAutoCompleteAdapter (view.Context, Android.Resource.Layout.SimpleListItem1);
			searchPlaces.Adapter = pacAdapter;

			Button addCity = view.FindViewById<Button> (Resource.Id.addCity);
			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			TextView cityList = (TextView) view.FindViewById (Resource.Id.cityList);
			addCity.Click += (object IntentSender, EventArgs e) => {

				Boolean duplicate =false;
				foreach (string s in locations)
				{
					if (s.Equals(searchPlaces.Text))
					{
						duplicate=true;
					}
				}
				if (!duplicate)
				{
					locations.Add(searchPlaces.Text);
					cityList.Text+=searchPlaces.Text+"\r\n";
				}
				else
				{
					Toast.MakeText (view.Context, "Location already added", ToastLength.Long).Show();
				}
				searchPlaces.Text="";
			};

			next.Click += (object IntentSender, EventArgs e) => {

				if (locations.Count>0)
				{
					((SignUpAsGuideActivity)Activity).newGuide.placesServedList = locations;

					var newFragment = new SignupExpertiseFragment ();

					FragmentTransaction transaction = FragmentManager.BeginTransaction();
					transaction.Replace(Resource.Id.signinup_fragment_container, newFragment);

					transaction.Commit();					
				}
				else
				{					
					Toast.MakeText (view.Context, "Please add at least one location", ToastLength.Long).Show();
				}
			};

			return view;
		}			

	}
}

