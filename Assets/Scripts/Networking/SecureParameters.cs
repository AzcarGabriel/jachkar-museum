using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SecureParameters
{
    // Use the common name you used to create the server certificate.
    public static string ServerCommonName = "museum.fen.uchile.cl";

    public static  string MyGameClientCA = "";

    public static  string MyGameServerCertificate;

    public static  string MyGameServerPrivateKey;

    public static void CheckCertificates()
    {

        //MyGameClientCA = File.ReadAllText("./etc/letsencrypt/live/museum.frocoa.me/fullchain.pem");
        MyGameServerCertificate = File.ReadAllText("/home/museum/LinuxServer/museum.fen.uchile.cl.crt");
        MyGameServerPrivateKey = File.ReadAllText("/home/museum/LinuxServer/museum.fen.uchile.cl.key");
    }
}
