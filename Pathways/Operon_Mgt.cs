using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Pathways
{
    public struct Operon
    {
        public string No;
        public string Name;
        public string GeneName;
        public string LocusName;
        public string GI; // gene id? | protein id?
        public int Strand;
        public int PosLeft;
        public int PosRight;
        public string GeneProduct;
        public int PositionWithinOperon;
        public string Info;
    }

    public static class Operon_Mgt
    {
        public static Operon ParseOperon(string HTTPResp)
        {
            Operon Oper = new Operon();

            string OperonBody = HTTPResp.Remove(0, HTTPResp.IndexOf(@"<TR CLASS=" + '"' + "colum2" + '"' + ">"));
            OperonBody = OperonBody.Remove(OperonBody.IndexOf(@"</TR>"), OperonBody.Length - OperonBody.IndexOf(@"</TR>"));

            string[] Values = OperonBody.Split(new string[] { "<TD>" }, StringSplitOptions.None);

            // I = 1 to skip header
            for (int I = 1; I < Values.Length; I++)
            {
                Values[I] = Values[I].Remove(Values[I].IndexOf("</TD>"), Values[I].Length - Values[I].IndexOf("</TD>"));
            }

            Oper.No = Values[1];
            Oper.Name = Values[2];
            Oper.GeneName = Values[3];
            Oper.LocusName = Values[4];
            Oper.GI = Values[5]; // gene id? | protein id?
            Oper.Strand = int.Parse(Values[6]);
            Oper.PosLeft = int.Parse(Values[7]);
            Oper.PosRight = int.Parse(Values[8]);
            Oper.GeneProduct = Values[9];
            Oper.PositionWithinOperon = int.Parse(Values[10]);
            Oper.Info = Values[11];
            
            return Oper;
        }

        public static Operon FindOperon(string OrganizmName, string OrganizmAlias, string GeneName)
        {
            string URL = @"http://operons.ibt.unam.mx/OperonPredictor/svlOperons?sTypeOperon=g&sOrganismDef=" + OrganizmName +
                "&iOption=1&sOrganism=" + OrganizmAlias + "&sTypeGeneSpecific=specific";
            string Params = @"sGeneName=" + GeneName + "&Search=Search+a+specific+gene";

            string Response = HTTP_Mgt.HTTPPost(URL, Params);

            //string[] FileContent = File.ReadAllLines(@"C:\deleteme\httppost.txt");
            //string Response = string.Join("", FileContent);

            return ParseOperon(Response);            
        }
    }
}
