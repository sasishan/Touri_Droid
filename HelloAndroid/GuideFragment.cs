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

namespace HelloAndroid
{
	public class GuideFragment : Fragment
	{
		
		private RecyclerView mRecyclerView;
		private RecyclerView.LayoutManager mLayoutManager;
		private RecyclerView.Adapter mAdapter;
		protected List<Guide> mGuideList = new List<Guide> ();
		public event EventHandler<int> ItemClick;


		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
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
			//CallAPI ca = new CallAPI();

			//List<Guide> myGuides = new List<Guide> ();

			/****** Uncomment when webservice is running *****/
			//JsonValue json = await ca.getWebApiData(url);
			//parseGuideProfiles(json);

		//	mAdapter.NotifyDataSetChanged ();

		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_guide, container, false);
			//((MainActivity)this.Activity).setCurrentFragment (Constants.GuideFragment);

			// Get our button from the layout resource,
			// and attach an event to it
			//Button button = view.FindViewById<Button> (Resource.Id.nextButton);
			//TextView text =  view.FindViewById<TextView> (Resource.Id.guideFirstName);
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
			mGuideList.Clear ();

			for (int i = 0; i < json.Count; i++) {
				Guide g = new Guide ();
				g.jsonText = json;

				JsonValue values = json [i];
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
			JsonValue json = await ca.getWebApiData(url);
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

		private List<Guide> buildTestData()
		{
			List<Guide> myGuides = new List<Guide> ();

			myGuides.Add (new Guide(){guideId = 1, fName = "John", lName = "Lennon"});
			myGuides [0].languageList.Add ("English");
			myGuides [0].languageList.Add ("French");
			myGuides [0].placesServedList.Add ("Toronto, Ontario, Canada");
			myGuides [0].placesServedList.Add ("Scarborough, Ontario, Canada");
			myGuides [0].placesServedList.Add ("Pickering, Ontario, Canada");
			myGuides [0].availability = Constants.AvailableNowValue;
			myGuides [0].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";			
			myGuides [0].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [0].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Museums" });
			myGuides [0].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Restaurants" });

			myGuides.Add (new Guide(){guideId = 2, fName = "Paul", lName = "McCartney"});
			myGuides [1].languageList.Add ("English");
			myGuides [1].placesServedList.Add ("Vancouver, British Columbia, Canada");
			myGuides [1].availability = Constants.AvailableNowValue;
			myGuides.Add (new Guide(){guideId = 3, fName = "Ringo", lName = "Starr"});
			myGuides [1].availability = Constants.AvailableNowValue;
			myGuides [1].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Landmarks" });
			myGuides [1].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Museums" });
			myGuides [1].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";

			myGuides [2].languageList.Add ("Italian");
			myGuides [2].languageList.Add ("French");
			myGuides [2].placesServedList.Add ("Colombo, Sri Lanka");
			myGuides [2].placesServedList.Add ("Galle, Sri Lanka");
			myGuides [2].availability = Constants.AvailableNowValue;
			myGuides [2].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [2].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Museums" });
			myGuides [2].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Restaurants" });
			myGuides [2].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Coffee Shops" });
			myGuides [2].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Outdoors" });
			myGuides [2].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";

			myGuides.Add (new Guide(){guideId = 4, fName = "Peter", lName = "Piper"});
			myGuides [3].languageList.Add ("English");
			myGuides [3].languageList.Add ("Macedonian");
			myGuides [3].languageList.Add ("Croatian");
			myGuides [3].placesServedList.Add ("Chennai, Tamil Nadu, India");
			myGuides [3].placesServedList.Add ("Mumbai, India");
			myGuides [3].availability = Constants.AvailableLaterValue;
			myGuides [3].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [3].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Museums" });
			myGuides [3].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Restaurants" });
			myGuides [3].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Coffee Shops" });
			myGuides [3].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Outdoors" });
			myGuides [3].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 5, fName = "Malcolm", lName = "X"});
			myGuides [4].languageList.Add ("English");
			myGuides [4].placesServedList.Add ("Mumbai, India");
			myGuides [4].availability = Constants.AvailableLaterValue;
			myGuides [4].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [4].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Museums" });
			myGuides [4].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 6, fName = "Mohammed", lName = "Ali"});
			myGuides [5].languageList.Add ("English");
			myGuides [5].languageList.Add ("French");
			myGuides [5].placesServedList.Add ("Mumbai, India");
			myGuides [5].availability = Constants.AvailableLaterValue;
			myGuides [5].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Restaurants" });
			myGuides [5].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Outdoors" });
			myGuides [5].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 7, fName = "Kamal", lName = "Hassan"});
			myGuides [6].languageList.Add ("English");
			myGuides [6].languageList.Add ("German");
			myGuides [6].languageList.Add ("French");
			myGuides [6].languageList.Add ("Italian");
			myGuides [6].languageList.Add ("Macedonian");
			myGuides [6].placesServedList.Add ("Istanbul, Turkey");
			myGuides [6].availability = Constants.NotAvailableForChatValue;
			myGuides [6].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Restaurants" });
			myGuides [6].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Outdoors" });
			myGuides [6].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 8, fName = "John", lName = "Smith"});
			myGuides [7].languageList.Add ("Hindi");
			myGuides [7].languageList.Add ("Tamil");
			myGuides [7].placesServedList.Add ("Chennai, Tamil Nadu, India");
			myGuides [7].availability =  Constants.NotAvailableForChatValue;;
			myGuides [7].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Restaurants" });
			myGuides [7].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Nightlife" });
			myGuides [7].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";

			myGuides.Add (new Guide(){guideId = 9, fName = "Rajni", lName = "Kanth"});
			myGuides [8].languageList.Add ("Tamil");
			myGuides [8].placesServedList.Add ("Chennai, Tamil Nadu, India");
			myGuides [8].availability =  Constants.AvailableNowValue;;
			myGuides [8].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [8].expertise.Add (new Expertise (){ expertiseId = 1, expertise = "Museums" });
			myGuides [8].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Restaurants" });
			myGuides [8].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Coffee Shops" });
			myGuides [8].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Outdoors" });
			myGuides [8].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 9, fName = "Igor", lName = "Siljanovski"});
			myGuides [9].languageList.Add ("Punjabi");
			myGuides [9].placesServedList.Add ("Athens, Greece");
			myGuides [9].availability =  Constants.AvailableLaterValue;
			myGuides [9].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [9].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Coffee Shops" });
			myGuides [9].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Outdoors" });
			myGuides [9].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 9, fName = "Bojan", lName = "Siljanovski"});
			myGuides [10].languageList.Add ("English");
			myGuides [10].placesServedList.Add ("Toronto, Ontario, Canada");
			myGuides [10].placesServedList.Add ("Scarborough, Ontario, Canada");
			myGuides [10].placesServedList.Add ("Pickering, Ontario, Canada");
			myGuides [10].availability = Constants.AvailableLaterValue;
			myGuides [10].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [10].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Coffee Shops" });
			myGuides [10].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Outdoors" });
			myGuides [10].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 9, fName = "Ryhaan", lName = "Shan"});
			myGuides [11].languageList.Add ("English");
			myGuides [11].languageList.Add ("Macedonian");
			myGuides [11].placesServedList.Add ("Scarborough, Ontario, Canada");
			myGuides [11].placesServedList.Add ("Pickering, Ontario, Canada");
			myGuides [11].availability =  Constants.NotAvailableForChatValue;
			myGuides [11].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [11].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Coffee Shops" });
			myGuides [11].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Outdoors" });
			myGuides [11].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";
			myGuides.Add (new Guide(){guideId = 9, fName = "Sasi", lName = "Shan"});
			myGuides [12].languageList.Add ("English");
			myGuides [12].placesServedList.Add ("Vancouver, British Columbia, Canada");
			myGuides [12].availability =  Constants.AvailableNowValue;
			myGuides [12].expertise.Add (new Expertise (){ expertiseId = 0, expertise = "Nightlife" });
			myGuides [12].expertise.Add (new Expertise (){ expertiseId = 2, expertise = "Outdoors" });		
			myGuides [12].description = "My name is Naga Subrahmanyam and my friends call me Subbu. I am an experienced, " +
				"professional English-speaking licensed tour guide of Hyderabad. I have been working since 2002 for the" +
				"tourists from different countries of the world. I was trained and licensed by Ministry of Tourism, Government of India." +
				" Tourism is my lifetime passion and I love working with people. I am providing private tours so that you will discover" +
				"and learn more about our beautiful city Hyderabad, its legends and anecdotes, and the way of life. I studied my graduation" +
				" and post graduation in tourism management and experienced in different kinds of tours such as heritage / historical / architectural" +
				" tours, religious (temple / faith) tours, Forest / Nature / Wildlife Tours, Buddhist tours, Offbeat coastal tours, walking tours and" +
				" textile tours.";

			return myGuides;
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

			MyView view = new MyView (row) { mFName = FName, mLocations=locations, mLanguages = languages, mPhoto=photo, mDescription=description, mAvailability=availability};
			return view;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			MyView myHolder = holder as MyView;

			string languages = "Languages I speak: ";
			string placesServed = "Locations I serve: ";
			// Set name
			myHolder.mFName.Text = mGuides[position].fName + " " + mGuides[position].lName;

			//Set description.. for summary only partial description is shown
			//@todo should their be a summary line for guides like Twitter?
			if (mGuides [position].description.Length > Constants.MaxDescriptionLengthInCard) 
			{
				myHolder.mDescription.Text = mGuides [position].description.Substring (0, Constants.MaxDescriptionLengthInCard-1) + "...";
			} else 
			{
				myHolder.mDescription.Text = mGuides [position].description;
			}	

			// Set availability and colors
			Color AvailableNowColor = Color.Green;
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
				languages += l + "; ";
			}				
			languages = languages.Remove (languages.Length - 2);
			myHolder.mLanguages.Text = languages;

			foreach (string l in mGuides[position].placesServedList) {
				placesServed += l + "; ";
			}				
			placesServed = placesServed.Remove (placesServed.Length - 2);
			myHolder.mLocations.Text = placesServed;

			//myHolder.mPhoto.SetImageResource (Resource.Drawable.placeholder_photo);
			if (mGuides [position].profileImage == null) {
				myHolder.mPhoto.SetImageResource (Resource.Drawable.placeholder_photo);
			} else {
				myHolder.mPhoto.SetImageBitmap (mGuides [position].profileImage);
			}



			myHolder.ItemView.Click += (sender, e) => {
				int itemPosition = myHolder.Position;

				var gprofileActivity = new Intent (thisActivity, typeof(GuideProfileActivity));
				gprofileActivity.PutExtra ("GuideId", mGuides[itemPosition].guideId.ToString());
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
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (imageUrl));
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

		public async Task<JsonValue> getWebApiData (string url)
		{
			// Create an HTTP web request using the URL:
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
			request.ContentType = "application/json";
			request.Method = "GET";

			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync ())
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
		}			
	}
}

