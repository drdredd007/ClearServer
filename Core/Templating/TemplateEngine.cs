using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearServer.Core.Templating
{
    /// <summary>
    /// Minimal, dependency-free HTML template renderer used instead of RazorEngine.
    /// Supports "{{Property}}" interpolation (HTML-encoded) and
    /// "{{#if Property}} ... {{else}} ... {{/if}}" conditional blocks.
    /// </summary>
    internal static class TemplateEngine
    {
        private static readonly Regex TagToken = new Regex(@"\{\{\s*(#if\s+[^\}]+|else|/if|[^\}]+?)\s*\}\}", RegexOptions.Compiled);

        public static string Render(string template, object model)
        {
            return RenderBlock(template, model);
        }

        private static string RenderBlock(string template, object model)
        {
            var sb = new StringBuilder();
            int pos = 0;

            while (pos < template.Length)
            {
                Match match = TagToken.Match(template, pos);
                if (!match.Success)
                {
                    sb.Append(template, pos, template.Length - pos);
                    break;
                }

                sb.Append(template, pos, match.Index - pos);
                string tag = match.Groups[1].Value.Trim();

                if (tag.StartsWith("#if", StringComparison.Ordinal))
                {
                    string condition = tag.Substring(3).Trim();
                    int blockStart = match.Index + match.Length;
                    string ifBody, elseBody;
                    int afterIndex = SplitIfBlock(template, blockStart, out ifBody, out elseBody);

                    bool isTrue = IsTruthy(GetValue(model, condition));
                    sb.Append(RenderBlock(isTrue ? ifBody : elseBody, model));
                    pos = afterIndex;
                }
                else
                {
                    object value = GetValue(model, tag);
                    sb.Append(WebUtility.HtmlEncode(value?.ToString() ?? string.Empty));
                    pos = match.Index + match.Length;
                }
            }

            return sb.ToString();
        }

        private static int SplitIfBlock(string template, int start, out string ifBody, out string elseBody)
        {
            int depth = 0;
            int elseIndex = -1;
            int pos = start;

            Match m = TagToken.Match(template, pos);
            while (m.Success)
            {
                string tag = m.Groups[1].Value.Trim();

                if (tag.StartsWith("#if", StringComparison.Ordinal))
                {
                    depth++;
                }
                else if (tag == "/if")
                {
                    if (depth == 0)
                    {
                        ifBody = elseIndex >= 0
                            ? template.Substring(start, elseIndex - start)
                            : template.Substring(start, m.Index - start);
                        elseBody = elseIndex >= 0
                            ? template.Substring(elseIndex, m.Index - elseIndex)
                            : string.Empty;
                        return m.Index + m.Length;
                    }
                    depth--;
                }
                else if (tag == "else" && depth == 0)
                {
                    elseIndex = m.Index + m.Length;
                }

                pos = m.Index + m.Length;
                m = TagToken.Match(template, pos);
            }

            throw new FormatException("Template contains an unterminated {{#if}} block.");
        }

        private static object GetValue(object model, string propertyName)
        {
            if (model == null)
            {
                return null;
            }

            PropertyInfo property = model.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            return property?.GetValue(model);
        }

        private static bool IsTruthy(object value)
        {
            switch (value)
            {
                case null:
                    return false;
                case bool b:
                    return b;
                case string s:
                    return !string.IsNullOrEmpty(s);
                default:
                    return true;
            }
        }
    }
}
