CREATE TABLE analysis (
	ticker TEXT,
	created TIMESTAMP,
	lastprice DECIMAL,
	lastbookvalue DECIMAL,
	lastpevalue DECIMAL,
	industry TEXT,
	PRIMARY KEY(ticker)
);
ALTER TABLE analysis OWNER TO stocks;

CREATE TABLE metrics (
	ticker TEXT,
	created TIMESTAMP,
	json TEXT,
	PRIMARY KEY(ticker)
);
ALTER TABLE metrics OWNER TO stocks;

CREATE TABLE companies (
	ticker TEXT,
	created TIMESTAMP,
	name TEXT,
	industry TEXT,
	sector TEXT,
	json TEXT,
	PRIMARY KEY(ticker)
);
ALTER TABLE companies OWNER TO stocks;

CREATE TABLE prices (
	ticker TEXT,
	created TIMESTAMP,
	lastclose DECIMAL,
	json TEXT,
	PRIMARY KEY(ticker)
);
ALTER TABLE prices OWNER TO stocks;


CREATE TABLE ownedstocks (
	ticker TEXT,
	userid TEXT,
	created TIMESTAMP,
	version INT,
	eventjson TEXT,
	PRIMARY KEY(ticker, userid, version)
);
ALTER TABLE ownedstocks OWNER TO stocks;