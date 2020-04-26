import urllib.request

urllib.request.urlretrieve("https://s3.amazonaws.com/nyc-tlc/trip+data/yellow_tripdata_2016-01.csv", "yellow_tripdata_2016-01.csv")

import duckdb
conn = duckdb.connect('ytd.duckdb')
cursor = conn.cursor()

cursor.execute("""
CREATE TABLE yellow_tripdata_2016_01 (   
    VendorID bigint,
    tpep_pickup_datetime timestamp,
    tpep_dropoff_datetime timestamp,
    passenger_count bigint,
    trip_distance double,
    pickup_longitude double,
    pickup_latitude double,
    RatecodeID bigint,
    store_and_fwd_flag varchar,
    dropoff_longitude double,
    dropoff_latitude double,
    payment_type bigint,
    fare_amount double,
    extra double,
    mta_tax double,
    tip_amount double,
    tolls_amount double,
    improvement_surcharge double,
    total_amount double
) 
""")

cursor.execute("""
COPY yellow_tripdata_2016_01 FROM './yellow_tripdata_2016-01.csv'
WITH HEADER
""")

cursor.execute(f"""
SELECT
    (SUM(trip_distance * fare_amount) - SUM(trip_distance) * SUM(fare_amount) / COUNT(*)) /
    (SUM(trip_distance * trip_distance) - SUM(trip_distance) * SUM(trip_distance) / COUNT(*)) AS beta,
    AVG(fare_amount) AS avg_fare_amount,
    AVG(trip_distance) AS avg_trip_distance
FROM 
    yellow_tripdata_2016_01,
    (
        SELECT 
            AVG(fare_amount) + 3 * STDDEV_SAMP(fare_amount) as max_fare,
            AVG(trip_distance) + 3 * STDDEV_SAMP(trip_distance) as max_distance
        FROM yellow_tripdata_2016_01
    ) AS sub
WHERE 
    fare_amount > 0 AND
    fare_amount < sub.max_fare AND 
    trip_distance > 0 AND
    trip_distance < sub.max_distance
""")
beta, avg_fare_amount, avg_trip_distance = cursor.fetchone()
alpha = avg_fare_amount - beta * avg_trip_distance
print(alpha)

cursor.close()
conn.close()
