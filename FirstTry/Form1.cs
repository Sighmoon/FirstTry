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
            outTextBox.Text += tender.name;
        }

        private Tender xmlProcces(XmlElement xRoot)
        {
            Tender tender = new Tender();
            XmlNode purchase = xRoot.SelectSingleNode("//data/_embedded/Purchase");
            if(purchase!=null)
            {
                XmlNode node = purchase.SelectSingleNode("id");
                tender.id = node.InnerText;
                node = purchase.SelectSingleNode("name");
                tender.name = node.InnerText;
                node = purchase.SelectSingleNode("status");
                tender.status = node.InnerText;
                node = purchase.SelectSingleNode("statusId");
                tender.statusId = Convert.ToInt32(node.InnerText);
                node = purchase.SelectSingleNode("typeTorgs");
                tender.typeTorgs = node.InnerText;
                node = purchase.SelectSingleNode("publicDate");
                tender.status = node.InnerText;
                node = purchase.SelectSingleNode("endPublicationDate");
                tender.endPublicationDate = node.InnerText;
                node = purchase.SelectSingleNode("dateUpdate");
                tender.dateUpdate = node.InnerText;
                node = purchase.SelectSingleNode("requestVersions");
                tender.requestVersion = node.InnerText;
                node = purchase.SelectSingleNode("//organization/shortName");
                tender.organization.shortName = node.InnerText;
                node = purchase.SelectSingleNode("//organization/inn");
                tender.organization.inn = node.InnerText;
                node = purchase.SelectSingleNode("//organization/kpp");
                tender.organization.kpp = node.InnerText;
                node = purchase.SelectSingleNode("//organization/postAddress");
                if (node != null) tender.organization.postAddress = node.InnerText;
                node = purchase.SelectSingleNode("//organization/legalAddress");
                if (node != null) tender.organization.legalAddress = node.InnerText;
                XmlNodeList lots = purchase.SelectNodes("//lots/lot");
                foreach (XmlNode lot in lots)
                {
                    string tmp;
                    Lot currentLot = new Lot();
                    node = lot.SelectSingleNode("agreementSubject");
                    currentLot.agreementSubject = node.InnerText;
                    node = lot.SelectSingleNode("lotPrice");
                    if (node != null) currentLot.lotPrice = Decimal.Parse(node.InnerText.Replace('.', ',')); else currentLot.lotPrice = null;
                    node = lot.SelectSingleNode("unitAmount");
                    if (node.InnerText != "") currentLot.unitAmount = Convert.ToInt64(node.InnerText); else currentLot.unitAmount = null;
                    node = lot.SelectSingleNode("//nds/digitalCode");
                    if (node != null) currentLot.nds.digitalCode = Convert.ToInt32(node.InnerText); else currentLot.nds.digitalCode = null;
                    node = lot.SelectSingleNode("//nds/name");
                    if (node != null) currentLot.nds.name = node.InnerText; else currentLot.nds.digitalCode = null;
                    node = lot.SelectSingleNode("//currency/digitalCode");
                    if (node != null) currentLot.currency.digitalCode = Convert.ToInt32(node.InnerText); else currentLot.currency.digitalCode = null;
                    node = lot.SelectSingleNode("//currency/code");
                    if (node != null) currentLot.currency.code = node.InnerText; else currentLot.currency.code = null;
                    node = lot.SelectSingleNode("maxUnitPrice");
                    if (node.InnerText != "") currentLot.maxUnitPrice = Decimal.Parse(node.InnerText.Replace('.', ',')); else currentLot.maxUnitPrice = null;
                    node = lot.SelectSingleNode("//offerPriceType/code");
                    if (node != null) currentLot.offerPriceType.code = Convert.ToInt32(node.InnerText); else currentLot.offerPriceType.code = null;
                    node = lot.SelectSingleNode("//offerPruceType/name");
                    if (node != null) currentLot.offerPriceType.name = node.InnerText; else currentLot.offerPriceType.name = null;
                    node = lot.SelectSingleNode("delivBasis");
                    currentLot.delivBasis = node.InnerText;
                    node = lot.SelectSingleNode("condSupply");
                    if (node != null) currentLot.condSupply = node.InnerText; else currentLot.condSupply = null;
                    node = lot.SelectSingleNode("requirments");
                    if (node != null) currentLot.requirements = node.InnerText; else currentLot.requirements = null;
                    if (currentLot != null) tender.lots.Add(currentLot);
                }
            }

            return tender;
        }
    }
}
