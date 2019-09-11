using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Face : MonoBehaviour
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void FaceCallback(IntPtr bytes, int size, String res);

    // sdk初始化
    [DllImport("BaiduFaceApi", EntryPoint = "sdk_init", CharSet = CharSet.Ansi
         , CallingConvention = CallingConvention.Cdecl)]
    private static extern int sdk_init(bool id_card);

    // 是否授权
    [DllImport("BaiduFaceApi", EntryPoint = "is_auth", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
    private static extern bool is_auth();

    // 获取设备指纹
    [DllImport("BaiduFaceApi", EntryPoint = "get_device_id", CharSet = CharSet.Ansi
             , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_device_id();

    // sdk销毁
    [DllImport("BaiduFaceApi", EntryPoint = "sdk_destroy", CharSet = CharSet.Ansi
         , CallingConvention = CallingConvention.Cdecl)]
    private static extern void sdk_destroy();

    private FaceTrack faceTrack;
    private FaceManager faceManager;
    private FaceCompare faceCompare;

    [SerializeField] RawImage videoImg;
    [SerializeField] string groupId;
    [SerializeField] string userId;
    [SerializeField] int deviceId;
    [SerializeField] float loginCheckIntervalTime;
    [SerializeField] float loginPassScore;

    public Text loginInfo;
    public Text registerInfo;

    void Start()
    {
        InitFaceIDSDK();

        faceTrack = new FaceTrack();
        faceManager = new FaceManager();
        faceCompare = new FaceCompare();

        LoadDBFace();
        StartFaceTrack(deviceId);
    }

    /// <summary>
    /// 初始化人脸识别SDK
    /// </summary>
    void InitFaceIDSDK()
    {
        bool id = false;

        int n = sdk_init(id); //sdk初始化
        if (n != 0)
            Debug.Log("初始化失败" + n);
        else
            Debug.Log("初始化成功" + n);
        // 测试是否授权
        bool authed = is_auth();
        Debug.Log("授权结果：" + authed);

        IntPtr ptr = get_device_id();
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("id获取成功，为：" + buf);
    }

    /// <summary>
    /// 加载人脸库数据在内存中
    /// </summary>
    void LoadDBFace()
    {
        faceCompare.LoadDBFace();
    }

    /// <summary>
    /// 开始人脸追踪
    /// </summary>
    /// <param name="deviceId">设备Id，自带摄像头为0，外置摄像头为1</param>
    void StartFaceTrack(int deviceId)
    {
        StartCoroutine(faceTrack.IStartFaceTrack(deviceId, videoImg, faceManager, faceCompare));
    }

    /// <summary>
    /// 开始登录
    /// </summary>
    public void BeginLogin()
    {
        faceTrack.CanLogin();
        videoImg.gameObject.SetActive(false);
        StartCoroutine(faceTrack.ILogin(loginCheckIntervalTime, loginPassScore,
            (result) =>
            {
                loginInfo.text = result;
            },
            (userid) =>
            {
                Debug.Log("用户" + userid + "登录成功");
                EndTrack();
            }));
        Invoke("ShowImage", 0.06f);
    }

    /// <summary>
    /// 开始注册
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="onComplete"></param>
    public void BeginRegister()
    {
        faceTrack.CanRegister();
        videoImg.gameObject.SetActive(false);
        StartCoroutine(faceTrack.IRegister(userId, groupId, 1f,
            (result) =>
            {
                registerInfo.text = result;
            },
            (userid, groupid) =>
            {
                Debug.Log("组" + groupid + "用户" + userid + "注册成功");
                EndTrack();
                LoadDBFace();
            }));
        Invoke("ShowImage", 0.06f);
    }

    /// <summary>
    /// 延迟显示视频流（因为视频还处于上一次暂停的状态）
    /// </summary>
    void ShowImage()
    {
        videoImg.gameObject.SetActive(true);
    }

    /// <summary>
    /// 结束检测
    /// </summary>
    void EndTrack()
    {
        faceTrack.EndTrack();
        GetComponent<BtnPanelManager>().ShowBackButton();
        //sdk_destroy();
    }

    private void OnDestroy()
    {
        sdk_destroy();
    }
}
