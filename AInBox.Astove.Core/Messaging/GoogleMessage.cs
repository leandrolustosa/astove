using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Messaging
{
    /**
    *   More information in:
    *   https://developers.google.com/cloud-messaging/http-server-ref#table1"
    **/

    public class GoogleMessage
    {
        public string to { get; set; }
        public string[] registration_ids { get; set; }
        public string collapse_key { get; set; }
        public string priority { get; set; }
        public bool content_available { get; set; }
        public string restricted_package_name { get; set; }
        public bool dry_run { get; set; }
        public int? time_to_live { get; set; }
        public string delay_while_idle { get; set; }
        public GoogleData data { get; set; }
        public GoogleNotification notification { get; set; }
    }

    public class GoogleData
    {   
    }

    public class GoogleNotification
    {
        public string title { get; set; }
        public string body { get; set; }
        public string icon { get; set; }
        public string sound { get; set; }
        public string badge { get; set; }
        public string tag { get; set; }
        public string color { get; set; }
        public string click_action { get; set; }
        public string body_loc_key { get; set; }
        public string[] body_loc_args { get; set; }
        public string title_loc_key { get; set; }
        public string[] title_loc_args { get; set; }
    }
}
