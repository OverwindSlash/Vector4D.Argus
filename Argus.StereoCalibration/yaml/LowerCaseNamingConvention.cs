using YamlDotNet.Serialization;

namespace Argus.StereoCalibration.yaml
{
    public class OnlyLowerCaseNamingConvention : INamingConvention
    {
        private OnlyLowerCaseNamingConvention() { }

        public string Apply(string value)
        {
            return value.ToLower();
        }

        public static readonly INamingConvention Instance = new OnlyLowerCaseNamingConvention();
    }
}
