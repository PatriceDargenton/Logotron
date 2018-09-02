
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogotronLib
{
    public sealed class clsInitTirage
    {
        public List<int> lstNumSegmentDejaTires;
        public List<string> lstSegmentsDejaTires;
        public List<string> lstSensSegmentDejaTires;
        public List<string> lstUnicitesSegmentDejaTires;

        public clsInitTirage()
        {
            this.lstNumSegmentDejaTires = new List<int>();
            this.lstSegmentsDejaTires = new List<string>();
            this.lstSensSegmentDejaTires = new List<string>();
            this.lstUnicitesSegmentDejaTires = new List<string>();
        }

        public clsInitTirage(clsSegmentBase segment)
        {
            this.lstNumSegmentDejaTires = new List<int>();
            this.lstSegmentsDejaTires = new List<string>();
            this.lstSensSegmentDejaTires = new List<string>();
            this.lstUnicitesSegmentDejaTires = new List<string>();
            if (segment == null) throw new ArgumentNullException("segment");
            this.lstNumSegmentDejaTires.Add(segment.iNumSegment);
            this.lstSegmentsDejaTires.Add(segment.sSegment);
            this.lstSensSegmentDejaTires.Add(segment.sSens);
            if (segment.sUnicite.Length > 0)
                this.lstUnicitesSegmentDejaTires.Add(segment.sUnicite);
        }
    }
}
