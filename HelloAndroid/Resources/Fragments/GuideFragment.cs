	using System;
using System.Net;
using System.IO;
using System.Json;
using System.Threading.Tasks;

using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Content;
using Android.Graphics;
using Java.Net;
using Android.Views.Animations;
using Android.Animation;
using Android.Support.V4.View;
using System.Collections.Specialized;
using System.Text;
using Android.Util;

namespace TouriDroid
{
	public class GuideFragment : Fragment
	{		
		private	const string TAG = "GuideFragment";
		private RecyclerView 				mRecyclerView;
		private RecyclerView.LayoutManager 	mLayoutManager;
		private RecyclerView.Adapter 		mAdapter;
		protected List<Guide> 				mGuideList = new List<Guide> ();
		public string 						mPlace="";
		public GuideSearch 					mGuideSearch;
		private TextView 					noGuides = null;
		private bool 						guidesLoaded = true;
		protected Comms						mComms;
		private Converter 					mConverter;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			((SecondActivity)Activity).SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			((SecondActivity)Activity).SupportActionBar.SetHomeButtonEnabled (true);
			mComms	= new Comms ();
			mConverter = new Converter ();

		}	

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_guide, container, false);
			SetHasOptionsMenu(true);

			if (mGuideSearch == null) {
				mGuideSearch = ((SecondActivity)this.Activity).mGuideSearch;
			}

			ImageView mapIcon  = view.FindViewById<ImageView> (Resource.Id.mapicon);
			noGuides= view.FindViewById<TextView> (Resource.Id.noguides);

			mRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.my_recycler_view);
			mLayoutManager =  new LinearLayoutManager(view.Context);//new GridLayoutManager(view.Context, 2, GridLayoutManager.Horizontal, false);		
			mRecyclerView.SetLayoutManager (mLayoutManager);

			mPlace = Activity.Intent.GetStringExtra (Constants.selectedLocation) ?? "";
			string expertise = Activity.Intent.GetStringExtra (Constants.selectedExpertise) ?? "";

			mGuideSearch.placesServedList.Clear ();
			mGuideSearch.placesServedList.Add (mPlace);
			mGuideSearch.expertiseList.Add (expertise);

			RefineSearch(mGuideSearch);

			mAdapter = new RecyclerAdapter (mGuideList, this.Activity);
			mRecyclerView.SetAdapter (mAdapter);

			mapIcon.Click += (object sender, EventArgs e) => {

				if (guidesLoaded==true)
				{
					var newFragment = new Map_Fragment ();
					//var ft = FragmentManager.BeginTransaction ();
					FragmentTransaction transaction = FragmentManager.BeginTransaction();

					transaction.Replace(Resource.Id.fragment_container, newFragment);
					transaction.AddToBackStack(null);
					//transaction.AddToBackStack(null);
					transaction.Commit();
				}
			};

			return view;
		}

		public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
		{
			menu.Clear ();
			menuInflater.Inflate(Resource.Menu.menu_filters, menu);
			var item = menu.FindItem (Resource.Id.map);

			base.OnCreateOptionsMenu(menu, menuInflater);				
		}

		private void searchPlaces_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (sender != null) {

				string place = ((AutoCompleteTextView)sender).Text;
				mGuideSearch.placesServedList.Clear ();
				mGuideSearch.placesServedList.Add (place);
				GuideFragment gf = FragmentManager.FindFragmentById<GuideFragment> (Resource.Id.fragment_container);
				gf.RefineSearch(mGuideSearch);
			} 
			else 
			{
				//@todo
			}
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			//load filter fragment
			if (item.ItemId == Resource.Id.filter) {
				var newFragment = new FilterFragment ();
				//var ft = FragmentManager.BeginTransaction ();
				FragmentTransaction transaction = FragmentManager.BeginTransaction ();

				// Replace whatever is in the fragment_container view with this fragment,
				// and add the transaction to the back stack if needed
				transaction.Replace (Resource.Id.fragment_container, newFragment);
			//	transaction.AddToBackStack("GuideFragment");
				//transaction.AddToBackStack(null);

				// Commit the transaction
				transaction.Commit ();
			} else if (item.ItemId == Resource.Id.map) {
				
				if (guidesLoaded==true)
				{
					var newFragment = new Map_Fragment ();
					//var ft = FragmentManager.BeginTransaction ();
					FragmentTransaction transaction = FragmentManager.BeginTransaction();
					transaction.Replace(Resource.Id.fragment_container, newFragment);
					transaction.AddToBackStack(null);
					//transaction.AddToBackStack(null);
					transaction.Commit();
				}
			}
			
			return base.OnOptionsItemSelected (item);
		}

		public async void RefineSearch(GuideSearch guideSearch)
		{
			string url;
			bool atLeastOneSearchParameter = false;

			url = Constants.DEBUG_BASE_URL;
			//check places first
			url += Constants.URL_SearchGuides;
			if (guideSearch.placesServedList.Count>0)
			{				
				atLeastOneSearchParameter = true;
				foreach(string l in guideSearch.placesServedList)
				{
					url += "locs=" + l +"&";
				}
			}

			if (guideSearch.languageList.Count>0)
			{	
				atLeastOneSearchParameter = true;
				foreach(string l in guideSearch.languageList)
				{
					url += "langs=" + l +"&";
				}
			}

			if (guideSearch.expertiseList.Count>0)
			{				
				atLeastOneSearchParameter = true;
				foreach(string l in guideSearch.expertiseList)
				{
					url += "exps=" + l +"&";
				}
			}
			url = url.Remove (url.Length - 1);	

			if (atLeastOneSearchParameter == false) 
			{
				url = Constants.DEBUG_BASE_URL+Constants.URL_Get_All_Guides;
			}
				
			guidesLoaded = false;
			await loadGuideProfiles (url);
			guidesLoaded = true;
		}


		public async Task<List<Guide>> loadGuideProfiles(string url)
		{
			mGuideList.Clear ();
			((SecondActivity)this.Activity).availableLanguages.Clear ();

			if (mConverter == null || mComms==null) {
				Log.Debug(TAG, "mConverter or mCA was null");
				return mGuideList;
			}

			JsonValue json = await mComms.getWebApiData(url, null);

			if (json != null) 
			{
				for (int i = 0; i < json.Count; i++) {
					Guide g = mConverter.parseOneGuideProfile (json[i]);

					if (g == null) {
						continue;
					}
					mGuideList.Add (g);
					mAdapter.NotifyItemInserted (i);

					//add the languages to filterable languages list for all guides
					foreach (string l in g.languageList) {
						((SecondActivity)this.Activity).availableLanguages.Add (l);
					}
				}

				((SecondActivity)this.Activity).mGuideList = mGuideList;
			//	mAdapter.NotifyDataSetChanged ();

				// If there are no guides, set the message
				if (mGuideList.Count == 0) {
					noGuides.Visibility = ViewStates.Visible;
				} else {
					noGuides.Visibility = ViewStates.Gone;
				}				
			}
			return mGuideList;
		}
	}
}

