using Kvasir.Localization;
using System;

namespace UT.Kvasir.Translation {
    internal static class TestLocalizations {
        public enum Language { English, Spanish, French, Hebrew, Hindi, German, Arabic, Japanese, Esperanto, Italian }
        public enum System { Imperial, Metric, Celsius, Fahrenheit, Kelvin }
        public enum Calendar { Julian, Gregorian }

        public record struct Measurement(double Value, string Unit);

        public sealed class LocalizedText : Localization<string, Language, string> {
            public LocalizedText(string key) : base(key) {}
            public new string this[Language locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public sealed class LocalizedNullableText : Localization<string, Language, string?> {
            public LocalizedNullableText(string key) : base(key) {}
            public new string? this[Language locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public sealed class LocalizedReadOnlyText : Localization<string, Language, string> {
            public LocalizedReadOnlyText(string key) : base(key) {}
        }
        public sealed class LocalizedMeasure : Localization<ulong, System, Measurement> {
            public LocalizedMeasure(ulong key) : base(key) {}
            public new Measurement this[System locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public sealed class LocalizedDate : Localization<Guid, Calendar, DateOnly> {
            public LocalizedDate(Guid key) : base(key) {}
            public new DateOnly this[Calendar locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public sealed class LocalizedCurrency : Localization<string, string, decimal> {
            public LocalizedCurrency(string key) : base(key) {}
            public new decimal this[string locale] {
                get { return base[locale]; }
                set { base[locale] = value; }
            }
        }
        public sealed class LocalizedRating : Localization<short, char, double> {
            public LocalizedRating(short key) : base(key) {}
        }
    }
}
