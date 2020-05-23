using System.Collections;
using System.Collections.Generic;
using Models.Interfaces;

namespace Models
{
    public abstract class BaseFilterModel : IBaseFilterModel
    {
        public string GetFilter()
        {
            string result = "?";

            foreach (var prop in this.GetType().GetProperties())
            {
                var valore = prop.GetValue(this);
                if (valore != null)
                {
                    if (valore is IList)
                    {
                        if (prop.Name.EndsWith("ListInt"))
                        {
                            foreach (var item in ((List<int>)valore))
                            {
                                if (result != "?")
                                    result += "&" + prop.Name.Replace("ListInt", "") + "=" + item;
                                else
                                    result += prop.Name.Replace("ListInt", "") + "=" + item;
                            }
                        }
                    }
                    else if (valore.ToString().Trim().Length > 0)
                        if (valore.ToString() != "0" && !string.IsNullOrEmpty(valore.ToString()))
                            if (result != "?")
                                result += "&" + prop.Name + "=" + valore;
                            else
                                result += prop.Name + "=" + valore;
                }
            }
            return result;
        }

    }
}
