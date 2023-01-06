using System.Reflection;

namespace FfMpeg.Models
{
    [Obfuscation(Exclude = true)]
    internal class AudibleCliProfile
    {
        public Website_Cookies website_cookies { get; set; }
        public string adp_token { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string device_private_key { get; set; }
        public Store_Authentication_Cookie store_authentication_cookie { get; set; }
        public Device_Info device_info { get; set; }
        public Customer_Info customer_info { get; set; }
        public float expires { get; set; }
        public string locale_code { get; set; }
        public bool with_username { get; set; }
        public string activation_bytes { get; set; }

        [Obfuscation(Exclude = true)]

        public class Website_Cookies
        {
            public string sessionid { get; set; }
            public string ubidacbuk { get; set; }
            public string xacbuk { get; set; }
            public string atacbuk { get; set; }
            public string sessatacbuk { get; set; }
        }

        [Obfuscation(Exclude = true)]
        public class Store_Authentication_Cookie
        {
            public string cookie { get; set; }
        }

        [Obfuscation(Exclude = true)]
        public class Device_Info
        {
            public string device_name { get; set; }
            public string device_serial_number { get; set; }
            public string device_type { get; set; }
        }

        [Obfuscation(Exclude = true)]
        public class Customer_Info
        {
            public string account_pool { get; set; }
            public string user_id { get; set; }
            public string home_region { get; set; }
            public string name { get; set; }
            public string given_name { get; set; }
        }
    }
}
