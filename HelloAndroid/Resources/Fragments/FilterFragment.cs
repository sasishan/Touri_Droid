
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
	public class FilterFragment : Fragment
	{
		private static readonly int[] mExpertiseValues = new []
		{
			Resource.Drawable.bar48, Resource.Drawable.lunch4, Resource.Drawable.cup54,
			Resource.Drawable.camera, Resource.Drawable.hiking48, Resource.Drawable.museum37
		};

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.fragment_filter, container, false);
			Button apply = view.FindViewById<Button> (Resource.Id.applyFilter);

			apply.Click += (object IntentSender, EventArgs e) => {
				var newFragment = new GuideFragment ();
				//var ft = FragmentManager.BeginTransaction ();

				((SecondActivity)this.Activity).mGuideSearch.languageList.Clear();
				foreach (String l in ((SecondActivity)this.Activity).checkedLanguages)
				{
					((SecondActivity)this.Activity).mGuideSearch.languageList.Add(l);
				}

				FragmentTransaction transaction = FragmentManager.BeginTransaction();

				// Replace whatever is in the fragment_container view with this fragment,
				// and add the transaction to the back stack if needed
				transaction.Replace(Resource.Id.fragment_container, newFragment);
				//transaction.AddToBackStack(null);

				// Commit the transaction
				transaction.Commit();
			};

			SupportFunctions sf = new SupportFunctions ();

			// build out the expertises table
		//	TableLayout expertiseTable = (TableLayout)view.FindViewById (Resource.Id.table_Expertise);
		//	List<Expertise> expertises = sf.BuildExpertiseTable (view, expertiseTable, Resource.Layout.expertise_tablerow);
		//	expertiseTable.RequestLayout();

			//((SecondActivity)this.Activity).checkedLanguages.Clear ();
			TableLayout languagesTable = (TableLayout)view.FindViewById (Resource.Id.table_Languages);
			for (int i = 0; i < Constants.AvailableLanguages.Count; i++) {
				TableRow row = (TableRow)LayoutInflater.From (view.Context).Inflate (Resource.Layout.language_tablerow, null);
				CheckBox c = row.FindViewById<CheckBox> (Resource.Id.languageCheck);
				c.Text = Constants.AvailableLanguages [i].Item2;

				if (((SecondActivity)this.Activity).checkedLanguages.Contains (Constants.AvailableLanguages [i].Item2)) {
					c.Checked = true;
					//((SecondActivity)this.Activity).checkedLanguages.Add(c.Text);
				}

				c.Click += (object sender, EventArgs e) => {
					if (c.Checked==true)
					{
						((SecondActivity)this.Activity).checkedLanguages.Add(c.Text);
					}
					else
					{
						((SecondActivity)this.Activity).checkedLanguages.Remove(c.Text);
					}
				};
				languagesTable.AddView(row);
			}

			languagesTable.RequestLayout();

		/*	for (int i = 0; i < mExpertiseValues.Length; i=i+3) {
				TableRow row = (TableRow)LayoutInflater.From (view.Context).Inflate (Resource.Layout.expertise_tablerow, null);
				TableRow blankRow = (TableRow)LayoutInflater.From (view.Context).Inflate (Resource.Layout.blank_tablerow, null);
				TableRow blankRow2 = (TableRow)LayoutInflater.From (view.Context).Inflate (Resource.Layout.blank_tablerow, null);
				ImageButton b1 = row.FindViewById<ImageButton> (Resource.Id.exp1);
				ImageButton b2 = row.FindViewById<ImageButton> (Resource.Id.exp2);
				ImageButton b3 = row.FindViewById<ImageButton> (Resource.Id.exp3);

				b1.SetImageResource (mExpertiseValues[i]);
				b2.SetImageResource (mExpertiseValues[i+1]);
				b3.SetImageResource (mExpertiseValues[i+2]);

				b1.Click += (object sender, EventArgs e) => {
					if (!b1.Selected)
					{						
						b1.SetImageResource (Resource.Drawable.bar48_pressed);
						b1.Selected=true;
					}
					else
					{
						b1.SetImageResource (Resource.Drawable.bar48);
						b1.Selected=false;
					}
				};*/

			//	expertiseTable.AddView (blankRow);
			//	expertiseTable.AddView (row);
				//expertiseTable.AddView (blankRow2);
			//}


		//	GridView gridview = (GridView) view.FindViewById(Resource.Id.gridviewExpertise);
			//ArrayAdapter<String> adapter = new ArrayAdapter<String> (view.Context, Resource.Layout.Expertise_Grid);

			//gridview.SetAdapter(new ImageAdapter(this.Activity.BaseContext, mExpertiseValues,this.Activity));

//			gridview.setAdapter(new ImageAdapter(this));

			return view;
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			//return base.OnCreateView (inflater, container, savedInstanceState);
		}
			
	}

	public class ImageAdapter: BaseAdapter {
		private Context context;
		private String[] mobileValues;
		Activity activity;


		public ImageAdapter(Context context, String[] mobileValues, Activity _activity) {
			this.context = context;
			this.mobileValues = mobileValues;
			activity = _activity;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater) context.GetSystemService(Context.LayoutInflaterService);

			View gridView;

			if (convertView == null) {

				gridView = new View(context);

				// get layout from mobile.xml
				gridView = activity.LayoutInflater.Inflate (Resource.Layout.Expertise_Grid, parent, false);//inflater.Inflate(Resource.Id., null);

				// set value into textview
				TextView textView = (TextView) gridView.FindViewById(Resource.Id.gvExpertise);

			//	textView.SetText(mobileValues[position]);

				// set image based on selected text
				ImageView imageView = (ImageView) gridView.FindViewById(Resource.Id.gvExpertiseImage);
				imageView.SetImageResource(Resource.Drawable.abc_btn_radio_material);
			/*	String mobile = mobileValues[position];

				if (mobile.equals("Windows")) {
					imageView.setImageResource(R.drawable.windows_logo);
				} else if (mobile.equals("iOS")) {
					imageView.setImageResource(R.drawable.ios_logo);
				} else if (mobile.equals("Blackberry")) {
					imageView.setImageResource(R.drawable.blackberry_logo);
				} else {
					imageView.setImageResource(R.drawable.android_logo);
				}*/

			} else {
				gridView = (View) convertView;
			}

			return gridView;
		}


		public override int Count {
			get { return mobileValues.Length;}
		}

		public override Java.Lang.Object GetItem(int position) {
			return null;
		}

		public override long GetItemId(int position) {
			return 0;
		}

	}
}

