using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BestPathUI.Persistence
{
    //we might have to use it but hopefully not because it's really messed up
    public static class PersistanceComponents
    {
        private static IList<ComponentBase> _components { get; set; }
        public static ComponentBase GetComponent(Type type)
        {
            if(_components.Any(x => x.GetType() == type))
                return _components.FirstOrDefault(x => x.GetType() == type);
            else
            {
                var component = (ComponentBase)FormatterServices.GetUninitializedObject(type);
                _components.Add(component);
                return component;
            }
        }

        public static void SaveComponent(ComponentBase component)
        {
            if(!_components.Contains(component))
                _components.Add(component);
            else
            {
                _components.Remove(component);
                _components.Add(component);
            }
        }
    }
}
