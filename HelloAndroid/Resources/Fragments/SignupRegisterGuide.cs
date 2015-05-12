
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
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Security;
using System.Collections.Specialized;
using System.Json;

namespace TouriDroid
{
	public class SignupRegisterGuide : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public async void RegisterGuide(View view)
		{
			EditText username =view.FindViewById<EditText> (Resource.Id.username);
			EditText password =view.FindViewById<EditText> (Resource.Id.password);
			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			RegisterService rs = new RegisterService();
			bool registered = await rs.Register(username.Text, password.Text, password.Text);

			if (registered) 
			{
				LoginService ls = new LoginService ();
				String token = await ls.Login (username.Text, password.Text);

				//add a guide record
				Guide newGuide = ((SignUpAsGuideActivity)Activity).newGuide;
				NameValueCollection parameters = new NameValueCollection ();
				parameters.Add (Constants.Guide_WebAPI_Key_Username, username.Text);
				parameters.Add (Constants.Guide_WebAPI_Key_FirstName, newGuide.fName);
				parameters.Add (Constants.Guide_WebAPI_Key_LastName, newGuide.lName);
				parameters.Add (Constants.Guide_WebAPI_Key_ProfileImageId, Constants.DefaultProfileId.ToString());

				JsonValue jsonResponse = PostDataSync (Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile/*"/api/guides"*/, 
					parameters, token);
				
				string url;
				if (jsonResponse.ContainsKey (Constants.Guide_WebAPI_Key_GuideId)) 
				{
					newGuide.guideId = jsonResponse [Constants.Guide_WebAPI_Key_GuideId];
					parameters.Clear ();
					url = String.Format (Constants.URL_AddGuideLocation, newGuide.guideId);

					foreach (string l in newGuide.placesServedList) {
						parameters.Add ("location", l);

						jsonResponse = PostDataSync (Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile+
							url, parameters, token);
						parameters.Clear ();
					}

					url = String.Format (Constants.URL_AddGuideExpertise, newGuide.guideId);
					foreach (Expertise expt in newGuide.expertise) 
					{
						parameters.Add (Constants.Guide_WebAPI_Key_ExpertiseId, expt.expertiseId.ToString());
				
						jsonResponse = PostDataSync (Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile +
							url, parameters, token);
						parameters.Clear ();
					}

					url = String.Format (Constants.URL_AddGuideLanguage, newGuide.guideId);
					foreach (GuideLanguage lang in newGuide.languages) 
					{
						parameters.Add (Constants.Guide_WebAPI_Key_Language, lang.language);
						parameters.Add (Constants.Guide_WebAPI_Key_LanguageId, lang.languageId.ToString());

						jsonResponse = PostDataSync (Constants.DEBUG_BASE_URL + Constants.URL_MyGuideProfile +
							url, parameters, token);
						parameters.Clear ();
					}
				}

				Toast.MakeText (view.Context, "You are now registered as a guide. Please sign in", ToastLength.Long).Show ();

				Intent i = new Intent(view.Context, typeof(LoginOrSignupActivity));
				// Closing all the Activities
				i.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
				this.StartActivity (i);
				//LoginService ls = new LoginService ();

				//string token = await ls.Login (username.Text, password.Text);

			} else {
				Toast.MakeText (view.Context, "Error registering your profile", ToastLength.Long).Show ();
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

		void Client_UploadValuesCompleted (object sender, UploadValuesCompletedEventArgs e)
		{
			Activity.RunOnUiThread (() => {
				string s = e.Result.ToString();
				s.ToString();
					
		//		JsonValue jsonDoc =  JsonObject.Load (e.Result);
		//		jsonDoc.ToString();
			});
		}	
			
		public async void RegisterTraveller(View view)
		{
			EditText username =view.FindViewById<EditText> (Resource.Id.username);
			EditText password =view.FindViewById<EditText> (Resource.Id.password);
			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			RegisterService rs = new RegisterService();
			bool registered = await rs.Register(username.Text, password.Text, password.Text);

			if (registered) {
				Toast.MakeText (view.Context, "You are now registered. Please sign in", ToastLength.Long).Show ();

				Intent i = new Intent(view.Context, typeof(LoginOrSignupActivity));
				// Closing all the Activities
				i.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
				this.StartActivity (i);

			} else {
				Toast.MakeText (view.Context, "Error registering your profile", ToastLength.Long).Show ();
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.SignupRegister, container, false);

			EditText username =view.FindViewById<EditText> (Resource.Id.username);
			EditText password =view.FindViewById<EditText> (Resource.Id.password);
			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			Boolean becomeAGuide = false;

			//this fragment is called by two different activities - SignUpAsGuideActivity and LoginOrSignupActivity
			//reason being that become a guide requires multiple steps
			if (Activity.GetType() == typeof(SignUpAsGuideActivity)) {
				becomeAGuide = true;
			}

			next.Click += (object IntentSender, EventArgs e) => {

				if (!username.Text.Equals("") && !password.Text.Equals("") )
				{					
					if (Constants.isValidEmail(username.Text))
					{
						if (becomeAGuide)
						{
							RegisterGuide(view);
						}
						else
						{
							RegisterTraveller(view);
						}
					}
					else
					{
						Toast.MakeText (view.Context, "Enter a valid email address", ToastLength.Long).Show();
					}						
				}
				else
				{
					Toast.MakeText (view.Context, "Please enter a valid email address and password", ToastLength.Long).Show();
				}
			};

			return view;
		}			
	}

