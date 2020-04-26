import duckdb

con = duckdb.connect(':memory:')

c = con.cursor()

# create a table
c.execute("CREATE TABLE items(item VARCHAR, value DECIMAL(10,2), count INTEGER)")

# insert an item into the table
c.execute("INSERT INTO items VALUES ('jeans', 20.0, 1)")

# retrieve the item again
c.execute("SELECT * FROM items")
print(c.fetchall())
