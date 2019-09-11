using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using OpenCvSharp;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.Collections;

// 人脸检测
class FaceTrack
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FaceCallback(IntPtr bytes, int size, String res);

    [DllImport("BaiduFaceApi", EntryPoint = "track", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr track(string file_name, int max_track_num);
    /*  trackMat
     *  传入参数: maxTrackObjNum:检测到的最大人脸数，传入外部分配人脸数，需要分配对应的内存大小。
     *            传出检测到的最大人脸数
     *    返回值: 传入的人脸数 和 检测到的人脸数 中的最小值。
     ****/
    [DllImport("BaiduFaceApi", EntryPoint = "track_mat", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    public static extern int track_mat(IntPtr oface, IntPtr mat, ref int max_track_num);

    [DllImport("BaiduFaceApi", EntryPoint = "track_max_face", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr track_max_face(string file_name);

    [DllImport("BaiduFaceApi", EntryPoint = "track_by_buf", CharSet = CharSet.Ansi
      , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr track_by_buf(byte[] image, int size, int max_track_num);

    [DllImport("BaiduFaceApi", EntryPoint = "track_max_face_by_buf", CharSet = CharSet.Ansi
      , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr track_max_face_by_buf(byte[] image, int size);

    [DllImport("BaiduFaceApi", EntryPoint = "clear_tracked_faces", CharSet = CharSet.Ansi
     , CallingConvention = CallingConvention.Cdecl)]
    private static extern void clear_tracked_faces();

    // 测试人脸检测(直接传入图片路径)
    public void test_track()
    {
        // 传入图片文件绝对路径
        string file_name = "D:\\img\\1yangmi.jpg";
        int max_track_num = 1; // 设置最多检测人数（多人脸检测），默认为1，最多可设为10
        IntPtr ptr = track(file_name, max_track_num);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("检测结果为：\n" + buf);
    }
    // 测试最大人脸检测(直接传入图片路径)
    public void test_track_max_face()
    {
        // 传入图片文件绝对路径
        string file_name = "D:\\img\\1yangmi.jpg";
        IntPtr ptr = track_max_face(file_name);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("track_max_face res is:" + buf);
    }
    // 测试人脸检测(传入图片二进制信息)
    public void test_track_by_buf()
    {
        //System.Drawing.Image img = System.Drawing.Image.FromFile("d:\\3.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        byte[] img_bytes = ImageUtil.GetImageBytes("D:\\img\\1yangmi.jpg");
        int max_track_num = 2; // 设置最多检测人数（多人脸检测），默认为1，最多可设为10
        IntPtr ptr = track_by_buf(img_bytes, img_bytes.Length, max_track_num);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("track_by_buf res is:" + buf);
    }
    // 测试最大人脸检测(传入图片二进制信息)
    public void test_track_max_face_by_buf()
    {
        //System.Drawing.Image img = System.Drawing.Image.FromFile("d:\\2.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        byte[] img_bytes = ImageUtil.GetImageBytes("D:\\img\\1yangmi.jpg");
        IntPtr ptr = track_max_face_by_buf(img_bytes, img_bytes.Length);
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("track_max_face_by_buf res is:" + buf);
    }

    //人脸图片
    Mat image;
    //当前帧人脸信息
    TrackFaceInfo trackFaceInfo;
    //摄像头中心
    float xcenter;
    float ycenter;
    FaceManager faceManager;
    FaceCompare faceCompare;
    bool isRegisterSuccess;
    bool isLoginSuccess;
    Texture2D videoTexture;
    bool isCheck = false;
    bool isRegisterCheck = false;
    bool isLoginCheck = false;
    byte[] imgBytes;

    public void setCheck(bool _isCheck)
    {
        isCheck = _isCheck;
    }
    public void resetMat()
    {
        image = new Mat();
        videoTexture = null;
    }

    public IEnumerator IStartFaceTrack(int dev, RawImage img, FaceManager _faceManager, FaceCompare _faceCompare)
    {
        faceManager = _faceManager;
        faceCompare = _faceCompare;
        image = new Mat();
        using (VideoCapture cap = VideoCapture.FromCamera(dev))
        {
            if (!cap.IsOpened())
            {
                Debug.LogError("open camera error");
                yield break;
            }
            // When the movie playback reaches end, Mat.data becomes NULL.
            while (true)
            {
                yield return null;
                if (isCheck)
                {
                    RotatedRect box;
                    cap.Read(image); // same as cvQueryFrame
                    if (!image.Empty())
                    {
                        int ilen = 2;//传入的人脸数
                        TrackFaceInfo[] track_info = new TrackFaceInfo[ilen];
                        for (int i = 0; i < ilen; i++)
                        {
                            track_info[i] = new TrackFaceInfo();
                            track_info[i].landmarks = new int[144];
                            track_info[i].headPose = new float[3];
                            track_info[i].face_id = 0;
                            track_info[i].score = 0;
                        }
                        int sizeTrack = Marshal.SizeOf(typeof(TrackFaceInfo));
                        IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * ilen);
                        /*  trackMat
                         *  传入参数: maxTrackObjNum:检测到的最大人脸数，传入外部分配人脸数，需要分配对应的内存大小。
                         *            传出检测到的最大人脸数
                         *    返回值: 传入的人脸数 和 检测到的人脸数 中的最小值,实际返回的人脸。
                         ****/
                        int faceSize = ilen;//返回人脸数  分配人脸数和检测到人脸数的最小值
                        int curSize = ilen;//当前人脸数 输入分配的人脸数，输出实际检测到的人脸数
                        faceSize = track_mat(ptT, image.CvPtr, ref curSize);
                        for (int index = 0; index < faceSize; index++)
                        {
                            IntPtr ptr = new IntPtr();
                            if (8 == IntPtr.Size)
                            {
                                ptr = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
                            }
                            else if (4 == IntPtr.Size)
                            {
                                ptr = (IntPtr)(ptT.ToInt32() + sizeTrack * index);
                            }

                            track_info[index] = (TrackFaceInfo)Marshal.PtrToStructure(ptr, typeof(TrackFaceInfo));
                            trackFaceInfo = track_info[index];
                            {
                                //face_info[index] = (FaceInfo)Marshal.PtrToStructure(info_ptr, typeof(FaceInfo));
                                //Debug.Log("in Liveness::usb_track face_id is {0}:" + track_info[index].face_id);
                                //Debug.Log("in Liveness::usb_track landmarks is:");
                                //for (int k = 0; k < 1; k++)
                                //{
                                //    Debug.Log(
                                //        track_info[index].landmarks[k + 0] + "," + track_info[index].landmarks[k + 1] + "," +
                                //        track_info[index].landmarks[k + 2] + "," + track_info[index].landmarks[k + 3] + "," +
                                //        track_info[index].landmarks[k + 4] + "," + track_info[index].landmarks[k + 5] + "," +
                                //        track_info[index].landmarks[k + 6] + "," + track_info[index].landmarks[k + 7] + "," +
                                //        track_info[index].landmarks[k + 8] + "," + track_info[index].landmarks[k + 9]
                                //        );
                                //}
                                //for (int k = 0; k < track_info[index].headPose.Length; k++)
                                //{
                                //    Debug.Log("in Liveness::usb_track angle is:" + track_info[index].headPose[k]);
                                //}
                                //Debug.Log("in Liveness::usb_track score is:" + track_info[index].score);
                                //// 角度
                                //Debug.Log("in Liveness::usb_track mAngle is:" + track_info[index].box.mAngle);
                                //// 人脸宽度
                                //Debug.Log("in Liveness::usb_track mWidth is:" + track_info[index].box.mWidth);
                                //// 中心点X,Y坐标
                                //Debug.Log("in Liveness::usb_track mCenter_x is:" + track_info[index].box.mCenter_x);
                                //Debug.Log("in Liveness::usb_track mCenter_y is:" + track_info[index].box.mCenter_y);
                            }
                            // 画人脸框
                            box = bounding_box(track_info[index].landmarks, track_info[index].landmarks.Length);
                            draw_rotated_box(ref image, ref box, new Scalar(0, 255, 0));
                            xcenter = image.Width / 2;
                            ycenter = image.Height / 2;
                        }
                        Marshal.FreeHGlobal(ptT);
                        if (videoTexture == null)
                        {
                            videoTexture = new Texture2D(image.Width, image.Height);
                        }
                        videoTexture.LoadImage(image.ToBytes());
                        videoTexture.Apply();
                        img.texture = videoTexture;
                        //imgBytes = image.ToBytes();
                        Cv2.WaitKey(1);
                    }
                    else
                    {

                    }
                }
            }
            image.Release();
        }
    }

    public IEnumerator IRegister(string userid, string groupid, float time, Action<string> onProgress, Action<string, string> onComplete)
    {
        yield return new WaitForSeconds(0.1f);
        while (isCheck && isRegisterCheck)
        {
            //必须是人脸,且人脸位置必须在正中央偏移不超过30像素
            if (!isRegisterSuccess && trackFaceInfo.score >= 0.999f && Mathf.Abs(trackFaceInfo.box.mCenter_x - xcenter) <= 30 && Mathf.Abs(trackFaceInfo.box.mCenter_y - ycenter) <= 30)
            {
                string result = faceManager.FaceRegister(userid, groupid, image.ToBytes());
                FaceIdentifyInfo info = JsonUtility.FromJson<FaceIdentifyInfo>(result);
                if (onProgress != null)
                {
                    onProgress.Invoke(result);
                }
                //是否注册成功
                if (info.errno == 0)
                {
                    Debug.Log(info.data.face_token);
                    isRegisterSuccess = true;
                    isRegisterCheck = false;
                    if (onComplete != null)
                    {
                        onComplete.Invoke(userid, groupid);
                    }
                    yield break;
                }
            }
            yield return new WaitForSeconds(time);
        }
    }

    public IEnumerator ILogin(float time, float okScore, Action<string> onProgress, Action<string> onComplete)
    {
        yield return new WaitForSeconds(0.1f);
        //Dictionary<string, int> userIdCounts = new Dictionary<string, int>();
        //int crtCheckCount = 0;
        while (isCheck && isLoginCheck)
        {
            if (!isLoginSuccess && trackFaceInfo.score >= 0.999f && Mathf.Abs(trackFaceInfo.box.mCenter_x - xcenter) <= 30 && Mathf.Abs(trackFaceInfo.box.mCenter_y - ycenter) <= 30)//首先必须是人脸
            {
                string result = faceCompare.IdentifyWithAllDB(image.ToBytes());
                FaceIdentifyInfo info = JsonUtility.FromJson<FaceIdentifyInfo>(result);
                if (onProgress != null)
                {
                    onProgress.Invoke(result);
                }
                //是否找到相似脸
                if (info.errno == 0)
                {
                    float score = float.Parse(info.data.result[0].score);
                    //相似脸分数是否达标
                    if (score > okScore)
                    {
                        //crtCheckCount++;
                        //if (!userIdCounts.ContainsKey(info.data.result[0].user_id))
                        //{
                        //    userIdCounts.Add(info.data.result[0].user_id, 1);
                        //}
                        //else
                        //{
                        //    userIdCounts[info.data.result[0].user_id] += 1;
                        //}
                        //if (crtCheckCount>= ConfigReader.Instance.fConfig.loginCheckCount)
                        //{
                        //    int max = 0;
                        //    string key = "";
                        //    foreach (var item in userIdCounts)
                        //    {
                        //        if (item.Value > max)
                        //        {
                        //            max = item.Value;
                        //            key = item.Key;
                        //        }
                        //    }
                        Debug.Log(info.data.result[0].group_id + ";" + info.data.result[0].user_id + ";" + info.data.result[0].score);
                        isLoginSuccess = true;
                        isLoginCheck = false;
                        if (onComplete != null)
                        {
                            //正常应该是得到一个Json字符串，内含groupid和userid，传入callback参数
                            onComplete.Invoke(info.data.result[0].user_id/*key*/);
                        }
                        yield break;
                        //}
                    }
                }
            }
            yield return new WaitForSeconds(time);
        }
    }

    public RotatedRect bounding_box(int[] landmarks, int size)
    {
        int min_x = 1000000;
        int min_y = 1000000;
        int max_x = -1000000;
        int max_y = -1000000;
        for (int i = 0; i < size / 2; ++i)
        {
            min_x = (min_x < landmarks[2 * i] ? min_x : landmarks[2 * i]);
            min_y = (min_y < landmarks[2 * i + 1] ? min_y : landmarks[2 * i + 1]);
            max_x = (max_x > landmarks[2 * i] ? max_x : landmarks[2 * i]);
            max_y = (max_y > landmarks[2 * i + 1] ? max_y : landmarks[2 * i + 1]);
        }
        int width = ((max_x - min_x) + (max_y - min_y)) / 2;
        float angle = 0;
        Point2f center = new Point2f((min_x + max_x) / 2, (min_y + max_y) / 2);
        return new RotatedRect(center, new Size2f(width, width), angle);
    }
    public void draw_rotated_box(ref Mat img, ref RotatedRect box, Scalar color)
    {
        Point2f[] vertices = new Point2f[4];
        vertices = box.Points();
        for (int j = 0; j < 4; j++)
        {
            Cv2.Line(img, vertices[j], vertices[(j + 1) % 4], color);
        }
    }

    public void CanRegister()
    {
        resetMat();
        setCheck(true);
        isLoginCheck = false;
        isRegisterCheck = true;
        isRegisterSuccess = false;
    }

    public void CanLogin()
    {
        resetMat();
        setCheck(true);
        isLoginCheck = true;
        isRegisterCheck = false;
        isLoginSuccess = false;
    }

    public void EndTrack()
    {
        resetMat();
        setCheck(false);
        isLoginCheck = false;
        isRegisterCheck = false;
        isRegisterSuccess = false;
        isLoginSuccess = false;
    }


    #region test
    // 清除跟踪的人脸信息
    public void test_clear_tracked_faces()
    {
        clear_tracked_faces();
        Debug.Log("清除所有跟踪的人脸");
    }
    #endregion
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FaceInfo
{
    public FaceInfo(float iWidth, float iAngle, float iCenter_x, float iCenter_y, float iConf)
    {
        mWidth = iWidth;
        mAngle = iAngle;
        mCenter_x = iCenter_x;
        mCenter_y = iCenter_y;
        mConf = iConf;
    }
    public float mWidth;     // rectangle width
    public float mAngle;//; = 0.0F;     // rectangle tilt angle [-45 45] in degrees
    public float mCenter_y;// = 0.0F;  // rectangle center y
    public float mCenter_x;// = 0.0F;  // rectangle center x
    public float mConf;// = 0.0F;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TrackFaceInfo
{
    [MarshalAs(UnmanagedType.Struct)]
    public FaceInfo box;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 144)]
    public int[] landmarks;// = new int[144];
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] headPose;// = new float[3];
    public float score;// = 0.0F;
    public UInt32 face_id;// = 0;
}