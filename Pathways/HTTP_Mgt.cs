using System;
using System.Net;
using System.IO;

using System.Text;

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

        public static string HTTPPost(string URL, string Params)
        {
            string Res = null;

            HttpWebRequest Req = WebRequest.Create(URL) as HttpWebRequest;

            Req.Accept = "*/*";
            Req.KeepAlive = false;
            Req.ProtocolVersion = HttpVersion.Version10;
            Req.ServicePoint.ConnectionLimit = 1;
            //Req.KeepAlive = false;
            //Req.ProtocolVersion = 

            //string ProxyString = "";
            //Req.Proxy = new WebProxy(ProxyString, true);

            Req.ContentType = @"application/x-www-form-urlencoded";
            Req.Method = "POST";

            byte[] Bytes = Encoding.ASCII.GetBytes(Params);
            Req.ContentLength = Bytes.Length;

            System.IO.Stream Str = Req.GetRequestStream();

            Str.Write(Bytes, 0, Bytes.Length);
            Str.Close();
            
            using (WebResponse Resp = Req.GetResponse())
            {
                if (Resp != null)
                {
                    using (System.IO.StreamReader Sr = new System.IO.StreamReader(Resp.GetResponseStream()))
                    {
                        Res = Sr.ReadToEnd().Trim();
                        Sr.Close();
                    }
                }
                Resp.Close();
            }
            return Res;
        }
	}
}

