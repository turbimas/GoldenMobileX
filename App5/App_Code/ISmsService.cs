using System;
using System.Collections.Generic;
using System.Text;
public interface ISmsService
{
    List<SmsMessage> GetRecentSmsMessages(DateTime fromDate);
 
}
public class SmsMessage
{
    public string Address { get; set; }
    public string Body { get; set; }
    public DateTime Date { get; set; }
 
}