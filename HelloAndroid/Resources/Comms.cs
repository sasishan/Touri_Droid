using System;
using System.Json;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Util;

namespace TouriDroid
{
	public class Comms
	{
		private const string TAG = "Comms";
		public Comms()
		{
		}

		public JsonValue PostFile (string p_url, string filename, string accessToken)
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
			client.UploadFileAsync (url, filename);


			return null;
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
			//@todo use UploadValuesAsync?
			byte[] result = client.UploadValues (url, parameters);

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
						if (e.InnerException!=null)
						{
							Log.Debug(TAG, e.InnerException.ToString());
						}
						else
						{
							Log.Debug(TAG, "Error in comms");
						}
						return null;
					}
				}				
			}
			catch (Exception e) {
				if (e.InnerException != null) {
					Log.Debug (TAG, e.InnerException.ToString ());
				} else if (e.Message != null) {
					Log.Debug (TAG, e.Message);
				}
				else {
					Log.Debug(TAG, "Error in comms");
				}				
				return null;
			}

		}			
	}

}

