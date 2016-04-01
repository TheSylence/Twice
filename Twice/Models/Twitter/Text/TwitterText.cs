using System.Text.RegularExpressions;
using Pattern = System.Text.RegularExpressions.Regex;
// ReSharper disable InconsistentNaming

/*The MIT License (MIT)
Copyright (c) 2012 Linus Birgerstam

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.*/

namespace Twice.Models.Twitter.Text
{
	public static class TldLib
	{

		public static readonly string[] Country = { "ac", "ad", "ae", "af", "ag", "ai", "al", "am", "an", "ao", "aq", "ar", "as", "at", "au", "aw", "ax", "az", "ba", "bb", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bl", "bm", "bn", "bo", "bq", "br", "bs", "bt", "bv", "bw", "by", "bz", "ca", "cc", "cd", "cf", "cg", "ch", "ci", "ck", "cl", "cm", "cn", "co", "cr", "cu", "cv", "cw", "cx", "cy", "cz", "de", "dj", "dk", "dm", "do", "dz", "ec", "ee", "eg", "eh", "er", "es", "et", "eu", "fi", "fj", "fk", "fm", "fo", "fr", "ga", "gb", "gd", "ge", "gf", "gg", "gh", "gi", "gl", "gm", "gn", "gp", "gq", "gr", "gs", "gt", "gu", "gw", "gy", "hk", "hm", "hn", "hr", "ht", "hu", "id", "ie", "il", "im", "in", "io", "iq", "ir", "is", "it", "je", "jm", "jo", "jp", "ke", "kg", "kh", "ki", "km", "kn", "kp", "kr", "kw", "ky", "kz", "la", "lb", "lc", "li", "lk", "lr", "ls", "lt", "lu", "lv", "ly", "ma", "mc", "md", "me", "mf", "mg", "mh", "mk", "ml", "mm", "mn", "mo", "mp", "mq", "mr", "ms", "mt", "mu", "mv", "mw", "mx", "my", "mz", "na", "nc", "ne", "nf", "ng", "ni", "nl", "no", "np", "nr", "nu", "nz", "om", "pa", "pe", "pf", "pg", "ph", "pk", "pl", "pm", "pn", "pr", "ps", "pt", "pw", "py", "qa", "re", "ro", "rs", "ru", "rw", "sa", "sb", "sc", "sd", "se", "sg", "sh", "si", "sj", "sk", "sl", "sm", "sn", "so", "sr", "ss", "st", "su", "sv", "sx", "sy", "sz", "tc", "td", "tf", "tg", "th", "tj", "tk", "tl", "tm", "tn", "to", "tp", "tr", "tt", "tv", "tw", "tz", "ua", "ug", "uk", "um", "us", "uy", "uz", "va", "vc", "ve", "vg", "vi", "vn", "vu", "wf", "ws", "ye", "yt", "za", "zm", "zw", "ελ", "бел", "мкд", "мон", "рф", "срб", "укр", "қаз", "հայ", "الاردن", "الجزائر", "السعودية", "المغرب", "امارات", "ایران", "بھارت", "تونس", "سودان", "سورية", "عراق", "عمان", "فلسطين", "قطر", "مصر", "مليسيا", "پاکستان", "भारत", "বাংলা", "ভারত", "ਭਾਰਤ", "ભારત", "இந்தியா", "இலங்கை", "சிங்கப்பூர்", "భారత్", "ලංකා", "ไทย", "გე", "中国", "中國", "台湾", "台灣", "新加坡", "澳門", "香港", "한국" };

		public static readonly string[] Generic = { "abb", "abbott", "abogado", "academy", "accenture", "accountant", "accountants", "aco", "active", "actor", "ads", "adult", "aeg", "aero", "afl", "agency", "aig", "airforce", "airtel", "allfinanz", "alsace", "amsterdam", "android", "apartments", "app", "aquarelle", "archi", "army", "arpa", "asia", "associates", "attorney", "auction", "audio", "auto", "autos", "axa", "azure", "band", "bank", "bar", "barcelona", "barclaycard", "barclays", "bargains", "bauhaus", "bayern", "bbc", "bbva", "bcn", "beer", "bentley", "berlin", "best", "bet", "bharti", "bible", "bid", "bike", "bing", "bingo", "bio", "biz", "black", "blackfriday", "bloomberg", "blue", "bmw", "bnl", "bnpparibas", "boats", "bond", "boo", "boots", "boutique", "bradesco", "bridgestone", "broker", "brother", "brussels", "budapest", "build", "builders", "business", "buzz", "bzh", "cab", "cafe", "cal", "camera", "camp", "cancerresearch", "canon", "capetown", "capital", "caravan", "cards", "care", "career", "careers", "cars", "cartier", "casa", "cash", "casino", "cat", "catering", "cba", "cbn", "ceb", "center", "ceo", "cern", "cfa", "cfd", "chanel", "channel", "chat", "cheap", "chloe", "christmas", "chrome", "church", "cisco", "citic", "city", "claims", "cleaning", "click", "clinic", "clothing", "cloud", "club", "coach", "codes", "coffee", "college", "cologne", "com", "commbank", "community", "company", "computer", "condos", "construction", "consulting", "contractors", "cooking", "cool", "coop", "corsica", "country", "coupons", "courses", "credit", "creditcard", "cricket", "crown", "crs", "cruises", "cuisinella", "cymru", "cyou", "dabur", "dad", "dance", "date", "dating", "datsun", "day", "dclk", "deals", "degree", "delivery", "delta", "democrat", "dental", "dentist", "desi", "design", "dev", "diamonds", "diet", "digital", "direct", "directory", "discount", "dnp", "docs", "dog", "doha", "domains", "doosan", "download", "drive", "durban", "dvag", "earth", "eat", "edu", "education", "email", "emerck", "energy", "engineer", "engineering", "enterprises", "epson", "equipment", "erni", "esq", "estate", "eurovision", "eus", "events", "everbank", "exchange", "expert", "exposed", "express", "fage", "fail", "faith", "family", "fan", "fans", "farm", "fashion", "feedback", "film", "finance", "financial", "firmdale", "fish", "fishing", "fit", "fitness", "flights", "florist", "flowers", "flsmidth", "fly", "foo", "football", "forex", "forsale", "forum", "foundation", "frl", "frogans", "fund", "furniture", "futbol", "fyi", "gal", "gallery", "game", "garden", "gbiz", "gdn", "gent", "genting", "ggee", "gift", "gifts", "gives", "giving", "glass", "gle", "global", "globo", "gmail", "gmo", "gmx", "gold", "goldpoint", "golf", "goo", "goog", "google", "gop", "gov", "graphics", "gratis", "green", "gripe", "group", "guge", "guide", "guitars", "guru", "hamburg", "hangout", "haus", "healthcare", "help", "here", "hermes", "hiphop", "hitachi", "hiv", "hockey", "holdings", "holiday", "homedepot", "homes", "honda", "horse", "host", "hosting", "hoteles", "hotmail", "house", "how", "hsbc", "ibm", "icbc", "ice", "icu", "ifm", "iinet", "immo", "immobilien", "industries", "infiniti", "info", "ing", "ink", "institute", "insure", "int", "international", "investments", "ipiranga", "irish", "ist", "istanbul", "itau", "iwc", "java", "jcb", "jetzt", "jewelry", "jlc", "jll", "jobs", "joburg", "jprs", "juegos", "kaufen", "kddi", "kim", "kitchen", "kiwi", "koeln", "komatsu", "krd", "kred", "kyoto", "lacaixa", "lancaster", "land", "lasalle", "lat", "latrobe", "law", "lawyer", "lds", "lease", "leclerc", "legal", "lexus", "lgbt", "liaison", "lidl", "life", "lighting", "limited", "limo", "link", "live", "lixil", "loan", "loans", "lol", "london", "lotte", "lotto", "love", "ltda", "lupin", "luxe", "luxury", "madrid", "maif", "maison", "man", "management", "mango", "market", "marketing", "markets", "marriott", "mba", "media", "meet", "melbourne", "meme", "memorial", "men", "menu", "miami", "microsoft", "mil", "mini", "mma", "mobi", "moda", "moe", "mom", "monash", "money", "montblanc", "mormon", "mortgage", "moscow", "motorcycles", "mov", "movie", "movistar", "mtn", "mtpc", "museum", "nadex", "nagoya", "name", "navy", "nec", "net", "netbank", "network", "neustar", "new", "news", "nexus", "ngo", "nhk", "nico", "ninja", "nissan", "nokia", "nra", "nrw", "ntt", "nyc", "office", "okinawa", "omega", "one", "ong", "onl", "online", "ooo", "oracle", "orange", "org", "organic", "osaka", "otsuka", "ovh", "page", "panerai", "paris", "partners", "parts", "party", "pet", "pharmacy", "philips", "photo", "photography", "photos", "physio", "piaget", "pics", "pictet", "pictures", "pink", "pizza", "place", "play", "plumbing", "plus", "pohl", "poker", "porn", "post", "praxi", "press", "pro", "prod", "productions", "prof", "properties", "property", "pub", "qpon", "quebec", "racing", "realtor", "realty", "recipes", "red", "redstone", "rehab", "reise", "reisen", "reit", "ren", "rent", "rentals", "repair", "report", "republican", "rest", "restaurant", "review", "reviews", "rich", "ricoh", "rio", "rip", "rocks", "rodeo", "rsvp", "ruhr", "run", "ryukyu", "saarland", "sakura", "sale", "samsung", "sandvik", "sandvikcoromant", "sanofi", "sap", "sarl", "saxo", "sca", "scb", "schmidt", "scholarships", "school", "schule", "schwarz", "science", "scor", "scot", "seat", "seek", "sener", "services", "sew", "sex", "sexy", "shiksha", "shoes", "show", "shriram", "singles", "site", "ski", "sky", "skype", "sncf", "soccer", "social", "software", "sohu", "solar", "solutions", "sony", "soy", "space", "spiegel", "spreadbetting", "srl", "starhub", "statoil", "studio", "study", "style", "sucks", "supplies", "supply", "support", "surf", "surgery", "suzuki", "swatch", "swiss", "sydney", "systems", "taipei", "tatamotors", "tatar", "tattoo", "tax", "taxi", "team", "tech", "technology", "tel", "telefonica", "temasek", "tennis", "thd", "theater", "tickets", "tienda", "tips", "tires", "tirol", "today", "tokyo", "tools", "top", "toray", "toshiba", "tours", "town", "toyota", "toys", "trade", "trading", "training", "travel", "trust", "tui", "ubs", "university", "uno", "uol", "vacations", "vegas", "ventures", "vermögensberater", "vermögensberatung", "versicherung", "vet", "viajes", "video", "villas", "vin", "vision", "vista", "vistaprint", "vlaanderen", "vodka", "vote", "voting", "voto", "voyage", "wales", "walter", "wang", "watch", "webcam", "website", "wed", "wedding", "weir", "whoswho", "wien", "wiki", "williamhill", "win", "windows", "wine", "wme", "work", "works", "world", "wtc", "wtf", "xbox", "xerox", "xin", "xperia", "xxx", "xyz", "yachts", "yandex", "yodobashi", "yoga", "yokohama", "youtube", "zip", "zone", "zuerich", "дети", "ком", "москва", "онлайн", "орг", "рус", "сайт", "קום", "بازار", "شبكة", "كوم", "موقع", "कॉम", "नेट", "संगठन", "คอม", "みんな", "グーグル", "コム", "世界", "中信", "中文网", "企业", "佛山", "信息", "健康", "八卦", "公司", "公益", "商城", "商店", "商标", "在线", "大拿", "娱乐", "工行", "广东", "慈善", "我爱你", "手机", "政务", "政府", "新闻", "时尚", "机构", "淡马锡", "游戏", "点看", "移动", "组织机构", "网址", "网店", "网络", "谷歌", "集团", "飞利浦", "餐厅", "닷넷", "닷컴", "삼성", "onion" };
	}

