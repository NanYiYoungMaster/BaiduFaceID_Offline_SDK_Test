using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageUtil
{
    public static byte[] GetImageBytes(string path)
    {
        //读取  
        FileInfo file = new FileInfo(path);
        var stream = file.OpenRead();
        byte[] buffer = new byte[file.Length];
        stream.Read(buffer, 0, Convert.ToInt32(file.Length));
        return buffer;
    }

    private string GetImageString(string path)
    {
        //读取  
        FileInfo file = new FileInfo(path);
        var stream = file.OpenRead();
        byte[] buffer = new byte[file.Length];
        //读取图片字节流  
        stream.Read(buffer, 0, Convert.ToInt32(file.Length));
        //base64字符串  
        string imageBase64 = Convert.ToBase64String(buffer);
        return imageBase64;
    }
}
