using System;
using System.Net;
using System.IO;

namespace Pathways
{
	public static class HTTP_Mgt
	{
		 public static string HttpGet(string url) {
		  	HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
		  	string result = null;
		  	using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
		  	{
		    	StreamReader reader = new StreamReader(resp.GetResponseStream());
		    	result = reader.ReadToEnd();
		  	}
		  	return result;
		}
	}
}

