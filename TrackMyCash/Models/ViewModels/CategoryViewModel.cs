using System.ComponentModel.DataAnnotations;

namespace TrackMyCash.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва категорії є обов'язковою")]
        [StringLength(50, ErrorMessage = "Назва не може перевищувати 50 символів")]
        [Display(Name = "Назва категорії")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Тип категорії є обов'язковим")]
        [Display(Name = "Тип")]
        public string Type { get; set; } = string.Empty; // "Income" або "Expense"

        [Display(Name = "Стандартна категорія")]
        public bool IsDefault { get; set; } = false;
    }
}