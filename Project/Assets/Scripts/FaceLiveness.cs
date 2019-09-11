using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using OpenCvSharp;
//using OpenCVForUnity;
using UnityEngine;

// 活体检测
class FaceLiveness
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void FaceCallback(IntPtr bytes, int size, String res);
    // 单目RGB静默活体检测（传入图片文件路径)
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_liveness_check", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_liveness_check(string file_name);

    // 单目RGB静默活体检测（传入图片文件二进制buffer)
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_liveness_check_by_buf", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_liveness_check_by_buf(byte[] buf, int size);

    // 双目RGB和IR静默活体检测（传入二进制图片buffer)
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_ir_liveness_check_by_buf", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_ir_liveness_check_by_buf(byte[] rgb_buf, int rgb_size,
        byte[] ir_buf, int ir_size);

    // 双目RGB和DEPTH静默活体检测（传入二进制图片buffer)
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_depth_liveness_check_by_buf", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_depth_liveness_check_by_buf(byte[] rgb_buf, int rgb_size,
        byte[] depth_buf, int depth_size);

    // 双目RGB和IR静默活体检测（传入opencv视频帧)
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_ir_liveness_check", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_ir_liveness_check(IntPtr rgb_mat, IntPtr ir_mat, ref float rgb_score, ref float ir_score);

    // 双目RGB和IR静默活体检测（传入opencv视频帧，传出检测到人脸信息)
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_ir_liveness_check_faceinfo", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_ir_liveness_check_faceinfo(IntPtr rgb_mat, IntPtr ir_mat, ref float rgb_score,
        ref float ir_score, ref int face_size, ref long ir_time, IntPtr oFace);
    //双目深度(depth)静脉活体检测（传入opencv视频帧，传出检测到人脸信息）
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_depth_liveness_check_faceinfo", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_depth_liveness_check_faceinfo(IntPtr rgb_mat, IntPtr depth_mat,
       ref float rgb_score, ref float depth_score, ref int face_size, IntPtr oFace);

    //双目深度(depth)静脉活体检测（传入opencv视频帧）
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_depth_liveness_check", CharSet = CharSet.Ansi
        , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rgb_depth_liveness_check(IntPtr rgb_mat, IntPtr depth_mat, ref float rgb_score, ref float depth_score);

    // 双目RGB和DEPTH静默活体检测（sdk内部调用opencv，返回FaceCallback)适配奥比中光mini双目摄像头
    [DllImport("BaiduFaceApi", EntryPoint = "rgb_depth_liveness_check_by_orbe", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern int rgb_depth_liveness_check_by_orbe(FaceCallback callback);

    // 获取orbeCamera对象  奥比中光mini
    [DllImport("BaiduFaceApi", EntryPoint = "new_orbe", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr new_orbe();

    // 打开奥比中光mini
    [DllImport("BaiduFaceApi", EntryPoint = "open_orbe", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr open_orbe(IntPtr porbe, IntPtr cvRGBImg, IntPtr cvDepthImg);

    // 释放奥比中光mini
    [DllImport("BaiduFaceApi", EntryPoint = "orbe_release", CharSet = CharSet.Ansi
       , CallingConvention = CallingConvention.Cdecl)]
    private static extern void orbe_release(IntPtr orbe);

    // 测试单目RGB静默活体检测（传入图片文件路径)
    public void test_rgb_liveness_check()
    {
        // 传入图片文件绝对路径
        IntPtr ptr = rgb_liveness_check("d:\\2.jpg");
        string buf = Marshal.PtrToStringAnsi(ptr);
        Debug.Log("rgb_liveness_check res is:" + buf);
    }

    // 测试单目RGB静默活体检测（传入图片文件二进制buffer)
    public void test_rgb_liveness_check_by_buf()
    {
        // 传入图片文件绝对路径
        //System.Drawing.Image img = System.Drawing.Image.FromFile("d:\\2.jpg");
        //byte[] img_bytes = ImageUtil.img2byte(img);
        //IntPtr ptr = rgb_liveness_check_by_buf(img_bytes, img_bytes.Length);
        //string buf = Marshal.PtrToStringAnsi(ptr);
        //Debug.Log("rgb_liveness_check_by_buf res is:" + buf);
    }
    // 选择usb视频摄像头id,方法里面有获取连接的摄像头列表，包括id，名称和路径等
    public int select_usb_device_id()
    {
        //ArrayList capDevs = new ArrayList();
        //int device_id = 0;
        //try
        //{
        //    if (!File.Exists(Path.Combine(Environment.SystemDirectory, @"dpnhpast.dll")))
        //    {
        //        Debug.Log("DirectX NOT installed!");
        //        return -1;
        //    }
        //    if (!DevEnum.GetDevicesOfCat(FilterCategory.VideoInputDevice, out capDevs))
        //    {
        //        Debug.Log("No video capture devices found!");
        //        return -1;
        //    }
        //    if (capDevs.Count < 2)
        //    {
        //        Debug.Log("ir and rgb camera devices needed");
        //        return -1;
        //    }
        //    foreach (DsDevice d in capDevs)
        //    {
        //        Debug.Log("== VIDEO DEVICE (id:{0}) ==", d.id);
        //        Debug.Log("Name: {0}", d.Name);
        //        Debug.Log("Path: {0}", d.Path);
        //    }

        //    if (capDevs.Count > 0)
        //    {
        //        device_id = ((DsDevice)capDevs[0]).id;
        //        Debug.Log("select device id is:{0}", device_id);
        //    }
        //}
        //catch (Exception)
        //{
        //    if (capDevs != null)
        //    {
        //        foreach (DsDevice d in capDevs)
        //            d.Dispose();
        //        capDevs = null;
        //    }
        //    return -1;
        //}
        //return device_id;
        return 1;
    }

    // 双目RGB和IR静默活体检测（sdk内部调用opencv，返回FaceCallback)
    public bool rgb_ir_liveness_check_mat()
    {
        //int faceNum = 2;//传入的人脸数
        //int face_size = faceNum;//当前传入人脸数，传出人脸数
        //TrackFaceInfo[] track_info = new TrackFaceInfo[faceNum];
        //for (int i = 0; i < faceNum; i++)
        //{
        //    track_info[i] = new TrackFaceInfo();
        //    track_info[i].landmarks = new int[144];
        //    track_info[i].headPose = new float[3];
        //    track_info[i].face_id = 0;
        //    track_info[i].score = 0;
        //}
        //int sizeTrack = Marshal.SizeOf(typeof(TrackFaceInfo));
        //IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * faceNum);
        //long ir_time = 0;
        //// 序号0为电脑识别的usb摄像头编号，本demo中0为ir红外摄像头
        //// 不同摄像头和电脑识别可能有区别
        //// 编号一般从0-10   */            
        //int device = select_usb_device_id();
        //VideoCapture camera1 = VideoCapture.FromCamera(device);
        //if (!camera1.IsOpened())
        //{
        //    Debug.Log("camera1 open error");
        //    return false;
        //}

        //VideoCapture camera2 = VideoCapture.FromCamera(device + 1);
        //if (!camera2.IsOpened())
        //{
        //    Debug.Log("camera2 open error");
        //    return false;
        //}

        //RotatedRect box;
        //Mat frame1 = new Mat();
        //Mat frame2 = new Mat();
        //Mat rgb_mat = new Mat();
        //Mat ir_mat = new Mat();
        //var window_ir = new Window("ir_face");
        //var window_rgb = new Window("rgb_face");
        //while (true)
        //{
        //    camera1.Read(frame1);
        //    camera2.Read(frame2);
        //    if (!frame1.Empty() && !frame2.Empty())
        //    {
        //        if (frame1.Size(0) > frame2.Size(0))
        //        {
        //            rgb_mat = frame1;
        //            ir_mat = frame2;
        //        }
        //        else
        //        {
        //            rgb_mat = frame2;
        //            ir_mat = frame1;
        //        }
        //        float rgb_score = 0;
        //        float ir_score = 0;

        //        IntPtr ptr = rgb_ir_liveness_check_faceinfo(rgb_mat.CvPtr, ir_mat.CvPtr, ref rgb_score, ref ir_score, ref face_size, ref ir_time, ptT);
        //        string res = Marshal.PtrToStringAnsi(ptr);
        //        Debug.Log("res is:{0}", res);
        //        string msg_ir = "ir score is:" + ir_score.ToString();
        //        Cv2.PutText(ir_mat, msg_ir, new Point(20, 50), HersheyFonts.HersheyComplex, 1, new Scalar(255, 100, 0));
        //        window_ir.ShowImage(ir_mat);
        //        Cv2.WaitKey(1);
        //        Debug.Log("{0}", msg_ir);

        //        string msg_rgb = "rgb score is:" + rgb_score.ToString();
        //        Cv2.PutText(rgb_mat, msg_rgb, new Point(20, 50), HersheyFonts.HersheyComplex, 1, new Scalar(255, 100, 0));
        //        for (int index = 0; index < face_size; index++)
        //        {
        //            IntPtr ptrTrack = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
        //            track_info[index] = (TrackFaceInfo)Marshal.PtrToStructure(ptrTrack, typeof(TrackFaceInfo));
        //            Debug.Log("face_id is {0}:", track_info[index].face_id);
        //            Debug.Log("landmarks is:");
        //            for (int k = 0; k < 1; k++)
        //            {
        //                Debug.Log("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},",
        //                    track_info[index].landmarks[k], track_info[index].landmarks[k + 1],
        //                    track_info[index].landmarks[k + 2], track_info[index].landmarks[k + 3],
        //                    track_info[index].landmarks[k + 4], track_info[index].landmarks[k + 5],
        //                    track_info[index].landmarks[k + 6], track_info[index].landmarks[k + 7],
        //                    track_info[index].landmarks[k + 8], track_info[index].landmarks[k + 9]
        //                    );
        //            }

        //            for (int k = 0; k < track_info[index].headPose.Length; k++)
        //            {
        //                Debug.Log("angle is:{0:f}", track_info[index].headPose[k]);
        //            }
        //            Debug.Log("score is:{0:f}", track_info[index].score);
        //            // 角度
        //            Debug.Log("mAngle is:{0:f}", track_info[index].box.mAngle);
        //            // 人脸宽度
        //            Debug.Log("mWidth is:{0:f}", track_info[index].box.mWidth);
        //            // 中心点X,Y坐标
        //            Debug.Log("mCenter_x is:{0:f}", track_info[index].box.mCenter_x);
        //            Debug.Log("mCenter_y is:{0:f}", track_info[index].box.mCenter_y);
        //            // 画人脸框
        //            FaceTrack track = new FaceTrack();
        //            box = track.bounding_box(track_info[index].landmarks, track_info[index].landmarks.Length);
        //            track.draw_rotated_box(ref rgb_mat, ref box, new Scalar(0, 255, 0));
        //        }
        //        window_rgb.ShowImage(rgb_mat);
        //        Cv2.WaitKey(1);
        //        Debug.Log("{0}", msg_rgb);
        //    }
        //}
        //Marshal.FreeHGlobal(ptT);
        //rgb_mat.Release();
        //ir_mat.Release();
        //frame1.Release();
        //frame2.Release();
        //Cv2.DestroyWindow("ir_face");
        //Cv2.DestroyWindow("rgb_face");
        return true;
    }
    // 双目摄像头进行rgb,depth活体检测(此处适配了华杰艾米的双目摄像头)
    public bool rgb_depth_liveness_check_hjimi()
    {
        //int faceNum = 2;//传入的人脸数
        //int face_size = faceNum;//当前传入人脸数，传出人脸数
        //TrackFaceInfo[] track_info = new TrackFaceInfo[faceNum];
        //for (int i = 0; i < faceNum; i++)
        //{
        //    track_info[i] = new TrackFaceInfo();
        //    track_info[i].landmarks = new int[144];
        //    track_info[i].headPose = new float[3];
        //    track_info[i].face_id = 0;
        //    track_info[i].score = 0;
        //}
        //int sizeTrack = Marshal.SizeOf(typeof(TrackFaceInfo));
        //IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * faceNum);

        //RotatedRect box;

        //IntPtr phjimi = HjimiCamera.new_hjimi();
        //var rgb_win = new Window("rgb", WindowMode.AutoSize);
        //var depth_win = new Window("depth", WindowMode.Normal);
        //float rgb_score = 0;
        //float depth_score = 0;
        //Mat cv_depth = new Mat();
        //Mat cv_rgb = new Mat();
        //while (true)
        //{
        //    bool ok = HjimiCamera.open_hjimimat(phjimi, cv_rgb.CvPtr, cv_depth.CvPtr);
        //    if (!ok)
        //    {
        //        Debug.Log("open camera faile");
        //        continue;
        //    }
        //    if (cv_rgb.Empty())
        //    {
        //        continue;
        //    }
        //    if (cv_depth.Empty())
        //    {
        //        continue;
        //    }
        //    IntPtr resptr = rgb_depth_liveness_check_faceinfo(cv_rgb.CvPtr, cv_depth.CvPtr, ref rgb_score, ref depth_score, ref face_size, ptT);
        //    string res = Marshal.PtrToStringAnsi(resptr);
        //    Debug.Log("res is:{0}", res);

        //    for (int index = 0; index < face_size; index++)
        //    {
        //        IntPtr ptrTrack = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
        //        track_info[index] = (TrackFaceInfo)Marshal.PtrToStructure(ptrTrack, typeof(TrackFaceInfo));
        //        Debug.Log("in Liveness::usb_track face_id is {0}:", track_info[index].face_id);
        //        Debug.Log("landmarks is:");
        //        for (int k = 0; k < 1; k++)
        //        {
        //            Debug.Log("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},",
        //                track_info[index].landmarks[k], track_info[index].landmarks[k + 1],
        //                track_info[index].landmarks[k + 2], track_info[index].landmarks[k + 3],
        //                track_info[index].landmarks[k + 4], track_info[index].landmarks[k + 5],
        //                track_info[index].landmarks[k + 6], track_info[index].landmarks[k + 7],
        //                track_info[index].landmarks[k + 8], track_info[index].landmarks[k + 9]
        //                );
        //        }

        //        for (int k = 0; k < track_info[index].headPose.Length; k++)
        //        {
        //            Debug.Log("angle is:{0:f}", track_info[index].headPose[k]);
        //        }
        //        Debug.Log("score is:{0:f}", track_info[index].score);
        //        // 角度
        //        Debug.Log("mAngle is:{0:f}", track_info[index].box.mAngle);
        //        // 人脸宽度
        //        Debug.Log("mWidth is:{0:f}", track_info[index].box.mWidth);
        //        // 中心点X,Y坐标
        //        Debug.Log("mCenter_x is:{0:f}", track_info[index].box.mCenter_x);
        //        Debug.Log("mCenter_y is:{0:f}", track_info[index].box.mCenter_y);
        //        //// 画人脸框
        //        FaceTrack track = new FaceTrack();
        //        box = track.bounding_box(track_info[index].landmarks, track_info[index].landmarks.Length);
        //        track.draw_rotated_box(ref cv_rgb, ref box, new Scalar(0, 255, 0));
        //    }

        //    Mat depth_img = new Mat();
        //    cv_depth.ConvertTo(depth_img, MatType.CV_8UC1, 255.0 / 4500);
        //    string msg_depth = "depth score is:" + depth_score.ToString();
        //    Cv2.PutText(depth_img, msg_depth, new Point(20, 50), HersheyFonts.HersheyComplex, 1, new Scalar(255, 100, 0));

        //    string msg_rgb = "rgb score is:" + rgb_score.ToString();
        //    Cv2.PutText(cv_rgb, msg_rgb, new Point(20, 50), HersheyFonts.HersheyComplex, 1, new Scalar(255, 100, 0));

        //    rgb_win.ShowImage(cv_rgb);
        //    depth_win.ShowImage(depth_img);
        //    Cv2.WaitKey(1);
        //    depth_img.Release();
        //}
        //Marshal.FreeHGlobal(ptT);
        //cv_rgb.Release();
        //cv_depth.Release();
        //Cv2.DestroyWindow("rgb");
        //Cv2.DestroyWindow("depth");
        //HjimiCamera.hjimi_release(phjimi);
        return true;
    }
    //双目RGB和DEPTH静默活体检测（传入opencv视频帧)适配奥比中光mini双目摄像头
    public bool rgb_depth_liveness_check_orbe()
    {
        //int faceNum = 2;//传入的人脸数
        //int face_size = faceNum;//当前传入人脸数，传出人脸数
        //TrackFaceInfo[] track_info = new TrackFaceInfo[faceNum];
        //for (int i = 0; i < faceNum; i++)
        //{
        //    track_info[i] = new TrackFaceInfo();
        //    track_info[i].landmarks = new int[144];
        //    track_info[i].headPose = new float[3];
        //    track_info[i].face_id = 0;
        //    track_info[i].score = 0;
        //}
        //int sizeTrack = Marshal.SizeOf(typeof(TrackFaceInfo));
        //IntPtr ptT = Marshal.AllocHGlobal(sizeTrack * faceNum);

        //IntPtr pOrbe = new_orbe(); //与OrbeRelease成对出现
        //Mat rgb_mat = new Mat(480, 640, MatType.CV_8UC3);
        //Mat depth_mat = new Mat(480, 640, MatType.CV_16UC1);
        //float rgb_score = 0;
        //float depth_score = 0;
        //var window_depth = new Window("depth_face");
        //var window_rgb = new Window("rgb_face");
        //while (true)
        //{
        //    RotatedRect box;
        //    open_orbe(pOrbe, rgb_mat.CvPtr, depth_mat.CvPtr);
        //    Debug.Log("rgb_mat rows {0} depth_mat rows {1}", rgb_mat.Rows, depth_mat.Rows);
        //    if (!rgb_mat.Empty() && !depth_mat.Empty())
        //    {
        //        IntPtr resptr = rgb_depth_liveness_check_faceinfo(rgb_mat.CvPtr, depth_mat.CvPtr, ref rgb_score, ref depth_score, ref face_size, ptT);
        //        string res = Marshal.PtrToStringAnsi(resptr);
        //        Debug.Log("res is:{0}", res);

        //        for (int index = 0; index < face_size; index++)
        //        {
        //            IntPtr ptrTrack = (IntPtr)(ptT.ToInt64() + sizeTrack * index);
        //            track_info[index] = (TrackFaceInfo)Marshal.PtrToStructure(ptrTrack, typeof(TrackFaceInfo));
        //            Debug.Log("in Liveness::usb_track face_id is {0}:", track_info[index].face_id);
        //            Debug.Log("landmarks is:");
        //            for (int k = 0; k < 1; k++)
        //            {
        //                Debug.Log("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},",
        //                    track_info[index].landmarks[k], track_info[index].landmarks[k + 1],
        //                    track_info[index].landmarks[k + 2], track_info[index].landmarks[k + 3],
        //                    track_info[index].landmarks[k + 4], track_info[index].landmarks[k + 5],
        //                    track_info[index].landmarks[k + 6], track_info[index].landmarks[k + 7],
        //                    track_info[index].landmarks[k + 8], track_info[index].landmarks[k + 9]
        //                    );
        //            }

        //            for (int k = 0; k < track_info[index].headPose.Length; k++)
        //            {
        //                Debug.Log("angle is:{0:f}", track_info[index].headPose[k]);
        //            }
        //            Debug.Log("score is:{0:f}", track_info[index].score);
        //            // 角度
        //            Debug.Log("mAngle is:{0:f}", track_info[index].box.mAngle);
        //            // 人脸宽度
        //            Debug.Log("mWidth is:{0:f}", track_info[index].box.mWidth);
        //            // 中心点X,Y坐标
        //            Debug.Log("mCenter_x is:{0:f}", track_info[index].box.mCenter_x);
        //            Debug.Log("mCenter_y is:{0:f}", track_info[index].box.mCenter_y);
        //            //// 画人脸框
        //            FaceTrack track = new FaceTrack();
        //            box = track.bounding_box(track_info[index].landmarks, track_info[index].landmarks.Length);
        //            track.draw_rotated_box(ref rgb_mat, ref box, new Scalar(0, 255, 0));
        //        }
        //    }
        //    string msg_rgb = "rgb score is:" + rgb_score.ToString();
        //    Cv2.PutText(rgb_mat, msg_rgb, new Point(20, 50), HersheyFonts.HersheyComplex, 1, new Scalar(255, 100, 0));
        //    window_rgb.ShowImage(rgb_mat);
        //    //Cv2.ImShow("rgb_face", rgb_mat);

        //    string msg_depth = "depth score is:" + depth_score.ToString();
        //    Cv2.PutText(depth_mat, msg_depth, new Point(20, 50), HersheyFonts.HersheyComplex, 1, new Scalar(255, 255, 255));
        //    window_depth.ShowImage(depth_mat);
        //    //Cv2.ImShow("depth_face", depth_mat);
        //    int c = Cv2.WaitKey(2);
        //    if (27 == c) break;
        //}
        //orbe_release(pOrbe);//与new_orbe成对出现
        //Marshal.FreeHGlobal(ptT);
        //rgb_mat.Release();
        //depth_mat.Release();
        //Cv2.DestroyWindow("depth_face");
        //Cv2.DestroyWindow("rgb_face");
        return true;
    }
    public void test_rgb_ir_liveness_check_by_opencv()
    {
        //FaceCallback callback =
        //(bytes, buf_len, res_out) =>
        //{
        //    Debug.Log("in call back");
        //    if (buf_len > 0)
        //    {
        //        byte[] b = new byte[buf_len];
        //        Marshal.Copy(bytes, b, 0, buf_len);
        //        // ImageUtil.byte2img(b, buf_len,buf_len+"test.jpg");
        //        Debug.Log("callback result is {0} and {1} and {2}", bytes, buf_len, res_out);
        //    }
        //};
        rgb_ir_liveness_check_mat();
    }

    // 双目RGB和DEPTH静默活体检测,适配奥比中光mini双目摄像头
    public void test_rgb_depth_liveness_check_by_orbe()
    {
        //FaceCallback callback =
        //(bytes, buf_len, res_out) =>
        //{
        //    Debug.Log("in call back");
        //    if (buf_len > 0)
        //    {
        //        byte[] b = new byte[buf_len];
        //        Marshal.Copy(bytes, b, 0, buf_len);
        //        // 返回的byte可保存为图片
        //        // ImageUtil.byte2img(b, buf_len,buf_len+"test.jpg");
        //        Debug.Log("callback result is {0} and {1} and {2}", bytes, buf_len, res_out);
        //    }
        //};

        //rgb_depth_liveness_check_by_orbe(callback);
        rgb_depth_liveness_check_orbe();
    }
    // 双目摄像头进行rgb,depth活体检测(此处适配了华杰艾米的双目摄像头)
    public void test_rgb_depth_liveness_check_by_hjimi()
    {
        rgb_depth_liveness_check_hjimi();
    }
}
