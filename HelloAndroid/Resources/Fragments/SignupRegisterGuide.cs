
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
		int guideId=-1;
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public async void RegisterGuide(View view)
		{
			EditText username =view.FindViewById<EditText> (Resource.Id.username);
			EditText password =view.FindViewById<EditText> (Resource.Id.password);
			Button next = view.FindViewById<Button> (Resource.Id.buttonNext);

			RegisterService rs = new RegisterService();
			bool registered = await rs.Register(username.Text, password.Text, password.Text);

			if (registered) {

				NameValueCollection parameters = new NameValueCollection ();
				parameters.Add ("username", username.Text);
				parameters.Add ("profileImage", "1020");
				PostDataSync (Constants.DEBUG_BASE_URL + "/api/guides", parameters);

	/*			if (jsonResponse.ContainsKey ("GuideId")) {
					((SignUpAsGuideActivity)Activity).newGuide.guideId = jsonResponse ["GuideId"];
				}
				*/
				Toast.MakeText (view.Context, "You are now registered as a guide", ToastLength.Long).Show ();

				LoginService ls = new LoginService ();

				string token = await ls.Login (username.Text, password.Text);

			} else {
				Toast.MakeText (view.Context, "Error registering your profile", ToastLength.Long).Show ();
			}
		}

		public void PostDataSync (string p_url, NameValueCollection parameters)
		{
			// Create an HTTP web request using the URL:
			WebClient client = new WebClient();

			Uri url = new Uri(p_url);

			client.UploadValuesCompleted += Client_UploadValuesCompleted;
			//@todo use UploadValuesAsync?
			byte[] result = client.UploadValues (url, parameters);
			string s = result.ToString ();
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
				Toast.MakeText (view.Context, "You are now registered", ToastLength.Long).Show ();

				LoginService ls = new LoginService ();

				string token = await ls.Login (username.Text, password.Text);

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
				requestStream.Write(bytes, 0, bytes.Length);
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

