using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Web.Common
{
    public class BootstrapModel
    {
        public string ID { get; set; }
        public string AreaLabeledId { get; set; }
        public ModalSize Size { get; set; }
        public string Message { get; set; }
        public ModalStyle Style { get; set; }
        public string ModalSizeClass
        {
            get
            {
                switch (this.Size)
                {
                    case ModalSize.Small:
                        return "modal-sm";
                    case ModalSize.Large:
                        return "modal-lg";
                    case ModalSize.Medium:
                    default:
                        return "";
                }
            }
        }

        public string ModalStyleClass
        {
            get
            {
                switch (this.Style)
                {
                    case ModalStyle.Success:
                        return "modal-notify modal-success";
                    case ModalStyle.Info:
                        return "modal-notify modal-info";
                    case ModalStyle.Warning:
                        return "modal-notify modal-warning";
                    case ModalStyle.Danger:
                        return "modal-notify modal-danger";
                    default:
                        return "";
                }
            }
        }
    }


    public enum ModalSize
    {
        Small,
        Large,
        Medium
    }

    public enum ModalStyle
    {
        None,
        Success,
        Info,
        Danger,
        Warning
    }
}
