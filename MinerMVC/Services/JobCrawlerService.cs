using HtmlAgilityPack;
using MinerMVC.Models;

namespace MinerMVC.Services;

public class JobCrawlerService : IJobCrawlerService
{
    public List<Company> GetHsinChuScienceParkCompany()
    {
        var allCompany = new List<Company>();
        for (var i = 1; i < 16; i++)
        {
            var web = new HtmlWeb();
            var doc = web.Load("https://www.sipa.gov.tw/home.jsp?serno=201001210039&mserno=201001210037" +
                               "&menudata=ChineseMenu&contlink=ap/introduction_2_5.jsp&serno3=201002010011&language=chinese" +
                               "&intpage=1&address=&companyname=&sipa=&stype=&qsipa=&fid=&Qstring=請輸入關鍵字" +
                               "&page=" + i);
            var nodes = doc.DocumentNode.SelectNodes("//td[@data-th='廠商名稱']");

            var companies = nodes.Distinct().Select(node => new Company(node.InnerText));
            allCompany.AddRange(companies);
        }

        return allCompany;
    }
}