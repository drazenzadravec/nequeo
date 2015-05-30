/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Runtime.InteropServices;

using Nequeo.Extension;

namespace Nequeo.Invention
{
    /// <summary>
    /// International time zone implementation class.
    /// </summary>
    public sealed class TimeZone
    {
        /// <summary>
        /// Convert the current time zone to the specified time zone.
        /// </summary>
        /// <param name="currentDateTime">The current time and date.</param>
        /// <param name="timeZoneSystemName">The system time zone name to convert to.</param>
        /// <param name="includeUTCOffset">Include the UTC offset in the converted time.</param>
        /// <returns>The converted time zone.</returns>
        public static DateTime TimeZoneDateTimeConverter(DateTime currentDateTime, string timeZoneSystemName, bool includeUTCOffset)
        {
            // Get the current time zone data from the system and
            // convert this time zone information from the current
            // date time to the specified time zone.
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneSystemName);
            DateTime destinationDateTime = TimeZoneInfo.ConvertTime(currentDateTime, timeZoneInfo);

            if (!includeUTCOffset)
            {
                // Add only the date time information to the
                // returned date time, does not include any
                // time zone offset data.
                DateTime convertedDateTime =
                    new DateTime(
                        destinationDateTime.Year,
                        destinationDateTime.Month,
                        destinationDateTime.Day,
                        destinationDateTime.Hour,
                        destinationDateTime.Minute,
                        destinationDateTime.Second);

                // The time zone without UTC offset.
                return convertedDateTime;
            }
            else
                // Return the converted time zone.
                return destinationDateTime;
        }

        /// <summary>
        /// Gets the local time from the UTC time for the specified offset.
        /// </summary>
        /// <param name="utc">The UTC date time.</param>
        /// <param name="offset">The offset including sign, e.g. 1030, +1030, -1030</param>
        /// <returns>The local time for the offset.</returns>
        public static DateTime LocalDateTime(DateTime utc, string offset)
        {
            // If the offset is backwars utc.
            if (offset.Contains("-"))
            {
                // Subtract the offset.
                string newOffset = offset.Trim().TrimStart('-').Trim();
                return utc.Subtract(new TimeSpan(newOffset.GetUtcOffsetHours(), newOffset.GetUtcOffsetMinutes(), 0));
            }
            else
            {
                // Add the offset.
                string newOffset = offset.Trim().TrimStart('+').Trim();
                return utc.Add(new TimeSpan(newOffset.GetUtcOffsetHours(), newOffset.GetUtcOffsetMinutes(), 0));
            }
        }

        /// <summary>
        /// Gets the local time from the UTC time for the specified offset.
        /// </summary>
        /// <param name="utc">The UTC date time.</param>
        /// <param name="offsetHours">The offset hours</param>
        /// <param name="offsetMinutes">The offset minutes</param>
        /// <param name="isForwardHours">True if the offset values are ahead of +0 else false behide</param>
        /// <returns>The local time for the offset.</returns>
        public static DateTime LocalDateTime(DateTime utc, Int32 offsetHours, Int32 offsetMinutes, bool isForwardHours)
        {
            if (isForwardHours)
                return utc.Add(new TimeSpan(offsetHours, offsetMinutes, 0));
            else
                return utc.Subtract(new TimeSpan(offsetHours, offsetMinutes, 0));
        }

        /// <summary>
        /// Gets the utc offset for the current local time.
        /// </summary>
        /// <returns>The utc offset as string.</returns>
        public static string GetUtcOffset()
        {
            TimeSpan TimeSpan = GetOffset();
            return GetOffset(TimeSpan);
        }

        /// <summary>
        /// Gets the utc offset for the current local time.
        /// </summary>
        /// <param name="dateTime">The current date time.</param>
        /// <returns>The utc offset as string.</returns>
        public static string GetUtcOffset(DateTime dateTime)
        {
            TimeSpan TimeSpan = GetOffset(dateTime);
            return GetOffset(TimeSpan);
        }

