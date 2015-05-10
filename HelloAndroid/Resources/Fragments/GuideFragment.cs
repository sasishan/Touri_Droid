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

namespace TouriDroid
{
	public class GuideFragment : Fragment
	{		
		private RecyclerView 				mRecyclerView;
		private RecyclerView.LayoutManager 	mLayoutManager;
		private RecyclerView.Adapter 		mAdapter;
		protected List<Guide> 				mGuideList = new List<Guide> ();
		public string 						mPlace="";
		public GuideSearch 					mGuideSearch;
		private TextView 					noGuides = null;

		public override void OnCreate (Bundle savedInstanceState)
		{
			mGuideSearch = ((SecondActivity)this.Activity).mGuideSearch;
			base.OnCreate (savedInstanceState);


			SetHasOptionsMenu(true);
		}			

		public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
		{
			//menu.Clear ();
			menuInflater.Inflate(Resource.Menu.menu_filters, menu);
			mPlace = Activity.Intent.GetStringExtra (Constants.selectedLocation) ?? "";
			string expertise = Activity.Intent.GetStringExtra (Constants.selectedExpertise) ?? "";

			var item = menu.FindItem (Resource.Id.search);

			if (item != null) {
				View v = (View) MenuItemCompat.GetActionView (item);
				AutoCompleteTextView searchPlaces = (AutoCompleteTextView) v.FindViewById (Resource.Id.search_places);

				PlacesAutoCompleteAdapter pacAdapter = new PlacesAutoCompleteAdapter (v.Context, Android.Resource.Layout.SimpleListItem1);
				searchPlaces.Adapter = pacAdapter;
				searchPlaces.ItemClick += searchPlaces_ItemClick;
				searchPlaces.Text = mPlace;

			}
				
			mGuideSearch.placesServedList.Clear ();
			mGuideSearch.placesServedList.Add (mPlace);
			mGuideSearch.expertiseList.Add (expertise);

			RefineSearch(mGuideSearch);

			//var item = menu.FindItem (Resource.Id.filter);
			//item.SetOnMenuItemClickListener(

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
				FragmentTransaction transaction = FragmentManager.BeginTransaction();

				// Replace whatever is in the fragment_container view with this fragment,
				// and add the transaction to the back stack if needed
				transaction.Replace(Resource.Id.fragment_container, newFragment);
				//transaction.AddToBackStack(null);

				// Commit the transaction
				transaction.Commit();
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

			if (atLeastOneSearchParameter == false) {
				url = Constants.DEBUG_BASE_URL+Constants.URL_Get_All_Guides;
			}
				
			await loadGuideProfiles (url);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_guide, container, false);
			//((MainActivity)this.Activity).setCurrentFragment (Constants.GuideFragment);

			// Get our button from the layout resource,
			// and attach an event to it
			//Button button = view.FindViewById<Button> (Resource.Id.nextButton);
			//TextView text =  view.FindViewById<TextView> (Resource.Id.guideFirstName);

	//		Button button = (Button) ((SecondActivity)this.Activity).FindViewById(Resource.Id.moreButton);
			noGuides= view.FindViewById<TextView> (Resource.Id.noguides);
			mRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.my_recycler_view);

			mLayoutManager =  new LinearLayoutManager(view.Context);//new GridLayoutManager(view.Context, 2, GridLayoutManager.Horizontal, false);

			mRecyclerView.SetLayoutManager (mLayoutManager);


			//mRecyclerView.HasFixedSize = true;
			//mRecyclerView.SetItemAnimator(new DefaultItemAnimator ());
			CallAPI ca = new CallAPI();
			//string searchView = this.View.FindViewById<AutoCompleteTextView> (Resource.Id.search);
			GuideSearch search = new GuideSearch();
			search.placesServedList.Add (((SecondActivity)this.Activity).mPlace);
			search.expertiseList.Add(((SecondActivity)this.Activity).mExpertise);
		//	RefineSearch (search);
			//string url = Constants.DEBUG_BASE_URL + Constants.URL_Get_All_Guides;
			//loadGuideProfiles (url);
			mAdapter = new RecyclerAdapter (mGuideList, this.Activity);
			mRecyclerView.SetAdapter (mAdapter);


