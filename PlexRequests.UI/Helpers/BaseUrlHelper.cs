﻿#region Copyright
// /************************************************************************
//    Copyright (c) 2016 Jamie Rees
//    File: BaseUrlHelper.cs
//    Created By: Jamie Rees
//   
//    Permission is hereby granted, free of charge, to any person obtaining
//    a copy of this software and associated documentation files (the
//    "Software"), to deal in the Software without restriction, including
//    without limitation the rights to use, copy, modify, merge, publish,
//    distribute, sublicense, and/or sell copies of the Software, and to
//    permit persons to whom the Software is furnished to do so, subject to
//    the following conditions:
//   
//    The above copyright notice and this permission notice shall be
//    included in all copies or substantial portions of the Software.
//   
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//  ************************************************************************/
#endregion
using System.Collections.Generic;
using System.Text;
using Nancy;
using Nancy.ViewEngines.Razor;

using PlexRequests.Core;
using PlexRequests.Core.SettingModels;
using PlexRequests.Helpers;

namespace PlexRequests.UI.Helpers
{
    public static class BaseUrlHelper
    {
        static BaseUrlHelper()
        {
            Locator = ServiceLocator.Instance;
            Cache = Locator.Resolve<ICacheProvider>();
        }
        private static ICacheProvider Cache { get; }
        private static ServiceLocator Locator { get; }
        private static string _Assembly;

        // We don't need to call the AssemblyHelper every time, the value will never change in the application life cycle.
        private static string Assembly
        {
            get
            {
                if (string.IsNullOrEmpty(_Assembly))
                {
                    _Assembly = AssemblyHelper.GetProductVersion();
                }
                return _Assembly;
            }
        }

        public static IHtmlString LoadAssets(this HtmlHelpers helper)
        {
            var sb = new StringBuilder();
            var assetLocation = GetBaseUrl();

            var content = GetContentUrl(assetLocation);
            var settings = GetSettings();
            if (string.IsNullOrEmpty(settings.ThemeName))
            {
                settings.ThemeName = Themes.PlexTheme;
            }
            if (settings.ThemeName == "PlexBootstrap.css") settings.ThemeName = Themes.PlexTheme;
            if (settings.ThemeName == "OriginalBootstrap.css") settings.ThemeName = Themes.OriginalTheme;

            var startUrl = $"{content}/Content_{Assembly}";

            var styleAssets = new List<string>
            {
                $"<link rel=\"stylesheet\" href=\"{startUrl}/bootstrap.css\" type=\"text/css\"/>",
                $"<link rel=\"stylesheet\" href=\"{startUrl}/font-awesome.css\" type=\"text/css\"/>",
                $"<link rel=\"stylesheet\" href=\"{startUrl}/pace.min.css\" type=\"text/css\"/>",
                $"<link rel=\"stylesheet\" href=\"{startUrl}/awesome-bootstrap-checkbox.css\" type=\"text/css\"/>",
                $"<link rel=\"stylesheet\" href=\"{startUrl}/base.css\" type=\"text/css\"/>",
                $"<link rel=\"stylesheet\" href=\"{startUrl}/Themes/{settings.ThemeName}\" type=\"text/css\"/>",
                $"<link rel=\"stylesheet\" href=\"{startUrl}/datepicker.min.css\" type=\"text/css\"/>"
            };


            var scriptAssets = new List<string>
            {
                $"<script src=\"{startUrl}/jquery-2.2.1.min.js\"></script>",
                $"<script src=\"{startUrl}/app/app.js\"></script>",
                $"<script src=\"{startUrl}/handlebars.min.js\"></script>",
                $"<script src=\"{startUrl}/bootstrap.min.js\"></script>",
                $"<script src=\"{startUrl}/bootstrap-notify.min.js\"></script>",
                $"<script src=\"{startUrl}/site.js\"></script>",
                $"<script src=\"{startUrl}/pace.min.js\"></script>",
                $"<script src=\"{startUrl}/jquery.mixitup.js\"></script>",
                $"<script src=\"{startUrl}/moment.min.js\"></script>",
                $"<script src=\"{startUrl}/bootstrap-datetimepicker.min.js\"></script>"
            };


            foreach (var a in styleAssets)
            {
                sb.AppendLine(a);
            }

            foreach (var a in scriptAssets)
            {
                sb.AppendLine(a);
            }


            return helper.Raw(sb.ToString());
        }

        public static IHtmlString LoadSearchAssets(this HtmlHelpers helper)
        {
            var sb = new StringBuilder();
            var assetLocation = GetBaseUrl();

            var content = GetContentUrl(assetLocation);

            sb.AppendLine($"<script src=\"{content}/Content_{Assembly}/search.js\" type=\"text/javascript\"></script>");

            return helper.Raw(sb.ToString());
        }

