using System;
using System.Net;
using System.IO;

using System.Collections.Generic;

namespace Pathways
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// genes:
			// http://rest.kegg.jp/link/genes/enzyme:2.7.2.4/
			// ortologs:
			// http://www.kegg.jp/kegg-bin/show_pathway?ko00270

			// Enzymes EC No
			List<string> ECList = new List<string>();
			ECList.Add("4.4.1.1");
			ECList.Add("2.5.1.48");
			ECList.Add("2.3.1.46");
			ECList.Add("2.7.2.4");
			ECList.Add("1.2.1.11");
			ECList.Add("1.1.1.3");
			ECList.Add("2.3.1.31");
			ECList.Add("4.2.1.22");
			ECList.Add("4.4.1.8");
			ECList.Add("2.5.1.49");
			ECList.Add("2.1.1.5");
			ECList.Add("2.1.1.10");
			ECList.Add("2.1.1.13");
			ECList.Add("2.1.1.14");
			string Organizm = "eco"; // ece = ecoli / hum = hsa

			//string OverallRes = "ec:4.4.1.1\thsa:1491\nec:4.2.1.22\thsa:875\nec:2.1.1.5\thsa:635\nec:2.1.1.13\thsa:4548\n";
			List<string> KEGGGenes = KEGG_Mgt.GetKEGGGenesIDs(Organizm, ECList);
			//List<string> NCBIGenes = KEGG_Mgt.GetNCBIGenesIDs(Organizm, KEGGGenes);
			List<string> FastaSeq = KEGG_Mgt.GetDNAByKEGGGeneIds(Organizm, KEGGGenes, 200);

			foreach (string Seq in FastaSeq)
				Console.WriteLine(Seq + "\n");

			//to do 
			// get rec from ncbi - get 200 before cds


			//ECList.Add("2.7.2.4");
			//ECList.Add("2.1.1.14");
			//ECList.Add("1.1.1.3");


//			// find orthologs
//			List<string> Species = new List<string>();
//			//Species.Add("eco");
//			Species.Add("ppf"); // pseudomonada putida f1
//			Species.Add("noc"); // Nitrosococcus oceani
//			Species.Add("vce"); // Vibrio cholerae O1
//
//			FastaSeq = SpeciesGenes(Species, "2.7.2.4");
//			foreach (string Seq in FastaSeq)
//				Console.WriteLine(Seq + "\n");

		}

		static List<string> SpeciesGenes(List<string> Species, string Enzyme)
		{
			List<string> FastaSeq = new List<string>();

			List<string> Enz = new List<string>();
			Enz.Add(Enzyme);
			foreach(string Org in Species)
			{
				List<string> KEGGGenes = KEGG_Mgt.GetKEGGGenesIDs(Org, Enz);
				List<string> SpecFasta = KEGG_Mgt.GetDNAByKEGGGeneIds(Org, KEGGGenes, 200); 
				foreach(string S in SpecFasta)
					FastaSeq.Add(S);
			}

			return FastaSeq;
		}
	}
}
