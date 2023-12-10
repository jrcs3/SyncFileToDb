
DELETE dbo.EmployeeIdAndSsn WHERE EmployeeId IN (1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987)


INSERT INTO dbo.EmployeeIdAndSsn(EmployeeId,ssn) 
VALUES 
('1111','111111111'),
('2222','222222222'),
('3333','333333333'),
('4444','444444444'),
('5555','555555555'),
('6666','666666666'),
('7777','777777777'),
('8888','888888888'),
('9999','999999999')


UPDATE dbo.EmployeeIdAndSsn SET SSN = '123456' + EmployeeId WHERE EmployeeId IN ('111','222','333')