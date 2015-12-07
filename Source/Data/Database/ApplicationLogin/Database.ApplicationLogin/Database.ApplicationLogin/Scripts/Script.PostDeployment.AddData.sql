/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

USE [ApplicationLogin]
GO

INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AU', 'Australia', 1, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AL', 'Albanian', 2, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('DZ', 'Algeria', 3, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AR', 'Argentina', 4, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AM', 'Armenia', 5, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AT', 'Austria', 6, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AZ', 'Azerbaijan', 7, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BH', 'Bahrain', 8, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BY', 'Belarus', 9, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BE', 'Belgium', 10, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BZ', 'Belize', 11, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BO', 'Bolivia', 12, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BA', 'Bosnia and Herzegovina', 13, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BR', 'Brazil', 14, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BN', 'Brunei Darussalam', 15, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BG', 'Bulgaria', 16, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CA', 'Canada', 17, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CL', 'Chile', 18, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CO', 'Colombia', 19, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CR', 'Costa Rica', 20, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('HR', 'Croatia', 21, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CZ', 'Czech Republic', 22, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('DK', 'Denmark', 23, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('DO', 'Dominican Republic', 24, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('EC', 'Ecuador', 25, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('EG', 'Egypt', 26, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SV', 'El Salvador', 27, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('EE', 'Estonia', 28, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('FO', 'Faroe Islands', 29, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('FI', 'Finland', 30, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('FR', 'France', 31, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('GE', 'Georgia', 32, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('DE', 'Germany', 33, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('GR', 'Greece', 34, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('GT', 'Guatemala', 35, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('HN', 'Honduras', 36, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('HK', 'Hong Kong S.A.R.', 37, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('HU', 'Hungary', 38, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('IS', 'Iceland', 39, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('IN', 'India', 40, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('ID', 'Indonesia', 41, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('IR', 'Iran', 42, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('IQ', 'Iraq', 43, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('IE', 'Ireland', 44, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PK', 'Pakistan', 45, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('IL', 'Israel', 46, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('IT', 'Italy', 47, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('JM', 'Jamaica', 48, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('JP', 'Japan', 49, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('JO', 'Jordan', 50, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('KZ', 'Kazakhstan', 51, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('KE', 'Kenya', 52, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('KR', 'South Korea', 53, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('KW', 'Kuwait', 54, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('KG', 'Kyrgyzstan', 55, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LV', 'Latvia', 56, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LB', 'Lebanon', 57, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LY', 'Libya', 58, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LI', 'Liechtenstein', 59, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LT', 'Lithuania', 60, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LU', 'Luxembourg', 61, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MO', 'Macao S.A.R.', 62, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MK', 'Macedonia', 63, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MY', 'Malaysia', 64, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MV', 'Maldives', 65, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MT', 'Malta', 66, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MX', 'Mexico', 67, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MN', 'Mongolia', 68, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MA', 'Morocco', 69, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NL', 'Netherlands', 70, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NZ', 'New Zealand', 71, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NI', 'Nicaragua', 72, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NO', 'Norway', 73, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('OM', 'Oman', 74, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PA', 'Panama', 75, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PY', 'Paraguay', 76, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CN', 'China', 77, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PE', 'Peru', 78, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PH', 'Philippines', 79, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PL', 'Poland', 80, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PT', 'Portugal', 81, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MC', 'Monaco', 82, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PR', 'Puerto Rico', 83, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('QA', 'Qatar', 84, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('RO', 'Romania', 85, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('RU', 'Russia', 86, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SA', 'Saudi Arabia', 87, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CS', 'Serbia', 88, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SG', 'Singapore', 89, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SK', 'Slovakia', 90, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SI', 'Slovenia', 91, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('ZA', 'South Africa', 92, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('ES', 'Spain', 93, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SE', 'Sweden', 94, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('CH', 'Switzerland', 95, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SY', 'Syria', 96, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('TW', 'Taiwan', 97, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('TH', 'Thailand', 98, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('TT', 'Trinidad and Tobago', 99, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('TN', 'Tunisia', 100, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('TR', 'Turkey', 101, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AE', 'United Arab Emirates', 102, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('UA', 'Ukraine', 103, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('GB', 'United Kingdom', 104, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('US', 'United States', 105, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('UY', 'Uruguay', 106, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('UZ', 'Uzbekistan', 107, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('VE', 'Venezuela', 108, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('VN', 'Vietnam', 109, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('YE', 'Yemen', 110, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('ZW', 'Zimbabwe', 111, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('AF', 'Afghanistan', 112, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('BD', 'Bangladesh', 113, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('SB', 'Solomon Islands', 114, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NC', 'New Caledonia', 115, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('FJ', 'Fiji', 116, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MH', 'Marshall Islands', 117, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('GL', 'GreenLand', 118, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LR', 'Liberia', 119, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('MM', 'Myanmar', 120, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NA', 'Namibia', 121, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NP', 'Nepal', 122, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('YK', 'Newfoundland', 123, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('LK', 'Samoa', 124, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('ZW', 'Sri Lanka', 125, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('TO', 'Tonga', 126, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('NG', 'Nigeria', 127, 1)
INSERT INTO [dbo].[Country]([CountryCode],[CountryName],[GroupOrder],[CountryVisible])VALUES('PG', 'Papua New Guinea', 128, 1)
GO



insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-AU', 'Australia', 'English', 1, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('af', 'af-ZA', 'South Africa', 'Afrikaans', 2, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sq', 'sq-AL', 'Albanian', 'Albanian', 3, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-DZ', 'Algeria', 'Arabic', 4, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-BH', 'Bahrain', 'Arabic', 5, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-EG', 'Egypt', 'Arabic', 6, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-IQ', 'Iraq', 'Arabic', 7, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-JO', 'Jordan', 'Arabic', 8, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-KW', 'Kuwait', 'Arabic', 9, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-LB', 'Lebanon', 'Arabic', 10, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-LY', 'Libya', 'Arabic', 11, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-MA', 'Morocco', 'Arabic', 12, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-OM', 'Oman', 'Arabic', 13, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-QA', 'Qatar', 'Arabic', 14, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-SA', 'Saudi Arabia', 'Arabic', 15, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-SY', 'Syria', 'Arabic', 16, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-TN', 'Tunisia', 'Arabic', 17, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-AE', 'United Arab Emirates', 'Arabic', 18, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ar', 'ar-YE', 'Yemen', 'Arabic', 19, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('hy', 'hy-AM', 'Armenia', 'Armenian', 20, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('az-Cyrl', 'az-Cyrl-AZ', 'Azerbaijan', 'Cyrillic Azeri', 21, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('az-Latn', 'az-Latn-AZ', 'Azerbaijan', 'Latin Azeri', 22, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('eu', 'eu-ES', 'Spain', 'Basque', 23, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('be', 'be-BY', 'Belarus', 'Belarusian', 24, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('bs-Latn', 'bs-Latn-BA', 'Bosnia and Herzegovina', 'Latin Bosnian', 25, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('bs-Cyr', 'bs-Cyrl-BA', 'Bosnia and Herzegovina', 'Cyrillic Bosnian', 26, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('bg', 'bg-BG', 'Bulgaria', 'Bulgarian', 27, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ca', 'ca-ES', 'Spain', 'Catalan', 28, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('zh', 'zh-HK', 'Hong Kong S.A.R.', 'Chinese', 29, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('zh', 'zh-MO', 'Macao S.A.R.', 'Chinese', 30, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('zh', 'zh-CN', 'China', 'Chinese', 31, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('zh', 'zh-SG', 'Singapore', 'Chinese', 32, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('zh', 'zh-TW', 'Taiwan', 'Chinese', 33, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('hr', 'hr-BA', 'Bosnia and Herzegovina', 'Croatian', 34, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('hr', 'hr-HR', 'Croatia', 'Croatian', 35, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('cs', 'cs-CZ', 'Czech Republic', 'Czech', 36, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('da', 'da-DK', 'Denmark', 'Danish', 37, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('dv', 'dv-MV', 'Maldives', 'Divehi', 38, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('nl', 'nl-BE', 'Belgium', 'Dutch', 39, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('nl', 'nl-NL', 'Netherlands', 'Dutch', 40, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-BZ', 'Belize', 'English', 41, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-CA', 'Canada', 'English', 42, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-IE', 'Ireland', 'English', 43, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-JM', 'Jamaica', 'English', 44, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-NZ', 'New Zealand', 'English', 45, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-PH', 'Philippines', 'English', 46, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-ZA', 'South Africa', 'English', 47, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-TT', 'Trinidad and Tobago', 'English', 48, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-GB', 'United Kingdom', 'English', 49, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-US', 'United States', 'English', 50, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('en', 'en-ZW', 'Zimbabwe', 'English', 51, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('et', 'et-EE', 'Estonia', 'Estonian', 52, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fo', 'fo-FO', 'Faroe Islands', 'Faroese', 53, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fil', 'fil-PH', 'Philippines', 'Filipino', 54, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fi', 'fi-FI', 'Finland', 'Finnish', 55, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fr', 'fr-BE', 'Belgium', 'French', 56, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fr', 'fr-CA', 'Canada', 'French', 57, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fr', 'fr-FR', 'France', 'French', 58, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fr', 'fr-LU', 'Luxembourg', 'French', 59, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fr', 'fr-MC', 'Monaco', 'French', 60, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fr', 'fr-CH', 'Switzerland', 'French', 61, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fy', 'fy-NL', 'Netherlands', 'Frisian', 62, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('gl', 'gl-ES', 'Spain', 'Galician', 63, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ka', 'ka-GE', 'Georgia', 'Georgian', 64, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('de', 'de-AT', 'Austria', 'German', 65, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('de', 'de-DE', 'Germany', 'German', 66, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('de', 'de-LI', 'Liechtenstein', 'German', 67, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('de', 'de-LU', 'Luxembourg', 'German', 68, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('de', 'de-CH', 'Switzerland', 'German', 69, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('el', 'el-GR', 'Greece', 'Greek', 70, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('gu', 'gu-IN', 'India', 'Gujarati', 71, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('he', 'he-IL', 'Israel', 'Hebrew', 72, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('hi', 'hi-IN', 'India', 'Hindi', 73, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('hu', 'hu-HU', 'Hungary', 'Hungarian', 74, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('is', 'is-IS', 'Iceland', 'Icelandic', 75, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('id', 'id-ID', 'Indonesia', 'Indonesian', 76, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('iu-Latn', 'iu-Latn-CA', 'Canada', 'Latin Inuktitut', 77, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ga', 'ga-IE', 'Ireland', 'Irish', 78, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('it', 'it-IT', 'Italy', 'Italian', 79, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('it', 'it-CH', 'Switzerland', 'Italian', 80, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ja', 'ja-JP', 'Japan', 'Japanese', 81, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('kn', 'kn-IN', 'India', 'Kannada', 82, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('kk', 'kk-KZ', 'Kazakhstan', 'Kazakh', 83, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sw', 'sw-KE', 'Kenya', 'Kiswahili', 84, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('kok', 'kok-IN', 'India', 'Konkani', 85, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ko', 'ko-KR', 'South Korea', 'Korean', 86, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ky', 'ky-KG', 'Kyrgyzstan', 'Kyrgyz', 87, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('lv', 'lv-LV', 'Latvia', 'Latvian', 88, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('lt', 'lt-LT', 'Lithuania', 'Lithuanian', 89, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('lb', 'lb-LU', 'Luxembourg', 'Luxembourgish', 90, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('mk', 'mk-MK', 'Macedonia', 'Macedonian', 91, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ms', 'ms-BN', 'Brunei Darussalam', 'Malay', 92, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ms', 'ms-MY', 'Malaysia', 'Malay', 93, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('mt', 'mt-MT', 'Malta', 'Maltese', 94, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('mi', 'mi-NZ', 'New Zealand', 'Maori', 95, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('arn', 'arn-CL', 'Chile', 'Mapudungun', 96, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('mr', 'mr-IN', 'India', 'Marathi', 97, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('moh', 'moh-CA', 'Canada', 'Mohawk', 98, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('mn', 'mn-MN', 'Mongolia', 'Cyrillic Mongolian', 99, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('nb', 'nb-NO', 'Norway', 'Norwegian', 100, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('nn', 'nn-NO', 'Norway', 'Norwegian', 101, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('fa', 'fa-IR', 'Iran', 'Persian', 102, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('pl', 'pl-PL', 'Poland', 'Polish', 103, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('pt', 'pt-BR', 'Brazil', 'Portuguese', 104, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('pt', 'pt-PT', 'Portugal', 'Portuguese', 105, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('pa', 'pa-IN', 'India', 'Punjabi', 106, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('quz', 'quz-BO', 'Bolivia', 'Quechua', 107, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('quz', 'quz-EC', 'Ecuador', 'Quechua', 108, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('quz', 'quz-PE', 'Peru', 'Quechua', 109, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ro', 'ro-RO', 'Romania', 'Romanian', 110, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('rm', 'rm-CH', 'Switzerland', 'Romansh', 111, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ru', 'ru-RU', 'Russia', 'Russian', 112, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('smn', 'smn-FI', 'Finland', 'Sami, Inari', 113, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('smj', 'smj-NO', 'Norway', 'Sami, Lule', 114, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('smj', 'smj-SE', 'Sweden', 'Sami, Lule', 115, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('se', 'se-FI', 'Finland', 'Sami, Northern', 116, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('se', 'se-NO', 'Norway', 'Sami, Northern', 117, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('se', 'se-SE', 'Sweden', 'Sami, Northern', 118, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sms', 'sms-FI', 'Finland', 'Sami, Skolt', 119, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sma', 'sma-NO', 'Norway', 'Sami, Southern', 120, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sma', 'sma-SE', 'Sweden', 'Sami, Southern', 121, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sa', 'sa-IN', 'India', 'Sanskrit', 122, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sr-Cyrl', 'sr-Cyrl-BA', 'Bosnia and Herzegovina', 'Cyrillic Serbian', 123, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sr-Cyr', 'sr-Cyrl-CS', 'Serbia', 'Cyrillic Serbian', 124, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sr-Latn', 'sr-Latn-BA', 'Bosnia and Herzegovina', 'Latin Serbian', 125, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sr-Latn', 'sr-Latn-CS', 'Serbia', 'Serbian', 126, 1)
	
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-AR', 'Argentina', 'Spanish', 127, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-BO', 'Bolivia', 'Spanish', 128, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-CL', 'Chile', 'Spanish', 129, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-CO', 'Colombia', 'Spanish', 130, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-CR', 'Costa Rica', 'Spanish', 131, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-DO', 'Dominican Republic', 'Spanish', 132, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-EC', 'Ecuador', 'Spanish', 133, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-SV', 'El Salvador', 'Spanish', 134, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-GT', 'Guatemala', 'Spanish', 135, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-HN', 'Honduras', 'Spanish', 136, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-MX', 'Mexico', 'Spanish', 137, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-NI', 'Nicaragua', 'Spanish', 138, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-PA', 'Panama', 'Spanish', 139, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-PY', 'Paraguay', 'Spanish', 140, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-PE', 'Peru', 'Spanish', 141, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-PR', 'Puerto Rico', 'Spanish', 142, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-ES', 'Spain', 'Spanish', 143, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-UY', 'Uruguay', 'Spanish', 144, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('es', 'es-VE', 'Venezuela', 'Spanish', 145, 1)

insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ns', 'ns-ZA', 'South Africa', 'Sesotho sa Leboa', 146, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('tn', 'tn-ZA', 'South Africa', 'Setswana', 147, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sk', 'sk-SK', 'Slovakia', 'Slovak', 148, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sl', 'sl-SI', 'Slovenia', 'Slovenian', 149, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sv', 'sv-FI', 'Finland', 'Swedish', 150, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('sv', 'sv-SE', 'Sweden', 'Swedish', 151, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('syr', 'syr-SY', 'Syria', 'Syriac', 152, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ta', 'ta-IN', 'India', 'Tamil', 153, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('tt', 'tt-RU', 'Russia', 'Tatar', 154, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('te', 'te-IN', 'India', 'Telugu', 155, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('th', 'th-TH', 'Thailand', 'Thai', 156, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('tr', 'tr-TR', 'Turkey', 'Turkish', 157, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('uk', 'uk-UA', 'Ukraine', 'Ukrainian', 158, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('ur', 'ur-PK', 'Pakistan', 'Urdu', 159, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('uz-Cyrl', 'uz-Cyrl-UZ', 'Uzbekistan', 'Cyrillic Uzbek', 160, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('uz-Latn', 'uz-Latn-UZ', 'Uzbekistan', 'Latin Uzbek', 161, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('vi', 'vi-VN', 'Vietnam', 'Vietnamese', 162, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('cy', 'cy-GB', 'United Kingdom', 'Welsh', 163, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('xh', 'xh-ZA', 'South Africa', 'Xhosa', 164, 1)
insert into [dbo].[Language]([LanguageCode],[LanguageCountryCode],[CountryName],[LanguageName],[GroupOrder],[LanguageVisible])VALUES('zu', 'zu-ZA', 'South Africa', 'Zulu', 165, 1)
GO 



INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Aus Eastern Standard Time', 'Melbourne', '+ 10 H', 1, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('E. Australia Standard Time', 'Brisbane', '+ 9 H', 2, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('AUS Central Standard Time', 'Darwin', '+ 9.5 H', 3, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Cen. Australia Standard Time', 'Adelaide', '+ 9.5 H', 4, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Tasmania Standard Time', 'Hobart', '+ 10 H', 5, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Australia Standard Time', 'Perth', '+ 8 H', 6, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Aus Eastern Standard Time', 'Canberra', '+ 10 H', 7, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Aus Eastern Standard Time', 'Sydney', '+ 10 H', 8, 1, 'Australia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Afghanistan Standard Time', 'Kabul', '+ 4 H', 9, 1, 'Afghanistan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Alaskan Standard Time', 'Anchorage', '- 9 H', 10, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Arab Standard Time', 'Riyadh', '+ 3 H', 11, 1, 'Saudi Arabia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Arab Standard Time', 'Kuwai', '+ 3 H', 12, 1, 'Kuwait')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Arabian Standard Time', 'Dubai', '+ 4 H', 13, 1, 'United Arab Emirates')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Arabian Standard Time', 'Abu Dhabi', '+ 4 H', 14, 1, 'United Arab Emirates')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Arabian Standard Time', 'Muscat', '+ 4 H', 15, 1, 'Oman')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Arabic Standard Time', 'Baghdad', '+ 3 H', 16, 1, 'Iraq')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Argentina Standard Time', 'Buenos Aires', '- 3 H', 17, 1, 'Argentina')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Armenian Standard Time', 'Yerevan', '+ 4 H', 18, 1, 'Armenia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Atlantic Standard Time', 'Halifax', '- 4 H', 19, 1, 'Canada')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Azerbaijan Standard Time', 'Baku', '+ 4 H', 20, 1, 'Azerbaijan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Azores Standard Time', 'Azores', '- 1 H', 21, 1, 'Portugal')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Canada Central Standard Time', 'Regina', '- 6 H', 22, 1, 'Canada')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Cape Verde Standard Time', 'Cape Verde', '- 1 H', 23, 1, 'Portugal')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Caucasus Standard Time', 'Tbilisi', '+ 4 H', 24, 1, 'Georgia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central America Standard Time', 'Guatemala', '- 6 H', 25, 1, 'Guatemala')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Asia Standard Time', 'Dhaka', '+ 6 H', 26, 1, 'Bangladesh')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Asia Standard Time', 'Astana', '+ 6 H', 27, 1, 'Kazakhstan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Brazilian Standard Time', 'Manaus', '- 4 H', 28, 1, 'Brazil')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Europe Standard Time', 'Budapest', '+ 1 H', 29, 1, 'Hungary')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Europe Standard Time', 'Belgrade', '+ 1 H', 30, 1, 'Serbia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Europe Standard Time', 'Bratislava', '+ 1 H', 31, 1, 'Slovakia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Europe Standard Time', 'Ljubljana', '+ 1 H', 32, 1, 'Slovenia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Europe Standard Time', 'Prague', '+ 1 H', 33, 1, 'Czech Republic')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central European Standard Time', 'Sarajevo', '+ 1 H', 34, 1, 'Bosnia and Herzegovina')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central European Standard Time', 'Skopje', '+ 1 H', 35, 1, 'Macedonia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central European Standard Time', 'Warsaw', '+ 1 H', 36, 1, 'Poland')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central European Standard Time', 'Zagreb', '+ 1 H', 37, 1, 'Croatia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Pacific Standard Time', 'Guadalcanal', '+ 11 H', 38, 1, 'Solomon Islands')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Pacific Standard Time', 'Magadan', '+ 11 H', 39, 1, 'New Caledonia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Pacific Standard Time', 'Solomon Is.', '+ 11 H', 40, 1, 'Solomon Islands')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Pacific Standard Time', 'New Caledonia', '+ 11 H', 41, 1, 'New Caledonia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Central Standard Time', 'Chicago', '- 6 H', 42, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mexico Standard Time', 'Guadalajara', '- 6 H', 43, 1, 'Mexico')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mexico Standard Time', 'Mexico City', '- 6 H', 44, 1, 'Mexico')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mexico Standard Time', 'Monterrey', '- 6 H', 45, 1, 'Mexico')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('China Standard Time', 'Shanghai', '+ 8 H', 46, 1, 'China')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('China Standard Time', 'Beijing', '+ 8 H', 47, 1, 'China')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('China Standard Time', 'Chongqing', '+ 8 H', 48, 1, 'China')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('China Standard Time', 'Hong Kong', '+ 8 H', 49, 1, 'China')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('China Standard Time', 'Urumqi', '+ 8 H', 50, 1, 'China')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('E. Africa Standard Time', 'Nairobi', '+ 3 H', 51, 1, 'Kenya')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('E. Europe Standard Time', 'Minsk', '+ 2 H', 52, 1, 'Belarus')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('E. South America Standard Time', 'Sao Paulo', '- 3 H', 53, 1, 'Brazil')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Eastern Standard Time', 'New York', '- 5 H', 54, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Egypt Standard Time', 'Cairo', '+ 2 H', 55, 1, 'Egypt')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Ekaterinburg Standard Time', 'Yekaterinburg', '+ 5 H', 56, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Fiji Standard Time', 'Fiji', '+ 12 H', 57, 1, 'Fiji')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Fiji Standard Time', 'Kamchatka', '+ 12 H', 58, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Fiji Standard Time', 'Marshall Is.', '+ 12 H', 59, 1, 'Marshall Islands')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('FLE Standard Time', 'Helsinki', '+ 2 H', 60, 1, 'Finland')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('FLE Standard Time', 'Kiev', '+ 2 H', 61, 1, 'Ukraine')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('FLE Standard Time', 'Riga', '+ 2 H', 62, 1, 'Latvia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('FLE Standard Time', 'Sofia', '+ 2 H', 63, 1, 'Bulgaria')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('FLE Standard Time', 'Tallinn', '+ 2 H', 64, 1, 'Estonia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('FLE Standard Time', 'Vilnius', '+ 2 H', 65, 1, 'Lithuania')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Georgian Standard Time', 'Tbilisi', '+ 3 H', 66, 1, 'Georgia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('GMT Standard Time', 'Dublin', '+ 0 H', 67, 1, 'Ireland')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('GMT Standard Time', 'Edinburgh', '+ 0 H', 68, 1, 'United Kingdom')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('GMT Standard Time', 'Lisbon', '+ 0 H', 69, 1, 'Portugal')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('GMT Standard Time', 'London', '+ 0 H', 70, 1, 'United Kingdom')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Greenland Standard Time', 'Godthab', '- 3 H', 71, 1, 'GreenLand')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Greenwich Standard Time', 'Casablanca', '+ 0 H', 72, 1, 'Monaco')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Greenwich Standard Time', 'Monrovia', '+ 0 H', 73, 1, 'Liberia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Greenwich Standard Time', 'Reykjavik', '+ 0 H', 74, 1, 'Iceland')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('GTB Standard Time', 'Athens', '+ 2 H', 75, 1, 'Greece')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('GTB Standard Time', 'Bucharest', '+ 2 H', 76, 1, 'Romania')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('GTB Standard Time', 'Istanbul', '+ 2 H', 77, 1, 'Turkey')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Hawaiian Standard Time', 'Honolulu', '- 10 H', 78, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('India Standard Time', 'Calcutta', '+ 5.5 H', 79, 1, 'India')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('India Standard Time', 'Chennai', '+ 5.5 H', 80, 1, 'India')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('India Standard Time', 'Kolkata', '+ 5.5 H', 81, 1, 'India')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('India Standard Time', 'Mumbai', '+ 5.5 H', 82, 1, 'India')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('India Standard Time', 'New Delhi', '+ 5.5 H', 83, 1, 'India')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Iran Standard Time', 'Tehran', '+ 3.5 H', 84, 1, 'Iran')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Israel Standard Time', 'Jerusalem', '+ 2 H', 85, 1, 'Israel')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Jordan Standard Time', 'Amman', '+ 2 H', 86, 1, 'Jordan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Korea Standard Time', 'Seoul', '+ 9 H', 87, 1, 'South Korea')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mexico Standard Time 2', 'Chihuahua', '- 7 H', 88, 1, 'Mexico')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mexico Standard Time 2', 'La Paz', '- 7 H', 89, 1, 'Bolivia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mexico Standard Time 2', 'Mazatlan', '- 7 H', 90, 1, 'Mexico')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mid-Atlantic Standard Time', 'South Georgia', '- 2 H', 91, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Middle East Standard Time', 'Beirut', '+ 2 H', 92, 1, 'Lebanon')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Montevideo Standard Time', 'Montevideo', '- 3 H', 93, 1, 'Uruguay')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Mountain Standard Time', 'Denver', '- 7 H', 94, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Myanmar Standard Time', 'Rangoon', '+ 6.5 H', 95, 1, 'Myanmar')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('N. Central Asia Standard Time', 'Almaty', '+ 6 H', 96, 1, 'Kazakhstan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('N. Central Asia Standard Time', 'Novosibirsk', '+ 6 H', 97, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Namibia Standard Time', 'Windhoek', '+ 2 H', 98, 1, 'Namibia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Nepal Standard Time', 'Katmandu', '+ 5.75 H', 99, 1, 'Nepal')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('New Zealand Standard Time', 'Auckland', '+ 12 H', 100, 1, 'New Zealand')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('New Zealand Standard Time', 'Wellington', '+ 12 H', 101, 1, 'New Zealand')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Newfoundland Standard Time', 'St Johns', '- 3.5 H', 102, 1, 'Newfoundland')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('North Asia East Standard Time', 'Irkutsk', '+ 8 H', 103, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('North Asia East Standard Time', 'Ulaan Bataar', '+ 8 H', 104, 1, 'Mongolia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('North Asia Standard Time', 'Krasnoyarsk', '+ 7 H', 105, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Pacific SA Standard Time', 'Santiago', '- 4 H', 106, 1, 'Chile')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Pacific Standard Time', 'Los Angeles', '- 8 H', 107, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Pacific Standard Time', 'Tijuana', '- 8 H', 108, 1, 'Mexico')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Pacific Standard Time', 'Baja California', '- 8 H', 109, 1, 'Mexico')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Romance Standard Time', 'Brussels', '+ 1 H', 110, 1, 'Belgium')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Romance Standard Time', 'Copenhagen', '+ 1 H', 111, 1, 'Denmark')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Romance Standard Time', 'Madrid', '+ 1 H', 112, 1, 'Spain')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Romance Standard Time', 'Paris', '+ 1 H', 113, 1, 'France')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Russian Standard Time', 'Moscow', '+ 3 H', 114, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Russian Standard Time', 'St.Petersburg', '+ 3 H', 115, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Russian Standard Time', 'Volgograd', '+ 3 H', 116, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SA Eastern Standard Time', 'Georgetown', '- 3 H', 117, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SA Eastern Standard Time', 'Bogota', '- 5 H', 118, 1, 'Colombia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SA Eastern Standard Time', 'Lima', '- 5 H', 119, 1, 'Peru')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SA Eastern Standard Time', 'Quito', '- 5 H', 120, 1, 'Ecuador')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SA Eastern Standard Time', 'Rio Branco', '- 5 H', 121, 1, 'Brazil')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Samoa Standard Time', 'Apia', '- 11 H', 122, 1, 'Samoa')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Samoa Standard Time', 'Samoa', '- 11 H', 123, 1, 'Samoa')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SE Asia Standard Time', 'Bangkok', '+ 7 H', 124, 1, 'Thailand')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SE Asia Standard Time', 'Hanoi', '+ 7 H', 125, 1, 'Vietnam')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('SE Asia Standard Time', 'Jakarta', '+ 7 H', 126, 1, 'Indonesia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Singapore Standard Time', 'Kuala Lumpur', '+ 8 H', 127, 1, 'Malaysia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Singapore Standard Time', 'Singapore', '+ 8 H', 128, 1, 'Singapore')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('South Africa Standard Time', 'Johannesburg', '+ 2 H', 129, 1, 'South Africa')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('South Africa Standard Time', 'Harare', '+ 2 H', 130, 1, 'Zimbabwe')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('South Africa Standard Time', 'Pretoria', '+ 2 H', 131, 1, 'South Africa')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Sri Lanka Standard Time', 'Colombo', '+ 5.5 H', 132, 1, 'Sri Lanka')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Sri Lanka Standard Time', 'Sri Jayawardenepura', '+ 5.5 H', 133, 1, 'Sri Lanka')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Taipei Standard Time', 'Taipei', '+ 8 H', 134, 1, 'Taiwan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Tokyo Standard Time', 'Tokyo', '+ 9 H', 135, 1, 'Japan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Tokyo Standard Time', 'Sapporo', '+ 9 H', 136, 1, 'Japan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Tokyo Standard Time', 'Osaka', '+ 9 H', 137, 1, 'Japan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Tonga Standard Time', 'Tongatapu', '+ 13 H', 138, 1, 'Tonga')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('US Eastern Standard Time', 'Indiana', '- 5 H', 139, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('US Mountain Standard Time', 'Phoenix', '- 7 H', 140, 1, 'United States')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Venezuela Standard Time', 'Caracas', '- 4.5 H', 141, 1, 'Venezuela')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Vladivostok Standard Time', 'Vladivostok', '+ 10 H', 142, 1, 'Russia')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Central Africa Standard Time', 'Lagos', '+ 1 H', 143, 1, 'Nigeria')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Europe Standard Time', 'Amsterdam', '+ 1 H', 144, 1, 'Netherlands')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Europe Standard Time', 'Berlin', '+ 1 H', 145, 1, 'Germany')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Europe Standard Time', 'Bern', '+ 1 H', 146, 1, 'Switzerland')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Europe Standard Time', 'Rome', '+ 1 H', 147, 1, 'Italy')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Europe Standard Time', 'Stockholm', '+ 1 H', 148, 1, 'Sweden')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('W. Europe Standard Time', 'Vienna', '+ 1 H', 149, 1, 'Austria')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('West Asia Standard Time', 'Islamabad', '+ 5 H', 150, 1, 'Pakistan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('West Asia Standard Time', 'Karachi', '+ 5 H', 151, 1, 'Pakistan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('West Asia Standard Time', 'Tashkent', '+ 5 H', 152, 1, 'Uzbekistan')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('West Pacific Standard Time', 'Port Moresby', '+ 10 H', 153, 1, 'Papua New Guinea')
INSERT INTO dbo.InternationalTimeZone(SystemTimeZoneName, DisplayTimeZoneName, UniversalTimeOffset, GroupOrder, TimeZoneVisible, CountryName)VALUES('Yakutsk Standard Time', 'Yakutsk', '+ 9 H', 154, 1, 'Russia')
GO

INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Australia','Australia',1,0,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('AfroEurasia','Middle east countries',2,1,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Eurasia','European and asia',3,2,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Asia','Central asia',4,3,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Africa','Central and southern',5,4,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Americas','Central america',6,5,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('NorthAmerica','Nothern america',7,6,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('SouthAmerica','Southern america',8,7,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Oceania','Southern and eastern pacific',9,8,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Europe','Central and western europe',10,9,1)
INSERT INTO dbo.Continent([ContinentName],[ContinentDescription],[ContinentCodeID],[ContinentGroupOrder],[ContinentVisible])VALUES('Antarctica','Antarctica',11,10,1)
GO

UPDATE [dbo].[Country] SET [ContinentID] = 1 WHERE [CountryID] = 1
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 2
UPDATE [dbo].[Country] SET [ContinentID] = 5 WHERE [CountryID] = 3
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 4
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 5
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 6
UPDATE [dbo].[Country] SET [ContinentID] = 3 WHERE [CountryID] = 7
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 8
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 9
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 10
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 11
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 12
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 13
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 14
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 15
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 16
UPDATE [dbo].[Country] SET [ContinentID] = 7 WHERE [CountryID] = 17
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 18
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 19
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 20
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 21
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 22
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 23
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 24
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 25
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 26
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 27
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 28
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 29
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 30
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 31
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 32
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 33
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 34
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 35
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 36
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 37
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 38
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 39
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 40
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 41
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 42
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 43
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 44
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 45
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 46
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 47
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 48
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 49
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 50
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 51
UPDATE [dbo].[Country] SET [ContinentID] = 5 WHERE [CountryID] = 52
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 53
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 54
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 55
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 56
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 57
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 58
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 59
UPDATE [dbo].[Country] SET [ContinentID] = 3 WHERE [CountryID] = 60
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 61
UPDATE [dbo].[Country] SET [ContinentID] = 5 WHERE [CountryID] = 62
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 63
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 64
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 65
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 66
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 67
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 68
UPDATE [dbo].[Country] SET [ContinentID] = 5 WHERE [CountryID] = 69
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 70
UPDATE [dbo].[Country] SET [ContinentID] = 11 WHERE [CountryID] = 71
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 72
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 73
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 74
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 75
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 76
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 77
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 78
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 79
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 80
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 81
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 82
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 83
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 84
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 85
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 86
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 87
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 88
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 89
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 90
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 91
UPDATE [dbo].[Country] SET [ContinentID] = 5 WHERE [CountryID] = 92
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 93
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 94
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 95
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 96
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 97
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 98
UPDATE [dbo].[Country] SET [ContinentID] = 6 WHERE [CountryID] = 99
UPDATE [dbo].[Country] SET [ContinentID] = 5 WHERE [CountryID] = 100
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 101
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 102
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 103
UPDATE [dbo].[Country] SET [ContinentID] = 10 WHERE [CountryID] = 104
UPDATE [dbo].[Country] SET [ContinentID] = 7 WHERE [CountryID] = 105
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 106
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 107
UPDATE [dbo].[Country] SET [ContinentID] = 8 WHERE [CountryID] = 108
UPDATE [dbo].[Country] SET [ContinentID] = 4 WHERE [CountryID] = 109
UPDATE [dbo].[Country] SET [ContinentID] = 2 WHERE [CountryID] = 110
UPDATE [dbo].[Country] SET [ContinentID] = 5 WHERE [CountryID] = 111
GO

UPDATE [dbo].[Country] SET [CountryDialCode] = '+61' WHERE [CountryID] = 1
UPDATE [dbo].[Country] SET [CountryDialCode] = '+355' WHERE [CountryID] = 2
UPDATE [dbo].[Country] SET [CountryDialCode] = '+213' WHERE [CountryID] = 3
UPDATE [dbo].[Country] SET [CountryDialCode] = '+54' WHERE [CountryID] = 4
UPDATE [dbo].[Country] SET [CountryDialCode] = '+374' WHERE [CountryID] = 5
UPDATE [dbo].[Country] SET [CountryDialCode] = '+43' WHERE [CountryID] = 6
UPDATE [dbo].[Country] SET [CountryDialCode] = '+994' WHERE [CountryID] = 7
UPDATE [dbo].[Country] SET [CountryDialCode] = '+973' WHERE [CountryID] = 8
UPDATE [dbo].[Country] SET [CountryDialCode] = '+375' WHERE [CountryID] = 9
UPDATE [dbo].[Country] SET [CountryDialCode] = '+32' WHERE [CountryID] = 10
UPDATE [dbo].[Country] SET [CountryDialCode] = '+501' WHERE [CountryID] = 11
UPDATE [dbo].[Country] SET [CountryDialCode] = '+591' WHERE [CountryID] = 12
UPDATE [dbo].[Country] SET [CountryDialCode] = '+387' WHERE [CountryID] = 13
UPDATE [dbo].[Country] SET [CountryDialCode] = '+55' WHERE [CountryID] = 14
UPDATE [dbo].[Country] SET [CountryDialCode] = '+673' WHERE [CountryID] = 15
UPDATE [dbo].[Country] SET [CountryDialCode] = '+359' WHERE [CountryID] = 16
UPDATE [dbo].[Country] SET [CountryDialCode] = '+1' WHERE [CountryID] = 17
UPDATE [dbo].[Country] SET [CountryDialCode] = '+56' WHERE [CountryID] = 18
UPDATE [dbo].[Country] SET [CountryDialCode] = '+57' WHERE [CountryID] = 19
UPDATE [dbo].[Country] SET [CountryDialCode] = '+506' WHERE [CountryID] = 20
UPDATE [dbo].[Country] SET [CountryDialCode] = '+385' WHERE [CountryID] = 21
UPDATE [dbo].[Country] SET [CountryDialCode] = '+420' WHERE [CountryID] = 22
UPDATE [dbo].[Country] SET [CountryDialCode] = '+45' WHERE [CountryID] = 23
UPDATE [dbo].[Country] SET [CountryDialCode] = '+1-809+1-829' WHERE [CountryID] = 24
UPDATE [dbo].[Country] SET [CountryDialCode] = '+593' WHERE [CountryID] = 25
UPDATE [dbo].[Country] SET [CountryDialCode] = '+20' WHERE [CountryID] = 26
UPDATE [dbo].[Country] SET [CountryDialCode] = '+503' WHERE [CountryID] = 27
UPDATE [dbo].[Country] SET [CountryDialCode] = '+372' WHERE [CountryID] = 28
UPDATE [dbo].[Country] SET [CountryDialCode] = '+298' WHERE [CountryID] = 29
UPDATE [dbo].[Country] SET [CountryDialCode] = '+358' WHERE [CountryID] = 30
UPDATE [dbo].[Country] SET [CountryDialCode] = '+33' WHERE [CountryID] = 31
UPDATE [dbo].[Country] SET [CountryDialCode] = '+995' WHERE [CountryID] = 32
UPDATE [dbo].[Country] SET [CountryDialCode] = '+49' WHERE [CountryID] = 33
UPDATE [dbo].[Country] SET [CountryDialCode] = '+30' WHERE [CountryID] = 34
UPDATE [dbo].[Country] SET [CountryDialCode] = '+502' WHERE [CountryID] = 35
UPDATE [dbo].[Country] SET [CountryDialCode] = '+504' WHERE [CountryID] = 36
UPDATE [dbo].[Country] SET [CountryDialCode] = '+852' WHERE [CountryID] = 37
UPDATE [dbo].[Country] SET [CountryDialCode] = '+36' WHERE [CountryID] = 38
UPDATE [dbo].[Country] SET [CountryDialCode] = '+354' WHERE [CountryID] = 39
UPDATE [dbo].[Country] SET [CountryDialCode] = '+91' WHERE [CountryID] = 40
UPDATE [dbo].[Country] SET [CountryDialCode] = '+62' WHERE [CountryID] = 41
UPDATE [dbo].[Country] SET [CountryDialCode] = '+98' WHERE [CountryID] = 42
UPDATE [dbo].[Country] SET [CountryDialCode] = '+964' WHERE [CountryID] = 43
UPDATE [dbo].[Country] SET [CountryDialCode] = '+353' WHERE [CountryID] = 44
UPDATE [dbo].[Country] SET [CountryDialCode] = '+92' WHERE [CountryID] = 45
UPDATE [dbo].[Country] SET [CountryDialCode] = '+972' WHERE [CountryID] = 46
UPDATE [dbo].[Country] SET [CountryDialCode] = '+39' WHERE [CountryID] = 47
UPDATE [dbo].[Country] SET [CountryDialCode] = '+1-876' WHERE [CountryID] = 48
UPDATE [dbo].[Country] SET [CountryDialCode] = '+81' WHERE [CountryID] = 49
UPDATE [dbo].[Country] SET [CountryDialCode] = '+962' WHERE [CountryID] = 50
UPDATE [dbo].[Country] SET [CountryDialCode] = '+7' WHERE [CountryID] = 51
UPDATE [dbo].[Country] SET [CountryDialCode] = '+254' WHERE [CountryID] = 52
UPDATE [dbo].[Country] SET [CountryDialCode] = '+82' WHERE [CountryID] = 53
UPDATE [dbo].[Country] SET [CountryDialCode] = '+965' WHERE [CountryID] = 54
UPDATE [dbo].[Country] SET [CountryDialCode] = '+996' WHERE [CountryID] = 55
UPDATE [dbo].[Country] SET [CountryDialCode] = '+371' WHERE [CountryID] = 56
UPDATE [dbo].[Country] SET [CountryDialCode] = '+961' WHERE [CountryID] = 57
UPDATE [dbo].[Country] SET [CountryDialCode] = '+218' WHERE [CountryID] = 58
UPDATE [dbo].[Country] SET [CountryDialCode] = '+423' WHERE [CountryID] = 59
UPDATE [dbo].[Country] SET [CountryDialCode] = '+370' WHERE [CountryID] = 60
UPDATE [dbo].[Country] SET [CountryDialCode] = '+352' WHERE [CountryID] = 61
UPDATE [dbo].[Country] SET [CountryDialCode] = '' WHERE [CountryID] = 62
UPDATE [dbo].[Country] SET [CountryDialCode] = '+389' WHERE [CountryID] = 63
UPDATE [dbo].[Country] SET [CountryDialCode] = '+60' WHERE [CountryID] = 64
UPDATE [dbo].[Country] SET [CountryDialCode] = '+960' WHERE [CountryID] = 65
UPDATE [dbo].[Country] SET [CountryDialCode] = '+356' WHERE [CountryID] = 66
UPDATE [dbo].[Country] SET [CountryDialCode] = '+52' WHERE [CountryID] = 67
UPDATE [dbo].[Country] SET [CountryDialCode] = '+976' WHERE [CountryID] = 68
UPDATE [dbo].[Country] SET [CountryDialCode] = '+212' WHERE [CountryID] = 69
UPDATE [dbo].[Country] SET [CountryDialCode] = '+31' WHERE [CountryID] = 70
UPDATE [dbo].[Country] SET [CountryDialCode] = '+64' WHERE [CountryID] = 71
UPDATE [dbo].[Country] SET [CountryDialCode] = '+505' WHERE [CountryID] = 72
UPDATE [dbo].[Country] SET [CountryDialCode] = '+47' WHERE [CountryID] = 73
UPDATE [dbo].[Country] SET [CountryDialCode] = '+968' WHERE [CountryID] = 74
UPDATE [dbo].[Country] SET [CountryDialCode] = '+507' WHERE [CountryID] = 75
UPDATE [dbo].[Country] SET [CountryDialCode] = '+595' WHERE [CountryID] = 76
UPDATE [dbo].[Country] SET [CountryDialCode] = '+86' WHERE [CountryID] = 77
UPDATE [dbo].[Country] SET [CountryDialCode] = '+51' WHERE [CountryID] = 78
UPDATE [dbo].[Country] SET [CountryDialCode] = '+63' WHERE [CountryID] = 79
UPDATE [dbo].[Country] SET [CountryDialCode] = '+48' WHERE [CountryID] = 80
UPDATE [dbo].[Country] SET [CountryDialCode] = '+351' WHERE [CountryID] = 81
UPDATE [dbo].[Country] SET [CountryDialCode] = '+377' WHERE [CountryID] = 82
UPDATE [dbo].[Country] SET [CountryDialCode] = '+1' WHERE [CountryID] = 83
UPDATE [dbo].[Country] SET [CountryDialCode] = '+974' WHERE [CountryID] = 84
UPDATE [dbo].[Country] SET [CountryDialCode] = '+40' WHERE [CountryID] = 85
UPDATE [dbo].[Country] SET [CountryDialCode] = '+7' WHERE [CountryID] = 86
UPDATE [dbo].[Country] SET [CountryDialCode] = '+966' WHERE [CountryID] = 87
UPDATE [dbo].[Country] SET [CountryDialCode] = '+381' WHERE [CountryID] = 88
UPDATE [dbo].[Country] SET [CountryDialCode] = '+65' WHERE [CountryID] = 89
UPDATE [dbo].[Country] SET [CountryDialCode] = '+421' WHERE [CountryID] = 90
UPDATE [dbo].[Country] SET [CountryDialCode] = '+386' WHERE [CountryID] = 91
UPDATE [dbo].[Country] SET [CountryDialCode] = '+27' WHERE [CountryID] = 92
UPDATE [dbo].[Country] SET [CountryDialCode] = '+34' WHERE [CountryID] = 93
UPDATE [dbo].[Country] SET [CountryDialCode] = '+46' WHERE [CountryID] = 94
UPDATE [dbo].[Country] SET [CountryDialCode] = '+41' WHERE [CountryID] = 95
UPDATE [dbo].[Country] SET [CountryDialCode] = '+963' WHERE [CountryID] = 96
UPDATE [dbo].[Country] SET [CountryDialCode] = '+886' WHERE [CountryID] = 97
UPDATE [dbo].[Country] SET [CountryDialCode] = '+66' WHERE [CountryID] = 98
UPDATE [dbo].[Country] SET [CountryDialCode] = '+1-868' WHERE [CountryID] = 99
UPDATE [dbo].[Country] SET [CountryDialCode] = '+216' WHERE [CountryID] = 100
UPDATE [dbo].[Country] SET [CountryDialCode] = '+90' WHERE [CountryID] = 101
UPDATE [dbo].[Country] SET [CountryDialCode] = '+971' WHERE [CountryID] = 102
UPDATE [dbo].[Country] SET [CountryDialCode] = '+380' WHERE [CountryID] = 103
UPDATE [dbo].[Country] SET [CountryDialCode] = '+44' WHERE [CountryID] = 104
UPDATE [dbo].[Country] SET [CountryDialCode] = '+1' WHERE [CountryID] = 105
UPDATE [dbo].[Country] SET [CountryDialCode] = '+598' WHERE [CountryID] = 106
UPDATE [dbo].[Country] SET [CountryDialCode] = '+998' WHERE [CountryID] = 107
UPDATE [dbo].[Country] SET [CountryDialCode] = '+58' WHERE [CountryID] = 108
UPDATE [dbo].[Country] SET [CountryDialCode] = '+84' WHERE [CountryID] = 109
UPDATE [dbo].[Country] SET [CountryDialCode] = '+967' WHERE [CountryID] = 110
UPDATE [dbo].[Country] SET [CountryDialCode] = '+263' WHERE [CountryID] = 111
GO


INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'NSW','New South Wales',0,1,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'ACT','Australian Capital Territory',1,2,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'VIC','Victoria',2,3,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'QLD','Queensland',3,4,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'TAS','Tasmania',4,5,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'SA','South Australia',5,6,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'NT','Northern Territory',6,7,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(1,'WA','Western Australia',7,8,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'AL','Alabama',0,9,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'AK','Alaska',1,10,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'AZ','Arizona',2,11,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'AR','Arkansas',3,12,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'CA','California',4,13,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'CO','Colorado',5,14,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'CT','Connecticut',6,15,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'DE','Delaware',7,16,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'FL','Florida',8,17,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'GA','Georgia',9,18,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'HI','Hawaii',10,19,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'ID','Idaho',11,20,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'IL','Illinois',12,21,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'IN','Indiana',13,22,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'IA','Iowa',14,23,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'KS','Kansas',15,24,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'KY','Kentucky',16,25,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'LA','Louisiana',17,26,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'ME','Maine',18,27,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'MD','Maryland',19,28,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'MA','Massachusetts',20,29,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'MI','Michigan',21,30,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'MN','Minnesota',22,31,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'MS','Mississippi',23,32,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'MO','Missouri',24,33,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'MT','Montana',25,34,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'NE','Nebraska',26,35,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'NV','Nevada',27,36,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'NH','New Hampshire',28,37,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'NJ','New Jersey',29,38,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'NM','New Mexico',30,39,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'NY','New York',31,40,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'NC','North Carolina',32,41,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'ND','North Dakota',33,42,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'OH','Ohio',34,43,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'OK','Oklahoma',35,44,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'OR','Oregon',36,45,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'PA','Pennsylvania',37,46,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'RI','Rhode Island',38,47,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'SC','South Carolina',39,48,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'SD','South Dakota',40,49,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'TN','Tennessee',41,50,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'TX','Texas',42,51,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'UT','Utah',43,52,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'VT','Vermont',44,53,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'VA','Virginia',45,54,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'WA','Washington',46,55,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'WV','West Virginia',47,56,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'WI','Wisconsin',48,57,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(105,'WY','Wyoming',49,58,1)
GO

INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 15)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 16)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 17)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 18)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 22)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 25)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 27)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 28)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 29)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 30)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 37)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 38)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 40)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 41)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 43)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 46)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 47)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 48)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 50)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 53)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 54)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(54, 56)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 9)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 12)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 17)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 21)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 22)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 23)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 24)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 25)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 26)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 30)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 31)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 32)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 33)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 35)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 42)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 44)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 49)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 50)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(42, 57)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 11)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 14)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 20)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 24)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 34)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 35)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 36)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 39)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 42)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 45)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 49)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 51)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 52)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(94, 58)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(107, 13)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(107, 20)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(107, 36)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(107, 45)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(107, 55)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(10, 10)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(78, 10)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(78, 19)
GO

INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(104,'EN','England',50,59,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(104,'WA','Wales',51,60,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(104,'SC','Scotland',52,61,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(104,'NI','Northern Ireland',53,62,1)
GO

INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(70, 59)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(68, 61)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(70, 60)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(70, 62)
GO

INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'AUK','Auckland',54,63,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'BOP','Bay of Plenty',55,64,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'CAN','Canterbury',56,65,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'CI','Chatham Islands',57,66,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'GIS','Gisborne',58,67,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'NKB','Hawkes Bay',59,68,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'MWT','Manawatu-Wanganui',60,69,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'MBH','Marlborough',61,70,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'NSN','Nelson',62,71,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'NTL','Northland',63,72,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'OTA','Otago',64,73,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'STL','Southland',65,74,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'TKI','Taranaki',66,75,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'TAS','Tasman',67,76,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'WKO','Waikato',68,77,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'WGN','Wellington',69,78,1)
INSERT INTO [dbo].[State]([CountryID],[StateShortName],[StateLongName],[StateCodeID],[GroupOrder],[StateVisible])VALUES(71,'WTC','West Coast',70,79,1)
GO

INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 63)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 64)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 65)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 66)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 67)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 68)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 69)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(100, 70)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 71)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 72)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 73)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 74)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 75)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 76)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 77)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 78)
INSERT INTO [dbo].[StateTimeZone]([InternationalTimeZoneID],[StateID]) VALUES(101, 79)
GO