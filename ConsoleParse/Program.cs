using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;
using ConsoleParse;




namespace ConsoleParse
{

    class Program
    {
        
        static void Main(string[] args)
        {
            string page, pageSize, startDateTime, endDateTime;
            List<string> idList;
            List<Tender> tenderList;

            //временный ввод ручками
            page = Console.ReadLine();
            pageSize = Console.ReadLine();
            startDateTime = Console.ReadLine();
            endDateTime = Console.ReadLine();         

            string address = addressForm(page, pageSize, startDateTime, endDateTime); // формирование адреса из компонентов
            XmlDocument xDoc = httpRequestXml(address); // загрузка документа 
            idList = xmlParseShort(xDoc.DocumentElement); // формирование списка id, по которым формируется адрес каждого торга
            tenderList = xmlParseById(idList); // формирование списка тендеров
            
            Console.WriteLine("Обработка закончена. Обработано объектов: " + tenderList.Count);
            Console.ReadLine();
        }

        static List<string> xmlParseShort(XmlElement xRoot) // парсинг документа, содержащего список торгов и выдирание айдишников, с целью использования их в формирование адреса
        {
            List<string> idList = new List<string>();

            foreach(XmlNode node in xRoot.SelectNodes("//data/_embedded/Purchase"))
            {
                idList.Add(node.FirstChild.InnerText);
            }
            return idList; 
        }

        static XmlDocument httpRequestXml(string address) // запрос к апи
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(address);
            request.Accept = "*/*";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(stream);
            response.Close();
            return xDoc;
        } 

        static List<Tender> xmlParseById(List<string> idList) //парсинг документа, содержащего отдельный торг
        {
            string address;
            Tender tender;
            XmlDocument xDoc;
            List<Tender> tenderList = new List<Tender>();

            foreach (string id in idList)
            {
                address = addressForm(id);
                xDoc = httpRequestXml(address);
                tender = xmlParseFull(xDoc.DocumentElement);
                tenderList.Add(tender);
            }
            return tenderList;
        }
        static string addressForm(string page, string pageSize, string startDateTime, string endDateTime) // формирование адреса большого документа
        {
            string[] pattern = { "http://api.federal1.ru/api/registry?page=", "&pageSize=", "&startDateTime=", "&endDateTime=" };
            return pattern[0]+page.ToString()+pattern[1]+pageSize.ToString()+pattern[2]+startDateTime+pattern[3]+endDateTime;
        } 

