﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ChangedEventArgs
    {
        public string Key { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
