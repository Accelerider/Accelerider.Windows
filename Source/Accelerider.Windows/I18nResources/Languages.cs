using System.Collections.Generic;
using System.Globalization;

namespace Accelerider.Windows.I18nResources
{
    public static class Languages
    {
        public static List<CultureInfo> AvailableCultureInfos { get; } = new List<CultureInfo>
        {
            new CultureInfo("en"),
            new CultureInfo("zh-CN"),
            new CultureInfo("ja"),
        };
    }
}
