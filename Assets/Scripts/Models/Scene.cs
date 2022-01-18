using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot]
public class Scene
{
    public List<Khachkar> Khachkars { get; set; }
}
