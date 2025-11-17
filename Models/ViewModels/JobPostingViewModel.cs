using System.ComponentModel.DataAnnotations;

namespace StajPortal.Models.ViewModels
{
    public class JobPostingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İlan başlığı gereklidir")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        [Display(Name = "İlan Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlan açıklaması gereklidir")]
        [StringLength(5000, MinimumLength = 50, ErrorMessage = "Açıklama 50-5000 karakter arasında olmalıdır")]
        [Display(Name = "İlan Açıklaması")]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Departman")]
        public string? Department { get; set; }

        [StringLength(100)]
        [Display(Name = "Şehir")]
        public string? City { get; set; }

        [Display(Name = "Başlangıç Tarihi")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
    }
}

