using System.ComponentModel.DataAnnotations;
using TrackMyCash.Models;

namespace TrackMyCash.Models.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Сума є обов'язковою")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сума має бути більше 0")]
        [Display(Name = "Сума")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Тип транзакції є обов'язковим")]
        [Display(Name = "Тип")]
        public string Type { get; set; } = string.Empty; // "Income" або "Expense"

        [Required(ErrorMessage = "Категорія є обов'язковою")]
        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }

        [Display(Name = "Коментар")]
        [StringLength(200)]
        public string? Comment { get; set; }

        [Display(Name = "Дата")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Для відображення назви категорії у списку
        public string? CategoryName { get; set; }
    }
}