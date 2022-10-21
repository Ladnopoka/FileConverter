using HtmlAgilityPack;
using System.Text.Json;

public class Table // Class declaration
{
    private string fileName;    //file name of the table class
    private List<List<string>> genericTable;    // the converted generic table

    public Table()  // default constructor that sets filename to default
    {
        fileName = "defaultFileName.text";
        genericTable = null;
    }
    public Table(string fileName)   // parameterized constructor that sets a file name that was passed into it
    {
        this.fileName = fileName;
    }

    public string getFileName() //getter for file name
    {
        return fileName;
    }
    public void setFileName(string fileName)    //setter for file name
    {
        this.fileName = fileName;
    }
    public List<List<string>> getGenericTable() //getter for generic table
    {
        return genericTable;
    }
    public void setGenericTable(List<List<string>> genericTable)    //setter for generic table
    {
        this.genericTable = genericTable;
    }


    public void csvParser(string csvPath) // method that parses csv format to a 2d list
    {
        List<string> listColumns = new List<string>();  // to store the headers
        List<string> listRows = new List<string>(); // to store the rows 
        List<List<string>> list = new List<List<string>>(); // to store data into matrix
        string s;   // to store lines

        using (StreamReader sr = File.OpenText(csvPath))    // read the file
        {
            s = sr.ReadLine();  // read line from file
            listColumns = s.Split(",").ToList();    //split headers with comma

            list.Add(listColumns);  // add headers to list

            while ((s = sr.ReadLine()) != null)    // while line is not empty
            {
                listRows = s.Split(",").ToList();   // content under headers
                list.Add(listRows); // add to matrix
            }

            for (int i = 0; i < list.Count; i++)    //iterate through all the elements and remove quotations marks
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    if(list[i][j][0] == '"')
                        list[i][j] = list[i][j].Substring(1, list[i][j].Length-2);
                }
            }
            genericTable = list;
        } 
    }

    public void htmlParser(string htmlPath)   // method that parses html format to a 2d list
    {
        HtmlDocument doc = new HtmlDocument(); // Declare an object 'doc' of class HtmlDocument
        //Create a HTML <table> as a Verbatim multiline string literal
        //using doc.Load, specifying file location and name
        doc.Load(htmlPath);
        List<string> listColumns = new List<string>();  // to store the headers
        List<List<string>> list = new List<List<string>>(); // to store data into matrix
        bool headerDone = false;    // checker for the headers

        //A process with nested foreach statements that parse 
        //the rows and cells as I iterate over the nodes
        foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table")) //for each <table> tags
        {
            foreach (HtmlNode row in table.SelectNodes("tr"))   //for each <tr> tags
            {
                if (!headerDone)    // if we didn't go through the headers
                {
                    foreach (HtmlNode cell in row.SelectNodes("th")) //for each <th> tags
                    {
                        listColumns.Add(cell.InnerText);   //inner text are the details that are in there
                    }
                    headerDone = true;  //headers are now complete checker
                    list.Add(listColumns.ToList()); // add headers to a matrix
                }
                else
                {
                    listColumns.Clear();    // clear our headers list to use for other lines
                    foreach (HtmlNode cell in row.SelectNodes("td")) //for each <td> tags
                    {
                        listColumns.Add(cell.InnerText);   //add inner text to our list 
                    }
                    list.Add(listColumns.ToList()); // add list to matrix
                }
            }
        }
        genericTable = list;    // return the matrix
    }

    public void jsonParser(string jsonPath)
    {
        List<List<string>> list = new List<List<string>>(); // to store data into matrix
        List<string> listColumns = new List<string>();  // to store the headers
        List<string> listRows = new List<string>(); // to store the rows 
        string json;    // to hold the json format

        using (StreamReader r = new StreamReader(jsonPath)) //read from file 
        {
            while ((json = r.ReadLine()) != null)   // store each line into json until nothing is left
            {
                if (json.Contains(":")) // our delimiter is ":"
                {
                    int index = json.IndexOf(':');  // get index of delimeter

                    listColumns.Add(json.Substring(7, index-8));    // get 
                }

                if (json.Contains("}")) // if we are at the end of the section
                {
                    if (!list.Any())    // if my list is empty
                    {
                        list.Add(listColumns.ToList());     // add headers and we can then exit the loob with break
                        break;
                    }
                }
            }
        }

        using (StreamReader r = new StreamReader(jsonPath))
        {
            json = r.ReadToEnd();   // get the whole json file into a string format 
        }

        using JsonDocument docJSON = JsonDocument.Parse(json);  //get json document using System.Text.Json;
        JsonElement rootJSON = docJSON.RootElement;     // get root 

        for (int i = 0; i < rootJSON.GetArrayLength(); i++)    // go through elements of json root
        {
            for (int j = 0; j < listColumns.Count; j++)     // go through the number of headers we collected at the start
            {
                listRows.Add("" + rootJSON[i].GetProperty(listColumns[j])); // add to rows list by extracing property using json doc root
            }
            list.Add(listRows.ToList());    // add our rows to final matrix
            listRows.Clear();   // clear rows to start again in the loop fresh
        }

        genericTable = list;    // return the matrix
    }

    public void mdParser(string mdPath)
    {
        List<List<string>> list = new List<List<string>>(); // to store data into matrix
        List<string> listColumns = new List<string>();  // to store the headers
        List<string> listRows = new List<string>(); // to store the rows 
        string md, word = "";

        bool flag = false;  // keep track where we are in our iterator

        using (StreamReader r = new StreamReader(mdPath))   // read from path
        {
            while ((md = r.ReadLine()) != null) // read line by line until it's empty
            {
                if (!flag)  // if we are on the first line
                {
                    for (int i = 1; i < md.Length; i++) // loop through headers
                    {
                        if (md[i] is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_' or '-')    // if character belongs to alphabet or other acceptable symbols
                        {
                            word = word + md[i];    // extract our header
                        }
                        else
                        {
                            if (md[i] != ' ')   // if our character is not space
                            {
                                listColumns.Add(word);  // we add our header that we previously extracted on line 189
                                word = ""; // reset the header to start fresh next time around
                            }
                        }
                    }
                    flag = true;    // we won't be going to headers again
                    list.Add(listColumns.ToList()); // add to our final matrix
                }
                else    // if we are not on the first line anymore
                {
                    if (md[1] != '-')   // and our line does not consist of dashes only 
                    {
                        for (int i = 1; i < md.Length; i++) // loop through the line
                        {
                            if (md[i] is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_' or '-' or >= '0' and <= '9')   // if character belongs to alphabet or numbers ro dashes
                            {
                                word = word + md[i];    // extract our word
                                if (md[i+1] == ' ' && md[i+2] != ' ')   // we need to add a space when extracting word to avoid concatenation
                                    word += " ";
                            }
                            else
                            {
                                if (md[i] != ' ')   // if we are not dealing with space
                                {
                                    listRows.Add(word);     // add our extracted word to rows list
                                    word = ""; // reset the word
                                }
                            }
                        }
                        list.Add(listRows.ToList());    // add to our matrix
                        listRows.Clear();   // reset rows list to start again fresh
                    }
                }
            }
        }
        genericTable = list; // return our matrix
    }

    public void toCSV(List<List<string>> list)
    {
        bool flag = false; // to check if we are on headers
        string csv = ""; // append data to string
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Count; j++) // dealing with rows now
            {
                if (!flag)  // id we are dealing with headers
                {
                    csv = csv + '"' + list[i][j] + '"';
                    flag = true;    // won't bea dealing with headers anymore
                }
                else
                {
                    if (Char.IsDigit(list[i][j][0]))    //if we are dealing with years
                    {
                        csv = csv + list[i][j]; // append year to our csv string
                    }
                    else
                    {
                        csv = csv + '"' + list[i][j] + '"';     // if its not a year we add quotations marks
                    }
                }

                if (j != list[i].Count-1)   // if we are not on the last line
                    csv = csv + ",";    // then add a coma
            }
            csv = csv + "\n";   //jump to next line
        }

        using (StreamWriter sw = File.CreateText(@".\tabConvOutput\" + getFileName())) // write to file, place the file into tabConvOutput folder
        {
            sw.Write(csv);
        }
    }

    public void toHTML(List<List<string>> list)
    {
        bool flag = false; // to check if we are on headers
        string html = "<table>"; // append data to string
        for (int i = 0; i < list.Count; i++)    //iterate through matrix
        {
            if (!flag)  // if these are headers
            {
                html = html + "\n\t<tr>\n\t\t";
                for (int j = 0; j < list[i].Count; j++) // go thourhg all headers and append them to html string
                {
                    if (!flag)
                    {
                        html = html + "<th>" + list[i][j] + "</th>";
                    }
                    if (j != list[i].Count-1)   // don't do linebreak when its last row
                        html = html + "\n\t\t";
                }
                html = html + "\n\t</tr>";  //closing tag
            }
            else    // if we are not dealing with headers
            {
                html = html + "\n\t<tr>\n\t\t";
                for (int j = 0; j < list[i].Count; j++) //iterate through list
                {
                    if (Char.IsDigit(list[i][j][0]) && list[i][j].Length == 4)  // if we are dealing with years
                        html = html + "<td align=\"\"right\"\">" + list[i][j] + "</td>";
                    else
                        html = html + "<td>" + list[i][j] + "</td>";

                    if (j != list[i].Count-1)   //don't do linebreak when its last row
                        html = html + "\n\t\t";
                }
                html = html + "\n\t</tr>";
            }
            flag = true;    // we won't be dealing with headers anymore
        }
        html = html + "\n</table>";
        using (StreamWriter sw = File.CreateText(@".\tabConvOutput\" + getFileName())) // write to file, place the file into tabConvOutput folder
        {
            sw.Write(html);
        }
    }

    public void toJSON(List<List<string>> list)  //convert from generic table to JSON method
    {
        string json = "[\n\t{\n\t\t";   // append the beginning of JSON format
        string [] headers = new string [list[0].Count]; // array for only headers
        for (int i = 0; i < headers.Length; i++)
        {
            headers[i] = list[0][i];    //store the headers
        }

        for (int i = 1; i < list[0].Count+1; i++)   //iterate through number of sections
        {
            int flag = 0;   // to keep track of where I'm at in the iteration
            for (int j = 0; j < headers.Length; j++)    // iterate through number of headers
            {
                if(Char.IsDigit(list[i][j][0])) // if we are dealing with numbers
                    json += '"' + headers[j] + "\": " + list[i][j]; // append json format with header name and its data without quotations marks  
                else
                {
                    json += '"' + headers[j] + "\": " + '"' + list[i][j] + '"'; // append json format with header name and its data with quotations marks  
                }
                if (flag < headers.Length-1)    // add comma unless its the last line of section
                    json += ",";

                flag++; //to keep track until I'm on the last line of section

                if (j < headers.Length-1)   // end of line, jump to next line with some tabs
                    json += "\n\t\t";
            }
            json += "\n\t}";    // append end of section

            if (i < list[0].Count)  // start of a new section
                json += ",\n\t{\n\t\t";
        }

        json += "\n]";  // end of json file
        using (StreamWriter sw = File.CreateText(@".\tabConvOutput\" + getFileName())) // write to file, place the file into tabConvOutput folder
        {
            sw.Write(json);
        }
    }

    public void toMD(List<List<string>> list)
    {   
        string md = ""; //will be used to append string to final output
        int [] dashArray = new int [list[0].Count]; //need this array to store number of dashes for 2nd line (under headers)
        int flag = 0; // to check where we are in the iteration
        for (int i = 0; i < list[0].Count; i++) // a nested for loop to count how many dashes do we need for 2nd line
        {
            for (int j = 0; j < list[i].Count; j++)
            {
                if (dashArray[i] < list[j][i].Length)
                    dashArray[i] = list[j][i].Length;
            }
        }

        for (int i = 0; i < list.Count; i++)    // main loop to go through all elements
        {
            if (flag == 1)  // to check if we are on the second line
            {
                for (int j = 0; j < dashArray.Length; j++)  //append required number of dashes to our md string
                {
                    md = md + "|";  // start of appending
                    for (int k = 0; k < dashArray[j]; k++)
                        md = md + "-";

                    if (j == dashArray.Length-1)
                        md = md + "|";  // the end of appending
                }
                i--;    // move back once to ignore dashed line and keep appending the right lines
            }
            else    // if we are not on the 2nd line
            {
                for (int j = 0; j < list[i].Count; j++) // loop that goes through columns and appends to md
                {
                    md = md + "|" + list[i][j];     // start of appending per line
                    if (flag != 1)  // if we are on the second line
                    {
                        for (int k = list[i][j].Length; k < dashArray[j]; k++)    //pad the word with spaces
                            md = md + " ";
                    }

                    if (j == list[i].Count-1)
                        md = md + "|";  // end of appending per line
                }
            }
            if (i < list.Count-1)   // if we are not on the very last line of appending 
                md = md + "\n";
            flag++;
        }
        using (StreamWriter sw = File.CreateText(@".\tabConvOutput\" + getFileName())) // write to file, place the file into tabConvOutput folder
        {
            sw.Write(md);
        }
    }
}
//All code was tested on Windows 10 in Visual Studio Code version 1.72.0