	/// <summary>
	/// Patterns and regular expressions used by the twitter text methods.
	/// </summary>
	internal static class Regex
	{

		// Create the equivalent of Java's \p{Alpha}
		// \p{Alpha}: An alphabetic character:[\p{Lower}\p{Upper}]
		// \p{Lower}: A lower-case alphabetic character: [a-z]
		// \p{Upper}: An upper-case alphabetic character:[A-Z]
		private const string ALPHA_CHARS = "a-zA-Z";

		// Create the equivalent of Java's \p{Digit}
		// \p{Digit}: A decimal digit: [0-9]
		private const string NUM_CHARS = "0-9";

		// Create the equivalent of Java's \p{Alnum}
		// \p{Alnum}: An alphanumeric character:[\p{Alpha}\p{Digit}]
		private const string ALNUM_CHARS = ALPHA_CHARS + NUM_CHARS;

		// Create the quivalent of Java's \p{Punct}
		// \p{Punct}: Punctuation: One of !"#$%&'()*+,-./:;<=>?@[\]^_`{|}~
		private static readonly string PUNCT_CHARS = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".Replace( @"\", @"\\" ).Replace( @"]", @"\]" ).Replace( @"-", @"\-" );

		// Space is more than %20, U+3000 for example is the full-width space used with Kanji. Provide a short-hand
		// to access both the list of characters and a pattern suitible for use with String#split
		// Taken from: ActiveSupport::Multibyte::Handlers::UTF8Handler::UNICODE_WHITESPACE
		private const string UNICODE_SPACES = "[" +
				"\u0009-\u000d" +     // White_Space # Cc [5]    <control-0009>..<control-000D>
				"\u0020" +            // White_Space # Zs        SPACE
				"\u0085" +            // White_Space # Cc        <control-0085>
				"\u00a0" +            // White_Space # Zs        NO-BREAK SPACE
				"\u1680" +            // White_Space # Zs        OGHAM SPACE MARK
				"\u180E" +            // White_Space # Zs        MONGOLIAN VOWEL SEPARATOR
				"\u2000-\u200a" +     // White_Space # Zs [11]   EN QUAD..HAIR SPACE
				"\u2028" +            // White_Space # Zl        LINE SEPARATOR
				"\u2029" +            // White_Space # Zp        PARAGRAPH SEPARATOR
				"\u202F" +            // White_Space # Zs        NARROW NO-BREAK SPACE
				"\u205F" +            // White_Space # Zs        MEDIUM MATHEMATICAL SPACE
				"\u3000" +            // White_Space # Zs        IDEOGRAPHIC SPACE
			"]";

