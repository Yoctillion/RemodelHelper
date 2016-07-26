using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemodelHelper.Models
{
    public class UnsureValue
    {
        public int Value { get; set; }

        public bool IsSure { get; set; }

        public override string ToString()
        {
            return (Value >= 0 ? Value.ToString() : "") + (this.IsSure ? "" : "?");
        }

        public static implicit operator int(UnsureValue value)
        {
            return value.Value;
        }

        public static implicit operator UnsureValue(int value)
        {
            return new UnsureValue { Value = value, IsSure = true };
        }

        public static implicit operator UnsureValue(double value)
        {
            return new UnsureValue { Value = (int)value, IsSure = false };
        }

        public static implicit operator UnsureValue(decimal value)
        {
            return new UnsureValue { Value = (int)value, IsSure = false };
        }
    }
}
