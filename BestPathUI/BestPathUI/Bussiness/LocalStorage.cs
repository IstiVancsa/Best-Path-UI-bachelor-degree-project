using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Bussiness
{
    public class LocalStorage
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Cities { get; set; }
        public object this[string propertyName]
        {
            get
            {
                System.Reflection.PropertyInfo property = GetType().GetProperty(propertyName);
                return property.GetValue(this, null);
            }
            set
            {
                System.Reflection.PropertyInfo property = GetType().GetProperty(propertyName);
                property.SetValue(this, value);
            }
        }
    }
}