			// When the user clicks the button ...
		/*	int currentGuideIndex = 0;
			button.Click += delegate {
				if (currentGuideIndex>=guideList.Count)
				{
					currentGuideIndex=0;
				}
				string s =  guideList[currentGuideIndex].fName + " " + guideList[currentGuideIndex].lName + ", Languages spoken: ";
				foreach (string l in guideList[currentGuideIndex].languageList)
				{
					s+=l+", ";
				}


				for (int i = 0; i < guideList[currentGuideIndex].languageList.Count; i++)
				{
					CheckBox cb = new CheckBox(view.Context);
					cb.Text = (string)guideList[currentGuideIndex].languageList[i];
					cb.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
					//view.AddContentView(cb, cb.LayoutParameters);

				}

				s.Remove(s.Length-3);
				text.Text = s;
				currentGuideIndex++;
			};*/

			return view;
		}

		public void parseGuideProfiles(JsonValue json)
		{
			if (json == null) {
				return;
			}
			mGuideList.Clear ();
			((SecondActivity)this.Activity).availableLanguages.Clear ();

			for (int i = 0; i < json.Count; i++) {
				Guide g = new Guide ();
				g.jsonText = json;

				JsonValue values = json [i];

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_Username)) {
					string username = values [Constants.Guide_WebAPI_Key_Username];
					g.userName= username;
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_FirstName)) {
					string fName = values [Constants.Guide_WebAPI_Key_FirstName];

					g.fName = fName;
					g.guideId = values [Constants.Guide_WebAPI_Key_GuideId];

				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_LastName)) {
					string lName = values [Constants.Guide_WebAPI_Key_LastName];
					g.lName= lName;
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_Address1)) {
					g.address1 = values [Constants.Guide_WebAPI_Key_Address1];
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_Address2)) {
					g.address2 = values [Constants.Guide_WebAPI_Key_Address2];
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_Description)) {
					g.description = values [Constants.Guide_WebAPI_Key_Description];
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_Availability)) {
					g.availability = values [Constants.Guide_WebAPI_Key_Availability];
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_ProfileImageId)) {
					g.profileImageId = values [Constants.Guide_WebAPI_Key_ProfileImageId];
				} else {
					g.profileImageId = Constants.Uninitialized;
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_LanguageList)) {
					JsonValue temp = values [Constants.Guide_WebAPI_Key_LanguageList];
					for (int j=0; j<temp.Count;j++)
					{
						JsonValue l = temp[j];
						g.languageList.Add (l [Constants.Guide_WebAPI_Key_Language]);
						((SecondActivity)this.Activity).availableLanguages.Add (l [Constants.Guide_WebAPI_Key_Language]);
						//@todo get languageId too
					}
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_LocationList)) {
					JsonValue temp = values [Constants.Guide_WebAPI_Key_LocationList];
					for (int j=0; j<temp.Count;j++)
					{
						JsonValue l = temp[j];
						g.placesServedList.Add (l [Constants.Guide_WebAPI_Key_Location]);
						//@todo 
					}
				}

				if (values.ContainsKey (Constants.Guide_WebAPI_Key_ExpertiseList)) {
					JsonValue temp = values [Constants.Guide_WebAPI_Key_ExpertiseList];
					for (int j=0; j<temp.Count;j++)
					{
						JsonValue l = temp[j];
						g.expertise.Add (new Expertise() {expertise=l [Constants.Guide_WebAPI_Key_Expertise], expertiseId=l [Constants.Guide_WebAPI_Key_ExpertiseId]});
						//@todo 
					}
				}
				mGuideList.Add (g);
			}
		}

		public async Task<List<Guide>> loadGuideProfiles(string url)
		{
			
			CallAPI ca = new CallAPI();

			//List<Guide> myGuides = new List<Guide> ();

			/****** Uncomment when webservice is running *****/
			JsonValue json = await ca.getWebApiData(url, null);
			parseGuideProfiles(json);

			string imageUrl;
			foreach (Guide g in mGuideList) {

				if (g.profileImageId == Constants.Uninitialized) {
					g.profileImage = null;
				} else {
					imageUrl= Constants.DEBUG_BASE_URL + "/api/images/"+ g.profileImageId;

					Bitmap image = (Bitmap) await ca.getScaledImage (imageUrl);
					g.profileImage = image;
				}
			}

			//mGuideList = myGuides;

			mAdapter.NotifyDataSetChanged ();

			if (mGuideList.Count == 0) {
				noGuides.Visibility = ViewStates.Visible;
			} else {
				noGuides.Visibility = ViewStates.Gone;
			}

			return mGuideList;
//			mAdapter = new RecyclerAdapter (mGuideList, this.Activity);
	//		mRecyclerView.SetAdapter (mAdapter);
			//RecyclerView.ItemDecoration itemDecoration =new DividerItemDecoration(this, DividerItemDecoration.VERTICAL_LIST);
			//new DividerItemDecoration

		}

		void OnItemClick (object sender, int position)
		{
			int photoNum = position + 1;
			//Toast.MakeText(this, "This is photo number " + photoNum, ToastLength.Short).Show();
		}			
	}


	public class RecyclerAdapter: RecyclerView.Adapter
	{
		private List<Guide> mGuides;
		private Activity thisActivity;

		public RecyclerAdapter(List<Guide> guideList, Activity thisAct)
		{
			mGuides = guideList;
			thisActivity = thisAct;
		}

		public class MyView:RecyclerView.ViewHolder
		{
			public View mMainView { get; set; }
			public TextView mFName { get; set;} 
			public TextView mDescription { get; set;} 
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
			TextView description = row.FindViewById<TextView> (Resource.Id.description);
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


			MyView view = new MyView (row) { mFName = FName, mLocations=locations, mLanguages = languages, mPhoto=photo, mDescription=description, mAvailability=availability};
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
			if ( (mGuides [position].description!=null) && (mGuides [position].description.Length > Constants.MaxDescriptionLengthInCard) )
			{
				myHolder.mDescription.Text = mGuides [position].description.Substring (0, Constants.MaxDescriptionLengthInCard-1) + "...";
			} else 
			{
				myHolder.mDescription.Text = mGuides [position].description;
			}	

			// Set availability and colors
			Color AvailableNowColor = Color.SeaGreen;
			Color AvailableLaterColor = Color.DarkOrange;
			Color NotAvailableForChatColor = Color.Red;

			switch (mGuides [position].availability) 
			{

			case Constants.AvailableLaterValue:
				myHolder.mAvailability.Text = Constants.AvailableLaterString;
				myHolder.mAvailability.SetTextColor (AvailableLaterColor);
				break;
			
			case Constants.AvailableNowValue:
				myHolder.mAvailability.Text = Constants.AvailableNowString;
				myHolder.mAvailability.SetTextColor (AvailableNowColor);
				break;

			case Constants.NotAvailableForChatValue:
				myHolder.mAvailability.Text = Constants.NotAvailableForChatString;
				myHolder.mAvailability.SetTextColor (NotAvailableForChatColor);
				break;
			
			default:
				myHolder.mAvailability.Text = Constants.NoValue;
				myHolder.mAvailability.SetTextColor (NotAvailableForChatColor);
				break;
			}

			foreach (string l in mGuides[position].languageList) {
				languages += "• "+l+"\r\n" ;
			}				
			if (languages.Length > 0) {
				languages = languages.Remove (languages.Length - 2);
			}

			myHolder.mLanguages.Text = languages;

			foreach (string l in mGuides[position].placesServedList) {
				placesServed += "• "+l+"\r\n" ;
			}				
			//placesServed = placesServed.Remove (placesServed.Length - 2);
			myHolder.mLocations.Text = placesServed;

			//myHolder.mPhoto.SetImageResource (Resource.Drawable.placeholder_photo);
			if (mGuides [position].profileImage == null) {
				myHolder.mPhoto.SetImageResource (Resource.Drawable.placeholder_photo);
			} else {
				myHolder.mPhoto.SetImageBitmap (mGuides [position].profileImage);
			}				
				
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
		

	public class CallAPI
	{
		public CallAPI()
		{
		}

		public static int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
			// Raw height and width of image
			int height = options.OutHeight;
			int width = options.OutWidth;
			int inSampleSize = 1;

			if (height > reqHeight || width > reqWidth) {

				int halfHeight = height / 2;
				int halfWidth = width / 2;

				// Calculate the largest inSampleSize value that is a power of 2 and keeps both
				// height and width larger than the requested height and width.
				while ((halfHeight / inSampleSize) > reqHeight
					&& (halfWidth / inSampleSize) > reqWidth) {
					inSampleSize *= 2;
				}
			}

			return inSampleSize;
		}
			
		public async Task<Bitmap> getScaledImage (string imageUrl)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri (imageUrl));
			request.ContentType = "image/png";
			request.Method = "GET";
			BitmapFactory.Options options = new BitmapFactory.Options();

			// Send the request to the server and wait for the response:
			Bitmap bitMap=null;
			using (WebResponse response = await request.GetResponseAsync ()) {
				// Get a stream representation of the HTTP web response:
				using (System.IO.Stream stream = response.GetResponseStream ()) {
					// Use this stream to build a JSON document object:
					Rect outpadding = new Rect ();
					options.InJustDecodeBounds = true;
					bitMap = await Task.Run (() => BitmapFactory.DecodeStream (stream, null, options));
					//Bitmap bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream));

					options.InSampleSize = calculateInSampleSize (options, Constants.ProfileReqWidth, Constants.ProfileReqHeight);
				}
			}
			//@todo - reuse the stream!!
			request = (HttpWebRequest)HttpWebRequest.Create (new Uri (imageUrl));
			using (WebResponse response2= await request.GetResponseAsync ()) {
				using (System.IO.Stream stream = response2.GetResponseStream ()) {
					options.InJustDecodeBounds = false;

					bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream, null, options));
					//BitmapFactory.decodeResource(getResources(), R.id.myimage, options);
					int imageHeight = options.OutHeight;
					int imageWidth = options.OutWidth;
					String imageType = options.OutMimeType;

					//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

					// Return the bitmap:
					return bitMap;
				}
			}
		}
		public async Task<Bitmap> getImage (string imageUrl)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (imageUrl));
			request.ContentType = "image/png";
			request.Method = "GET";
			BitmapFactory.Options options = new BitmapFactory.Options();

			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync ())
			{
				// Get a stream representation of the HTTP web response:
				using (System.IO.Stream stream = response.GetResponseStream ())
				{
					// Use this stream to build a JSON document object:
					Rect outpadding = new Rect();
					//options.InJustDecodeBounds = true;
					//Bitmap bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream, null, options));
					Bitmap bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream));

					options.InSampleSize = calculateInSampleSize(options, Constants.ProfileReqWidth, Constants.ProfileReqHeight);
					//options.InJustDecodeBounds = false;
			//		bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream, null, options));
					//BitmapFactory.decodeResource(getResources(), R.id.myimage, options);
					int imageHeight = options.OutHeight;
					int imageWidth = options.OutWidth;
					String imageType = options.OutMimeType;

					//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

					// Return the bitmap:
					return bitMap;
				}
			}
		}

		public void PostWebApiData (string p_url, NameValueCollection parameters)
		{
			// Create an HTTP web request using the URL:
			WebClient client = new WebClient();
			Uri url = new Uri(p_url);	

			client.UploadValuesCompleted += Client_UploadValuesCompleted;
			client.UploadValuesAsync (url, parameters);
		}

		void Client_UploadValuesCompleted (object sender, UploadValuesCompletedEventArgs e)
		{
			
		}	

		public async Task<JsonValue> getWebApiData (string url, string accessToken)
		{
			// Create an HTTP web request using the URL:
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
			if (accessToken != null) {
				request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
			}

			request.ContentType = "application/json";
			request.Method = "GET";

			try
			{
				// Send the request to the server and wait for the response:
				using (WebResponse response = await request.GetResponseAsync ())
				{
					try 
					{
						// Get a stream representation of the HTTP web response:
						using (System.IO.Stream stream = response.GetResponseStream ())
						{
							// Use this stream to build a JSON document object:
							JsonValue jsonDoc = await Task.Run (() => JsonObject.Load (stream));
							//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

							// Return the JSON document:
							return jsonDoc;
						}						
					}
					catch (Exception e)
					{						
						return null;
					}
				}				
			}
			catch (Exception e) {				
				return null;
			}

		}			
	}
}

