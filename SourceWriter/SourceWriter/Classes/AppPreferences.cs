using System;
using Foundation;
using AppKit;

namespace SourceWriter
{
	/// <summary>
	/// This class handles all of the user's preferences for the app. The values are 
	/// persisted using the system-wide <c>NSUserDefaults.StandardUserDefaults</c> object.
	/// </summary>
	/// <remarks>This section uses Data Binding and Key-Value Coding to bind
	/// preference values to UI Controls on the Storyboard. For more information
	/// see: http://developer.xamarin.com/guides/mac/application_fundamentals/databinding/</remarks>
	[Register("AppPreferences")]
	public class AppPreferences : NSObject
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the default language that will be automatically selected when creating a new
		/// document.
		/// </summary>
		/// <value>The default <see cref="AppKit.TextKit.Formatter.LanguageDescriptor"/>.</value>
		[Export("DefaultLanguage")]
		public int DefaultLanguage {
			get { 
				var value = LoadInt ("DefaultLanguage", 0);
				//Console.WriteLine ("Get Language: {0}", value);
				return value; 
			}
			set {
				WillChangeValue ("DefaultLanguage");
				SaveInt ("DefaultLanguage", value, true);
				DidChangeValue ("DefaultLanguage");
				//Console.WriteLine ("Set Language: {0}", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Preview Window live updates as the user types.
		/// </summary>
		/// <value><c>true</c> if live preview changes; otherwise, <c>false</c>.</value>
		[Export("LivePreviewChanges")]
		public bool LivePreviewChanges {
			get { return LoadBool ("LivePreviewChanges", true); }
			set {
				WillChangeValue ("LivePreviewChanges");
				SaveBool ("LivePreviewChanges", value, true);
				DidChangeValue ("LivePreviewChanges");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor window allows for auto
		/// complete.
		/// </summary>
		/// <value><c>true</c> if allow auto complete; otherwise, <c>false</c>.</value>
		[Export("AllowAutoComplete")]
		public bool AllowAutoComplete {
			get { return LoadBool ("AllowAutoComplete", true); }
			set {
				WillChangeValue ("AllowAutoComplete");
				SaveBool ("AllowAutoComplete", value, true);
				DidChangeValue ("AllowAutoComplete");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document window will auto complete <c>Keywords</c>
		/// as defined in the document's <see cref="AppKit.TextKit.Formatter.LanguageDescriptor"/>.
		/// </summary>
		/// <value><c>true</c> if auto complete keywords; otherwise, <c>false</c>.</value>
		[Export("AutoCompleteKeywords")]
		public bool AutoCompleteKeywords {
			get { return LoadBool ("AutoCompleteKeywords", true); }
			set {
				WillChangeValue ("AutoCompleteKeywords");
				SaveBool ("AutoCompleteKeywords", value, true);
				DidChangeValue ("AutoCompleteKeywords");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will add the default, OS provided
		/// auto complete words.
		/// </summary>
		/// <value><c>true</c> if auto complete default words; otherwise, <c>false</c>.</value>
		[Export("AutoCompleteDefaultWords")]
		public bool AutoCompleteDefaultWords {
			get { return LoadBool ("AutoCompleteDefaultWords", true); }
			set {
				WillChangeValue ("AutoCompleteDefaultWords");
				SaveBool ("AutoCompleteDefaultWords", value, true);
				DidChangeValue ("AutoCompleteDefaultWords");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will only use the default, OS provided
		/// auto complete keywords in the editor's <see cref="AppKit.TextKit.Formatter.LanguageDescriptor"/>
		/// return no <c>Keywords</c>.
		/// </summary>
		/// <value><c>true</c> if default words only if keywords empty; otherwise, <c>false</c>.</value>
		[Export("DefaultWordsOnlyIfKeywordsEmpty")]
		public bool DefaultWordsOnlyIfKeywordsEmpty {
			get { return LoadBool ("DefaultWordsOnlyIfKeywordsEmpty", true); }
			set {
				WillChangeValue ("DefaultWordsOnlyIfKeywordsEmpty");
				SaveBool ("DefaultWordsOnlyIfKeywordsEmpty", value, true);
				DidChangeValue ("DefaultWordsOnlyIfKeywordsEmpty");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will automatically complete
		/// <see cref="AppKit.TextKit.Formatter.LanguageClosure"/> as defined in the 
		/// <see cref="AppKit.TextKit.Formatter.LanguageDescriptor"/>.
		/// </summary>
		/// <value><c>true</c> if complete closures; otherwise, <c>false</c>.</value>
		[Export("CompleteClosures")]
		public bool CompleteClosures {
			get { return LoadBool ("CompleteClosures", true); }
			set {
				WillChangeValue ("CompleteClosures");
				SaveBool ("CompleteClosures", value, true);
				DidChangeValue ("CompleteClosures");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the editor will wrap <see cref="AppKit.TextKit.Formatter.LanguageClosure"/>
		/// around the selected text in the document editor.
		/// </summary>
		/// <value><c>true</c> if wrap closures; otherwise, <c>false</c>.</value>
		[Export("WrapClosures")]
		public bool WrapClosures {
			get { return LoadBool ("WrapClosures", true); }
			set {
				WillChangeValue ("WrapClosures");
				SaveBool ("WrapClosures", value, true);
				DidChangeValue ("WrapClosures");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will select all of the text that was just wrapped
		/// in a <see cref="AppKit.TextKit.Formatter.LanguageClosure"/>.
		/// </summary>
		/// <value><c>true</c> if select after wrap; otherwise, <c>false</c>.</value>
		[Export("SelectAfterWrap")]
		public bool SelectAfterWrap {
			get { return LoadBool ("SelectAfterWrap", true); }
			set {
				WillChangeValue ("SelectAfterWrap");
				SaveBool ("SelectAfterWrap", value, true);
				DidChangeValue ("SelectAfterWrap");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will automatically detect links in the text.
		/// </summary>
		/// <value><c>true</c> if smart links; otherwise, <c>false</c>.</value>
		[Export("SmartLinks")]
		public bool SmartLinks {
			get { return LoadBool ("SmartLinks", true); }
			set {
				WillChangeValue ("SmartLinks");
				SaveBool ("SmartLinks", value, true);
				DidChangeValue ("SmartLinks");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will use typographic quotation marks when
		/// editing text.
		/// </summary>
		/// <value><c>true</c> if smart quotes; otherwise, <c>false</c>.</value>
		[Export("SmartQuotes")]
		public bool SmartQuotes {
			get { return LoadBool ("SmartQuotes", true); }
			set {
				WillChangeValue ("SmartQuotes");
				SaveBool ("SmartQuotes", value, true);
				DidChangeValue ("SmartQuotes");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether document editor will automatically insert dashes while editing
		/// text.
		/// </summary>
		/// <value><c>true</c> if smart dashes; otherwise, <c>false</c>.</value>
		[Export("SmartDashes")]
		public bool SmartDashes {
			get { return LoadBool ("SmartDashes", true); }
			set {
				WillChangeValue ("SmartDashes");
				SaveBool ("SmartDashes", value, true);
				DidChangeValue ("SmartDashes");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will automatically detect data entered
		/// by the user shuch as phone numbers and dates.
		/// </summary>
		/// <value><c>true</c> if data detectors; otherwise, <c>false</c>.</value>
		[Export("DataDetectors")]
		public bool DataDetectors {
			get { return LoadBool ("DataDetectors", true); }
			set {
				WillChangeValue ("DataDetectors");
				SaveBool ("DataDetectors", value, true);
				DidChangeValue ("DataDetectors");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will automatically perform know text
		/// replacement while editing text.
		/// </summary>
		/// <value><c>true</c> if text replacement; otherwise, <c>false</c>.</value>
		[Export("TextReplacement")]
		public bool TextReplacement {
			get { return LoadBool ("TextReplacement", true); }
			set {
				WillChangeValue ("TextReplacement");
				SaveBool ("TextReplacement", value, true);
				DidChangeValue ("TextReplacement");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will use smart insert and delete logic
		/// while editing text.
		/// </summary>
		/// <value><c>true</c> if smart insert delete; otherwise, <c>false</c>.</value>
		[Export("SmartInsertDelete")]
		public bool SmartInsertDelete {
			get { return LoadBool ("SmartInsertDelete", true); }
			set {
				WillChangeValue ("SmartInsertDelete");
				SaveBool ("SmartInsertDelete", value, true);
				DidChangeValue ("SmartInsertDelete");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will check spelling while editing
		/// text.
		/// </summary>
		/// <value><c>true</c> if spell checking; otherwise, <c>false</c>.</value>
		[Export("SpellChecking")]
		public bool SpellChecking {
			get { return LoadBool ("SpellChecking", true); }
			set {
				WillChangeValue ("SpellChecking");
				SaveBool ("SpellChecking", value, true);
				DidChangeValue ("SpellChecking");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will automatically correct spelling
		/// while editing text.
		/// </summary>
		/// <value><c>true</c> if auto correct; otherwise, <c>false</c>.</value>
		[Export("AutoCorrect")]
		public bool AutoCorrect {
			get { return LoadBool ("AutoCorrect", true); }
			set {
				WillChangeValue ("AutoCorrect");
				SaveBool ("AutoCorrect", value, true);
				DidChangeValue ("AutoCorrect");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will check grammar while editing
		/// text.
		/// </summary>
		/// <value><c>true</c> if grammar checking; otherwise, <c>false</c>.</value>
		[Export("GrammarChecking")]
		public bool GrammarChecking {
			get { return LoadBool ("GrammarChecking", true); }
			set {
				WillChangeValue ("GrammarChecking");
				SaveBool ("GrammarChecking", value, true);
				DidChangeValue ("GrammarChecking");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor will allow for rich text documents.
		/// </summary>
		/// <value><c>true</c> if rich text; otherwise, <c>false</c>.</value>
		[Export("RichText")]
		public bool RichText {
			get { return LoadBool ("RichText", true); }
			set {
				WillChangeValue ("RichText");
				SaveBool ("RichText", value, true);
				DidChangeValue ("RichText");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor allows the insertion of pictures into the
		/// document text.
		/// </summary>
		/// <value><c>true</c> if allow graphics; otherwise, <c>false</c>.</value>
		[Export("AllowGraphics")]
		public bool AllowGraphics {
			get { return LoadBool ("AllowGraphics", true); }
			set {
				WillChangeValue ("AllowGraphics");
				SaveBool ("AllowGraphics", value, true);
				DidChangeValue ("AllowGraphics");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor allows the user to edit images inserted
		/// into the document's text.
		/// </summary>
		/// <value><c>true</c> if allow image editing; otherwise, <c>false</c>.</value>
		[Export("AllowImageEditing")]
		public bool AllowImageEditing {
			get { return LoadBool ("AllowImageEditing", true); }
			set {
				WillChangeValue ("AllowImageEditing");
				SaveBool ("AllowImageEditing", value, true);
				DidChangeValue ("AllowImageEditing");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor allows for non-continous layout of
		/// the text it is editing.
		/// </summary>
		/// <value><c>true</c> if non continous layout; otherwise, <c>false</c>.</value>
		[Export("NonContinousLayout")]
		public bool NonContinousLayout {
			get { return LoadBool ("NonContinousLayout", true); }
			set {
				WillChangeValue ("NonContinousLayout");
				SaveBool ("NonContinousLayout", value, true);
				DidChangeValue ("NonContinousLayout");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor allows for the background color of the
		/// text to be changed.
		/// </summary>
		/// <value><c>true</c> if allow background color; otherwise, <c>false</c>.</value>
		[Export("AllowBackgroundColor")]
		public bool AllowBackgroundColor {
			get { return LoadBool ("AllowBackgroundColor", true); }
			set {
				WillChangeValue ("AllowBackgroundColor");
				SaveBool ("AllowBackgroundColor", value, true);
				DidChangeValue ("AllowBackgroundColor");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor only allows for Roman characters in the
		/// text.
		/// </summary>
		/// <value><c>true</c> if only roman chars; otherwise, <c>false</c>.</value>
		[Export("OnlyRomanChars")]
		public bool OnlyRomanChars {
			get { return LoadBool ("OnlyRomanChars", true); }
			set {
				WillChangeValue ("OnlyRomanChars");
				SaveBool ("OnlyRomanChars", value, true);
				DidChangeValue ("OnlyRomanChars");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor uses font panel.
		/// </summary>
		/// <value><c>true</c> if use font panel; otherwise, <c>false</c>.</value>
		[Export("UseFontPanel")]
		public bool UseFontPanel {
			get { return LoadBool ("UseFontPanel", true); }
			set {
				WillChangeValue ("UseFontPanel");
				SaveBool ("UseFontPanel", value, true);
				DidChangeValue ("UseFontPanel");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor uses ruler.
		/// </summary>
		/// <value><c>true</c> if use ruler; otherwise, <c>false</c>.</value>
		[Export("UseRuler")]
		public bool UseRuler {
			get { return LoadBool ("UseRuler", true); }
			set {
				WillChangeValue ("UseRuler");
				SaveBool ("UseRuler", value, true);
				DidChangeValue ("UseRuler");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor uses inspector bar.
		/// </summary>
		/// <value><c>true</c> if use inspector bar; otherwise, <c>false</c>.</value>
		[Export("UseInspectorBar")]
		public bool UseInspectorBar {
			get { return LoadBool ("UseInspectorBar", true); }
			set {
				WillChangeValue ("UseInspectorBar");
				SaveBool ("UseInspectorBar", value, true);
				DidChangeValue ("UseInspectorBar");
			}
		}

		/// <summary>
		/// Gets or sets the type of the search the document editor uses
		/// </summary>
		/// <value>The type of the search.</value>
		[Export("SearchType")]
		public int SearchType {
			get { 
				return LoadInt ("SearchType", 1); 
			}
			set {
				WillChangeValue ("SearchType");
				SaveInt ("SearchType", value, true);
				DidChangeValue ("SearchType");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the document editor allows for incremental search.
		/// </summary>
		/// <value><c>true</c> if incremental search; otherwise, <c>false</c>.</value>
		[Export("IncrementalSearch")]
		public bool IncrementalSearch {
			get { return LoadBool ("IncrementalSearch", true); }
			set {
				WillChangeValue ("IncrementalSearch");
				SaveBool ("IncrementalSearch", value, true);
				DidChangeValue ("IncrementalSearch");
			}
		}

		/// <summary>
		/// Gets or sets the color of the editor background of the document editor.
		/// </summary>
		/// <value>The <c>NSColor</c> of the editor background.</value>
		[Export("EditorBackgroundColor")]
		public NSColor EditorBackgroundColor {
			get { return LoadColor("EditorBackgroundColor", NSColor.White); }
			set {
				WillChangeValue ("EditorBackgroundColor");
				SaveColor ("EditorBackgroundColor", value, true);
				DidChangeValue ("EditorBackgroundColor");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceWriter.AppPreferences"/> class.
		/// </summary>
		public AppPreferences ()
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Loads the given integer value for the specified key. If the key is not found,
		/// the default value is returned.
		/// </summary>
		/// <returns>The int.</returns>
		/// <param name="key">Key.</param>
		/// <param name="defaultValue">Default value.</param>
		public int LoadInt(string key, int defaultValue) {
			// Attempt to read int
			var number = NSUserDefaults.StandardUserDefaults.IntForKey(key);

			// Take action based on value
			if (number == null) {
				return defaultValue;
			} else {
				return (int)number;
			}
		}
			
		/// <summary>
		/// Saves the given integer value to the system-wide user defaults.
		/// </summary>
		/// <param name="key">The key for the integer to load.</param>
		/// <param name="value">The value of the key.</param>
		/// <param name="sync">If set to <c>true</c> sync changes to preferences.</param>
		public void SaveInt(string key, int value, bool sync) {
			NSUserDefaults.StandardUserDefaults.SetInt(value, key);
			if (sync) NSUserDefaults.StandardUserDefaults.Synchronize ();
		}

		/// <summary>
		/// Loads the bool value from the system-wide user defaults.
		/// </summary>
		/// <returns>The value of the key or the default value if not found.</returns>
		/// <param name="key">The key to load the value for.</param>
		/// <param name="defaultValue">The default value if not found.</param>
		public bool LoadBool(string key, bool defaultValue) {
			// Attempt to read int
			var value = NSUserDefaults.StandardUserDefaults.BoolForKey(key);

			// Take action based on value
			if (value == null) {
				return defaultValue;
			} else {
				return value;
			}
		}

		/// <summary>
		/// Saves the bool value to the system-wide user defaults.
		/// </summary>
		/// <param name="key">The key to save the value to.</param>
		/// <param name="value">The value to save.</param>
		/// <param name="sync">If set to <c>true</c> sync.</param>
		public void SaveBool(string key, bool value, bool sync) {
			NSUserDefaults.StandardUserDefaults.SetBool(value, key);
			if (sync) NSUserDefaults.StandardUserDefaults.Synchronize ();
		}

		/// <summary>
		/// Converts the given color into a web style hex string in the form #RRBBGG or optionally #RRBBGGAA.
		/// </summary>
		/// <returns>The web hex string representing the given color.</returns>
		/// <param name="color">The <c>NSColor</c> to convert.</param>
		/// <param name="withAlpha">If set to <c>true</c> with the alpha (transparency) of the color will be
		/// included.</param>
		public string NSColorToHexString(NSColor color, bool withAlpha) {
			//Break color into pieces
			nfloat red=0, green=0, blue=0, alpha=0;
			color.GetRgba (out red, out green, out blue, out alpha);

			// Adjust to byte
			alpha *= 255;
			red *= 255;
			green *= 255;
			blue *= 255;

			//With the alpha value?
			if (withAlpha) {
				return String.Format ("#{0:X2}{1:X2}{2:X2}{3:X2}", (int)alpha, (int)red, (int)green, (int)blue);
			} else {
				return String.Format ("#{0:X2}{1:X2}{2:X2}", (int)red, (int)green, (int)blue);
			}
		}

		/// <summary>
		/// Converts a web formatted hex string in the form #RRGGBB or #RRGGBBAA into a
		/// color.
		/// </summary>
		/// <returns>The <c>NSColor</c> represented by the hex string.</returns>
		/// <param name="hexValue">The web formatted hex string in the form #RRGGBB or #RRGGBBAA.</param>
		public NSColor NSColorFromHexString (string hexValue)
		{
			var colorString = hexValue.Replace ("#", "");
			float red, green, blue, alpha;

			// Convert color based on length
			switch (colorString.Length) {
			case 3 : // #RGB
				red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
				green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
				blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
				return NSColor.FromRgba(red, green, blue, 1.0f);
			case 6 : // #RRGGBB
				red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
				green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
				blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
				return NSColor.FromRgba(red, green, blue, 1.0f);
			case 8 : // #AARRGGBB
				alpha = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
				red = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
				green = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
				blue = Convert.ToInt32(colorString.Substring(6, 2), 16) / 255f;
				return NSColor.FromRgba(red, green, blue, alpha);
			default :
				throw new ArgumentOutOfRangeException(string.Format("Invalid color value '{0}'. It should be a hex value of the form #RBG, #RRGGBB or #AARRGGBB", hexValue));
			}
		}

		/// <summary>
		/// Loads the requested color from system-wide user defaults.
		/// </summary>
		/// <returns>The <c>NSColor</c> for the given key or the default value if the key
		/// cannot be found in the user defaults.</returns>
		/// <param name="key">The user default key for the color.</param>
		/// <param name="defaultValue">The default <c>NSColor</c> value.</param>
		public NSColor LoadColor(string key, NSColor defaultValue) {

			// Attempt to read color
			var hex = NSUserDefaults.StandardUserDefaults.StringForKey(key);

			// Take action based on value
			if (hex == null) {
				return defaultValue;
			} else {
				return NSColorFromHexString (hex);
			}
		}

		/// <summary>
		/// Saves the given color to the systwm-wide user defaults with the give keyword.
		/// </summary>
		/// <param name="color">The <c>NSColor</c> to save to the user defaults.</param>
		/// <param name="key">The user default key to assign the color to.</param>
		/// <param name="sync">If set to <c>true</c> sync changes to preferences.</param>
		public void SaveColor(string key, NSColor color, bool sync) {
			// Save to default
			NSUserDefaults.StandardUserDefaults.SetString(NSColorToHexString(color,true), key);
			if (sync) NSUserDefaults.StandardUserDefaults.Synchronize ();
		}
		#endregion
	}
}

