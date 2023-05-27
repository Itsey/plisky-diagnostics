<Query Kind="Statements" />

string fn = @"C:\Temp\_Deleteme\Lg\log_2412022.log";

FileStream fs = new FileStream(fn,FileMode.Open,FileAccess.ReadWrite,FileShare.None);
Thread.Sleep(15000);
fs.Close();