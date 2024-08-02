# Split SQL script
Split an SQL script file into smaller chinks based on the specified maximum byte size.

## Usage

>   SplitSQLScript [options]

## Options

| Option                      | Description                                                                          | Default |
|-----------------------------|--------------------------------------------------------------------------------------|---------|
| file                        | The SQL script file to split.                                                        |         |
| destination                 | The destination folder. If this is empty or null, the new files will be created in the same directory as the original file. | |
| limit                       | The maximum bytes limit for the new files. [default: 10240000]                       | 10240000 |
| add-go                      | If set to true the procedure will add the command GO at the end of each file.        | True     |
| version                     | Show version information                                                             |          |
| ?, h, help                  | Show help and usage information                                                      |          |