		// Character not allowed in Tweets
		private const string INVALID_CONTROL_CHARS = "[" +
				"\ufffe\ufeff" +    // BOM
				"\uffff" +          // Special
				"\u202a-\u202e" +   // Directional change
			"]";

		// Latin accented characters
		// Excludes 0xd7 from the range (the multiplication sign, confusable with "x").
		// Also excludes 0xf7, the division sign
		private const string LATIN_ACCENTS_CHARS =
			"\u00c0-\u00d6\u00d8-\u00f6\u00f8-\u00ff" +                                     // Latin-1
			"\u0100-\u024f" +                                                               // Latin Extended A and B
			"\u0253\u0254\u0256\u0257\u0259\u025b\u0263\u0268\u026f\u0272\u0289\u028b" +    // IPA Extensions
			"\u02bb" +                                                                      // Hawaiian
			"\u0300-\u036f" +                                                               // Combining diacritics
			"\u1e00-\u1eff";                                                                // Latin Extended Additional (mostly for Vietnamese)

		private const string RTL_CHARS =
			"\u0600-\u06FF" +
			"\u0750-\u077F" +
			"\u0590-\u05FF" +
			"\uFE70-\uFEFF";

		// Hashtag related patterns
		private const string HASHTAG_LETTERS = "\\p{L}\\p{M}";
		private const string HASHTAG_NUMERALS = "\\p{Nd}";
		private const string HASHTAG_SPECIAL_CHARS = "_" +       // underscore
													 "\\u200c" + // ZERO WIDTH NON-JOINER (ZWNJ)
													 "\\u200d" + // ZERO WIDTH JOINER (ZWJ)
													 "\\ua67e" + // CYRILLIC KAVYKA
													 "\\u05be" + // HEBREW PUNCTUATION MAQAF
													 "\\u05f3" + // HEBREW PUNCTUATION GERESH
													 "\\u05f4" + // HEBREW PUNCTUATION GERSHAYIM
													 "\\u309b" + // KATAKANA-HIRAGANA VOICED SOUND MARK
													 "\\u309c" + // KATAKANA-HIRAGANA SEMI-VOICED SOUND MARK
													 "\\u30a0" + // KATAKANA-HIRAGANA DOUBLE HYPHEN
													 "\\u30fb" + // KATAKANA MIDDLE DOT
													 "\\u3003" + // DITTO MARK
													 "\\u0f0b" + // TIBETAN MARK INTERSYLLABIC TSHEG
													 "\\u0f0c" + // TIBETAN MARK DELIMITER TSHEG BSTAR
													 "\\u00b7";  // MIDDLE DOT

