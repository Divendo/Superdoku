using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Superdoku
{
    class ResultsExporter
    {
        // NOTE: for simplicity's sake this class relies on the fact that each row has exactly the same columns

        /// <summary>The file to export the results to.</summary>
        private string filename;

        /// <summary>The values we are going to export. Stored in the following formates: values[row][column] = value</summary>
        private SortedDictionary<string, SortedDictionary<string, long>> values;

        /// <summary>The extra values that should be appended to the end of the file.</summary>
        private SortedDictionary<string, long> extraValues;

        /// <summary>Constructor.</summary>
        /// <param name="filename">The file to export the results to.</param>
        public ResultsExporter(string filename)
        {
            this.filename = filename;
            values = new SortedDictionary<string, SortedDictionary<string, long>>();
            extraValues = new SortedDictionary<string, long>();
        }

        /// <summary>Property to get the filename.</summary>
        public string Filename
        { get { return filename; } }

        /// <summary>Adds a result to the given column and row.</summary>
        /// <param name="column">The name of the column where the result should be stored.</param>
        /// <param name="row">The name of the row where the result should be stored.</param>
        /// <param name="value">The value of the result.</param>
        public void addResult(string row, string column, long value)
        {
            if(!values.ContainsKey(row))
                values.Add(row, new SortedDictionary<string, long>());

            if(!values[row].ContainsKey(column))
                values[row].Add(column, value);
            else
                values[row][row] = value;
        }

        /// <summary>Adds an extra value, should be used for global values.</summary>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value.</param>
        public void addExtraValue(string name, long value)
        {
            if(!extraValues.ContainsKey(name))
                extraValues.Add(name, value);
            else
                extraValues[name] = value;
        }

        /// <summary>Writes everything to the file (in CSV format).</summary>
        public void write()
        {
            // Open the file
            StreamWriter file = new StreamWriter(filename);

            // Write the values
            bool firstRow = true;
            foreach(string row in values.Keys)
            {
                string line;

                if(firstRow)
                {
                    line = "";
                    foreach(string column in values[row].Keys)
                        line += ';' + column;                   // Yes, we start with a ';' because we want one empty field at the beginning
                    file.WriteLine(line);

                    firstRow = false;
                }
                
                line = row;
                foreach(string column in values[row].Keys)
                    line += ';' + values[row][column].ToString();
                file.WriteLine(line);
            }

            // Write the extra values
            if(extraValues.Count != 0)
            {
                file.WriteLine();                               // Skip a line
                foreach(KeyValuePair<string, long> extraValue in extraValues)
                    file.WriteLine(extraValue.Key + ';' + extraValue.Value.ToString());
            }

            // Don't forget to close the file
            file.Close();
        }
    }
}
