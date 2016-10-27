using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;
using ConsoleParse;
using System.Timers;




namespace ConsoleParse
{

    class Program
    {
        static Timer timer = new Timer(86400000);
        static List<Tender> tenderList = new List<Tender>();
        static string pageSize = "2000";
        static string startDateTime = "0";

        static void Main(string[] args)
        {
            TimerSet();
            Console.ReadLine();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e) //событие, срабатывающее по таймеру. 
        {
            string endDateTime = currentTime();
            string address = addressForm(pageSize, startDateTime, endDateTime);
            mainParse(address, ref tenderList);
            Console.WriteLine("Обработка закончена. Обработано объектов: " + tenderList.Count);
        }

        static void TimerSet() // запуск таймера + первое принудительное срабатывание события OnTimedEvent
        {
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Start();
            OnTimedEvent(timer, null);
        }


        static void mainParse(string address, ref List<Tender> tenderList) //главная функция парсерра, автоматически переходит к следующей странице
        {
            XmlDocument xDoc = httpRequestXml(address);
            XmlDocument xIdDoc;
            Tender tender;
            string idAddress, nextAddress;
            foreach(XmlNode node in xDoc.DocumentElement.SelectNodes("//data/_embedded/Purchase"))
            {
                idAddress = addressForm(node.FirstChild.InnerText); // формирование адреса отдельного тендера
                xIdDoc = httpRequestXml(idAddress); // получение документа, содержащего отдельный тендер
                tender = xmlParseFull(xIdDoc.DocumentElement); // парсинг документа
                tenderList.Add(tender);
            }
            Console.WriteLine("Обработка закончена. Обработано объектов: " + tenderList.Count); // временный вывод отчета
            if (xDoc.DocumentElement.SelectSingleNode("//data/_links/next") != null) // переход к следующей странице, если она существует
            {
                nextAddress = xDoc.DocumentElement.SelectSingleNode("//data/_links/next/href").InnerText;
                mainParse(nextAddress, ref tenderList);
            }
        }

        static string currentTime() //текущее времяб возвращается в формате YYYYMMDDhhmmss 
        {
            string timestr = "";

            DateTime date = DateTime.Now;
            timestr = string.Format("{0:yyyy''MM''dd''hh''mm''ss}",date);
            return timestr;
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

        static string addressForm(string pageSize, string startDateTime, string endDateTime) // формирование адреса большого документа
        {
            string[] pattern = { "http://api.federal1.ru/api/registry?", "&pageSize=", "&startDateTime=", "&endDateTime="};
            return pattern[0] + pattern[1] + pageSize.ToString() + pattern[2] + startDateTime + pattern[3] + endDateTime;
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
                    if (curNode.InnerText != "" && curNode.FirstChild.FirstChild == null)
                    {
                        tender.purchaseData.Add(curNode.Name, curNode.InnerText);
                        if(curNode.Name == "id")
                        {
                            Console.WriteLine(curNode.InnerText); // временный вывод айдишников
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
                                currentLot.unitAmount = Decimal.Parse(lot.SelectSingleNode("unitAmount").InnerText.Replace('.', ','));
                            }
                            else
                            {
                                currentLot.unitAmount = null;
                            }
                            if(lot.SelectSingleNode("nds").InnerText != "")
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
                                currentLot.nds.name = null;
                            }

                            if(lot.SelectSingleNode("currency").InnerText !="")
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
