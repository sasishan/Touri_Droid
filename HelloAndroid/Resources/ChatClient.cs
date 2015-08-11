using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Android.Util;
using System.Reflection;
using Java.Lang.Reflect;

namespace TouriDroid
{
	public class Message 
	{
		public string fromUser {get;set;}
		public string message { get; set; }
		public int fromUserId { get; set; }
	}

	public class ChatClient
	{
		public string _myUsername;
		public string _targetUserName;
		public int _myId;
		public bool   isConnected=false;

		public 	readonly HubConnection _connection;
		private IHubProxy _proxy;

		public event EventHandler<Message> OnMessageReceived;
		public event EventHandler<string> PingMe;

		public ChatClient(string myUsername, string targetUserName, int myId)
		{
			//_platform = platform;
			_myUsername = myUsername;
			_targetUserName = targetUserName;
			_myId = myId;

			_connection = new HubConnection(Constants.DEBUG_BASE_URL, "username=" + _myUsername+"&targetUserName="+_targetUserName+"&userid="+_myId.ToString());
			_proxy = _connection.CreateHubProxy("ChatHub");
		}

		public async Task Connect()
		{		
			Log.Debug (Constants.TOURI_TAG, "In connect");	

			_proxy.On("messageReceived", (string fromUser, string message, int messageId, int fromUserId) =>
				{
					if (OnMessageReceived != null)
					{
						//acknowledge to the server we received this message

						try
						{
							_proxy.Invoke("AcknowledgeMessage", messageId);
						}
						catch (Exception e)
						{
							//return;
							Log.Debug(Constants.TOURI_TAG, "AcknowledgeMessage error");
						}
						Log.Debug (Constants.TOURI_TAG, "Acknowledged message with id " + messageId);

						  Message m = new Message ();
						m.fromUser = fromUser;
						m.message = message;
						m.fromUserId = fromUserId;

						//*add fromuserid here - is it int or string?

						OnMessageReceived(this, m);
					}
				});

			_proxy.On("PingClient", (string timestamp) =>
				{
					if (PingMe != null)
					{
						PingMe(this, timestamp);
					}
				});

			try
			{
				await _connection.Start();
				isConnected = true;
			}
			catch (Exception e) {
				//do nothing
				isConnected = false;
			}
			//	await Send("Connected");
		}

		public async void disconnect()
		{
			Log.Debug (Constants.TOURI_TAG, "Disconnecting");

			try
			{
				Log.Debug (Constants.TOURI_TAG, "Trying to stop connection");
				_connection.Stop();
				//_proxy = null;
			}
			catch (Exception e)
			{
				Log.Debug (Constants.TOURI_TAG, "Disconnect error");
				//do nothing
			}
		}

		public async Task<int> SendPrivateMessage(string message, string targetUsername, int myUserId)
		{
			Log.Debug (Constants.TOURI_TAG, "In SendPrivateMessage");
			int messageId;
			try
			{
				messageId = await _proxy.Invoke<int> ("SendPrivateMessage", message, _myUsername, targetUsername, myUserId);
			}
			catch (Exception e) {
				Log.Debug (Constants.TOURI_TAG, "SendPrivateMessage erorr");
				return Constants.FAIL;
			}

			return messageId;
		}

		public Task Send(string message)
		{
			return _proxy.Invoke("Send", _myUsername, message);
		}

		public async Task<int> PingServer()
		{
			Log.Info (Constants.TOURI_TAG, "In PingServer");
			isConnected = false;
			int result = Constants.FAIL;

			try
			{
				Log.Debug (Constants.TOURI_TAG, "Invoking PingClient");
				await _proxy.Invoke ("PingClient", _myUsername);
				result = Constants.SUCCESS;
			}
			catch (TargetInvocationException  e) {
				Log.Debug (Constants.TOURI_TAG, "PingClient TargetInvocationException error");
			}
			catch (InvocationTargetException e) {
				Log.Debug (Constants.TOURI_TAG, "PingClient InvocationTargetException error");
			}
			catch (InvalidOperationException e) {
				Log.Debug (Constants.TOURI_TAG, "PingClient InvalidOperationException error");
			}
			catch (Exception e) {
				Log.Debug (Constants.TOURI_TAG, "PingClient error 4");
			}

			return result;

		}

		public async Task ConnectOnExpertise()
		{

		}

	}
}

