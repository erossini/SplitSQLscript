# Split SQL script
Split an SQL script file into smaller chinks based on the specified maximum byte size.

## Usage

>   SplitSQLScript [options]

## Options
  --file <file>                The SQL script file to split.
  --destination <destination>  The destination folder. If this is empty or null,
                                           the new files will be created in the same directory as the original file.
  --limit <limit>              The maximum bytes limit for the new files. [default: 10240000]
  --add-go                     if set to true the procedure will add the command GO
                                           at the end of each file. [default: True]
  --version                    Show version information
  -?, -h, --help               Show help and usage information