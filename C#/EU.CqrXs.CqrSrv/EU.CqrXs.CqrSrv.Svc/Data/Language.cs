using SwaggerWcf.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace EU.CqrXs.CqrSrv.Svc.Data
{
    [DataContract(Name = "language")]
    [Description("Cultural language")]
    [SwaggerWcfDefinition(ExternalDocsUrl = "https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-8.0", ExternalDocsDescription = "Reduced Culture Info")]
    public class Language
    {
        [DataMember]
        [Description("Language Name")]
        public string Name { get; set; }


        [DataMember]
        [Description("Language DisplayName")]
        public string DisplayName { get; set; }


        [DataMember]
        [Description("Language Native Name")]
        public string NativeName { get; set; }


        [DataMember]
        [Description("Language English Name")]
        public string EnglishName { get; set; }


        [DataMember]
        [Description("TopLevel country domain")]
        public string TopLevelDomain { get; set; }



        [DataMember]
        [Description("Language ISO2 Code")]
        public string ISO2 { get; set; }

        [DataMember]
        [Description("Language ISO3 Code")]
        public string ISO3 { get; set; }


        public Language(string tag)
        {
            Language lang = GetLanguageByTopLevelDomain(tag);
            Name = lang.Name;
            DisplayName = lang.DisplayName;
            NativeName = lang.NativeName;
            EnglishName = lang.EnglishName;
            TopLevelDomain = lang.TopLevelDomain;
            ISO2 = lang.ISO2;
            ISO3 = lang.ISO3;
            
        }

        public Language()
        {            
            Name = "en";
            NativeName = "English";
            DisplayName = "English";
            ISO2 = "en";
            ISO3 = "eng";
            TopLevelDomain = "uk";
            EnglishName = "English";
        }

        public Language(string name, string nativeName, string displayName, string iso2, string iso3, string englishName, string toplevelDomain)
        {
            Name = name;
            NativeName = nativeName;
            DisplayName = displayName;
            ISO2 = iso2;
            ISO3 = iso3;
            EnglishName = englishName;
            TopLevelDomain = toplevelDomain;
        }

        public Language(string englishName, string nativeName, string iso2, string iso3) : this(englishName, nativeName, iso2, iso3, iso2) { }

        public Language(string englishName, string nativeName, string iso2, string iso3, string toplevelDomain)
        {
            EnglishName = englishName;
            NativeName = nativeName;
            DisplayName = englishName;
            Name = iso2;
            ISO2 = iso2;
            ISO3 = iso3;
            TopLevelDomain = toplevelDomain;
        }

        public Language(string englishName, string nativeName, string iso3)
        {            
            EnglishName = englishName;
            NativeName = nativeName;
            DisplayName = englishName;
            Name = iso3.Substring(0, 2);
            ISO2 = iso3.Substring(0, 2);
            ISO3 = iso3;
            TopLevelDomain = ISO2;
        }

        internal static Language GetLanguageByTopLevelDomain(string topLevelDomain)
        {
            Language lang;
            switch (topLevelDomain)
            {
                case "en": lang = new Language(); break;
                case "fr": lang = new Language("French", "français", "fra"); break;
                case "de": lang = new Language("German", "Deutsch", "deu"); break;
                case "pl": lang = new Language("Polish", "polski", "pl", "pol"); break;
                case "pt": lang = new Language("Portuguese", "português", "pt", "por"); break;
                case "it": lang = new Language("Italian", "italiano", "ita"); break;
                case "es": lang = new Language("Spanish", "español", "es", "spa"); break;
                case "da": lang = new Language("Danish", "dansk", "dan"); break;
                case "se": lang = new Language("Swedish", "dansk", "swe", "sv", "se"); break;
                case "no": lang = new Language("Norwegian Nynorsk", "norsk nynorsk", "nn", "nno", "no"); break;
                case "nl": lang = new Language("Dutch", "Nederlands", "nld"); break;
                case "fi": lang = new Language("Finnish", "suomi", "fin"); break;
                case "is": lang = new Language("Icelandic", "íslenska", "isl"); break;
                case "hr": lang = new Language("Croatian", "hrvatski", "hrv"); break;
                case "sl": lang = new Language("Slovenian", "slovenščina", "slv"); break;
                case "sk": lang = new Language("Slovak", "slovenčina", "sk", "slk"); break;
                case "cz": lang = new Language("Czech", "čeština", "cs", "ces", "cz"); break;
                case "bg": lang = new Language("Bulgarian", "български", "bul", "bg"); break;
                case "ro": lang = new Language("Romanian", "română", "ron"); break;
                case "ie": lang = new Language("Irish", "Gaeilge", "gle", "ga", "ie"); break;
                default: lang = new Language(); break;
            }
            return lang;
        }


        internal static Language[] GetAllLanguages()
        {
            Dictionary<string, Language> langDict = new Dictionary<string, Language>();
            langDict.Add("en", GetLanguageByTopLevelDomain("en"));
            langDict.Add("fr", GetLanguageByTopLevelDomain("fr"));
            langDict.Add("de", GetLanguageByTopLevelDomain("de"));
            langDict.Add("pl", GetLanguageByTopLevelDomain("pl"));
            langDict.Add("pt", GetLanguageByTopLevelDomain("pt"));
            langDict.Add("it", GetLanguageByTopLevelDomain("it"));
            langDict.Add("es", GetLanguageByTopLevelDomain("es"));
            langDict.Add("da", GetLanguageByTopLevelDomain("da"));
            langDict.Add("se", GetLanguageByTopLevelDomain("se"));
            langDict.Add("no", GetLanguageByTopLevelDomain("no"));
            langDict.Add("nl", GetLanguageByTopLevelDomain("nl"));
            langDict.Add("fi", GetLanguageByTopLevelDomain("fi"));
            langDict.Add("is", GetLanguageByTopLevelDomain("is"));
            langDict.Add("hr", GetLanguageByTopLevelDomain("hr"));
            langDict.Add("sl", GetLanguageByTopLevelDomain("sl"));
            langDict.Add("sk", GetLanguageByTopLevelDomain("sk"));
            langDict.Add("cz", GetLanguageByTopLevelDomain("cz"));
            langDict.Add("bg", GetLanguageByTopLevelDomain("bg"));
            langDict.Add("ro", GetLanguageByTopLevelDomain("ro"));
            langDict.Add("ie", GetLanguageByTopLevelDomain("ie"));


            return langDict.Values.ToArray();
        }

    }

}
