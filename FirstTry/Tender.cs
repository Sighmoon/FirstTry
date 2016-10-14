﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class Tender
    {
        public Tender()
        {
            lots = new List<Lot>();
        }
        public string id { get; set; }
        public string name { get; set; }
        public int? statusId { get; set; }
        public string status { get; set; }
        public string typeTorgs { get; set; }
        public string publicDate { get; set; }
        public string endPublicationDate { get; set; }
        public string dateUpdate { get; set; }
        public string requestVersion { get; set; }
        public organizationStruc organization = new organizationStruc();
        public List <Lot> lots;
        public struct organizationStruc
        {
            public string shortName { set; get; }
            public string inn { set; get; }
            public string kpp { set; get; }
            public string postAddress { set; get; }
            public string legalAddress { set; get; }
        }
    }

    public class Lot
    {
        public string agreementSubject { get; set; }
        public decimal? lotPrice { get; set; }
        public ndsStruct nds;
        public currencyStruct currency;
        public long? unitAmount { get; set; }
        public decimal? maxUnitPrice { get; set; }
        public string delivBasis { get; set; }
        public string condSupply { get; set; }
        public string requirements { get; set; }
        public offerPriceTypeStruct offerPriceType;
    }
    public struct currencyStruct
    {
        public int? digitalCode { get; set; }
        public string code { get; set; }
    }
    public struct ndsStruct
    {
        public int? digitalCode { get; set; }
        public string name { get; set; }
    }
    public struct offerPriceTypeStruct
    {
        public int? code { get; set; }
        public string name { get; set; }
        }
}