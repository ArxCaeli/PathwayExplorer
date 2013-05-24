using System;
using System.Net;
using System.IO;

using System.Collections.Generic;

using System.Runtime.Serialization.Formatters.Binary;


namespace Pathways
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            //GetPathwaySequences();

            //to do: find orthologs http://www.kegg.jp/ssdb-bin/ssdb_best?org_gene=eco:b0002
            // http://exploringlifedata.blogspot.co.uk/2012/11/have-rest-with-kegg.html
            GetGeneOrtologSequences();

		}

        public static void GetPathwaySequences()
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

            List<string> KEGGGenes = KEGG_Mgt.GetKEGGGenesIDs(Organizm, ECList);
            List<GeneInfo> GeneInfoList = KEGG_Mgt.GetGeneInfoByKEGGGenes(Organizm, KEGGGenes);

            List<Operon> Operons = Operon_Mgt.GetOperons(GeneInfoList);

            List<string> DNASequences = KEGG_Mgt.GetFastaDNASequencesByOperons(Operons, GeneInfoList);

            foreach (string Fasta in DNASequences)
                Console.WriteLine(Fasta);

            Console.ReadKey();
        }

        public static void GetGeneOrtologSequences()
        {
            List<string> Species = new List<string>();
            Species.Add("eco");
            Species.Add("ppf"); // pseudomonada putida f1
            Species.Add("noc"); // Nitrosococcus oceani
            Species.Add("vco"); // Vibrio cholerae O395

            List<GeneInfo> GeneInfoList = KEGG_Mgt.GetGeneInfoByKEGGGenes(Species, "metB");
            List<Operon> Operons = Operon_Mgt.GetOperons(GeneInfoList);
            List<string> DNASequences = KEGG_Mgt.GetFastaDNASequencesByOperons(Operons, GeneInfoList);

            foreach (string Fasta in DNASequences)
                Console.WriteLine(Fasta);

            Console.ReadKey();
        }


		static List<string> SpeciesGenes(List<string> Species, string Enzyme)
		{
			List<string> FastaSeq = new List<string>();

			List<string> Enz = new List<string>();
			Enz.Add(Enzyme);
			foreach(string Org in Species)
			{
				List<string> KEGGGenes = KEGG_Mgt.GetKEGGGenesIDs(Org, Enz);
				List<string> SpecFasta = KEGG_Mgt.GetDNAByKEGGGeneIds(Org, KEGGGenes); 
				foreach(string S in SpecFasta)
					FastaSeq.Add(S);
			}

			return FastaSeq;
		}      

	}
}
