using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantic.Ef
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Guid { get; set; }
        public bool IsDeleted { get;set;}
        public long UpdatedAt { get;set;}
        public long InsertedAt { get;set;}    
        public string CreatedBy {get;set;}    
        public string UpdatedBy {get;set;}          
    }
}