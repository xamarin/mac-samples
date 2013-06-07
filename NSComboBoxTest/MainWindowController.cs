using System;
using System.Collections.Generic;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NSComboBoxTest
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			comboBox.UsesDataSource = true;
			comboBox.Completes = true;
			comboBox.DataSource = new CountryDataSource ();
		}

		class CountryDataSource : NSComboBoxDataSource
		{
			public override string CompletedString (NSComboBox comboBox, string uncompletedString)
			{
				return countries.Find (n => n.StartsWith (uncompletedString, StringComparison.InvariantCultureIgnoreCase));
			}

			public override int IndexOfItem (NSComboBox comboBox, string value)
			{
				return countries.FindIndex (n => n.Equals (value, StringComparison.InvariantCultureIgnoreCase));
			}

			public override int ItemCount (NSComboBox comboBox)
			{
				return countries.Count;
			}

			public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
			{
				return NSObject.FromObject (countries [index]);
			}

			List<string> countries = new List<string> {
				"Afghanistan",
				"Albania",
				"Algeria",
				"Andorra",
				"Angola",
				"Antigua and Barbuda",
				"Argentina",
				"Armenia",
				"Australia",
				"Austria",
				"Azerbaijan",
				"Bahamas, The",
				"Bahrain",
				"Bangladesh",
				"Barbados",
				"Belarus",
				"Belgium",
				"Belize",
				"Benin",
				"Bhutan",
				"Bolivia",
				"Bosnia and Herzegovina",
				"Botswana",
				"Brazil",
				"Brunei",
				"Bulgaria",
				"Burkina Faso",
				"Burma",
				"Burundi",
				"Cambodia",
				"Cameroon",
				"Canada",
				"Cape Verde",
				"Central African Republic",
				"Chad",
				"Chile",
				"China",
				"China, Republic of",
				"Colombia",
				"Comoros",
				"Congo, Democratic Republic of the",
				"Congo, Republic of the",
				"Cook Islands",
				"Costa Rica",
				"Côte d'Ivoire",
				"Croatia",
				"Cuba",
				"Cyprus",
				"Czech Republic",
				"Democratic People's Republic of Korea",
				"Democratic Republic of the Congo",
				"Denmark",
				"Djibouti",
				"Dominica",
				"Dominican Republic",
				"East Timor",
				"Ecuador",
				"Egypt",
				"El Salvador",
				"Equatorial Guinea",
				"Eritrea",
				"Estonia",
				"Ethiopia",
				"Fiji",
				"Finland",
				"France",
				"Gabon",
				"Gambia, The",
				"Georgia",
				"Germany",
				"Ghana",
				"Greece",
				"Grenada",
				"Guatemala",
				"Guinea",
				"Guinea-Bissau",
				"Guyana",
				"Haiti",
				"Holy See",
				"Honduras",
				"Hungary",
				"Iceland",
				"India",
				"Indonesia",
				"Iran",
				"Iraq",
				"Ireland",
				"Israel",
				"Italy",
				"Ivory Coast",
				"Jamaica",
				"Japan",
				"Jordan",
				"Kazakhstan",
				"Kenya",
				"Kiribati",
				"Korea, North",
				"Korea, South",
				"Kosovo",
				"Kuwait",
				"Kyrgyzstan",
				"Laos",
				"Latvia",
				"Lebanon",
				"Lesotho",
				"Liberia",
				"Libya",
				"Liechtenstein",
				"Lithuania",
				"Luxembourg",
				"Macedonia",
				"Madagascar",
				"Malawi",
				"Malaysia",
				"Maldives",
				"Mali",
				"Malta",
				"Marshall Islands",
				"Mauritania",
				"Mauritius",
				"Mexico",
				"Micronesia, Federated States of",
				"Moldova",
				"Monaco",
				"Mongolia",
				"Montenegro",
				"Morocco",
				"Mozambique",
				"Myanmar",
				"Nagorno-Karabakh",
				"Namibia",
				"Nauru",
				"Nepal",
				"Netherlands",
				"New Zealand",
				"Nicaragua",
				"Niger",
				"Nigeria",
				"Niue",
				"Northern Cyprus",
				"North Korea",
				"Norway",
				"Oman",
				"Pakistan",
				"Palau",
				"Palestine",
				"Panama",
				"Papua New Guinea",
				"Paraguay",
				"Peru",
				"Philippines",
				"Poland",
				"Portugal",
				"Pridnestrovie",
				"Qatar",
				"Republic of Korea",
				"Republic of the Congo",
				"Romania",
				"Russia",
				"Rwanda",
				"Sahrawi Arab Democratic Republic",
				"Saint Kitts and Nevis",
				"Saint Lucia",
				"Saint Vincent and the Grenadines",
				"Samoa",
				"San Marino",
				"São Tomé and Príncipe",
				"Saudi Arabia",
				"Senegal",
				"Serbia",
				"Seychelles",
				"Sierra Leone",
				"Singapore",
				"Slovakia",
				"Slovenia",
				"Solomon Islands",
				"Somalia",
				"Somaliland",
				"South Africa",
				"South Korea",
				"South Ossetia",
				"South Sudan",
				"Spain",
				"Sri Lanka",
				"Sudan",
				"Sudan, South",
				"Suriname",
				"Swaziland",
				"Sweden",
				"Switzerland",
				"Syria",
				"Taiwan (Republic of China)",
				"Tajikistan",
				"Tanzania",
				"Thailand",
				"Timor-Leste",
				"Togo",
				"Tonga",
				"Transnistria",
				"Trinidad and Tobago",
				"Tunisia",
				"Turkey",
				"Turkmenistan",
				"Tuvalu",
				"Uganda",
				"Ukraine",
				"United Arab Emirates",
				"United Kingdom",
				"United States",
				"Uruguay",
				"Uzbekistan",
				"Vanuatu",
				"Vatican City",
				"Venezuela",
				"Vietnam",
				"Yemen",
				"Zambia",
				"Zimbabwe"
			};
		}
	}
}