		private const string HASHTAG_LETTERS_NUMERALS = HASHTAG_LETTERS + HASHTAG_NUMERALS + HASHTAG_SPECIAL_CHARS;

		private const string HASHTAG_LETTERS_SET = "[" + HASHTAG_LETTERS + "]";

		private const string HASHTAG_LETTERS_NUMERALS_SET = "[" + HASHTAG_LETTERS_NUMERALS + "]";

		private const string VALID_HASHTAG_STRING = "(^|[^&" + HASHTAG_LETTERS_NUMERALS + "])(#|\uFF03)(?!\uFE0F|\u20E3)(" + HASHTAG_LETTERS_NUMERALS_SET + "*" + HASHTAG_LETTERS_SET + HASHTAG_LETTERS_NUMERALS_SET + "*)";

		// URL related patterns
		private const string URL_VALID_PRECEEDING_CHARS = "(?:[^A-Z0-9@＠$#＃\u202A-\u202E]|^)";

		private const string URL_VALID_CHARS = ALNUM_CHARS + LATIN_ACCENTS_CHARS;

		private const string URL_VALID_SUBDOMAIN = "(?>(?:[" + URL_VALID_CHARS + "][" + URL_VALID_CHARS + "\\-_]*)?[" + URL_VALID_CHARS + "]\\.)";

