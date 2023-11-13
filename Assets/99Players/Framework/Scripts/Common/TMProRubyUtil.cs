using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
public static class TMProRubyUtil {
    private static readonly Regex TagRegex = new Regex("<r=\"?(?<ruby>.*?)\"?>(?<kanji>.*?)</r>", RegexOptions.IgnoreCase);
    private const string StartTag = "<voffset=1em><size=50%>";
    private const string EndTag = "</size></voffset>";
    private static readonly StringBuilder builder = new StringBuilder(1024);
    private const int StringBuilderCapacity = 1024;
    public static void SetTextAndExpandRuby(this TMP_Text tmpText, string text, bool fixedLineHeight = false, bool autoMarginTop = true) {
        bool flag = false;
        if (fixedLineHeight && autoMarginTop) {
            int num = text.IndexOf('\n');
            string input = (num > 1) ? text.Substring(0, num + 1) : text;
            flag = TagRegex.IsMatch(input);
        }
        text = GetExpandText(text);
        if (fixedLineHeight) {
            float num2 = tmpText.font.faceInfo.lineHeight / (float)tmpText.font.faceInfo.pointSize;
            text = $"<line-height={num2:F3}em>{text}";
            if (autoMarginTop) {
                Vector4 margin = tmpText.margin;
                margin.y = (flag ? (0f - tmpText.fontSize * 0.55f) : 0f);
                margin.y *= (tmpText.isOrthographic ? 1f : 0.1f);
                tmpText.margin = margin;
            }
        }
        tmpText.text = text;
    }
    public static string GetExpandText(string text) {
        Match match = TagRegex.Match(text);
        while (match.Success) {
            if (match.Groups.Count > 2) {
                builder.Length = 0;
                string value = match.Groups["ruby"].Value;
                int length = value.Length;
                string value2 = match.Groups["kanji"].Value;
                int num = value2.Length * 2;
                float num2 = (num < length) ? ((float)(length - num) * 0.25f) : 0f;
                if (num2 < 0f || num2 > 0f) {
                    builder.Append($"<space={num2:F2}em>");
                }
                num2 = 0f - ((float)num * 0.25f + (float)length * 0.25f);
                builder.Append(string.Format("{0}<space={1:F2}em>{2}{3}{4}", value2, num2, "<voffset=1em><size=50%>", value, "</size></voffset>"));
                num2 = ((num > length) ? ((float)(num - length) * 0.25f) : 0f);
                if (num2 < 0f || num2 > 0f) {
                    builder.Append($"<space={num2:F2}em>");
                }
                text = text.Replace(match.Groups[0].Value, builder.ToString());
            }
            match = match.NextMatch();
        }
        return text;
    }
    public static string RemoveRubyTag(string text) {
        Match match = TagRegex.Match(text);
        while (match.Success) {
            if (match.Groups.Count > 2) {
                text = text.Replace(match.Groups[0].Value, match.Groups["kanji"].Value);
            }
            match = match.NextMatch();
        }
        return text;
    }
}
