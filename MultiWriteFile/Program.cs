namespace MultiWriteFile
{
    internal class Program
    {
        private const string FILE_SUB_PATH = @"log/out.txt";

        private static readonly object _fileLock = new();

        static async Task Main(string[] args)
        {
            try
            {
                Console.Clear();

                var fileProcessor = new FileProcessor(Path.Combine(Environment.CurrentDirectory, FILE_SUB_PATH));

                //Write the initial record
                fileProcessor.WriteLIne($"0, 0, {DateTime.Now:HH:mm:ss.fff}");
                
                var writeTasks = new Task[10];
                for (int i = 0; i < 10; i++)
                {
                    writeTasks[i] = Task.Run(() => WriteToFile(fileProcessor));
                }

                try
                {
                    await Task.WhenAll(writeTasks);
                }
                catch
                {
                    //UNwrap exceptions raised by tasks if any  
                    var failedTasks = writeTasks.Where(t => t.IsFaulted).ToList();
                    failedTasks.Select(t => t.Exception?.Flatten()).ToList().ForEach(ex =>
                        {
                            Console.WriteLine("An exception occured running a task: ");
                            Console.WriteLine(ex.ToString());
                        }
                    );
                }

                Console.WriteLine($"Total lines {fileProcessor.Lines} written in {fileProcessor.FileName}");
                
                if(!Console.IsOutputRedirected)
                {
                    // Wait for key press to exit only if a terminal is attached
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: ");
                Console.WriteLine(ex.ToString());
            }
        }

        private static void WriteToFile(FileProcessor fileProcessor)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            for (int i = 0; i < 10; i++)
            {
                lock (_fileLock)
                {
                    fileProcessor.WriteLIne($"{fileProcessor.Lines}, {threadId}, {DateTime.Now:HH:mm:ss.fff}");
                }
            }
        }
    }
}