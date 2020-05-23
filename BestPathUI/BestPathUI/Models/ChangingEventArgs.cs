using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ChangingEventArgs : ChangedEventArgs
    {
        public bool Cancel { get; set; }
    }
}