        static string addressForm(string id) // формирование адреса документа отдельного торга
        {
            const string pattern = "http://api.federal1.ru/api/registry/";
            return pattern + id;  ;
        }
        static Tender xmlParseFull(XmlElement xRoot) // большая и ужасная функция обработки отдельного документа. Как смог :)
        {
            Tender tender = new Tender();
            XmlNode purchase = xRoot.SelectSingleNode("//data/_embedded/Purchase");
            if (purchase != null)
            {
                foreach (XmlNode curNode in purchase.ChildNodes)
                {
                    if (curNode.FirstChild.FirstChild == null && curNode.InnerText != "")
                    {
                        tender.purchaseData.Add(curNode.Name, curNode.InnerText);
                        if(curNode.Name == "id")
                        {
                            Console.WriteLine(curNode.InnerText);
                        }
                    }
                    else if (curNode.Name == "organization")
                    {
                        foreach (XmlNode orgNode in curNode.ChildNodes)
                        {
                            if (curNode != null)
                                tender.organization.Add(orgNode.Name, orgNode.InnerText);
                            else tender.organization.Add(orgNode.Name, null);
                        }
                    }
                    else if (curNode.Name == "lots")
                    {

                        foreach (XmlNode lot in curNode.ChildNodes)
                        {
                            //мне пришлось
                            Lot currentLot = new Lot();
                            currentLot.agreementSubject = lot.SelectSingleNode("agreementSubject").InnerText;
                            if (lot.SelectSingleNode("lotPrice").InnerText != "")
                            {
                                currentLot.lotPrice = Decimal.Parse(lot.SelectSingleNode("lotPrice").InnerText.Replace('.', ','));
                            }
                            else
                            {
                                currentLot.lotPrice = null;
                            }
                            if (lot.SelectSingleNode("unitAmount").InnerText != "")
                            {
                                //currentLot.unitAmount = Convert.ToInt64(lot.SelectSingleNode("unitAmount").InnerText);
                                currentLot.unitAmount = Decimal.Parse(lot.SelectSingleNode("unitAmount").InnerText.Replace('.', ','));
                            }
                            else
                            {
                                currentLot.unitAmount = null;
                            }
                            if(lot.SelectSingleNode("nds")!=null)
                            {
                                if (lot.SelectSingleNode("//nds/digitalCode").InnerText != "")
                                {
                                    currentLot.nds.digitalCode = Convert.ToInt32(lot.SelectSingleNode("//nds/digitalCode").InnerText);
                                }
                                else
                                {
                                    currentLot.nds.digitalCode = null;
                                }

                                if (lot.SelectSingleNode("//nds/name").InnerText != "")
                                {
                                    currentLot.nds.name = lot.SelectSingleNode("//nds/name").InnerText;
                                }
                                else
                                {
                                    currentLot.nds.digitalCode = null;
                                }
                            }
                            else
                            {
                                currentLot.nds.digitalCode = null;
                                currentLot.nds.digitalCode = null;
                            }

                            if(lot.SelectSingleNode("currency")!=null)
                            {
                                if (lot.SelectSingleNode("//currency/digitalCode").InnerText != "")
                                {
                                    currentLot.currency.digitalCode = Convert.ToInt32(lot.SelectSingleNode("//currency/digitalCode").InnerText);
                                }
                                else
                                {
                                    currentLot.currency.digitalCode = null;
                                }

                                if (lot.SelectSingleNode("//currency/code").InnerText != "")
                                {
                                    currentLot.currency.code = lot.SelectSingleNode("//currency/code").InnerText;
                                }
                                else
                                {
                                    currentLot.currency.code = null;
                                }
                            }
                            else
                            {
                                currentLot.currency.code = null;
                                currentLot.currency.digitalCode = null;
                            }


                            if (lot.SelectSingleNode("maxUnitPrice").InnerText != "")
                            {
                                currentLot.maxUnitPrice = Decimal.Parse(lot.SelectSingleNode("maxUnitPrice").InnerText.Replace('.', ','));
                            }
                            else
                            {
                                currentLot.maxUnitPrice = null;
                            }

                            if(lot.SelectSingleNode("offerPriceType").InnerText!="")
                            {
                                if (lot.SelectSingleNode("//offerPriceType/code").InnerText != "")
                                {
                                    currentLot.offerPriceType.code = Convert.ToInt32(lot.SelectSingleNode("//offerPriceType/code").InnerText);
                                }
                                else
                                {
                                    currentLot.offerPriceType.code = null;
                                }

                                if (lot.SelectSingleNode("//offerPriceType/name").InnerText != "")
                                {
                                    currentLot.offerPriceType.name = lot.SelectSingleNode("//offerPriceType/name").InnerText;
                                }
                                else
                                {
                                    currentLot.offerPriceType.name = null;
                                }
                            }
                            else
                            {
                                currentLot.offerPriceType.name = null;
                                currentLot.offerPriceType.code = null;
                            }
                            

                            currentLot.delivBasis = lot.SelectSingleNode("delivBasis").InnerText;

                            if (lot.SelectSingleNode("condSupply").InnerText != "")
                            {
                                currentLot.condSupply = lot.SelectSingleNode("condSupply").InnerText;
                            }
                            else
                            {
                                currentLot.condSupply = null;
                            }
                            if (lot.SelectSingleNode("requirements").InnerText != "")
                            {
                                currentLot.requirements = lot.SelectSingleNode("requirements").InnerText;
                            }
                            else
                            {
                                currentLot.requirements = null;
                            }
                            tender.lots.Add(currentLot);
                        }

                    }
                }
            }

            return tender;
        }

    }

}
