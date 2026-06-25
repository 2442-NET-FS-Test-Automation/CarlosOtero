-- Parking Lot*******
-- *                *
-- *                *
--- *****************



-- Comment can be done single line with --
-- Comment can be done multi line with /* */

/*
DQL - Data Query Language
Keywords:

SELECT - retrieve data, select the columns from the resulting set
FROM - the table(s) to retrieve data from
WHERE - a conditional filter of the data
GROUP BY - group the data based on one or more columns
HAVING - a conditional filter of the grouped data
ORDER BY - sort the data
*/


-- BASIC CHALLENGES
-- List all customers (full name, customer id, and country) who are not in the USA

SELECT CONCAT(FirstName,' ', LastName) as FullName,CustomerId,Country FROM Customer WHERE [State] != 'USA';

-- List all customers from Brazil
SELECT * FROM Customer WHERE Country = 'Brazil';

-- List all sales agents

SELECT * FROM employee WHERE title LIKE '%Agent%';

-- SELECT * FROM employee WHERE title LIKE '%Agent%;


-- Retrieve a list of all countries in billing addresses on invoices

SELECT BillingCountry From Invoice GROUP BY BillingCountry;

-- Retrieve how many invoices there were in 2009, and what was the sales total for that year?
--====
Select COUNT(*) as InvoiceAmount, SUM(Total) as Total from Invoice where Year(InvoiceDate) = 2021;

-- (challenge: find the invoice count sales total for every year using one query)

Select COUNT(*) as InvoiceAmount, SUM(Total) as Total, YEAR(InvoiceDate) as Year from Invoice GROUP BY YEAR(InvoiceDate);
-- how many line items were there for invoice #37

SELECT i.InvoiceId, count (*) as TotalInvoices from Invoice AS i    
JOIN InvoiceLine AS l ON l.InvoiceId = i.InvoiceId where l.InvoiceId = 37 GROUP BY i.InvoiceId;

-- how many invoices per country? BillingCountry  # of invoices 

Select COUNT(*) InvoiceAmount, BillingCountry from Invoice GROUP BY BillingCountry ;

-- Retrieve the total sales per country, ordered by the highest total sales first.

SELECT BillingCountry as Country,SUM(Total) as TotalSale From Invoice GROUP BY BillingCountry ORDER BY TotalSale ASC;

-- JOINS CHALLENGES
-- Every Album by Artist
SELECT t.Composer, a.Title from Track AS t    
JOIN Album AS a ON a.AlbumId = t.AlbumId GROUP BY t.Composer, a.Title;

-- (inner keyword is optional for inner join)

-- All songs of the rock genre

SELECT t.Composer, t.Name from Track t  
JOIN Genre AS a ON a.GenreId = t.GenreId
where a.Name = 'Rock' 

-- Show all invoices of customers from brazil (mailing address not billing)
Select * from Invoice;
SELECT * from Customer;
SELECT COUNT(*) as Invoice, CONCAT(c.FirstName,' ', c.LastName) as FullName from Customer as c
JOIN Invoice AS i ON i.CustomerId = c.CustomerId where c.Country = 'Brazil' GROUP BY CONCAT(c.FirstName,' ', c.LastName);

-- Show all invoices together with the name of the sales agent for each one
SELECT i.InvoiceId,CONCAT(e.FirstName,' ', e.LastName) as FullName from Invoice as i
JOIN Customer AS c ON c.CustomerId = i.CustomerId 
JOIN Employee as e  on e.EmployeeId = c.SupportRepId
GROUP BY i.InvoiceId, CONCAT(e.FirstName,' ', e.LastName);

-- Which sales agent made the most sales in 2009?

SELECT TOP 1 CONCAT(e.FirstName,' ', e.LastName) as FullName from Invoice as i
JOIN Customer AS c ON c.CustomerId = i.CustomerId 
JOIN Employee as e  on e.EmployeeId = c.SupportRepId
WHERE YEAR(i.InvoiceDate) = 2021 
GROUP BY CONCAT(e.FirstName,' ', e.LastName);
-- How many customers are assigned to each sales agent?

