using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace Std.Utility.Net
{
	/// <summary>
	/// Represents the currently active request.
	/// Note this class is designed to be immutable once created.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("RequestId = {RequestId}")]
	public class RequestContext : ILogicalThreadAffinative
	{
		public const string CallContextKey = "StdRequestContext";
		public const string RequestContextHeader = "X-" + CallContextKey;

		public string RequestId { get; private set; }
		
		internal RequestContext(string requestId)
		{
			RequestId = requestId;
		}

		public override string ToString()
		{
			return "RequestId=" + RequestId;
		}

		/// <summary>
		/// Returns the current RequestContext, if any.
		/// </summary>
		public static RequestContext Current
		{
			get
			{
				//first check the call context
				var context = CallContext.GetData(CallContextKey) as RequestContext;
				if (context == null)
				{
					//then check the http context
					var httpContext = HttpContext.Current;
					if (httpContext != null)
					{
						if (httpContext.Items.Contains(CallContextKey))
						{
							context = httpContext.Items[CallContextKey] as RequestContext;
						}
						else
						{
							//this can fail if early enough in the request pipeline
							//so in this case just return nothing
							try
							{
								//and lastly, check the inbound http request if there is one
								if (httpContext.Request != null)
								{
									var requestId = httpContext.Request.Headers.Get(RequestContextHeader);
									if (!string.IsNullOrEmpty(requestId))
									{
										context = new RequestContext(requestId);
									}
								}
							}
							catch (Exception)
							{
								return null;
							}
						}

						if (context != null)
						{
							//update the call context
							CallContext.LogicalSetData(CallContextKey, context);
						}
					}
				}

				return context;
			}
		}

		/// <summary>
		/// Add the current request context (if any) to an exception's Data collection.
		/// Can be called more than once, but only the first caller wins.
		/// </summary>
		/// <param name="ex">The target exception</param>
		public static void AddToException(Exception ex)
		{
			var context = Current;
			
			if (context == null ||
				ex.Data.Contains(CallContextKey))
			{
				return;
			}

			ex.Data[CallContextKey] = context.ToString();
		}

		/// <summary>
		/// Used to initialize a request. If the request context doesnt exist
		/// it will be created and a new id assigned.
		/// </summary>
		/// <returns>The unique request id.</returns>
		public static string InitializeRequest()
		{
			var context = Current;

			if (context == null)
			{
				context = new RequestContext(Guid.NewGuid().ToString("N"));
				SetContext(context);
			}

			return context.RequestId;
		}

		private static void SetContext(RequestContext current)
		{
			CallContext.LogicalSetData(CallContextKey, current);

			//also add to the current http context if one is available
			var httpContext = HttpContext.Current;
			if (httpContext != null)
			{
				httpContext.Items[CallContextKey] = current;
			}
		}

		/// <summary>
		/// Used to initialize a downstream request. Only use this method
		/// to propagate an existing context across different remoting boundaries.
		/// </summary>
		/// <param name="upstreamId">The upstream request id.</param>
		public static void InitializeRequest(string upstreamId)
		{
			InitializeRequest(new RequestContext(upstreamId));
		}

		public static void InitializeRequest(RequestContext upstreamContext)
		{
			if (upstreamContext == null)
			{
				throw new ArgumentNullException("upstreamContext");
			}

			if (Current != null)
			{
				if (Current.RequestId == upstreamContext.RequestId)
				{
					//the requests are the same.. at this point there's no reason to change
					//the context since they're supposed to be identical.
					return;
				}

				throw new InvalidOperationException("Cannot override an existing request context");
			}

			SetContext(upstreamContext);
		}
	}
}