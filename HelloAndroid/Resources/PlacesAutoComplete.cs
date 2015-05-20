using System;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Java.Net;
using Java.IO;
using Android.Util;
using Org.Json;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using System.Linq;
using System.Text;

namespace TouriDroid
{
	public class PlacesAutoCompleteAdapter:ArrayAdapter,IFilterable
	{
		public List<string> resultList;
		protected Filter filter;

		public PlacesAutoCompleteAdapter(Context context, int textViewResourceId):base(context, textViewResourceId) {
			filter = new SuggestionsFilter (this);
			resultList = new List<string> ();

		}

		public override int Count {
			get {
				return resultList.Count;
			} 
		}

		public override Java.Lang.Object GetItem(int position) {
			return resultList [position];
		}

		public override Filter Filter {
			get {
				return filter;
			}
		}					
	}

	public class SuggestionsFilter : Filter
	{
		PlacesAutoCompleteAdapter a;
		private static string API_KEY = "AIzaSyDPRsZJ3iQcO8PdUU1yCjFAKA7etzg7PPM";

		public SuggestionsFilter (PlacesAutoCompleteAdapter adapter) : base() {
			a = adapter;
		}

		protected override Filter.FilterResults PerformFiltering (Java.Lang.ICharSequence constraint)
		{
			FilterResults filterResults  = new FilterResults();

			if (constraint != null) {
				a.resultList = autocomplete(constraint.ToString());

				Java.Lang.Object[] matchObjects = new Java.Lang.Object[a.resultList.Count];
				for (int i = 0; i < a.resultList.Count; i++) {
					matchObjects[i] = new Java.Lang.String(a.resultList[i]);
				}

				// Assign the data to the FilterResults
				filterResults.Values = matchObjects;
				filterResults.Count = matchObjects.Count();

			}
			return filterResults;
		}

		protected override void PublishResults (Java.Lang.ICharSequence constraint, Filter.FilterResults results)
		{
			a.NotifyDataSetChanged();
		}



		protected List<string> autocomplete(string input) 
		{
			string TAG = "SuggestionsFilter";
			List<string> resultList = null;

			HttpURLConnection conn = null;
			StringBuilder jsonResults = new StringBuilder();

			try {
				StringBuilder sb = new StringBuilder(Constants.PLACES_API_BASE + Constants.TYPE_AUTOCOMPLETE + Constants.OUT_JSON);
				sb.Append("?key=" + API_KEY);
				//    sb.Append("&components=country:uk");
				sb.Append("&input=" + URLEncoder.Encode(input, "utf8"));

				URL url = new URL(sb.ToString());
				conn = (HttpURLConnection) url.OpenConnection();
				InputStreamReader inS;
				inS = new InputStreamReader(conn.InputStream);


				// Load the results into a StringBuilder
				int read;
				char[] buff = new char[1024];
				while ((read = inS.Read(buff)) != -1) {
					jsonResults.Append(buff, 0, read);
				}
			} catch (MalformedURLException e) {
				Log.Error(TAG, "Error processing Places API URL", e);
				return resultList;
			} catch (System.IO.IOException e) {
				Log.Error(TAG, "Error connecting to Places API", e);
				return resultList;
			} finally {
				if (conn != null) {
					conn.Disconnect();
				}
			}

			try {
				// Create a JSON object hierarchy from the results
				JSONObject jsonObj = new JSONObject(jsonResults.ToString());
				JSONArray predsJsonArray = jsonObj.GetJSONArray("predictions");

				// Extract the Place descriptions from the results
				resultList = new List<string>();
				for (int i = 0; i < predsJsonArray.Length(); i++) {
					resultList.Add(predsJsonArray.GetJSONObject(i).GetString("description"));
				}
			} catch (JSONException e) {
				Log.Error(TAG, "Cannot process JSON results", e);
			}

			return resultList;
		}
	}
}

