using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VipAnalyser.Web.Models
{
    public class CKPlayerJsonViewModel
    {
        public CKPlayerJsonViewModel()
        {
            video = new List<CKVideo>();
        }
        /// <summary>
        /// 是否自动播放
        /// </summary>
        public bool autoplay { get; set; }
        /// <summary>
        /// 视频地址
        /// </summary>
        public List<CKVideo> video { get; set; }
    }

    public class CKVideo
    {
        public CKVideo()
        {
            video = new List<CKVideoInfo>();
        }
        public List<CKVideoInfo> video { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 权重 权重最大的优先播放
        /// </summary>
        public int weight { get; set; }
        /// <summary>
        /// 清晰度名称
        /// </summary>
        public string definition { get; set; }
    }

    public class CKVideoInfo
    {
        /// <summary>
        /// 当前段  视频地址
        /// </summary>
        public string file { get; set; }
        /// <summary>
        /// 当前段 视频时长，单位：秒
        /// </summary>
        public double duration { get; set; }
        /// <summary>
        /// 当前段的文件大小，单位：字节
        /// </summary>
        public long bytesTotal { get; set; }
    }
}