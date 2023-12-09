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

Results in lots of small queries to the database for updates

Elapsed time in ms: 962

## SyncFileToDbEFSimple

Experiment using Entity Framework (EF) using a simple drop and write full list (this is how the original app worked). 
The hope was that EF would be smart enough to just make the net changes, That assumption proved to be false.

Results in one query to the database for updates

Elapsed time in ms: 3635
