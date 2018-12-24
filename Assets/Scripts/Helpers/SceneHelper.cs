using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class SceneHelper
{
    public static IDictionary<string, object> GetKhachKar(string sceneName, int id)
    {
        string fileName = sceneName + ".xml";
        string path = Path.Combine(@"Assets\Scripts\Scenes\", fileName);
        //string path = Path.Combine(@"metaInfo\", fileName);
        //path = Path.Combine(Environment.CurrentDirectory, path);

        Scene scene = XmlHelper.FromXmlFile<Scene>(path);
        Khachkar khachkar = scene.Khachkars.Find(x => x.Id == 1);
        return khachkar.ToDictionary();
    }

    public static IDictionary<string, object> GetKhachkarByXML(string xml)
    {
        Scene scene = XmlHelper.FromXml<Scene>(xml);
        Khachkar khachkar = scene.Khachkars.Find(x => x.Id == 1);
        return khachkar.ToDictionary();
    }
}

