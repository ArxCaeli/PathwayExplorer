using System;
using System.Collections.Generic;

namespace Pathways
{
	public static class KEGG_Mgt
	{
		public static List<string> SplitREST(string RESTResult)
		{
//			ec:4.4.1.1	hsa:1491
//			ec:4.2.1.22	hsa:875
//			ec:2.1.1.5	hsa:635
//			ec:2.1.1.13	hsa:4548

			List<string> Result = new List<string>();

			int PrefixPos = RESTResult.IndexOf(":");

			if (PrefixPos == -1)
				return null;

			while (PrefixPos != -1)
			{
				RESTResult = RESTResult.Remove(0,PrefixPos + 1); // ec column
				RESTResult = RESTResult.Remove(0,RESTResult.IndexOf(":") + 1); // kegg id

				Result.Add(RESTResult.Substring(0,RESTResult.IndexOf("\n")));

				PrefixPos = RESTResult.IndexOf(":");
			}

			return Result;
		}

		public static List<string> GetNCBIGenesIDs (string Organizm, List<string> KEGGGenesIds)
		{
			string Reply = HTTP_Mgt.HttpGet(KEGGREST_url_mgt.GetKEGGtoNCBIconvURL(Organizm,KEGGGenesIds));

			return SplitREST(Reply);
		}

		public static List<string> GetKEGGGenesIDs (string Organizm, List<string> ECList)
		{
			string Reply = HTTP_Mgt.HttpGet(KEGGREST_url_mgt.GetOrgPathwayGenesURL(Organizm, ECList));

			return SplitREST(Reply);
		}

		public static List<string> GetDNAByKEGGGeneIds (string Organizm, List<string> ECList, int Limit)
		{
			return GetCleanFasta(
				HTTP_Mgt.HttpGet(KEGGREST_url_mgt.GetSequencesURL(Organizm, ECList)), Limit);
		}

		private static List<string> GetCleanFasta(string HTMLData, int Limit)
		{
			List<string> FastaResult = new List<string>();
			while(HTMLData.Contains(">"))
			{
				if (HTMLData.IndexOf("<") < HTMLData.IndexOf(">"))
					HTMLData = HTMLData.Remove(0, HTMLData.IndexOf(">") + 1);
				else
				{
					FastaResult.Add(
						TruncateFasta(HTMLData.Substring(0, HTMLData.IndexOf("<")), Limit));
					HTMLData = HTMLData.Remove(0, HTMLData.IndexOf("<"));
				}
			}

			return FastaResult;
		}

		private static string TruncateFasta(string Fasta, int Limit)
		{
			string Result = Fasta.Substring(0, Fasta.IndexOf("\n") + 1);
			Fasta = Fasta.Remove(0, Fasta.IndexOf("\n") + 1);
			Result += Fasta.Substring(0, Math.Min(Limit, Fasta.Length));
			return Result;
		}
	}
}