		private const string URL_VALID_DOMAIN_NAME = "(?:(?:[" + URL_VALID_CHARS + "][" + URL_VALID_CHARS + "\\-]*)?[" + URL_VALID_CHARS + "]\\.)";

		// Any non-space, non-punctuation characters. \p{Z} = any kind of whitespace or invisible separator.
		private static readonly string URL_VALID_UNICODE_CHARS = "(?:\\.|[^" + PUNCT_CHARS + "\\s\\p{Z}\\p{IsGeneralPunctuation}])";
		private static readonly string URL_VALID_GTLD = "(?:(?:" + string.Join( "|", TldLib.Generic ) + ")(?=[^" + ALNUM_CHARS + "@]|$))";
		private static readonly string URL_VALID_CCTLD = "(?:(?:" + string.Join( "|", TldLib.Country ) + ")(?=[^" + ALNUM_CHARS + "@]|$))";
		private const string URL_PUNYCODE = "(?:xn--[0-9a-z]+)";
		private const string SPECIAL_URL_VALID_CCTLD = "(?:(?:" + "co|tv" + ")(?=[^" + ALNUM_CHARS + "@]|$))";

		private static readonly string URL_VALID_DOMAIN =
			"(?:" +                                                      // subdomains + domain + TLD
				URL_VALID_SUBDOMAIN + "+" + URL_VALID_DOMAIN_NAME +      // e.g. www.twitter.com, foo.co.jp, bar.co.uk
				"(?:" + URL_VALID_GTLD + "|" + URL_VALID_CCTLD + "|" + URL_PUNYCODE + ")" +
				")" +
			"|(?:" +                                                     // domain + gTLD + some ccTLD
				URL_VALID_DOMAIN_NAME +                                  // e.g. twitter.com
				"(?:" + URL_VALID_GTLD + "|" + URL_PUNYCODE + "|" + SPECIAL_URL_VALID_CCTLD + ")" +
			")" +
			"|(?:" + "(?<=https?://)" +
				"(?:" +
				"(?:" + URL_VALID_DOMAIN_NAME + URL_VALID_CCTLD + ")" +  // protocol + domain + ccTLD
				"|(?:" +
					URL_VALID_UNICODE_CHARS + "+\\." +                   // protocol + unicode domain + TLD
					"(?:" + URL_VALID_GTLD + "|" + URL_VALID_CCTLD + ")" +
				")" +
				")" +
			")" +
			"|(?:" +                                                     // domain + ccTLD + '/'
				URL_VALID_DOMAIN_NAME + URL_VALID_CCTLD + "(?=/)" +      // e.g. t.co/
			")";

