using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pathways
{
    public struct GeneInfo
    {
        public string OrganizmAlias;
        public string OrganizmName;
        public string GeneName;
        public int Chr; // plasmid or chr
        public int Start;
        public int Stop;
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

        public static List<GeneInfo> GetGeneInfoByKEGGGenes(List<string> Organisms, string GeneName)
        {
            string AllInfo = HTTP_Mgt.HttpGet(KEGGREST_url_mgt.GetGenesInfoURL(Organisms, GeneName));

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

            // get position: to get chr (plasmid / chromosome)
            R = new Regex(@"\nPOSITION\s+((\d:)?(\d+)..(\d+))\n", RegexOptions.Multiline);
            M = R.Match(GeneInfoStr);
            if (M.Success)
            {
                if (M.Groups[2].Captures.Count != 0)
                    Gene.Chr = int.Parse(M.Groups[2].Captures[0].Value.Replace(":", ""));
                else
                    Gene.Chr = 0;

                Gene.Start = int.Parse(M.Groups[3].Captures[0].Value);
                Gene.Stop = int.Parse(M.Groups[4].Captures[0].Value);
            }
            return Gene;
        }

        public static string GetCutSequence(int SeqStart, int SeqEnd, int Strand, int Chr, string OrganismAlias)
        {
            return 
                GetCleanFasta(
                    HTTP_Mgt.HttpGet(
                        KEGGREST_url_mgt.GetCutSequenceURL(SeqStart, SeqEnd, Strand, Chr, OrganismAlias)));
        }

        public static List<string> GetFastaDNASequencesByOperons(List<Operon> Operons, List<GeneInfo> GeneInfoList)
        {
            List<string> DNASequences = new List<string>();

            for (int I = 0; I < GeneInfoList.Count; I++)
            {
                if (Operons[I].Strand == 1)
                    DNASequences.Add(
                        KEGG_Mgt.AddFastaDescription(
                            KEGG_Mgt.GetCutSequence(
                                Operons[I].PosLeft - 200, Operons[I].PosLeft, 1, GeneInfoList[I].Chr, GeneInfoList[I].OrganizmAlias),
                            GeneInfoList[I]));
                else
                    DNASequences.Add(
                        KEGG_Mgt.AddFastaDescription(
                            KEGG_Mgt.GetCutSequence(
                                Operons[I].PosRight, Operons[I].PosRight + 200, 2, GeneInfoList[I].Chr, GeneInfoList[I].OrganizmAlias),
                        GeneInfoList[I]));
            }

            SerializeFasta(@"C:\deleteme\Fasta.txt", DNASequences);

            //DNASequences = DeserializeFasta(@"C:\deleteme\Fasta.txt");

            return DNASequences;
        }

        public static string AddFastaDescription(string Fasta, GeneInfo Info)
        {
            return ">" + Info.OrganizmAlias + "|" + Info.GeneName + "\n" + Fasta;
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

        public static void SerializeFasta(string FileName, List<string> Fasta)
        {
            using (Stream stream = File.Open(FileName, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, Fasta);
            }
        }

        public static List<string> DeserializeFasta(string FileName)
        {
            var Fasta = new List<string>();
            using (Stream stream = File.Open(FileName, FileMode.Open))
            {
                BinaryFormatter bin = new BinaryFormatter();

                Fasta = (List<string>)bin.Deserialize(stream);
            }

            return Fasta;
        }
	}
}

