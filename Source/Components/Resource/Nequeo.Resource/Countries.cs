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
        /// Get the country code for each country.
        /// </summary>
        /// <returns>The array of country codes.</returns>
        public static Dictionary<string, string> GetCountryCodes()
        {
            Dictionary<string, string> countryCodes = new Dictionary<string, string>();

            // For each country
            foreach (KeyValuePair<string, string> country in Names)
            {
                string countryCode = null;
                try
                {
                    // Get the language code.
                    countryCode = Nequeo.Resource.Country.ResourceManager.GetString(country.Key, Nequeo.Resource.Country.Culture);
                    countryCodes.Add(country.Value, countryCode);
                }
                catch { }
            }

            // Return the lanaguage codes.
            return countryCodes;
        }

        /// <summary>
        /// Get the country dial code for each country.
        /// </summary>
        /// <returns>The array of country dial codes.</returns>
        public static Dictionary<string, string> GetCountryDialCodes()
        {
            Dictionary<string, string> countryCodes = new Dictionary<string, string>();

            // For each country
            foreach (KeyValuePair<string, string> country in Names)
            {
                string countryCode = null;
                try
                {
                    // Get the language code.
                    countryCode = Nequeo.Resource.CountryDial.ResourceManager.GetString(country.Key, Nequeo.Resource.CountryDial.Culture);
                    countryCodes.Add(country.Value, countryCode);
                }
                catch { }
            }

            // Return the lanaguage codes.
            return countryCodes;
        }

        /// <summary>
        /// Gets the list of country names.
        /// </summary>
        public readonly static Dictionary<string, string> Names = new Dictionary<string, string>
        {
            {"Afghanistan","Afghanistan"},
            {"AlandIslands","Aland Islands"},
            {"Albania","Albania"},
            {"Algeria","Algeria"},
            {"AmericanSamoa","American Samoa"},
            {"Andorra","Andorra"},
            {"Angola","Angola"},
            {"Anguilla","Anguilla"},
            {"Antarctica","Antarctica"},
            {"AntiguaAndBarbuda","Antigua & Barbuda"},
            {"Argentina","Argentina"},
            {"Armenia","Armenia"},
            {"Aruba","Aruba"},
            {"Australia","Australia"},
            {"Austria","Austria"},
            {"Azerbaijan","Azerbaijan"},
            {"Bahamas","Bahamas"},
            {"Bahrain","Bahrain"},
            {"Bangladesh","Bangladesh"},
            {"Barbados","Barbados"},
            {"Belarus","Belarus"},
            {"Belgium","Belgium"},
            {"Belize","Belize"},
            {"Benin","Benin"},
            {"Bermuda","Bermuda"},
            {"Bhutan","Bhutan"},
            {"Bolivia","Bolivia"},
            {"BosniaAndHerzegovina","Bosnia & Herzegovina"},
            {"Botswana","Botswana"},
            {"BouvetIsland","Bouvet Island"},
            {"Brazil","Brazil"},
            {"BritishIndianOceanTerritory","British Indian Ocean Territory"},
            {"BritishVirginIslands","British Virgin Islands"},
            {"Brunei","Brunei"},
            {"Bulgaria","Bulgaria"},
            {"BurkinaFaso","Burkina Faso"},
            {"Burundi","Burundi"},
            {"Cambodia","Cambodia"},
            {"Cameroon","Cameroon"},
            {"Canada","Canada"},
            {"CapeVerde","Cape Verde"},
            {"CaribbeanNetherlands","Caribbean Netherlands"},
            {"CaymanIslands","Cayman Islands"},
            {"CentralAfricanRepublic","Central African Republic"},
            {"Chad","Chad"},
            {"Chile","Chile"},
            {"China","China"},
            {"ChristmasIsland","Christmas Island"},
            {"CocosIslands","Cocos Islands"},
            {"Colombia","Colombia"},
            {"Comoros","Comoros"},
            {"CongoBrazzaville","Congo Brazzaville"},
            {"CongoKinshasa","Congo Kinshasa"},
            {"CookIslands","Cook Islands"},
            {"CostaRica","Costa Rica"},
            {"CoteDIvoire","Cote DIvoire"},
            {"Croatia","Croatia"},
            {"Cuba","Cuba"},
            {"Curacao","Curacao"},
            {"Cyprus","Cyprus"},
            {"CzechRepublic","Czech Republic"},
            {"Denmark","Denmark"},
            {"Dominica","Dominica"},
            {"DominicanRepublic","Dominican Republic"},
            {"Ecuador","Ecuador"},
            {"Egypt","Egypt"},
            {"ElSalvador","El Salvador"},
            {"EquatorialGuinea","Equatorial Guinea"},
            {"Eritea","Eritrea"},
            {"Estonia","Estonia"},
            {"Ethiopia","Ethiopia"},
            {"FalklandIslands","Falkland Islands"},
            {"FaroeIslands","Faroe Islands"},
            {"Fiji","Fiji"},
            {"Finland","Finland"},
            {"France","France"},
            {"FrenchGuiana","French Guiana"},
            {"FrenchPolynesia","French Polynesia"},
            {"FrenchSouthernTerritories","French Southern Territories"},
            {"Gabon","Gabon"},
            {"Gambia","Gambia"},
            {"Georgia","Georgia"},
            {"Germany","Germany"},
            {"Ghana","Ghana"},
            {"Gibraltar","Gibraltar"},
            {"Greece","Greece"},
            {"Greenland","Greenland"},
            {"Grenada","Grenada"},
            {"Guadeloupe","Guadeloupe"},
            {"Guam","Guam"},
            {"Guatemala","Guatemala"},
            {"Guernsey","Guernsey"},
            {"Guinea","Guinea"},
            {"GuineaBissau","Guinea Bissau"},
            {"Guyana","Guyana"},
            {"Haiti","Haiti"},
            {"HeardAndMcDonaldIslands","Heard & McDonald Islands"},
            {"Honduras","Honduras"},
            {"HongKongSARChina","Hong Kong SAR China"},
            {"Hungary","Hungary"},
            {"Iceland","Iceland"},
            {"India","India"},
            {"Indonesia","Indonesia"},
            {"Iran","Iran"},
            {"Iraq","Iraq"},
            {"Ireland","Ireland"},
            {"IsleOfMan","Isle Of Man"},
            {"Israel","Israel"},
            {"Italy","Italy"},
            {"Jamaica","Jamaica"},
            {"Japan","Japan"},
            {"Jersey","Jersey"},
            {"Jordan","Jordan"},
            {"Kazakhstan","Kazakhstan"},
            {"Kenya","Kenya"},
            {"Kiribati","Kiribati"},
            {"Kuwait","Kuwait"},
            {"Kyrgyzstan","Kyrgyzstan"},
            {"Laos","Laos"},
            {"Latvia","Latvia"},
            {"Lebanon","Lebanon"},
            {"Lesotho","Lesotho"},
            {"Liberia","Liberia"},
            {"Libya","Libya"},
            {"Liechtenstein","Liechtenstein"},
            {"Lithuania","Lithuania"},
            {"Luxembourg","Luxembourg"},
            {"MacauSARChina","Macau SAR China"},
            {"Macedonia","Macedonia"},
            {"Madagascar","Madagascar"},
            {"Malawi","Malawi"},
            {"Malaysia","Malaysia"},
            {"Maldives","Maldives"},
            {"Mali","Mali"},
            {"Malta","Malta"},
            {"MarshallIslands","Marshall Islands"},
            {"Martinique","Martinique"},
            {"Mauritania","Mauritania"},
            {"Mauritius","Mauritius"},
            {"Mayotte","Mayotte"},
            {"Mexico","Mexico"},
            {"Micronesia","Micronesia"},
            {"Moldova","Moldova"},
            {"Monaco","Monaco"},
            {"Mongolia","Mongolia"},
            {"Montenegro","Montenegro"},
            {"Montserrat","Montserrat"},
            {"Morocco","Morocco"},
            {"Mozambique","Mozambique"},
            {"Myanmar","Myanmar"},
            {"Namibia","Namibia"},
            {"Nauru","Nauru"},
            {"Nepal","Nepal"},
            {"Netherlands","Netherlands"},
            {"NewCaledonia","NewC aledonia"},
            {"NewZealand","New Zealand"},
            {"Nicaragua","Nicaragua"},
            {"Niger","Niger"},
            {"Nigeria","Nigeria"},
            {"Niue","Niue"},
            {"NorfolkIsland","Norfolk Island"},
            {"NorthernMarianaIslands","Northern Mariana Islands"},
            {"NorthKorea","North Korea"},
            {"Norway","Norway"},
            {"Oman","Oman"},
            {"Pakistan","Pakistan"},
            {"Palau","Palau"},
            {"PalestinianTerritories","Palestinian Territories"},
            {"Panama","Panama"},
            {"PapuaNewGuinea","Papua New Guinea"},
            {"Paraguay","Paraguay"},
            {"Peru","Peru"},
            {"Philippines","Philippines"},
            {"PitcairnIslands","PitcairnIslands"},
            {"Poland","Poland"},
            {"Portugal","Portugal"},
            {"PuertoRico","PuertoRico"},
            {"Qatar","Qatar"},
            {"Reunion","Reunion"},
            {"Romania","Romania"},
            {"Russia","Russia"},
            {"Rwanda","Rwanda"},
            {"Samoa","Samoa"},
            {"SanMarino","San Marino"},
            {"SaoTomeAndPrincipe","Sao Tome & Principe"},
            {"SaudiArabia","Saudi Arabia"},
            {"Senegal","Senegal"},
            {"Serbia","Serbia"},
            {"Seychelles","Seychelles"},
            {"SierraLeone","Sierra Leone"},
            {"Singapore","Singapore"},
            {"SintMaarten","Sint Maarten"},
            {"Slovakia","Slovakia"},
            {"Slovenia","Slovenia"},
            {"SoGeorgiaAndSoSandwichIsl","So. Georgia & So. Sandwich Isl"},
            {"SolomonIslands","Solomon Islands"},
            {"Somalia","Somalia"},
            {"SouthAfrica","South Africa"},
            {"SouthKorea","South Korea"},
            {"SouthSudan","South Sudan"},
            {"Spain","Spain"},
            {"SriLanka","Sri Lanka"},
            {"StBarthelemy","St Barthelemy"},
            {"StHelena","St Helena"},
            {"StKittsAndNevis","St Kitts & Nevis"},
            {"StLucia","St Lucia"},
            {"StMartin","St Martin"},
            {"StPierreAndMiquelon","St Pierre & Miquelon"},
            {"StVincentAndGrenadines","St Vincent & Grenadines"},
            {"Sudan","Sudan"},
            {"Suriname","Suriname"},
            {"SvalbardAndJanMayen","Svalbard & Jan Mayen"},
            {"Sweden","Sweden"},
            {"Switzerland","Switzerland"},
            {"Syria","Syria"},
            {"Taiwan","Taiwan"},
            {"Tajikistan","Tajikistan"},
            {"Tanzania","Tanzania"},
            {"Thailand","Thailand"},
            {"TimorLeste","Timor Leste"},
            {"Togo","Togo"},
            {"Tokelau","Tokelau"},
            {"Tonga","Tonga"},
            {"TrinidadAndTobago","Trinidada & Tobago"},
            {"Tunisia","Tunisia"},
            {"Turkey","Turkey"},
            {"Turkmenistan","Turkmenistan"},
            {"TurksAndCaicosIslands","Turks & Caicos Islands"},
            {"Tuvalu","Tuvalu"},
            {"Uganda","Uganda"},
            {"Ukraine","Ukraine"},
            {"UnitedArabEmirates","United Arab Emirates"},
            {"UnitedKingdom","United Kingdom"},
            {"UnitedStates","United States"},
            {"Uruguay","Uruguay"},
            {"USOutlyingIslands","U.S. Outlying Islands"},
            {"USVirginIslands","U.S. Virgin Islands"},
            {"Uzbekistan","Uzbekistan"},
            {"Vanuatu","Vanuatu"},
            {"VaticanCity","Vatican City"},
            {"Venezuela","Venezuela"},
            {"Vietnam","Vietnam"},
            {"WallisAndFutuna","Wallis & Futuna"},
            {"Yemen","Yemen"},
            {"Zambia","Zambia"},
            {"Zanzibar","Zanzibar"},
            {"Zimbabwe","Zimbabwe"}
        };
    }
}
