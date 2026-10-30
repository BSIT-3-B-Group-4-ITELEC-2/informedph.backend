CREATE DATABASE IF NOT EXISTS informph_db;
USE informph_db;

-- Drop old tables and triggers if resetting
DROP TRIGGER IF EXISTS trg_subscriber_insert;
DROP TRIGGER IF EXISTS trg_subscriber_update;
DROP TRIGGER IF EXISTS trg_subscriber_delete;
DROP TABLE IF EXISTS EventLogs;
DROP TABLE IF EXISTS Subscribers;

CREATE TABLE Subscribers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Gender VARCHAR(50) NOT NULL,
    Birthday VARCHAR(50) NOT NULL,
    Age INT NOT NULL,
    Status VARCHAR(50) DEFAULT 'active',
    SubscribedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE EventLogs (
    eventId INT AUTO_INCREMENT PRIMARY KEY,
    event_logs TEXT NOT NULL,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
);

DELIMITER //

-- TRIGGER 1: Fires when a new user signs up (INSERT)
CREATE TRIGGER trg_subscriber_insert
AFTER INSERT ON Subscribers
FOR EACH ROW
BEGIN
    INSERT INTO EventLogs (event_logs) 
    VALUES (CONCAT('User ', NEW.FullName, ' has been added.'));
END //

-- TRIGGER 2: Fires when a user updates their info or unsubscribes (UPDATE)
CREATE TRIGGER trg_subscriber_update
AFTER UPDATE ON Subscribers
FOR EACH ROW
BEGIN
    IF NEW.Status = 'inactive' AND OLD.Status = 'active' THEN
        INSERT INTO EventLogs (event_logs) 
        VALUES (CONCAT('User ', NEW.FullName, ' has unsubscribed.'));
    ELSE
        INSERT INTO EventLogs (event_logs) 
        VALUES (CONCAT('User ', NEW.FullName, ' updated their info.'));
    END IF;
END //

-- TRIGGER 3: Fires when a user is permanently deleted (DELETE)
CREATE TRIGGER trg_subscriber_delete
AFTER DELETE ON Subscribers
FOR EACH ROW
BEGIN
    INSERT INTO EventLogs (event_logs) 
    VALUES (CONCAT('User ', OLD.FullName, ' has been deleted.'));
END //

-- Reset delimiter back to default
DELIMITER ;

show triggers;
show tables;
select *from Subscribers;
select *from EventLogs;
