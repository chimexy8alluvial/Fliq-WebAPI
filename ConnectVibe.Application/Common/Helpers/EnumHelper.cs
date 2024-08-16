using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Common.Helpers
{
    public static class EnumHelper
    {
        public static TEnum ConvertToEnum<TEnum>(int value) where TEnum : struct, Enum
        {
            // Check if the value is a valid enum value
            if (Enum.IsDefined(typeof(TEnum), value))
            {
                return (TEnum)(object)value;
            }

            throw new ArgumentException($"The value {value} is not valid for enum {typeof(TEnum).Name}");
        }
    }
}