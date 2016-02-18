using System;
using System.Collections.Generic;
using Foundation;
using AppKit;

namespace AppKit.TextKit.Formatter
{
	/// <summary>
	/// Defines how HTML should be colorized using a <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/>.
	/// </summary>
	/// <remarks>
	/// This is a simplified, sample format provided as an example only. The <c>HighlightSyntaxRegion</c> method
	/// of the <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/> would need to be modified to properly format
	/// HTML Keywords as the current version was designed for languages like C#, JavaScript or Visual Basic.
	/// </remarks>
	public class HTMLDescriptor : LanguageDescriptor
	{
		#region Computed Properties
		/// <summary>
		/// Gets the language identifier.
		/// </summary>
		/// <value>The language identifier.</value>
		public override string LanguageIdentifier {
			get { return "HTML"; }
		}

		/// <summary>
		/// Gets or sets the language separators for HTML.
		/// </summary>
		/// <value>The language separators.</value>
		public override char[] LanguageSeparators { get; set; } = new char[]{'=','+','-','*','/','%','&','<','>',';',':','^','!','~','?','|',',','"','\'','(',')','[',']','{','}'};
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AppKit.TextKit.Formatter.HTMLDescriptor"/> class.
		/// </summary>
		public HTMLDescriptor ()
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Define this instance.
		/// </summary>
		public override void Define ()
		{
			base.Define ();

			// Keywords
			// Keywords.Add("", new KeywordDescriptor(KeywordType.Keyword, KeywordColor, ""));

			// Parameters
			// Keywords.Add("", new KeywordDescriptor(KeywordType.MethodParameters, MethodParameterColor, ""));

			// Define formats
			Formats.Add (new FormatDescriptor ("<!--", "-->", CommentColor));
			Formats.Add(new FormatDescriptor ("\"","\"", StringLiteralColor));
			Formats.Add (new FormatDescriptor ("<code>", "</code>", StatementColor));
			Formats.Add (new FormatDescriptor ("&", ";", PreprocessorDirectiveColor));
			Formats.Add (new FormatDescriptor ("<", ">", KeywordColor));

			// Define formatting commands
			FormattingCommands.Add(new LanguageFormatCommand("Stong","<b>","</b>"));
			FormattingCommands.Add(new LanguageFormatCommand("Emphasize","<i>","</i>"));
			FormattingCommands.Add(new LanguageFormatCommand("Inline Code","<code>","</code>"));
			FormattingCommands.Add(new LanguageFormatCommand("Code Block","<code>\n","\n<code>\n"));
			FormattingCommands.Add(new LanguageFormatCommand("Comment","<!--","-->"));
			FormattingCommands.Add (new LanguageFormatCommand ());
			FormattingCommands.Add(new LanguageFormatCommand("Unordered List","<ul>\n\t<li>","</li>\n<</ul>\n"));
			FormattingCommands.Add(new LanguageFormatCommand("Ordered List","<ol>\n\t<li>","</li>\n<</ol>\n"));
			FormattingCommands.Add (new LanguageFormatCommand ());

			var Headings = new LanguageFormatCommand ("Headings");
			Headings.SubCommands.Add(new LanguageFormatCommand("Heading 1","<h1>","</h1>"));
			Headings.SubCommands.Add(new LanguageFormatCommand("Heading 2","<h2>","</h2>"));
			Headings.SubCommands.Add(new LanguageFormatCommand("Heading 3","<h3>","</h3>"));
			Headings.SubCommands.Add(new LanguageFormatCommand("Heading 4","<h4>","</h4>"));
			Headings.SubCommands.Add(new LanguageFormatCommand("Heading 5","<h5>","</h5>"));
			Headings.SubCommands.Add(new LanguageFormatCommand("Heading 6","<h6>","</h6>"));
			FormattingCommands.Add (Headings);

			FormattingCommands.Add (new LanguageFormatCommand ());
			FormattingCommands.Add(new LanguageFormatCommand("Body","<body>","</body>"));
			FormattingCommands.Add(new LanguageFormatCommand("Paragraph","<p>","</p>"));
		}
		#endregion
	}
}

