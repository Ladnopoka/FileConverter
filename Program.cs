class Program
{
    public static void Main(string[] args)
    {
        string file, fileFormat;    // storing file name and storing file format
        bool inputBool = true;  // to see if it is the first file name

        Table table = new Table();  // creating an instance of a class table
        foreach (string arg in args)    // loop through each console argument
        {
            if (arg.Contains(".") && inputBool == true) // if we are dealing with a file, and it is an input file (comes first)
            {
                //Table table = new Table();   // calling default constructor
                table.setFileName(arg); // setting file name through table's setter
                file = table.getFileName(); // getting file name through table's getter
                fileFormat = file.Substring(file.IndexOf('.')); // extract file format

                if (File.Exists(@".\tabConvFiles\" + file)) // if the input file exists in our tabConvFiles folder
                {   //swith for which file format are we using
                    switch (fileFormat) 
                    {
                        case ".csv": table.csvParser(@".\tabConvFiles\" + file); break;
                        case ".json": table.jsonParser(@".\tabConvFiles\" + file); break;
                        case ".html": table.htmlParser(@".\tabConvFiles\" + file); break;
                        case ".md": table.mdParser(@".\tabConvFiles\" + file); break;
                        default: Console.WriteLine("There was an unexpected error."); break;
                    }
                }
                else    //if wrong file was writte, nothing will happen and you have to start again
                {
                    Console.WriteLine("The specified input file does not exist, please try again.");
                    break;
                }
                inputBool = false;  // we have dealt with our input file
            }
            else if (arg.Contains(".") && inputBool == false)   // if we are dealing with a file, and it is an output file (comes second)
            {
                //Table table2 = new Table(arg);  // calling parameterized constructor
                table.setFileName(arg);
                file = table.getFileName(); // extract file name from getter
                fileFormat = file.Substring(file.IndexOf('.')); // extract file format
                // switch for which file format are using
                switch (fileFormat)
                {
                    case ".csv": table.toCSV(table.getGenericTable()); break;
                    case ".json": table.toJSON(table.getGenericTable()); break;
                    case ".html": table.toHTML(table.getGenericTable()); break;
                    case ".md": table.toMD(table.getGenericTable()); break;
                    default: Console.WriteLine("There was an unexpected error. You must provide file extension format"); break;
                }
            }
            else    //if console command is not a file, perform any of the methods from the list
            {
                switch (arg)
                {
                    case "-v": verbose(); break;
                    case "-verbose": verbose(); break;
                    case "-o": output(); break;
                    case "-output": output(); break;
                    case "-l": listFormats(); break;
                    case "-list-formats": listFormats(); break;
                    case "-h": help(); break;
                    case "-help": help(); break;
                    case "-i": info(); break;
                    case "-info": info(); break;
                    default: Console.WriteLine("There was an unexpected error. Please try a different console input."); break;
                }
            }
        }

        //These methods will be used to perform functions when the appropriate console command is called
        static void verbose()
        {
            Console.WriteLine("Converting files...");
        }
        static void output()
        {
            Console.WriteLine("The output files can be found in .\\tabConvOutput folder");
        }
        static void listFormats()
        {
            Console.WriteLine("The available formats are: ");
            Console.WriteLine(".html");
            Console.WriteLine(".md");
            Console.WriteLine(".csv");
            Console.WriteLine(".json");
        }
        static void help()
        {
            Console.WriteLine("Options: ");
            Console.WriteLine("-v | -verbose                Verbose mode (debugging output to STDOUT)");
            Console.WriteLine("-o <file> | -output=<file>   Output file specified by <file>");
            Console.WriteLine("-l | -list-formats           List formats");
            Console.WriteLine("-h | -help                   Show usage message");
            Console.WriteLine("-i | -info                   Show version information");
        }
        static void info()
        {
            Console.WriteLine("Program version 1.0. Developped by Jevgenij Ivanov.");
        }
    }
}
//All code was tested on Windows 10 in Visual Studio Code version 1.72.0