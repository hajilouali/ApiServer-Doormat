using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public enum ExpertCordition
    {
        [Display(Name ="در انتظار بازدید")]
        PendingForVisit,
        [Display(Name = "بازدید شده")]
        Issued,

    }
    public class Expert : BaseEntity
    {
        public Expert()
        {
            
            DateTime = DateTime.Now;
            ExpertCordition = ExpertCordition.PendingForVisit;
        }
        public DateTime DateTime { get; set; }
        public ExpertCordition ExpertCordition { get; set; }
        public int Client_ID { get; set; }
        



        public Client Client { get; set; }
        public ICollection<ExpertHistory> ExpertHistories { get; set; }
    }
    public class ExpertConfiguration : IEntityTypeConfiguration<Expert>
    {
        public void Configure(EntityTypeBuilder<Expert> builder)
        {
            builder.HasOne(p => p.Client).WithMany(p => p.Experts).HasForeignKey(p => p.Client_ID);
        }
    }
}