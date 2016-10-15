using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static WindowsFormsApplication1.Tender;
using static WindowsFormsApplication1.Lot;
using System.Globalization;
using System.Net;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void resultButton_Click(object sender, EventArgs e)
        {
            
            string adress=requestTextBox.Text;
            
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(adress);
            request.Accept = "*/*";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(stream);
            response.Close();
            Tender tender;
            XmlElement xRoot = xDoc.DocumentElement;
            tender=xmlProcces(xRoot);
            //outTextBox.Text += tender.name;
        }

        private Tender xmlProcces(XmlElement xRoot)
        {
            Tender tender = new Tender();
            XmlNode purchase = xRoot.SelectSingleNode("//data/_embedded/Purchase");
            if (purchase != null)
            {
                foreach (XmlNode curNode in purchase.ChildNodes)
                {
                    if (curNode.FirstChild.FirstChild == null && curNode.Name != null && curNode.InnerText != null)
                        {
                            tender.purchaseData.Add(curNode.Name, curNode.InnerText);
                        }
                    else if(curNode.Name == "organization" )
                    {
                        foreach(XmlNode orgNode in curNode.ChildNodes)
                        {
                            if (curNode != null)
                                tender.organization.Add(orgNode.Name, orgNode.InnerText);
                            else tender.organization.Add(orgNode.Name, null);
                        }
                    }
                    else if(curNode.Name == "lots")
                    {
                        
                        foreach (XmlNode lot in curNode.ChildNodes)
                        {
                            Lot currentLot = new Lot();
                            XmlNode node;
                            //node = lot.SelectSingleNode("agreementSubject");
                            currentLot.agreementSubject = lot.SelectSingleNode("agreementSubject").InnerText;
                            if (lot.SelectSingleNode("lotPrice") != null)
                                currentLot.lotPrice = Decimal.Parse(lot.SelectSingleNode("lotPrice").InnerText.Replace('.', ',')); else currentLot.lotPrice = null;
                            if (lot.SelectSingleNode("unitAmount").InnerText != "")
                                currentLot.unitAmount = Convert.ToInt64(lot.SelectSingleNode("unitAmount").InnerText); else currentLot.unitAmount = null;
                            if (lot.SelectSingleNode("//nds/digitalCode") != null)
                                currentLot.nds.digitalCode = Convert.ToInt32(lot.SelectSingleNode("//nds/digitalCode").InnerText); else currentLot.nds.digitalCode = null;
                            if (lot.SelectSingleNode("//nds/name") != null)
                                currentLot.nds.name = lot.SelectSingleNode("//nds/name").InnerText; else currentLot.nds.digitalCode = null;
                            if (lot.SelectSingleNode("//currency/digitalCode") != null)
                                currentLot.currency.digitalCode = Convert.ToInt32(lot.SelectSingleNode("//currency/digitalCode").InnerText); else currentLot.currency.digitalCode = null;
                            if (lot.SelectSingleNode("//currency/code") != null)
                                currentLot.currency.code = lot.SelectSingleNode("//currency/code").InnerText; else currentLot.currency.code = null;
                            if (lot.SelectSingleNode("maxUnitPrice").InnerText != "")
                                currentLot.maxUnitPrice = Decimal.Parse(lot.SelectSingleNode("maxUnitPrice").InnerText.Replace('.', ',')); else currentLot.maxUnitPrice = null;
                            if (lot.SelectSingleNode("//offerPriceType/code") != null)
                                currentLot.offerPriceType.code = Convert.ToInt32(lot.SelectSingleNode("//offerPriceType/code").InnerText); else currentLot.offerPriceType.code = null;
                            if (lot.SelectSingleNode("//offerPruceType/name") != null)
                                currentLot.offerPriceType.name = lot.SelectSingleNode("//offerPruceType/name").InnerText; else currentLot.offerPriceType.name = null;
                            currentLot.delivBasis = lot.SelectSingleNode("delivBasis").InnerText;
                            if (lot.SelectSingleNode("condSupply") != null)
                                currentLot.condSupply = lot.SelectSingleNode("condSupply").InnerText; else currentLot.condSupply = null;
                            if (lot.SelectSingleNode("requirments") != null) currentLot.requirements = lot.SelectSingleNode("requirments").InnerText; else currentLot.requirements = null;
                            tender.lots.Add(currentLot);
                        }

                    }
                }
            }

            return tender;
        }
    }
}
