import duckdb

con = duckdb.connect(':memory:')

c = con.cursor()

# create a table
c.execute("CREATE TABLE items(item VARCHAR, value DECIMAL(10,2), count INTEGER)")

# insert an item into the table
c.execute("INSERT INTO items VALUES ('jeans', 20.0, 1)")

# fetch as pandas data frame
df = c.execute("SELECT * FROM items").fetchdf()
print(df)
#    item  value  count
# 0  jeans   20.0      1

# fetch as dictionary of numpy arrays
arr = c.execute("SELECT * FROM items").fetchnumpy()
print(arr)
# {'item': masked_array(data=['jeans'], mask=False, fill_value='?', dtype=object),
#  'value': masked_array(data=[20.], mask=False, fill_value=1e+20),
#  'count': masked_array(data=[1], mask=False, fill_value=999999, dtype=int32)}
