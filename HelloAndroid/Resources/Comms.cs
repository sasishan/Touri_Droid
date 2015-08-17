using System;
using System.Json;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Util;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Client.Methods;
using Org.Apache.Http.Entity;
using Org.Apache.Http;
using Org.Apache.Http.Util;
using Org.Json;
using Org.Apache.Http.Message;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace TouriDroid
{
	public class Comms
	{
		public Comms()
		{
		}

		public string ScaleAndUploadPic(string p_url, string filename, string accessToken, int targetW, int targetH) {

			// Get the dimensions of the bitmap
			BitmapFactory.Options bmOptions = new BitmapFactory.Options();

			bmOptions.InJustDecodeBounds = true;
			BitmapFactory.DecodeFile(filename, bmOptions);
			int photoW = bmOptions.OutWidth;
			int photoH = bmOptions.OutHeight;

			// Determine how much to scale down the image
			int scaleFactor = Math.Min(photoW/targetW, photoH/targetH);

			// Decode the image file into a Bitmap sized to fill the View
			bmOptions.InJustDecodeBounds = false;
			bmOptions.InSampleSize = scaleFactor;
			bmOptions.InPurgeable = true;

			Bitmap bitmap = BitmapFactory.DecodeFile(filename, bmOptions);

			byte[] bitmapData;
			byte[] encodedBitmapData;
			using (var stream = new MemoryStream())
			{
				try{
					bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
					bitmapData = stream.ToArray();
					encodedBitmapData = Base64.Encode( bitmapData, Base64Flags.Default);
				}
				catch (Exception e) {
					Log.Debug (Constants.TOURI_TAG, "Error converting image");
					return null;
				}
			}


			WebClient client = new WebClient();

			Uri url = new Uri(p_url);

			if (accessToken != null) {
				client.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
			}

			client.UploadValuesCompleted += Client_UploadValuesCompleted;
			//@todo use UploadValuesAsync?
			//byte[] result = 
			//client.UploadFileAsync (url, filename);
			try
			{
				Byte[] bytes = client.UploadData(url, encodedBitmapData);
				if (bytes != null) {
					return (Encoding.ASCII.GetString (bytes));
				} else {
					return null;
				}
			}
			catch (Exception e) {
				if (e.Message != null) {
					Log.Debug (Constants.TOURI_TAG, "Error PostFile" + e.Message);
				} else {
					Log.Debug (Constants.TOURI_TAG, "Error PostFile");
				}
				return null;
			}
		}


		public string PostFile (string p_url, string filename, string accessToken)
		{
			// Create an HTTP web request using the URL:
			WebClient client = new WebClient();

			Uri url = new Uri(p_url);

			if (accessToken != null) {
				client.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
			}

			client.UploadValuesCompleted += Client_UploadValuesCompleted;
			//@todo use UploadValuesAsync?
			//byte[] result = 
			//client.UploadFileAsync (url, filename);
			try
			{
				Byte[] bytes = client.UploadFile (url, filename);
				if (bytes != null) {
					return (Encoding.ASCII.GetString (bytes));
				} else {
					return null;
				}
			}
			catch (WebException wex) {
				if (wex.Response != null) {
					using (var errorResponse = (HttpWebResponse)wex.Response) {
						using (var reader = new StreamReader(errorResponse.GetResponseStream())) {
							string error = reader.ReadToEnd();
							Log.Debug (Constants.TOURI_TAG, "Error PostFile" + error);
						}
					}
				}
				return null;
			}
			catch (Exception e) {
				if (e.Message != null) {
					Log.Debug (Constants.TOURI_TAG, "Error PostFile" + e.Message);
				} else {
					Log.Debug (Constants.TOURI_TAG, "Error PostFile");
				}
				return null;
			}
		}

		public JsonValue PostDataSync (string p_url, NameValueCollection parameters, string accessToken)
		{
			// Create an HTTP web request using the URL:
			WebClient client = new WebClient();

			Uri url = new Uri(p_url);

			if (accessToken != null) {
				client.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
			}

			client.UploadValuesCompleted += Client_UploadValuesCompleted;
			byte[] result;
			//@todo use UploadValuesAsync?
			try 
			{
				result = client.UploadValues (url, parameters);
			}
			catch (Exception e) {
				if (e.Message != null) {
					Log.Debug (Constants.TOURI_TAG, "Error PostDataSync" + e.Message);
				} else {
					Log.Debug (Constants.TOURI_TAG, "Error PostDataSync");
				}

				return null;
			}

			string s;
			JsonValue json = "";
			if (result != null) {
				s = Encoding.UTF8.GetString (result);
				json = JsonObject.Parse (s);
			}

			return json;
			//			JsonValue json = JsonObject.Parse (s);

			//		return json;
		}

		public async Task<JsonValue> PostDataSyncUsingUri (string p_url, string data, string accessToken)
		{
			// Create an HTTP web request using the URL:
			DefaultHttpClient httpclient = new DefaultHttpClient();

			//p_url
			HttpPost httppostreq = new HttpPost(p_url);
			if (accessToken != null) {
				httppostreq.SetHeader("Authorization", String.Format("Bearer {0}", accessToken));
			}
			StringEntity se = new StringEntity(data);
			se.ContentType = new BasicHeader("Content-Type", "application/x-www-form-urlencoded");
			se.ContentEncoding  = new BasicHeader ("Content-Encoding", "application/x-www-form-urlencoded");
			httppostreq.Entity=se;

			//@todo use UploadValuesAsync?\
			IHttpResponse httpresponse;
			try 
			{
				httpresponse = await Task.Run(() => httpclient.Execute(httppostreq));
			}
			catch (Exception e) {
				if (e.Message != null) {
					Log.Debug (Constants.TOURI_TAG, "Error PostDataSyncUsingUri" + e.Message);
				} else {
					Log.Debug (Constants.TOURI_TAG, "Error PostDataSyncUsingUri");
				}
				return null;
			}

			String responseText = null; 
			try 
			{ 
				responseText = EntityUtils.ToString(httpresponse.Entity); 
			} 
			catch (ParseException e) 
			{ 
				if (e.Message != null) {
					Log.Debug (Constants.TOURI_TAG, "Error PostDataSyncUsingUri" + e.Message);
				} else {
					Log.Debug (Constants.TOURI_TAG, "Error PostDataSyncUsingUri");
				}
			}

			return responseText;
		}

		private static int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
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

		public async Task<Bitmap> getScaledImage (string imageUrl, int scaledWidth, int scaledHeight)
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

					options.InSampleSize = calculateInSampleSize (options, scaledWidth, scaledHeight);
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

			try {
				WebResponse response = await request.GetResponseAsync ();
				System.IO.Stream stream = response.GetResponseStream ();

				//options.InJustDecodeBounds = true;
				//Bitmap bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream, null, options));
				Bitmap bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream));

//				options.InSampleSize = calculateInSampleSize(options, Constants.ProfileReqWidth, Constants.ProfileReqHeight);

				return bitMap;
			}
			catch (Exception e) {
				return null;
			}

			// Send the request to the server and wait for the response:
