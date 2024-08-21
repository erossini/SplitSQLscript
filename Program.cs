using System;
using Spectre.Console;

public class Program
{
    static string GetFileName(string filePath, int chunkCount, string destinationFolder)
    {
        string pth = filePath;

        if (destinationFolder != null)
        {
            string fn = Path.GetFileName(filePath);
            pth = Path.Combine(destinationFolder, fn);
        }

        string fileName = $"{pth}_chunk_{chunkCount}.sql";
        return fileName;
    }

    /// <summary>
    /// Splits a file into smaller chunks based on the specified maximum chunk size.
    /// </summary>
    /// <param name="filePath">The path to the file to be split.</param>
    /// <param name="destinationFolder">The destination folder.</param>
    /// <param name="maxChunkSize">The maximum size of each chunk in bytes.</param>
    /// <param name="addGoAtTheEnd">if set to <c>true</c> the procedure add the GO command 
    /// at the end of each file.</param>
    /// <returns></returns>
    static void SplitFile(string filePath, string destinationFolder = "",
        int maxChunkSize = 1024000, bool addGoAtTheEnd = true)
    {
        // Read all lines from the file
        List<string> lines = new List<string>(File.ReadAllLines(filePath));

        if (lines.Count == 0)
        {
            AnsiConsole.MarkupLine("[#ff0000]The file is empty.[/]");
            throw new Exception("The file is empty.");
        }

        Console.WriteLine($"Found {lines.Count} line in the SQL script.");

        List<string> chunk = new List<string>();
        int chunkSize = 0;
        int chunkCount = 1;
        int currentRows = 0;
        int totalRows = 0;
        string? fName;

        if (!string.IsNullOrEmpty(destinationFolder))
            if (!Directory.Exists(destinationFolder))
            {
                try
                {
                    Directory.CreateDirectory(destinationFolder);
                    Console.WriteLine($"Desctination fodler created.");
                }
                catch (Exception ex)
                {
                    throw new Exception("The destination folder can't be created.");
                }
            }

        AnsiConsole.Progress().Start(ctx =>
        {
            var table = new Table();
            table.AddColumn("FileName");
            table.AddColumn("Rows");
            table.Border(TableBorder.MinimalHeavyHead);

            var task = ctx.AddTask("[green]Split file[/]");
            task.MaxValue = lines.Count();

            // Iterate through each line in the file
            foreach (string line in lines)
            {
                chunk.Add(line);
                chunkSize += line.Length;

                currentRows += 1;
                totalRows += 1;

                // Check if the current chunk size exceeds the maximum chunk size
                if (chunkSize >= maxChunkSize)
                {
                    if (addGoAtTheEnd)
                    {
                        chunk.Add("GO");
                        chunkSize += line.Length;
                    }

                    fName = GetFileName(filePath, chunkCount, destinationFolder);
                    table.AddRow(fName, currentRows.ToString());

                    // Write the current chunk to a new file
                    File.WriteAllLines(fName, chunk);
                    chunk.Clear();
                    chunkSize = 0;
                    chunkCount++;

                    //Console.WriteLine($"Created {fName}");
                    //Console.WriteLine($"Contains {currentRows} rows");
                    task.Increment(currentRows);

                    currentRows = 0;
                }
            }

            fName = GetFileName(filePath, chunkCount, destinationFolder);
            // Write any remaining lines in the last chunk
            if (chunk.Count > 0)
            {
                File.WriteAllLines(fName, chunk);
            }

            task.Increment(currentRows);

            table.ShowFooters();
            table.AddRow("Total line read", lines.Count().ToString());
            table.AddRow("Total line wrote", totalRows.ToString());
            AnsiConsole.Write(table);
        });
    }

    static void ShowHelp()
    {
        AnsiConsole.MarkupLine("See how to use this application adding [white]--help[/] as a parameter");
    }

    /// <summary>
    /// Split SQL script in multiple files based on the required size.
    /// </summary>
    /// <param name="file">The SQL script file to split.</param>
    /// <param name="destination">The destination folder. If this is empty or null, the new files will be created in the same directory as the original file.</param>
    /// <param name="limit">The maximum bytes limit for the new files.</param>
    /// <param name="addGo">if set to <c>true</c> the procedure will add the command GO at the end of each file.</param>
    public static void Main(string file, string? destination,
        int limit = 10240000, bool addGo = true)
    {
        if (string.IsNullOrEmpty(file))
        {
            AnsiConsole.MarkupLine("[#ff0000]The SQL script file must be passed![/]");
            ShowHelp();
            Environment.Exit(1);
        }
        if (!File.Exists(file))
        {
            AnsiConsole.MarkupLine("[#ff0000]The SQL script file doesn't not exists or accessible.[/]");
            ShowHelp();
            Environment.Exit(1);
        }

        SplitFile(file, destination ?? "", limit, addGo);
    }
}