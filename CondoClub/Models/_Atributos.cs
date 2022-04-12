using System;

namespace CondoClub.Web.Models {

    public class ExportExcel : Attribute {

        public enum ExportType {
            String,
            Numeric,
            DateOnly,
            TimeOnly,
            DateTime
        }

        public bool ExportToExcel { get; set; }

        public ExportType Type { get; set; }

        public int? ExportOrder { get; set; }

        public ExportExcel(bool ExportToExcel) :
            this(ExportToExcel, ExportType.String, null) {
        }

        public ExportExcel(bool ExportToExcel, int ExportOrder) :
            this(ExportToExcel, ExportType.String, ExportOrder) {
        }

        public ExportExcel(bool ExportToExcel, ExportType Type) :
            this(ExportToExcel, Type, null) {
        }

        public ExportExcel(bool ExportToExcel, ExportType Type, int? ExportOrder) {
            this.ExportToExcel = ExportToExcel;
            this.Type = Type;
            this.ExportOrder = ExportOrder;
        }

    }

}