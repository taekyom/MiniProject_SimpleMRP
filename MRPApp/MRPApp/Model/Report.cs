using System;

namespace MRPApp.Model
{
    public class Report
    {
        /// <summary>
        /// 리포트를 위한 가상 테이블 클래스 생성
        /// </summary>
        public int SchIdx { get; set; }
        public string PlantCode { get; set; }
        public Nullable<int> SchAmount { get; set; }
        public System.DateTime PrcDate { get; set; }
        public Nullable<int> PrcOKAmount { get; set; }
        public Nullable<int> PrcFailAmount { get; set; }

    }
}
