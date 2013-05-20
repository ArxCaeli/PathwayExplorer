using System;
using System.Net;

using System.Collections.Generic;


namespace Pathways
{
	public static class NCBISOAP_mgt
	{
		public static List<string> GetFastaByAccNo(List<string> NCBI_GI, int FastaLimit)
		{
			List<string> FastaSequences = new List<string>();

			eFetchSequenceService Serv = new eFetchSequenceService();
			MessageEFetchRequest Message = new MessageEFetchRequest();

			Message.db = "gene";///"nuccore";
			Message.id = NCBI_GI[0];//ComposeRequest(NCBI_GI);
			//Message.strand = "1";
			//Message.retmax = "10"; 
//			Message.seq_start = "1";
//			Message.seq_stop = FastaLimit.ToString(); // not working :/
//			Message.rettype = "fasta"; 
//			Message.complexity = "0"; // minimum xxx
//
			MessageEFetchResult Res = Serv.run_eFetch(Message);

			foreach(TSeq Seq in Res.TSeqSet)
			{
				if (Seq.TSeq_seqtype.value.ToString() == "nucleotide")
					FastaSequences.Add(ComposeFasta(Seq, FastaLimit));
			}

			return FastaSequences;
		}

//		public static List<string> GetFastaByGenes(List<string> NCBI_GI, int FastaLimit)
//		{
//		
//			List<string> FastaSequences = new List<string>();
//
//			eFetchGeneService Serv = new eFetchGeneService();
//
//
//			MessageEFetchRequest Message = new MessageEFetchRequest();
//
//			Message.id = NCBI_GI[0];//ComposeRequest(NCBI_GI);
//
//
//			MessageEFetchResult Res = Serv.run_eFetch(Message);
//
////			foreach(TSeq Seq in Res.TSeqSet)
////			{
////				if (Seq.TSeq_seqtype.value.ToString() == "nucleotide")
////					FastaSequences.Add(ComposeFasta(Seq, FastaLimit));
////			}
//
//			return FastaSequences;
//		}

		private static string ComposeFasta (TSeq Seq, int FastaLimit)
		{
			return ">gi|" + Seq.TSeq_gi + "|gb|" + Seq.TSeq_accver + // gb - wrong
				"| " + Seq.TSeq_defline + "\n" + Seq.TSeq_sequence.Substring(0,FastaLimit);
		}

		public static string ComposeRequest(List<string> Entries)
		{
			string Request = "";

			foreach(string Entry in Entries)
			{
				if (Request != "")
					Request += ",";
				Request += Entry;
			}
			return Request;
		}
	}
}

