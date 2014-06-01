using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ValueUtilsBenchmark {
    class HashAnalysisResult {
        public string Name;
        public CollisionStats Collisions;
        public double GetHashCodeNS, EqualsNS, DistinctCountNS, DictionaryNS;
        public XElement ToTableRow() {
            return new XElement("tr",
                new XElement("td", Name),
                new XElement("td", (Collisions.Rate * 100).ToString("f2") + "%"),
                new XElement("td", Collisions.DistinctHashCodes + " / " + Collisions.DistinctValues),
                new XElement("td", DictionaryNS.ToString("f1")),
                new XElement("td", DistinctCountNS.ToString("f1")),
                new XElement("td", EqualsNS.ToString("f1")),
                new XElement("td", GetHashCodeNS.ToString("f1"))
                );
        }
        public static XElement ToTableHead(string title) {
            return
                new XElement("thead",
                    new XElement("tr",
                        new XElement("th",
                             new XAttribute("colspan", 7),
                             title
                        )
                    ),
                    new XElement("tr",
                        new XElement("th", "Name"),
                        new XElement("th", "Collisions"),
                        new XElement("th", "Distinct Hashcodes"),
                        new XElement("th", ".ToDictionary()"),
                        new XElement("th", ".Distinct().Count()"),
                        new XElement("th", ".Equals()"),
                        new XElement("th", ".GetHashCode()")
                    )
                );
        }
        public static XElement ToTable(string title, IEnumerable<HashAnalysisResult> results) {
            return
                new XElement("table",
                    ToTableHead(title),
                    new XElement("tbody",
                        results.Select(r => r.ToTableRow())
                    )
                );
        }
    }
}