SELECT Count(*) as CustomerAmount, CONCAT(e.FirstName,' ', e.LastName) as FullName from Customer as c
JOIN Employee as e  on e.EmployeeId = c.SupportRepId
GROUP BY CONCAT(e.FirstName,' ', e.LastName);

-- Which track was purchased the most in 2010?
SELECT top 1 t.Name from InvoiceLine as il
JOIN Track as t  on t.TrackId= il.TrackId
GROUP BY t.Name, il.TrackId
order by count(*) desc;

-- Show the top three best selling artists.
SELECT top 3 a.Name from InvoiceLine as il
JOIN Track as t  on t.TrackId= il.TrackId
JOIN Album as b  on b.AlbumId= t.AlbumId
JOIN Artist as a  on a.ArtistId= b.ArtistId
GROUP BY a.Name
order by count(*) desc;

-- Which customers have the same initials as at least one other customer?


-- Which countries have the most invoices?
select top 3 BillingCountry from Invoice
GROUP BY BillingCountry
ORDER BY COUNT(*) desc;

-- Which city has the customer with the highest sales total?

SELECT TOP 1 i.BillingCity, CONCAT(c.FirstName,' ', c.LastName), COUNT(*) as Frequency from Invoice as i
JOIN Customer AS c ON c.CustomerId = i.CustomerId 
GROUP BY i.BillingCity, CONCAT(c.FirstName,' ', c.LastName)
ORDER BY COUNT(*)DESC;

-- Who is the highest spending customer?
SELECT top 1 CONCAT(c.FirstName,' ', c.LastName), Sum(i.Total) as TotalSpent from Invoice as i
JOIN Customer AS c ON c.CustomerId = i.CustomerId
GROUP BY CONCAT(c.FirstName,' ', c.LastName)
ORDER BY COUNT(CONCAT(c.FirstName,' ', c.LastName)) DESC, TotalSpent DESC;
-- Return the email and full name of of all customers who listen to Rock.

SELECT c.Email, CONCAT(c.FirstName,' ', c.LastName) as FullName from Customer as c
JOIN Invoice AS i ON i.CustomerId = c.CustomerId
JOIN InvoiceLine AS il ON il.InvoiceId = i.InvoiceId
JOIN Track AS t ON t.TrackId = il.TrackId
JOIN Genre AS g ON g.GenreId = t.GenreId
WHERE g.Name = 'Rock'
GROUP BY CONCAT(c.FirstName,' ', c.LastName), Email;

-- Which artist has written the most Rock songs?

SELECT top 1 a.Name from Track as t
JOIN Album AS m ON m.AlbumId = t.AlbumId
JOIN Artist AS a ON a.ArtistId = m.AlbumId
JOIN Genre AS g ON g.GenreId = t.GenreId
WHERE g.Name = 'Rock'
GROUP BY a.Name
ORDER BY COUNT(*) desc;

-- Which artist has generated the most revenue?

SELECT top 1 a.Name, sum(il.UnitPrice) as TotalRevenue from InvoiceLine as il
JOIN Track AS t ON t.TrackId = il.TrackId
JOIN Album AS m ON m.AlbumId = t.AlbumId
JOIN Artist AS a ON a.ArtistId = m.AlbumId
JOIN Genre AS g ON g.GenreId = t.GenreId
GROUP BY a.Name, il.UnitPrice
ORDER BY count(*)desc;


-- ADVANCED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?


-- 2. which artists did not record any tracks of the Latin genre?


-- 3. which video track has the longest length? (use media type table)



-- 4. boss employee (the one who reports to nobody)


-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?



-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.




-- DML exercises

-- 1. insert two new records into the employee table.

-- 2. insert two new records into the tracks table.

-- 3. update customer Aaron Mitchell's name to Robert Walter

-- 4. delete one of the employees you inserted.

-- 5. delete customer Robert Walter.
