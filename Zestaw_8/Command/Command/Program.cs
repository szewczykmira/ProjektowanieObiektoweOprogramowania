using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace Command
{
    public abstract class Command
    {
        protected string source;
        protected string filename;
        public abstract void Execute();
    }

    public class RandomWriter
    {
        public void WriteToFile(string filename, int length)
        {
            Console.WriteLine("Writing to file");
            byte[] data = new byte[length];
            Random rng = new Random();
            rng.NextBytes(data);
            File.WriteAllBytes(filename, data);
        }
    }


    public class FtpDownloadCommand : Command
    {
        string source;
        string filename;
        public FtpDownloadCommand(string stream, string f)
        {
            source = stream;
            filename = f;
        }
        public override void Execute()
        {
            Console.WriteLine("Starting downloading via Ftp");
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(source);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            
            request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
             Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            file.Write(reader.ReadToEnd());
            file.Close();
            Console.WriteLine("Download Complete, status {0}", response.StatusDescription);
    
            reader.Close();
            response.Close();  
        
        }
    }

    public class HttpDownloadCommand : Command
    {
        string source;
        string filename;
        public HttpDownloadCommand(string stream, string f)
        {
            source = stream;
            filename = f;
        }
        public override void Execute()
        {
            Console.WriteLine("Starting downloading via http");
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileAsync(new Uri(source), filename);
            
        }

        static void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //zobaczmy ile procent pobrano: 
            //Console.WriteLine(e.ProgressPercentage.ToString());
        }
        static void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("Downloading via http is complete");
        }

    }

    public class RandomFileCommand : Command
    {
        string source;
        string filename;
        public RandomFileCommand(string source, string f)
        {
            this.source = source;
            filename = f;
        }
        public override void Execute()
        {
            Console.WriteLine("Creating randomfile {0}", filename);
            var writer = new RandomWriter();
            writer.WriteToFile(filename, 1024);
        }
    }

    public class CopyFileCommand : Command
    {
        public CopyFileCommand(string s, string f)
        {
            source = s; filename = f;
        }
        public override void Execute()
        {
            Console.WriteLine("Copying {0} to {1}", source, filename);
            File.Copy(source, filename);
        }
    }


    public class CommandInvoker
    {
        List<Command> commands;
        Queue<Command> queue;
        public CommandInvoker(List<Command> cmds)
        {
            commands = cmds;
            queue = new Queue<Command>();
        }

        public void Start()
        {
            Thread t1 = new Thread(new ThreadStart(this.EnqueueCommands));
            Thread t2 = new Thread(new ThreadStart(this.Execute));
            Thread t3 = new Thread(new ThreadStart(this.Execute));
            t1.Start();
            Thread.Sleep(100);
            t2.Start();
            t3.Start();
            t1.Join();
            t2.Join();
            t3.Join();
        }

        public void EnqueueCommands()
        {
            foreach (var cmd in commands)
            {
                Console.WriteLine(cmd);
                queue.Enqueue(cmd);
            }
        }

        public void Execute()
        {
            while (queue.Count > 0)
            {
                Command cmd = queue.Dequeue();
                cmd.Execute();
            }
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            var commands = new List<Command>();
            commands.Add(new HttpDownloadCommand("http://www.google.pl/images/srpr/logo4w.png", "test/logo.png"));
            commands.Add(new FtpDownloadCommand("ftp://kernel.org/pub/README_ABOUT_BZ2_FILES", "test/test.txt")); 
            commands.Add(new RandomFileCommand("", "test/random"));
            commands.Add(new CopyFileCommand("test.txt", "test/test_copy.txt"));
            var invoker = new CommandInvoker(commands);
            invoker.Start();
            Console.ReadLine();
        }
    }
}