		private const string URL_VALID_PORT_NUMBER = "(?>[0-9]+)";

		private const string URL_VALID_GENERAL_PATH_CHARS = "[a-z\\p{IsCyrillic}0-9!\\*';:=\\+,.\\$/%#\\[\\]\\-_~\\|&@" + LATIN_ACCENTS_CHARS + "]";

		// Allow URL paths to contain up to two nested levels of balanced parens
		// 1. Used in Wikipedia URLs like /Primer_(film)
		// 2. Used in IIS sessions like /S(dfd346)/
		// 3. Used in Rdio URLs like /track/We_Up_(Album_Version_(Edited))/
		private const string URL_BALANCED_PARENS =
			"\\(" +
				"(?:" +
					URL_VALID_GENERAL_PATH_CHARS + "+" +
					"|" +
					// allow one nested level of balanced parentheses
					"(?:" +
						URL_VALID_GENERAL_PATH_CHARS + "*" +
						"\\(" +
							URL_VALID_GENERAL_PATH_CHARS + "+" +
						"\\)" +
						URL_VALID_GENERAL_PATH_CHARS + "*" +
					")" +
				")" +
			"\\)";

		// Valid end-of-path characters (so /foo. does not gobble the period).
		// 1. Allow =&# for empty URL parameters and other URL-join artifacts
		private const string URL_VALID_PATH_ENDING_CHARS = "[a-z\\p{IsCyrillic}0-9=_#/\\-\\+" + LATIN_ACCENTS_CHARS + "]|(?:" + URL_BALANCED_PARENS + ")";

		private const string URL_VALID_PATH =
			"(?:" +
				"(?:" +
					URL_VALID_GENERAL_PATH_CHARS + "*" +
					"(?:" + URL_BALANCED_PARENS + URL_VALID_GENERAL_PATH_CHARS + "*)*" +
					URL_VALID_PATH_ENDING_CHARS +
				")|(?:@" + URL_VALID_GENERAL_PATH_CHARS + "+/)" +
			")";

		private const string URL_VALID_URL_QUERY_CHARS = "[a-z0-9!?\\*'\\(\\);:&=\\+\\$/%#\\[\\]\\-_\\.,~\\|@]";

		private const string URL_VALID_URL_QUERY_ENDING_CHARS = "[a-z0-9_&=#/-]";

		private static readonly string VALID_URL_PATTERN_STRING =
			"(" +                                                   //  $1 total match
				"(" + URL_VALID_PRECEEDING_CHARS + ")" +            //  $2 Preceeding chracter
				"(" +                                               //  $3 URL
					"(https?://)?" +                                //  $4 Protocol (optional)
					"(" + URL_VALID_DOMAIN + ")" +                  //  $5 Domain(s)
					"(?::(" + URL_VALID_PORT_NUMBER + "))?" +       //  $6 Port number (optional)
					"(/" +
						"(?>" + URL_VALID_PATH + "*)" +
					")?" +                                          //  $7 URL Path and anchor
					"(\\?" + URL_VALID_URL_QUERY_CHARS + "*" +      //  $8 Query string 
						URL_VALID_URL_QUERY_ENDING_CHARS + ")?" +
				")" +
			")";

