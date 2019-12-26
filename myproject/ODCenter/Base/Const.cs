using System;
using System.Configuration;

namespace ODCenter.Base
{
    public static class Const
    {
        public static String Claim_Institute_Id = "insid";
        public static String Claim_Institute_Name = "insname";

        private static Boolean GetSetting(String key, Boolean def = false)
        {
            Boolean val;
            return Boolean.TryParse(ConfigurationManager.AppSettings[key], out val) ? val : def;
        }

        public static Boolean RegisterEnabled
        {
            get
            {
                return GetSetting("account:register", false);
            }
        }

        public static Boolean AccountEdit
        {
            get
            {
                return GetSetting("account:edit", false);
            }
        }

        public static Boolean InviteRequired
        {
            get
            {
                return GetSetting("account:invite", false);
            }
        }
    }
}