        /// <summary>
        /// Gets the utc offset time span for the current local time.
        /// </summary>
        /// <returns>The utc time span.</returns>
        public static TimeSpan GetOffset()
        {
            return DateTimeOffset.Now.Offset;
        }

        /// <summary>
        /// ets the utc offset time span for the current local time.
        /// </summary>
        /// <param name="dateTime">The current date time.</param>
        /// <returns>The utc time span.</returns>
        public static TimeSpan GetOffset(DateTime dateTime)
        {
            DateTimeOffset offset = new DateTimeOffset(dateTime);
            return offset.Offset;
        }

        /// <summary>
        /// Gets the utc offset for the current local time.
        /// </summary>
        /// <param name="timeSpan">The utc offset time span.</param>
        /// <returns>The utc offset as string.</returns>
        public static string GetOffset(TimeSpan timeSpan)
        {
            string hours = (timeSpan.Hours <= 9 ? timeSpan.Hours.ToString().PadLeft(2, '0') : timeSpan.Hours.ToString());
            string minutes = (timeSpan.Minutes <= 9 ? timeSpan.Minutes.ToString().PadLeft(2, '0') : timeSpan.Minutes.ToString());
            return (timeSpan.Ticks >= 0 ? "+" + hours + minutes : "-" + hours + minutes);
        }
    }

    /// <summary>
    /// The time zone system name.
    /// </summary>
    [SerializableAttribute()]
    public enum TimeZoneSystemName
    {
        #region System Name
        /// <summary>
        /// The Aus Eastern Standard Time enum value
        /// </summary>
        AusEasternStandardTime = 1,

        /// <summary>
        /// The E. Australia Standard Time enum value
        /// </summary>
        EAustraliaStandardTime = 2,

        /// <summary>
        /// The AUS Central Standard Time enum value
        /// </summary>
        AUSCentralStandardTime = 3,

        /// <summary>
        /// The Cen. Australia Standard Time enum value
        /// </summary>
        CenAustraliaStandardTime = 4,

        /// <summary>
        /// The Tasmania Standard Time enum value
        /// </summary>
        TasmaniaStandardTime = 5,

        /// <summary>
        /// The W. Australia Standard Time enum value
        /// </summary>
        WAustraliaStandardTime = 6,

        /// <summary>
        /// The Afghanistan Standard Time enum value
        /// </summary>
        AfghanistanStandardTime = 9,

        /// <summary>
        /// The Alaskan Standard Time enum value
        /// </summary>
        AlaskanStandardTime = 10,

        /// <summary>
        /// The Arab Standard Time enum value
        /// </summary>
        ArabStandardTime = 11,

        /// <summary>
        /// The Arabian Standard Time enum value
        /// </summary>
        ArabianStandardTime = 13,

        /// <summary>
        /// The Arabic Standard Time enum value
        /// </summary>
        ArabicStandardTime = 16,

        /// <summary>
        /// The Argentina Standard Time enum value
        /// </summary>
        ArgentinaStandardTime = 17,

        /// <summary>
        /// The Armenian Standard Time enum value
        /// </summary>
        ArmenianStandardTime = 18,

        /// <summary>
        /// The Atlantic Standard Time enum value
        /// </summary>
        AtlanticStandardTime = 19,

        /// <summary>
        /// The Azerbaijan Standard Time enum value
        /// </summary>
        AzerbaijanStandardTime = 20,

        /// <summary>
        /// The Azores Standard Time enum value
        /// </summary>
        AzoresStandardTime = 21,

        /// <summary>
        /// The Canada Central Standard Time enum value
        /// </summary>
        CanadaCentralStandardTime = 22,

        /// <summary>
        /// The Cape Verde Standard Time enum value
        /// </summary>
        CapeVerdeStandardTime = 23,

        /// <summary>
        /// The Caucasus Standard Time enum value
        /// </summary>
        CaucasusStandardTime = 24,

        /// <summary>
        /// The Central America Standard Time enum value
        /// </summary>
        CentralAmericaStandardTime = 25,

        /// <summary>
        /// The Central Asia Standard Time enum value
        /// </summary>
        CentralAsiaStandardTime = 26,

        /// <summary>
        /// The Central Brazilian Standard Time enum value
        /// </summary>
        CentralBrazilianStandardTime = 28,

        /// <summary>
        /// The Central Europe Standard Time enum value
        /// </summary>
        CentralEuropeStandardTime = 29,

        /// <summary>
        /// The Central European Standard Time enum value
        /// </summary>
        CentralEuropeanStandardTime = 34,

        /// <summary>
        /// The Central Pacific Standard Time enum value
        /// </summary>
        CentralPacificStandardTime = 38,

        /// <summary>
        /// The Central Standard Time enum value
        /// </summary>
        CentralStandardTime = 42,

        /// <summary>
        /// The Mexico Standard Time enum value
        /// </summary>
        MexicoStandardTime = 43,

        /// <summary>
        /// The China Standard Time enum value
        /// </summary>
        ChinaStandardTime = 46,

        /// <summary>
        /// The E. Africa Standard Time enum value
        /// </summary>
        EAfricaStandardTime = 51,

        /// <summary>
        /// The E. Europe Standard Time enum value
        /// </summary>
        EEuropeStandardTime = 52,

        /// <summary>
        /// The E. South America Standard Time enum value
        /// </summary>
        ESouthAmericaStandardTime = 53,

        /// <summary>
        /// The Eastern Standard Time enum value
        /// </summary>
        EasternStandardTime = 54,

        /// <summary>
        /// The Egypt Standard Time enum value
        /// </summary>
        EgyptStandardTime = 55,

        /// <summary>
        /// The Ekaterinburg Standard Time enum value
        /// </summary>
        EkaterinburgStandardTime = 56,

        /// <summary>
        /// The Fiji Standard Time enum value
        /// </summary>
        FijiStandardTime = 57,

        /// <summary>
        /// The FLE Standard Time enum value
        /// </summary>
        FLEStandardTime = 60,

        /// <summary>
        /// The Georgian Standard Time enum value
        /// </summary>
        GeorgianStandardTime = 66,

        /// <summary>
        /// The GMT Standard Time enum value
        /// </summary>
        GMTStandardTime = 67,

        /// <summary>
        /// The Greenland Standard Time enum value
        /// </summary>
        GreenlandStandardTime = 71,

        /// <summary>
        /// The Greenwich Standard Time enum value
        /// </summary>
        GreenwichStandardTime = 72,

        /// <summary>
        /// The GTB Standard Time enum value
        /// </summary>
        GTBStandardTime = 75,

        /// <summary>
        /// The Hawaiian Standard Time enum value
        /// </summary>
        HawaiianStandardTime = 78,

        /// <summary>
        /// The India Standard Time enum value
        /// </summary>
        IndiaStandardTime = 79,

        /// <summary>
        /// The Iran Standard Time enum value
        /// </summary>
        IranStandardTime = 84,

        /// <summary>
        /// The Israel Standard Time enum value
        /// </summary>
        IsraelStandardTime = 85,

        /// <summary>
        /// The Jordan Standard Time enum value
        /// </summary>
        JordanStandardTime = 86,

        /// <summary>
        /// The Korea Standard Time enum value
        /// </summary>
        KoreaStandardTime = 87,

        /// <summary>
        /// The Mexico Standard Time 2 enum value
        /// </summary>
        MexicoStandardTime2 = 88,

        /// <summary>
        /// The Mid-Atlantic Standard Time enum value
        /// </summary>
        Mid_AtlanticStandardTime = 91,

        /// <summary>
        /// The Middle East Standard Time enum value
        /// </summary>
        MiddleEastStandardTime = 92,

        /// <summary>
        /// The Montevideo Standard Time enum value
        /// </summary>
        MontevideoStandardTime = 93,

        /// <summary>
        /// The Mountain Standard Time enum value
        /// </summary>
        MountainStandardTime = 94,

        /// <summary>
        /// The Myanmar Standard Time enum value
        /// </summary>
        MyanmarStandardTime = 95,

        /// <summary>
        /// The N. Central Asia Standard Time enum value
        /// </summary>
        NCentralAsiaStandardTime = 96,

        /// <summary>
        /// The Namibia Standard Time enum value
        /// </summary>
        NamibiaStandardTime = 98,

        /// <summary>
        /// The Nepal Standard Time enum value
        /// </summary>
        NepalStandardTime = 99,

        /// <summary>
        /// The New Zealand Standard Time enum value
        /// </summary>
        NewZealandStandardTime = 100,

        /// <summary>
        /// The Newfoundland Standard Time enum value
        /// </summary>
        NewfoundlandStandardTime = 102,

        /// <summary>
        /// The North Asia East Standard Time enum value
        /// </summary>
        NorthAsiaEastStandardTime = 103,

        /// <summary>
        /// The North Asia Standard Time enum value
        /// </summary>
        NorthAsiaStandardTime = 105,

        /// <summary>
        /// The Pacific SA Standard Time enum value
        /// </summary>
        PacificSAStandardTime = 106,

        /// <summary>
        /// The Pacific Standard Time enum value
        /// </summary>
        PacificStandardTime = 107,

        /// <summary>
        /// The Romance Standard Time enum value
        /// </summary>
        RomanceStandardTime = 110,

        /// <summary>
        /// The Russian Standard Time enum value
        /// </summary>
        RussianStandardTime = 114,

        /// <summary>
        /// The SA Eastern Standard Time enum value
        /// </summary>
        SAEasternStandardTime = 117,

        /// <summary>
        /// The Samoa Standard Time enum value
        /// </summary>
        SamoaStandardTime = 122,

        /// <summary>
        /// The SE Asia Standard Time enum value
        /// </summary>
        SEAsiaStandardTime = 124,

        /// <summary>
        /// The Singapore Standard Time enum value
        /// </summary>
        SingaporeStandardTime = 127,

        /// <summary>
        /// The South Africa Standard Time enum value
        /// </summary>
        SouthAfricaStandardTime = 129,

        /// <summary>
        /// The Sri Lanka Standard Time enum value
        /// </summary>
        SriLankaStandardTime = 132,

        /// <summary>
        /// The Taipei Standard Time enum value
        /// </summary>
        TaipeiStandardTime = 134,

        /// <summary>
        /// The Tokyo Standard Time enum value
        /// </summary>
        TokyoStandardTime = 135,

        /// <summary>
        /// The Tonga Standard Time enum value
        /// </summary>
        TongaStandardTime = 138,

        /// <summary>
        /// The US Eastern Standard Time enum value
        /// </summary>
        USEasternStandardTime = 139,

        /// <summary>
        /// The US Mountain Standard Time enum value
        /// </summary>
        USMountainStandardTime = 140,

        /// <summary>
        /// The Venezuela Standard Time enum value
        /// </summary>
        VenezuelaStandardTime = 141,

        /// <summary>
        /// The Vladivostok Standard Time enum value
        /// </summary>
        VladivostokStandardTime = 142,

        /// <summary>
        /// The W. Central Africa Standard Time enum value
        /// </summary>
        WCentralAfricaStandardTime = 143,

        /// <summary>
        /// The W. Europe Standard Time enum value
        /// </summary>
        WEuropeStandardTime = 144,

        /// <summary>
        /// The West Asia Standard Time enum value
        /// </summary>
        WestAsiaStandardTime = 150,

        /// <summary>
        /// The West Pacific Standard Time enum value
        /// </summary>
        WestPacificStandardTime = 153,

        /// <summary>
        /// The Yakutsk Standard Time enum value
        /// </summary>
        YakutskStandardTime = 154,

        #endregion
    }

    /// <summary>
    /// The time zone display name.
    /// </summary>
    [SerializableAttribute()]
    public enum TimeZoneDisplayName
    {
        #region Display Name
        /// <summary>
        /// The Melbourne enum value
        /// </summary>
        Melbourne = 1,

        /// <summary>
        /// The Brisbane enum value
        /// </summary>
        Brisbane = 2,

        /// <summary>
        /// The Darwin enum value
        /// </summary>
        Darwin = 3,

        /// <summary>
        /// The Adelaide enum value
        /// </summary>
        Adelaide = 4,

        /// <summary>
        /// The Hobart enum value
        /// </summary>
        Hobart = 5,

        /// <summary>
        /// The Perth enum value
        /// </summary>
        Perth = 6,

        /// <summary>
        /// The Canberra enum value
        /// </summary>
        Canberra = 7,

        /// <summary>
        /// The Sydney enum value
        /// </summary>
        Sydney = 8,

        /// <summary>
        /// The Kabul enum value
        /// </summary>
        Kabul = 9,

        /// <summary>
        /// The Anchorage enum value
        /// </summary>
        Anchorage = 10,

        /// <summary>
        /// The Riyadh enum value
        /// </summary>
        Riyadh = 11,

        /// <summary>
        /// The Kuwai enum value
        /// </summary>
        Kuwai = 12,

        /// <summary>
        /// The Dubai enum value
        /// </summary>
        Dubai = 13,

        /// <summary>
        /// The Abu Dhabi enum value
        /// </summary>
        AbuDhabi = 14,

        /// <summary>
        /// The Muscat enum value
        /// </summary>
        Muscat = 15,

        /// <summary>
        /// The Baghdad enum value
        /// </summary>
        Baghdad = 16,

        /// <summary>
        /// The Buenos Aires enum value
        /// </summary>
        BuenosAires = 17,

        /// <summary>
        /// The Yerevan enum value
        /// </summary>
        Yerevan = 18,

        /// <summary>
        /// The Halifax enum value
        /// </summary>
        Halifax = 19,

        /// <summary>
        /// The Baku enum value
        /// </summary>
        Baku = 20,

        /// <summary>
        /// The Azores enum value
        /// </summary>
        Azores = 21,

        /// <summary>
        /// The Regina enum value
        /// </summary>
        Regina = 22,

        /// <summary>
        /// The Cape Verde enum value
        /// </summary>
        CapeVerde = 23,

        /// <summary>
        /// The Tbilisi enum value
        /// </summary>
        Tbilisi = 24,

        /// <summary>
        /// The Guatemala enum value
        /// </summary>
        Guatemala = 25,

        /// <summary>
        /// The Dhaka enum value
        /// </summary>
        Dhaka = 26,

        /// <summary>
        /// The Astana enum value
        /// </summary>
        Astana = 27,

        /// <summary>
        /// The Manaus enum value
        /// </summary>
        Manaus = 28,

        /// <summary>
        /// The Budapest enum value
        /// </summary>
        Budapest = 29,

        /// <summary>
        /// The Belgrade enum value
        /// </summary>
        Belgrade = 30,

        /// <summary>
        /// The Bratislava enum value
        /// </summary>
        Bratislava = 31,

        /// <summary>
        /// The Ljubljana enum value
        /// </summary>
        Ljubljana = 32,

        /// <summary>
        /// The Prague enum value
        /// </summary>
        Prague = 33,

        /// <summary>
        /// The Sarajevo enum value
        /// </summary>
        Sarajevo = 34,

        /// <summary>
        /// The Skopje enum value
        /// </summary>
        Skopje = 35,

        /// <summary>
        /// The Warsaw enum value
        /// </summary>
        Warsaw = 36,

        /// <summary>
        /// The Zagreb enum value
        /// </summary>
        Zagreb = 37,

        /// <summary>
        /// The Guadalcanal enum value
        /// </summary>
        Guadalcanal = 38,

        /// <summary>
        /// The Magadan enum value
        /// </summary>
        Magadan = 39,

        /// <summary>
        /// The Solomon Is. enum value
        /// </summary>
        SolomonIs = 40,

        /// <summary>
        /// The New Caledonia enum value
        /// </summary>
        NewCaledonia = 41,

        /// <summary>
        /// The Chicago enum value
        /// </summary>
        Chicago = 42,

        /// <summary>
        /// The Guadalajara enum value
        /// </summary>
        Guadalajara = 43,

        /// <summary>
        /// The Mexico City enum value
        /// </summary>
        MexicoCity = 44,

        /// <summary>
        /// The Monterrey enum value
        /// </summary>
        Monterrey = 45,

        /// <summary>
        /// The Shanghai enum value
        /// </summary>
        Shanghai = 46,

        /// <summary>
        /// The Beijing enum value
        /// </summary>
        Beijing = 47,

        /// <summary>
        /// The Chongqing enum value
        /// </summary>
        Chongqing = 48,

        /// <summary>
        /// The Hong Kong enum value
        /// </summary>
        HongKong = 49,

        /// <summary>
        /// The Urumqi enum value
        /// </summary>
        Urumqi = 50,

        /// <summary>
        /// The Nairobi enum value
        /// </summary>
        Nairobi = 51,

        /// <summary>
        /// The Minsk enum value
        /// </summary>
        Minsk = 52,

        /// <summary>
        /// The Sao Paulo enum value
        /// </summary>
        SaoPaulo = 53,

        /// <summary>
        /// The New York enum value
        /// </summary>
        NewYork = 54,

        /// <summary>
        /// The Cairo enum value
        /// </summary>
        Cairo = 55,

        /// <summary>
        /// The Yekaterinburg enum value
        /// </summary>
        Yekaterinburg = 56,

        /// <summary>
        /// The Fiji enum value
        /// </summary>
        Fiji = 57,

        /// <summary>
        /// The Kamchatka enum value
        /// </summary>
        Kamchatka = 58,

        /// <summary>
        /// The Marshall Is. enum value
        /// </summary>
        MarshallIs = 59,

        /// <summary>
        /// The Helsinki enum value
        /// </summary>
        Helsinki = 60,

        /// <summary>
        /// The Kiev enum value
        /// </summary>
        Kiev = 61,

        /// <summary>
        /// The Riga enum value
        /// </summary>
        Riga = 62,

        /// <summary>
        /// The Sofia enum value
        /// </summary>
        Sofia = 63,

        /// <summary>
        /// The Tallinn enum value
        /// </summary>
        Tallinn = 64,

        /// <summary>
        /// The Vilnius enum value
        /// </summary>
        Vilnius = 65,

        /// <summary>
        /// The Dublin enum value
        /// </summary>
        Dublin = 67,

        /// <summary>
        /// The Edinburgh enum value
        /// </summary>
        Edinburgh = 68,

        /// <summary>
        /// The Lisbon enum value
        /// </summary>
        Lisbon = 69,

        /// <summary>
        /// The London enum value
        /// </summary>
        London = 70,

        /// <summary>
        /// The Godthab enum value
        /// </summary>
        Godthab = 71,

        /// <summary>
        /// The Casablanca enum value
        /// </summary>
        Casablanca = 72,

        /// <summary>
        /// The Monrovia enum value
        /// </summary>
        Monrovia = 73,

        /// <summary>
        /// The Reykjavik enum value
        /// </summary>
        Reykjavik = 74,

        /// <summary>
        /// The Athens enum value
        /// </summary>
        Athens = 75,

        /// <summary>
        /// The Bucharest enum value
        /// </summary>
        Bucharest = 76,

        /// <summary>
        /// The Istanbul enum value
        /// </summary>
        Istanbul = 77,

        /// <summary>
        /// The Honolulu enum value
        /// </summary>
        Honolulu = 78,

        /// <summary>
        /// The Calcutta enum value
        /// </summary>
        Calcutta = 79,

        /// <summary>
        /// The Chennai enum value
        /// </summary>
        Chennai = 80,

        /// <summary>
        /// The Kolkata enum value
        /// </summary>
        Kolkata = 81,

        /// <summary>
        /// The Mumbai enum value
        /// </summary>
        Mumbai = 82,

        /// <summary>
        /// The New Delhi enum value
        /// </summary>
        NewDelhi = 83,

        /// <summary>
        /// The Tehran enum value
        /// </summary>
        Tehran = 84,

        /// <summary>
        /// The Jerusalem enum value
        /// </summary>
        Jerusalem = 85,

        /// <summary>
        /// The Amman enum value
        /// </summary>
        Amman = 86,

        /// <summary>
        /// The Seoul enum value
        /// </summary>
        Seoul = 87,

        /// <summary>
        /// The Chihuahua enum value
        /// </summary>
        Chihuahua = 88,

        /// <summary>
        /// The La Paz enum value
        /// </summary>
        LaPaz = 89,

        /// <summary>
        /// The Mazatlan enum value
        /// </summary>
        Mazatlan = 90,

        /// <summary>
        /// The South Georgia enum value
        /// </summary>
        SouthGeorgia = 91,

        /// <summary>
        /// The Beirut enum value
        /// </summary>
        Beirut = 92,

        /// <summary>
        /// The Montevideo enum value
        /// </summary>
        Montevideo = 93,

        /// <summary>
        /// The Denver enum value
        /// </summary>
        Denver = 94,

        /// <summary>
        /// The Rangoon enum value
        /// </summary>
        Rangoon = 95,

        /// <summary>
        /// The Almaty enum value
        /// </summary>
        Almaty = 96,

        /// <summary>
        /// The Novosibirsk enum value
        /// </summary>
        Novosibirsk = 97,

        /// <summary>
        /// The Windhoek enum value
        /// </summary>
        Windhoek = 98,

        /// <summary>
        /// The Katmandu enum value
        /// </summary>
        Katmandu = 99,

        /// <summary>
        /// The Auckland enum value
        /// </summary>
        Auckland = 100,

        /// <summary>
        /// The Wellington enum value
        /// </summary>
        Wellington = 101,

        /// <summary>
        /// The St Johns enum value
        /// </summary>
        StJohns = 102,

        /// <summary>
        /// The Irkutsk enum value
        /// </summary>
        Irkutsk = 103,

        /// <summary>
        /// The Ulaan Bataar enum value
        /// </summary>
        UlaanBataar = 104,

        /// <summary>
        /// The Krasnoyarsk enum value
        /// </summary>
        Krasnoyarsk = 105,

        /// <summary>
        /// The Santiago enum value
        /// </summary>
        Santiago = 106,

        /// <summary>
        /// The Los Angeles enum value
        /// </summary>
        LosAngeles = 107,

        /// <summary>
        /// The Tijuana enum value
        /// </summary>
        Tijuana = 108,

        /// <summary>
        /// The Baja California enum value
        /// </summary>
        BajaCalifornia = 109,

        /// <summary>
        /// The Brussels enum value
        /// </summary>
        Brussels = 110,

        /// <summary>
        /// The Copenhagen enum value
        /// </summary>
        Copenhagen = 111,

        /// <summary>
        /// The Madrid enum value
        /// </summary>
        Madrid = 112,

        /// <summary>
        /// The Paris enum value
        /// </summary>
        Paris = 113,

        /// <summary>
        /// The Moscow enum value
        /// </summary>
        Moscow = 114,

        /// <summary>
        /// The St. Petersburg enum value
        /// </summary>
        StPetersburg = 115,

        /// <summary>
        /// The Volgograd enum value
        /// </summary>
        Volgograd = 116,

        /// <summary>
        /// The Georgetown enum value
        /// </summary>
        Georgetown = 117,

        /// <summary>
        /// The Bogota enum value
        /// </summary>
        Bogota = 118,

        /// <summary>
        /// The Lima enum value
        /// </summary>
        Lima = 119,

        /// <summary>
        /// The Quito enum value
        /// </summary>
        Quito = 120,

        /// <summary>
        /// The Rio Branco enum value
        /// </summary>
        RioBranco = 121,

        /// <summary>
        /// The Apia enum value
        /// </summary>
        Apia = 122,

        /// <summary>
        /// The Samoa enum value
        /// </summary>
        Samoa = 123,

        /// <summary>
        /// The Bangkok enum value
        /// </summary>
        Bangkok = 124,

        /// <summary>
        /// The Hanoi enum value
        /// </summary>
        Hanoi = 125,

        /// <summary>
        /// The Jakarta enum value
        /// </summary>
        Jakarta = 126,

        /// <summary>
        /// The Kuala Lumpur enum value
        /// </summary>
        KualaLumpur = 127,

        /// <summary>
        /// The Singapore enum value
        /// </summary>
        Singapore = 128,

        /// <summary>
        /// The Johannesburg enum value
        /// </summary>
        Johannesburg = 129,

        /// <summary>
        /// The Harare enum value
        /// </summary>
        Harare = 130,

        /// <summary>
        /// The Pretoria enum value
        /// </summary>
        Pretoria = 131,

        /// <summary>
        /// The Colombo enum value
        /// </summary>
        Colombo = 132,

        /// <summary>
        /// The Sri Jayawardenepura enum value
        /// </summary>
        SriJayawardenepura = 133,

        /// <summary>
        /// The Taipei enum value
        /// </summary>
        Taipei = 134,

        /// <summary>
        /// The Tokyo enum value
        /// </summary>
        Tokyo = 135,

        /// <summary>
        /// The Sapporo enum value
        /// </summary>
        Sapporo = 136,

        /// <summary>
        /// The Osaka enum value
        /// </summary>
        Osaka = 137,

        /// <summary>
        /// The Tongatapu enum value
        /// </summary>
        Tongatapu = 138,

        /// <summary>
        /// The Indiana enum value
        /// </summary>
        Indiana = 139,

        /// <summary>
        /// The Phoenix enum value
        /// </summary>
        Phoenix = 140,

        /// <summary>
        /// The Caracas enum value
        /// </summary>
        Caracas = 141,

        /// <summary>
        /// The Vladivostok enum value
        /// </summary>
        Vladivostok = 142,

        /// <summary>
        /// The Lagos enum value
        /// </summary>
        Lagos = 143,

        /// <summary>
        /// The Amsterdam enum value
        /// </summary>
        Amsterdam = 144,

        /// <summary>
        /// The Berlin enum value
        /// </summary>
        Berlin = 145,

        /// <summary>
        /// The Bern enum value
        /// </summary>
        Bern = 146,

        /// <summary>
        /// The Rome enum value
        /// </summary>
        Rome = 147,

        /// <summary>
        /// The Stockholm enum value
        /// </summary>
        Stockholm = 148,

        /// <summary>
        /// The Vienna enum value
        /// </summary>
        Vienna = 149,

        /// <summary>
        /// The Islamabad enum value
        /// </summary>
        Islamabad = 150,

        /// <summary>
        /// The Karachi enum value
        /// </summary>
        Karachi = 151,

        /// <summary>
        /// The Tashkent enum value
        /// </summary>
        Tashkent = 152,

        /// <summary>
        /// The Port Moresby enum value
        /// </summary>
        PortMoresby = 153,

        /// <summary>
        /// The Yakutsk enum value
        /// </summary>
        Yakutsk = 154,

        #endregion
    }
}
