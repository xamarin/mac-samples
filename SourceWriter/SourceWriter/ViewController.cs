using System;
using AppKit;
using Foundation;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using AppKit.TextKit.Formatter;

namespace SourceWriter
{
	/// <summary>
	/// Defines the View Controller for a syntax highlighting text editor view.
	/// </summary>
	public partial class ViewController : NSViewController
	{
		#region Application Access
		/// <summary>
		/// A helper shortcut to the app delegate.
		/// </summary>
		/// <value>The app.</value>
		public static AppDelegate App {
			get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Private Variables
		/// <summary>
		/// The information on the currently highlighted keyword.
		/// </summary>
		private KeywordDescriptor _keywordInfo = null;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the represented object.
		/// </summary>
		/// <value>The represented object.</value>
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

		/// <summary>
		/// Gets or sets the default language that this <see cref="SourceWriter.ViewController"/> will
		/// be editing.
		/// </summary>
		/// <value>An integer representing the default language as: 0 - C#,
		/// 1 - HTML, 2 - MarkDown, 3 - XML.</value>
		public int DefaultLanguage { get; set; } = App.Preferences.DefaultLanguage;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SourceWriter.ViewController"/> document 
		/// has been edited.
		/// </summary>
		/// <value><c>true</c> if document has been edited; otherwise, <c>false</c>.</value>
		public bool DocumentEdited {
			get { return View.Window.DocumentEdited; }
			set { View.Window.DocumentEdited = value; }
		}

		/// <summary>
		/// Gets the <see cref="AppKit.TextKit.Formatter.SourceTextView"/> attached to this view.
		/// </summary>
		/// <value>The <see cref="AppKit.TextKit.Formatter.SourceTextView"/> used to edit source.</value>
		public SourceTextView Editor {
			get { return TextEditor; }
		}

		/// <summary>
		/// Gets or sets the text for the <c>NSTextView</c> being used as a text editor
		/// </summary>
		/// <value>The string content of the <c>NSTextView</c>.</value>
		public string Text {
			get { return TextEditor.TextStorage.Value; }
			set {
				TextEditor.Value = value;
				Formatter.Reformat ();
				DocumentEdited = false;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/> used to perform
		/// syntax highlighting on the <c>NSTextView</c> containing the contents of the document being
		/// edited.
		/// </summary>
		/// <value>The <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/> for the selected language.</value>
		public LanguageFormatter Formatter {
			get { return TextEditor.Formatter; }
			set { TextEditor.Formatter = value; }
		}

		/// <summary>
		/// Gets or sets the full file path where this document was last loaded from
		/// or saved to.
		/// </summary>
		/// <value>The file path.</value>
		/// <remarks>>The path will be the empty string ("") if the document has never
		/// been saved to a file.</remarks>
		public string FilePath { get; set; } = "";

		/// <summary>
		/// Gets or sets the info about the currently selected keyword.
		/// </summary>
		/// <value>The keyword info.</value>
		public KeywordDescriptor KeywordInfo { 
			get { return _keywordInfo; }
			set {
				_keywordInfo = value;
				if (WindowController != null) {
					WindowController.DefinitionItem.Disabled = (_keywordInfo == null);
					App.DefinitionItem.Enabled = (!WindowController.DefinitionItem.Disabled);
				}
			}
		}

		/// <summary>
		/// Gets the window controller.
		/// </summary>
		/// <value>The window controller.</value>
		public EditorWindowController WindowController {
			get { return View.Window.WindowController as EditorWindowController; }
		}

		/// <summary>
		/// Gets or sets the keyword that is currently selected.
		/// </summary>
		/// <value>The keyword.</value>
		public string Keyword { get; set; } = "";
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceWriter.ViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// This method is called once the view controller has been inflated from the 
		/// Storyboard file. 
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Configure editor from user preferences
			ConfigureEditor ();

			// Highligh the syntax of the text after an edit has been made
			TextEditor.TextStorage.DidProcessEditing += (sender, e) => {
				DocumentEdited = true;
				Formatter.HighlightSyntaxRegion(TextEditor.TextStorage.Value, TextEditor.TextStorage.EditedRange);
			};

			// If the text selection or cursor location changes, attempt to display the Tool Tip
			// for any keyword defined in the current language being syntax highlighted
			TextEditor.SourceSelectionChanged += (sender, e) => {
				var range = Formatter.FindWordBoundries(TextEditor.TextStorage.Value, TextEditor.SelectedRange);
				var word = TextEditor.TextStorage.Value.Substring((int)range.Location, (int)range.Length);

				// Update UI
				if (WindowController !=null) {
					WindowController.Indent.Disabled = (TextEditor.SelectedRange.Length == 0);
					App.IndentItem.Enabled = (!WindowController.Indent.Disabled);
					WindowController.Outdent.Disabled = WindowController.Indent.Disabled;
					App.OutdentItem.Enabled = (!WindowController.Outdent.Disabled);
					App.ReformatItem.Enabled = (Text.Length > 0);
				}

				// Live preview content changes
				if (App.Preferences.LivePreviewChanges) {
					PreviewContents();
				}

				// Found a keyword?
				KeywordDescriptor info;
				if (Formatter.Language.Keywords.TryGetValue(word, out info)) {

					// Display the tool tip
					StatusText.StringValue = string.Format("{0}: {1}", info.Type, word);
					StatusText.TextColor = info.Color;
					StatusDesc.StringValue = info.Tooltip;
					Keyword = word;
					KeywordInfo = info;
				} else {
					// Display the currently selected text
					StatusText.StringValue = "Selection:";
					StatusText.TextColor = NSColor.Black;
					StatusDesc.StringValue = word;
					Keyword = "";
					KeywordInfo = null;
				}
			};
		}

		/// <summary>
		/// This method is called just before the View being handled by this View Controller
		/// will be displayed so you can do any preperation first.
		/// </summary>
		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			// Initialize a formatter to handle language highlighting
			switch(DefaultLanguage) {
			case 0:
				Formatter = new LanguageFormatter (TextEditor, new CSharpDescriptor ());
				StatusLanguage.StringValue = "C# Code";
				break;
			case 1:
				Formatter = new LanguageFormatter (TextEditor, new HTMLDescriptor ());
				StatusLanguage.StringValue = "HTML";
				break;
			case 2:
				Formatter = new LanguageFormatter (TextEditor, new MarkDownDescriptor ());
				StatusLanguage.StringValue = "MarkDown";
				break;
			case 3:
				Formatter = new LanguageFormatter (TextEditor, new XMLDescriptor ());
				StatusLanguage.StringValue = "XML";
				break;
			}

			// Update Menus
			PopulateFormattingMenu ();

		}

		/// <summary>
		/// This method is called after the View being handled by this View Controller has
		/// been displayed on screen.
		/// </summary>
		public override void ViewDidAppear ()
		{
			base.ViewDidAppear ();

			// Set Window Title
			if (++App.NewWindowNumber == 0) {
				this.View.Window.Title = "untitled";
			} else {
				this.View.Window.Title = string.Format("untitled {0}", App.NewWindowNumber);
			}
				
			// Configure
			Keyword = "";
			KeywordInfo = null;
	
			// Update UI
			App.ReformatItem.Enabled = (Text.Length > 0);
			WindowController.Print.Disabled = false;
		}

		/// <summary>
		/// This method is called before the view being handled by this View Controller is removed 
		/// from the screen to allow you to do any last minute clean-up.
		/// </summary>
		public override void ViewWillDisappear ()
		{
			base.ViewWillDisappear ();

		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Recursively build a menu from the set of Language Format Commands.
		/// </summary>
		/// <param name="menu">The <c>NSMenu</c> to grow.</param>
		/// <param name="commands">The list of <see cref="AppKit.TextKit.Formatter.LanguageFormatCommand"/>s.</param>
		private void AssembleMenu(NSMenu menu, List<LanguageFormatCommand> commands) {
			NSMenuItem menuItem;

			// Add any formatting commands to the Formatting menu
			foreach (LanguageFormatCommand command in commands) {
				// Add separator or item?
				if (command.Title == "") {
					menuItem = NSMenuItem.SeparatorItem;
				} else {
					menuItem = new NSMenuItem (command.Title);

					// Submenu?
					if (command.SubCommands.Count > 0) {
						// Yes, populate submenu
						menuItem.Submenu = new NSMenu (command.Title);
						AssembleMenu (menuItem.Submenu, command.SubCommands);
					} else {
						// No, add normal menu item
						menuItem.Activated += (sender, e) => {
							// Apply the command on the selected text
							TextEditor.PerformFormattingCommand (command);
						};
					}
				}
				menu.AddItem (menuItem);
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Re-run syntax highlighting for the entire text of the document.
		/// </summary>
		/// <param name="updateLanguage">If set to <c>true</c>, the language descriptor will be reloaded as well.</param>
		public void ReformatText(bool updateLanguage) {

			// Redefine language to get any preference changes?
			if (updateLanguage) {
				Formatter.Language.Define ();
				ConfigureEditor ();
			}

			// Re-highlight all text.
			Formatter.Reformat ();
		}

		/// <summary>
		/// Previews the current contents of the editor.
		/// </summary>
		public void PreviewContents() {

			// Anything to process?
			if (Formatter == null) return;

			// Is the preview window open
			if (App.PreviewWindow == null) return;

			// Display the preview of the content
			App.PreviewWindow.DisplayPreview (View.Window.Title, Formatter.Language.FormatForPreview (TextEditor.TextStorage.Value), this, FilePath);
		}

		/// <summary>
		/// Populates the formatting menu with any additional commands defined in the language.
		/// </summary>
		public void PopulateFormattingMenu() {
			
			// Anything to process?
			if (Formatter == null) return;

			// Separator required?
			if (Formatter.Language.FormattingCommands.Count > 0) {
				// Yes, add separator item
				App.FormattingMenu.AddItem (NSMenuItem.SeparatorItem);
			}

			// Build menu
			AssembleMenu(App.FormattingMenu, Formatter.Language.FormattingCommands);

		}

		/// <summary>
		/// Unpopulates the formatting menu with the previous languages extra formatting commands.
		/// </summary>
		public void UnpopulateFormattingMenu() {

			// Remove any additional items
			for (int n = (int)App.FormattingMenu.Count - 1; n > 4; --n) {
				App.FormattingMenu.RemoveItemAt (n);
			}
		}

		/// <summary>
		/// Configures the editor with the current user preferences.
		/// </summary>
		public void ConfigureEditor() {

			// General Preferences
			TextEditor.AutomaticLinkDetectionEnabled = App.Preferences.SmartLinks;
			TextEditor.AutomaticQuoteSubstitutionEnabled = App.Preferences.SmartQuotes;
			TextEditor.AutomaticDashSubstitutionEnabled = App.Preferences.SmartDashes;
			TextEditor.AutomaticDataDetectionEnabled = App.Preferences.DataDetectors;
			TextEditor.AutomaticTextReplacementEnabled = App.Preferences.TextReplacement;
			TextEditor.SmartInsertDeleteEnabled = App.Preferences.SmartInsertDelete;
			TextEditor.ContinuousSpellCheckingEnabled = App.Preferences.SpellChecking;
			TextEditor.AutomaticSpellingCorrectionEnabled = App.Preferences.AutoCorrect;
			TextEditor.GrammarCheckingEnabled = App.Preferences.GrammarChecking;

			// Editor Preferences
			TextEditor.RichText = App.Preferences.RichText;
			TextEditor.ImportsGraphics = App.Preferences.AllowGraphics;
			TextEditor.AllowsImageEditing = App.Preferences.AllowImageEditing;
			TextEditor.AllowsDocumentBackgroundColorChange = App.Preferences.AllowBackgroundColor;
			TextEditor.BackgroundColor = App.Preferences.EditorBackgroundColor;
			TextEditor.UsesFontPanel = App.Preferences.UseFontPanel;
			TextEditor.UsesRuler = App.Preferences.UseRuler;
			TextEditor.UsesInspectorBar = App.Preferences.UseInspectorBar;
			TextEditor.CompleteClosures = App.Preferences.CompleteClosures;
			TextEditor.WrapClosures = App.Preferences.WrapClosures;
			TextEditor.SelectAfterWrap = App.Preferences.SelectAfterWrap;

			// Search Preferences
			switch (App.Preferences.SearchType) {
			case 0:
				// None
				TextEditor.UsesFindBar = false;
				TextEditor.UsesFindPanel = false;
				break;
			case 1:
				// Uses bar
				TextEditor.UsesFindBar = true;
				break;
			case 2:
				// Uses panel
				TextEditor.UsesFindPanel = true;
				break;
			}
			TextEditor.IsIncrementalSearchingEnabled = App.Preferences.IncrementalSearch;

			// Auto Complete Preferences
			TextEditor.AllowAutoComplete = App.Preferences.AllowAutoComplete;
			TextEditor.AutoCompleteKeywords = App.Preferences.AutoCompleteKeywords;
			TextEditor.AutoCompleteKeywords = App.Preferences.AutoCompleteDefaultWords;
			TextEditor.DefaultWordsOnlyIfKeywordsEmpty = App.Preferences.DefaultWordsOnlyIfKeywordsEmpty;

		}

		/// <summary>
		/// Sets the language to C sharp.
		/// </summary>
		public void SetLanguageToCSharp() {
			UnpopulateFormattingMenu ();
			Formatter.Language = new CSharpDescriptor ();
			StatusLanguage.StringValue = "C# Code";
			PopulateFormattingMenu ();
		}

		/// <summary>
		/// Sets the language to HTML.
		/// </summary>
		public void SetLanguageToHTML() {
			UnpopulateFormattingMenu ();
			Formatter.Language = new HTMLDescriptor ();
			StatusLanguage.StringValue = "HTML";
			PopulateFormattingMenu ();
		}

		/// <summary>
		/// Sets the language to MarkDown.
		/// </summary>
		public void SetLanguageToMarkDown() {
			UnpopulateFormattingMenu ();
			Formatter.Language = new MarkDownDescriptor ();
			StatusLanguage.StringValue = "MarkDown";
			PopulateFormattingMenu ();
		}

		/// <summary>
		/// Sets the language to XML.
		/// </summary>
		public void SetLanguageToXML() {
			UnpopulateFormattingMenu ();
			Formatter.Language = new XMLDescriptor ();
			StatusLanguage.StringValue = "XML";
			PopulateFormattingMenu ();
		}

		/// <summary>
		/// Attempts to set the syntax highlighting language based on
		/// the extension of the file being opened.
		/// </summary>
		/// <param name="path">Path.</param>
		public void SetLanguageFromPath(string path) {

			// Save path
			FilePath = path;

			// Attempt to set the language based on the file
			// extension
			if (path.EndsWith (".cs")) {
				SetLanguageToCSharp ();
			} else if (path.EndsWith (".htm")) {
				SetLanguageToHTML ();
			} else if (path.EndsWith (".md")) {
				SetLanguageToMarkDown ();
			} else if (path.EndsWith (".xml")) {
				SetLanguageToXML ();
			}

		}

		/// <summary>
		/// Prints the document that is currently being edited.
		/// </summary>
		/// <param name="info">A <c>NSPrintInfo</c> object defining the page layout to use
		/// while printing.</param>
		public void PrintDocument(NSPrintInfo info) {

			// Configure print job
			TextEditor.Print (this);
		}
		#endregion

	}
}
