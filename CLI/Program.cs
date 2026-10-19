using System.CommandLine;
using System.Threading.Channels;
Console.WriteLine("PROGRAM STARTED");
Console.WriteLine(string.Join(" ", args));
Console.ReadLine();
var bundleCommand = new Command("bundle", "bundle code files to a single file");
var rootCommand = new RootCommand("Root command for Bundler CLI");
var create_rsp = new Command("create-rsp");

Option<FileInfo> output = new("--output", "-o"){
    Description = "the name of the bundle path",
    Required = true,
};
Option<string[]> languages = new("--language", "-l"){
    Description = "the languages",
    Required = true,
    AllowMultipleArgumentsPerToken = true,

};
Option<bool> note = new("--note", "-n"){
    Description = "A note with the source of the code",
};
Option<bool> remove_empty_lines = new("--remove", "-r"){
    Description = "option to remove empty lines",
};
Option<string> author = new("--author", "-a"){
    Description = "write the author in the head of file.",
};
bundleCommand.Add(output);
bundleCommand.Add(languages);
bundleCommand.Add(note);
bundleCommand.Add(remove_empty_lines);
bundleCommand.Add(author);
create_rsp.SetAction(_ =>
{
    Console.WriteLine("s");
    string currentDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
    #region output

    string outputPath = string.Empty;
    string directoryPath = string.Empty;
    string fileName = string.Empty;
    while (true)
    {
        Console.WriteLine("Enter the path for the output file:");
        outputPath = Console.ReadLine()?.Trim() ?? string.Empty;
        outputPath = outputPath.Trim('\"', '\'');
        if (string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine("Error: path can`t be empty");
            continue;
        }
        try
        {
            // בדיקה האם התיקייה שבה אמור להישמר הקובץ תקינה/קיימת
            string? dirName = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            break;
        }
        catch (Exception)
        {
            Console.WriteLine("Error: path not valid.");
        }
    }
    #endregion
    #region languges
    string[] selectedLanguages;

    while (true)
    {
        Console.WriteLine("Enter the langs. to all enter 'all' ");
        string input = Console.ReadLine()?.Trim() ?? string.Empty;

        // בדיקה שהקלט אינו ריק
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Error:empty line");
            continue;
        }

        // פירוק המחרוזת למערך של שפות לפי רווחים
        selectedLanguages = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // בדיקת תקינות נוספת במידת הצורך
        if (selectedLanguages.Length > 0)
        {
            break; // הקלט תקין, יוצאים מהלולאה
        }
    }
    #endregion
    #region note
    bool note1;
    Console.WriteLine("Do you want to write the source in a note");
    note1 = Console.ReadLine().ToLower() == "y" ? true : false;
    #endregion
    #region sort
    bool sort;
    Console.WriteLine("Do you want to sort by nameFile (y)");
    sort = Console.ReadLine().ToLower() == "y" ? true : false;
    #endregion
    #region remove
    bool remove;
    Console.WriteLine("Do you want to remove empty lines");
    remove = Console.ReadLine().ToLower() == "y" ? true : false;
    #endregion
    #region author
    Console.WriteLine("Enter name of author or enter");
    string author = Console.ReadLine()?.Trim() ?? string.Empty;
    #endregion
    #region כתיבה לקובץ rsp
    string rspFilePath = Path.Combine(currentDirectoryPath, "bundle-config.rsp");
    var tokens = new List<string> { "bundle" };
    tokens.Add("--output");
    tokens.Add(outputPath);
    if (note1)
    {
        tokens.Add("--note");
    }
    if (remove)
    {
        tokens.Add("--remove");
    }
    if (!string.IsNullOrEmpty(author))
    {
        tokens.Add("--author");
        tokens.Add(author);
    }
    tokens.Add("--language");
    foreach (var item in selectedLanguages)
    {
        tokens.Add(item);
    }
    try
    {
        using (StreamWriter writer = new StreamWriter(rspFilePath))
        {
            writer.WriteLine("bundle");
            writer.WriteLine("--output");
            writer.WriteLine(outputPath);
            Console.WriteLine("k");
            writer.WriteLine("--language");
            foreach (var item in selectedLanguages)
            {
                writer.WriteLine(item);
                Console.WriteLine("gfff");
            }
            if (note1)   
            {
                writer.WriteLine("--note");
            }
            if (remove)
            {
                writer.WriteLine("--remove");
            }
            if (!string.IsNullOrEmpty(author))
            {
                writer.WriteLine("--author");
                writer.WriteLine(author);
            }
        }
        Console.WriteLine("succeded to write rsp...");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR:{ex.Message}");
    }

    #endregion
});
rootCommand.Add(create_rsp);
rootCommand.Add(bundleCommand);

Console.WriteLine("Before invoke");

rootCommand.Parse(args).Invoke();

Console.WriteLine("After invoke");