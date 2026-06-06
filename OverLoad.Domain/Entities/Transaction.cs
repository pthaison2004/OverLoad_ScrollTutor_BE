using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OverLoad.Domain.Entities;

public class Transaction
{
    [Key]
    [Column("transaction_id")]
    [MaxLength(50)]
    public string TransactionId { get; set; } = string.Empty;

    [Column("order_code")]
    public long OrderCode { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("currency")]
    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "PENDING";

    [Column("payment_time")]
    public DateTime PaymentTime { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public Course? Course { get; set; }
}
