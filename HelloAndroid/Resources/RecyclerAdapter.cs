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

			public MyView(View view): base(view)
			{
				mMainView = view;
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
			moreButton.Click += (sender, e) => {
				LinearLayout more = (LinearLayout) row.FindViewById(Resource.Id.moreLayout);

				View card = row.FindViewById(Resource.Id.guideCardViewLayout);

				float newHeight = 0;
				if (more.Visibility==ViewStates.Visible)
				{ 
					// make it invisible
					newHeight =card.Height-more.Height;	
					more.Visibility=ViewStates.Gone;
					moreButton.SetBackgroundResource(Resource.Drawable.expander_ic_minimized);
					//moreButton.Background=DRawabl(Resource.Drawable.expander_ic_minimized);
				}
				else //make it visible
				{
					newHeight =card.Height+more.Height;
					more.Visibility=ViewStates.Visible;
					moreButton.SetBackgroundResource(Resource.Drawable.expander_ic_maximized);
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


			MyView view = new MyView (row) { mFName = FName, mLocations=locations, mLanguages = languages, mPhoto=photo, mSummary=summary, mAvailability=availability};
			return view;
		}

		public async override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
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

			string imageUrl= Constants.DEBUG_BASE_URL + "/api/images/"+ mGuides [position].profileImageId;

			Bitmap image = (Bitmap) await mCa.getScaledImage (imageUrl, Constants.GuideListingReqWidth, Constants.GuideListingReqHeight);
			mGuides [position].profileImage = image;
			myHolder.mPhoto.SetImageBitmap (mGuides [position].profileImage);		

			//myHolder.ItemView.Click += (sender, e) => {
			LinearLayout content = myHolder.ItemView.FindViewById<LinearLayout> (Resource.Id.guideContentLayout);

			content.Click += (sender, e) => {
				int itemPosition = myHolder.Position;

				var gprofileActivity = new Intent (thisActivity, typeof(GuideProfileActivity));
				gprofileActivity.PutExtra ("GuideId", mGuides[itemPosition].guideId.ToString());
				gprofileActivity.PutExtra ("UName", mGuides[itemPosition].userName);
				gprofileActivity.PutExtra ("FName", mGuides[itemPosition].fName);
				gprofileActivity.PutExtra ("LName", mGuides[itemPosition].lName);

				string langs="";
				foreach(string l in mGuides[itemPosition].languageList)
				{
					langs+=l+"; ";
				}

				string expertises="";
				foreach(Expertise exp in mGuides[itemPosition].expertise)
				{
					expertises+=exp.expertise+"; ";
				}
				if (mGuides[itemPosition].expertise.Count>0)
				{
					langs = langs.Remove (langs.Length - 2);
					expertises = expertises.Remove (expertises.Length - 2);
				}

				gprofileActivity.PutExtra ("Languages", langs);
				gprofileActivity.PutExtra ("Description", mGuides[itemPosition].description);
				gprofileActivity.PutExtra ("Expertise", expertises);
				//		gprofileActivity.PutExtra ("JSON", mGuides[itemPosition].jsonText.ToString());

				thisActivity.StartActivity(gprofileActivity);

				//				StartActivity (typeof(GuideProfileActivity));

			};
		}

		public override int ItemCount{
			get { return mGuides.Count; }
		}
	}

}