//			using (WebResponse response = await request.GetResponseAsync ())
//			{
				// Get a stream representation of the HTTP web response:
//				using (System.IO.Stream stream = response.GetResponseStream ())
//				{
					// Use this stream to build a JSON document object:
//					Rect outpadding = new Rect();
					//options.InJustDecodeBounds = true;
					//Bitmap bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream, null, options));
//					Bitmap bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream));

//					options.InSampleSize = calculateInSampleSize(options, Constants.ProfileReqWidth, Constants.ProfileReqHeight);
					//options.InJustDecodeBounds = false;
					//		bitMap =  await Task.Run (() => BitmapFactory.DecodeStream(stream, null, options));
					//BitmapFactory.decodeResource(getResources(), R.id.myimage, options);
//					int imageHeight = options.OutHeight;
//					int imageWidth = options.OutWidth;
//					String imageType = options.OutMimeType;

					//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

					// Return the bitmap:
//					return bitMap;
//				}
			//}
		}

		public void PostWebApiData (string p_url, NameValueCollection parameters)
		{
			// Create an HTTP web request using the URL:
			WebClient client = new WebClient();
			Uri url = new Uri(p_url);	

			client.UploadValuesCompleted += Client_UploadValuesCompleted;

			try
			{
				client.UploadValuesAsync (url, parameters);
			}
			catch (Exception e) {
				if (e.Message != null) {
					Log.Debug (Constants.TOURI_TAG, "Error PostWebApiData" + e.Message);
				} else {
					Log.Debug (Constants.TOURI_TAG, "Error PostWebApiData");
				}
			}
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
						if (e.Message!=null)
						{
							Log.Debug(Constants.TOURI_TAG, e.Message);
						}
						else
						{
							Log.Debug(Constants.TOURI_TAG, "Error in comms");
						}
						return null;
					}
				}				
			}
			catch (Exception e) {
				if (e.InnerException != null) {
					Log.Debug (Constants.TOURI_TAG, e.InnerException.ToString ());
				} else if (e.Message != null) {
					Log.Debug (Constants.TOURI_TAG, e.Message);
				}
				else {
					Log.Debug(Constants.TOURI_TAG, "Error in comms");
				}				
				return null;
			}

		}			
	}

}

