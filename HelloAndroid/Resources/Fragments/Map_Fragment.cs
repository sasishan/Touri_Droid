
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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;

namespace TouriDroid
{
	public class Map_Fragment : Fragment, IOnMapReadyCallback, Android.Gms.Maps.GoogleMap.IOnMarkerClickListener,Android.Gms.Maps.GoogleMap.IOnInfoWindowClickListener
	{
		//private LocationManager _locationManager = null;
		private Location _currentLocation;
		protected List<Guide> mGuideList = new List<Guide> ();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu(true);
			// Create your fragment here
		}

		public override void OnDestroyView() {
			base.OnDestroyView();
			MapFragment f = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			if (f != null) {
				FragmentManager.BeginTransaction().Remove(f).Commit ();
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			//_locationManager = Activity.GetSystemService (Context.LocationService) as LocationManager;
			var view = inflater.Inflate(Resource.Layout.MapView, container, false);
			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			mapFrag.GetMapAsync (this);

			return view;
		}

		public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
		{
			menu.Clear ();
		}

		private void markGuides()
		{
			if (map == null) {
				return;
			}
			//The guide list is set to the activity in GuideFragment's refineSearch method
			//mGuideSearch = ((SecondActivity)this.Activity).mGuideSearch;
			List<Guide> mGuideList =  ((SecondActivity)Activity).mGuideList;

			foreach (Guide g in mGuideList) {		
				foreach (LocationWrapper lw in g.placesServedList) {
					LatLng l = new LatLng (lw.latitude, lw.longitude);
					string snip = g.description;
					setMapLocation (l, g.fName+" "+g.lName, g.guideId.ToString(), BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueAzure));
				}
			}
		}

		public Boolean OnMarkerClick(Marker arg0)
		{
			if (arg0.Id.Equals("1")) // The ID of a specific marker the user clicked on.
			{
				//_map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(20.72110, -156.44776), 13));
			}
			else
			{
				arg0.ShowInfoWindow ();
				Toast.MakeText(Activity, String.Format("You clicked on Marker ID {0}", arg0.Id), ToastLength.Short).Show();
			}
			return true;
		}

		public void OnInfoWindowClick (Marker arg0)
		{
			if (arg0.Snippet.Equals(Constants.Uninitialized.ToString())) // The ID of a specific marker the user clicked on.
			{
				//_map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(20.72110, -156.44776), 13));
			}
			else
			{
				var gprofileActivity = new Intent (Activity, typeof(GuideProfileActivity));
				gprofileActivity.PutExtra ("GuideId", arg0.Snippet);
				Activity.StartActivity(gprofileActivity);

			}
		}

		public override void OnPause ()
		{
			base.OnPause ();
			//_locationManager.RemoveUpdates (this);
		}

		public override void OnResume ()
		{
			base.OnResume ();
		//	string Provider = LocationManager.GpsProvider;

		//	if(_locationManager.IsProviderEnabled(Provider))
		//	{
			//	_locationManager.RequestLocationUpdates (Provider, 0, 100, this);
		//	} 
		//	else 
		//	{
		//		Log.Info( "loc", Provider + " is not available. Does the device have location services enabled?");
		//	}
		}

		public void OnLocationChanged (Location location)
		{
			_currentLocation = location;
			LatLng loc = new LatLng (_currentLocation.Latitude, _currentLocation.Longitude);
			setMapLocation (loc, "Your Location", Constants.Uninitialized.ToString(), BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueCyan));
			setCameraLocation(loc);
			markGuides ();
		//	_locationManager.RemoveUpdates (this);

		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}

		public GoogleMap map { get; private set; }

		private void setCameraLocation(LatLng location)
		{
			CameraPosition.Builder builder = CameraPosition.InvokeBuilder ();
			builder.Target (location);
			builder.Zoom(10);
			CameraPosition cameraPosition = builder.Build ();
			CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition (cameraPosition);
			map.MoveCamera (cameraUpdate);
		}

		private void setMapLocation (LatLng location, string title, string snippet, BitmapDescriptor icon)
		{
			if (location != null) {
				if (map != null) {
					map.UiSettings.ZoomControlsEnabled = true;
					//map.UiSettings.CompassEnabled = true;

					MarkerOptions markerOpt1 = new MarkerOptions();
					markerOpt1.SetPosition(location);
					markerOpt1.SetTitle(title);
					//markerOpt1.SetSnippet (snippet);
					markerOpt1.InvokeIcon(icon);
					markerOpt1.SetSnippet (snippet);
					map.AddMarker(markerOpt1);
				}
			}
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			map.SetOnMarkerClickListener (this);	
			map.SetOnInfoWindowClickListener (this);

			SessionManager sm = new SessionManager (Activity);
			double latit = sm.getCurrentLatitude ();
			double longit = sm.getCurrentLongitude ();

			if (latit != Constants.Uninitialized) {
				LatLng loc = new LatLng (latit, longit);
				setMapLocation (loc, "Your Location", "-1", BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueCyan));
				setCameraLocation(loc);
				markGuides ();
			}
		}
	}
}

