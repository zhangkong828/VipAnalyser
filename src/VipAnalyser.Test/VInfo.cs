using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.Test
{
    public class VInfo
    {
        public string dltype { get; set; }

        public string exem { get; set; }

        public fl fl { get; set; }

        public string fp2p { get; set; }

        public string hs { get; set; }

        public string ip { get; set; }

        public string ls { get; set; }

        public string preview { get; set; }

        public string s { get; set; }

        public sfl sfl { get; set; }

        public string tm { get; set; }

        public vl vl { get; set; }
    }
    public class fl
    {
        public string cnt { get; set; }

        public fi fi { get; set; }
    }
    public class fi
    {
        public int id { get; set; }

        public string name { get; set; }

        public string lmt { get; set; }

        public string sb { get; set; }

        public string cname { get; set; }

        public string br { get; set; }

        public string drm { get; set; }

        public int video { get; set; }

        public string fs { get; set; }

        public string sl { get; set; }
    }
    public class sfl
    {
        public string cnt { get; set; }
    }
    public class vl
    {
        public string cnt { get; set; }

        public vi vi { get; set; }
    }
    public class vi
    {
        public string @base { get; set; }

        public string br { get; set; }

        public string ch { get; set; }

        public string ct { get; set; }

        public string dm { get; set; }

        public string drm { get; set; }

        public string dsb { get; set; }

        public string enc { get; set; }

        public string fc { get; set; }

        public string fmd5 { get; set; }

        public string fn { get; set; }

        public string fps { get; set; }

        public string fs { get; set; }

        public string fst { get; set; }

        public string head { get; set; }

        public string hevc { get; set; }

        public string hfs { get; set; }

        public string iflag { get; set; }

        public int keyid { get; set; }

        public string lnk { get; set; }

        public string logo { get; set; }

        public string mst { get; set; }

        public pl pl { get; set; }

        public string share { get; set; }

        public string st { get; set; }

        public string tail { get; set; }

        public int targetid { get; set; }

        public string td { get; set; }

        public string ti { get; set; }

        public string type { get; set; }

        public ul ul { get; set; }

        public string vh { get; set; }

        public int vid { get; set; }

        public int videotype { get; set; }

        public string vr { get; set; }

        public string vst { get; set; }

        public string vw { get; set; }

        public string wh { get; set; }

        public wl wl { get; set; }
    }
    public class pl
    {
        public string cnt { get; set; }

        public pd pd { get; set; }
    }
    public class pd
    {
        public string cd { get; set; }

        public string h { get; set; }

        public string w { get; set; }

        public string r { get; set; }

        public string c { get; set; }

        public string fmt { get; set; }

        public string fn { get; set; }

        public string url { get; set; }
    }
    public class ul
    {
        public ui ui { get; set; }
    }
    public class ui
    {
        public string url { get; set; }

        public string vt { get; set; }

        public string dtc { get; set; }

        public string dt { get; set; }

        public hls hls { get; set; }
    }
    public class hls
    {
        public string et { get; set; }

        public string fbw { get; set; }

        public string ftype { get; set; }

        public string hk { get; set; }

        public pnl pnl { get; set; }

        public string st { get; set; }

        public string stype { get; set; }

        public string pname { get; set; }

        public string pt { get; set; }
    }
    public class pnl
    {
        public pi pi { get; set; }
    }
    public class pi
    {
        public string bw { get; set; }

        public string fc { get; set; }

        public string fn { get; set; }
    }
    public class wl
    {
        public wi wi { get; set; }
    }
    public class wi
    {
        public int id { get; set; }

        public string x { get; set; }

        public string y { get; set; }

        public string w { get; set; }

        public string h { get; set; }

        public string a { get; set; }

        public string md5 { get; set; }

        public string url { get; set; }

        public string surl { get; set; }
    }

}
