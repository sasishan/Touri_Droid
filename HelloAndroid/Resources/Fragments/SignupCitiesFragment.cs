
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

			LinearLayout cityList = (LinearLayout) view.FindViewById (Resource.Id.dynamicSelections);

			addCity.Click += (object IntentSender, EventArgs e) => {

				Boolean duplicate =false;
				if (searchPlaces.Text.Equals(""))
				{
					Toast.MakeText (view.Context, "Location cannot be blank", ToastLength.Short).Show();
					return;
				}
					
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

					View cityRow = LayoutInflater.From(Activity).Inflate(Resource.Layout.RowwithDeleteLayout, null);

					TextView city = cityRow.FindViewById<TextView> (Resource.Id.item);
					ImageView remove = cityRow.FindViewById<ImageView> (Resource.Id.remove);

					remove.Click += (object sender, EventArgs events) => 
					{
						locations.Remove(city.Text);
						cityList.RemoveView(cityRow);
					};
					city.Text = searchPlaces.Text;
					cityList.AddView(cityRow);

					//cityList.Text+=searchPlaces.Text+"\r\n";
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
					List<LocationWrapper> lwList = new List<LocationWrapper>();
					foreach (string l in locations)
					{
						LocationWrapper lw = new LocationWrapper();
						lw.location = l;
						lwList.Add(lw);
					}
					((SignUpAsGuideActivity)Activity).newGuide.placesServedList = lwList;

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

