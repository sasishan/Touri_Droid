
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
using Android.Graphics;
using System.Json;
using Android.Support.V7.Widget;

namespace TouriDroid
{
	public class ExpertiseFragment : Fragment
	{
		private RecyclerView mRecyclerView;
		private RecyclerView.LayoutManager mLayoutManager;
		private RecyclerView.Adapter mAdapter;
		protected List<Expertise> mExpertiseList = new List<Expertise> ();

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			var view = inflater.Inflate(Resource.Layout.fragment_expertise, container, false);
			((MainActivity)this.Activity).setCurrentFragment (Constants.ExpertiseFragment);

			mRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.expertise_recycler_view);

			mLayoutManager =  new LinearLayoutManager(view.Context);//new GridLayoutManager(view.Context, 2, GridLayoutManager.Horizontal, false);

			mRecyclerView.SetLayoutManager (mLayoutManager);
			mAdapter = new RecyclerAdapterExpertise (mExpertiseList, this.Activity);
			((RecyclerAdapterExpertise)mAdapter).ItemClick += OnItemClick;

			mRecyclerView.SetAdapter (mAdapter);

			//string url = Constants.DEBUG_BASE_URL + Constants.URL_Get_All_Expertises;
			string url = Constants.DEBUG_BASE_URL + "/api/expertises/search?locs=";
			string place = ((MainActivity)this.Activity).getPlace();

			if (place.Equals("")) {
				//@todo get current location
				place="Toronto, ON, Canada";
			} else {
				place = ((MainActivity)this.Activity).getPlace();
			}
			url+=place;

			loadExpertises (url);

			return view;
		}
			
		void OnItemClick (object sender, int position)
		{
			Expertise exp = ((RecyclerAdapterExpertise)mAdapter).getExpertise (position);
			string place = ((MainActivity) Activity).getPlace();
			((MainActivity) Activity).mExpertise = exp;

			// pass the Second activity the location and currently selected expertise so 
			// it can start of with it
			var gprofileActivity = new Intent (Activity, typeof(SecondActivity));
			gprofileActivity.PutExtra (Constants.selectedLocation, place);
			gprofileActivity.PutExtra (Constants.selectedExpertise, exp.expertise);		
			this.StartActivity (gprofileActivity);
		}

		public async void loadExpertises(string url)
		{
			CallAPI ca = new CallAPI();

			var json = await ca.getWebApiData(url, null);
			parseExpertises(json);

			// load the images for each expertise now
			string imageUrl;
			List<Expertise> filteredE = new List<Expertise> ();
			foreach (Expertise e in mExpertiseList)
			{
				//@remove this if you want to always show all expertises
				if (e.numberOfGuides >0) {
					filteredE.Add (e);
					//mExpertiseList.Remove (e);
				}

				imageUrl= Constants.DEBUG_BASE_URL + "/api/images/"+ e.expertiseImageId;

				Bitmap image = (Bitmap) await ca.getImage (imageUrl);

				e.expertiseImage = image;
			}
			mExpertiseList.RemoveAll(item => item.numberOfGuides ==0);
			mAdapter.NotifyDataSetChanged ();
		}

		private async void parseExpertises(JsonValue json)
		{
			mExpertiseList.Clear ();

			for (int i = 0; i < json.Count; i++) {
				Expertise e = new Expertise ();

				JsonValue values = json [i];
				if (values.ContainsKey ("expertise")) {
					e.expertise = values ["expertise"];
				}

				if (values.ContainsKey ("imageId")) {					
					e.expertiseImageId = values ["imageId"];
				}					

				if (values.ContainsKey ("description")) {
					e.description = values ["description"];
				}	

				if (values.ContainsKey ("expertiseId")) {
					e.expertiseId = values ["expertiseId"];
				}
				if (values.ContainsKey ("numberOfGuides")) {
					e.numberOfGuides = values ["numberOfGuides"];
				}

				mExpertiseList.Add (e);
			}

		}

	}


	public class RecyclerAdapterExpertise: RecyclerView.Adapter
	{
		private List<Expertise> mExpertise;
		private Activity thisActivity;
		public event EventHandler<int> ItemClick;

		public RecyclerAdapterExpertise(List<Expertise> expertiseList, Activity thisAct)
		{
			mExpertise = expertiseList;
			thisActivity = thisAct;
		}

		public class MyView:RecyclerView.ViewHolder
		{
			public View mMainView { get; set; }
			public TextView mExpertise { get; set;} 
			public TextView mDescription { get; set;} 
			public TextView mGuideCount { get; set;} 
			public ImageView mExpertiseImage { get; set; }

			public MyView(View view, Action<int> listener): base(view)
			{
				mMainView = view;

				view.Click += (sender, e) => listener (base.Position);
				//mMainView.Click += (sender, e) => OrientationListener(base.Position);

			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			//View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cardview_expertise, parent, false);
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_expertise, parent, false);
			TextView expertise = row.FindViewById<TextView> (Resource.Id.expertise);
			TextView guideCount = row.FindViewById<TextView> (Resource.Id.expertise_guide_count);
			ImageView expImage = row.FindViewById<ImageView> (Resource.Id.expertise_image);

			MyView view = new MyView (row, OnClick) { mExpertise = expertise, mGuideCount=guideCount, mExpertiseImage = expImage};
			return view;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			MyView myHolder = holder as MyView;

			// Set name
			myHolder.mExpertise.Text = mExpertise[position].expertise;
			myHolder.mGuideCount.Text = mExpertise[position].numberOfGuides.ToString();
			myHolder.mExpertiseImage.SetImageBitmap(mExpertise[position].expertiseImage);
		}

		public override int ItemCount{
			get { return mExpertise.Count; }
		}

		void OnClick (int position)
		{
			if (ItemClick != null) {
				ItemClick (this, position);
			}
		}

		public Expertise getExpertise(int position)
		{
			return mExpertise [position];
		}
	}
}

