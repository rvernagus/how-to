import duckdb

cursor = duckdb.connect(':memory:').cursor()
print(cursor.execute('SELECT 42').fetchall())
