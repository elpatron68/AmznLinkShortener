using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AmznLinkShortener
{
    class AmznShorten
    {
        public static string ShortenUrl(string url)
        {
            // https://smile.amazon.de/KIWI-Design-aktualisierte-verbesserten-Komfort/dp/B098JTDPQC/ref=pd_sbs_2/257-3571497-0813564?pd_rd_w=jxTmk&pf_rd_p=93dc29b0-720a-4b5a-b099-a3d0ce40f79e&pf_rd_r=3BHWECJWBE7J68XZ7RGJ&pd_rd_r=b66e8ae6-e11d-4863-81ed-278911a431d9&pd_rd_wg=Fhpa2&pd_rd_i=B098JTDPQC&psc=1
            // https://smile.amazon.de/CYCPLUS-Fahrradcomputer-wasserdichte-Kilometerz%C3%A4hler-Hintergrundbeleuchtung/dp/B08B5QX5LM/ref=pd_sbs_4/257-3571497-0813564?pd_rd_w=3Xu1K&pf_rd_p=93dc29b0-720a-4b5a-b099-a3d0ce40f79e&pf_rd_r=N46VWMWW3BDP2H7GCP6N&pd_rd_r=dea843cb-71c8-4d1e-81f5-ea126679280a&pd_rd_wg=I2uh7&pd_rd_i=B08B5QX5LM&psc=1
            string pattern1 = @"https\:\/\/[a-z]+\.amazon\.[a-z]{2,3}";
            string pattern2 = @"B[A-Z0-9]{9}";
            try
            {
                string asin = Regex.Matches(url, pattern2)[0].Value;
                url = Regex.Matches(url, pattern1)[0].Value + "/dp/" + asin;
            }
            catch
            {
                Debug.WriteLine("Parsing url failed");
            }
            return url;
        }
        public static bool IsAmazonLongUrl(string url)
        {
            string pattern1 = @"https\:\/\/[a-z]+\.amazon\.[a-z]{2,3}";
            string pattern2 = @"B[A-Z0-9]{9}";
            if (Regex.Matches(url, pattern2).Count > 0 && Regex.Matches(url, pattern1).Count > 0 && url.Length > 40)
            {
                Debug.WriteLine("Long Amazon url found");
                return true;
            }
            else
            {
                Debug.WriteLine("No long Amazon url found");
                return false;
            }
        }
    }
}
