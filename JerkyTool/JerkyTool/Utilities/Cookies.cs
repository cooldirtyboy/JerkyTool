using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Web;
using System.Net;
using JerkyTool;


namespace JerkyTool.Utilities
{
    public class Cookies
    {
        public static bool SetCookie(string cookiename, string cookievalue)
        {
            
           
            return SetCookie(cookiename, cookievalue, string.Empty);
        }

        public static bool SetCookie(string cookiename, int iDaysToExpire)
        {

            return SetCookie(cookiename, "", "", iDaysToExpire);
        }

        public static bool SetCookie(string cookiename, string cookievalue, string cookiearrayname)
        {
            return SetCookie(cookiename, cookievalue, cookiearrayname, DefaultDomain);
        }

        public static bool SetCookie(string cookiename, string cookievalue, string cookiearrayname, string cookiedomain)
        {
            return SetCookie(cookiename, cookievalue, cookiearrayname, cookiedomain, 0);
        }

        public static bool SetCookie(string cookiename, string cookievalue, string cookiearrayname, int iDaysToExpire)
        {
            return SetCookie(cookiename, cookievalue, cookiearrayname, DefaultDomain, iDaysToExpire);
        }

        //public static bool SetCookie(string cookiename, string cookievalue, string cookiedomain)
        //{
        //    return SetCookie(cookiename, cookievalue, cookiedomain, 0);  // set iDaysToExpire = 0 , cookie valid only in session
        //}

        public static bool SetCookie(string cookiename, string cookievalue, string cookiearrayname, string cookiedomain, int iDaysToExpire)
        {
            try
            {
                //HttpCookie objCookie = new HttpCookie(cookiename);
                //System.Web.HttpContext.Current.Response.Cookies.Clear();
                //System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                //objCookie.Values.Add(cookiename, cookievalue);

                //HttpCookie aCookie = new HttpCookie("userInfo");
                //aCookie.Values["userName"] = "patrick";
                //aCookie.Values["lastVisit"] = DateTime.Now.ToString();
                //aCookie.Expires = DateTime.Now.AddDays(1);
                //Response.Cookies.Add(aCookie);


                //HttpResponse Response = HttpContext.Current.Response;
                //HttpRequest Request = HttpContext.Current.Request;

                //HttpCookie aCookie ;

                // aCookie = Request.Cookies[cookiename];

                // if (null == aCookie)
                //{
                //   if (cookievalue == string.Empty)                   
                //       aCookie = new HttpCookie(cookiename,"notset");                             
                //}

                if (cookievalue == string.Empty)
                    cookievalue = "";



                if (cookiearrayname == string.Empty)
                {
                    //System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie(cookiename, cookievalue));

                    HttpCookie aCookie;
                    aCookie = System.Web.HttpContext.Current.Request.Cookies[cookiename];

                    if (null == aCookie)
                    {
                        if (cookievalue != string.Empty)
                        {
                            aCookie = new HttpCookie(cookiename, cookievalue);
                            if (cookiedomain != string.Empty && cookiedomain.ToLower() != "localhost")
                                aCookie.Domain = cookiedomain;

                            if (iDaysToExpire > 0)
                            {
                                DateTime dtExpiry = DateTime.Now.AddDays(iDaysToExpire);
                                aCookie.Expires = dtExpiry;
                            }

                            System.Web.HttpContext.Current.Response.Cookies.Add(aCookie);

                            return true;
                        }
                    }
                    else
                        System.Web.HttpContext.Current.Response.Cookies[cookiename].Value = cookievalue;

                }
                else
                    System.Web.HttpContext.Current.Response.Cookies[cookiename][cookiearrayname] = cookievalue;

                if (iDaysToExpire > 0)
                {
                    DateTime dtExpiry = DateTime.Now.AddDays(iDaysToExpire);
                    System.Web.HttpContext.Current.Response.Cookies[cookiename].Expires = dtExpiry;
                }

                if (cookiedomain != string.Empty && cookiedomain.ToLower() != "localhost")
                    System.Web.HttpContext.Current.Response.Cookies[cookiename].Domain = cookiedomain;


                //if (cookiearrayname != string.Empty)
                ////    System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie(cookiename, cookievalue));
                ////else
                //   aCookie.Values[cookiearrayname] = cookievalue;

                //if (iDaysToExpire > 0)
                //{
                //    DateTime dtExpiry = DateTime.Now.AddDays(iDaysToExpire);
                //     aCookie.Expires = dtExpiry;
                //}

                //if (cookiedomain != string.Empty && cookiedomain.ToLower() != "localhost")
                //     aCookie.Domain = cookiedomain;

                //Response.Cookies.Add(aCookie);

            }
            catch //(Exception e)
            {
                return false;
            }


            return true;
        }