	public class RegisterService
	{
		public async Task<bool> Register(string username, string password, string confirmPassword)
		{
			RegisterModel model = new RegisterModel
			{
				ConfirmPassword = password,
				Password = password,
				Email = username
			};

			HttpWebRequest request = new HttpWebRequest(new Uri(String.Format("{0}/api/Account/Register", Constants.DEBUG_BASE_URL)));
			request.Method = "POST";
			request.ContentType = "application/json";
			request.Accept = "application/json";
			string json = JsonConvert.SerializeObject(model);
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			using(Stream stream = await request.GetRequestStreamAsync())
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			try
			{
				await request.GetResponseAsync();
				return true;
			}
			catch (Exception ex)
			{
				//Toast.MakeText (, "Error ", ToastLength.Long).Show();
				Log.Debug("Network error", ex.InnerException.ToString());
				return false;
			}
		}
	}

	class LoginService
	{
		public async Task<string> Login(string username, string password)
		{
			HttpWebRequest request = new HttpWebRequest(new Uri(String.Format("{0}/Token", Constants.DEBUG_BASE_URL)));
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			string postString = String.Format("username={0}&password={1}&grant_type=password", HttpUtility.HtmlEncode(username), HttpUtility.HtmlEncode(password));
			byte[] bytes = Encoding.UTF8.GetBytes(postString);
			using (Stream requestStream = await request.GetRequestStreamAsync())
			{
				if (requestStream != null) {
					requestStream.Write(bytes, 0, bytes.Length);
				}
				requestStream.Close ();
			}

			try
			{
				HttpWebResponse httpResponse =  (HttpWebResponse)(await request.GetResponseAsync());
				string json;
				using (Stream responseStream = httpResponse.GetResponseStream())
				{
					json = new StreamReader(responseStream).ReadToEnd();
				}
				TokenResponseModel tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(json);
				return tokenResponse.AccessToken;
			}
			catch (Exception ex)
			{
				//Log.Debug("Network error", ex.InnerException.ToString());
				return null;
			}
		}
	}

	class ValuesService
	{
		public async Task<List<TokenResponseModel>> GetValues(string accessToken)
		{
			HttpWebRequest request = new HttpWebRequest(new Uri(String.Format("{0}api/Values", Constants.DEBUG_BASE_URL)));
			request.Method = "GET";
			request.Accept = "application/json";
			request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

			try
			{
				HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
				string json;
				using (Stream responseStream = httpResponse.GetResponseStream())
				{
					json = new StreamReader(responseStream).ReadToEnd();
				}
				List<TokenResponseModel> values = (List<TokenResponseModel>) JsonConvert.DeserializeObject(json);
				return values;
			}
			catch (Exception ex)
			{
				throw new SecurityException("Bad credentials", ex);
			}
		}
	}



	class TokenResponseModel
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonProperty("userName")]
		public string Username { get; set; }

		[JsonProperty(".issued")]
		public string IssuedAt { get; set; }

		[JsonProperty(".expires")]
		public string ExpiresAt { get; set; }
	}
}

