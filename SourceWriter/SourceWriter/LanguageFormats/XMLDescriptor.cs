using System;
using System.Collections.Generic;
using Foundation;
using AppKit;

namespace AppKit.TextKit.Formatter
{
	/// <summary>
	/// Defines how XML should be colorized using a <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/>.
	/// </summary>
	/// <remarks>
	/// This is a simplified, sample format provided as an example only. The <c>HighlightSyntaxRegion</c> method
	/// of the <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/> would need to be modified to properly format
	/// XML Keywords as the current version was designed for languages like C#, JavaScript or Visual Basic.
	/// </remarks>
	public class XMLDescriptor: LanguageDescriptor
	{
		#region Computed Properties
		/// <summary>
		/// Gets the language identifier.
		/// </summary>
		/// <value>The language identifier.</value>
		public override string LanguageIdentifier {
			get { return "XML"; }
		}

		/// <summary>
		/// Gets or sets the language separators for XML.
		/// </summary>
		/// <value>The language separators.</value>
		public override char[] LanguageSeparators { get; set; } = new char[]{'=','+','-','*','/','%','&','<','>',';',':','^','!','~','?','|',',','"','\'','(',')','[',']','{','}'};
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AppKit.TextKit.Formatter.XMLDescriptor"/> class.
		/// </summary>
		public XMLDescriptor ()
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
			Keywords.Add("xml", new KeywordDescriptor(KeywordType.Keyword, KeywordColor, "The XML prolog is optional. If it exists, it must come first in the document."));
			Keywords.Add("xsl", new KeywordDescriptor(KeywordType.Keyword, KeywordColor, "XSL stands for EXtensible Stylesheet Language, and is a style sheet language for XML documents."));
			Keywords.Add("xs", new KeywordDescriptor(KeywordType.Keyword, KeywordColor, "Defines an XML schema."));

			// Parameters
			// Keywords.Add("", new KeywordDescriptor(KeywordType.MethodParameters, MethodParameterColor, ""));
			Keywords.Add("version", new KeywordDescriptor(KeywordType.MethodParameters, MethodParameterColor, "Specifies the XML version."));
			Keywords.Add("encoding", new KeywordDescriptor(KeywordType.MethodParameters, MethodParameterColor, "Specifies the files encoding type."));
			Keywords.Add("xmlns", new KeywordDescriptor(KeywordType.MethodParameters, MethodParameterColor, "The namespace can be defined by an xmlns attribute in the start tag of an element."));

			// Define HTML formats
			Formats.Add (new FormatDescriptor ("<!--", "-->", CommentColor));
			Formats.Add(new FormatDescriptor ("\"","\"", StringLiteralColor));
			Formats.Add (new FormatDescriptor ("&", ";", PreprocessorDirectiveColor));
			Formats.Add (new FormatDescriptor ("<", ">", StatementColor));

		}

		/// <summary>
		/// Formats the passed in string of text for previewing.
		/// </summary>
		/// <returns>The string formatted for preview.</returns>
		/// <param name="text">Text.</param>
		public override string FormatForPreview (string text)
		{
			return "<pre><code>" + text + "</code></pre>";
		}
		#endregion
	}
}

