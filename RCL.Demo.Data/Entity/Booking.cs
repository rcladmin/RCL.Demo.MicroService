#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RCL.Demo.Data
{
    [Table(name: "rcldemo_booking")]
    public class Booking
    {
        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Begin Date")]
        public DateTime BeginDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Room Number")]
        [MaxLength(50)]
        public string RoomNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("User Id")]
        [MaxLength(50)]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Payment Id")]
        [MaxLength(50)]
        public string PaymentId { get; set; }
    }
}
