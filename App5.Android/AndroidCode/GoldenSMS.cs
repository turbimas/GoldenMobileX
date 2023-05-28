using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Telephony;
using System;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(SmsService_Android))]

public class SmsService_Android : ISmsService
{
    public List<SmsMessage> GetRecentSmsMessages(DateTime fromDate)
    {
        long millis = Convert.ToInt64(Math.Floor((fromDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds));
        var resolver = Application.Context.ContentResolver;
        var smsMessages = new List<SmsMessage>();

        string[] projection = { "address", "body", "date" };




        var cursor = resolver.Query(
            Telephony.Sms.Inbox.ContentUri,
            projection,
            $"date >   {millis}",
            null,
            "date DESC");

        while (cursor.MoveToNext())
        {
            var address = cursor.GetString(cursor.GetColumnIndex("address"));
            var body = cursor.GetString(cursor.GetColumnIndex("body"));
            var dateStr = cursor.GetString(cursor.GetColumnIndex("date"));
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(long.Parse(dateStr)).ToLocalTime();

            //if (date > fromDate)
            smsMessages.Add(new SmsMessage { Address = address, Body = body, Date = date });

        }
        // smsMessages.Add(new SmsMessage { Address="test", Body="test", Date=DateTime.Now });
        return smsMessages;
    }

 
}
