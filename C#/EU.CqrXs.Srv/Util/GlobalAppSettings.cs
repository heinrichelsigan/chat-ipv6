﻿using Area23.At.Framework.Library.Static;
using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace EU.CqrXs.Srv.Util
{
    [Serializable]
    public class GlobalAppSettings
    {
        public String pictureUrl = Constants.URL_PIC;
        public string prefixUrl = Constants.URL_PREFIX;
        public Uri prefixUri = null;
        public Uri pictureUri = null;
        public CultureInfo systemLocale, locale;
        // private DIALOGS dialogOpened = DIALOGS.None;
        private static HttpContext context;
        private HttpSessionState session;
        private static HttpApplicationState application;
        private static HttpApplication sApplication;

        #region properties

        public Uri PictureUri { get { InitPictureUrl(); return this.pictureUri; } }

        public String PictureUrl
        {
            get { InitPictureUrl(); return this.pictureUrl; }
            set
            {
                try
                {
                    this.pictureUri = new Uri(value);
                    this.pictureUrl = value;
                }
                catch (Exception exi)
                {
                    Console.Error.WriteLine(exi.StackTrace);
                }
            }
        }

        public String PrefixUrl { get { InitPrefixUrl(); return this.prefixUrl; } }

        public Uri PrefixUri { get { InitPrefixUrl(); return this.prefixUri; } }


        public CultureInfo Locale { get { InitLocale(); return locale; } set => locale = value; }

        public CultureInfo SystemLLocale { get { InitLocale(); return systemLocale; } }

        public String LocaleString { get => Locale.DisplayName; set => locale = new CultureInfo(value); }

        public String TwoLetterISOLanguageName { get => Locale.TwoLetterISOLanguageName; }
        
        public Exception LastException { get; set; }

        #endregion properties

        public GlobalAppSettings()
        {

        }

        /// <summary>
        /// constructor for GlobalAppSettings
        /// </summary>
        /// <param name="app">HttpApplication</param>
        /// <param name="c">HttpContext</param>
        public GlobalAppSettings(HttpApplication app, HttpContext c, HttpApplicationState haps, HttpSessionState hses)
        {
            sApplication = app;
            context = c;
            application = haps;
            this.session = hses;            
        }

        public GlobalAppSettings(HttpContext c, HttpApplicationState haps, HttpSessionState hses)
        {
            context = c;
            application = haps;
            this.session = hses;
        }

        public GlobalAppSettings(HttpApplicationState haps, HttpSessionState hses)
        {
            application = haps;
            this.session = hses;
        }

        public GlobalAppSettings(HttpContext c, HttpSessionState hses)
        {
            context = c;
            this.session = hses;
        }

        public GlobalAppSettings(HttpSessionState hses)
        {
            this.session = hses;
        }


        #region members

        public HttpContext getContext()
        {
            if (context == null)
                context = getApplication().Context;
            
            return context;
        }

        public HttpApplicationState getAppState()
        {
            if (context == null) context = getApplication().Context;
            if (application == null) application = context.Application;

            return application;
        }

        public HttpSessionState getSession()
        {
            if (context == null) context = getApplication().Context;
            if (session == null) session = context.Session;

            return session;
        }

        public void InitLocale()
        {
            if (systemLocale == null)
            {
                try
                {                    
                    string defaultLang = getContext().Request.Headers["Accept-Language"].ToString();
                    string firstLang = defaultLang.Split(',').FirstOrDefault();
                    defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
                    systemLocale = new CultureInfo(defaultLang);
                }
                catch (Exception e)
                {
                    LastException = e;
                    systemLocale = new CultureInfo("en");
                }
            }
            if (locale == null)
            {
                try
                {
                    string defaultLang = getContext().Request.Headers["Accept-Language"].ToString().Split(',').FirstOrDefault();
                    if (string.IsNullOrEmpty(defaultLang)) defaultLang = "en";
                    locale = new CultureInfo(defaultLang);
                }
                catch (Exception e)
                {
                    LastException = e;
                    locale = new CultureInfo(systemLocale.TwoLetterISOLanguageName.ToLower());
                }
            }
        }

        public void InitPrefixUrl()
        {
            try
            {
                if (prefixUri == null)
                    prefixUri = new Uri(prefixUrl);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        public void InitPictureUrl()
        {
            try
            {
                if (pictureUri == null)
                    pictureUri = new Uri(pictureUrl);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }


        // public DIALOGS getDialog() { return dialogOpened; }
        // public void setDialog(DIALOGS dia) { dialogOpened = dia; }

        #endregion members

        public static HttpApplication getApplication()
        {
            return sApplication;
        }

    }
}