        public static IHtmlString LoadRequestAssets(this HtmlHelpers helper)
        {
            var sb = new StringBuilder();
            var assetLocation = GetBaseUrl();

            var content = GetContentUrl(assetLocation);

            sb.AppendLine($"<script src=\"{content}/Content_{Assembly}/requests.js\" type=\"text/javascript\"></script>");

            return helper.Raw(sb.ToString());
        }

        public static IHtmlString LoadIssueAssets(this HtmlHelpers helper)
        {
            var sb = new StringBuilder();
            var assetLocation = GetBaseUrl();

            var content = GetContentUrl(assetLocation);

            sb.AppendLine($"<script src=\"{content}/Content_{Assembly}/issues.js\" type=\"text/javascript\"></script>");

            return helper.Raw(sb.ToString());
        }

        public static IHtmlString LoadIssueDetailsAssets(this HtmlHelpers helper)
        {
            var assetLocation = GetBaseUrl();
            var content = GetContentUrl(assetLocation);

            var asset = $"<script src=\"{content}/Content_{Assembly}/issue-details.js\" type=\"text/javascript\"></script>";

            return helper.Raw(asset);
        }

        public static IHtmlString LoadTableAssets(this HtmlHelpers helper)
        {
            var sb = new StringBuilder();
            var assetLocation = GetBaseUrl();

            var content = GetContentUrl(assetLocation);

            sb.AppendLine($"<script src=\"{content}/Content_{Assembly}/datatables.min.js\" type=\"text/javascript\"></script>");
            sb.AppendLine($"<link rel=\"stylesheet\" type=\"text/css\" href=\"{content}/Content_{Assembly}/dataTables.bootstrap.css\" />");

            return helper.Raw(sb.ToString());
        }

        public static IHtmlString LoadAnalytics(this HtmlHelpers helper)
        {
            var settings = GetSettings();
            if (!settings.CollectAnalyticData)
            {
                return helper.Raw(string.Empty);
            }

            var assetLocation = GetBaseUrl();
            var content = GetContentUrl(assetLocation);

            var asset = $"<script src=\"{content}/Content_{Assembly}/analytics.js\" type=\"text/javascript\"></script>";

            return helper.Raw(asset);
        }

        public static IHtmlString GetSidebarUrl(this HtmlHelpers helper, NancyContext context, string url, string title)
        {
            var content = GetLinkUrl(GetBaseUrl());
            if (!string.IsNullOrEmpty(content))
            {
                url = $"/{content}{url}";
            }
            var returnString = context.Request.Path == url 
                                      ? $"<a class=\"list-group-item active\" href=\"{url}\">{title}</a>" 
                                      : $"<a class=\"list-group-item\" href=\"{url}\">{title}</a>";

            return helper.Raw(returnString);
        }

        public static IHtmlString GetNavbarUrl(this HtmlHelpers helper, NancyContext context, string url, string title, string fontIcon)
        {
            var content = GetLinkUrl(GetBaseUrl());
            if (!string.IsNullOrEmpty(content))
            {
                url = $"/{content}{url}";
            }
            var returnString = context.Request.Path == url ? 
                                      $"<li class=\"active\"><a href=\"{url}\"><i class=\"fa fa-{fontIcon}\"></i> {title}</a></li>" 
                                      : $"<li><a href=\"{url}\"><i class=\"fa fa-{fontIcon}\"></i> {title}</a></li>";

            return helper.Raw(returnString);
        }

        public static IHtmlString GetNavbarUrl(this HtmlHelpers helper, NancyContext context, string url, string title, string fontIcon, string extraHtml)
        {
            var content = GetLinkUrl(GetBaseUrl());
            if (!string.IsNullOrEmpty(content))
            {
                url = $"/{content}{url}";
            }

            var returnString = context.Request.Path == url 
                ? $"<li class=\"active\"><a href=\"{url}\"><i class=\"fa fa-{fontIcon}\"></i> {title} {extraHtml}</a></li>" 
                : $"<li><a href=\"{url}\"><i class=\"fa fa-{fontIcon}\"></i> {title} {extraHtml}</a></li>";

            return helper.Raw(returnString);
        }

        public static IHtmlString GetBaseUrl(this HtmlHelpers helper)
        {
            return helper.Raw(GetBaseUrl());
        }

        private static string GetBaseUrl()
        {
            return GetSettings().BaseUrl;
        }

        private static PlexRequestSettings GetSettings()
        {
            var returnValue = Cache.GetOrSet(CacheKeys.GetPlexRequestSettings, () =>
            {
                var settings = Locator.Resolve<ISettingsService<PlexRequestSettings>>().GetSettings();
                return settings;
            });
            return returnValue;
        }

        private static string GetLinkUrl(string assetLocation)
        {
            return string.IsNullOrEmpty(assetLocation) ? string.Empty : $"{assetLocation}";
        }
        private static string GetContentUrl(string assetLocation)
        {
            return string.IsNullOrEmpty(assetLocation) ? string.Empty : $"/{assetLocation}";
        }
    }
}