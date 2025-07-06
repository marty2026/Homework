using Homework.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Homework.Controllers
{

    [ApiController]
    public class LeaderBoardController : ControllerBase
    {
        private static readonly SortedSet<Customer> sortedCustomers =
            new (Comparer<Customer>.Create((x, y) =>
            { if (y.Score.CompareTo(x.Score) == 0) 
                    return x.Id.CompareTo(y.Id); 
                return y.Score.CompareTo(x.Score); }));
        private static readonly Dictionary<long, Customer> customerMap = [];

        [HttpPost]
        [Route("/customer/{customerid}/score/{score}")]
        public ActionResult<int> CreateCustomer(long customerid, [Range(-1000, 1000)] decimal score)
        {
            if (!customerMap.TryGetValue(customerid, out Customer? current)) // Use nullable type for 'current'
            {
                Customer newCustomer = new (customerid, score);
                sortedCustomers.Add(newCustomer);
                customerMap[customerid] = newCustomer;
            }
            else
            {
                sortedCustomers.Remove(current);
                current.Score += score;
                sortedCustomers.Add(current);
            }
            return Ok();
        }

        [HttpGet]
        [Route("/leaderboard")]
        public ActionResult<IEnumerable<Customer>> GetCustomerByRank(int start,int end)
        {
            List <Customer> res = new();
            SortedSet<Customer>.Enumerator en = sortedCustomers.GetEnumerator();
            int rank = 0;
            while (en.MoveNext())
            {   
                rank++;
                if (rank >= start && rank <= end)
                {
                    res.Add(en.Current);
                    en.Current.Rank = rank;
                }
                if(rank > end)
                {
                    break;
                }
            }
            return Ok(res);
        }

        [HttpGet]
        [Route("/leaderboard/{customerid}")]
        public ActionResult<IEnumerable<Customer>> Details(long customerid,int high,int low)
        {
            int range = high;
            Customer customer = customerMap[customerid];
            Queue<Customer> queue = new ();
            SortedSet<Customer>.Enumerator en = sortedCustomers.GetEnumerator();
            bool foundCustomer = false;
            int rank = 0;
            while (en.MoveNext())
            {
                rank++;
                Customer current = en.Current;
                current.Rank = rank;
                queue.Enqueue(current);
                
                if (current.Id == customerid)
                {
                    foundCustomer = true;
                    range = low;
                }else range--;
                if (range < 0&&!foundCustomer)
                {
                    queue.Dequeue();
                    range++;
                }
                if(range==0&&foundCustomer) break;
            }

            return Ok(queue);
        }

    }
}
