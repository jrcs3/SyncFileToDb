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