using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using MyList.Models;
using System.Xml.Linq;

namespace MyList.Services
{
    public class TileService
    {
        static public void SetBadgeCountOnTile(int count)
        {
            // Update the badge on the real tile
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", count.ToString());

            BadgeNotification badge = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
            
        }


        public static XmlDocument CreateTiles(TodoItem primaryTile)
        {
            XDocument xDoc = new XDocument(
                new XElement("tile", new XAttribute("version", 3),
                    new XElement("visual",
                        // Small Tile
                        new XElement("binding", new XAttribute("template", "TileSmall"),
                                    new XElement("image", new XAttribute("src", "Assets/142p.png"), new XAttribute("placement", "background")),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile._title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile._description, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),

                        // Medium Tile
                        new XElement("binding", new XAttribute("template", "TileMedium"),
                                    new XElement("image", new XAttribute("src", "Assets/300p.png"), new XAttribute("placement", "background")),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile._title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile._description, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),

                        // Wide Tile
                        new XElement("binding", new XAttribute("template", "TileWide"),
                                    new XElement("image", new XAttribute("src", "Assets/620p.png"), new XAttribute("placement", "background")),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile._title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile._description, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),

                        // Large Tile
                        new XElement("binding", new XAttribute("template", "TileLarge"),
                                    new XElement("image", new XAttribute("src", "Assets/two620.png"), new XAttribute("placement", "background")),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile._title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile._description, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        )
                    )
                )
            );

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;
        }
    }

    
}
