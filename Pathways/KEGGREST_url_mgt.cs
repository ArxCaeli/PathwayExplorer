using System;
using System.Collections.Generic;

namespace Pathways
{
	public static class KEGGREST_url_mgt
	{
		public static string GetOrgPathwayGenesURL(string Organizm, List<string> EnzymeNo)
		{
			// genes:
			// http://rest.kegg.jp/link/genes/enzyme:2.7.2.4/			
			return @"http://rest.kegg.jp/link/" + 
				Organizm + "/" + ComposeRequest("enzyme",EnzymeNo);
		}

		public static string GetKEGGtoNCBIconvURL(string Organizm, List<string> KEGGGenes)
		{
			// http://rest.kegg.jp/conv/ncbi-gi/hsa:10458+ece:Z5100

			//return @"http://rest.kegg.jp/conv/ncbi-gi/" + 
			return @"http://rest.kegg.jp/conv/ncbi-geneid/" + 
				ComposeRequest(Organizm,KEGGGenes);
		}

		public static string GetSequencesURL (string Organizm, List<string> KEGGGenes)
		{
			//http://www.genome.jp/dbget-bin/www_bget?-h
			//http://www.genome.jp/dbget-bin/www_bget?-f+dbname1:identifier1+dbname2:identifier2+..
			//-f+-n+n nucleotide sequence in FASTA format (GENES database only)
			return @"http://www.genome.jp/dbget-bin/www_bget?-f+-n+n+" +
				ComposeRequest(Organizm,KEGGGenes);
		}

        public static string GetGenesInfoURL(string Organizm, List<string> KEGGGenes)
        {
            return "http://rest.kegg.jp/get/" + ComposeRequest(Organizm, KEGGGenes);            
        }

        public static string GetGenesInfoURL(List<string> Organisms, string KEGGGene)
        {
            return "http://rest.kegg.jp/get/" + ComposeRequest(Organisms, KEGGGene);
        }

        public static string GetGenesInfoURL(List<string> Organisms, List<string> KEGGGenes)
        {
            return "http://rest.kegg.jp/get/" + ComposeRequest(Organisms, KEGGGenes);
        }


        public static string GetCutSequenceURL(int SeqStart, int SeqEnd, int Strand, int Chr, string OrganismAlias)
        {
            //http://www.genome.jp/dbget-bin/cut_sequence_genes.pl?FROM=4127658&TO=4130290&VECTOR=1&ORG=eco
            return "http://www.genome.jp/dbget-bin/cut_sequence_genes.pl?FROM=" + SeqStart.ToString() + 
                "&TO=" + SeqEnd.ToString() + "&VECTOR=" + Strand.ToString() + "&ORG=" + OrganismAlias +
                ((Chr == 0) ? "" : "&CHR=" + Chr.ToString());
        }

        public static string GetOrthologListURL(string OrthologNo)
        {
            //http://rest.kegg.jp/link/genes/K00928
            return "http://rest.kegg.jp/link/genes/" + OrthologNo;
        }

		private static string ComposeRequest(string Prefix, List<string> Entries)
		{
			string Request = "";
			foreach(string Entry in Entries)
			{
				if (Request != "")
					Request += "+";
				Request += Prefix + ":" + Entry; 
			}
			return Request;
		}

		private static string ComposeRequest(List<string> Prefixes, string Entry)
		{
			string Request = "";
			foreach(string Prefix in Prefixes)
			{
				if (Request != "")
					Request += "+";
				Request += Prefix + ":" + Entry; 
			}
			return Request;
		}

        private static string ComposeRequest(List<string> Prefixes, List<string> Entries)
        {
            string Request = "";
            for (int I = 0; I < Prefixes.Count; I++) 
            {
                if (Request != "")
                    Request += "+";
                Request += Prefixes[I] + ":" + Entries[I];
            }
            return Request;
        }
	}
}

