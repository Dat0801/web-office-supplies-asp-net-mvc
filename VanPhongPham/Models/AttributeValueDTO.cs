using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class AttributeValueDTO
    {
        public string Attribute_value_id { get; set; }
        public string Value { get; set; }
        public string Attribute_id { get; set; }
        public static object GetSortKey(string value)
        {
            var numberString = new string(value.TakeWhile(c => char.IsDigit(c)).ToArray());

            if (int.TryParse(numberString, out int number)) // Nếu tìm thấy số, trả về số
            {
                return number;
            }
            else
            {
                return value; // Nếu không có số, trả về chuỗi
            }
        }
    }
}