﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon.Models
{
    public class VideoAnalyseResponse : BaseResponse
    {
        public VideoAnalyseResponse()
        {
            Data = new List<VideoInfo>();
        }


        public List<VideoInfo> Data { get; set; }
    }


    public class VideoInfo
    {
        public VideoInfo()
        {
            Part = new List<PartInfo>();
        }


        public string Name { get; set; }

        public List<PartInfo> Part { get; set; }

        public int PartCount { get; set; }

    }

    public class PartInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
    }


}