        public static bool SetCookie(string cookiename, string cookievalue, string cookiearrayname, string cookiedomain, DateTime DaysToExpire)
        {
            try
            {
                //HttpCookie objCookie = new HttpCookie(cookiename);
                //System.Web.HttpContext.Current.Response.Cookies.Clear();
                //System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                //objCookie.Values.Add(cookiename, cookievalue);
                if (cookievalue == string.Empty)
                    cookievalue = "";

                HttpResponse Response = HttpContext.Current.Response;

                if (cookiearrayname == string.Empty)
                    Response.Cookies.Add(new HttpCookie(cookiename, cookievalue));
                else
                    Response.Cookies[cookiename][cookiearrayname] = cookievalue;

                //if (DaysToExpire <> DateTime.No)
                //{
                //DateTime dtExpiry = DateTime.Now.AddDays(iDaysToExpire);
                System.Web.HttpContext.Current.Response.Cookies[cookiename].Expires = DaysToExpire;
                //}

                if (cookiedomain != string.Empty && cookiedomain.ToLower() != "localhost")
                    System.Web.HttpContext.Current.Response.Cookies[cookiename].Domain = cookiedomain;

            }
            catch //(Exception e)
            {
                return false;
            }
            return true;
        }

        public static string GetCookie(string cookiename)
        {

            return GetCookie(cookiename, string.Empty);
        }

        public static string GetCookie(string cookiename, string key)
        {
            string cookyval = string.Empty;
            try
            {
                //if (key != string.Empty)
                //    cookyval = System.Web.HttpContext.Current.Request.Cookies[cookiename][key];
                //else
                //    cookyval = System.Web.HttpContext.Current.Request.Cookies[cookiename].Value;

                if (System.Web.HttpContext.Current.Request.Cookies[cookiename] != null)
                {
                    if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Cookies[cookiename].Value))
                    {
                        if (key != string.Empty)
                            cookyval = System.Web.HttpContext.Current.Request.Cookies[cookiename][key].ToString();
                        else
                            cookyval = System.Web.HttpContext.Current.Request.Cookies[cookiename].Value.ToString();
                    }
                }
                //else
                //    cookyval = string.Empty;
            }
            catch //(Exception e)
            {
                cookyval = null;
            }
            return (cookyval != null ? cookyval.ToString() : "");
        }

        #region Detect if Cookie Enable
        public static bool Detect()
        {
            #region How to Used -- PS. Copy and Paste Coding below . Note Remember unremark it...
            //			bool iscookie;
            //
            //			iscookie = Detect() ;
            //
            //			if(iscookie)
            //			{
            //				DateTime dtNow = DateTime.Now;
            //				HttpCookieCollection cookies = Request.Cookies;
            //				HttpCookie cookie = cookies["_ytra"];
            //
            //				try
            //				{
            //					string str = cookies["_ytra"].Value;
            //				}
            //				catch
            //				{
            //					HttpCookie newcookie = new HttpCookie("_ytra");
            //					newcookie.Expires = DateTime.Now.AddHours(24); /* cookie expire after 24hr.*/
            //					Response.Cookies.Add(newcookie);
            //					Response.Redirect("http://promos.domain.com/intercept/default.asp?ru=" + Request.Url.AbsoluteUri); /* remarks this line to disable intercept */
            //				}
            //			}
            #endregion

            const string cookie_name = "_ytra";
            const string querystring_name = "q";

            bool _isenabled = false;
            
            // Need this to display properly in designer
            if (HttpContext.Current == null) return false;

            // Get Session and Request objects
            HttpRequest Request = HttpContext.Current.Request;
            HttpResponse Response = HttpContext.Current.Response;

            // Perform redirect test
            if (Request.Form.Count == 0 && Request.Cookies[cookie_name] == null && Request.QueryString[querystring_name] == null)
            {
                string Q = Request.QueryString.ToString();
                if (Q.Length > 0) { Q = "?" + Q + "&"; }
                else { Q = "?"; }
                Q += querystring_name + "=true";
                Response.Cookies.Add(new HttpCookie(cookie_name, "_o"));
                Response.Cookies["_ytra"].Expires = DateTime.Parse("01/10/2030");
                if (!string.IsNullOrEmpty(DefaultDomain) && DefaultDomain.ToLower() != "localhost")
                    Response.Cookies["_ytra"].Domain = DefaultDomain;
                Response.Redirect(Request.Path + Q);
            }

            if (Request.Cookies[cookie_name] != null) _isenabled = true;

            return _isenabled;
        }
        #endregion

        public static bool SetSessions()
        {
            string s;
            bool iscookie = false;

            s = GetCookie("_ytrs", "s");

            if (string.IsNullOrEmpty(s))
                iscookie = SetCookie("_ytrs", Guid.NewGuid().ToString(), "s", DefaultDomain, DateTime.Parse("01/10/2030"));

            return iscookie;
        }


        public static string GetSessions()
        {
            string s;


            s = GetCookie("_ytrs", "s");

            if (string.IsNullOrEmpty(s))
                return string.Empty;
            else
                return s;
        }


        public static bool SetLanguage(string language)
        {
            string s;
            bool iscookie = false;

            s = GetCookie("_ylang", "lang");

            //if (string.IsNullOrEmpty(s))
            iscookie = SetCookie("_ylang", language, "lang", DefaultDomain, DateTime.Parse("01/10/2030"));

            return iscookie;
        }

        public static string GetLanguage()
        {
            string s;


            s = GetCookie("_ylang", "lang");

            if (string.IsNullOrEmpty(s))
                return string.Empty;
            else
                return s;
        }

        /// <summary>
        /// Get Default Domain from the web.config
        /// </summary>
        public static string DefaultDomain
        {
            get
            {
                //var domain = Properties.Settings.Default.DefaultDomain;
                var domain = string.Empty;
                return string.IsNullOrEmpty(domain) == true  ?  string.Empty : domain  ;                
            }
        }


    }
}
