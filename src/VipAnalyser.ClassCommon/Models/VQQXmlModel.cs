using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VipAnalyser.ClassCommon.Models
{
    [XmlRoot(ElementName = "root")]
    public class VQQXmlModel
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

        [XmlElement("fi")]
        public List<fi> fi { get; set; }
    }
    public class fi
    {
        public string id { get; set; }

        public string name { get; set; }

        public string lmt { get; set; }

        public string sb { get; set; }

        public string cname { get; set; }

        public string br { get; set; }

        public string drm { get; set; }

        public string video { get; set; }

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
        public string br { get; set; }

        public string ch { get; set; }

        public cl cl { get; set; }

        public string ct { get; set; }

        public string dm { get; set; }

        public string drm { get; set; }

        public string dsb { get; set; }

        public string enc { get; set; }

        public string fclip { get; set; }

        public string fmd5 { get; set; }

        public string fn { get; set; }

        public string fps { get; set; }

        public string fs { get; set; }

        public string fst { get; set; }

        public string fvkey { get; set; }

        public string head { get; set; }

        public string hevc { get; set; }

        public string iflag { get; set; }

        public string level { get; set; }

        public string lnk { get; set; }

        public string logo { get; set; }

        public string mst { get; set; }

        public pl pl { get; set; }

        public string share { get; set; }

        public string sp { get; set; }

        public string st { get; set; }

        public string tail { get; set; }

        public string targetid { get; set; }

        public string td { get; set; }

        public string ti { get; set; }

        public string type { get; set; }

        public ul ul { get; set; }

        public string vh { get; set; }

        public string vid { get; set; }

        public string videotype { get; set; }

        public string vr { get; set; }

        public string vst { get; set; }

        public string vw { get; set; }

        public string wh { get; set; }

        public string wl { get; set; }
    }
    public class cl
    {
        public string fc { get; set; }

        [XmlElement("ci")]
        public List<ci> ci { get; set; }
    }
    public class ci
    {
        public string idx { get; set; }

        public string cs { get; set; }

        public string cd { get; set; }

        public string cmd5 { get; set; }

        public string keyid { get; set; }
    }
    public class pl
    {
        public string cnt { get; set; }

        [XmlElement("pd")]
        public List<pd> pd { get; set; }
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
        [XmlElement("ui")]
        public List<ui> ui { get; set; }
    }
    public class ui
    {
        public string url { get; set; }

        public string vt { get; set; }

        public string dtc { get; set; }

        public string dt { get; set; }
    }

}
