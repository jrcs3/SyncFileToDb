# SyncFileToDb

Here I'm syncing the values in the CSV file into the database. First I read in 2 lists:
- *employeesDb* - from the db
- *employeesCsv* - from the csv
 
employeesCsv the truth. After the process is run, it should alter the contents of the db to match employeesCsv

When I compare the 2 lists, the items are split into 4 lists:
- *toSkipQuery* - EmployeeId and Ssn match - No update required (saves DB activity)
- *toChangeSsnQuery* - EmployeeId match but Ssn don't - Update the Ssn in the db
- *toAddQuery* - EmployeeId not in db list - Add it to db
- *toDeleteQuery* - EmployeeID not in csv list - remove it from db
 
 Checks:
 - *toSkipQuery.Count* + *toChangeSsnQuery.Count* + *toAddQuery.Count* == *employeesCsv.Count* - if not fail the process
	 
## SyncFileToDbAdoNet

Break the changes into the 4 groups and make the changes with ADO.NET one at a time.

Results in lots of small queries to the database for updates. (For the At Work problem that inspired this investigation, we
were wondering if the numbers of interactions with SQL Server may be a problem.)

Elapsed time in ms: 962
Elapsed time in ms second run: 569

## SyncFileToDbEFSimple

Experiment using Entity Framework (EF) using a simple drop and write full list (this is how the original app worked). 

Results in one query to the database for updates. 

The hope was that EF would be smart enough to just make the net changes, That assumption proved to be false. I kept 
this one around to document the process.

Elapsed time in ms: 3635
Elapsed time in ms second run: 4568

## SyncFileToDbEFManuallyFilter

Experiment using Entity Framework (EF) breaking the changes into the same 4 groups and making the changes with EF and 
calling SaveChanges()

Results in one query to the database for updates, I did have to do some tweeking to avoid additional queries when 
issuing the EF command earlier in the process.

The perf was slower than SyncFileToDbAdoNet for this small sample size, but since there are fewer SQL exchanges, it may
perform better for larger batch sizes.

Elapsed time in ms second run: 3490

## SyncFileToDbBatchAdoNet

Drop table an insert the whole CSV with ADO.NET in batches. Build a large INSERT INTO query with paramaters.
The number of records inserted can be configured. 

I did this experiment because the origional application did a truncate and bulk import, but did it as single record inserts.
The hope is that this method would perform better because it made fewer DB calls.

Elapsed time in ms, batch size 1000: 639
Elapsed time in ms, batch size 100: 663
Elapsed time in ms, batch size 10: 873
Elapsed time in ms, batch size 1: 1001

## Experiments

### First Experiment
In the [First_Experiment](https://github.com/jrcs3/SyncFileToDb/releases/tag/First_Experiment), I wrote 
[SyncFileToDb](https://github.com/jrcs3/SyncFileToDb/tree/First_Experiment/SyncFileToDb), a simple C# console app 
that broke up the lists into the 4 groups and make the changes with ADO.NET one at a time. 

### Second Experiment
In the [Second_Experiment](https://github.com/jrcs3/SyncFileToDb/releases/tag/Second_Experiment), I renamed *SyncFileToDb* to 
[SyncFileToDbAdoNet](https://github.com/jrcs3/SyncFileToDb/tree/Second_Experiment/SyncFileToDbAdoNet), added a second console app 
[SyncFileToDbEFSimple](https://github.com/jrcs3/SyncFileToDb/tree/Second_Experiment/SyncFileToDbEFSimple), moved the common
CSV helper code to the DLL [CsvCommonLib](https://github.com/jrcs3/SyncFileToDb/tree/Second_Experiment/CsvCommonLib), and moved the
sample data down one level to [data](https://github.com/jrcs3/SyncFileToDb/tree/Second_Experiment/data) so that it would be accessable 
to both projects. This experiment was a flop.

### Third Experiment
In the [Third_Experiment](https://github.com/jrcs3/SyncFileToDb/releases/tag/Third_Experiment), added a third console app
[SyncFileToDbEFManuallyFilter](https://github.com/jrcs3/SyncFileToDb/tree/Third_Experiment/SyncFileToDbEFManuallyFilter), moved the common
Entity Framework (EF) code to the DLL [EFCommonLib](https://github.com/jrcs3/SyncFileToDb/tree/Third_Experiment/EFCommonLib). Under the 
test conditions, *SyncFileToDbEFManuallyFilter* did not perform as well as *SyncFileToDbAdoNet*, but will do some more experimentation with
the actual data during business hours.

### Fourth Experiment
In the [Fourth_Experiment](https://github.com/jrcs3/SyncFileToDb/releases/tag/Fourth_Experiment), added a fourth console app
[SyncFileToDbBatchAdoNet](https://github.com/jrcs3/SyncFileToDb/tree/Fourth_Experiment/SyncFileToDbBatchAdoNet) which does a drop and
replace; the replace is done with massive INSERT INTO queries to reduce the number of interactions with the SQL Server.

## Other Files

- [EmployeeIdAndSsn.csv](https://github.com/jrcs3/SyncFileToDb/blob/master/data/EmployeeIdAndSsn.csv) - the CSV file I'm syncing with the database.
- [EmployeeIdAndSsn.sql](https://github.com/jrcs3/SyncFileToDb/blob/master/data/EmployeeIdAndSsn.sql) - query to make the test table.
- [messUpDb.sql](https://github.com/jrcs3/SyncFileToDb/blob/master/data/messUpDb.sql) - script to mess up the table between runs.