delete from tripdetails where id in (select td.id from trips t inner join TripDetails td on t.id = td.TripId where t.Qualification = 0)
delete from trips where Qualification = 0