		private const string AT_SIGNS_CHARS = "@\uFF20";

		private const string DOLLAR_SIGN_CHAR = "\\$";

		// Cashtag related patterns
		private const string CASHTAG = "[a-z]{1,6}(?:[._][a-z]{1,2})?";

		//
		// Begin internal constants
		//
		internal static readonly Pattern INVALID_CHARACTERS = new Pattern( INVALID_CONTROL_CHARS, RegexOptions.IgnoreCase );

		internal static readonly Pattern VALID_HASHTAG = new Pattern( VALID_HASHTAG_STRING, RegexOptions.IgnoreCase );

		internal const int VALID_HASHTAG_GROUP_BEFORE = 1;
		internal const int VALID_HASHTAG_GROUP_HASH = 2;
		internal const int VALID_HASHTAG_GROUP_TAG = 3;

		internal static readonly Pattern INVALID_HASHTAG_MATCH_END = new Pattern( "^(?:[#＃]|://)" );

		internal static readonly Pattern RTL_CHARACTERS = new Pattern( "[" + RTL_CHARS + "]" );

		internal static readonly Pattern AT_SIGNS = new Pattern( "[" + AT_SIGNS_CHARS + "]" );

		internal static readonly Pattern VALID_MENTION_OR_LIST = new Pattern( "([^a-z0-9_!#$%&*" + AT_SIGNS_CHARS + "]|^|(?:^|[^a-z0-9_+~.-])RT:?)(" + AT_SIGNS + "+)([a-z0-9_]{1,20})(/[a-z][a-z0-9_\\-]{0,24})?", RegexOptions.IgnoreCase );
		internal const int VALID_MENTION_OR_LIST_GROUP_BEFORE = 1;
		internal const int VALID_MENTION_OR_LIST_GROUP_AT = 2;
		internal const int VALID_MENTION_OR_LIST_GROUP_USERNAME = 3;
		internal const int VALID_MENTION_OR_LIST_GROUP_LIST = 4;

		internal static readonly Pattern VALID_REPLY = new Pattern( "^(?:" + UNICODE_SPACES + ")*" + AT_SIGNS + "([a-z0-9_]{1,20})", RegexOptions.IgnoreCase );
		internal const int VALID_REPLY_GROUP_USERNAME = 1;

		internal static readonly Pattern INVALID_MENTION_MATCH_END = new Pattern( "^(?:[" + AT_SIGNS_CHARS + LATIN_ACCENTS_CHARS + "]|://)" );

		internal static readonly Pattern VALID_URL = new Pattern( VALID_URL_PATTERN_STRING, RegexOptions.IgnoreCase );

		internal const int VALID_URL_GROUP_ALL = 1;
		internal const int VALID_URL_GROUP_BEFORE = 2;
		internal const int VALID_URL_GROUP_URL = 3;
		internal const int VALID_URL_GROUP_PROTOCOL = 4;
		internal const int VALID_URL_GROUP_DOMAIN = 5;
		internal const int VALID_URL_GROUP_PORT = 6;
		internal const int VALID_URL_GROUP_PATH = 7;
		internal const int VALID_URL_GROUP_QUERY_STRING = 8;

		internal static readonly Pattern VALID_TCO_URL = new Pattern( "^https?:\\/\\/t\\.co\\/[a-z0-9]+", RegexOptions.IgnoreCase );

		internal static readonly Pattern INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN = new Pattern( "[-_./]$" );

		internal static readonly Pattern VALID_CASHTAG = new Pattern( "(^|" + UNICODE_SPACES + ")(" + DOLLAR_SIGN_CHAR + ")(" + CASHTAG + ")" + "(?=$|\\s|[" + PUNCT_CHARS + "])", RegexOptions.IgnoreCase );
		internal const int VALID_CASHTAG_GROUP_BEFORE = 1;
		internal const int VALID_CASHTAG_GROUP_DOLLAR = 2;
		internal const int VALID_CASHTAG_GROUP_CASHTAG = 3;
	}
}
