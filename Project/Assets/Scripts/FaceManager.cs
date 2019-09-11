using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

class FaceManager
{
    // 人脸注册(传入图片文件路径)
    [DllImport("BaiduFaceApi", EntryPoint = "user_add", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr user_add(string user_id, string group_id, string file_name,
        string user_info = "");

    // 人脸注册(传入图片二进制buffer)
    [DllImport("BaiduFaceApi", EntryPoint = "user_add_by_buf", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr user_add_by_buf(string user_id, string group_id, byte[] image,
       int size, string user_info = "");

    // 人脸注册(传入特征值)
    [DllImport("BaiduFaceApi", EntryPoint = "user_add_by_feature", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr user_add_by_feature(string user_id, string group_id, byte[] fea,
       int fea_len, string user_info = "");

    // 人脸更新(传入图片文件路径)
    [DllImport("BaiduFaceApi", EntryPoint = "user_update", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr user_update(string user_id, string group_id, string file_name,
        string user_info = "");

    // 人脸更新(传入图片二进制buffer)
    [DllImport("BaiduFaceApi", EntryPoint = "user_update_by_buf", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr user_update_by_buf(string user_id, string group_id, byte[] image,
       int size, string user_info = "");

    // 人脸删除
    [DllImport("BaiduFaceApi", EntryPoint = "user_face_delete", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr user_face_delete(string user_id, string group_id, string face_token);

    // 用户删除
    [DllImport("BaiduFaceApi", EntryPoint = "user_delete", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr user_delete(string user_id, string group_id);

    // 组添加
    [DllImport("BaiduFaceApi", EntryPoint = "group_add", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr group_add(string group_id);

    // 组删除
    [DllImport("BaiduFaceApi", EntryPoint = "group_delete", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr group_delete(string group_id);

    // 查询用户信息
    [DllImport("BaiduFaceApi", EntryPoint = "get_user_info", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_user_info(string user_id, string group_id);

    // 用户组列表查询
    [DllImport("BaiduFaceApi", EntryPoint = "get_user_list", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_user_list(string group_id, int start = 0, int length = 100);

    // 组列表查询
    [DllImport("BaiduFaceApi", EntryPoint = "get_group_list", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_group_list(int start = 0, int length = 100);

    // 人脸注册(传入二进制图片buffer)
    public string FaceRegister(string user_id, string group_id, byte[] img_bytes)
    {
        string user_info = "user_info";
        IntPtr ptr = user_add_by_buf(user_id, group_id, img_bytes, img_bytes.Length, user_info);
        return Marshal.PtrToStringAnsi(ptr);
    }
    // 组删除
    public void GroupDelete(string group_id)
    {
        IntPtr ptr = group_delete(group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("group_delete res is:" + buf);
        UpdateText(buf);
    }
    // 查询用户信息
    public string GetUserInfo(string user_id, string group_id)
    {
        IntPtr ptr = get_user_info(user_id, group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        //Debug.Log("get_user_info res is:" + buf);
        return buf;
    }
    // 用户组列表查询
    public string GetUserList(string group_id)
    {
        IntPtr ptr = get_user_list(group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        //Debug.Log("get_user_list res is:" + buf);
        return buf;
    }

    #region test
    // 测试人脸注册
    public void test_user_add()
    {
        // 人脸注册
        string user_id = "RHY";
        string group_id = "Male";
        string file_name = "D:\\img\\test_me.png";//41494a99b518845ae4a1581470477b68
        //facetoken为ef4dbbc91849c0c3667969eaec077915
        string user_info = "user_info";
        IntPtr ptr = user_add(user_id, group_id, file_name, user_info);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("user_add res is:" + buf);
        UpdateText(buf);
    }
    // 测试人脸注册(传入二进制图片buffer)
    public void test_user_add_by_buf()
    {
        // 人脸注册
        string user_id = "ZhaoLiYing";
        string group_id = "Female";
        //System.Drawing.Image img = System.Drawing.Image.FromFile("d:\\4.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        byte[] img_bytes = ImageUtil.GetImageBytes("D:\\img\\3zhaoliying.png");
        string user_info = "user_info";
        IntPtr ptr = user_add_by_buf(user_id, group_id, img_bytes, img_bytes.Length, user_info);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("user_add_by_buf res is:" + buf);
        UpdateText(buf);
    }
    // 测试人脸注册(传入特征值)
    public void test_user_add_by_feature()
    {
        // 人脸注册
        string user_id = "ZhaoLiYing";
        string group_id = "Female";
        // 传入人脸特征值，提取特征值demo，可参考FaceCompare文件中
        byte[] feature = new byte[2048];
        string user_info = "user_info";
        IntPtr ptr = user_add_by_feature(user_id, group_id, feature, feature.Length, user_info);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("user_add_by_feature res is:" + buf);
        UpdateText(buf);
    }
    // 测试人脸更新
    public void test_user_update()
    {
        string user_id = "ZhaoLiYing";
        string group_id = "Female";
        string file_name = "D:\\img\\1tangyan.jpg";
        string user_info = "user_info";
        IntPtr ptr = user_update(user_id, group_id, file_name, user_info);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("user_update res is:" + buf);
        UpdateText(buf);
    }
    // 测试人脸更新(传入二进制图片buffer)
    public void test_user_update_by_buf()
    {
        // 人脸更新
        string user_id = "ZhaoLiYing";
        string group_id = "Female";
        //System.Drawing.Image img = System.Drawing.Image.FromFile("d:\\8.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        byte[] img_bytes = ImageUtil.GetImageBytes(Application.streamingAssetsPath + "/8.jpg");
        string user_info = "user_info";
        IntPtr ptr = user_update_by_buf(user_id, group_id, img_bytes, img_bytes.Length, user_info);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("user_update_by_buf res is:" + buf);
        UpdateText(buf);
    }
    // 测试人脸删除
    public void test_user_face_delete()
    {
        string user_id = "ZhaoLiYing";
        string group_id = "Female";
        string face_token = "ef4dbbc91849c0c3667969eaec077915";
        IntPtr ptr = user_face_delete(user_id, group_id, face_token);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("user_face_delete res is:" + buf);
        UpdateText(buf);
    }
    // 测试用户删除
    public void test_user_delete()
    {
        string user_id = "ZhaoLiYing";
        string group_id = "Female";
        IntPtr ptr = user_delete(user_id, group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("user_delete res is:" + buf);
        UpdateText(buf);
    }
    // 组添加
    public void test_group_add()
    {
        string group_id = "Male";
        IntPtr ptr = group_add(group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("group_add res is:" + buf);
        UpdateText(buf);
    }
    // 组删除
    public void test_group_delete()
    {
        string group_id = "test_group";
        IntPtr ptr = group_delete(group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("group_delete res is:" + buf);
        UpdateText(buf);
    }
    // 查询用户信息
    public string test_get_user_info()
    {
        string user_id = "ZhaoLiYing";
        string group_id = "Female";
        IntPtr ptr = get_user_info(user_id, group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        //Debug.Log("get_user_info res is:" + buf);
        return buf;
    }
    // 用户组列表查询
    public string test_get_user_list()
    {
        string group_id = "Female";
        IntPtr ptr = get_user_list(group_id);
        string buf = Marshal.PtrToStringAnsi(ptr);
        //Debug.Log("get_user_list res is:" + buf);
        return buf;
    }
    // 组列表查询
    public string test_get_group_list()
    {
        IntPtr ptr = get_group_list();
        string buf = Marshal.PtrToStringAnsi(ptr);
        //Debug.Log("get_group_list res is:" + buf);
        return buf;
    }
    public void UpdateText(string buf)
    {
        //backInfo.text = buf;
        //groupInfo.text = test_get_group_list();
        //userInfo.text = test_get_user_list();
        //faceInfo.text = test_get_user_info();
    }
    #endregion
}