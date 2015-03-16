namespace HotelGolfBooking
{
    using System;
    using System.Web;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Security.Cryptography;

    /// <summary>
    /// Summary description for Vpc
    /// </summary>
    public class Payment
    {
        private String secureSecret;
        private String virtualPaymentClientUrl;
        private Boolean validSecureHash;
        private Boolean emptySecureSecret;

        public String SecureSecret
        {
            get
            {
                return secureSecret;
            }
            set
            {
                secureSecret = value;
            }
        }

        public String VirtualPaymentClientUrl
        {
            get
            {
                return virtualPaymentClientUrl;
            }
            set
            {
                virtualPaymentClientUrl = value;
            }
        }

        public Boolean isValidsecureHash()
        {
            return this.validSecureHash;
        }

        public Boolean isEmptysecureSecret()
        {
            return this.emptySecureSecret;
        }

        public String getRedirectUrl(Hashtable parameters)
        {
            String vpcUrl = this.virtualPaymentClientUrl + "?";
            String md5HashData = this.secureSecret;

            ArrayList keys = new ArrayList(parameters.Keys);
            Object[] keyArray = keys.ToArray();
            Array.Sort(keyArray, StringComparer.Ordinal);
            int appendFlag = 0;
            foreach (Object obj in keyArray)
            {
                String key = (String)obj;
                String value = (String)parameters[key];
                if (value.Length > 0)
                {
                    if (appendFlag == 0)
                    {
                        vpcUrl += HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value);
                        appendFlag = 1;
                    }
                    else
                    {
                        vpcUrl += "&" + HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value);
                    }
                    md5HashData += value;
                }
            }

            Byte[] originalBytes;
            StringBuilder sb = new StringBuilder();
            MD5 md5 = new MD5CryptoServiceProvider();
            originalBytes = UTF8Encoding.UTF8.GetBytes(md5HashData);

            foreach (Byte b in md5.ComputeHash(originalBytes))
                sb.Append(b.ToString("x2").ToUpper());

            string checksum = sb.ToString();
            vpcUrl += "&vpc_SecureHash=" + checksum;
            return vpcUrl;
        }

        public void checkSum(Hashtable parameters)
        {
            String md5HashData = this.secureSecret;
            if (this.secureSecret.Length > 0)
            {
                ArrayList keys = new ArrayList(parameters.Keys);
                Object[] keyArray = keys.ToArray();
                Array.Sort(keyArray, StringComparer.Ordinal);
                foreach (String key in keyArray)
                {
                    String value = (String)parameters[key];
                    if (!key.Equals("vpc_SecureHash") && value.Length > 0)
                    {
                        md5HashData += value;
                    }
                }

                Byte[] originalBytes;
                StringBuilder sb = new StringBuilder();
                MD5 md5 = new MD5CryptoServiceProvider();
                originalBytes = UTF8Encoding.UTF8.GetBytes(md5HashData);

                foreach (Byte b in md5.ComputeHash(originalBytes))
                    sb.Append(b.ToString("x2").ToUpper());

                String checksum = sb.ToString();
                String secureHash = (String)parameters["vpc_SecureHash"];
                if (checksum.ToUpper().Equals(secureHash))
                {
                    validSecureHash = true;
                }
                else
                {
                    validSecureHash = false;
                }
            }
            else
            {
                emptySecureSecret = true;
            }
        }
    }
}