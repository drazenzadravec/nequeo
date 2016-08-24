using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Resource
{
    /// <summary>
    /// Countries.
    /// </summary>
    public sealed partial class Countries
    {
        /// <summary>
        /// Get all the languages that the specified country speeks.
        /// </summary>
        /// <param name="countryName">The name of the country.</param>
        /// <returns>The array of languages.</returns>
        public static string[] GetCountryLanguages(string countryName)
        {
            List<string> languages = new List<string>();

            // For each langauge
            foreach (KeyValuePair<string, string[]> language in Languages)
            {
                // For each country in the language.
                foreach (string country in language.Value)
                {
                    // If the country matches.
                    if (country.ToLower() == countryName.ToLower())
                    {
                        // Add the language.
                        languages.Add(language.Key);
                    }
                }
            }

            // Return the array of languages.
            return languages.ToArray();
        }

        /// <summary>
        /// Get all the countries and the languages these countries speek.
        /// </summary>
        /// <returns>The array of countries and languages.</returns>
        public static Dictionary<string, string[]> GetCountryLanguages()
        {
            Dictionary<string, string[]> countryLanguages = new Dictionary<string, string[]>();

            // For each contry.
            foreach (KeyValuePair<string, string> country in Names)
            {
                List<string> languages = new List<string>();

                // For each langauge
                foreach (KeyValuePair<string, string[]> language in Languages)
                {
                    // For each country in the language.
                    foreach (string item in language.Value)
                    {
                        // If the country matches.
                        if (country.Value.ToLower() == item.ToLower())
                        {
                            // Add the language.
                            languages.Add(language.Key);
                        }
                    }
                }

                // Add the country languages.
                countryLanguages.Add(country.Value, languages.ToArray());
            }

            // Return the country lanaguages.
            return countryLanguages;
        }

        /// <summary>
        /// Get the lanaguage code for each language.
        /// </summary>
        /// <returns>The array of language codes.</returns>
        public static Dictionary<string, string> GetLanguageCodes()
        {
            Dictionary<string, string> languageCodes = new Dictionary<string, string>();

            // For each langauge
            foreach (KeyValuePair<string, string[]> language in Languages)
            {
                string languageCode = null;
                try
                {
                    // Get the language code.
                    languageCode = Nequeo.Resource.Language.ResourceManager.GetString(language.Key, Nequeo.Resource.Language.Culture);
                    languageCodes.Add(language.Key, languageCode);
                }
                catch { }
            }

            // Return the lanaguage codes.
            return languageCodes;
        }

        /// <summary>
        /// Gets the list of language names and the countries that speek that language.
        /// </summary>
        public readonly static Dictionary<string, string[]> Languages = new Dictionary<string, string[]>
        {
            { "Afrikaans", new[] { "South Africa" } },
            { "Albanian", new[] { "Albania" } },
            { "Amharic", new[] { "Ethiopia" } },
            { "Arabic", new[] { "Algeria", "Bahrain", "Egypt", "Iraq", "Jordan", "Kuwait", "Lebanon", "Libya", "Morocco", "Oman", "Qatar", "Saudi Arabia", "Syria", "Tunisia", "United Arab Emirates", "Yemen" } },
            { "Armenian", new[] { "Armenia" } },
            { "Azerbaijani", new[] { "Azerbaijan", "Russia" } },
            { "Basque", new[] { "Spain" } },
            { "Belarusian", new[] { "Belarus" } },
            { "Bengali", new[] { "India", "Bangladesh" } },
            { "Bihari", new[] { "India" } },
            { "Bulgarian", new[] { "Bulgaria" } },
            { "Burmese", new[] { "Myanmar" } },
            { "Catalan", new[] { "Spain" } },
            { "Cherokee", new[] { "Canada", "United States" } },
            { "Chinese", new[] { "Hong Kong SAR China", "Macao SAR China", "China", "Singapore", "Taiwan" } },
            { "ChineseSimplified", new[] { "China" } },
            { "ChineseTraditional", new[] { "China", "Taiwan" } },
            { "Croatian", new[] { "Bosnia & Herzegovina", "Croatia" } },
            { "CyrillicAzeri", new[] { "Azerbaijan" } },
            { "CyrillicBosnian", new[] { "Bosnia & Herzegovina" } },
            { "CyrillicMongolian", new[] { "Mongolia" } },
            { "CyrillicSerbian", new[] { "Bosnia & Herzegovina", "Serbia" } },
            { "CyrillicUzbek", new[] { "Uzbekistan" } },
            { "Czech", new[] { "Czech Republic" } },
            { "Danish", new[] { "Denmark" } },
            { "Divehi", new[] { "Maldives" } },
            { "Dutch", new[] { "Belgium", "Netherlands" } },
            { "English", new[] { "Belize", "Canada", "Ireland", "Jamaica", "New Zealand", "Philippines", "South Africa", "Trinidad & Tobago", "United Kingdom", "United States", "Zimbabwe", "Australia" } },
            { "Esperanto", new[] { "Togo" } },
            { "Estonian", new[] { "Estonia" } },
            { "Faroese", new[] { "Faroe Islands" } },
            { "Filipino", new[] { "Philippines" } },
            { "Finnish", new[] { "Finland" } },
            { "French", new[] { "Belgium", "Canada", "France", "Luxembourg", "Monaco", "Switzerland" } },
            { "Frisian", new[] { "Netherlands" } },
            { "Galician", new[] { "Spain" } },
            { "Georgian", new[] { "Georgia" } },
            { "German", new[] { "Austria", "Germany", "Liechtenstein", "Luxembourg", "Switzerland" } },
            { "Greek", new[] { "Greece" } },
            { "Guarani", new[] { "Paraguay" } },
            { "Gujarati", new[] { "India", } },
            { "Hebrew", new[] { "Israel" } },
            { "Hindi", new[] { "India" } },
            { "Hungarian", new[] { "Hungary" } },
            { "Icelandic", new[] { "Iceland" } },
            { "Indonesian", new[] { "Indonesia" } },
            { "Inuktitut", new[] { "Canada" } },
            { "Irish", new[] { "Ireland" } },
            { "Italian", new[] { "Italy", "Switzerland" } },
            { "Japanese", new[] { "Japan" } },
            { "Kannada", new[] { "India" } },
            { "Kazakh", new[] { "Kazakhstan" } },
            { "Khmer", new[] { "Cambodia", "", "Vietnam", "Thailand" } },
            { "Kiswahili", new[] { "Kenya" } },
            { "Konkani", new[] { "India" } },
            { "Korean", new[] { "South Korea" } },
            { "Kurdish", new[] { "Iraq", "" } },
            { "Kyrgyz", new[] { "Kyrgyzstan" } },
            { "Laothian", new[] { "Laos" } },
            { "LatinAzeri", new[] { "Azerbaijan" } },
            { "LatinBosnian", new[] { "Bosnia & Herzegovina" } },
            { "LatinInuktitut", new[] { "Canada" } },
            { "LatinSerbian", new[] { "Bosnia & Herzegovina" } },
            { "LatinUzbek", new[] { "Uzbekistan" } },
            { "Latvian", new[] { "Latvia" } },
            { "Lithuanian", new[] { "Lithuania" } },
            { "Luxembourgish", new[] { "Luxembourg" } },
            { "Macedonian", new[] { "Macedonia" } },
            { "Malay", new[] { "Brunei Darussalam", "Malaysia" } },
            { "Malayalam", new[] { "India" } },
            { "Maltese", new[] { "Malta" } },
            { "Maori", new[] { "New Zealand" } },
            { "Mapudungun", new[] { "Chile" } },
            { "Marathi", new[] { "India" } },
            { "Mohawk", new[] { "Canada" } },
            { "Mongolian", new[] { "Mongolia" } },
            { "Nepali", new[] { "Nepal" } },
            { "Norwegian", new[] { "Norway" } },
            { "Oriya", new[] { "India" } },
            { "Pashto", new[] { "Pakistan", "Afghanistan" } },
            { "Persian", new[] { "Iran" } },
            { "Polish", new[] { "Poland" } },
            { "Portuguese", new[] { "Brazil", "Portugal" } },
            { "Punjabi", new[] { "India" } },
            { "Quechua", new[] { "Bolivia", "Ecuador", "Peru" } },
            { "Romanian", new[] { "Romania" } },
            { "Romansh", new[] { "Switzerland" } },
            { "Russian", new[] { "Russia" } },
            { "SamiInari", new[] { "Finland" } },
            { "SamiLule", new[] { "Norway", "Sweden" } },
            { "SamiNorthern", new[] { "Finland", "Norway", "Sweden" } },
            { "SamiSkolt", new[] { "Finland" } },
            { "SamiSouthern", new[] { "Norway", "Sweden" } },
            { "Sanskrit", new[] { "India" } },
            { "Serbian", new[] { "Serbia" } },
            { "SesothoSaLeboa", new[] { "South Africa" } },
            { "Setswana", new[] { "South Africa" } },
            { "Sindhi", new[] { "Pakistan", "India" } },
            { "Sinhalese", new[] { "SriLanka" } },
            { "Slovak", new[] { "Slovakia" } },
            { "Slovenian", new[] { "Slovenia" } },
            { "Spanish", new[] { "Argentina", "Bolivia", "Chile", "Colombia", "Costa Rica", "Dominican Republic", "Ecuador", "El Salvador", "Guatemala", "Honduras", "Mexico",
                                 "Nicaragua", "Panama", "Paraguay", "Peru", "Puerto Rico", "Spain", "Uruguay", "Venezuela" } },
            { "Swahili", new[] { "Tanzania", "Kenya", "Uganda", "Dominican Republic", "Congo", "Zanzibar" } },
            { "Swedish", new[] { "Finland", "Sweden" } },
            { "Syriac", new[] { "Syria" } },
            { "Tagalog", new[] { "Philippines" } },
            { "Tajik", new[] { "Tajikistan" } },
            { "Tamil", new[] { "India" } },
            { "Tatar", new[] { "Russia" } },
            { "Telugu", new[] { "India" } },
            { "Thai", new[] { "Thailand" } },
            { "Tibetan", new[] { "Nepal", "China" } },
            { "Turkish", new[] { "Turkey" } },
            { "Uighur", new[] { "China" } },
            { "Ukrainian", new[] { "Ukraine" } },
            { "Urdu", new[] { "Pakistan" } },
            { "Uzbek", new[] { "Uzbekistan", "Kyrgyzstan", "Afghanistan", "Kazakhstan", "Turkmenistan", "Tajikistan", "Russia", "China" } },
            { "Vietnamese", new[] { "Vietnam" } },
            { "Welsh", new[] { "United Kingdom" } },
            { "Xhosa", new[] { "South Africa" } },
            { "Yiddish", new[] { "Germany", "Bosnia & Herzegovina", "Netherlands", "Poland", "Romania", "Sweden", "Ukraine" } },
            { "Zulu", new[] { "South Africa" } }
        };
    }
}