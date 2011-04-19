using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace WcfWebApi.Preview4.Explorations.Common
{
    public static class HttpUtils
    {
        public static CultureInfo GetCultureInfoForAcceptedLanguages(IEnumerable<StringWithQualityHeaderValue> accepts)
        {
            var langs = accepts
                .OrderByDescending(v => v.Quality ?? 1)
                .Where(v => (v.Quality ?? 1) > 0)
                .Select(v => v.Value);
            CultureInfo ci = null;
            foreach (var lang in langs)
            {
                try
                {
                    ci = CultureInfo.GetCultureInfo(lang);
                    break;
                }
                catch (CultureNotFoundException e)
                {
                    // retry (did't found another way)
                }
            }
            return ci ?? CultureInfo.CurrentCulture;
        }
    }
}
