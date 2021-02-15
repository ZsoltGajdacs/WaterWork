﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkTimeStat.Enums
{
    internal enum WorkPlaceType
    {
        [Description("Irodai munkanap")]
        [Display(Name = "Iroda")]
        OFFICE,
        [Description("Home Office")]
        [Display(Name = "Home Office")]
        HOME,
        [Description("Ügyfél telephelyén töltött munkanap")]
        [Display(Name = "Ügyfél telephelyén")]
        CUSTOMER,
        [Description("Máshol történt munkavégzés")]
        [Display(Name = "Máshol")]
        OTHER
    }
}