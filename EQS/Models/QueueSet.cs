//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EQS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QueueSet
    {
        public int Id { get; set; }
        public int QueueTime { get; set; }
        public string Client { get; set; }
        public string Operation { get; set; }
        public int TimeNeeded { get; set; }
    }
}
