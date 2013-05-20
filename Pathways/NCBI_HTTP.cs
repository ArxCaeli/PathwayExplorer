using System;
using System.Collections.Generic;

using System.Xml;
using System.Xml.Linq;

using System.IO;


namespace Pathways
{
	public static class NCBI_HTTP
	{
		//http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=sequences&id=312836839,34577063&rettype=fasta&retmode=text
		public static string GetFastaByGenes (List<string> NCBI_GI)
		{
			//List<string> FastaSequences = new List<string> ();

			return HTTP_Mgt.HttpGet(FetchSequenceURL(NCBI_GI));
		}

		public static string GetGeneInfoXML (string NCBI_GeneId)
		{
			return HTTP_Mgt.HttpGet(FetchGeneInfoXMLURL(NCBI_GeneId));
		}

		private static string FetchSequenceURL(List<string> NCBI_GI)
		{
			//http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=sequences&id=312836839,34577063&rettype=fasta&retmode=text

			// for protein:
			//return @"http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=sequences&id=" + 
			// for mRna
			//return @"http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=nucleotide&id=" +	
			
			return @"http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=nuccore&id=" +			
				NCBISOAP_mgt.ComposeRequest(NCBI_GI) + @"&rettype=fasta&retmode=text";
		}

		private static string FetchGeneInfoXMLURL (string GeneID)
		{
			//http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=gene&id=2&retmode=xml
			return @"http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=gene&id=" + 
				GeneID + "&retmode=xml";
		}

		public static List<string> SplitFasta (string Fasta, int FastaLimit)
		{
			List<string> FastaSeq = new List<string>();

			while(Fasta.Contains(">"))
			{
				//to do: not working
				Fasta = Fasta.Remove(0, Fasta.IndexOf(">"));
				
				string Descr = Fasta.Substring(0, Fasta.IndexOf("\n"));
				Fasta = Fasta.Remove(0, Fasta.IndexOf("\n") + 1);
				string FastaData = Fasta.Substring(0, Fasta.IndexOf(">"));

				FastaData = FastaData.Substring(0, Math.Min(FastaLimit, FastaData.Length));

				FastaSeq.Add(Descr + "\n" + FastaData);
			}

			return FastaSeq;
		}

		public static string GetGeneAccessionNo (string XMLGeneInfo)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(XMLGeneInfo);
			writer.Flush();

			// to do:http://www.biostars.org/p/375/
			XDocument Doc = XDocument.Load(stream);
			// get 
			return "";
		}
	}
}

