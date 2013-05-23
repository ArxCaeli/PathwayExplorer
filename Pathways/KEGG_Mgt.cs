using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;

namespace Pathways
{
    public struct GeneInfo
    {
        public string OrganizmAlias;
        public string OrganizmName;
        public string GeneName;
    }

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

        public static List<GeneInfo> GetGeneInfoByKEGGGenes(string Organizm, List<string> KEGGGenes)
        {
            string AllInfo = HTTP_Mgt.HttpGet(KEGGREST_url_mgt.GetGenesInfoURL(Organizm, KEGGGenes));

            Console.WriteLine(AllInfo);

            return GetGeneInfoFromREST(AllInfo);
        }

        private static List<GeneInfo> GetGeneInfoFromREST(string RESTReply)
        {
            List<GeneInfo> GeneInfoList = new List<GeneInfo>();

            string[] GI = RESTReply.Split(new string[] { "///\n" }, StringSplitOptions.None);
            foreach (string S in GI)
                if (S != "")
                    GeneInfoList.Add(ParseGeneInfo(S));

            return GeneInfoList;
        }

        private static GeneInfo ParseGeneInfo(string GeneInfoStr)
        {
            GeneInfo Gene = new GeneInfo();

            Regex R = new Regex(@"\nNAME\s+(.+)\n", RegexOptions.Multiline);
            Match M = R.Match(GeneInfoStr);
            Gene.GeneName = M.Groups[1].Captures[0].Value;

            R = new Regex(@"\nORGANISM\s+((\w+)\s+(.+))\n", RegexOptions.Multiline);
            M = R.Match(GeneInfoStr);

            Gene.OrganizmAlias = M.Groups[2].Captures[0].Value;
            Gene.OrganizmName = M.Groups[3].Captures[0].Value;

            return Gene;
        }

        public static string GetCutSequence(int SeqStart, int SeqEnd, int Strand, string OrganismAlias)
        {
            return 
                GetCleanFasta(
                    HTTP_Mgt.HttpGet(
                        KEGGREST_url_mgt.GetCutSequenceURL(SeqStart, SeqEnd, Strand, OrganismAlias)));
        }



        /// <summary>
        /// Gets dna from gene start (operon not used)
        /// </summary>
        /// <param name="Organizm"></param>
        /// <param name="ECList"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
		public static List<string> GetDNAByKEGGGeneIds (string Organizm, List<string> ECList)
		{
            return null;
            //GetCleanFasta(
            //     HTTP_Mgt.HttpGet(KEGGREST_url_mgt.GetSequencesURL(Organizm, ECList)));
		}
                
		private static string GetCleanFasta(string HTMLData)
		{            
            HTMLData = HTMLData.Remove(0, HTMLData.IndexOf("<PRE>") + 6);
            HTMLData = HTMLData.Remove(HTMLData.IndexOf("</PRE>"), HTMLData.Length - HTMLData.IndexOf("</PRE>"));
            HTMLData = HTMLData.Remove(0, HTMLData.IndexOf("\n") + 1); // remove description
            HTMLData = HTMLData.Replace("\n", "");

            while (HTMLData.Contains("<"))
			{
                HTMLData = HTMLData.Remove(HTMLData.IndexOf("<"), HTMLData.IndexOf(">") - HTMLData.IndexOf("<") + 1);
			}

			return HTMLData;
		}
	}
}

