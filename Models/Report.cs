//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineExam.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Report
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public string ExamName { get; set; }
        public int Score { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
        public float Percentage { get; set; }
        public string Subject1 { get; set; }
        public Nullable<int> Marks1 { get; set; }
        public string Subject2 { get; set; }
        public Nullable<int> Marks2 { get; set; }
        public string Subject3 { get; set; }
        public Nullable<int> Marks3 { get; set; }
    
        public virtual Student Student { get; set; }
    }
}
