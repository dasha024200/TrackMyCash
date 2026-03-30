using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackMyCash.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public string Type { get; set; } = string.Empty; 

        // Це поле база знає, залишаємо його для роботи
        public DateTime DateCreated { get; set; } = DateTime.Now; 
        
        // Додаємо [NotMapped], щоб база НЕ шукала колонку "Date"
        [NotMapped]
        public DateTime Date { get; set; } = DateTime.Now;

        public int UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual Category? Category { get; set; }

        [NotMapped]
        public string CategoryName { get; set; } = string.Empty;
    }
}