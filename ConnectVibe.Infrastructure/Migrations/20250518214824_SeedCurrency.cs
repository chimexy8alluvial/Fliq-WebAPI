using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CurrencyCode", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                    { 1, "AED", DateTime.UtcNow, null, false }, // United Arab Emirates Dirham
                    { 2, "AFN", DateTime.UtcNow, null, false }, // Afghan Afghani
                    { 3, "ALL", DateTime.UtcNow, null, false }, // Albanian Lek
                    { 4, "AMD", DateTime.UtcNow, null, false }, // Armenian Dram
                    { 5, "ANG", DateTime.UtcNow, null, false }, // Netherlands Antillean Guilder
                    { 6, "AOA", DateTime.UtcNow, null, false }, // Angolan Kwanza
                    { 7, "ARS", DateTime.UtcNow, null, false }, // Argentine Peso
                    { 8, "AUD", DateTime.UtcNow, null, false }, // Australian Dollar
                    { 9, "AWG", DateTime.UtcNow, null, false }, // Aruban Florin
                    { 10, "AZN", DateTime.UtcNow, null, false }, // Azerbaijani Manat
                    { 11, "BAM", DateTime.UtcNow, null, false }, // Bosnia-Herzegovina Convertible Mark
                    { 12, "BBD", DateTime.UtcNow, null, false }, // Barbadian Dollar
                    { 13, "BDT", DateTime.UtcNow, null, false }, // Bangladeshi Taka
                    { 14, "BGN", DateTime.UtcNow, null, false }, // Bulgarian Lev
                    { 15, "BHD", DateTime.UtcNow, null, false }, // Bahraini Dinar
                    { 16, "BIF", DateTime.UtcNow, null, false }, // Burundian Franc
                    { 17, "BMD", DateTime.UtcNow, null, false }, // Bermudian Dollar
                    { 18, "BND", DateTime.UtcNow, null, false }, // Brunei Dollar
                    { 19, "BOB", DateTime.UtcNow, null, false }, // Bolivian Boliviano
                    { 20, "BRL", DateTime.UtcNow, null, false }, // Brazilian Real
                    { 21, "BSD", DateTime.UtcNow, null, false }, // Bahamian Dollar
                    { 22, "BTN", DateTime.UtcNow, null, false }, // Bhutanese Ngultrum
                    { 23, "BWP", DateTime.UtcNow, null, false }, // Botswanan Pula
                    { 24, "BYN", DateTime.UtcNow, null, false }, // Belarusian Rubles
                    { 25, "BZD", DateTime.UtcNow, null, false }, // Belize Dollar
                    { 26, "CAD", DateTime.UtcNow, null, false }, // Canadian Dollar
                    { 27, "CDF", DateTime.UtcNow, null, false }, // Congolese Franc
                    { 28, "CHF", DateTime.UtcNow, null, false }, // Swiss Franc
                    { 29, "CLP", DateTime.UtcNow, null, false }, // Chilean Peso
                    { 30, "CNY", DateTime.UtcNow, null, false }, // Chinese Yuan
                    { 31, "COP", DateTime.UtcNow, null, false }, // Colombian Peso
                    { 32, "CRC", DateTime.UtcNow, null, false }, // Costa Rican Colón
                    { 33, "CUP", DateTime.UtcNow, null, false }, // Cuban Peso
                    { 34, "CVE", DateTime.UtcNow, null, false }, // Cape Verdean Escudo
                    { 35, "CZK", DateTime.UtcNow, null, false }, // Czech Koruna
                    { 36, "DJF", DateTime.UtcNow, null, false }, // Djiboutian Franc
                    { 37, "DKK", DateTime.UtcNow, null, false }, // Danish Krone
                    { 38, "DOP", DateTime.UtcNow, null, false }, // Dominican Peso
                    { 39, "DZD", DateTime.UtcNow, null, false }, // Algerian Dinar
                    { 40, "EGP", DateTime.UtcNow, null, false }, // Egyptian Pound
                    { 41, "ERN", DateTime.UtcNow, null, false }, // Eritrean Nakfa
                    { 42, "ETB", DateTime.UtcNow, null, false }, // Ethiopian Birr
                    { 43, "EUR", DateTime.UtcNow, null, false }, // Euro
                    { 44, "FJD", DateTime.UtcNow, null, false }, // Fijian Dollar
                    { 45, "FKP", DateTime.UtcNow, null, false }, // Falkland Islands Pound
                    { 46, "GBP", DateTime.UtcNow, null, false }, // British Pound
                    { 47, "GEL", DateTime.UtcNow, null, false }, // Georgian Lari
                    { 48, "GHS", DateTime.UtcNow, null, false }, // Ghanaian Cedi
                    { 49, "GIP", DateTime.UtcNow, null, false }, // Gibraltar Pound
                    { 50, "GMD", DateTime.UtcNow, null, false }, // Gambian Dalasi
                    { 51, "GNF", DateTime.UtcNow, null, false }, // Guinean Franc
                    { 52, "GTQ", DateTime.UtcNow, null, false }, // Guatemalan Quetzal
                    { 53, "GYD", DateTime.UtcNow, null, false }, // Guyanaese Dollar
                    { 54, "HKD", DateTime.UtcNow, null, false }, // Hong Kong Dollar
                    { 55, "HNL", DateTime.UtcNow, null, false }, // Honduran Lempira
                    { 56, "HRK", DateTime.UtcNow, null, false }, // Croatian Kuna
                    { 57, "HTG", DateTime.UtcNow, null, false }, // Haitian Gourde
                    { 58, "HUF", DateTime.UtcNow, null, false }, // Hungarian Forint
                    { 59, "IDR", DateTime.UtcNow, null, false }, // Indonesian Rupiah
                    { 60, "ILS", DateTime.UtcNow, null, false }, // Israeli New Shekel
                    { 61, "INR", DateTime.UtcNow, null, false }, // Indian Rupee
                    { 62, "IQD", DateTime.UtcNow, null, false }, // Iraqi Dinar
                    { 63, "IRR", DateTime.UtcNow, null, false }, // Iranian Rial
                    { 64, "ISK", DateTime.UtcNow, null, false }, // Icelandic Króna
                    { 65, "JMD", DateTime.UtcNow, null, false }, // Jamaican Dollar
                    { 66, "JOD", DateTime.UtcNow, null, false }, // Jordanian Dinar
                    { 67, "JPY", DateTime.UtcNow, null, false }, // Japanese Yen
                    { 68, "KES", DateTime.UtcNow, null, false }, // Kenyan Shilling
                    { 69, "KGS", DateTime.UtcNow, null, false }, // Kyrgystani Som
                    { 70, "KHR", DateTime.UtcNow, null, false }, // Cambodian Riel
                    { 71, "KMF", DateTime.UtcNow, null, false }, // Comorian Franc
                    { 72, "KPW", DateTime.UtcNow, null, false }, // North Korean Won
                    { 73, "KRW", DateTime.UtcNow, null, false }, // South Korean Won
                    { 74, "KWD", DateTime.UtcNow, null, false }, // Kuwaiti Dinar
                    { 75, "KYD", DateTime.UtcNow, null, false }, // Cayman Islands Dollar
                    { 76, "KZT", DateTime.UtcNow, null, false }, // Kazakhstani Tenge
                    { 77, "LAK", DateTime.UtcNow, null, false }, // Laotian Kip
                    { 78, "LBP", DateTime.UtcNow, null, false }, // Lebanese Pound
                    { 79, "LKR", DateTime.UtcNow, null, false }, // Sri Lankan Rupee
                    { 80, "LRD", DateTime.UtcNow, null, false }, // Liberian Dollar
                    { 81, "LSL", DateTime.UtcNow, null, false }, // Lesotho Loti
                    { 82, "LYD", DateTime.UtcNow, null, false }, // Libyan Dinar
                    { 83, "MAD", DateTime.UtcNow, null, false }, // Moroccan Dirham
                    { 84, "MDL", DateTime.UtcNow, null, false }, // Moldovan Leu
                    { 85, "MGA", DateTime.UtcNow, null, false }, // Malagasy Ariary
                    { 86, "MKD", DateTime.UtcNow, null, false }, // Macedonian Denar
                    { 87, "MMK", DateTime.UtcNow, null, false }, // Myanmar Kyat
                    { 88, "MNT", DateTime.UtcNow, null, false }, // Mongolian Tugrik
                    { 89, "MOP", DateTime.UtcNow, null, false }, // Macanese Pataca
                    { 90, "MRU", DateTime.UtcNow, null, false }, // Mauritanian Ouguiya
                    { 91, "MUR", DateTime.UtcNow, null, false }, // Mauritian Rupee
                    { 92, "MVR", DateTime.UtcNow, null, false }, // Maldivian Rufiyaa
                    { 93, "MWK", DateTime.UtcNow, null, false }, // Malawian Kwacha
                    { 94, "MXN", DateTime.UtcNow, null, false }, // Mexican Peso
                    { 95, "MYR", DateTime.UtcNow, null, false }, // Malaysian Ringgit
                    { 96, "MZN", DateTime.UtcNow, null, false }, // Mozambican Metical
                    { 97, "NAD", DateTime.UtcNow, null, false }, // Namibian Dollar
                    { 98, "NGN", DateTime.UtcNow, null, false }, // Nigerian Naira
                    { 99, "NIO", DateTime.UtcNow, null, false }, // Nicaraguan Córdoba
                    { 100, "NOK", DateTime.UtcNow, null, false }, // Norwegian Krone
                    { 101, "NPR", DateTime.UtcNow, null, false }, // Nepalese Rupee
                    { 102, "NZD", DateTime.UtcNow, null, false }, // New Zealand Dollar
                    { 103, "OMR", DateTime.UtcNow, null, false }, // Omani Rial
                    { 104, "PAB", DateTime.UtcNow, null, false }, // Panamanian Balboa
                    { 105, "PEN", DateTime.UtcNow, null, false }, // Peruvian Sol
                    { 106, "PGK", DateTime.UtcNow, null, false }, // Papua New Guinean Kina
                    { 107, "PHP", DateTime.UtcNow, null, false }, // Philippine Peso
                    { 108, "PKR", DateTime.UtcNow, null, false }, // Pakistani Rupee
                    { 109, "PLN", DateTime.UtcNow, null, false }, // Polish Zloty
                    { 110, "PYG", DateTime.UtcNow, null, false }, // Paraguayan Guarani
                    { 111, "QAR", DateTime.UtcNow, null, false }, // Qatari Rial
                    { 112, "RON", DateTime.UtcNow, null, false }, // Romanian Leu
                    { 113, "RSD", DateTime.UtcNow, null, false }, // Serbian Dinar
                    { 114, "RUB", DateTime.UtcNow, null, false }, // Russian Rubles
                    { 115, "RWF", DateTime.UtcNow, null, false }, // Rwandan Franc
                    { 116, "SAR", DateTime.UtcNow, null, false }, // Saudi Riyal
                    { 117, "SBD", DateTime.UtcNow, null, false }, // Solomon Islands Dollar
                    { 118, "SCR", DateTime.UtcNow, null, false }, // Seychellois Rupee
                    { 119, "SDG", DateTime.UtcNow, null, false }, // Sudanese Pound
                    { 120, "SEK", DateTime.UtcNow, null, false }, // Swedish Krona
                    { 121, "SGD", DateTime.UtcNow, null, false }, // Singapore Dollar
                    { 122, "SHP", DateTime.UtcNow, null, false }, // Saint Helena Pound
                    { 123, "SLL", DateTime.UtcNow, null, false }, // Sierra Leonean Leone
                    { 124, "SOS", DateTime.UtcNow, null, false }, // Somali Shilling
                    { 125, "SRD", DateTime.UtcNow, null, false }, // Surinamese Dollar
                    { 126, "SSP", DateTime.UtcNow, null, false }, // South Sudanese Pound
                    { 127, "STN", DateTime.UtcNow, null, false }, // São Tomé and Príncipe Dobra
                    { 128, "SVC", DateTime.UtcNow, null, false }, // Salvadoran Colón
                    { 129, "SYP", DateTime.UtcNow, null, false }, // Syrian Pound
                    { 130, "SZL", DateTime.UtcNow, null, false }, // Swazi Lilangeni
                    { 131, "THB", DateTime.UtcNow, null, false }, // Thai Baht
                    { 132, "TJS", DateTime.UtcNow, null, false }, // Tajikistani Somoni
                    { 133, "TMT", DateTime.UtcNow, null, false }, // Turkmenistani Manat
                    { 134, "TND", DateTime.UtcNow, null, false }, // Tunisian Dinar
                    { 135, "TOP", DateTime.UtcNow, null, false }, // Tongan Paʻanga
                    { 136, "TRY", DateTime.UtcNow, null, false }, // Turkish Lira
                    { 137, "TTD", DateTime.UtcNow, null, false }, // Trinidad and Tobago Dollar
                    { 138, "TWD", DateTime.UtcNow, null, false }, // New Taiwan Dollar
                    { 139, "TZS", DateTime.UtcNow, null, false }, // Tanzanian Shilling
                    { 140, "UAH", DateTime.UtcNow, null, false }, // Ukrainian Hryvnia
                    { 141, "UGX", DateTime.UtcNow, null, false }, // Ugandan Shilling
                    { 142, "USD", DateTime.UtcNow, null, false }, // United States Dollar
                    { 143, "UYU", DateTime.UtcNow, null, false }, // Uruguayan Peso
                    { 144, "UZS", DateTime.UtcNow, null, false }, // Uzbekistani Som
                    { 145, "VES", DateTime.UtcNow, null, false }, // Venezuelan Bolívar
                    { 146, "VND", DateTime.UtcNow, null, false }, // Vietnamese Dong
                    { 147, "VUV", DateTime.UtcNow, null, false }, // Vanuatu Vatu
                    { 148, "WST", DateTime.UtcNow, null, false }, // Samoan Tala
                    { 149, "XAF", DateTime.UtcNow, null, false }, // Central African CFA Franc
                    { 150, "XCD", DateTime.UtcNow, null, false }, // Eastern Caribbean Dollar
                    { 151, "XOF", DateTime.UtcNow, null, false }, // West African CFA Franc
                    { 152, "XPF", DateTime.UtcNow, null, false }, // CFP Franc
                    { 153, "YER", DateTime.UtcNow, null, false }, // Yemeni Rial
                    { 154, "ZAR", DateTime.UtcNow, null, false }, // South African Rand
                    { 155, "ZMW", DateTime.UtcNow, null, false }, // Zambian Kwacha
                    { 156, "ZWL", DateTime.UtcNow, null, false }, // Zimbabwean Dollar
                    { 157, "XAG", DateTime.UtcNow, null, false }, // Silver (Troy Ounce)
                    { 158, "XAU", DateTime.UtcNow, null, false }, // Gold (Troy Ounce)
                    { 159, "XPD", DateTime.UtcNow, null, false }, // Palladium (Troy Ounce)
                    { 160, "XPT", DateTime.UtcNow, null, false }, // Platinum (Troy Ounce)
                    { 161, "XDR", DateTime.UtcNow, null, false }, // Special Drawing Rights (IMF)
                    { 162, "XBA", DateTime.UtcNow, null, false }, // European Composite Unit
                    { 163, "XBB", DateTime.UtcNow, null, false }, // European Monetary Unit
                    { 164, "XBC", DateTime.UtcNow, null, false }, // European Unit of Account 9
                    { 165, "XBD", DateTime.UtcNow, null, false }, // European Unit of Account 17
                    { 166, "XTS", DateTime.UtcNow, null, false }, // Test Currency Code
                    { 167, "XXX", DateTime.UtcNow, null, false }, // No Currency
                    { 168, "XUA", DateTime.UtcNow, null, false }, // ADB Unit of Account
                    { 169, "XSU", DateTime.UtcNow, null, false }, // SUCRE
                    { 170, "XBT", DateTime.UtcNow, null, false }, // Bitcoin (non-ISO, included for completeness)
                    { 171, "XMR", DateTime.UtcNow, null, false }, // Monero (non-ISO, included for completeness)
                    { 172, "ETH", DateTime.UtcNow, null, false }  // Ethereum (non-ISO, included for completeness)
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
            table: "Currencies",
            keyColumn: "Id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
            61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
            81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100,
            101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
            121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140,
            141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160,
            161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172 });
        }
    }
}
