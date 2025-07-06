namespace Homework.Models
{
    public class Customer
    {
        public long Id { get; set; }
        public decimal Score { get; set; }
        public int Rank { get; set; }

        public Customer(long id, decimal score)
        {
            Id = id;
            Score = score;
        }
       
    }
}
