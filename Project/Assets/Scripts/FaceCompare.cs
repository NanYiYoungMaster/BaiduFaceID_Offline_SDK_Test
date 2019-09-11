using System;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

// 人脸比较1:1、1:N、抽取人脸特征值、按特征值比较等
class FaceCompare
{
    // 提取人脸特征值(传图片文件路径)
    [DllImport("BaiduFaceApi", EntryPoint = "get_face_feature", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_face_feature(string file_name, ref int length);

    // 提取人脸特征值(传二进制图片buffer）
    [DllImport("BaiduFaceApi", EntryPoint = "get_face_feature_by_buf", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_face_feature_by_buf(byte[] buf, int size, ref int length);

    // 获取人脸特征值（传入opencv视频帧及人脸信息，适应于多人脸）
    [DllImport("BaiduFaceApi", EntryPoint = "get_face_feature_by_face", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_face_feature_by_face(IntPtr mat, ref TrackFaceInfo info, ref IntPtr feaptr);

    // 人脸1:1比对(传图片文件路径)
    [DllImport("BaiduFaceApi", EntryPoint = "match", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr match(string file_name1, string file_name2);

    // 人脸1:1比对（传二进制图片buffer）
    [DllImport("BaiduFaceApi", EntryPoint = "match_by_buf", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr match_by_buf(byte[] buf1, int size1, byte[] buf2, int size2);

    // 人脸1:1比对（传opencv视频帧）
    [DllImport("BaiduFaceApi", EntryPoint = "match_by_mat", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr match_by_mat(IntPtr img1, IntPtr img2);

    // 人脸1:1比对（传人脸特征值和二进制图片buffer)
    [DllImport("BaiduFaceApi", EntryPoint = "match_by_feature", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr match_by_feature(byte[] feature, int fea_len, byte[] buf2, int size2);

    // 特征值比对（传2个人脸的特征值）
    [DllImport("BaiduFaceApi", EntryPoint = "compare_feature", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern float compare_feature(byte[] f1, int f1_len, byte[] f2, int f2_len);

    // 1:N人脸识别（传图片文件路径和库里的比对）
    [DllImport("BaiduFaceApi", EntryPoint = "identify", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr identify(string image, string group_id_list, string user_id, int user_top_num = 1);

    // 1:N人脸识别（传图片二进制文件buffer和库里的比对）
    [DllImport("BaiduFaceApi", EntryPoint = "identify_by_buf", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr identify_by_buf(byte[] buf, int size, string group_id_list,
        string user_id, int user_top_num = 1);

    // 1:N人脸识别（传人脸特征值和库里的比对）
    [DllImport("BaiduFaceApi", EntryPoint = "identify_by_feature", CharSet = CharSet.Ansi
      , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr identify_by_feature(byte[] feature, int fea_len, string group_id_list,
        string user_id, int user_top_num = 1);

    // 提前加载库里所有数据到内存中
    [DllImport("BaiduFaceApi", EntryPoint = "load_db_face", CharSet = CharSet.Ansi
      , CallingConvention = CallingConvention.Cdecl)]
    public static extern bool load_db_face();

    // 1:N人脸识别（传人脸图片文件和内存已加载的整个库数据比对）
    [DllImport("BaiduFaceApi", EntryPoint = "identify_with_all", CharSet = CharSet.Ansi
      , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr identify_with_all(string image, int user_top_num = 1);

    // 1:N人脸识别（传人脸图片文件和内存已加载的整个库数据比对）
    [DllImport("BaiduFaceApi", EntryPoint = "identify_by_buf_with_all", CharSet = CharSet.Ansi
      , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr identify_by_buf_with_all(byte[] image, int size, int user_top_num = 1);

    // 1:N人脸识别（传人脸特征值和内存已加载的整个库数据比对）
    [DllImport("BaiduFaceApi", EntryPoint = "identify_by_feature_with_all", CharSet = CharSet.Ansi
      , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr identify_by_feature_with_all(byte[] feature, int fea_len, int user_top_num = 1);


    /// <summary>
    /// 1:N比较，传入图片文件二进制buffer和已加载的内存中整个库比较
    /// </summary>
    /// <param name="img_bytes"></param>
    /// <returns></returns>    
    public string IdentifyWithAllDB(byte[] img_bytes)
    {
        IntPtr ptr = identify_by_buf_with_all(img_bytes, img_bytes.Length);
        return Marshal.PtrToStringAnsi(ptr);
    }

    /// <summary>
    /// 加载数据库
    /// </summary>
    public void LoadDBFace()
    {
        load_db_face();
    }

    #region test
    // 测试获取人脸特征值(2048个byte）
    public void test_get_face_feature()
    {
        byte[] fea = new byte[2048];
        string file_name = Application.streamingAssetsPath + "/2.jpg";
        int len = 0;
        IntPtr ptr = get_face_feature(file_name, ref len);
        if (ptr == IntPtr.Zero)
        {
            Debug.Log("get face feature error");
        }
        else
        {
            if (len == 2048)
            {
                Debug.Log("get face feature success");
                Marshal.Copy(ptr, fea, 0, 2048);
                // 可保存特征值2048个字节的fea到文件中
                // FileUtil.byte2file("d:\\fea1.txt",fea, 2048);
            }
            else
            {
                Debug.Log("get face feature error");
            }
        }
    }
    // 测试获取人脸特征值(2048个byte）
    public void test_get_face_feature_by_buf()
    {
        byte[] fea = new byte[2048];
        //System.Drawing.Image img = System.Drawing.Image.FromFile("d:\\2.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        byte[] img_bytes = ImageUtil.GetImageBytes(Application.streamingAssetsPath + "/2.jpg");
        int len = 0;
        IntPtr ptr = get_face_feature_by_buf(img_bytes, img_bytes.Length, ref len);
        if (ptr == IntPtr.Zero)
        {
            Debug.Log("get face feature error");
        }
        else
        {
            if (len == 2048)
            {
                Debug.Log("get face feature success");
                Marshal.Copy(ptr, fea, 0, 2048);
                // 可保存特征值2048个字节的fea到文件中
                //  FileUtil.byte2file("d:\\fea2.txt",fea, 2048);
            }
            else
            {
                Debug.Log("get face feature error");
            }
        }
    }
    // 测试1:1比较，传入图片文件路径
    public void test_match()
    {
        string file1 = "D:\\img\\normal.jpg";
        string file2 = "D:\\img\\me1.jpg";
        for (int i = 1; i <= 5; i++)
        {
            file2 = "D:\\img\\me" + i + ".jpg";
            IntPtr ptr = match(file1, file2);
            string buf = Marshal.PtrToStringAnsi(ptr);
            Debug.Log(file2 + "match res is:" + buf);
        }
    }
    // 测试1:1比较，传入图片文件二进制buffer
    public void test_match_by_buf()
    {
        //System.Drawing.Image img1 = System.Drawing.Image.FromFile("d:\\444.bmp");
        //byte[] img_bytes1 = ImageUtil.img2byte(img1);
        byte[] img_bytes1 = ImageUtil.GetImageBytes(Application.streamingAssetsPath + "/1.jpg");
        //System.Drawing.Image img2 = System.Drawing.Image.FromFile("d:\\555.png");
        //byte[] img_bytes2 = ImageUtil.img2byte(img2);
        byte[] img_bytes2 = ImageUtil.GetImageBytes(Application.streamingAssetsPath + "/2.jpg");
        Debug.Log("IntPtr ptr = match_by_buf");
        IntPtr ptr = match_by_buf(img_bytes1, img_bytes1.Length, img_bytes2, img_bytes2.Length);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("match_by_buf res is:" + buf);
    }
    // 测试1:1比较，传入opencv视频帧
    public void test_match_by_mat()
    {
        //Mat img1 = Cv2.ImRead("d:\\444.bmp");
        //Mat img2 = Cv2.ImRead("d:\\555.png");
        //IntPtr ptr = match_by_mat(img1.CvPtr, img2.CvPtr);// img_bytes1, img_bytes1.Length, img_bytes2, img_bytes2.Length);
        //string buf = Marshal.PtrToStringAnsi(ptr);
        //Debug.Log("match_by_buf res is:" + buf);
    }
    // 测试根据特征值和图片二进制buf比较
    public void test_match_by_feature()
    {
        // 获取特征值2048个字节
        byte[] fea = new byte[2048];
        string file_name = Application.streamingAssetsPath + "/2.jpg";
        int len = 0;
        IntPtr ptr = get_face_feature(file_name, ref len);
        if (len != 2048)
        {
            Debug.Log("get face feature error!");
            return;
        }
        Marshal.Copy(ptr, fea, 0, 2048);
        // 获取图片二进制buffer
        //System.Drawing.Image img2 = System.Drawing.Image.FromFile("d:\\8.jpg");
        //byte[] img_bytes2 = ImageUtil.img2byte(img2);
        byte[] img_bytes2 = ImageUtil.GetImageBytes(Application.streamingAssetsPath + "/1.jpg");

        IntPtr ptr_res = match_by_feature(fea, fea.Length, img_bytes2, img_bytes2.Length);
        string buf = Marshal.PtrToStringAnsi(ptr_res);
        Debug.Log("match_by_feature res is:" + buf);

    }
    // 测试1:N比较，传入图片文件路径
    public /*void*/string test_identify(string str, string usr_grp, string usr_id)
    {
        string file1 = str;//"d:\\6.jpg";
        string user_group = usr_grp;//"test_group";
        string user_id = usr_id;//"test_user";
        IntPtr ptr = identify(file1, user_group, user_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("identify res is:" + buf);
        return buf;
    }
    // 测试1:N比较，传入图片文件二进制buffer
    public void test_identify_by_buf(string str, string usr_grp, string usr_id)
    {
        //System.Drawing.Image img = System.Drawing.Image.FromFile(str);//"d:\\2.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        byte[] img_bytes = ImageUtil.GetImageBytes(str);

        string user_group = usr_grp;//"test_group";
        string user_id = usr_id;// "test_user";
        IntPtr ptr = identify_by_buf(img_bytes, img_bytes.Length, user_group, user_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("identify_by_buf res is:" + buf);
    }
    // 测试1:N比较，传入图片文件二进制buffer
    public void test_identify_by_buf(byte[] img_bytes, string usr_grp, string usr_id)
    {
        string user_group = usr_grp;//"test_group";
        string user_id = usr_id;// "test_user";
        IntPtr ptr = identify_by_buf(img_bytes, img_bytes.Length, user_group, user_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("identify_by_buf res is:" + buf);
    }
    // 测试1:N比较，传入提取的人脸特征值
    public void test_identify_by_feature()
    {
        // 获取特征值2048个字节
        byte[] fea = new byte[2048];
        string file_name = Application.streamingAssetsPath + "/2.jpg";
        int len = 0;
        IntPtr ptr = get_face_feature(file_name, ref len);
        if (len != 2048)
        {
            Debug.Log("get face feature error!");
            return;
        }
        Marshal.Copy(ptr, fea, 0, 2048);

        string user_group = "test_group";
        string user_id = "test_user";
        IntPtr ptr_res = identify_by_feature(fea, fea.Length, user_group, user_id);
        string buf = Marshal.PtrToStringAnsi(ptr_res);
        Debug.Log("identify_by_feature res is:" + buf);
    }
    // 通过特征值比对（1:1）
    public void test_compare_feature()
    {
        // 获取特征值1   共2048个字节
        byte[] fea1 = new byte[2048];
        string file_name1 = Application.streamingAssetsPath + "/2.jpg";
        int len1 = 0;
        IntPtr ptr1 = get_face_feature(file_name1, ref len1);
        if (len1 != 2048)
        {
            Debug.Log("get face feature error!");
            return;
        }
        Marshal.Copy(ptr1, fea1, 0, 2048);

        // 获取特征值2   共2048个字节
        byte[] fea2 = new byte[2048];
        string file_name2 = Application.streamingAssetsPath + "/1.jpg";
        int len2 = 0;
        IntPtr ptr2 = get_face_feature(file_name2, ref len2);
        if (len2 != 2048)
        {
            Debug.Log("get face feature error!");
            return;
        }
        Marshal.Copy(ptr2, fea2, 0, 2048);
        // 比对
        float score = compare_feature(fea1, len1, fea2, len2);
        Debug.Log("compare_feature score is:" + score);
    }
    // 测试1:N比较，传入提取的人脸特征值和已加载的内存中整个库比较
    public void test_identify_by_feature_with_all()
    {
        // 加载整个数据库到内存中
        load_db_face();
        // 获取特征值2048个字节
        byte[] fea = new byte[2048];
        string file_name = Application.streamingAssetsPath + "/2.jpg";
        int len = 0;
        IntPtr ptr = get_face_feature(file_name, ref len);
        if (len != 2048)
        {
            Debug.Log("get face feature error!");
            return;
        }
        Marshal.Copy(ptr, fea, 0, 2048);
        IntPtr ptr_res = identify_by_feature_with_all(fea, fea.Length);
        string buf = Marshal.PtrToStringAnsi(ptr_res);
        Debug.Log("identify_by_feature_with_all res is:" + buf);
    }
    // 测试1:N比较，传入图片文件路径和已加载的内存中整个库比较
    public void test_identify_with_all()
    {
        // 加载整个数据库到内存中
        load_db_face();
        // 1:N
        string file1 = "D:\\img\\me1.jpg";
        IntPtr ptr = identify_with_all(file1);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("identify_with_all res is:" + buf);
    }
    // 测试1:N比较，传入图片文件二进制buffer和已加载的内存中整个库比较
    public void test_identify_by_buf_with_all()
    {
        // 加载整个数据库到内存中
        load_db_face();
        // 1:N
        //System.Drawing.Image img = System.Drawing.Image.FromFile("d:\\2.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        byte[] img_bytes = ImageUtil.GetImageBytes(Application.streamingAssetsPath + "/2.jpg");

        IntPtr ptr = identify_by_buf_with_all(img_bytes, img_bytes.Length);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("identify_by_buf_with_all res is:" + buf);
    }
    #endregion
}
[System.Serializable]
public class FaceIdentifyInfo
{
    public IdData data = new IdData();
    public int errno = 111;
    public string msg = "111";
}
[System.Serializable]
public class IdData
{
    public string face_token = "111";
    public string log_id = "111";
    public List<resultInfo> result = new List<resultInfo>();
    public int result_num = 111;
}
[System.Serializable]
public class resultInfo
{
    public string group_id = "111";
    public string score = "111";
    public string user_id = "111";
}