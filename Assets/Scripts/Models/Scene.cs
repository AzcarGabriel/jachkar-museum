using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

[XmlRoot]
public class Scene
{
    public List<Khachkar> Khachkars { get; set; }
}
