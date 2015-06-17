using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Content;

namespace TouriDroid
{
	public class RecyclerAdapter: RecyclerView.Adapter
	{
		private List<Guide> mGuides;
		private Activity thisActivity;
		private Comms mCa;
		public event EventHandler<int> ItemClick;

		public RecyclerAdapter(List<Guide> guideList, Activity thisAct)
		{
			mGuides = guideList;
			thisActivity = thisAct;
			mCa = new Comms ();
		}

		public class MyView:RecyclerView.ViewHolder
		{
			public View mMainView { get; set; }
			public TextView mFName { get; set;} 
			public TextView mSummary { get; set;} 
			public TextView mAvailability { get; set;} 
			public TextView mLanguages { get; set;}
			public TextView mLocations { get; set;}
			public ImageView mPhoto { get; set; }

			public MyView(View view, Action<int> listener): base(view)
			{
				mMainView = view;
				view.Click += (sender, e) => listener (base.Position);
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			//this is the row_guide layout
			//View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.row_guide, parent, false);

			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cardview_guide, parent, false);
			TextView FName = row.FindViewById<TextView> (Resource.Id.guide_name);
			TextView locations = row.FindViewById<TextView> (Resource.Id.locationsServed);
			TextView languages = row.FindViewById<TextView> (Resource.Id.languages);
			TextView summary = row.FindViewById<TextView> (Resource.Id.description);
			TextView availability = row.FindViewById<TextView> (Resource.Id.availability);
			ImageView photo = row.FindViewById<ImageView> (Resource.Id.guide_photo);

			//set click listener for more button
			Button moreButton = row.FindViewById<Button>(Resource.Id.moreButton);
			LinearLayout moreContainer = row.FindViewById<LinearLayout>(Resource.Id.more);
			TextView moreText = row.FindViewById<TextView> (Resource.Id.moreText);
			moreContainer.Click += (sender, e) => {
				LinearLayout more = (LinearLayout) row.FindViewById(Resource.Id.moreLayout);

				View card = row.FindViewById(Resource.Id.guideCardViewLayout);

				float newHeight = 0;
				if (more.Visibility==ViewStates.Visible)
				{ 
					// make it invisible
					newHeight =card.Height-more.Height;	
					more.Visibility=ViewStates.Gone;
					moreButton.SetBackgroundResource(Resource.Drawable.expander_ic_minimized);
					moreText.Text = "More";
					//moreButton.Background=DRawabl(Resource.Drawable.expander_ic_minimized);
				}
				else //make it visible
				{
					newHeight =card.Height+more.Height;
					more.Visibility=ViewStates.Visible;
					moreButton.SetBackgroundResource(Resource.Drawable.expander_ic_maximized);
					moreText.Text = "Less";
				}

				//more.Alpha=0.0f;

				//TranslateAnimation animateSlideUp = new TranslateAnimation(0,0,0,h);
				//animateSlideUp.FillAfter=true;
				//card.StartAnimation(animateSlideUp);
				//row.LayoutParameters.Height=-2;//row.Height-(int)h;
				//	row.RequestLayout();
				//card.LayoutParameters.Height=card.Height-(int)h;
				card.RequestLayout();
				more.Animate().Alpha(1.0f);
				//page.=page.Height-(int)h;

				//card.Animate().TranslationYBy(h);

			};

			MyView view = new MyView (row, OnClick) { mFName = FName, mLocations=locations, mLanguages = languages, mPhoto=photo, mSummary=summary, mAvailability=availability};
			return view;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			MyView myHolder = holder as MyView;

			string languages = "";
			string placesServed = "";
			// Set name
			myHolder.mFName.Text = mGuides[position].fName + " " + mGuides[position].lName;

			//Set description.. for summary only partial description is shown
			//@todo should their be a summary line for guides like Twitter?
			if ( (mGuides [position].summary!=null) && (mGuides [position].summary.Length > Constants.MaxDescriptionLengthInCard) )
			{
				myHolder.mSummary.Text = mGuides [position].summary.Substring (0, Constants.MaxDescriptionLengthInCard-1) + "...";
			} else 
			{
				myHolder.mSummary.Text = mGuides [position].summary;
			}	

			Converter converter = new Converter ();
			myHolder.mAvailability.Text = converter.getOnlineStatusString (mGuides [position].availability);
			myHolder.mAvailability.SetTextColor( converter.getOnlineStatusColor (mGuides [position].availability));

			foreach (string l in mGuides[position].languageList) {
				languages += "• "+l+"\r\n" ;
			}				
			if (languages.Length > 0) {
				languages = languages.Remove (languages.Length - 2);
			}

			myHolder.mLanguages.Text = languages;

			foreach (LocationWrapper l in mGuides[position].placesServedList) {
				placesServed += "• "+l.location+"\r\n" ;
			}				
			//placesServed = placesServed.Remove (placesServed.Length - 2);
			myHolder.mLocations.Text = placesServed;

			//myHolder.mPhoto.SetImageResource (Resource.Drawable.placeholder_photo);

			//get the thumbnails if available
			//string imageUrl= Constants.DEBUG_BASE_URL + "/api/images/"+ mGuides [position].profileImageId + "/thumbnail";

//			Bitmap image = (Bitmap) await mCa.getScaledImage (imageUrl, Constants.GuideListingReqWidth, Constants.GuideListingReqHeight);
//			mGuides [position].profileImage = image;
			myHolder.mPhoto.SetImageBitmap (mGuides [position].profileImage);		

			//myHolder.ItemView.Click += (sender, e) => {
			LinearLayout content = myHolder.ItemView.FindViewById<LinearLayout> (Resource.Id.guideContentLayout);
		}

		public override int ItemCount{
			get { return mGuides.Count; }
		}

		public Guide GetGuide(int position)
		{
			return mGuides [position];
		}

		void OnClick (int position)
		{
			if (ItemClick != null) {
				ItemClick (this, position);
			}
		}

	}

}

