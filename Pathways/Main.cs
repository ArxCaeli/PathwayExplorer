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

            List<string> KEGGGenes = KEGG_Mgt.GetKEGGGenesIDs(Organizm, ECList);
            List<GeneInfo> GeneInfoList = KEGG_Mgt.GetGeneInfoByKEGGGenes(Organizm, KEGGGenes);
            
            List<Operon> Operons = new List<Operon>();
            //foreach(GeneInfo G in GeneInfoList)
            //{
            //    Console.WriteLine("Getting operon info for: " + G.GeneName);

            //    //if (G.GeneName != "metB")
            //        Operons.Add(Operon_Mgt.FindOperon(G.OrganizmName, G.OrganizmAlias, G.GeneName));//"Escherichia coli K-12 MG1655", "eco", "metL"));
            //}

            //Operon_Mgt.SerializeOperons(@"C:\deleteme\Operons.txt", Operons);
            Operons = Operon_Mgt.DeserializeOperons(@"C:\deleteme\Operons.txt");


            List<string> DNASequences = new List<string>();

            for (int I = 0; I < GeneInfoList.Count; I++)
            {
                if (Operons[I].Strand == 1)
                    DNASequences.Add(
                        KEGG_Mgt.GetCutSequence(
                            Operons[I].PosLeft - 200, Operons[I].PosLeft, 1, GeneInfoList[I].OrganizmAlias));
                else
                    DNASequences.Add(
                        KEGG_Mgt.GetCutSequence(
                            Operons[I].PosRight, Operons[I].PosRight + 200, 2, GeneInfoList[I].OrganizmAlias));            
            }
            
            //create fasta from upstream sequences

            Console.ReadKey();
		}


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
